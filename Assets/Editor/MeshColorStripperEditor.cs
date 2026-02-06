using UnityEngine;
using UnityEditor;
using System.IO;

public class MeshColorStripperEditor : EditorWindow
{
    private Mesh sourceMesh;
    private bool overwriteOriginal = false;

    [MenuItem("Tools/Mesh Color Stripper")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(MeshColorStripperEditor), false, "Mesh Color Stripper");
    }

    private void OnGUI()
    {
        GUILayout.Label("Strip Vertex Colors from Mesh", EditorStyles.boldLabel);
        sourceMesh = (Mesh)EditorGUILayout.ObjectField("Source Mesh", sourceMesh, typeof(Mesh), false);
        overwriteOriginal = EditorGUILayout.Toggle("Overwrite Original", overwriteOriginal);

        if (GUILayout.Button("Strip Colors"))
        {
            Object[] targets = Selection.GetFiltered(typeof(Mesh), SelectionMode.Assets);
            if (sourceMesh != null)
            {
                ProcessMesh(sourceMesh);
            }
            foreach (Object obj in targets)
            {
                Mesh mesh = obj as Mesh;
                if (mesh != null && mesh != sourceMesh)
                {
                    ProcessMesh(mesh);
                }
            }
        }
    }

    private void ProcessMesh(Mesh mesh)
    {
        string assetPath = AssetDatabase.GetAssetPath(mesh);
        if (string.IsNullOrEmpty(assetPath)) return;

        Mesh newMesh = Object.Instantiate(mesh);
        newMesh.name = mesh.name + "_NoColors";
        if (newMesh.colors32 != null && newMesh.colors32.Length > 0)
        {
            newMesh.colors32 = new Color32[0];
        }

        if (overwriteOriginal)
        {
            string tempPath = assetPath.Replace(".asset", "_temp.asset");
            AssetDatabase.CreateAsset(newMesh, tempPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            AssetDatabase.DeleteAsset(assetPath);
            File.Move(tempPath, assetPath);
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            Debug.Log("Mesh overwritten: " + assetPath);
        }
        else
        {
            int lastSlash = assetPath.LastIndexOf('/');
            string folder = (lastSlash >= 0) ? assetPath.Substring(0, lastSlash) : "Assets";
            string newPath = folder + "/" + newMesh.name + ".asset";
            AssetDatabase.CreateAsset(newMesh, newPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Mesh saved to: " + newPath);
        }
    }
}
