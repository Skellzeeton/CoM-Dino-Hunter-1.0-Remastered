using BehaviorTree;
using UnityEngine;

public class doSelectTargetBuildingTask : Task
{
	protected iGameSceneBase m_GameScene;

	protected iGameData m_GameData;

	protected UnityEngine.AI.NavMeshPath m_NavPath;

	public doSelectTargetBuildingTask(Node node)
		: base(node)
	{
		m_NavPath = new UnityEngine.AI.NavMeshPath();
	}

	public override void OnEnter(Object inputParam)
	{
		m_GameScene = iGameApp.GetInstance().m_GameScene;
		m_GameData = iGameApp.GetInstance().m_GameData;
	}

	public override kTreeRunStatus OnUpdate(Object inputParam, float deltaTime)
	{
		CCharMob cCharMob = inputParam as CCharMob;
		if (cCharMob == null)
		{
			return kTreeRunStatus.Failture;
		}
		CMobInfoLevel mobInfo = m_GameData.GetMobInfo(cCharMob.ID, cCharMob.Level);
		if (mobInfo == null)
		{
			return kTreeRunStatus.Failture;
		}
		iBuilding curBuilding = m_GameScene.CurBuilding;
		if (curBuilding == null)
		{
			return kTreeRunStatus.Failture;
		}
		Vector3 randomPoint = curBuilding.GetRandomPoint();
		Vector3 vector = randomPoint - cCharMob.GetBone(2).position;
		Vector3 normalized = vector.normalized;
		normalized.y = 0f;
		RaycastHit hitInfo;
		if (Physics.Raycast(cCharMob.GetBone(2).position, normalized, out hitInfo, vector.magnitude, -1870659584) && hitInfo.collider.transform.root.gameObject == curBuilding.gameObject)
		{
			cCharMob.m_TargetBuilding = curBuilding;
			if (cCharMob.m_TargetBuilding == null || cCharMob.m_TargetBuilding.IsBroken)
			{
				return kTreeRunStatus.Failture;
			}
			Vector3 point = hitInfo.point;
			point.y = hitInfo.collider.transform.root.position.y;
			if (!UnityEngine.AI.NavMesh.CalculatePath(cCharMob.Pos, point, -1, m_NavPath))
			{
				return kTreeRunStatus.Failture;
			}
			cCharMob.m_ltPath.Clear();
			for (int i = 0; i < m_NavPath.corners.Length; i++)
			{
				cCharMob.m_ltPath.Add(m_NavPath.corners[i]);
			}
			if (cCharMob.m_ltPath.Count < 2)
			{
				cCharMob.m_ltPath[0] = cCharMob.m_ltPath[0] - normalized * mobInfo.fMeleeRange;
			}
			else
			{
				int index = cCharMob.m_ltPath.Count - 1;
				if (Vector3.Distance(cCharMob.m_ltPath[index], point) < mobInfo.fMeleeRange)
				{
					cCharMob.m_ltPath[index] = point - normalized * mobInfo.fMeleeRange;
				}
			}
			return kTreeRunStatus.Success;
		}
		return kTreeRunStatus.Failture;
	}
}
