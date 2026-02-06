using UnityEngine;
using UnityEditor;
using System.IO;

public class TextureReexporter : EditorWindow
{
    private bool replaceOriginals = false;

    [MenuItem("Tools/Re Export Selected Textures")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(TextureReexporter), false, "Texture Reexporter");
    }

    private void OnGUI()
    {
        GUILayout.Label("Reexport Selected Textures", EditorStyles.boldLabel);
        replaceOriginals = EditorGUILayout.Toggle("Replace Originals", replaceOriginals);

        Object[] selectedTextures = Selection.GetFiltered(typeof(Texture2D), SelectionMode.Assets);
        if (selectedTextures.Length == 0)
        {
            GUILayout.Label("Select one or more Texture2D assets in the Project window.", EditorStyles.helpBox);
            return;
        }

        if (GUILayout.Button("Reexport Textures"))
        {
            for (int i = 0; i < selectedTextures.Length; i++)
            {
                Texture2D original = selectedTextures[i] as Texture2D;
                if (original == null) continue;

                string originalPath = AssetDatabase.GetAssetPath(original);
                int lastSlash = originalPath.LastIndexOf('/');
                string folder = (lastSlash >= 0) ? originalPath.Substring(0, lastSlash) : "Assets";
                string filename = originalPath.Substring(lastSlash + 1);
                int dot = filename.LastIndexOf('.');
                if (dot >= 0) filename = filename.Substring(0, dot);
                string newPath = folder + "/" + filename + "_Reexported.png";

                Texture2D readable = BlitToReadable(original);
                byte[] pngData = readable.EncodeToPNG();
                File.WriteAllBytes(newPath, pngData);
                AssetDatabase.Refresh();

                TextureImporter importer = AssetImporter.GetAtPath(newPath) as TextureImporter;
                if (importer != null)
                {
                    importer.textureCompression = TextureImporterCompression.Uncompressed;
                    importer.filterMode = FilterMode.Bilinear;
                    importer.wrapMode = TextureWrapMode.Clamp;
                    importer.mipmapEnabled = false;
                    importer.isReadable = true;
                    importer.SaveAndReimport();
                }

                Debug.Log("Saved Reexported texture: " + newPath);

                if (replaceOriginals)
                {
                    ReplaceOriginalTexture(originalPath, newPath);
                }
            }
        }
    }

    private Texture2D BlitToReadable(Texture2D source)
    {
        int w = source.width;
        int h = source.height;
        RenderTexture rt = RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.Default);
        Graphics.Blit(source, rt);
        RenderTexture.active = rt;

        Texture2D readable = new Texture2D(w, h, TextureFormat.RGBA32, false);
        readable.ReadPixels(new Rect(0, 0, w, h), 0, 0);
        readable.Apply();

        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);
        return readable;
    }

    private void ReplaceOriginalTexture(string originalPath, string newPath)
    {
        string metaPath = originalPath + ".meta";
        string tempMetaPath = originalPath + "_metaTemp";

        if (File.Exists(metaPath))
            File.Move(metaPath, tempMetaPath);

        AssetDatabase.DeleteAsset(originalPath);
        File.Move(newPath, originalPath);

        if (File.Exists(tempMetaPath))
            File.Move(tempMetaPath, metaPath);

        AssetDatabase.ImportAsset(originalPath, ImportAssetOptions.ForceUpdate);
        Debug.Log("Replaced original texture: " + originalPath);
    }
}
