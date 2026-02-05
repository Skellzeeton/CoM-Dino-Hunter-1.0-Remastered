using UnityEngine;

public static class UITextureUtil
{
    public static void LoadWeaponLikeIcon(
        TUIMeshSprite sprite,
        int itemID,
        string basePath)
    {
        if (sprite == null)
            return;

        string texName = TUIMappingInfo.Instance().GetWeaponTexture(itemID);
        if (string.IsNullOrEmpty(texName))
        {
            Debug.LogWarning("Missing weapon-like texture mapping for ID: " + itemID);
            return;
        }

        Texture tex = Resources.Load(basePath + texName) as Texture;
        if (tex == null)
        {
            Debug.LogWarning("Missing texture: " + basePath + texName);
            return;
        }

        sprite.texture = string.Empty;
        sprite.UseCustomize = true;
        sprite.CustomizeTexture = tex;
        sprite.CustomizeRect = new Rect(0f, 0f, 200f, 128f);
    }
}