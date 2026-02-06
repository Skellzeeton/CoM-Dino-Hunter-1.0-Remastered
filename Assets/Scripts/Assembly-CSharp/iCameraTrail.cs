using System.Reflection;
using UnityEngine;

public class iCameraTrail : iCamera
{
    public CCharBase m_Target;

    public Vector3 camera_offset_normal;

    public Vector3 camera_offset_shoot;

    public Vector3 camera_offset_melee;

    public Vector3 camera_offset_block;

    public Vector3 camera_lookat;

    protected AudioListener m_AudioListenerCamera;

    protected AudioListener m_AudioListenerTarget;

    protected Vector3 m_v3Camera_Offset_Near;

    protected Vector3 m_v3Camera_Offset_Far;

    protected Vector3 m_v3Camera_Offset_Cur;

    protected float m_fSmoothSpeed;

    protected float m_fCurCameraDis;

    protected float m_fMaxCameraDis;

    protected float m_fSrcYaw;

    protected float m_fDstYaw;

    protected float m_fSrcPitch;

    protected float m_fDstPitch;

    protected float m_fRateYaw;

    protected float m_fRatePitch;

    protected float yawSmoothTime = 0.022f;
    protected float pitchSmoothTime = 0.022f;
    private float yawSmoothVelocity = 0f;
    private float pitchSmoothVelocity = 0f;

    protected float characterRotationSpeed = 0f;

    private float currentCharacterYaw = 0f;

    public new void Awake()
    {
        base.Awake();
        m_AudioListenerCamera = GetComponent<AudioListener>();
        if (m_AudioListenerCamera != null) m_AudioListenerCamera.enabled = false;
        m_fSrcYaw = 0f;
        m_fDstYaw = m_fSrcYaw;
        m_fSrcPitch = 0f;
        m_fDstPitch = m_fSrcPitch;
        m_fRateYaw = 1f;
        m_fRatePitch = 1f;
        base.enabled = false;
        m_CameraController.enabled = false;
        base.enabled = true;
        m_CameraController.enabled = true;
    }
    
