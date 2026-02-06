using UnityEngine;

public class Btn_Select_Stash : MonoBehaviour
{
    public TUIMeshSprite img_quality;
    public TUIMeshSprite img_mask;
    public TUIMeshSprite img_texture;
    public TUILabel label_count;

    private TUIGoodsInfo goods_info;
    private int index;

    private string texture_path = "TUI/Goods/";

    private string texture_quality01 = "kuangdj_1";
    private string texture_quality02 = "kuangdj_2";
    private string texture_quality03 = "kuangdj_3";
    private string texture_quality04 = "kuangdj_4";
    private string texture_quality05 = "kuangdj_5";
    private string texture_quality06 = "kuangdj_6";

    private void Awake()
    {
        if (img_quality == null) Debug.LogError("no img_quality!");
        if (img_mask == null) Debug.LogError("no img_mask!");
        if (img_texture == null) Debug.LogError("no img_texture!");
        if (label_count == null) Debug.LogError("no label_count!");
        img_mask.gameObject.SetActiveRecursively(false);
        img_quality.gameObject.SetActiveRecursively(false);
    }

    public void SetIndex(int id) => index = id;
    public int GetIndex() => index;

    public void SetGoodsInfo(TUIGoodsInfo m_goods_info)
    {
        goods_info = m_goods_info;

        // Load texture like GoodsNeedItemImg
        string stashTexture = TUIMappingInfo.Instance().GetStashTexture(m_goods_info.id);
        SetGoodsCustomizeTexture(img_texture, texture_path + stashTexture);

        SetGoodsCount(m_goods_info.count);
        SetQualityTexture(goods_info.quality);
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
            m_sprite.CustomizeRect = new Rect(0f, 38f, 90f, 90f);
        }
    }

    public void SetQualityTexture(GoodsQualityType m_type)
    {
        img_quality.gameObject.SetActiveRecursively(true);
        switch (m_type)
        {
            case GoodsQualityType.Quality01: img_quality.texture = texture_quality01; break;
            case GoodsQualityType.Quality02: img_quality.texture = texture_quality02; break;
            case GoodsQualityType.Quality03: img_quality.texture = texture_quality03; break;
            case GoodsQualityType.Quality04: img_quality.texture = texture_quality04; break;
            case GoodsQualityType.Quality05: img_quality.texture = texture_quality05; break;
            case GoodsQualityType.Quality06: img_quality.texture = texture_quality06; break;
        }
    }

    public void SetGoodsCount(int m_count)
    {
        if (m_count == 0)
        {
            label_count.Text = string.Empty;
            img_mask.gameObject.SetActiveRecursively(true);
            img_mask.color = new Color(1f, 1f, 1f, 0.1f);
            img_texture.color = new Color(1f, 1f, 1f, 0.3f);
            img_quality.gameObject.SetActiveRecursively(true);
            img_quality.color = new Color(1f, 1f, 1f, 0.5f);
        }
        else
        {
            label_count.Text = m_count.ToString();
            img_mask.gameObject.SetActiveRecursively(false);
            img_texture.color = new Color(1f, 1f, 1f, 1f);
            img_quality.gameObject.SetActiveRecursively(true);
            img_quality.color = new Color(1f, 1f, 1f, 1f);
        }
    }

    public TUIGoodsInfo GetGoodsInfo() => goods_info;

    public void SetSellInfo(int m_count)
    {
        SetGoodsCount(m_count);
        goods_info.SetCount(m_count);
    }
}
