using BehaviorTree;
using UnityEngine;

public class doRandomFlyHeightTask : Task
{
	protected iGameSceneBase m_GameScene;

	public doRandomFlyHeightTask(Node node)
		: base(node)
	{
	}

	public override void OnEnter(Object inputParam)
	{
		m_GameScene = iGameApp.GetInstance().m_GameScene;
	}

	public override kTreeRunStatus OnUpdate(Object inputParam, float deltaTime)
	{
		CCharMob cCharMob = inputParam as CCharMob;
		if (cCharMob == null)
		{
			return kTreeRunStatus.Failture;
		}
		Vector3 vector;
		Vector3 vector2;
		float magnitude;
		do
		{
			vector = cCharMob.Pos + new Vector3(Random.Range(-50f, 50f), 0f, Random.Range(-50f, 50f));
			vector.y = m_GameScene.m_fNavPlane;
			vector2 = vector - cCharMob.Pos;
			magnitude = vector2.magnitude;
		}
		while (Physics.Raycast(cCharMob.Pos, vector2 / magnitude, magnitude, int.MinValue));
		cCharMob.m_v3BirthPos = vector;
		return kTreeRunStatus.Success;
	}
}
