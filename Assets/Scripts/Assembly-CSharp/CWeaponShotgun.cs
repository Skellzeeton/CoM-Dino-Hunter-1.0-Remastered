using System;
using System.Collections.Generic;
using UnityEngine;

public class CWeaponShotgun : CWeaponBase
{
	protected override void OnEquip(CCharPlayer player)
	{
		RefreshBulletUI();
	}

	protected override void OnFire(CCharPlayer player)
	{
		if (!player.IsCanAttack())
		{
			return;
		}
		if (base.IsBulletEmpty)
		{
			player.PlayAudio("Weapon_nobullet_gun");
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
		float actionLen = player.GetActionLen(kAnimEnum.Attack);
		if (actionLen > m_fFireInterval)
		{
			actionLen = player.PlayAnimMix(kAnimEnum.Attack, WrapMode.ClampForever, actionLen / m_fFireInterval);
		}
		else
		{
			actionLen = player.PlayAnimMix(kAnimEnum.Attack, WrapMode.ClampForever, 1f);
		}
		float fValue = 10000f;
		float fValue2 = 0f;
		m_pWeaponLvlInfo.GetAtkModeValue(0, ref fValue);
		m_pWeaponLvlInfo.GetAtkModeValue(1, ref fValue2);
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
		if (Physics.Raycast(ray, out hitInfo, fValue, -1610612736))
		{
			m_GameScene.AddHitEffect(hitInfo.point, hitInfo.normal, m_pWeaponLvlInfo.nHit);
		}
		m_GameScene.AddFireEffect(player.GetShootMouseTf(), vector, m_pWeaponLvlInfo.nFire, 2f);
		player.PlayAudio(m_pWeaponLvlInfo.sAudioFire);
		Dictionary<int, CCharMob> mobData = m_GameScene.GetMobData();
		foreach (CCharMob value in mobData.Values)
		{
			if (value.isDead)
			{
				continue;
			}
			Vector3 vector2 = value.Pos - player.Pos;
			float sqrMagnitude = vector2.sqrMagnitude;
			if (sqrMagnitude > fValue * fValue)
			{
				continue;
			}
			if (sqrMagnitude < 2f)
			{
				if (fValue2 > 0f)
				{
					vector2.y = 0f;
					if (Vector3.Dot(player.Dir2D, vector2.normalized) <= 0f)
					{
						continue;
					}
				}
			}
			else if (fValue2 > 0f)
			{
				vector2.y = 0f;
				if (Vector3.Dot(player.Dir2D, vector2.normalized) < Mathf.Cos(fValue2 * ((float)Math.PI / 180f) / 2f))
				{
					continue;
				}
			}
			Vector3 vector3 = value.Pos - player.Pos;
			Vector3 bloodPos = value.GetBloodPos(player.GetUpBodyPos() + new Vector3(0f, 0.7f, 0f), vector3);
			switch (m_pWeaponLvlInfo.nHit)
			{
			case 1103:
				m_GameScene.AddHitEffect(bloodPos, vector3, 1100);
				break;
			case 1104:
				m_GameScene.AddHitEffect(bloodPos, vector3, 1101);
				break;
			case 1105:
				m_GameScene.AddHitEffect(bloodPos, vector3, 1102);
				break;
			default:
				m_GameScene.AddHitEffect(bloodPos, vector3, 1110);
				break;
			}
			m_GameScene.ShakeCamera(0.2f, 0.1f);
			if (!base.isNetPlayerShoot)
			{
				OnHitMob(player, value, bloodPos, vector3, string.Empty);
			}
			value.PlayAudio(kAudioEnum.HitBody);
			switch (m_pWeaponLvlInfo.nElementType)
			{
			case 1:
				value.PlayAudio("Fx_Impact_fire");
				break;
			case 3:
				value.PlayAudio("Fx_Impact_freeze");
				break;
			case 2:
				value.PlayAudio("Fx_Impact_electric");
				break;
			}
		}
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
		mob.SetLifeBarParam(1f);
		float num = player.CalcWeaponDamage(m_pWeaponLvlInfo);
		float num2 = player.CalcCritical(m_pWeaponLvlInfo);
		float num3 = player.CalcCriticalDmg(m_pWeaponLvlInfo);
		bool bCritical = false;
		if (num2 > UnityEngine.Random.Range(1f, 100f))
		{
			num *= 1f + num3 / 100f;
			bCritical = true;
		}
		float num4 = mob.CalcProtect();
		num *= 1f - num4 / 100f;
		mob.OnHit(0f - num, m_pWeaponLvlInfo, string.Empty);
		m_GameScene.AddDamageText(num, hitpos, bCritical);
		iGameLogic.HitInfo hitinfo = new iGameLogic.HitInfo();
		hitinfo.v3HitDir = hitdir;
		hitinfo.v3HitPos = hitpos;
		m_GameLogic = m_GameScene.GetGameLogic();
		if (m_GameLogic != null)
		{
			m_GameLogic.CaculateFunc(player, mob, m_pWeaponLvlInfo.arrFunc, m_pWeaponLvlInfo.arrValueX, m_pWeaponLvlInfo.arrValueY, ref hitinfo);
			m_GameLogic.ltDamageInfo.Add(num);
			CGameNetSender.GetInstance().BattleDamageMob(mob.UID, m_GameLogic.ltDamageInfo);
		}
		if (!mob.isDead)
		{
			return;
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
			m_GameScene.AddExpText(num5, hitinfo.v3HitPos);
		}
	}
}
