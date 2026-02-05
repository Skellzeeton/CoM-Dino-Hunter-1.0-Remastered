using System.Collections.Generic;

public class TUIWeaponUpdateInfo
{
	public Dictionary<int, TUIPriceInfo> level_price;

	public Dictionary<int, int> level_damage;

	public Dictionary<int, float> level_fire_rate;

	public Dictionary<int, int> level_blast_radius;

	public Dictionary<int, int> level_knockback;

	public Dictionary<int, int> level_ammo;

	public Dictionary<int, string> level_introduce;

	public Dictionary<int, int> level_hp;

	public Dictionary<int, string> level_introduce_ex;

	public TUIWeaponUpdateInfo(Dictionary<int, TUIPriceInfo> m_price, Dictionary<int, int> m_damage, Dictionary<int, float> m_fire_rate, Dictionary<int, int> m_blast_radius, Dictionary<int, int> m_knockback, Dictionary<int, int> m_ammo, Dictionary<int, string> m_introduce)
	{
		level_price = m_price;
		level_damage = m_damage;
		level_fire_rate = m_fire_rate;
		level_blast_radius = m_blast_radius;
		level_knockback = m_knockback;
		level_ammo = m_ammo;
		level_introduce = m_introduce;
	}

	public TUIWeaponUpdateInfo(Dictionary<int, TUIPriceInfo> m_price, Dictionary<int, string> m_introduce, Dictionary<int, int> m_hp, Dictionary<int, string> m_introduce_ex)
	{
		level_price = m_price;
		level_introduce = m_introduce;
		level_hp = m_hp;
		level_introduce_ex = m_introduce_ex;
	}
}
