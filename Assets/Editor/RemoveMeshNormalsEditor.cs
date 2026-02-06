using UnityEngine;
using UnityEditor;
using System.IO;

public class RemoveMeshNormalsEditor : EditorWindow
{
    private Mesh sourceMesh;
    private bool overwriteOriginal = false;

    [MenuItem("Tools/Remove Mesh Normals")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(RemoveMeshNormalsEditor), false, "Remove Normals");
    }

    private void OnGUI()
    {
        GUILayout.Label("Strip Normals from Mesh", EditorStyles.boldLabel);
        sourceMesh = (Mesh)EditorGUILayout.ObjectField("Source Mesh", sourceMesh, typeof(Mesh), false);
        overwriteOriginal = EditorGUILayout.Toggle("Overwrite Original", overwriteOriginal);

        if (GUILayout.Button("Remove Normals"))
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

        Mesh stripped = StripNormals(inputMesh);
        stripped.name = inputMesh.name + "_NoNormals";

        if (overwriteOriginal)
        {
            string tempPath = assetPath.Replace(".asset", "_temp.asset");
            AssetDatabase.CreateAsset(stripped, tempPath);
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
            string newPath = folder + "/" + stripped.name + ".asset";
            AssetDatabase.CreateAsset(stripped, newPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Mesh saved to: " + newPath);
        }
    }

    private Mesh StripNormals(Mesh mesh)
    {
        Mesh newMesh = new Mesh();
        newMesh.name = mesh.name + "_NoNormals";

        newMesh.vertices = mesh.vertices;
        newMesh.tangents = mesh.tangents;
        newMesh.uv = mesh.uv;
        newMesh.uv2 = mesh.uv2;
        newMesh.colors32 = mesh.colors32;
        newMesh.boneWeights = mesh.boneWeights;
        newMesh.bindposes = mesh.bindposes;

        newMesh.subMeshCount = mesh.subMeshCount;
        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            newMesh.SetTriangles(mesh.GetTriangles(i), i);
        }

        newMesh.normals = null;
        newMesh.RecalculateBounds();

        return newMesh;
    }
}
