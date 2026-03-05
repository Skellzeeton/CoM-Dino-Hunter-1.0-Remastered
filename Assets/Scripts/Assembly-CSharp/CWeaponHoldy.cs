using System;
using System.Collections.Generic;
using UnityEngine;

public class CWeaponHoldy : CWeaponBase
{
	protected float m_fRadius;

	protected float m_fAngle;

	protected float m_fEffectTime;

	protected float m_fEffectTimeCount;

	protected GameObject m_FireEffect;

	protected ParticleSystem[] m_arrParticleSystem;

	protected override void OnEquip(CCharPlayer player)
	{
		if (m_FireEffect != null || m_pWeaponLvlInfo == null || player == null)
		{
			return;
		}
		RefreshBulletUI();
		GameObject gameObject = PrefabManager.Get(m_pWeaponLvlInfo.nFire);
		if (gameObject == null)
		{
			return;
		}
		m_FireEffect = (GameObject)UnityEngine.Object.Instantiate(gameObject);
		if (m_FireEffect == null)
		{
			return;
		}
		m_FireEffect.transform.parent = player.GetShootMouseTf();
		m_FireEffect.transform.localPosition = Vector3.zero;
		m_FireEffect.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
		m_arrParticleSystem = m_FireEffect.GetComponentsInChildren<ParticleSystem>();
		if (m_arrParticleSystem != null)
		{
			ParticleSystem[] arrParticleSystem = m_arrParticleSystem;
			foreach (ParticleSystem particleSystem in arrParticleSystem)
			{
				particleSystem.enableEmission = false;
			}
		}
	}

	protected override void OnDestroy()
	{
		if (m_FireEffect != null)
		{
			UnityEngine.Object.Destroy(m_FireEffect);
			m_FireEffect = null;
		}
	}

	protected override void OnFire(CCharPlayer player)
	{
		if (!player.IsCanAttack())
		{
			return;
		}
		m_fFireLightTime = 1.5f;
		player.PlayAnimMix(kAnimEnum.Attack, WrapMode.Loop, 1f);
		player.PlayAudio(m_pWeaponLvlInfo.sAudioFire);
		if (m_arrParticleSystem != null)
		{
			ParticleSystem[] arrParticleSystem = m_arrParticleSystem;
			foreach (ParticleSystem particleSystem in arrParticleSystem)
			{
				particleSystem.enableEmission = true;
			}
		}
		m_fRadius = 0f;
		m_fAngle = 0f;
		m_fEffectTime = 0.5f;
		m_pWeaponLvlInfo.GetAtkModeValue(0, ref m_fRadius);
		m_pWeaponLvlInfo.GetAtkModeValue(1, ref m_fAngle);
		m_pWeaponLvlInfo.GetAtkModeValue(2, ref m_fEffectTime);
		m_fEffectTimeCount = m_fEffectTime;
	}

	protected override void OnStop(CCharPlayer player)
	{
		player.StopAction(kAnimEnum.Attack);
		player.StopAudio(m_pWeaponLvlInfo.sAudioFire);
		player.PlayAudio("Weapon_flame_end");
		if (m_arrParticleSystem != null)
		{
			ParticleSystem[] arrParticleSystem = m_arrParticleSystem;
			foreach (ParticleSystem particleSystem in arrParticleSystem)
			{
				particleSystem.enableEmission = false;
			}
		}
	}

	protected override void OnUpdate(CCharPlayer player, float deltaTime)
	{
		if (m_fFireIntervalCount < m_fFireInterval)
		{
			m_fFireIntervalCount += deltaTime;
		}
		if (!m_bFire)
		{
			return;
		}
		m_fEffectTimeCount += deltaTime;
		if (m_fEffectTimeCount < m_fEffectTime)
		{
			return;
		}
		m_fEffectTimeCount = 0f;
		if (base.IsBulletEmpty)
		{
			player.PlayAudio("Weapon_nobullet_flamethrower");
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
		Dictionary<int, CCharMob> mobData = m_GameScene.GetMobData();
		foreach (CCharMob value in mobData.Values)
		{
			if (value.isDead)
			{
				continue;
			}
			Vector3 vector = value.Pos - player.Pos;
			if (vector.sqrMagnitude > m_fRadius * m_fRadius)
			{
				continue;
			}
			if (m_fRadius < 2f)
			{
				if (m_fAngle > 0f)
				{
					vector.y = 0f;
					if (Vector3.Dot(player.Dir2D, vector.normalized) <= 0f)
					{
						continue;
					}
				}
			}
			else if (m_fAngle > 0f)
			{
				vector.y = 0f;
				if (Vector3.Dot(player.Dir2D, vector.normalized) < Mathf.Cos(m_fAngle * ((float)Math.PI / 180f) / 2f))
				{
					continue;
				}
			}
			Vector3 vector2 = value.Pos - player.Pos;
			Vector3 bloodPos = value.GetBloodPos(player.GetUpBodyPos() + new Vector3(0f, 0.7f, 0f), vector2);
			m_GameScene.AddHitEffect(bloodPos, vector2, m_pWeaponLvlInfo.nHit);
			if (!base.isNetPlayerShoot)
			{
				OnHitMob(player, value, bloodPos, vector2, string.Empty);
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
		base.m_GameScene.AddHitEffect(hitpos, Vector3.forward, 1115);
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