    public void Initialize(CCharBase target, bool bMeleeView = false)
    {
        if (m_AudioListenerTarget == null && target != null)
            m_AudioListenerTarget = target.gameObject.AddComponent<AudioListener>();
        SwitchToTargetListener();
        ShootMode(false);
        bool isMeleeView = bMeleeView;
        if (target != null)
        {
            CCharUser user = target as CCharUser;
            if (user != null)
            {
                try
                {
                    FieldInfo gsField = user.GetType().GetField("m_GameState", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    object gameState = gsField != null ? gsField.GetValue(user) : null;
                    if (gameState != null)
                    {
                        MethodInfo getCurrWeaponMethod = gameState.GetType().GetMethod("GetCurrWeapon", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        object currWeapon = getCurrWeaponMethod != null ? getCurrWeaponMethod.Invoke(gameState, null) : null;

                        if (currWeapon != null)
                        {
                            PropertyInfo curLvlProp = currWeapon.GetType().GetProperty("CurWeaponLvlInfo", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                            FieldInfo curLvlField = currWeapon.GetType().GetField("CurWeaponLvlInfo", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                            object curLvlObj = null;
                            if (curLvlProp != null)
                                curLvlObj = curLvlProp.GetValue(currWeapon, null);
                            else if (curLvlField != null)
                                curLvlObj = curLvlField.GetValue(currWeapon);
                            if (curLvlObj != null)
                            {
                                PropertyInfo nTypeProp = curLvlObj.GetType().GetProperty("nType", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                                FieldInfo nTypeField = curLvlObj.GetType().GetField("nType", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                                int nType = -1;
                                if (nTypeProp != null)
                                {
                                    object val = nTypeProp.GetValue(curLvlObj, null);
                                    if (val is int) nType = (int)val;
                                }
                                else if (nTypeField != null)
                                {
                                    object val = nTypeField.GetValue(curLvlObj);
                                    if (val is int) nType = (int)val;
                                }
                                if (nType == 1)
                                    isMeleeView = true;
                                else if (nType >= 0)
                                    isMeleeView = false;
                            }
                        }
                    }
                }
                catch
                {
                    isMeleeView = bMeleeView;
                }
            }
            else
            {
                isMeleeView = bMeleeView;
            }
        }
        SetViewMelee(isMeleeView);
        m_Target = target;
        m_fDstYaw = m_fYaw = (target != null) ? target.transform.eulerAngles.x : 0f;
        m_fDstPitch = m_fPitch = 28f;
        m_v3Camera_Offset_Near = camera_offset_block;
        m_v3Camera_Offset_Cur = m_v3Camera_Offset_Near;
        m_fCurCameraDis = 0f;
        m_fMaxCameraDis = Vector3.Distance(m_v3Camera_Offset_Far, m_v3Camera_Offset_Near);
        m_fSmoothSpeed = 3f;
        currentCharacterYaw = (m_Target != null) ? m_Target.transform.eulerAngles.y : 0f;
        Quaternion baseRot = Quaternion.Euler(-m_fPitch, m_fYaw, 0f);
        Vector3 lookPt = (m_Target != null) ? m_Target.Pos + baseRot * camera_lookat : baseRot * camera_lookat;
        m_CameraController.Position = (m_Target != null) ? m_Target.Pos + baseRot * m_v3Camera_Offset_Cur : baseRot * m_v3Camera_Offset_Cur;
        m_CameraController.Rotation = Quaternion.LookRotation(lookPt - m_CameraController.Position, Vector3.up);
    }

    public void Destroy()
    {
        m_Target = null;
    }

    public new void LateUpdate()
    {
        if (!m_bActive || m_Target == null)
            return;
        float dt = Mathf.Max(0.0001f, Time.deltaTime);

        MyUtils.LimitAngle(ref m_fDstYaw, m_fYawMin, m_fYawMax);
        MyUtils.LimitAngle(ref m_fDstPitch, m_fPitchMin, m_fPitchMax);

        {
            float yawTime = Mathf.Max(0.0001f, yawSmoothTime);
            float pitchTime = Mathf.Max(0.0001f, pitchSmoothTime);
            m_fYaw = Mathf.SmoothDampAngle(m_fYaw, m_fDstYaw, ref yawSmoothVelocity, yawTime, Mathf.Infinity, dt);
            m_fPitch = Mathf.SmoothDampAngle(m_fPitch, m_fDstPitch, ref pitchSmoothVelocity, pitchTime, Mathf.Infinity, dt);
        }

        m_v3Camera_Offset_Cur = Vector3.Lerp(
            m_v3Camera_Offset_Cur,
            m_v3Camera_Offset_Far,
            m_fSmoothSpeed * dt
        );
        m_fMaxCameraDis = Vector3.Distance(m_v3Camera_Offset_Cur, m_v3Camera_Offset_Near);
        Vector3 baseRot = Quaternion.Euler(-m_fPitch, m_fYaw, 0f) * Vector3.forward;
        Vector3 lookPt = m_Target.Pos + Quaternion.Euler(-m_fPitch, m_fYaw, 0f) * camera_lookat;
        Vector3 nearPt = m_Target.Pos + Quaternion.Euler(-m_fPitch, m_fYaw, 0f) * m_v3Camera_Offset_Near;
        Vector3 farPt  = m_Target.Pos + Quaternion.Euler(-m_fPitch, m_fYaw, 0f) * m_v3Camera_Offset_Cur;
        float dist = Vector3.Distance(nearPt, farPt);
        Vector3 dir = dist > 0.0001f ? (farPt - nearPt).normalized : Vector3.forward;
        RaycastHit hit;
        if (Physics.Raycast(nearPt, dir, out hit, dist + 0.3f, -1610612736))
        {
            m_fCurCameraDis = Vector3.Distance(nearPt, hit.point) - 0.3f;
        }
        else
        {
            m_fCurCameraDis += m_fSmoothSpeed * dt;
            if (m_fCurCameraDis > m_fMaxCameraDis)
                m_fCurCameraDis = m_fMaxCameraDis;
        }

        Vector3 finalPos = Vector3.Lerp(nearPt, farPt, (m_fMaxCameraDis <= 0.00001f) ? 0f : (m_fCurCameraDis / m_fMaxCameraDis));
        Vector3 lookDir = lookPt - finalPos;
        if (lookDir.sqrMagnitude > 0.0001f)
        {
            m_CameraController.Rotation = Quaternion.LookRotation(lookDir.normalized, Vector3.up);
        }
        m_CameraController.Position = finalPos;
        float desiredCharacterYaw = m_fYaw;
        currentCharacterYaw = Mathf.MoveTowardsAngle(currentCharacterYaw, desiredCharacterYaw, characterRotationSpeed * dt);
        Quaternion charRot = Quaternion.Euler(0f, currentCharacterYaw, 0f);
        m_Target.transform.rotation = charRot;
    }

    public void Yaw(float angle)
    {
        m_fDstYaw += angle;
        MyUtils.LimitAngle(ref m_fDstYaw, m_fYawMin, m_fYawMax);
    }

    public void SetYaw(float angle)
    {
        m_fDstYaw = angle;
        MyUtils.LimitAngle(ref m_fDstYaw, m_fYawMin, m_fYawMax);
    }

    public void Pitch(float angle)
    {
        m_fDstPitch += angle;
        MyUtils.LimitAngle(ref m_fDstPitch, m_fPitchMin, m_fPitchMax);
    }

    public void SetPitch(float angle)
    {
        m_fDstPitch = angle;
        MyUtils.LimitAngle(ref m_fDstPitch, m_fPitchMin, m_fPitchMax);
    }

    public float GetYaw()
    {
        return m_fDstYaw;
    }

    public float GetPitch()
    {
        return m_fDstPitch;
    }

    public void SetViewMelee(bool on)
    {
        m_fSmoothSpeed = 5f;
        if (on)
        {
            m_v3Camera_Offset_Far = camera_offset_melee;
        }
        else
        {
            m_v3Camera_Offset_Far = camera_offset_normal;
        }
    }

    public void ShootMode(bool on)
    {
        if (on)
        {
            m_fSmoothSpeed = 8f;
            m_v3Camera_Offset_Far = camera_offset_shoot;
        }
        else
        {
            m_fSmoothSpeed = 5f;
            m_v3Camera_Offset_Far = camera_offset_normal;
        }
    }

    public void SwitchToTargetListener()
    {
        if (m_AudioListenerTarget != null)
        {
            m_AudioListenerTarget.enabled = true;
        }
        m_CameraController.ActiveListener(false);
    }

    public void SwitchToCameraListener()
    {
        if (m_AudioListenerTarget != null)
        {
            m_AudioListenerTarget.enabled = false;
        }
        m_CameraController.ActiveListener(true);
    }
}
