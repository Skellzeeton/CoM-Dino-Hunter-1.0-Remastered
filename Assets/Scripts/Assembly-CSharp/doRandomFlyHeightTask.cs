using BehaviorTree;
using UnityEngine;

public class doRandomFlyHeightTask : Task
{
    protected iGameSceneBase m_GameScene;
    private float m_fTimeToNextChange = 0f;
    private Vector3 m_v3LastDirection = Vector3.zero;

    public doRandomFlyHeightTask(Node node)
    : base(node)
    {
    }

    public override void OnEnter(Object inputParam)
    {
        m_GameScene = iGameApp.GetInstance().m_GameScene;
        m_fTimeToNextChange = 0f;
        m_v3LastDirection = Vector3.zero;
    }

    public override kTreeRunStatus OnUpdate(Object inputParam, float deltaTime)
    {
        CCharMob cCharMob = inputParam as CCharMob;
        if (cCharMob == null)
        {
            return kTreeRunStatus.Failture;
        }
        CCharUser user = m_GameScene != null ? m_GameScene.GetUser() : null;
        if (user == null)
        {
            return kTreeRunStatus.Success;
        }
        m_fTimeToNextChange -= deltaTime;
        float distToUser = Vector3.Distance(cCharMob.Pos, user.Pos);
        bool shouldChange = m_fTimeToNextChange <= 0f ||
        distToUser > doFlyParameters.MaxDistanceFromUser;
        if (!shouldChange)
        {
            return kTreeRunStatus.Success;
        }
        m_fTimeToNextChange = Random.Range(doFlyParameters.ChangeIntervalMin,
                doFlyParameters.ChangeIntervalMax);
        Vector3 target = GenerateCombatPosition(cCharMob, user);
        cCharMob.m_v3BirthPos = target;
        return kTreeRunStatus.Success;
    }

    private Vector3 GenerateCombatPosition(CCharMob cCharMob, CCharUser user)
    {
        Vector3 target = Vector3.zero;
        bool found = false;
        int attempts = 8;
        for (int i = 0; i < attempts; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle *
            Random.Range(doFlyParameters.MinRandomRadius, doFlyParameters.MaxRandomRadius);
            Vector3 candidate = user.Pos + new Vector3(randomCircle.x, 0f, randomCircle.y);
            candidate.y = m_GameScene.m_fNavPlane;
            if (IsValidPosition(cCharMob, candidate))
            {
                Vector3 newDirection = candidate - cCharMob.Pos;
                newDirection.y = 0f;
                if (m_v3LastDirection != Vector3.zero)
                {
                    float angle = Vector3.Angle(m_v3LastDirection, newDirection);
                    if (angle < 60f && i < attempts - 1)
                        continue;
                }
                target = candidate;
                found = true;
                m_v3LastDirection = newDirection.normalized;
                break;
            }
        }
        if (!found)
        {
            Vector3 toUser = user.Pos - cCharMob.Pos;
            toUser.y = 0f;
            if (toUser.sqrMagnitude > 0.001f)
            {
                Vector3 dirToUser = toUser.normalized;
                float fallbackDist = Mathf.Min(
                        Vector3.Distance(cCharMob.Pos, user.Pos),
                        doFlyParameters.MaxDistanceFromUser * 0.7f
                );
                Vector2 randomPerp = Random.insideUnitCircle * 3f;
                Vector3 perp = Vector3.Cross(dirToUser, Vector3.up).normalized * randomPerp.x;
                target = cCharMob.Pos + dirToUser * fallbackDist + perp;
                target.y = m_GameScene.m_fNavPlane;
            }
            else
            {
                target = cCharMob.Pos;
            }
        }
        Vector3 offsetFromUser = target - user.Pos;
        offsetFromUser.y = 0f;
        if (offsetFromUser.magnitude > doFlyParameters.MaxDistanceFromUser)
        {
            offsetFromUser = offsetFromUser.normalized * doFlyParameters.MaxDistanceFromUser;
            target = user.Pos + offsetFromUser;
            target.y = m_GameScene.m_fNavPlane;
        }
        return target;
    }

    private bool IsValidPosition(CCharMob cCharMob, Vector3 position)
    {
        Vector3 toPosition = position - cCharMob.Pos;
        float distance = toPosition.magnitude;
        if (distance < 0.5f)
            return false;
        if (Physics.Raycast(cCharMob.Pos, toPosition / distance, distance, int.MinValue))
            return false;
        return true;
    }
}