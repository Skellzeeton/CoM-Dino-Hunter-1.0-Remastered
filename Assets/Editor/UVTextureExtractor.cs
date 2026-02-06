using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class UVTextureExtractor : EditorWindow
{
    private Mesh mesh;
    private Texture2D sourceTexture;
    private Color backgroundColor = Color.clear;
    private int textureResolution = 256;

    [MenuItem("Tools/Extract UV Texture")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(UVTextureExtractor), false, "UV Texture Extractor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Extract Texture from Mesh UVs", EditorStyles.boldLabel);
        mesh = (Mesh)EditorGUILayout.ObjectField("Mesh", mesh, typeof(Mesh), false);
        sourceTexture =
            (Texture2D)EditorGUILayout.ObjectField("Source Texture", sourceTexture, typeof(Texture2D), false);
        backgroundColor = EditorGUILayout.ColorField("Background Color", backgroundColor);
        textureResolution = EditorGUILayout.IntField("Output Resolution", textureResolution);

        if (GUILayout.Button("Extract Texture"))
        {
            if (mesh == null || sourceTexture == null)
            {
                Debug.LogWarning("Mesh and texture must be assigned.");
                return;
            }

            Texture2D result = ExtractUVTexture(mesh, sourceTexture, backgroundColor, textureResolution);
            string sourcePath = AssetDatabase.GetAssetPath(sourceTexture);
            int lastSlash = sourcePath.LastIndexOf('/');
            string folder = (lastSlash >= 0) ? sourcePath.Substring(0, lastSlash) : "Assets";
            string path = folder + "/" + mesh.name + "_ExtractedTexture.png";
            System.IO.File.WriteAllBytes(path, result.EncodeToPNG());
            AssetDatabase.Refresh();
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.filterMode = FilterMode.Trilinear;
                importer.wrapMode = TextureWrapMode.Clamp;
                importer.SaveAndReimport();
            }

            Debug.Log("Extracted texture saved to: " + path);
        }
    }
    
    private void DilateFilledPixels(Color[] pixels, int res, int iterations)
    {
        for (int iter = 0; iter < iterations; iter++)
        {
            Color[] original = (Color[])pixels.Clone();

            for (int y = 1; y < res - 1; y++)
            {
                for (int x = 1; x < res - 1; x++)
                {
                    int index = y * res + x;
                    if (original[index].a == 0f) // background pixel
                    {
                        int[] neighbors = {
                            (y - 1) * res + x,
                            (y + 1) * res + x,
                            y * res + (x - 1),
                            y * res + (x + 1)
                        };

                        foreach (int ni in neighbors)
                        {
                            if (original[ni].a > 0.9f) // filled neighbor
                            {
                                pixels[index] = original[ni];
                                break;
                            }
                        }
                    }
                }
            }
        }
    }


    private Texture2D ExtractUVTexture(Mesh mesh, Texture2D source, Color bgColor, int resolution)
    {
        Texture2D output = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false);
        output.name = mesh.name + "_ExtractedTexture";
        output.wrapMode = TextureWrapMode.Clamp;
        output.hideFlags = HideFlags.DontSave;

        Color[] pixels = new Color[resolution * resolution];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = bgColor;

        Vector2[] uvs = mesh.uv;
        int[] triangles = mesh.triangles;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector2 uv0 = uvs[triangles[i]];
            Vector2 uv1 = uvs[triangles[i + 1]];
            Vector2 uv2 = uvs[triangles[i + 2]];

            RasterizeTriangle(uv0, uv1, uv2, source, pixels, resolution);
        }
        for (int i = 0; i < uvs.Length; i++)
        {
            Vector2 uv = uvs[i];
            int px = Mathf.Clamp(Mathf.FloorToInt(uv.x * resolution), 0, resolution - 1);
            int py = Mathf.Clamp(Mathf.FloorToInt(uv.y * resolution), 0, resolution - 1);
            int tx = Mathf.Clamp(Mathf.FloorToInt(uv.x * source.width), 0, source.width - 1);
            int ty = Mathf.Clamp(Mathf.FloorToInt(uv.y * source.height), 0, source.height - 1);
            Color sampled = source.GetPixel(tx, ty);
            pixels[py * resolution + px] = sampled;
        }
        output.SetPixels(pixels);
        output.Apply();
        return output;
    }


private void RasterizeTriangle(Vector2 uv0, Vector2 uv1, Vector2 uv2, Texture2D source, Color[] pixels, int res)
{
    int minX = Mathf.Clamp(Mathf.FloorToInt(res * Mathf.Min(uv0.x, Mathf.Min(uv1.x, uv2.x))) - 1, 0, res - 1);
    int maxX = Mathf.Clamp(Mathf.CeilToInt(res * Mathf.Max(uv0.x, Mathf.Max(uv1.x, uv2.x))) + 1, 0, res - 1);
    int minY = Mathf.Clamp(Mathf.FloorToInt(res * Mathf.Min(uv0.y, Mathf.Min(uv1.y, uv2.y))) - 1, 0, res - 1);
    int maxY = Mathf.Clamp(Mathf.CeilToInt(res * Mathf.Max(uv0.y, Mathf.Max(uv1.y, uv2.y))) + 1, 0, res - 1);

    for (int y = minY; y <= maxY; y++)
    {
        for (int x = minX; x <= maxX; x++)
        {
            // Sample 9 points per pixel: center + corners + edges
            Vector2[] samples = new Vector2[]
            {
                new Vector2((x + 0.5f) / res, (y + 0.5f) / res),
                new Vector2((x + 0.0f) / res, (y + 0.0f) / res),
                new Vector2((x + 1.0f) / res, (y + 0.0f) / res),
                new Vector2((x + 0.0f) / res, (y + 1.0f) / res),
                new Vector2((x + 1.0f) / res, (y + 1.0f) / res),
                new Vector2((x + 0.5f) / res, (y + 0.0f) / res),
                new Vector2((x + 0.5f) / res, (y + 1.0f) / res),
                new Vector2((x + 0.0f) / res, (y + 0.5f) / res),
                new Vector2((x + 1.0f) / res, (y + 0.5f) / res),
            };

            bool inside = false;
            for (int i = 0; i < samples.Length; i++)
            {
                if (PointInTriangle(samples[i], uv0, uv1, uv2))
                {
                    inside = true;
                    break;
                }
            }

            if (inside)
            {
                int px = x;
                int py = y;
                Vector2 uv = samples[0]; // center sample for color
                int tx = Mathf.Clamp(Mathf.FloorToInt(uv.x * source.width), 0, source.width - 1);
                int ty = Mathf.Clamp(Mathf.FloorToInt(uv.y * source.height), 0, source.height - 1);
                Color sampled = source.GetPixel(tx, ty);
                pixels[py * res + px] = sampled;
            }
        }
    }
}

    
    private bool PointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
    {
        Vector2 v0 = b - a;
        Vector2 v1 = c - a;
        Vector2 v2 = p - a;

        float d00 = Vector2.Dot(v0, v0);
        float d01 = Vector2.Dot(v0, v1);
        float d11 = Vector2.Dot(v1, v1);
        float d20 = Vector2.Dot(v2, v0);
        float d21 = Vector2.Dot(v2, v1);

        float denom = d00 * d11 - d01 * d01;
        if (denom == 0) return false;

        float v = (d11 * d20 - d01 * d21) / denom;
        float w = (d00 * d21 - d01 * d20) / denom;
        float u = 1.0f - v - w;

        return (u >= -0.01f && v >= -0.01f && w >= -0.01f); // slight tolerance
    }
}