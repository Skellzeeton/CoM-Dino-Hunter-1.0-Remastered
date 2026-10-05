using BehaviorTree;
using UnityEngine;

public class doFlyHeightTask : Task
{
    protected float m_fSpeed;
    protected Vector3 m_v3Dst;
    protected bool m_bRotateBody;
    protected Vector3 m_v3RotSrc;
    protected Vector3 m_v3RotDst;
    protected float m_fRotRate;
    protected iGameSceneBase m_GameScene;
    private float m_fTimeSinceLastCorrection = 0f;
    private const float m_fCorrectionInterval = 1f;

    public doFlyHeightTask(Node node, float fSpeed)
    : base(node)
    {
        m_fSpeed = fSpeed;
    }

    public override void OnEnter(Object inputParam)
    {
        CCharMob cCharMob = inputParam as CCharMob;
        if (!(cCharMob == null))
        {
            cCharMob.SetCurTask(this);
            m_GameScene = iGameApp.GetInstance().m_GameScene;
            m_v3Dst = cCharMob.m_v3BirthPos;
            ClampDestinationToUser(cCharMob, true);
            TurnRound(cCharMob, m_v3Dst - cCharMob.Pos);
            cCharMob.CrossAnim(kAnimEnum.MoveForward, WrapMode.Loop, 0.3f, 1f, 0f);
        }
    }

    public override kTreeRunStatus OnUpdate(Object inputParam, float deltaTime)
    {
        CCharMob cCharMob = inputParam as CCharMob;
        if (cCharMob == null)
        {
            return kTreeRunStatus.Failture;
        }
        float num = m_fSpeed;
        if (num == 0f)
        {
            num = cCharMob.Property.GetValue(kProEnum.MoveSpeed);
        }
        m_fTimeSinceLastCorrection += deltaTime;
        if (m_fTimeSinceLastCorrection >= m_fCorrectionInterval)
        {
            m_fTimeSinceLastCorrection = 0f;
            ClampDestinationToUser(cCharMob, false);
        }
        if (m_bRotateBody)
        {
            m_fRotRate += num * 0.5f * deltaTime;
            Vector3 vector = Vector3.Lerp(m_v3RotSrc, m_v3RotDst, m_fRotRate);
            if (vector != Vector3.zero)
            {
                cCharMob.Dir3D = vector;
            }
            if (m_fRotRate >= 1f)
            {
                m_bRotateBody = false;
            }
        }
        Vector3 vector2 = m_v3Dst - cCharMob.Pos;
        float num2 = num * deltaTime;
        float magnitude = vector2.magnitude;
        if (magnitude < 0.5f)
        {
            cCharMob.Pos = m_v3Dst;
            return kTreeRunStatus.Success;
        }
        if (num2 < magnitude)
        {
            cCharMob.Pos += vector2 / magnitude * num2;
            return kTreeRunStatus.Executing;
        }
        cCharMob.Pos = m_v3Dst;
        return kTreeRunStatus.Success;
    }

    protected void TurnRound(CCharBase charbase, Vector3 v3Forward)
    {
        m_bRotateBody = true;
        m_v3RotSrc = charbase.Dir2D;
        m_v3RotDst = v3Forward;
        m_fRotRate = 0f;
    }

    private void ClampDestinationToUser(CCharMob cCharMob, bool forceImmediate)
    {
        if (m_GameScene == null)
            return;
        CCharUser user = m_GameScene.GetUser();
        if (user == null)
            return;
        Vector3 currentOffset = cCharMob.Pos - user.Pos;
        currentOffset.y = 0f;
        Vector3 destOffset = m_v3Dst - user.Pos;
        destOffset.y = 0f;
        bool needNewDestination = false;
        Vector3 newDest = m_v3Dst;
        if (currentOffset.magnitude > doFlyParameters.MaxDistanceFromUser)
        {
            Vector3 dirToUser = (user.Pos - cCharMob.Pos).normalized;
            if (dirToUser.sqrMagnitude < 0.001f)
                dirToUser = Vector3.forward;
            float targetDist = doFlyParameters.MaxDistanceFromUser * 0.6f;
            newDest = cCharMob.Pos + dirToUser * targetDist;
            newDest.y = m_GameScene.m_fNavPlane;
            needNewDestination = true;
        }
        else if (destOffset.magnitude > doFlyParameters.MaxDistanceFromUser)
        {
            Vector3 clampedDirection = destOffset.normalized;
            newDest = user.Pos + clampedDirection * doFlyParameters.MaxDistanceFromUser;
            newDest.y = m_GameScene.m_fNavPlane;
            needNewDestination = true;
        }
        else if (forceImmediate || Random.value < 0.1f)
        {
            Vector2 randomAdjust = Random.insideUnitCircle * 3f;
            newDest = m_v3Dst + new Vector3(randomAdjust.x, 0f, randomAdjust.y);
            Vector3 adjustedOffset = newDest - user.Pos;
            adjustedOffset.y = 0f;
            if (adjustedOffset.magnitude <= doFlyParameters.MaxDistanceFromUser)
            {
                needNewDestination = true;
            }
        }
        if (needNewDestination)
        {
            if (Vector3.Distance(newDest, m_v3Dst) > 1f || forceImmediate)
            {
                m_v3Dst = newDest;
                TurnRound(cCharMob, m_v3Dst - cCharMob.Pos);
            }
        }
    }
}