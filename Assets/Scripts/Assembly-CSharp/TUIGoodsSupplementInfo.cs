public class TUIGoodsSupplementInfo
{
	public int id;

	public int count;

	public GoodsQualityType quality_type;

	public TUIGoodsSupplementInfo()
	{
	}

	public TUIGoodsSupplementInfo(int m_id, int m_count, GoodsQualityType m_type)
	{
		id = m_id;
		count = m_count;
		quality_type = m_type;
	}
}
