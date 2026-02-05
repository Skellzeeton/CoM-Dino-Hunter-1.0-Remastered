using System.Collections.Generic;

public class CTaskInfoCollect : CTaskInfo
{
	public class CCollectInfo
	{
		public int nItemID;

		public int nMaxCount;

		public CCollectInfo(int nItemID, int nMaxCount)
		{
			this.nItemID = nItemID;
			this.nMaxCount = nMaxCount;
		}
	}

	public List<CCollectInfo> ltCollectInfo;

	public CTaskInfoCollect()
	{
		ltCollectInfo = new List<CCollectInfo>();
	}
}
