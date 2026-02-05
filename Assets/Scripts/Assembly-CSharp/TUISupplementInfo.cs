using System.Collections.Generic;

public class TUISupplementInfo
{
	public List<TUIGoodsSupplementInfo> goods_list;

	public int price_value;

	public UnitType price_unit;

	public void AddSupplementGoods(TUIGoodsSupplementInfo m_info)
	{
		if (goods_list == null)
		{
			goods_list = new List<TUIGoodsSupplementInfo>();
		}
		goods_list.Add(m_info);
	}

	public void SetPriceInfo(int m_value, UnitType m_unit)
	{
		price_value = m_value;
		price_unit = m_unit;
	}
}
