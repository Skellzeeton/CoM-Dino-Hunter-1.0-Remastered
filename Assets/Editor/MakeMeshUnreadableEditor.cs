using UnityEngine;
using UnityEditor;
using System.IO;

public class MakeMeshUnreadableEditor : EditorWindow
{
    private Mesh sourceMesh;
    private bool overwriteOriginal = false;

    [MenuItem("Tools/Make Mesh Unreadable")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(MakeMeshUnreadableEditor), false, "Make Mesh Unreadable");
    }

    private void OnGUI()
    {
        GUILayout.Label("Strip Readable Data from Mesh", EditorStyles.boldLabel);
        sourceMesh = (Mesh)EditorGUILayout.ObjectField("Source Mesh", sourceMesh, typeof(Mesh), false);
        overwriteOriginal = EditorGUILayout.Toggle("Overwrite Original", overwriteOriginal);

        if (GUILayout.Button("Make Unreadable"))
        {
            Object[] selectedMeshes = Selection.GetFiltered(typeof(Mesh), SelectionMode.Assets);
            if (sourceMesh != null)
            {
                ProcessMesh(sourceMesh);
            }

            for (int i = 0; i < selectedMeshes.Length; i++)
            {
                Mesh selectedMesh = selectedMeshes[i] as Mesh;
                if (selectedMesh != null && selectedMesh != sourceMesh)
                {
                    ProcessMesh(selectedMesh);
                }
            }
        }
    }

    private void ProcessMesh(Mesh inputMesh)
    {
        string assetPath = AssetDatabase.GetAssetPath(inputMesh);
        if (string.IsNullOrEmpty(assetPath)) return;

        Mesh unreadableMesh = Object.Instantiate(inputMesh);
        unreadableMesh.name = inputMesh.name + "_Unreadable";
        unreadableMesh.UploadMeshData(true); // Discards CPU copy

        if (overwriteOriginal)
        {
            string tempPath = assetPath.Replace(".asset", "_temp.asset");
            AssetDatabase.CreateAsset(unreadableMesh, tempPath);
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
            string newPath = folder + "/" + unreadableMesh.name + ".asset";
            AssetDatabase.CreateAsset(unreadableMesh, newPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Mesh saved to: " + newPath);
        }
    }
}
