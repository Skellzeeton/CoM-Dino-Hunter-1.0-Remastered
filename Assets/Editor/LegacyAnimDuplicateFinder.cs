using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Scans a folder for .anim files and groups together clips whose keyframes are
/// identical (or within a tolerance). Useful for cleaning up legacy AnimationClips.
/// </summary>
public class LegacyAnimDuplicateFinder : EditorWindow
{
    [MenuItem("Tools/Animation/Legacy Duplicate Clip Finder")]
    private static void Open()
    {
        var w = GetWindow<LegacyAnimDuplicateFinder>("Anim Duplicates");
        w.minSize = new Vector2(480, 340);
    }

    // ---------- Settings ----------

    private DefaultAsset _folder;
    private float _tolerance = 0.0001f;
    private bool _compareTangents = false;
    private bool _compareClipSettings = true;
    private bool _ignoreEmptyClips = true;

    // ---------- State ----------

    private class ClipData
    {
        public AnimationClip clip;
        public string path;
        public string structuralKey;
        public float length;
        public EditorCurveBinding[] floatBindings;
        public AnimationCurve[] floatCurves;
        public EditorCurveBinding[] objBindings;
        public ObjectReferenceKeyframe[][] objCurves;
    }

    private readonly List<List<AnimationClip>> _groups = new List<List<AnimationClip>>();
    private int _scannedCount;
    private bool _hasScanned;
    private Vector2 _scroll;

    private static readonly Comparison<EditorCurveBinding> BindingComparison =
        (a, b) => string.CompareOrdinal(BindingKey(a), BindingKey(b));

    // ---------- GUI ----------

    private void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Scan Settings", EditorStyles.boldLabel);

        _folder = (DefaultAsset)EditorGUILayout.ObjectField(
            new GUIContent("Folder", "Drag a project folder here. Sub-folders are included."),
            _folder, typeof(DefaultAsset), false);

        _tolerance = Mathf.Max(0f, EditorGUILayout.FloatField(
            new GUIContent("Tolerance", "Maximum allowed difference for keyframe times and values (and tangents when enabled). 0 = exact."),
            _tolerance));

        _compareTangents = EditorGUILayout.Toggle(
            new GUIContent("Compare Tangents", "Also compare in/out tangents and weights. Turn this off if clips only differ by tangent recalculation."),
            _compareTangents);

        _compareClipSettings = EditorGUILayout.Toggle(
            new GUIContent("Compare Clip Settings", "Require matching loop flag and frame rate."),
            _compareClipSettings);

        _ignoreEmptyClips = EditorGUILayout.Toggle(
            new GUIContent("Ignore Empty Clips", "Skip clips that have no curves at all."),
            _ignoreEmptyClips);

        EditorGUILayout.Space();

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Scan", GUILayout.Height(24))) Scan();

