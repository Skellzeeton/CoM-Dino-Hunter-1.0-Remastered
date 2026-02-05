using UnityEngine;

public class CSlipAssistant
{
	public float m_fCurFrameYaw;

	public float m_fCurFramePitch;

	protected float m_fLstPointTime;

	protected float m_fCurPointTime;

	public void Tap()
	{
		m_fLstPointTime = Time.realtimeSinceStartup;
	}

	public bool Slip(Vector2 v2Delta)
	{
		float num = Mathf.Abs(v2Delta.x / (float)Screen.width);
		float num2 = Mathf.Abs(v2Delta.y / (float)Screen.height);
		float deltaTime = Time.deltaTime;
		if (deltaTime < 0.06f)
		{
			deltaTime = 0.03f;
		}
		m_fCurFrameYaw = num * 720f;
		m_fCurFramePitch = num2 * 120f;
		if (v2Delta.x < 0f)
		{
			m_fCurFrameYaw *= -1f;
		}
		if (v2Delta.y < 0f)
		{
			m_fCurFramePitch *= -1f;
		}
		return true;
	}
}
