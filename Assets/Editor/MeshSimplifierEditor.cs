using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class MeshSimplifierEditor : EditorWindow
{
    private Mesh sourceMesh;
    private bool overwriteOriginal = false;
    private float mergeThreshold = 0.001f;
    private bool simplifyVertices = true;
    private bool simplifyTriangles = true;
    private int matchModeIndex = 1; 
    private string[] matchModes = new string[] { "Low", "Balanced", "Loose" };

    [MenuItem("Tools/Mesh Simplifier")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(MeshSimplifierEditor), false, "Mesh Simplifier");
    }

    private void OnGUI()
    {
        GUILayout.Label("Simplify Mesh Geometry", EditorStyles.boldLabel);
        sourceMesh = (Mesh)EditorGUILayout.ObjectField("Source Mesh", sourceMesh, typeof(Mesh), false);
        overwriteOriginal = EditorGUILayout.Toggle("Overwrite Original", overwriteOriginal);
        simplifyVertices = EditorGUILayout.Toggle("Simplify Vertices", simplifyVertices);
        simplifyTriangles = EditorGUILayout.Toggle("Simplify Triangles", simplifyTriangles);
        mergeThreshold = EditorGUILayout.FloatField("Merge Threshold", mergeThreshold);
        matchModeIndex = EditorGUILayout.Popup("Match Mode", matchModeIndex, matchModes);

        if (GUILayout.Button("Simplify Mesh"))
        {
            Object[] selectedMeshes = Selection.GetFiltered(typeof(Mesh), SelectionMode.Assets);
            if (sourceMesh != null)
            {
                ProcessMesh(sourceMesh, mergeThreshold, simplifyVertices, simplifyTriangles, matchModeIndex);
            }

            for (int i = 0; i < selectedMeshes.Length; i++)
            {
                Mesh selectedMesh = selectedMeshes[i] as Mesh;
                if (selectedMesh != null && selectedMesh != sourceMesh)
                {
                    ProcessMesh(selectedMesh, mergeThreshold, simplifyVertices, simplifyTriangles, matchModeIndex);
                }
            }
        }
    }

    private void ProcessMesh(Mesh inputMesh, float threshold, bool simplifyVerts, bool simplifyTris, int matchMode)
    {
        string assetPath = AssetDatabase.GetAssetPath(inputMesh);
        if (string.IsNullOrEmpty(assetPath)) return;

        Mesh simplified = SimplifyMesh(inputMesh, threshold, simplifyVerts, simplifyTris, matchMode);
        simplified.name = inputMesh.name + "_Simplified";

        if (overwriteOriginal)
        {
            string tempPath = assetPath.Replace(".asset", "_temp.asset");
            AssetDatabase.CreateAsset(simplified, tempPath);
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
            string newPath = folder + "/" + simplified.name + ".asset";
            AssetDatabase.CreateAsset(simplified, newPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Mesh saved to: " + newPath);
        }
    }

    private Mesh SimplifyMesh(Mesh mesh, float threshold, bool simplifyVerts, bool simplifyTris, int matchMode)
    {
        Vector3[] verts = mesh.vertices;
        Dictionary<int, int> remap = new Dictionary<int, int>();
        List<Vector3> newVerts = new List<Vector3>();
        List<int> newToOriginal = new List<int>();

        if (simplifyVerts)
        {
            for (int i = 0; i < verts.Length; i++)
            {
                Vector3 v = verts[i];
                bool found = false;
                for (int j = 0; j < newVerts.Count; j++)
                {
                    int originalJ = newToOriginal[j];
                    bool uvMatch = true;
                    bool normalMatch = true;
                    bool tangentMatch = true;

                    if (mesh.uv != null && mesh.uv.Length == verts.Length)
                    {
                        Vector2 uvA = mesh.uv[i];
                        Vector2 uvB = mesh.uv[originalJ];
                        uvMatch = (uvA - uvB).sqrMagnitude < (matchMode == 0 ? 0.00001f : 0.001f);
                    }

                    if (matchMode <= 1 && mesh.normals != null && mesh.normals.Length == verts.Length)
                    {
                        Vector3 nA = mesh.normals[i];
                        Vector3 nB = mesh.normals[originalJ];
                        normalMatch = Vector3.Angle(nA, nB) < (matchMode == 0 ? 5f : 30f);
                    }

                    if (matchMode == 0 && mesh.tangents != null && mesh.tangents.Length == verts.Length)
                    {
                        Vector4 tA = mesh.tangents[i];
                        Vector4 tB = mesh.tangents[originalJ];
                        tangentMatch = Vector3.Angle(new Vector3(tA.x, tA.y, tA.z), new Vector3(tB.x, tB.y, tB.z)) < 10f;
                    }

                    if ((newVerts[j] - v).sqrMagnitude < threshold * threshold && uvMatch && normalMatch && tangentMatch)
                    {
                        remap[i] = j;
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    remap[i] = newVerts.Count;
                    newVerts.Add(v);
                    newToOriginal.Add(i);
                }
            }
        }
        else
        {
            for (int i = 0; i < verts.Length; i++)
            {
                remap[i] = i;
            }
            newVerts.AddRange(verts);
        }

        Mesh newMesh = new Mesh();
        newMesh.vertices = newVerts.ToArray();
        newMesh.subMeshCount = mesh.subMeshCount;

        for (int s = 0; s < mesh.subMeshCount; s++)
        {
            int[] tris = mesh.GetTriangles(s);
            List<int> newTris = new List<int>();

            for (int i = 0; i < tris.Length; i += 3)
            {
                int a = remap[tris[i]];
                int b = remap[tris[i + 1]];
                int c = remap[tris[i + 2]];

                if (!simplifyTris || (a != b && b != c && c != a))
                {
                    newTris.Add(a);
                    newTris.Add(b);
                    newTris.Add(c);
                }
            }

            newMesh.SetTriangles(newTris.ToArray(), s);
        }

        int newCount = newVerts.Count;

        if (mesh.normals != null && mesh.normals.Length == verts.Length)
            newMesh.normals = AverageVector3(mesh.normals, remap, newCount);
        if (mesh.tangents != null && mesh.tangents.Length == verts.Length)
            newMesh.tangents = AverageVector4(mesh.tangents, remap, newCount);
        if (mesh.uv != null && mesh.uv.Length == verts.Length)
            newMesh.uv = AverageVector2(mesh.uv, remap, newCount);
        if (mesh.uv2 != null && mesh.uv2.Length == verts.Length)
            newMesh.uv2 = AverageVector2(mesh.uv2, remap, newCount);
        if (mesh.colors32 != null && mesh.colors32.Length == verts.Length)
            newMesh.colors32 = AverageColor(mesh.colors32, remap, newCount);
        if (mesh.boneWeights != null && mesh.boneWeights.Length == verts.Length)
            newMesh.boneWeights = RemapBoneWeights(mesh.boneWeights, remap, newCount);
        if (mesh.bindposes != null)
            newMesh.bindposes = mesh.bindposes;

        newMesh.RecalculateBounds();
        return newMesh;
    }


    private Vector3[] AverageVector3(Vector3[] data, Dictionary<int, int> remap, int newCount)
    {
        Vector3[] result = new Vector3[newCount];
        int[] counts = new int[newCount];
        foreach (KeyValuePair<int, int> kv in remap)
        {
            result[kv.Value] += data[kv.Key];
            counts[kv.Value]++;
        }

        for (int i = 0; i < newCount; i++)
        {
            if (counts[i] > 0)
                result[i] /= counts[i];
        }

        return result;
    }

    private Vector4[] AverageVector4(Vector4[] data, Dictionary<int, int> remap, int newCount)
    {
        Vector4[] result = new Vector4[newCount];
        int[] counts = new int[newCount];
        foreach (KeyValuePair<int, int> kv in remap)
        {
            result[kv.Value] += data[kv.Key];
            counts[kv.Value]++;
        }

        for (int i = 0; i < newCount; i++)
        {
            if (counts[i] > 0)
                result[i] /= counts[i];
        }

        return result;
    }

    private Vector2[] AverageVector2(Vector2[] data, Dictionary<int, int> remap, int newCount)
    {
        Vector2[] result = new Vector2[newCount];
        int[] counts = new int[newCount];
        foreach (KeyValuePair<int, int> kv in remap)
        {
            result[kv.Value] += data[kv.Key];
            counts[kv.Value]++;
        }

        for (int i = 0; i < newCount; i++)
        {
            if (counts[i] > 0)
                result[i] /= counts[i];
        }

        return result;
    }

    private Color32[] AverageColor(Color32[] data, Dictionary<int, int> remap, int newCount)
    {
        Color32[] result = new Color32[newCount];
        int[] r = new int[newCount];
        int[] g = new int[newCount];
        int[] b = new int[newCount];
        int[] a = new int[newCount];
        int[] counts = new int[newCount];

        foreach (KeyValuePair<int, int> kv in remap)
        {
            Color32 c = data[kv.Key];
            int i = kv.Value;
            r[i] += c.r;
            g[i] += c.g;
            b[i] += c.b;
            a[i] += c.a;
            counts[i]++;
        }

        for (int i = 0; i < newCount; i++)
        {
            if (counts[i] > 0)
            {
                result[i] = new Color32(
                    (byte)(r[i] / counts[i]),
                    (byte)(g[i] / counts[i]),
                    (byte)(b[i] / counts[i]),
                    (byte)(a[i] / counts[i])
                );
            }
        }

        return result;
    }

    private BoneWeight[] RemapBoneWeights(BoneWeight[] data, Dictionary<int, int> remap, int newCount)
    {
        BoneWeight[] result = new BoneWeight[newCount];
        bool[] filled = new bool[newCount];

        foreach (var kv in remap)
        {
            int i = kv.Value;
            if (!filled[i])
            {
                result[i] = data[kv.Key];
                filled[i] = true;
            }
        }

        return result;
    }
}
