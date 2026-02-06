using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class AssetReferenceFinder : EditorWindow
{
    private Object targetAsset;
    private Vector2 scroll;
    private List<string> matches = new List<string>();
    private bool autoClear = true;

    [MenuItem("Tools/Find Asset References")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(AssetReferenceFinder), false, "Asset Reference Finder");
    }

    private void OnGUI()
    {
        GUILayout.Label("Find All References to an Asset", EditorStyles.boldLabel);
        targetAsset = EditorGUILayout.ObjectField("Target Asset", targetAsset, typeof(Object), false);
        autoClear = EditorGUILayout.Toggle("Auto Clear Matches", autoClear);

        GUILayout.Space(6);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Scan Project")) ScanProject();
        if (GUILayout.Button("Scan Scenes")) ScanScenes();
        if (GUILayout.Button("Scan Text/Script Files")) ScanTextFiles();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Scan Selected Assets")) ScanSelectedAssets();
        if (GUILayout.Button("Clear Matches")) ClearMatches();
        GUILayout.EndHorizontal();

        GUILayout.Space(6);

        GUILayout.Label("Matches Found: " + matches.Count, EditorStyles.boldLabel);

        if (matches.Count > 0)
        {
            scroll = GUILayout.BeginScrollView(scroll, GUILayout.Height(300));
            for (int i = 0; i < matches.Count; i++)
            {
                string path = matches[i];
                if (GUILayout.Button(path, EditorStyles.label))
                {
                    Object obj = AssetDatabase.LoadAssetAtPath(path == null ? "" : path, typeof(Object));
                    if (obj != null)
                        Selection.activeObject = obj;
                    else
                        EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath(path));
                }
            }
            GUILayout.EndScrollView();
        }
    }

    private void ClearMatches()
    {
        matches.Clear();
        Debug.Log("AssetReferenceFinder: matches cleared.");
    }

    // older-style out-compatible prepare
    private bool PrepareTarget(out string assetPath, out string assetName, out string assetGUID)
    {
        assetPath = "";
        assetName = "";
        assetGUID = "";

        if (targetAsset == null)
        {
            Debug.LogWarning("AssetReferenceFinder: No target asset selected.");
            return false;
        }

        assetPath = AssetDatabase.GetAssetPath(targetAsset);
        if (string.IsNullOrEmpty(assetPath))
        {
            Debug.LogWarning("AssetReferenceFinder: Target asset has no valid project path.");
            return false;
        }

        int lastSlash = assetPath.LastIndexOf('/');
        if (lastSlash >= 0)
            assetName = assetPath.Substring(lastSlash + 1);
        else
            assetName = assetPath;

        int dot = assetName.LastIndexOf('.');
        if (dot >= 0) assetName = assetName.Substring(0, dot);

        assetGUID = AssetDatabase.AssetPathToGUID(assetPath);
        return true;
    }

    private void AddMatchIfRelevant(string filePath, string assetName, string assetGUID, string assetPath)
    {
        string content;
        try { content = File.ReadAllText(filePath); }
        catch { return; }

        if (content.Contains(assetName) || content.Contains(assetGUID) || content.Contains(assetPath))
        {
            string relativePath = "Assets" + filePath.Substring(Application.dataPath.Length).Replace("\\", "/");
            if (!matches.Contains(relativePath))
                matches.Add(relativePath);
        }
    }

    private void ScanProject()
    {
        string assetPath;
        string assetName;
        string assetGUID;
        if (!PrepareTarget(out assetPath, out assetName, out assetGUID)) return;
        if (autoClear) ClearMatches();

        string[] allFiles = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories);
        for (int i = 0; i < allFiles.Length; i++)
        {
            string file = allFiles[i];
            int dotIndex = file.LastIndexOf('.');
            string ext = (dotIndex >= 0) ? file.Substring(dotIndex).ToLower() : "";
            if (ext == ".meta" || ext == ".dll" || ext == ".exe") continue;

            // scan text-like and Unity asset text files
            if (ext == ".cs" || ext == ".txt" || ext == ".xml" || ext == ".shader" || ext == ".json" ||
                ext == ".cginc" || ext == ".prefab" || ext == ".asset" || ext == ".mat" || ext == ".controller" ||
                ext == ".anim" || ext == ".unity" || ext == ".bytes" || ext == ".asmdef")
            {
                AddMatchIfRelevant(file, assetName, assetGUID, assetPath);
            }
        }

        Debug.Log("AssetReferenceFinder: Scan Project complete. Found " + matches.Count + " references.");
    }

    private void ScanScenes()
    {
        string assetPath;
        string assetName;
        string assetGUID;
        if (!PrepareTarget(out assetPath, out assetName, out assetGUID)) return;
        if (autoClear) ClearMatches();

        string[] sceneFiles = Directory.GetFiles(Application.dataPath, "*.unity", SearchOption.AllDirectories);
        for (int i = 0; i < sceneFiles.Length; i++)
        {
            AddMatchIfRelevant(sceneFiles[i], assetName, assetGUID, assetPath);
        }

        Debug.Log("AssetReferenceFinder: Scan Scenes complete. Found " + matches.Count + " references.");
    }

    private void ScanTextFiles()
    {
        string assetPath;
        string assetName;
        string assetGUID;
        if (!PrepareTarget(out assetPath, out assetName, out assetGUID)) return;
        if (autoClear) ClearMatches();

        string[] allFiles = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories);
        for (int i = 0; i < allFiles.Length; i++)
        {
            string file = allFiles[i];
            int dotIndex = file.LastIndexOf('.');
            string ext = (dotIndex >= 0) ? file.Substring(dotIndex).ToLower() : "";
            if (ext == ".meta" || ext == ".dll" || ext == ".exe") continue;

            if (ext == ".cs" || ext == ".txt" || ext == ".xml" || ext == ".shader" || ext == ".json" || ext == ".cginc")
            {
                AddMatchIfRelevant(file, assetName, assetGUID, assetPath);
            }
        }

        Debug.Log("AssetReferenceFinder: Scan Text/Script Files complete. Found " + matches.Count + " references.");
    }

    private void ScanSelectedAssets()
    {
        Object[] selectedAssets = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
        if (selectedAssets == null || selectedAssets.Length == 0)
        {
            Debug.LogWarning("AssetReferenceFinder: No assets selected in Project window.");
            return;
        }

        if (autoClear) ClearMatches();

        for (int s = 0; s < selectedAssets.Length; s++)
        {
            Object sel = selectedAssets[s];
            string selPath = AssetDatabase.GetAssetPath(sel);
            if (string.IsNullOrEmpty(selPath)) continue;

            string selName;
            int lastSlash = selPath.LastIndexOf('/');
            if (lastSlash >= 0)
                selName = selPath.Substring(lastSlash + 1);
            else
                selName = selPath;

            int dot = selName.LastIndexOf('.');
            if (dot >= 0) selName = selName.Substring(0, dot);
            string selGUID = AssetDatabase.AssetPathToGUID(selPath);

            // scan project for references to this selected asset
            string[] allFiles = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories);
            for (int i = 0; i < allFiles.Length; i++)
            {
                string file = allFiles[i];
                int dotIndex = file.LastIndexOf('.');
                string ext = (dotIndex >= 0) ? file.Substring(dotIndex).ToLower() : "";
                if (ext == ".meta" || ext == ".dll" || ext == ".exe") continue;

                if (ext == ".cs" || ext == ".txt" || ext == ".xml" || ext == ".shader" || ext == ".json" ||
                    ext == ".cginc" || ext == ".prefab" || ext == ".asset" || ext == ".mat" || ext == ".controller" ||
                    ext == ".anim" || ext == ".unity" || ext == ".bytes" || ext == ".asmdef")
                {
                    try
                    {
                        string content = File.ReadAllText(file);
                        if (content.Contains(selName) || content.Contains(selGUID) || content.Contains(selPath))
                        {
                            string relativePath = "Assets" + file.Substring(Application.dataPath.Length).Replace("\\", "/");
                            if (!matches.Contains(relativePath))
                                matches.Add(relativePath);
                        }
                    }
                    catch { /* ignore read errors */ }
                }
            }
        }

        Debug.Log("AssetReferenceFinder: Scan Selected Assets complete. Found " + matches.Count + " references.");
    }
}
