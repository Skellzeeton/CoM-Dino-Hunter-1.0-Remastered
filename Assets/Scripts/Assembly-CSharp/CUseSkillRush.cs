using UnityEngine;

public class CUseSkillRush : CUseSkill
{
	protected Vector3 m_v3Dst;

	protected float m_fExpandDis;

	protected float m_fSpeed;

	protected float m_fTimePoint;

	protected float m_fTimePointCount;

	public override kUseSkillStatus OnEnter(CCharBase charbase)
	{
		m_pSkillInfoLevel.GetSkillModeValue(0, ref m_fTimePoint);
		m_pSkillInfoLevel.GetSkillModeValue(1, ref m_fExpandDis);
		m_v3Dst = charbase.m_Target.Pos;
		Vector3 vector = m_v3Dst - charbase.Pos;
		float magnitude = vector.magnitude;
		Vector3 vector2 = vector / magnitude;
		if (magnitude + m_fExpandDis <= 0f)
		{
			return kUseSkillStatus.Failure;
		}
		if (Physics.Raycast(charbase.Pos, vector2, magnitude + m_fExpandDis, -1879048192))
		{
			return kUseSkillStatus.Failure;
		}
		if (m_fTimePoint == 0f)
		{
			m_fSpeed = 1000f;
		}
		else
		{
			m_fSpeed = magnitude / m_fTimePoint;
		}
		m_v3Dst += vector2 * m_fExpandDis;
		m_fTimePointCount = 0f;
		CCharMob cCharMob = charbase as CCharMob;
		if (cCharMob != null)
		{
			if (cCharMob.MobType == 1)
			{
				if (cCharMob.IsBoss())
				{
					cCharMob.PlayAudio("Ani_Dive_Ptero_Boss");
				}
				else
				{
					cCharMob.PlayAudio("Ani_Dive_Ptero");
				}
				Object @object = PrefabManager.Get(1402);
				if (@object != null)
				{
					m_SkillEffect = (GameObject)Object.Instantiate(@object);
					if (m_SkillEffect != null)
					{
						m_SkillEffect.transform.parent = charbase.GetBone(1);
						m_SkillEffect.transform.localPosition = Vector3.zero;
						m_SkillEffect.transform.localEulerAngles = new Vector3(270f, 180f, 0f);
					}
				}
			}
			CMobInfoLevel mobInfo = cCharMob.GetMobInfo();
			if (mobInfo != null)
			{
				if (m_pSkillInfoLevel.nAnim == 4)
				{
					charbase.CrossAnim((kAnimEnum)m_pSkillInfoLevel.nAnim, WrapMode.Loop, 0.3f, m_fSpeed * mobInfo.fMoveSpeedRate, 0f);
				}
				else if (m_pSkillInfoLevel.nAnim == 2504)
				{
					charbase.CrossAnim((kAnimEnum)m_pSkillInfoLevel.nAnim, WrapMode.Loop, 0.3f, m_fSpeed * mobInfo.fRushSpeedRate, 0f);
				}
				else
				{
					charbase.CrossAnim((kAnimEnum)m_pSkillInfoLevel.nAnim, WrapMode.Loop, 0.3f, 1f, 0f);
				}
			}
			else
			{
				charbase.CrossAnim((kAnimEnum)m_pSkillInfoLevel.nAnim, WrapMode.Loop, 0.3f, 1f, 0f);
			}
		}
		else
		{
			charbase.CrossAnim((kAnimEnum)m_pSkillInfoLevel.nAnim, WrapMode.Loop, 0.3f, 1f, 0f);
		}
		charbase.Dir3D = vector2;
		//Debug.Log(charbase.UID + " start rush state");
		return kUseSkillStatus.Success;
	}

	public override void OnExit(CCharBase charbase)
	{
		//Debug.Log(charbase.UID + " out of the rush state");
		if (m_SkillEffect != null)
		{
			//Debug.Log(charbase.UID + " destroy the effect");
			Object.Destroy(m_SkillEffect);
			m_SkillEffect = null;
		}
	}

	public override kUseSkillStatus OnUpdate(CCharBase charbase, float deltaTime)
	{
		m_fTimePointCount += deltaTime;
		if (m_fTimePointCount >= m_fTimePoint)
		{
			m_fTimePointCount = 0f;
			SkillEffect(charbase, m_Target);
		}
		float num = m_fSpeed * deltaTime;
		Vector3 vector = m_v3Dst - charbase.Pos;
		if (num * num < vector.sqrMagnitude)
		{
			charbase.Pos += vector.normalized * num;
			return kUseSkillStatus.Executing;
		}
		charbase.Pos = m_v3Dst;
		return kUseSkillStatus.Success;
	}
}
