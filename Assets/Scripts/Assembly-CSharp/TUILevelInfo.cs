using System.Collections.Generic;

public class TUILevelInfo
{
	public int id;

	public string introduce01 = string.Empty;

	public string introduce02 = string.Empty;

	public List<TUIGoodsInfo> goods_drop_list;

	public TUIRecommendRoleInfo recommend_role_info;

	public TUIRecommendWeaponInfo recommend_weapon_info;

	public string title = string.Empty;

	public TUILevelInfo(int m_id, string m_introduce01, string m_introduce02, List<TUIGoodsInfo> m_goods_drop_list, TUIRecommendRoleInfo m_recommend_role_info = null, TUIRecommendWeaponInfo m_recommend_weapon_info = null, string m_title = "")
	{
		id = m_id;
		introduce01 = m_introduce01;
		introduce02 = m_introduce02;
		goods_drop_list = m_goods_drop_list;
		recommend_role_info = m_recommend_role_info;
		recommend_weapon_info = m_recommend_weapon_info;
		title = m_title;
	}
}
