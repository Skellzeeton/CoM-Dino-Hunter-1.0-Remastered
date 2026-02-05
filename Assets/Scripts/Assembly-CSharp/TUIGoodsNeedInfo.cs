public class TUIGoodsNeedInfo
{
	public int goods_id;

	public GoodsQualityType goods_quality;

	public int need_count;

	public string goods_name = string.Empty;

	public TUIGoodsNeedInfo(int id, GoodsQualityType m_goods_quality, int count, string m_goods_name)
	{
		goods_id = id;
		need_count = count;
		goods_quality = m_goods_quality;
		goods_name = m_goods_name;
	}
}
