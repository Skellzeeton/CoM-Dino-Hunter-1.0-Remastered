using System.Collections.Generic;

namespace gyTaskSystem
{
	public class CTaskCollection : CTaskBase
	{
		public class CCollect
		{
			public int nItemID;

			public int nCurCount;

			public int nMaxCount;

			public CCollect(int nItemID, int nMaxCount)
			{
				this.nItemID = nItemID;
				this.nMaxCount = nMaxCount;
				nCurCount = 0;
			}

			public bool IsMax()
			{
				return nCurCount >= nMaxCount;
			}
		}

		public List<CCollect> m_ltCollections;

		public CTaskCollection()
		{
			m_ltCollections = new List<CCollect>();
		}

		public override void Initialize(CTaskInfo taskinfo)
		{
			base.Initialize(taskinfo);
			CTaskInfoCollect cTaskInfoCollect = m_curTaskInfo as CTaskInfoCollect;
			if (cTaskInfoCollect == null)
			{
				return;
			}
			foreach (CTaskInfoCollect.CCollectInfo item in cTaskInfoCollect.ltCollectInfo)
			{
				CCollect cCollect = new CCollect(item.nItemID, item.nMaxCount);
				if (cCollect != null)
				{
					m_ltCollections.Add(cCollect);
				}
			}
		}

		public override void Reset()
		{
			base.Reset();
			foreach (CCollect ltCollection in m_ltCollections)
			{
				ltCollection.nCurCount = 0;
			}
		}

		public override void OnGetItem(int nItemID, int nCount = 1)
		{
			int num = 0;
			foreach (CCollect ltCollection in m_ltCollections)
			{
				if (ltCollection.nItemID == nItemID)
				{
					ltCollection.nCurCount += nCount;
					if (ltCollection.nCurCount > ltCollection.nMaxCount)
					{
						ltCollection.nCurCount = ltCollection.nMaxCount;
					}
					base.isUpdateData = true;
				}
				if (ltCollection.IsMax())
				{
					num++;
				}
			}
			if (num == m_ltCollections.Count)
			{
				TaskCompleted();
			}
		}
	}
}
