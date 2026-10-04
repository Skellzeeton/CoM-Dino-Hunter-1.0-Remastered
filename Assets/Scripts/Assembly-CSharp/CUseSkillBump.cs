using System.Collections.Generic;
using UnityEngine;

public class CUseSkillBump : CUseSkill
{
	protected float m_fBumpDis;

	protected float m_fBumpTime = 1f;

	protected float m_fBumpTimeCount;

	protected float m_fBumpFuncTime;

	protected float m_fBumpFuncTimeCount;

	protected Vector3 m_v3Src;

	protected Vector3 m_v3Dst;

	protected float m_fSpeed;

	protected iRushEffect m_RushEffect;

	private const float BumpCollisionSkin = 0.02f;

	private CSkillInfoLevel m_pScaledSkillInfoLevel;

	private readonly HashSet<int> m_hitTargets = new HashSet<int>();

	public override kUseSkillStatus OnEnter(CCharBase charbase)
	{
		charbase.m_bBumping = true;
		m_pSkillInfoLevel.GetSkillModeValue(0, ref m_fBumpDis);
		m_pSkillInfoLevel.GetSkillModeValue(1, ref m_fBumpTime);
		m_pSkillInfoLevel.GetSkillModeValue(2, ref m_fBumpFuncTime);
		CCharMob bumpMob = charbase as CCharMob;
		if (bumpMob != null)
		{
			float slowMult = CWeaponBurnManager.Instance.GetMoveSpeedMultiplier(bumpMob);
			if (slowMult > 0f && slowMult < 1f)
				m_fBumpTime /= slowMult;
		}
		m_fBumpFuncTimeCount = 0f;
		m_v3Src = charbase.Pos;
		m_v3Dst = charbase.Pos + charbase.Dir2D * m_fBumpDis;
		Ray ray = new Ray(charbase.GetBone(1).position, charbase.Dir2D);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, m_fBumpDis, -1879048192))
		{
			m_fBumpDis = Mathf.Max(
					0f,
					Vector3.Distance(ray.origin, hitInfo.point) - 2f
			);
			m_v3Dst = charbase.Pos + charbase.Dir2D * m_fBumpDis;
		}
		m_fSpeed = m_fBumpDis / m_fBumpTime;
		float speed = 1f;
		switch (charbase.CharType)
		{
		case kCharType.Mob:
		case kCharType.Boss:
		{
			CCharMob cCharMob = charbase as CCharMob;
			if (!(cCharMob != null))
			{
				break;
			}
			CMobInfoLevel mobInfo = cCharMob.GetMobInfo();
			if (mobInfo != null)
			{
				if (m_pSkillInfoLevel.nAnim == 4)
				{
					speed = m_fSpeed * mobInfo.fMoveSpeedRate;
				}
				else if (m_pSkillInfoLevel.nAnim == 2504)
				{
					speed = m_fSpeed * mobInfo.fRushSpeedRate;
				}
			}
			break;
		}
		case kCharType.Player:
			if (m_pSkillInfoLevel.nAnim == 4)
			{
				speed = m_fSpeed * 0.3f;
			}
			break;
		case kCharType.User:
		{
			CCharUser cCharUser = charbase as CCharUser;
			if (!(cCharUser != null))
			{
				break;
			}
			if (m_pSkillInfoLevel.nAnim == 4)
			{
				speed = m_fSpeed * 0.3f;
			}
			cCharUser.MoveStop();
			cCharUser.SetFire(false);
			CWeaponInfoLevel curWeaponLvlInfo = cCharUser.GetCurWeaponLvlInfo();
			if (curWeaponLvlInfo != null && curWeaponLvlInfo.nType != 1)
			{
				iCameraTrail camera = m_GameScene.GetCamera();
				if (camera != null)
				{
					camera.SetViewMelee(true);
				}
			}
			break;
		}
		}
		charbase.PlayAnim((kAnimEnum)m_pSkillInfoLevel.nAnim, WrapMode.Loop, speed, 0f);
		if (charbase.Entity != null)
		{
			m_RushEffect = charbase.Entity.GetComponent<iRushEffect>();
			if (m_RushEffect != null)
			{
				m_RushEffect.iRushEffect_PlayEffect();
			}
		}
		//Debug.Log(charbase.UID + " start bump state");
		m_hitTargets.Clear();
		m_pScaledSkillInfoLevel = BuildLevelScaledSkillInfo(charbase);
		if (m_pSkillInfoLevel.sUseAudio.Length > 0)
		{
			charbase.PlayAudio(m_pSkillInfoLevel.sUseAudio);
		}
		return kUseSkillStatus.Success;
	}

	public override void OnExit(CCharBase charbase)
	{
		charbase.m_bBumping = false;
		//Debug.Log(charbase.UID + " out of the bump state");
		if (m_RushEffect != null)
		{
			//Debug.Log(charbase.UID + " stop effect");
			m_RushEffect.iRushEffect_StopEffect(true);
		}
		if (!charbase.IsPlayer() && !charbase.IsUser())
		{
			return;
		}
		charbase.CrossAnim(kAnimEnum.Idle, WrapMode.Loop, 0.3f, 1f, 0f);
		CCharUser cCharUser = charbase as CCharUser;
		if (!(cCharUser != null))
		{
			return;
		}
		CWeaponInfoLevel curWeaponLvlInfo = cCharUser.GetCurWeaponLvlInfo();
		if (curWeaponLvlInfo != null && curWeaponLvlInfo.nType != 1)
		{
			iCameraTrail camera = m_GameScene.GetCamera();
			if (camera != null)
			{
				camera.SetViewMelee(false);
			}
		}
	}

	public override kUseSkillStatus OnUpdate(CCharBase charbase, float deltaTime)
	{
		m_fBumpFuncTimeCount += deltaTime;
		if (m_fBumpFuncTimeCount >= m_fBumpFuncTime)
		{
			m_fBumpFuncTimeCount = 0f;
			SkillEffect(charbase, m_Target);
		}
		if (m_fBumpTimeCount < m_fBumpTime)
		{
			m_fBumpTimeCount += deltaTime;
			float t = Mathf.Clamp01(m_fBumpTimeCount / m_fBumpTime);
			Vector3 nextPos = Vector3.Lerp(m_v3Src, m_v3Dst, t);
			if (charbase.IsPlayer() || charbase.IsUser())
			{
				nextPos = ClampBumpPositionForPlayer(charbase, nextPos);
			}
			charbase.Pos = nextPos;
			if (m_fBumpTimeCount >= m_fBumpTime)
			{
				if (m_RushEffect != null)
				{
					m_RushEffect.iRushEffect_StopEffect();
				}
				return kUseSkillStatus.Success;
			}
		}
		return kUseSkillStatus.Executing;
	}

	private Vector3 ClampBumpPositionForPlayer(CCharBase charbase, Vector3 desiredPos)
	{
		Vector3 currentPos = charbase.Pos;
		Vector3 delta = desiredPos - currentPos;
		delta.y = 0f;
		float dist = delta.magnitude;
		if (dist < 0.0001f)
			return desiredPos;
		Vector3 dir = delta / dist;
		float radius = GetBumpRadius(charbase);
		Vector3 origin = charbase.GetBone(1).position;
		RaycastHit hit;
		int mask = -1879048192;
		if (Physics.SphereCast(
				origin,
				radius,
				dir,
				out hit,
				dist,
				mask,
				QueryTriggerInteraction.Ignore))
		{
			float allowedDistance = Mathf.Max(0f, hit.distance - BumpCollisionSkin);
			return currentPos + dir * allowedDistance;
		}
		return desiredPos;
	}

	private float GetBumpRadius(CCharBase charbase)
	{
		if (charbase.Entity == null)
			return 0.5f;
		CharacterController cc = charbase.Entity.GetComponent<CharacterController>();
		if (cc != null)
			return Mathf.Max(0.05f, cc.radius);
		CapsuleCollider capsule = charbase.Entity.GetComponent<CapsuleCollider>();
		if (capsule != null)
			return Mathf.Max(0.05f, capsule.radius);
		Collider col = charbase.Entity.GetComponent<Collider>();
		if (col != null)
		{
			return Mathf.Max(
					0.05f,
					Mathf.Max(col.bounds.extents.x, col.bounds.extents.z)
			);
		}
		return 0.5f;
	}

	protected override void SkillEffect(CCharBase actor, CCharBase target = null)
	{
		CSkillInfoLevel skillInfo = m_pScaledSkillInfoLevel ?? m_pSkillInfoLevel;
		switch (m_pSkillInfoLevel.nRangeType)
		{
			case 0:
			{
				if (target == null || target.isDead || !m_GameLogic.IsSkillCanUse(actor, target, m_pSkillInfoLevel))
					break;
				if (m_hitTargets.Contains(target.UID))
					break;
				Vector3 bloodPos2 = target.GetBloodPos(actor.GetBone(1).position, target.Pos - actor.Pos);
				iGameLogic.HitInfo hitinfo2 = new iGameLogic.HitInfo();
				hitinfo2.v3HitDir = (target.Pos - actor.Pos).normalized;
				hitinfo2.v3HitPos = bloodPos2;
				hitinfo2.weaponinfolevel = BuildBumpWeaponInfo(actor);
				hitinfo2.isPlayerSkill = true;
				m_GameLogic.Skill(skillInfo, actor, target, ref hitinfo2);
				m_hitTargets.Add(target.UID);
				GrantExpOnKill(actor, target, hitinfo2.v3HitPos);
				if (m_GameScene.IsRoomMaster())
				{
					if (target.IsMonster())
						CGameNetSender.GetInstance().BattleDamageMob(target.UID, m_GameLogic.ltDamageInfo);
					else if (target.IsUser())
						CGameNetSender.GetInstance().BattleDamagePlayer(m_GameLogic.ltDamageInfo);
				}
				target.PlayAudio(kAudioEnum.HitBody);
				break;
			}
			case 1:
			{
				int num = 0;
				int nValue = 0;
				m_pSkillInfoLevel.GetSkillRangeValue(3, ref nValue);
				List<CCharBase> unitList = m_GameScene.GetUnitList();
				for (int i = 0; i < unitList.Count; i++)
				{
					target = unitList[i];
					if (actor.IsAlly(target))
						continue;
					if (nValue > 0 && num >= nValue)
						break;
					if (target.isDead || !m_GameLogic.IsSkillCanUse(actor, target, m_pSkillInfoLevel))
						continue;
					if (m_hitTargets.Contains(target.UID))
						continue;
					Vector3 bloodPos = target.GetBloodPos(actor.GetBone(1).position, target.Pos - actor.Pos);
					iGameLogic.HitInfo hitinfo = new iGameLogic.HitInfo();
					hitinfo.v3HitDir = (target.Pos - actor.Pos).normalized;
					hitinfo.v3HitPos = bloodPos;
					hitinfo.weaponinfolevel = BuildBumpWeaponInfo(actor);
					hitinfo.isPlayerSkill = true;
					m_GameLogic.Skill(skillInfo, actor, target, ref hitinfo);
					m_hitTargets.Add(target.UID);
					GrantExpOnKill(actor, target, hitinfo.v3HitPos);
					if (m_GameScene.IsRoomMaster())
					{
						if (target.IsMonster())
							CGameNetSender.GetInstance().BattleDamageMob(target.UID, m_GameLogic.ltDamageInfo);
						else if (target.IsUser())
							CGameNetSender.GetInstance().BattleDamagePlayer(m_GameLogic.ltDamageInfo);
					}
					target.PlayAudio(kAudioEnum.HitBody);
					num++;
				}
				break;
			}
		}
	}

	private void GrantExpOnKill(CCharBase actor, CCharBase target, Vector3 hitPos)
	{
		if (target == null || !target.isDead)
			return;
		CCharMob mob = target as CCharMob;
		if (mob == null)
			return;
		CMobInfoLevel mobInfo = mob.GetMobInfo();
		if (mobInfo == null)
			return;
		CCharPlayer player = actor as CCharPlayer;
		if (player == null)
			return;
		int nExp = mobInfo.nExp;
		float expBonus = player.Property.GetValue(kProEnum.Char_IncreaseExp);
		if (expBonus > 0f)
			nExp = (int)((float)nExp * (1f + expBonus / 100f));
		player.AddExp(nExp);
		m_GameScene.AddExpText(nExp, hitPos);
	}

	private CSkillInfoLevel BuildLevelScaledSkillInfo(CCharBase actor)
	{
		CCharPlayer player = actor as CCharPlayer;
		if (player == null)
			return m_pSkillInfoLevel;
		int level = player.Level;
		if (level <= 1)
			return m_pSkillInfoLevel;
		var cloneMethod = typeof(object).GetMethod(
				"MemberwiseClone",
				System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		if (cloneMethod == null)
			return m_pSkillInfoLevel;
		CSkillInfoLevel clone = cloneMethod.Invoke(m_pSkillInfoLevel, null) as CSkillInfoLevel;
		if (clone == null)
			return m_pSkillInfoLevel;
		if (m_pSkillInfoLevel.arrFunc != null)
			clone.arrFunc = (int[])m_pSkillInfoLevel.arrFunc.Clone();
		if (m_pSkillInfoLevel.arrValueX != null)
			clone.arrValueX = (int[])m_pSkillInfoLevel.arrValueX.Clone();
		if (m_pSkillInfoLevel.arrValueY != null)
			clone.arrValueY = (int[])m_pSkillInfoLevel.arrValueY.Clone();
		if (clone.arrFunc == null || clone.arrValueX == null)
			return clone;
		int count = Mathf.Min(clone.arrFunc.Length, clone.arrValueX.Length);
		for (int i = 0; i < count; i++)
		{
			if (clone.arrFunc[i] == 2)
			{
				clone.arrValueX[i] = clone.arrValueX[i] * level;
			}
		}
		return clone;
	}

	private CWeaponInfoLevel BuildBumpWeaponInfo(CCharBase actor)
	{
		CWeaponInfoLevel info = new CWeaponInfoLevel();
		info.nAttackMode = 4;
		info.fCritical    = -100000f;
		info.fCriticalDmg = 0f;
		return info;
	}
}
