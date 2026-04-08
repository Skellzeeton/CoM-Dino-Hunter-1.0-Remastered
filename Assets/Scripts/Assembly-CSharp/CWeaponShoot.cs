using System.Collections.Generic;
using UnityEngine;

public class CWeaponShoot : CWeaponBase
{
	private const int EFF_FATAL_BOSS = 1904;
	private const float SMALL_AOE_RADIUS = 2.5f;
	private const float SMALL_AOE_DAMAGE_RATE = 0.166f;

	protected override void OnEquip(CCharPlayer player)
	{
		RefreshBulletUI();
	}

	private bool IsSmallAoEWeapon()
	{
		return m_pWeaponLvlInfo != null &&
			   m_pWeaponLvlInfo.nType == 0 &&
			   m_pWeaponLvlInfo.nElementType == 1;
	}

	protected override void OnFire(CCharPlayer player)
	{
		if (!player.IsCanAttack())
		{
			return;
		}

		if (base.IsBulletEmpty)
		{
			if (m_pWeaponLvlInfo != null)
			{
				switch (m_pWeaponLvlInfo.nType)
				{
				case 2:
					player.PlayAudio("Weapon_nobullet_gun");
					break;
				case 0:
					player.PlayAudio("Weapon_nobullet_crossbow");
					break;
				}
			}
			Stop(player);
			return;
		}

		ConsumeBullet();
		ShowFireLight(true);

		iGameUIBase gameUI = m_GameScene.GetGameUI();
		if (gameUI != null)
		{
			gameUI.ExpandAimCross();
		}

		player.PlayAnimMix(kAnimEnum.Attack, WrapMode.ClampForever, 1f);

		float fValue = 10000f;
		m_pWeaponLvlInfo.GetAtkModeValue(0, ref fValue);

		Vector3 shootMouse = player.GetShootMouse();
		Vector3 vector = player.m_v3CurNetAimDir;

		Ray ray;
		if (!base.isNetPlayerShoot)
		{
			ray = Camera.main.ScreenPointToRay(m_GameState.GetScreenCenterV3());
			vector = ray.direction;
		}
		else
		{
			ray = new Ray(shootMouse, vector);
		}

		RaycastHit hitInfo;
		if (!Physics.Raycast(ray, out hitInfo, fValue, -1543503872))
		{
			return;
		}

		float magnitude = (hitInfo.point - shootMouse).magnitude;
		if (magnitude > 5f)
		{
			m_GameScene.AddBulletTrack(player.GetShootMouse(), hitInfo.point, m_pWeaponLvlInfo.nBullet);
		}

		m_GameScene.AddFireEffect(player.GetShootMouseTf(), vector, m_pWeaponLvlInfo.nFire, 2f);
		player.PlayAudio(m_pWeaponLvlInfo.sAudioFire);
		m_GameScene.AddHitEffect(hitInfo.point, hitInfo.normal, m_pWeaponLvlInfo.nHit);

		CCharMob component = null;
		if (hitInfo.transform.gameObject.layer == 26)
		{
			component = hitInfo.transform.root.gameObject.GetComponent<CCharMob>();
		}

		if (IsSmallAoEWeapon())
		{
			ApplySmallAoE(player, hitInfo.point, component);
		}

		if (component == null || component.isDead)
		{
			return;
		}

		if (!base.isNetPlayerShoot)
		{
			OnHitMob(player, component, hitInfo.point, hitInfo.normal, hitInfo.transform.name);
		}
		switch (m_pWeaponLvlInfo.nElementType)
		{
			case 1:
				component.PlayAudio("Fx_Impact_flare");
				break;
			case 3:
				component.PlayAudio("Fx_Impact_freeze");
				break;
			case 2:
				component.PlayAudio("Fx_Impact_electric");
				break;
		}
	}

	private float GetAoERadius()
	{
		float radius = SMALL_AOE_RADIUS;

		if (m_GameScene != null && m_GameScene.CurGameLevelInfo != null)
		{
			if (m_GameScene.CurGameLevelInfo.bIsSkyScene)
			{
				radius *= 2f;
			}
		}

		return radius;
	}
	
