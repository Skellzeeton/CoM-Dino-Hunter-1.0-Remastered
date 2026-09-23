using System.Text;
using UnityEditor;
using UnityEngine;

namespace AnimOptimizer
{
    public class AnimationClipOptimizerWindow : EditorWindow
    {
        const string DefaultFolder = "Assets/AnimationClip";

        readonly OptimizerSettings _settings = new OptimizerSettings();
        string _folder = DefaultFolder;
        bool _wholeProject;
        BatchResult _lastScan;
        string[] _lastScanPaths;
        OptimizerSettings _lastScanSettings;
        Vector2 _scroll;

        [MenuItem("Tools/Animation Clip Optimizer", priority = 100)]
        public static void Open()
        {
            GetWindow<AnimationClipOptimizerWindow>("Anim Optimizer").minSize = new Vector2(520, 420);
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("Scope", EditorStyles.boldLabel);
            _wholeProject = EditorGUILayout.Toggle("Whole project", _wholeProject);
            using (new EditorGUI.DisabledScope(_wholeProject)) _folder = EditorGUILayout.TextField("Folder", _folder);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Rules", EditorStyles.boldLabel);
            _settings.DeleteIdentityScale = EditorGUILayout.Toggle("Delete identity scale", _settings.DeleteIdentityScale);
            _settings.CollapseConstant = EditorGUILayout.Toggle("Collapse constant curves", _settings.CollapseConstant);
            _settings.ZeroTinyTangents = EditorGUILayout.Toggle("Zero tiny tangents", _settings.ZeroTinyTangents);
            _settings.DecimateCurves = EditorGUILayout.Toggle("Decimate animated curves", _settings.DecimateCurves);
            _settings.Epsilon = EditorGUILayout.FloatField("Epsilon", _settings.Epsilon);
            _settings.RoundValues = EditorGUILayout.Toggle("Round values", _settings.RoundValues);

            using (new EditorGUI.DisabledScope(!_settings.DecimateCurves)) _settings.DecimateEpsilon = EditorGUILayout.FloatField("Decimate epsilon", _settings.DecimateEpsilon);

            using (new EditorGUI.DisabledScope(!_settings.RoundValues)) _settings.ValueDecimals = EditorGUILayout.IntSlider("Decimal places", _settings.ValueDecimals, 4, 9);

            EditorGUILayout.Space();
            if (GUILayout.Button("Scan Animations", GUILayout.Height(28)))
            {
                _lastScanPaths = ResolvePaths();
                _lastScanSettings = _settings.Clone();
                _lastScan = ClipBatchRunner.Run(_lastScanPaths, _lastScanSettings, apply: false);
            }

            using (new EditorGUI.DisabledScope(_lastScan == null))
            {
                if (GUILayout.Button("Apply", GUILayout.Height(28)))
                {
                    Apply();
                }
            }

            if (_lastScan != null) DrawResults();
        }

        string[] ResolvePaths()
        {
            return ClipBatchRunner.FindClips(_wholeProject ? null : new[] { _folder });
        }

        void Apply()
        {
            int writable = 0;
            foreach (var r in _lastScan.Reports)
            {
                if (r.Changed && r.SkipReason == null) writable++;
            }
            bool ok = EditorUtility.DisplayDialog("Optimize Animation Clips", string.Format("This rewrites {0} clip(s) in place, removing {1:N0} keyframes.\n\n" + "Originals are only recoverable through git. Continue?", writable, _lastScan.KeysSaved), "Optimize", "Cancel");

            if (!ok) return;

            _lastScan = ClipBatchRunner.Run(_lastScanPaths, _lastScanSettings, apply: true);
            Debug.Log(Summarize(_lastScan));
        }

        void DrawResults()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Results", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(Summarize(_lastScan), MessageType.Info);

            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            foreach (var report in _lastScan.Reports)
            {
                if (!report.Changed && report.SkipReason == null) continue;
                EditorGUILayout.LabelField(report.ToString(), EditorStyles.miniLabel);
            }
            EditorGUILayout.EndScrollView();
        }

        static string Summarize(BatchResult result)
        {
            int flagged = 0;
            foreach (var r in result.Reports)
            {
                if (r.SkipReason != null) flagged++;
            }
            var sb = new StringBuilder();
            sb.AppendFormat("{0} clip(s) processed\n", result.Reports.Count);
            sb.AppendFormat("Curves: {0:N0} -> {1:N0}\n", result.CurvesBefore, result.CurvesAfter);
            sb.AppendFormat("Keys:   {0:N0} -> {1:N0}  ({2:N0} removed, {3:P1})\n", result.KeysBefore, result.KeysAfter, result.KeysSaved, result.KeysBefore == 0 ? 0f : result.KeysSaved / (float)result.KeysBefore);
            sb.AppendFormat("Size: {0:N1} MB -> {1:N1} MB ({2:N1} MB saved)\n", result.BytesBefore / 1024f / 1024f, result.BytesAfter / 1024f / 1024f, result.BytesSaved / 1024f / 1024f);
            if (flagged > 0) sb.AppendFormat("FLAGGED (not written): {0}\n", flagged);
            if (result.Skipped > 0) sb.AppendFormat("Unreadable: {0}\n", result.Skipped);
            if (result.Cancelled) sb.Append("CANCELLED before finishing\n");
            return sb.ToString();
        }
    }
}
