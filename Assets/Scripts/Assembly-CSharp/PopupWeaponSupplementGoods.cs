using UnityEngine;

public class PopupWeaponSupplementGoods : MonoBehaviour
{
    public TUIMeshSprite img_goods01;
    public TUIMeshSprite img_goods02;
    public TUIMeshSprite img_goods03;

    public TUIMeshSprite img_goods_bg01;
    public TUIMeshSprite img_goods_bg02;
    public TUIMeshSprite img_goods_bg03;

    public TUILabel label_goods01;
    public TUILabel label_goods02;
    public TUILabel label_goods03;

    public TUIMeshSprite img_price_unit;
    public TUILabel label_price_value;

    private string goods_texture_path = "TUI/Goods/";

    private string gold_texture = "title_jingbi";
    private string crystal_texture = "title_shuijing";

    private string texture_quality01 = "kuangdj_1";
    private string texture_quality02 = "kuangdj_2";
    private string texture_quality03 = "kuangdj_3";
    private string texture_quality04 = "kuangdj_4";
    private string texture_quality05 = "kuangdj_5";
    private string texture_quality06 = "kuangdj_6";

    public void SetGoodsInfo(int m_index, int m_goods_id, int m_goods_value, GoodsQualityType m_type)
    {
        string stashTexture = TUIMappingInfo.Instance().GetStashTexture(m_goods_id);
        string texturePath = goods_texture_path + stashTexture;

        switch (m_index)
        {
            case 1:
                SetGoodsCustomizeTexture(img_goods01, texturePath);
                if (label_goods01 != null)
                    label_goods01.Text = m_goods_value.ToString();
                if (img_goods_bg01 != null)
                    img_goods_bg01.texture = GetQualityType(m_type);
                break;

            case 2:
                SetGoodsCustomizeTexture(img_goods02, texturePath);
                if (label_goods02 != null)
                    label_goods02.Text = m_goods_value.ToString();
                if (img_goods_bg02 != null)
                    img_goods_bg02.texture = GetQualityType(m_type);
                break;

            case 3:
                SetGoodsCustomizeTexture(img_goods03, texturePath);
                if (label_goods03 != null)
                    label_goods03.Text = m_goods_value.ToString();
                if (img_goods_bg03 != null)
                    img_goods_bg03.texture = GetQualityType(m_type);
                break;

            default:
                Debug.LogWarning("Invalid goods index: " + m_index);
                break;
        }
    }
    
    public void SetPriceInfo(int m_price, UnitType m_unit)
    {
        if (img_price_unit == null || label_price_value == null)
            return;

        switch (m_unit)
        {
            case UnitType.Crystal:
                img_price_unit.texture = crystal_texture;
                img_price_unit.transform.localPosition =
                    new Vector3(img_price_unit.transform.localPosition.x, -44f, img_price_unit.transform.localPosition.z);
                break;

            case UnitType.Gold:
                img_price_unit.texture = gold_texture;
                img_price_unit.transform.localPosition =
                    new Vector3(img_price_unit.transform.localPosition.x, -44f, img_price_unit.transform.localPosition.z);
                break;
        }

        label_price_value.Text = m_price.ToString();
        label_price_value.transform.localPosition =
            new Vector3(label_price_value.transform.localPosition.x, -44f, label_price_value.transform.localPosition.z);
    }

    public void SetOnlyPriceInfo(int m_price, UnitType m_unit)
    {
        if (img_price_unit == null || label_price_value == null)
            return;

        switch (m_unit)
        {
            case UnitType.Crystal:
                img_price_unit.texture = crystal_texture;
                break;

            case UnitType.Gold:
                img_price_unit.texture = gold_texture;
                break;
        }

        img_price_unit.transform.localPosition =
            new Vector3(img_price_unit.transform.localPosition.x, 0f, img_price_unit.transform.localPosition.z);

        label_price_value.Text = m_price.ToString();
        label_price_value.transform.localPosition =
            new Vector3(label_price_value.transform.localPosition.x, 0f, label_price_value.transform.localPosition.z);
    }
    
    public void ClearInfo()
    {
        ClearGoodsSprite(img_goods01);
        ClearGoodsSprite(img_goods02);
        ClearGoodsSprite(img_goods03);

        if (label_goods01 != null) label_goods01.Text = string.Empty;
        if (label_goods02 != null) label_goods02.Text = string.Empty;
        if (label_goods03 != null) label_goods03.Text = string.Empty;

        if (img_price_unit != null) img_price_unit.texture = string.Empty;
        if (label_price_value != null) label_price_value.Text = string.Empty;

        if (img_goods_bg01 != null) img_goods_bg01.texture = string.Empty;
        if (img_goods_bg02 != null) img_goods_bg02.texture = string.Empty;
        if (img_goods_bg03 != null) img_goods_bg03.texture = string.Empty;
    }

    private void ClearGoodsSprite(TUIMeshSprite sprite)
    {
        if (sprite == null) return;

        sprite.texture = string.Empty;
        sprite.UseCustomize = false;
        sprite.CustomizeTexture = null;
        sprite.CustomizeRect = new Rect(0f, 0f, 0f, 0f);
    }
    
    private void SetGoodsCustomizeTexture(TUIMeshSprite m_sprite, string m_path)
    {
        if (m_sprite == null) return;

        m_sprite.texture = string.Empty;
        m_sprite.UseCustomize = true;
        m_sprite.CustomizeTexture = Resources.Load(m_path) as Texture;

        if (m_sprite.CustomizeTexture == null)
        {
            Debug.LogWarning("Missing goods texture: " + m_path);
        }
        else
        {
            m_sprite.CustomizeRect = new Rect(0f, 38f, 90f, 90f);
        }
    }
    
    private string GetQualityType(GoodsQualityType m_type)
    {
        switch (m_type)
        {
            case GoodsQualityType.Quality01: return texture_quality01;
            case GoodsQualityType.Quality02: return texture_quality02;
            case GoodsQualityType.Quality03: return texture_quality03;
            case GoodsQualityType.Quality04: return texture_quality04;
            case GoodsQualityType.Quality05: return texture_quality05;
            case GoodsQualityType.Quality06: return texture_quality06;
            default: return texture_quality01;
        }
    }
}