            using (new EditorGUI.DisabledScope(!_hasScanned || _groups.Count == 0))
            {
                if (GUILayout.Button("Select All Duplicates", GUILayout.Height(24), GUILayout.Width(170)))
                    SelectAllDuplicates();
            }
        }

        if (_hasScanned)
        {
            EditorGUILayout.HelpBox(
                $"Scanned {_scannedCount} .anim file(s). Found {_groups.Count} duplicate group(s).",
                _groups.Count > 0 ? MessageType.Warning : MessageType.Info);
        }

        _scroll = EditorGUILayout.BeginScrollView(_scroll);
        for (int i = 0; i < _groups.Count; i++)
            DrawGroup(i, _groups[i]);
        EditorGUILayout.EndScrollView();
    }

    private void DrawGroup(int index, List<AnimationClip> group)
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField($"Group {index + 1}  ({group.Count} clips)", EditorStyles.boldLabel);

            if (GUILayout.Button("Select", GUILayout.Width(60)))
            {
                Selection.objects = group.ToArray();
                EditorGUIUtility.PingObject(group[0]);
            }

            if (GUILayout.Button("Keep First / Delete Rest", GUILayout.Width(180)))
                DeleteDuplicates(group);
        }

        foreach (var clip in group)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.ObjectField(clip, typeof(AnimationClip), false, GUILayout.Width(200));
                EditorGUILayout.SelectableLabel(
                    AssetDatabase.GetAssetPath(clip),
                    EditorStyles.miniLabel,
                    GUILayout.ExpandWidth(true),
                    GUILayout.Height(18));
            }
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(2);
    }

    // ---------- Actions ----------

    private void SelectAllDuplicates()
    {
        var all = new List<UnityEngine.Object>();
        foreach (var g in _groups)
            foreach (var c in g)
                all.Add(c);

        Selection.objects = all.ToArray();
        if (all.Count > 0) EditorGUIUtility.PingObject(all[0]);
    }

    private void DeleteDuplicates(List<AnimationClip> group)
    {
        var toDelete = group.GetRange(1, group.Count - 1);

        var sb = new StringBuilder();
        sb.AppendLine($"Delete {toDelete.Count} duplicate(s) and keep:");
        sb.AppendLine("  " + AssetDatabase.GetAssetPath(group[0]));
        sb.AppendLine();
        foreach (var c in toDelete)
            sb.AppendLine("  • " + AssetDatabase.GetAssetPath(c));

        if (!EditorUtility.DisplayDialog("Delete Duplicates", sb.ToString(), "Delete", "Cancel"))
            return;

        AssetDatabase.StartAssetEditing();
        try
        {
            foreach (var c in toDelete)
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(c));
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
        }

        AssetDatabase.Refresh();
        Scan();
    }

    // ---------- Scanning ----------

    private void Scan()
    {
        _groups.Clear();
        _hasScanned = false;

        if (_folder == null)
        {
            EditorUtility.DisplayDialog("No Folder", "Drag a project folder into the Folder field first.", "OK");
            return;
        }

        string folderPath = AssetDatabase.GetAssetPath(_folder);
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            EditorUtility.DisplayDialog("Invalid Folder", $"'{folderPath}' is not a valid project folder.", "OK");
            return;
        }

        string[] guids = AssetDatabase.FindAssets("t:AnimationClip", new[] { folderPath });

        var datas = new List<ClipData>();
        try
        {
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                if (!path.EndsWith(".anim", StringComparison.OrdinalIgnoreCase)) continue;

                EditorUtility.DisplayProgressBar("Scanning Animations", path, (float)i / Mathf.Max(1, guids.Length));

                var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
                if (clip == null) continue;

                var data = BuildClipData(clip, path);

                if (_ignoreEmptyClips && data.floatBindings.Length == 0 && data.objBindings.Length == 0)
                    continue;

                datas.Add(data);
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }

        // Deterministic ordering
        datas.Sort((a, b) => string.CompareOrdinal(a.path, b.path));
        _scannedCount = datas.Count;

        // Stage 1: bucket by structure (bindings + key counts). Cheap, prunes almost everything.
        var buckets = new Dictionary<string, List<ClipData>>();
        foreach (var d in datas)
        {
            if (!buckets.TryGetValue(d.structuralKey, out var list))
            {
                list = new List<ClipData>();
                buckets[d.structuralKey] = list;
            }
            list.Add(d);
        }

        // Stage 2: within each bucket, compare keyframe data with tolerance.
        foreach (var bucket in buckets.Values)
        {
            if (bucket.Count < 2) continue;

            var used = new bool[bucket.Count];

            for (int i = 0; i < bucket.Count; i++)
            {
                if (used[i]) continue;

                List<AnimationClip> group = null;

                for (int j = i + 1; j < bucket.Count; j++)
                {
                    if (used[j]) continue;
                    if (!ClipsMatch(bucket[i], bucket[j])) continue;

                    if (group == null)
                    {
                        group = new List<AnimationClip> { bucket[i].clip };
                        used[i] = true;
                    }

                    group.Add(bucket[j].clip);
                    used[j] = true;
                }

                if (group != null) _groups.Add(group);
            }
        }

        _groups.Sort((x, y) => string.CompareOrdinal(
            AssetDatabase.GetAssetPath(x[0]),
            AssetDatabase.GetAssetPath(y[0])));

        _hasScanned = true;
        Repaint();
    }

    // ---------- Data extraction ----------

    private ClipData BuildClipData(AnimationClip clip, string path)
    {
        var data = new ClipData
        {
            clip = clip,
            path = path,
            length = clip.length
        };

        var floatBindings = AnimationUtility.GetCurveBindings(clip);
        Array.Sort(floatBindings, BindingComparison);
        data.floatBindings = floatBindings;
        data.floatCurves = new AnimationCurve[floatBindings.Length];
        for (int i = 0; i < floatBindings.Length; i++)
            data.floatCurves[i] = AnimationUtility.GetEditorCurve(clip, floatBindings[i]);

        var objBindings = AnimationUtility.GetObjectReferenceCurveBindings(clip);
        Array.Sort(objBindings, BindingComparison);
        data.objBindings = objBindings;
        data.objCurves = new ObjectReferenceKeyframe[objBindings.Length][];
        for (int i = 0; i < objBindings.Length; i++)
            data.objCurves[i] = AnimationUtility.GetObjectReferenceCurve(clip, objBindings[i]);

        data.structuralKey = BuildStructuralKey(clip, data);
        return data;
    }

    private string BuildStructuralKey(AnimationClip clip, ClipData data)
    {
        var parts = new List<string>(data.floatBindings.Length + data.objBindings.Length + 2);

        for (int i = 0; i < data.floatBindings.Length; i++)
        {
            var c = data.floatCurves[i];
            parts.Add("F " + BindingKey(data.floatBindings[i]) + " #" + (c != null ? c.length : -1));
        }

        for (int i = 0; i < data.objBindings.Length; i++)
        {
            var k = data.objCurves[i];
            parts.Add("O " + BindingKey(data.objBindings[i]) + " #" + (k != null ? k.Length : -1));
        }

        parts.Sort(StringComparer.Ordinal);

        var sb = new StringBuilder(parts.Count * 48);
        foreach (var p in parts)
        {
            sb.Append(p);
            sb.Append('\n');
        }

        if (_compareClipSettings)
        {
            sb.Append("loop:").Append(clip.isLooping).Append('\n');
            sb.Append("fps:").Append(clip.frameRate.ToString("R")).Append('\n');
        }

        return sb.ToString();
    }

    private static string BindingKey(EditorCurveBinding b)
    {
        return (b.type != null ? b.type.FullName : "?") + "|" + b.path + "|" + b.propertyName;
    }

    // ---------- Comparison ----------

    private bool ClipsMatch(ClipData a, ClipData b)
    {
        float tol = _tolerance;

        if (!Approx(a.length, b.length, tol)) return false;

        if (a.floatBindings.Length != b.floatBindings.Length) return false;
        for (int i = 0; i < a.floatBindings.Length; i++)
        {
            if (BindingKey(a.floatBindings[i]) != BindingKey(b.floatBindings[i])) return false;
            if (!CurvesMatch(a.floatCurves[i], b.floatCurves[i], tol)) return false;
        }

        if (a.objBindings.Length != b.objBindings.Length) return false;
        for (int i = 0; i < a.objBindings.Length; i++)
        {
            if (BindingKey(a.objBindings[i]) != BindingKey(b.objBindings[i])) return false;

            var ka = a.objCurves[i];
            var kb = b.objCurves[i];
            if ((ka == null) != (kb == null)) return false;
            if (ka == null) continue;
            if (ka.Length != kb.Length) return false;

            for (int k = 0; k < ka.Length; k++)
            {
                if (!Approx(ka[k].time, kb[k].time, tol)) return false;
                if (ka[k].value != kb[k].value) return false;
            }
        }

        return true;
    }

    private bool CurvesMatch(AnimationCurve ca, AnimationCurve cb, float tol)
    {
        if (ca == null || cb == null) return ca == cb;
        if (ca.length != cb.length) return false;

        for (int i = 0; i < ca.length; i++)
        {
            Keyframe k1 = ca[i];
            Keyframe k2 = cb[i];

            if (!Approx(k1.time, k2.time, tol)) return false;
            if (!Approx(k1.value, k2.value, tol)) return false;

            if (_compareTangents)
            {
                if (!Approx(k1.inTangent, k2.inTangent, tol)) return false;
                if (!Approx(k1.outTangent, k2.outTangent, tol)) return false;
                if (!Approx(k1.inWeight, k2.inWeight, tol)) return false;
                if (!Approx(k1.outWeight, k2.outWeight, tol)) return false;
            }
        }

        return true;
    }

    /// <summary>NaN/Infinity-safe approximate float comparison.</summary>
    private static bool Approx(float x, float y, float tol)
    {
        if (float.IsNaN(x) || float.IsNaN(y)) return float.IsNaN(x) && float.IsNaN(y);
        if (float.IsInfinity(x) || float.IsInfinity(y)) return x == y; // inf == inf, -inf == -inf
        return Mathf.Abs(x - y) <= tol;
    }
}