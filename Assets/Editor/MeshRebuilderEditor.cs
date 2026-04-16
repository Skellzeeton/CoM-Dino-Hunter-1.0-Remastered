using UnityEngine;
using UnityEditor;
using System.IO;

public class MeshRebuilderEditor : EditorWindow
{
    private Mesh sourceMesh;
    private bool overwriteOriginal = false;
    private bool rebuildNormals = false;
    private bool removeTangents = false;
    private bool makeUnreadable = false;

    [MenuItem("Tools/Mesh Rebuilder")]
    public static void ShowWindow()
    {
        GetWindow<MeshRebuilderEditor>("Mesh Rebuilder");
    }

    private void OnGUI()
    {
        GUILayout.Label("Rebuild Legacy Mesh", EditorStyles.boldLabel);
        sourceMesh = (Mesh)EditorGUILayout.ObjectField("Source Mesh", sourceMesh, typeof(Mesh), false);
        overwriteOriginal = EditorGUILayout.Toggle("Replace Original Mesh", overwriteOriginal);
        EditorGUILayout.Space();
        GUILayout.Label("Rebuild Options", EditorStyles.boldLabel);
        rebuildNormals = EditorGUILayout.Toggle("Rebuild Normals", rebuildNormals);
        removeTangents = EditorGUILayout.Toggle("Remove Tangents", removeTangents);
        makeUnreadable = EditorGUILayout.Toggle("Make Mesh Unreadable", makeUnreadable);
        EditorGUILayout.Space();
        if (GUILayout.Button("Rebuild Mesh"))
        {
            Object[] targets = Selection.GetFiltered(typeof(Mesh), SelectionMode.Assets);

            if (sourceMesh != null)
                RebuildMesh(sourceMesh);

            foreach (Object obj in targets)
            {
                Mesh mesh = obj as Mesh;
                if (mesh != null && mesh != sourceMesh)
                    RebuildMesh(mesh);
            }
        }
    }

    private void RebuildMesh(Mesh mesh)
    {
        string assetPath = AssetDatabase.GetAssetPath(mesh);
        if (string.IsNullOrEmpty(assetPath)) return;
        Mesh newMesh = new Mesh();
        newMesh.Clear();
        if (mesh.vertices != null && mesh.vertexCount > 0)
            newMesh.vertices = mesh.vertices;
        if (!rebuildNormals && mesh.normals != null && mesh.normals.Length == mesh.vertexCount)
            newMesh.normals = mesh.normals;
        if (!removeTangents && mesh.tangents != null && mesh.tangents.Length == mesh.vertexCount)
            newMesh.tangents = mesh.tangents;
        if (mesh.uv != null && mesh.uv.Length == mesh.vertexCount) newMesh.uv = mesh.uv;
        if (mesh.uv2 != null && mesh.uv2.Length == mesh.vertexCount) newMesh.uv2 = mesh.uv2;
        if (mesh.uv3 != null && mesh.uv3.Length == mesh.vertexCount) newMesh.uv3 = mesh.uv3;
        if (mesh.uv4 != null && mesh.uv4.Length == mesh.vertexCount) newMesh.uv4 = mesh.uv4;
        if (mesh.boneWeights != null && mesh.boneWeights.Length == mesh.vertexCount)
            newMesh.boneWeights = mesh.boneWeights;
        if (mesh.bindposes != null && mesh.bindposes.Length > 0)
            newMesh.bindposes = mesh.bindposes;
        newMesh.subMeshCount = mesh.subMeshCount;
        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            int[] tris = mesh.GetTriangles(i);
            if (tris != null && tris.Length > 0)
                newMesh.SetTriangles(tris, i);
        }
        newMesh.RecalculateBounds();
        if (rebuildNormals)
            newMesh.RecalculateNormals();
        if (makeUnreadable)
            newMesh.UploadMeshData(true);
        if (overwriteOriginal)
        {
            string originalFileName = System.IO.Path.GetFileNameWithoutExtension(assetPath);
            newMesh.name = originalFileName;
            if (AssetDatabase.DeleteAsset(assetPath))
            {
                AssetDatabase.CreateAsset(newMesh, assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
                //Debug.Log("Mesh overwritten and renamed to: " + originalFileName + "  (" + assetPath + ")");
            }
            else
            {
                Debug.LogError("Failed to delete original asset at: " + assetPath);
            }
        }
        else
        {
            int lastSlash = assetPath.LastIndexOf('/');
            string folder = (lastSlash >= 0) ? assetPath.Substring(0, lastSlash) : "Assets";
            string baseFileName = System.IO.Path.GetFileNameWithoutExtension(assetPath) + "_Rebuilt";
            string newPath = folder + "/" + baseFileName + ".asset";
            string uniquePath = AssetDatabase.GenerateUniqueAssetPath(newPath);
            newMesh.name = System.IO.Path.GetFileNameWithoutExtension(uniquePath);
            AssetDatabase.CreateAsset(newMesh, uniquePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Mesh saved to: " + uniquePath + " (internal name: " + newMesh.name + ")");
        }
    }
}