namespace gyAchievementSystem
{
	public class CAchievementData
	{
		public int nID;

		public int nState;

		public int nCurValue;

		public bool[] arrIsGotReward;

		public CAchievementData()
		{
			arrIsGotReward = new bool[3];
		}

		public void SetGotReward(int nIndex, bool bGot)
		{
			if (nIndex >= 0 && nIndex < arrIsGotReward.Length)
			{
				arrIsGotReward[nIndex] = bGot;
			}
		}

		public bool IsGotReward(int nIndex)
		{
			if (nIndex < 0 || nIndex >= arrIsGotReward.Length)
			{
				return false;
			}
			return arrIsGotReward[nIndex];
		}
	}
}
