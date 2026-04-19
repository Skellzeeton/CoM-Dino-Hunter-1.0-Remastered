using UnityEngine;

public class CControlIphone : CControlBase
{
    protected float m_fSlashSpeed;
    protected Vector2 m_v2Slash;
    private const float kMobileYawMaxSpeed = 300f;
    private const float kMobilePitchMaxSpeed = 75f;

    public CControlIphone()
    {
        m_GameScene = iGameApp.GetInstance().m_GameScene;
        m_GameUI = m_GameScene.GetGameUI();
        m_GameUI.RegisterEvent();
    }

    public override void Initialize()
    {
        base.Initialize();
        m_fSlashSpeed = ((!Utils.IsPad()) ? 3f : 6f);
        m_v2Slash = Vector2.zero;
    }

    public override void Update(float deltaTime)
    {
        if (m_GameScene == null || m_User == null)
        {
            return;
        }
        if (m_User.IsCanAim() && (m_v2Slash.x != 0f || m_v2Slash.y != 0f))
        {
            Ray ray = m_Camera.ScreenPointToRay(m_GameState.ScreenCenter, 0f);
            m_User.LookAt(ray.GetPoint(1000f));
        }
    }

    public override void LateUpdate(float deltaTime)
    {
        if (m_GameScene == null || m_User == null || m_v2Slash == Vector2.zero)
        {
            return;
        }
        Vector2 input = Vector2.ClampMagnitude(m_v2Slash, 1f);
        if (Mathf.Abs(input.x) > 0.001f)
        {
            float yaw = input.x * kMobileYawMaxSpeed * deltaTime;
            m_Camera.Yaw(yaw);
            if (m_User.IsCanAim())
            {
                m_User.SetYaw(m_Camera.GetYaw());
            }
        }
        if (Mathf.Abs(input.y) > 0.001f)
        {
            float pitch = input.y * kMobilePitchMaxSpeed * deltaTime;
            m_Camera.Pitch(pitch);
        }
        if (m_User.IsCanAim())
        {
            Ray ray = m_Camera.ScreenPointToRay(m_GameState.ScreenCenter, 0f);
            m_User.LookAt(ray.GetPoint(1000f));
        }
        m_v2Slash = Vector2.zero;
    }
}