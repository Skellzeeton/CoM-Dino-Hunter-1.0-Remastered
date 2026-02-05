using BehaviorTree;

public class doBeatBackPlayerNode : Node
{
	public override Task CreateTask()
	{
		return new doBeatBackUserTask(this);
	}
}
