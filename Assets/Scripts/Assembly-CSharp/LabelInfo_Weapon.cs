using UnityEngine;

public class LabelInfo_Weapon : MonoBehaviour
{
	public TUILabel label_damage;

	public TUILabel label_damage_value;

	public TUILabel label_fire_rate;

	public TUILabel label_fire_rate_value;

	public TUILabel label_blast_radius;

	public TUILabel label_blast_radius_value;

	public TUILabel label_knockback;

	public TUILabel label_knockback_value;

	public TUILabel label_ammo;

	public TUILabel label_ammo_value;

	public TUILabel label_hp;

	public TUILabel label_hp_value;

	public TUILabel label_introduce;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void SetWeaponInfo(int m_damage, float m_fire_rate, int m_blast_radius, int m_knockback, int m_ammo)
	{
		label_damage.gameObject.SetActiveRecursively(true);
		label_damage_value.gameObject.SetActiveRecursively(true);
		label_fire_rate.gameObject.SetActiveRecursively(true);
		label_fire_rate_value.gameObject.SetActiveRecursively(true);
		label_blast_radius.gameObject.SetActiveRecursively(true);
		label_blast_radius_value.gameObject.SetActiveRecursively(true);
		label_knockback.gameObject.SetActiveRecursively(true);
		label_knockback_value.gameObject.SetActiveRecursively(true);
		label_ammo.gameObject.SetActiveRecursively(true);
		label_ammo_value.gameObject.SetActiveRecursively(true);
		label_hp.gameObject.SetActiveRecursively(false);
		label_hp_value.gameObject.SetActiveRecursively(false);
		label_introduce.gameObject.SetActiveRecursively(false);
		if (m_damage == 0)
		{
			label_damage_value.Text = "--";
		}
		else
		{
			label_damage_value.Text = m_damage.ToString();
		}
		if (m_fire_rate == 0f)
		{
			label_fire_rate_value.Text = "--";
		}
		else
		{
			label_fire_rate_value.Text = m_fire_rate.ToString();
		}
		if (m_blast_radius == 0)
		{
			label_blast_radius_value.Text = "--";
		}
		else
		{
			label_blast_radius_value.Text = m_blast_radius.ToString();
		}
		if (m_knockback == 0)
		{
			label_knockback_value.Text = "--";
		}
		else
		{
			label_knockback_value.Text = m_knockback.ToString();
		}
		if (m_ammo == 0)
		{
			label_ammo_value.Text = "--";
		}
		else
		{
			label_ammo_value.Text = m_ammo.ToString();
		}
	}

	public void SetWeaponInfo(float m_fire_rate, int m_blast_radius, int m_knockback, int m_ammo)
	{
		label_fire_rate.gameObject.SetActiveRecursively(true);
		label_fire_rate_value.gameObject.SetActiveRecursively(true);
		label_blast_radius.gameObject.SetActiveRecursively(true);
		label_blast_radius_value.gameObject.SetActiveRecursively(true);
		label_knockback.gameObject.SetActiveRecursively(true);
		label_knockback_value.gameObject.SetActiveRecursively(true);
		label_ammo.gameObject.SetActiveRecursively(true);
		label_ammo_value.gameObject.SetActiveRecursively(true);
		label_hp.gameObject.SetActiveRecursively(false);
		label_hp_value.gameObject.SetActiveRecursively(false);
		label_introduce.gameObject.SetActiveRecursively(false);
		if (m_fire_rate == 0f)
		{
			label_fire_rate_value.Text = "--";
		}
		else
		{
			label_fire_rate_value.Text = m_fire_rate.ToString();
		}
		if (m_blast_radius == 0)
		{
			label_blast_radius_value.Text = "--";
		}
		else
		{
			label_blast_radius_value.Text = m_blast_radius.ToString();
		}
		if (m_knockback == 0)
		{
			label_knockback_value.Text = "--";
		}
		else
		{
			label_knockback_value.Text = m_knockback.ToString();
		}
		if (m_ammo == 0)
		{
			label_ammo_value.Text = "--";
		}
		else
		{
			label_ammo_value.Text = m_ammo.ToString();
		}
	}

	public void SetStoneskinInfo(string m_introduce, int m_hp)
	{
		label_damage.gameObject.SetActiveRecursively(false);
		label_damage_value.gameObject.SetActiveRecursively(false);
		label_fire_rate.gameObject.SetActiveRecursively(false);
		label_fire_rate_value.gameObject.SetActiveRecursively(false);
		label_blast_radius.gameObject.SetActiveRecursively(false);
		label_blast_radius_value.gameObject.SetActiveRecursively(false);
		label_knockback.gameObject.SetActiveRecursively(false);
		label_knockback_value.gameObject.SetActiveRecursively(false);
		label_ammo.gameObject.SetActiveRecursively(false);
		label_ammo_value.gameObject.SetActiveRecursively(false);
		label_hp.gameObject.SetActiveRecursively(true);
		label_hp_value.gameObject.SetActiveRecursively(true);
		label_introduce.gameObject.SetActiveRecursively(true);
		if (m_hp == 0)
		{
			label_hp_value.Text = "--";
		}
		else
		{
			label_hp_value.Text = m_hp.ToString();
		}
		label_introduce.Text = m_introduce;
	}

	public void SetDamage(int m_damage)
	{
		label_damage.gameObject.SetActiveRecursively(true);
		label_damage_value.gameObject.SetActiveRecursively(true);
		label_damage_value.Text = m_damage.ToString();
	}

	public void SetHP(int m_hp)
	{
		label_hp.gameObject.SetActiveRecursively(true);
		label_hp_value.gameObject.SetActiveRecursively(true);
		label_hp_value.Text = m_hp.ToString();
	}

	public void OpenDamageAnimation()
	{
		if (label_damage_value != null && label_damage_value.GetComponent<Animation>() != null)
		{
			label_damage_value.GetComponent<Animation>().Play();
		}
	}

	public void OpenHPAnimation()
	{
		if (label_hp_value != null && label_hp_value.GetComponent<Animation>() != null)
		{
			label_hp_value.GetComponent<Animation>().Play();
		}
	}
}
