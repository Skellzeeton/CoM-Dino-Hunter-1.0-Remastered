using UnityEngine;

public class CUseSkillLock : CUseSkill
{
	protected float m_fTimeHold;

	protected float m_fTimeHoldCount;

	protected float m_fRotSpeed;

	protected float m_fRotRate;

	public override kUseSkillStatus OnEnter(CCharBase charbase)
	{
		int nValue = 1;
		m_pSkillInfoLevel.GetSkillModeValue(1, ref nValue);
		charbase.CrossAnim((kAnimEnum)m_pSkillInfoLevel.nAnim, (nValue != 1) ? WrapMode.ClampForever : WrapMode.Loop, 0.3f, 1f, 0f);
		m_pSkillInfoLevel.GetSkillModeValue(0, ref m_fTimeHold);
		m_fTimeHoldCount = 0f;
		if (charbase.m_Target != null && m_fTimeHold > 0f)
		{
			m_fRotSpeed = 1f / m_fTimeHold;
			m_fRotRate = 0f;
		}
		else
		{
			m_fRotRate = 1f;
		}
		return kUseSkillStatus.Success;
	}

	public override kUseSkillStatus OnUpdate(CCharBase charbase, float deltaTime)
	{
		if (m_fRotRate >= 1f)
		{
			return kUseSkillStatus.Success;
		}
		m_fRotRate += m_fRotSpeed * deltaTime;
		if (m_fRotRate > 1f)
		{
			m_fRotRate = 1f;
		}
		if (charbase.m_Target != null)
		{
			charbase.Dir3D = Vector3.Lerp(charbase.Dir3D, (charbase.m_Target.Pos - charbase.Pos).normalized, m_fRotRate);
		}
		return kUseSkillStatus.Executing;
	}

	public override void OnExit(CCharBase charbase)
	{
	}
}
