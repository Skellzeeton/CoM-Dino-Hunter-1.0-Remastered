using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class EmptyUnityMethodFinder : EditorWindow
{
    // Unity callback methods that are NOT stripped from builds.
    // Add any custom magic methods you use here.
    private static readonly HashSet<string> MagicMethodNames = new HashSet<string>
    {
        // Core lifecycle
        "Awake", "Start", "Update", "FixedUpdate", "LateUpdate",
        "OnEnable", "OnDisable", "OnDestroy",
        "OnGUI", "OnApplicationQuit", "OnApplicationPause",
        "OnApplicationFocus", "OnBecameVisible", "OnBecameInvisible",
        "OnPreCull", "OnPreRender", "OnPostRender", "OnRenderImage",
        "OnRenderObject", "OnWillRenderObject",
        "OnDrawGizmos", "OnDrawGizmosSelected",
        "OnValidate", "Reset",
        "OnCollisionEnter", "OnCollisionStay", "OnCollisionExit",
        "OnTriggerEnter", "OnTriggerStay", "OnTriggerExit",
        "OnControllerColliderHit", "OnJointBreak",
        "OnParticleCollision", "OnParticleTrigger",
        "OnMouseDown", "OnMouseUp", "OnMouseDrag", "OnMouseEnter",
        "OnMouseExit", "OnMouseOver", "OnMouseUpAsButton",
        "OnAudioFilterRead", "OnLevelWasLoaded",
        // Network / legacy
        "OnConnectedToServer", "OnDisconnectedFromServer",
        "OnFailedToConnect", "OnFailedToConnectToMasterServer",
        "OnMasterServerEvent", "OnNetworkInstantiate",
        "OnPlayerConnected", "OnPlayerDisconnected",
        "OnSerializeNetworkView", "OnServerInitialized"
    };

    private Vector2 _scrollPos;
    private List<EmptyMethodHit> _results = new List<EmptyMethodHit>();
    private bool _hasScanned;

    [MenuItem("Tools/Empty Unity Method Finder")]
    public static void ShowWindow()
    {
        GetWindow<EmptyUnityMethodFinder>("Empty Method Finder");
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(5);

        if (GUILayout.Button("Scan Project for Empty Magic Methods", GUILayout.Height(28)))
        {
            ScanProject();
        }

        // Selection / deletion toolbar
        if (_hasScanned && _results.Count > 0)
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Select All"))
                foreach (var r in _results) r.Selected = true;

            if (GUILayout.Button("Select None"))
                foreach (var r in _results) r.Selected = false;

            int selectedCount = _results.Count(r => r.Selected);
            GUI.enabled = selectedCount > 0;

            var prevColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(1f, 0.55f, 0.55f);
            if (GUILayout.Button($"Delete Selected ({selectedCount})", GUILayout.Height(22)))
                DeleteSelected();
            GUI.backgroundColor = prevColor;

            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField($"Found: {_results.Count} empty magic method(s)", EditorStyles.boldLabel);

        if (!_hasScanned)
        {
            EditorGUILayout.HelpBox(
                    "Click the button above to scan all .cs files in the project.\n\n" +
                    "This tool looks for Unity callback methods (Update, Start, FixedUpdate, etc.) " +
                    "that have completely empty bodies. These methods are NOT stripped from builds " +
                    "and incur per-frame overhead even when doing nothing.\n\n" +
                    "Tip: commit or back up your project before using Delete Selected.",
                    MessageType.Info);
            return;
        }

        if (_results.Count == 0)
        {
            EditorGUILayout.HelpBox("No empty magic methods found. Your project is clean!", MessageType.Info);
            return;
        }

        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

        foreach (var hit in _results)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

            hit.Selected = EditorGUILayout.Toggle(hit.Selected, GUILayout.Width(20));

            if (GUILayout.Button($"{hit.MethodName}  —  {hit.RelativePath}:{hit.LineNumber}", EditorStyles.linkLabel))
            {
                AssetDatabase.OpenAsset(hit.Asset, hit.LineNumber);
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }

    private void ScanProject()
    {
        _results.Clear();
        _hasScanned = true;

        string projectPath = Application.dataPath;

        string[] files = Directory.GetFiles(projectPath, "*.cs", SearchOption.AllDirectories);

        // Matches: optional access modifier, optional static, "void", method name,
        // empty parens, and a body containing only whitespace / optional semicolon.
        var methodRegex = new Regex(
                @"(?<access>public|private|protected|internal)?\s*" +
                @"(?<static>static)?\s*" +
                @"void\s+(?<name>\w+)\s*\(\s*\)\s*" +
                @"\{\s*(\s*;?\s*)?\}",
                RegexOptions.Compiled | RegexOptions.Multiline);

        foreach (string file in files)
        {
            string content;
            try
            {
                content = File.ReadAllText(file);
            }
            catch
            {
                continue;
            }

            string relativePath = file.Replace(Application.dataPath, "Assets").Replace('\\', '/');
            var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(relativePath);
            if (asset == null) continue;

            foreach (Match match in methodRegex.Matches(content))
            {
                string methodName = match.Groups["name"].Value;
                if (!MagicMethodNames.Contains(methodName))
                    continue;

                int lineNumber = content.Substring(0, match.Index).Count(c => c == '\n') + 1;

                int deleteStart, deleteEnd;
                ComputeDeleteRange(content, match, out deleteStart, out deleteEnd);

                _results.Add(new EmptyMethodHit
                {
                    MethodName = methodName,
                    RelativePath = relativePath,
                    FullPath = file,
                    LineNumber = lineNumber,
                    Asset = asset,
                    MatchIndex = match.Index,
                    MatchLength = match.Length,
                    DeleteStart = deleteStart,
                    DeleteEnd = deleteEnd,
                    Selected = false
                });
            }
        }

        _results = _results
        .OrderBy(r => r.RelativePath)
        .ThenBy(r => r.LineNumber)
        .ToList();

        Repaint();
    }

    /// <summary>
    /// Expands a method match into a deletion range that also removes the leading
    /// indentation, any attribute lines directly above (e.g. [ContextMenu(...)]),
    /// and the trailing newline, so no blank lines are left behind.
    /// </summary>
    private static void ComputeDeleteRange(string content, Match match, out int deleteStart, out int deleteEnd)
    {
        int matchStart = match.Index;
        int matchEnd = match.Index + match.Length;

        // Start of the line containing the method declaration.
        int lineStart = content.LastIndexOf('\n', Math.Max(0, matchStart - 1)) + 1;

        // Walk backwards over attribute lines that begin with '['.
        int scanPos = lineStart;
        while (scanPos > 0)
        {
            int prevLineEnd = scanPos - 1;
            if (prevLineEnd > 0 && content[prevLineEnd - 1] == '\r') prevLineEnd--;

            int prevLineStart = content.LastIndexOf('\n', Math.Max(0, prevLineEnd - 1)) + 1;
            string prevLine = content.Substring(prevLineStart, prevLineEnd - prevLineStart).Trim();

            if (prevLine.StartsWith("["))
            {
                lineStart = prevLineStart;
                scanPos = prevLineStart;
            }
            else
            {
                break;
            }
        }

        deleteStart = lineStart;

        // Skip trailing spaces/tabs/CR on the method's last line, then eat one newline.
        int end = matchEnd;
        while (end < content.Length &&
        (content[end] == ' ' || content[end] == '\t' || content[end] == '\r'))
        {
            end++;
        }
        if (end < content.Length && content[end] == '\n') end++;

        deleteEnd = end;
    }

    private void DeleteSelected()
    {
        var toDelete = _results.Where(r => r.Selected).ToList();
        if (toDelete.Count == 0)
        {
            EditorUtility.DisplayDialog("Nothing Selected",
                    "No methods are selected for deletion.", "OK");
            return;
        }

        bool confirm = EditorUtility.DisplayDialog(
                "Delete Empty Methods",
                $"Delete {toDelete.Count} empty method(s) from {toDelete.Select(r => r.RelativePath).Distinct().Count()} file(s)?\n\n" +
                "This modifies your source files. Make sure your project is under version control " +
                "or backed up first.",
                "Delete", "Cancel");

        if (!confirm) return;

        int deleted = 0;
        int filesChanged = 0;

        foreach (var group in toDelete.GroupBy(r => r.FullPath))
        {
            string content;
            try
            {
                content = File.ReadAllText(group.Key);
            }
            catch (Exception e)
            {
                Debug.LogError($"[EmptyUnityMethodFinder] Failed to read {group.Key}: {e.Message}");
                continue;
            }

            // Remove from the bottom of the file upward so earlier indices stay valid.
            var ordered = group.OrderByDescending(r => r.DeleteStart).ToList();

            bool changedThisFile = false;
            foreach (var hit in ordered)
            {
                if (hit.DeleteStart < 0 ||
                hit.DeleteEnd > content.Length ||
                hit.DeleteStart >= hit.DeleteEnd)
                {
                    continue;
                }

                content = content.Remove(hit.DeleteStart, hit.DeleteEnd - hit.DeleteStart);
                deleted++;
                changedThisFile = true;
            }

            if (changedThisFile)
            {
                try
                {
                    File.WriteAllText(group.Key, content);
                    filesChanged++;
                }
                catch (Exception e)
                {
                    Debug.LogError($"[EmptyUnityMethodFinder] Failed to write {group.Key}: {e.Message}");
                }
            }
        }

        AssetDatabase.Refresh();
        Debug.Log($"[EmptyUnityMethodFinder] Deleted {deleted} empty method(s) across {filesChanged} file(s).");

        // Re-scan so the list reflects reality.
        ScanProject();
    }

    private class EmptyMethodHit
    {
        public string MethodName;
        public string RelativePath;
        public string FullPath;
        public int LineNumber;
        public UnityEngine.Object Asset;

        public int MatchIndex;   // index of "void MethodName() {...}" in file content
        public int MatchLength;  // length of the match
        public int DeleteStart;  // expanded range start (includes attributes + indentation)
        public int DeleteEnd;    // expanded range end (includes trailing newline)

        public bool Selected;
    }
}