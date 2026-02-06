using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class SubmeshFusionEditor : EditorWindow
{
    private Mesh sourceMesh;
    private bool overwriteOriginal = false;

    [MenuItem("Tools/Submesh Fusion")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(SubmeshFusionEditor), false, "Submesh Fusion");
    }

    private void OnGUI()
    {
        GUILayout.Label("Fuse Submeshes into One", EditorStyles.boldLabel);
        sourceMesh = (Mesh)EditorGUILayout.ObjectField("Source Mesh", sourceMesh, typeof(Mesh), false);
        overwriteOriginal = EditorGUILayout.Toggle("Overwrite Original", overwriteOriginal);

        if (GUILayout.Button("Fuse Submeshes"))
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

        Mesh fused = FuseSubmeshes(inputMesh);
        fused.name = inputMesh.name + "_Fused";

        if (overwriteOriginal)
        {
            string tempPath = assetPath.Replace(".asset", "_temp.asset");
            AssetDatabase.CreateAsset(fused, tempPath);
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
            string newPath = folder + "/" + fused.name + ".asset";
            AssetDatabase.CreateAsset(fused, newPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Mesh saved to: " + newPath);
        }
    }

    private Mesh FuseSubmeshes(Mesh mesh)
    {
        Mesh newMesh = new Mesh();
        newMesh.name = mesh.name + "_Fused";

        newMesh.vertices = mesh.vertices;
        newMesh.normals = mesh.normals;
        newMesh.tangents = mesh.tangents;
        newMesh.uv = mesh.uv;
        newMesh.uv2 = mesh.uv2;
        newMesh.colors32 = mesh.colors32;
        newMesh.boneWeights = mesh.boneWeights;
        newMesh.bindposes = mesh.bindposes;

        List<int> allTriangles = new List<int>();
        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            int[] tris = mesh.GetTriangles(i);
            allTriangles.AddRange(tris);
        }

        newMesh.subMeshCount = 1;
        newMesh.SetTriangles(allTriangles.ToArray(), 0);
        newMesh.RecalculateBounds();

        return newMesh;
    }
}
