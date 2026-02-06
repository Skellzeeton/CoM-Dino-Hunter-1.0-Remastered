using UnityEngine;
using UnityEditor;
using System.IO;

public class MeshUVStripperEditor : EditorWindow
{
    private Mesh sourceMesh;
    private bool overwriteOriginal = false;

    [MenuItem("Tools/Mesh UV Stripper")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(MeshUVStripperEditor), false, "Mesh UV Stripper");
    }
    
    private bool stripUV = false;
    private bool stripUV2 = false;
    private bool stripUV3 = false;

    private void OnGUI()
    {
        GUILayout.Label("Strip UVS from Mesh", EditorStyles.boldLabel);
        sourceMesh = (Mesh)EditorGUILayout.ObjectField("Source Mesh", sourceMesh, typeof(Mesh), false);
        overwriteOriginal = EditorGUILayout.Toggle("Overwrite Original", overwriteOriginal);
        stripUV = EditorGUILayout.Toggle("Strip UV", stripUV);
        stripUV2 = EditorGUILayout.Toggle("Strip UV2", stripUV2);
        stripUV3 = EditorGUILayout.Toggle("Strip UV3", stripUV3);
        if (GUILayout.Button("Strip UVS"))
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
        newMesh.name = mesh.name + "_NoUVs";
        if (stripUV) newMesh.uv = new Vector2[0];
        if (stripUV2) newMesh.uv2 = new Vector2[0];
        if (stripUV3) newMesh.uv3 = new Vector2[0];



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
