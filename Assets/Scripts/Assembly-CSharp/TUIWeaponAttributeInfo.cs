using System.Collections.Generic;

public class TUIWeaponAttributeInfo
{
	public int id;

	public string name = string.Empty;

	public int level;

	public WeaponType kind = WeaponType.CloseWeapon;

	public TUIWeaponUpdateInfo weapon_update_info;

	public TUILevelGoodsNeedInfo level_goods_need_info;

	public Dictionary<int, TUIGoodsInfo> goods_list;

	public TUIWeaponAttributeInfo()
	{
	}

	public TUIWeaponAttributeInfo(WeaponType m_type, int m_id, string m_name, int m_level, TUIWeaponUpdateInfo m_weapon_update_info, TUILevelGoodsNeedInfo m_level_goods_need_info, Dictionary<int, TUIGoodsInfo> m_goods_list)
	{
		kind = m_type;
		id = m_id;
		name = m_name;
		level = m_level;
		weapon_update_info = m_weapon_update_info;
		level_goods_need_info = m_level_goods_need_info;
		goods_list = m_goods_list;
	}
}
