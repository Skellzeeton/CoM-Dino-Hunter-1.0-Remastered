using UnityEngine;

public class iRushEffect : _iAnimEventFollowBase
{
	public int nPrefabID;

	public float fDisappearTime = 2f;

	public void iRushEffect_PlayEffect()
	{
		PlayEffect(nPrefabID);
	}

	public void iRushEffect_StopEffect(bool bImmediately = false)
	{
		if (!(m_Effect != null))
		{
			return;
		}
		if (bImmediately)
		{
			Object.Destroy(m_Effect);
			m_Effect = null;
			return;
		}
		ParticleSystem[] componentsInChildren = m_Effect.GetComponentsInChildren<ParticleSystem>();
		if (componentsInChildren != null && componentsInChildren.Length > 0)
		{
			m_Effect.transform.parent = null;
			ParticleSystem[] array = componentsInChildren;
			foreach (ParticleSystem particleSystem in array)
			{
				particleSystem.enableEmission = false;
			}
			Object.Destroy(m_Effect, fDisappearTime);
			m_Effect = null;
		}
		else
		{
			Object.Destroy(m_Effect);
			m_Effect = null;
		}
	}
}
