using System.Collections.Generic;
using UnityEngine;

public abstract class CUseSkill
{
	protected iGameSceneBase m_GameScene;

	protected iGameLogic m_GameLogic;

	protected CSkillInfoLevel m_pSkillInfoLevel;

	protected CCharBase m_Target;

	protected GameObject m_SkillEffect;

	public virtual void Initialize(CSkillInfoLevel pSkillInfoLevel, CCharBase target = null)
	{
		m_GameScene = iGameApp.GetInstance().m_GameScene;
		m_GameLogic = m_GameScene.GetGameLogic();
		m_pSkillInfoLevel = pSkillInfoLevel;
		m_Target = target;
	}

	public abstract kUseSkillStatus OnEnter(CCharBase charbase);

	public abstract kUseSkillStatus OnUpdate(CCharBase charbase, float deltaTime);

	public abstract void OnExit(CCharBase charbase);

	public bool IsSkillValid()
	{
		if (m_pSkillInfoLevel == null)
		{
			return false;
		}
		if (m_pSkillInfoLevel.nRangeType == 0 && m_Target == null)
		{
			return false;
		}
		if (m_Target != null && (m_Target.isDead || m_Target.isStealth))
		{
			return false;
		}
		return true;
	}

	public bool IsAboutMyself(CCharBase actor, CCharBase target)
	{
		if (actor.IsUser() || target.IsUser())
		{
			return true;
		}
		return false;
	}

	public bool IsBroadcast(CCharBase actor, CCharBase target)
	{
		if (TNetManager.GetInstance().Connection == null)
		{
			return false;
		}
		if (IsAboutMyself(actor, target))
		{
			return true;
		}
		if (actor.IsMonster() && m_GameScene.IsRoomMaster())
		{
			return true;
		}
		return false;
	}

	protected virtual void SkillEffect(CCharBase actor, CCharBase target = null)
	{
		switch (m_pSkillInfoLevel.nRangeType)
		{
		case 0:
		{
			if (target == null || target.isDead || !m_GameLogic.IsSkillCanUse(actor, target, m_pSkillInfoLevel))
			{
				break;
			}
			Vector3 bloodPos2 = m_Target.GetBloodPos(actor.GetBone(1).position, target.Pos - actor.Pos);
			iGameLogic.HitInfo hitinfo2 = new iGameLogic.HitInfo();
			hitinfo2.v3HitDir = (m_Target.Pos - actor.Pos).normalized;
			hitinfo2.v3HitPos = bloodPos2;
			m_GameLogic.Skill(m_pSkillInfoLevel, actor, target, ref hitinfo2);
			if (m_GameScene.IsRoomMaster())
			{
				if (target.IsMonster())
				{
					CGameNetSender.GetInstance().BattleDamageMob(target.UID, m_GameLogic.ltDamageInfo);
				}
				else if (target.IsUser())
				{
					CGameNetSender.GetInstance().BattleDamagePlayer(m_GameLogic.ltDamageInfo);
				}
			}
			m_Target.PlayAudio(kAudioEnum.HitBody);
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
				{
					continue;
				}
				if (nValue > 0 && num >= nValue)
				{
					break;
				}
				if (target.isDead || !m_GameLogic.IsSkillCanUse(actor, target, m_pSkillInfoLevel))
				{
					continue;
				}
				Vector3 bloodPos = target.GetBloodPos(actor.GetBone(1).position, target.Pos - actor.Pos);
				iGameLogic.HitInfo hitinfo = new iGameLogic.HitInfo();
				hitinfo.v3HitDir = (target.Pos - actor.Pos).normalized;
				hitinfo.v3HitPos = bloodPos;
				m_GameLogic.Skill(m_pSkillInfoLevel, actor, target, ref hitinfo);
				if (m_GameScene.IsRoomMaster())
				{
					if (target.IsMonster())
					{
						CGameNetSender.GetInstance().BattleDamageMob(target.UID, m_GameLogic.ltDamageInfo);
					}
					else if (target.IsUser())
					{
						CGameNetSender.GetInstance().BattleDamagePlayer(m_GameLogic.ltDamageInfo);
					}
				}
				target.PlayAudio(kAudioEnum.HitBody);
			}
			break;
		}
		}
	}
}