	private void ApplySmallAoE(CCharPlayer player, Vector3 center, CCharMob primaryMob = null)
	{
		if (m_GameScene == null)
		{
			return;
		}

		List<CCharBase> unitList = m_GameScene.GetUnitList();
		if (unitList == null)
		{
			return;
		}

		for (int i = 0; i < unitList.Count; i++)
		{
			CCharBase unit = unitList[i];
			if (unit == null || unit == primaryMob)
			{
				continue;
			}

			if (!unit.IsMob() && !unit.IsBoss())
			{
				continue;
			}

			CCharMob mob = unit as CCharMob;
			if (mob == null || mob.isDead)
			{
				continue;
			}
			float radius = GetAoERadius();
			if (Vector3.Distance(center, mob.Pos) > radius)
			{
				continue;
			}
			if (player.IsAlly(mob))
			{
				continue;
			}
			Vector3 aoeDir = (mob.Pos - center).normalized;
			DealDamageToMob(player, mob, mob.Pos, aoeDir, string.Empty, SMALL_AOE_DAMAGE_RATE, false);
		}
	}

	private void DealDamageToMob(
		CCharPlayer player,
		CCharMob mob,
		Vector3 hitpos,
		Vector3 hitdir,
		string sBodyPart,
		float damageRate,
		bool playImpactEffect)
	{
		if (mob == null || mob.isDead)
		{
			return;
		}

		mob.SetLifeBarParam(3f);

		iGameUIBase gameUI = m_GameScene.GetGameUI();
		if (gameUI != null)
		{
			gameUI.ShootLifeBar(mob.UID);
		}

		float num = player.CalcWeaponDamage(m_pWeaponLvlInfo);
		num *= damageRate;

		float num2 = player.CalcCritical(m_pWeaponLvlInfo);
		float num3 = player.CalcCriticalDmg(m_pWeaponLvlInfo);
		bool bCritical = false;

		if (num2 > Random.Range(1f, 100f))
		{
			num *= 1f + num3 / 100f;
			bCritical = true;
		}

		float elementValue = m_pWeaponLvlInfo.GetElementValue(mob.ID);
		if (elementValue != 0f)
		{
			num *= 1f + elementValue / 100f;
		}

		float num4 = mob.CalcProtect();
		num *= 1f - num4 / 100f;

		mob.OnHit(0f - num, m_pWeaponLvlInfo, sBodyPart);
		mob.PlayAudio(kAudioEnum.HitBody);
		m_GameScene.AddDamageText(num, hitpos, bCritical);

		if (playImpactEffect)
		{
			base.m_GameScene.AddHitEffect(hitpos, Vector3.forward, 1115);
		}

		m_GameLogic = m_GameScene.GetGameLogic();
		if (m_GameLogic != null)
		{
			iGameLogic.HitInfo hitinfo = new iGameLogic.HitInfo();
			hitinfo.v3HitDir = hitdir;
			hitinfo.v3HitPos = hitpos;

			m_GameLogic.CaculateFunc(player, mob, m_pWeaponLvlInfo.arrFunc, m_pWeaponLvlInfo.arrValueX, m_pWeaponLvlInfo.arrValueY, ref hitinfo);
			m_GameLogic.ltDamageInfo.Add(num);
			CGameNetSender.GetInstance().BattleDamageMob(mob.UID, m_GameLogic.ltDamageInfo);
		}

		if (!mob.isDead)
		{
			return;
		}

		if (mob.IsBoss())
		{
			m_GameScene.AddEffect(mob.GetBone(1).position, Vector3.forward, 4.25f, EFF_FATAL_BOSS);
		}

		CMobInfoLevel mobInfo = mob.GetMobInfo();
		if (mobInfo != null)
		{
			int num5 = mobInfo.nExp;
			float value = player.Property.GetValue(kProEnum.Char_IncreaseExp);
			if (value > 0f)
			{
				num5 = (int)((float)num5 * (1f + value / 100f));
			}

			player.AddExp(num5);
			m_GameScene.AddExpText(num5, hitpos);
		}
	}

	protected override void OnStop(CCharPlayer player)
	{
	}

	protected override void OnUpdate(CCharPlayer player, float deltaTime)
	{
		if (m_fFireIntervalCount < m_fFireInterval)
		{
			m_fFireIntervalCount += deltaTime;
			if (m_fFireIntervalCount < m_fFireInterval)
			{
				return;
			}
		}

		if (m_bFire)
		{
			m_fFireIntervalCount = 0f;
			OnFire(player);
		}
	}

	protected override void OnHitMob(CCharPlayer player, CCharMob mob, Vector3 hitpos, Vector3 hitdir, string sBodyPart = "")
	{
		DealDamageToMob(player, mob, hitpos, hitdir, sBodyPart, 1f, true);
	}
}