public class TUIWeaponAttribute
{
	public float damage;

	public float fire_rate;

	public float blast_radius;

	public float knockback;

	public float ammo;

	public TUIWeaponAttribute()
	{
	}

	public TUIWeaponAttribute(float m_damage = 0f, float m_fire_rate = 0f, float m_blast_radius = 0f, float m_knockback = 0f, float m_ammo = 0f)
	{
		damage = m_damage;
		fire_rate = m_fire_rate;
		blast_radius = m_blast_radius;
		knockback = m_knockback;
		ammo = m_ammo;
	}
}
