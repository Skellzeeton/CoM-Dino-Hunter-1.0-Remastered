using BehaviorTree;
using UnityEngine;

public class doDeadPlayerTask : Task
{
	public doDeadPlayerTask(Node node)
		: base(node)
	{
	}

	public override void OnEnter(Object inputParam)
	{
		CCharPlayer cCharPlayer = inputParam as CCharPlayer;
		if (!(cCharPlayer == null))
		{
			cCharPlayer.SetCurTask(this);
			cCharPlayer.CrossAnim(kAnimEnum.Death, WrapMode.ClampForever, 0.3f, 1f, 0f);
			cCharPlayer.RemoveAllBuff();
			cCharPlayer.ClearAllStatus();
		}
	}
}
