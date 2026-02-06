namespace gyTaskSystem
{
	public class CTaskTimed : CTaskBase
	{
		public override void OnTaskLimitTimeOver()
		{
			TaskFailed();
		}
		public override void OnKillAllMonsters()
		{
			TaskCompleted();
		}
	}
}
