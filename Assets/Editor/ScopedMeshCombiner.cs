using UnityEngine;
using System.Collections.Generic;

public class ScopedMeshCombiner : MonoBehaviour
{
    public bool combineOnStart = true;
    public bool castShadows = false;
    public bool receiveShadows = false;

    void Start()
    {
        if (combineOnStart)
            CombineChildMeshes();
    }

    public void CombineChildMeshes()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        Dictionary<Transform, List<CombineInstance>> renderMap = new Dictionary<Transform, List<CombineInstance>>();
        Dictionary<Transform, List<CombineInstance>> colliderMap = new Dictionary<Transform, List<CombineInstance>>();
        Dictionary<Transform, List<Material>>    materialMap = new Dictionary<Transform, List<Material>>();
        Dictionary<Transform, Dictionary<Material,int>> lookupMap = new Dictionary<Transform, Dictionary<Material,int>>();
        Dictionary<Transform, List<int>>          indexMap = new Dictionary<Transform, List<int>>();
        List<CombineInstance> barrierCombine         = new List<CombineInstance>();
        List<CombineInstance> barrierColliderCombine = new List<CombineInstance>();
        /*List<CombineInstance> groundCombine          = new List<CombineInstance>();
        List<CombineInstance> groundColliderCombine  = new List<CombineInstance>();
        List<Material>       groundMaterials         = new List<Material>();*/
        List<GameObject> toDestroy = new List<GameObject>();
        for (int i = 0; i < meshFilters.Length; i++)
        {
            Transform tf = meshFilters[i].transform;
            if (tf == transform) continue;
            int objLayer = tf.gameObject.layer;
            if (objLayer == 29) continue;
            if (tf.parent == transform)
            {
                continue;
            }
            toDestroy.Add(tf.gameObject);
            if (tf.GetComponent<SkinnedMeshRenderer>() != null) continue;
            if (tf.GetComponent<BoxCollider>() != null)       continue;
            MeshRenderer mr = tf.GetComponent<MeshRenderer>();
            if (mr == null || meshFilters[i].sharedMesh == null)
                continue;
            Mesh mesh       = meshFilters[i].sharedMesh;
            Material[] mats = mr.sharedMaterials;
            bool  hasCol    = tf.GetComponent<MeshCollider>() != null;
            int   layer     = tf.gameObject.layer;
            for (int sub = 0; sub < mesh.subMeshCount; sub++)
            {
                Material mat = (sub < mats.Length) ? mats[sub]
                            : (mats.Length > 0 ? mats[0] : null);
                if (mat == null && layer != 28/* && layer != 29*/)
                    continue;

                CombineInstance ci = new CombineInstance();
                ci.mesh         = mesh;
                ci.subMeshIndex = sub;
                ci.transform    = tf.localToWorldMatrix;

                if (layer == 28)
                {
                    barrierCombine.Add(ci);
                    if (hasCol) barrierColliderCombine.Add(ci);
                    continue;
                }
                /*if (layer == 29)
                {
                    groundCombine.Add(ci);
                    if (mat != null && !groundMaterials.Contains(mat))
                        groundMaterials.Add(mat);
                    if (hasCol)
                        groundColliderCombine.Add(ci);
                }*/
                Transform parent = tf.parent;
                if (parent == transform) continue;
                if (!renderMap.ContainsKey(parent))
                {
                    renderMap[parent]       = new List<CombineInstance>();
                    colliderMap[parent]     = new List<CombineInstance>();
                    materialMap[parent]     = new List<Material>();
                    lookupMap[parent]       = new Dictionary<Material,int>();
                    indexMap[parent]        = new List<int>();
                }
                int matIndex;
                if (!lookupMap[parent].ContainsKey(mat))
                {
                    matIndex = materialMap[parent].Count;
                    materialMap[parent].Add(mat);
                    lookupMap[parent].Add(mat, matIndex);
                }
                else
                {
                    matIndex = lookupMap[parent][mat];
                }
                renderMap[parent].Add(ci);
                indexMap[parent].Add(matIndex);

                if (hasCol)
                {
                    CombineInstance cci = new CombineInstance();
                    cci.mesh         = mesh;
                    cci.subMeshIndex = sub;
                    cci.transform    = tf.localToWorldMatrix;
                    colliderMap[parent].Add(cci);
                }
            }

            toDestroy.Add(tf.gameObject);
        }
        foreach (Transform parent in renderMap.Keys)
        {
            List<CombineInstance>    rcList = renderMap[parent];
            List<CombineInstance>    ccList = colliderMap[parent];
            List<Material>           mats   = materialMap[parent];
            List<int>                ids    = indexMap[parent];
            List<Mesh>      subMeshes      = new List<Mesh>();
            List<Material>  finalMaterials = new List<Material>();
            for (int m = 0; m < mats.Count; m++)
            {
                List<CombineInstance> part = new List<CombineInstance>();
                for (int j = 0; j < rcList.Count; j++)
                    if (ids[j] == m)
                        part.Add(rcList[j]);

                Mesh bucket = new Mesh();
                bucket.CombineMeshes(part.ToArray(), true, true);
                subMeshes.Add(bucket);
                finalMaterials.Add(mats[m]);
            }
            CombineInstance[] flat = new CombineInstance[subMeshes.Count];
            for (int i = 0; i < subMeshes.Count; i++)
            {
                flat[i].mesh         = subMeshes[i];
                flat[i].subMeshIndex = 0;
                flat[i].transform    = Matrix4x4.identity;
            }

            Mesh finalMesh = new Mesh();
            finalMesh.name = "CombinedMesh_" + parent.name;
            finalMesh.CombineMeshes(flat, false, false);
            GameObject go = new GameObject("CombinedMesh_" + parent.name);
            go.layer = 31;
            go.transform.parent        = transform;
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale    = Vector3.one;

            MeshFilter mf = go.AddComponent<MeshFilter>();
            mf.sharedMesh = finalMesh;

            MeshRenderer mr = go.AddComponent<MeshRenderer>();
            mr.sharedMaterials             = finalMaterials.ToArray();
            mr.shadowCastingMode           = castShadows
                ? UnityEngine.Rendering.ShadowCastingMode.On
                : UnityEngine.Rendering.ShadowCastingMode.Off;
            mr.receiveShadows              = receiveShadows;
            mr.motionVectorGenerationMode  = MotionVectorGenerationMode.Camera;
            mr.allowOcclusionWhenDynamic   = false;
            mr.lightProbeUsage             = UnityEngine.Rendering.LightProbeUsage.Off;
            mr.reflectionProbeUsage        = UnityEngine.Rendering.ReflectionProbeUsage.Off;

            if (ccList.Count > 0)
            {
                Mesh colMesh = new Mesh();
                colMesh.name = "CombinedCollider_" + parent.name;
                colMesh.CombineMeshes(ccList.ToArray(), false, true);

                MeshCollider mc = go.AddComponent<MeshCollider>();
                mc.sharedMesh = colMesh;
                mc.convex     = false;
            }
        }
        if (barrierCombine.Count > 0)
        {
            Mesh bm = new Mesh();
            bm.name = "CombinedMesh_Barrier";
            bm.CombineMeshes(barrierCombine.ToArray(), true, true);

            GameObject bgo = new GameObject("CombinedMesh_Barrier");
            bgo.layer = 28;
            bgo.transform.parent        = transform;
            bgo.transform.localPosition = Vector3.zero;
            bgo.transform.localRotation = Quaternion.identity;
            bgo.transform.localScale    = Vector3.one;

            MeshFilter  bmf = bgo.AddComponent<MeshFilter>();
            bmf.sharedMesh = bm;

            if (barrierColliderCombine.Count > 0)
            {
                Mesh bcm = new Mesh();
                bcm.name = "CombinedMesh_Barrier";
                bcm.CombineMeshes(barrierColliderCombine.ToArray(), false, true);

                MeshCollider bmc = bgo.AddComponent<MeshCollider>();
                bmc.sharedMesh = bcm;
                bmc.convex     = false;
            }
        }
        for (int i = 0; i < toDestroy.Count; i++)
            Destroy(toDestroy[i]);
    }
}
