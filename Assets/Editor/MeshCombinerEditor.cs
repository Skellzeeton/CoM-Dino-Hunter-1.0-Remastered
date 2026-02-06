using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MeshCombinerEditor : EditorWindow
{
    private bool overwriteOriginals = false;
    private bool saveToMeshFolder = false;

    [MenuItem("Tools/Mesh Combiner")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(MeshCombinerEditor), false, "Mesh Combiner");
    }

    private void OnGUI()
    {
        GUILayout.Label("Combine Selected Meshes", EditorStyles.boldLabel);
        overwriteOriginals = EditorGUILayout.Toggle("Overwrite Originals", overwriteOriginals);
        saveToMeshFolder = EditorGUILayout.Toggle("Save to Mesh Folder", saveToMeshFolder);

        if (GUILayout.Button("Combine Selected"))
        {
            GameObject[] selectedObjects = Selection.gameObjects;
            if (selectedObjects.Length < 2)
            {
                Debug.LogWarning("Select at least two GameObjects with MeshFilters.");
                return;
            }

            CombineSelectedMeshes(selectedObjects, overwriteOriginals, saveToMeshFolder);
        }
    }

    private void CombineSelectedMeshes(GameObject[] objects, bool overwrite, bool saveMesh)
    {
        List<CombineInstance> combineList = new List<CombineInstance>();
        List<Material> materials = new List<Material>();
        List<int> materialIndices = new List<int>();
        List<GameObject> toDestroy = new List<GameObject>();

        for (int i = 0; i < objects.Length; i++)
        {
            GameObject go = objects[i];
            MeshFilter mf = go.GetComponent<MeshFilter>();
            MeshRenderer mr = go.GetComponent<MeshRenderer>();
            if (mf == null || mr == null || mf.sharedMesh == null) continue;

            Mesh mesh = mf.sharedMesh;
            Material[] mats = mr.sharedMaterials;

            for (int sub = 0; sub < mesh.subMeshCount; sub++)
            {
                Material mat = (sub < mats.Length) ? mats[sub] : (mats.Length > 0 ? mats[0] : null);
                if (mat == null) continue;

                CombineInstance ci = new CombineInstance();
                ci.mesh = mesh;
                ci.subMeshIndex = sub;
                ci.transform = go.transform.localToWorldMatrix;
                combineList.Add(ci);

                int matIndex = materials.IndexOf(mat);
                if (matIndex == -1)
                {
                    materials.Add(mat);
                    matIndex = materials.Count - 1;
                }
                materialIndices.Add(matIndex);
            }

            if (overwrite) toDestroy.Add(go);
        }

        if (combineList.Count == 0)
        {
            Debug.LogWarning("No valid meshes found to combine.");
            return;
        }

        // Combine by material index
        List<Mesh> subMeshes = new List<Mesh>();
        for (int m = 0; m < materials.Count; m++)
        {
            List<CombineInstance> part = new List<CombineInstance>();
            for (int j = 0; j < combineList.Count; j++)
            {
                if (materialIndices[j] == m)
                    part.Add(combineList[j]);
            }

            Mesh bucket = new Mesh();
            bucket.CombineMeshes(part.ToArray(), true, true);
            subMeshes.Add(bucket);
        }

        CombineInstance[] finalCombine = new CombineInstance[subMeshes.Count];
        for (int i = 0; i < subMeshes.Count; i++)
        {
            finalCombine[i].mesh = subMeshes[i];
            finalCombine[i].subMeshIndex = 0;
            finalCombine[i].transform = Matrix4x4.identity;
        }

        Mesh finalMesh = new Mesh();
        finalMesh.name = "CombinedMesh";
        finalMesh.CombineMeshes(finalCombine, false, false);

        if (saveMesh)
        {
            string folderPath = "Assets/Meshes";
            if (!AssetDatabase.IsValidFolder(folderPath))
                AssetDatabase.CreateFolder("Assets", "Meshes");

            string meshPath = folderPath + "/" + finalMesh.name + ".asset";
            AssetDatabase.CreateAsset(finalMesh, meshPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Mesh saved to: " + meshPath);
        }

        GameObject combined = new GameObject("CombinedMesh");
        combined.transform.position = Vector3.zero;
        combined.transform.rotation = Quaternion.identity;
        combined.transform.localScale = Vector3.one;

        MeshFilter newMF = combined.AddComponent<MeshFilter>();
        newMF.sharedMesh = finalMesh;

        MeshRenderer newMR = combined.AddComponent<MeshRenderer>();
        newMR.sharedMaterials = materials.ToArray();

        if (overwrite)
        {
            for (int i = 0; i < toDestroy.Count; i++)
                DestroyImmediate(toDestroy[i]);
        }

        Selection.activeGameObject = combined;
        Debug.Log("Combined mesh created at origin (0,0,0)");
    }
}
