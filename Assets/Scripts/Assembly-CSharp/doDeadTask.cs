using BehaviorTree;
using UnityEngine;

public class doDeadTask : Task
{
	protected enum kDeadProcess
	{
		None = 0,
		Deading = 1,
		Wait = 2,
		Flash = 3,
		Disappear = 4,
		Destroy = 5
	}

	protected iGameSceneBase m_GameScene;

	protected kDeadProcess m_State;

	protected float m_fDeadTime;

	protected float m_fWaitTime;

	protected float m_fFlashTime;

	protected float m_fDisappearTime;

	protected float m_fTimeCount;

	protected float m_fRate;

	protected float m_fRateSpeed;

	protected Vector3 m_v3SrcDeathPos;

	protected Vector3 m_v3DstDeathPos;

	protected float m_fFloorHeight;

	protected bool m_bFall;

	protected float m_fSpeedYInt;

	protected float m_fSpeedYAcc = -20f;

	protected Color m_Color1 = new Color(0.8f, 0.8f, 0.8f, 0.1f);

	protected Color m_Color2 = new Color(1f, 1f, 1f, 1f);

	protected Color m_srcColor;

	protected Color m_dstColor;

	protected float m_ColorRate;

	public doDeadTask(Node node)
		: base(node)
	{
	}

	public override void OnEnter(Object inputParam)
	{
		CCharMob cCharMob = inputParam as CCharMob;
		if (cCharMob == null)
		{
			return;
		}
		cCharMob.SetCurTask(this);
		m_GameScene = iGameApp.GetInstance().m_GameScene;
		m_fWaitTime = ((!cCharMob.IsBoss()) ? 2 : 5);
		m_fFlashTime = 2f;
		m_fDisappearTime = 0.2f;
		m_State = kDeadProcess.Deading;
		m_fTimeCount = 0f;
		switch (cCharMob.m_DeadMode)
		{
		case kDeadMode.FlyDead:
			m_fDeadTime = cCharMob.CrossAnim(kAnimEnum.Mob_DeadFly, WrapMode.ClampForever, 0.3f, 1f, 0f);
			break;
		case kDeadMode.HitFly:
		{
			if (cCharMob.m_v3DeadDirection != Vector3.zero)
			{
				cCharMob.Dir2D = -cCharMob.m_v3DeadDirection;
				cCharMob.m_v3DeadDirection = Vector3.zero;
			}
			else
			{
				CCharUser user = m_GameScene.GetUser();
				if (user != null)
				{
					cCharMob.Dir2D = user.Pos - cCharMob.Pos;
				}
			}
			m_fDeadTime = cCharMob.CrossAnim(kAnimEnum.Mob_DeadHitFly, WrapMode.ClampForever, 0.3f, 1f, 0f);
			RaycastHit hitInfo;
			if (Physics.Raycast(cCharMob.Pos, -cCharMob.Dir2D, out hitInfo, cCharMob.m_fDeadDistance + 2f, -1870659584))
			{
				m_v3DstDeathPos = hitInfo.point + cCharMob.Dir2D * 2f;
			}
			else
			{
				m_v3DstDeathPos = cCharMob.Pos - cCharMob.Dir2D * cCharMob.m_fDeadDistance;
			}
			m_v3SrcDeathPos = cCharMob.Pos;
			m_fRate = 0f;
			m_fRateSpeed = 1f / m_fDeadTime;
			break;
		}
		default:
			m_fDeadTime = cCharMob.CrossAnim(kAnimEnum.Mob_Dead, WrapMode.ClampForever, 0.3f, 1f, 0f);
			break;
		}
		Vector3 pos = cCharMob.Pos;
		pos.y += 100f;
		RaycastHit hitInfo2;
		if (Physics.Raycast(new Ray(pos, Vector3.down), out hitInfo2, 1000f, 536870912))
		{
			m_fFloorHeight = hitInfo2.point.y;
		}
		if (cCharMob.Pos.y > m_fFloorHeight)
		{
			m_bFall = true;
		}
	}

	public override kTreeRunStatus OnUpdate(Object inputParam, float deltaTime)
	{
		CCharMob cCharMob = inputParam as CCharMob;
		if (cCharMob == null)
		{
			return kTreeRunStatus.Failture;
		}
		switch (m_State)
		{
		case kDeadProcess.Deading:
		{
			m_fTimeCount += deltaTime;
			if (m_fTimeCount >= m_fDeadTime)
			{
				m_State = kDeadProcess.Wait;
				m_fTimeCount = 0f;
				break;
			}
			kDeadMode deadMode = cCharMob.m_DeadMode;
			if (deadMode == kDeadMode.HitFly)
			{
				Vector3 pos = Vector3.Lerp(m_v3SrcDeathPos, m_v3DstDeathPos, m_fTimeCount / m_fDeadTime);
				pos.y = cCharMob.Pos.y;
				cCharMob.Pos = pos;
			}
			break;
		}
		case kDeadProcess.Wait:
			m_fTimeCount += deltaTime;
			if (m_fTimeCount >= m_fWaitTime)
			{
				if (cCharMob.IsBoss())
				{
					m_State = kDeadProcess.Disappear;
				}
				else
				{
					m_State = kDeadProcess.Flash;
					m_ColorRate = 0f;
					m_srcColor = m_Color1;
					m_dstColor = m_Color2;
				}
				m_fTimeCount = 0f;
			}
			break;
		case kDeadProcess.Flash:
			m_fTimeCount += deltaTime;
			m_ColorRate += 10f * deltaTime;
			cCharMob.SetColor(Color.Lerp(m_srcColor, m_dstColor, m_ColorRate));
			if (m_ColorRate >= 1f)
			{
				m_ColorRate = 0f;
				m_srcColor = ((!(m_srcColor == m_Color1)) ? m_Color1 : m_Color2);
				m_dstColor = ((!(m_dstColor == m_Color1)) ? m_Color1 : m_Color2);
			}
			if (m_fTimeCount >= m_fFlashTime)
			{
				m_State = kDeadProcess.Disappear;
				m_fTimeCount = 0f;
			}
			break;
		case kDeadProcess.Disappear:
			m_fTimeCount += deltaTime;
			cCharMob.SetAlpha(1f - m_fTimeCount / m_fDisappearTime);
			if (m_fTimeCount >= m_fDisappearTime)
			{
				cCharMob.isNeedDestroy = true;
				m_State = kDeadProcess.Destroy;
			}
			break;
		}
		if (m_bFall)
		{
			cCharMob.Pos += new Vector3(0f, m_fSpeedYInt * deltaTime, 0f);
			if (cCharMob.m_DeadMode == kDeadMode.FlyDead)
			{
				cCharMob.Pos += cCharMob.Dir2D * cCharMob.Property.GetValue(kProEnum.MoveSpeed) * deltaTime;
			}
			m_fSpeedYInt += m_fSpeedYAcc * deltaTime;
			if (cCharMob.Pos.y <= m_fFloorHeight)
			{
				cCharMob.Pos = new Vector3(cCharMob.Pos.x, m_fFloorHeight, cCharMob.Pos.z);
				m_bFall = false;
			}
		}
		return kTreeRunStatus.Executing;
	}
}
