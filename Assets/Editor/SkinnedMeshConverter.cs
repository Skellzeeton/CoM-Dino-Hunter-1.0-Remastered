using UnityEngine;
using UnityEditor;

public class SkinnedMeshConverter : EditorWindow
{
    private bool overwriteOriginals = false;

    [MenuItem("Tools/Convert Skinned Meshes (Scene Only)")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(SkinnedMeshConverter), false, "Skinned Mesh Converter");
    }

    private void OnGUI()
    {
        GUILayout.Label("Convert SkinnedMeshRenderer to MeshRenderer", EditorStyles.boldLabel);
        overwriteOriginals = EditorGUILayout.Toggle("Overwrite Originals", overwriteOriginals);

        if (GUILayout.Button("Convert Selected Scene Objects"))
        {
            GameObject[] selectedObjects = Selection.gameObjects;
            if (selectedObjects.Length == 0)
            {
                Debug.LogWarning("Select one or more GameObjects in the scene.");
                return;
            }

            int convertedCount = 0;

            for (int i = 0; i < selectedObjects.Length; i++)
            {
                GameObject go = selectedObjects[i];
                if (go.scene.name == null || go.scene.name == "")
                {
                    Debug.LogWarning("Skipping project asset: " + go.name);
                    continue;
                }

                convertedCount += ConvertSkinnedMeshes(go, overwriteOriginals);
            }

            Debug.Log("Converted " + convertedCount + " SkinnedMeshRenderers to MeshRenderers.");
        }
    }

    private int ConvertSkinnedMeshes(GameObject root, bool overwrite)
    {
        SkinnedMeshRenderer[] skinnedRenderers = root.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        int count = 0;

        for (int i = 0; i < skinnedRenderers.Length; i++)
        {
            SkinnedMeshRenderer smr = skinnedRenderers[i];
            Mesh originalMesh = smr.sharedMesh;
            if (originalMesh == null) continue;

            GameObject targetGO;
            if (overwrite)
            {
                targetGO = smr.gameObject;
            }
            else
            {
                targetGO = new GameObject(smr.name + "_Static");
                targetGO.transform.parent = smr.transform.parent;
                targetGO.transform.localPosition = smr.transform.localPosition;
                targetGO.transform.localRotation = smr.transform.localRotation;
                targetGO.transform.localScale = smr.transform.localScale;
            }

            MeshFilter mf = targetGO.GetComponent<MeshFilter>();
            if (mf == null) mf = targetGO.AddComponent<MeshFilter>();
            mf.sharedMesh = originalMesh;

            MeshRenderer mr = targetGO.GetComponent<MeshRenderer>();
            if (mr == null) mr = targetGO.AddComponent<MeshRenderer>();

            mr.sharedMaterials = smr.sharedMaterials;
            mr.lightProbeUsage = smr.lightProbeUsage;
            mr.reflectionProbeUsage = smr.reflectionProbeUsage;
            mr.shadowCastingMode = smr.shadowCastingMode;
            mr.receiveShadows = smr.receiveShadows;
            mr.motionVectorGenerationMode = smr.motionVectorGenerationMode;
            mr.allowOcclusionWhenDynamic = smr.allowOcclusionWhenDynamic;

            if (overwrite)
            {
                DestroyImmediate(smr, true);
            }

            count++;
        }

        return count;
    }
}
