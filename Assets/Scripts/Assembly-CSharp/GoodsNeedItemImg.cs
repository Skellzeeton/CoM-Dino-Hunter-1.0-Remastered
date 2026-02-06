using UnityEngine;

public class GoodsNeedItemImg : MonoBehaviour
{
    public TUIMeshSprite img_goods;
    public TUIMeshSprite img_bg;

    private string texture_path = "TUI/Goods/";

    private string texture_quality01 = "kuangdj_1";
    private string texture_quality02 = "kuangdj_2";
    private string texture_quality03 = "kuangdj_3";
    private string texture_quality04 = "kuangdj_4";
    private string texture_quality05 = "kuangdj_5";
    private string texture_quality06 = "kuangdj_6";

    private int goods_id;
    private string goods_name = string.Empty;

    public void SetInfo(int m_goods_id, GoodsQualityType m_type, string m_goods_name)
    {
        goods_id = m_goods_id;
        goods_name = m_goods_name;

        string stashTexture = TUIMappingInfo.Instance().GetStashTexture(m_goods_id);
        SetGoodsCustomizeTexture(img_goods, texture_path + stashTexture);

        switch (m_type)
        {
            case GoodsQualityType.Quality01: img_bg.texture = texture_quality01; break;
            case GoodsQualityType.Quality02: img_bg.texture = texture_quality02; break;
            case GoodsQualityType.Quality03: img_bg.texture = texture_quality03; break;
            case GoodsQualityType.Quality04: img_bg.texture = texture_quality04; break;
            case GoodsQualityType.Quality05: img_bg.texture = texture_quality05; break;
            case GoodsQualityType.Quality06: img_bg.texture = texture_quality06; break;
        }
    }

    private void SetGoodsCustomizeTexture(TUIMeshSprite m_sprite, string m_path)
    {
        if (m_sprite == null) return;

        m_sprite.texture = string.Empty;
        m_sprite.UseCustomize = true;
        m_sprite.CustomizeTexture = Resources.Load(m_path) as Texture;

        if (m_sprite.CustomizeTexture == null)
        {
            Debug.LogWarning("Missing texture: " + m_path);
        }
        else
        {
            // Set fixed rect offset: X=0, Y=38, W=90, H=90
            m_sprite.CustomizeRect = new Rect(0f, 38f, 90f, 90f);
        }
    }

    public string GetGoodsName() => goods_name;
    public int GetGoodsID() => goods_id;
}
