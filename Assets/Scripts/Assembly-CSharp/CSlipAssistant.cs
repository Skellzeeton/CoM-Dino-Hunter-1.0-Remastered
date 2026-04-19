using UnityEngine;

public class CSlipAssistant
{
    public float m_fCurFrameYaw;
    public float m_fCurFramePitch;
    protected float m_fLstPointTime;
    protected float m_fCurPointTime;
    private const float kMaxYawSpeed = 280f;
    private const float kMaxPitchSpeed = 70f;

    public void Tap()
    {
        m_fLstPointTime = Time.realtimeSinceStartup;
    }

    public bool Slip(Vector2 v2Delta)
    {
        if (v2Delta == Vector2.zero)
        {
            m_fCurFrameYaw = 0f;
            m_fCurFramePitch = 0f;
            return false;
        }
        float x = Mathf.Clamp(v2Delta.x / (float)Screen.width, -1f, 1f);
        float y = Mathf.Clamp(v2Delta.y / (float)Screen.height, -1f, 1f);
        m_fCurFrameYaw = x * kMaxYawSpeed;
        m_fCurFramePitch = y * kMaxPitchSpeed;
        return true;
    }
}