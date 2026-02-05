using System.Collections;
using System.Collections.Generic;

namespace gyAchievementSystem
{
	public class CAchievementManager
	{
		protected static CAchievementManager m_Instance;

		protected CAchievementCenter m_AchievementCenter;

		protected List<CAchievementTip> m_ltAchievementTip;

		protected bool m_bNeedSave;

		public static CAchievementManager GetInstance()
		{
			if (m_Instance == null)
			{
				m_Instance = new CAchievementManager();
				m_Instance.Initialize();
			}
			return m_Instance;
		}

		public CAchievementTip PopTip()
		{
			CAchievementTip result = m_ltAchievementTip[0];
			m_ltAchievementTip.RemoveAt(0);
			return result;
		}

		public int GetTipCount()
		{
			return m_ltAchievementTip.Count;
		}

		public void Initialize()
		{
			m_AchievementCenter = new CAchievementCenter();
			m_AchievementCenter.LoadInfo();
			m_AchievementCenter.LoadData();
			m_ltAchievementTip = new List<CAchievementTip>();
		}

		public void Update(float deltaTime)
		{
		}

		protected void SetAchievementCount(int nID, int nCount)
		{
			CAchievementInfo info = m_AchievementCenter.GetInfo(nID);
			if (info == null)
			{
				return;
			}
			CAchievementData data = m_AchievementCenter.GetData(nID);
			if (data == null)
			{
				return;
			}
			int nCurValue = data.nCurValue;
			data.nCurValue = nCount;
			int stepCount = info.GetStepCount();
			for (int i = 0; i < stepCount; i++)
			{
				CAchievementStep step = info.GetStep(i);
				if (step != null && nCurValue < step.nStepPurpose && data.nCurValue >= step.nStepPurpose)
				{
					AddAchievementTip(info.nID, info.sName, i + 1);
					if (i == stepCount - 1)
					{
						data.nState = 2;
					}
					m_bNeedSave = true;
					break;
				}
			}
		}

		protected void AddAchievementTip(int nID, string sName, int nStep)
		{
			CAchievementTip cAchievementTip = new CAchievementTip();
			if (cAchievementTip != null)
			{
				cAchievementTip.nID = nID;
				cAchievementTip.sName = sName;
				cAchievementTip.nStep = nStep;
				m_ltAchievementTip.Add(cAchievementTip);
				//iGameApp.GetInstance().Flurry_GainAchi(nID, nStep);
			}
		}

		protected IEnumerable GetAchievementData()
		{
			Dictionary<int, CAchievementInfo> dictAchievementInfo = m_AchievementCenter.GetDataInfo();
			if (dictAchievementInfo == null)
			{
				yield break;
			}
			foreach (CAchievementInfo info in dictAchievementInfo.Values)
			{
				CAchievementData data = m_AchievementCenter.GetData(info.nID);
				if (data == null)
				{
					data = new CAchievementData
					{
						nID = info.nID,
						nState = 1,
						nCurValue = 0
					};
					m_AchievementCenter.AddData(data.nID, data);
				}
				if (data.nState == 1)
				{
					yield return data;
				}
			}
		}

		public CAchievementCenter GetAchievementCenter()
		{
			return m_AchievementCenter;
		}

		public void Save()
		{
			if (m_AchievementCenter != null)
			{
				m_AchievementCenter.SaveData();
			}
		}

		public int GetAchiStar(int nAchiID)
		{
			if (m_AchievementCenter == null)
			{
				return 0;
			}
			CAchievementInfo info = m_AchievementCenter.GetInfo(nAchiID);
			CAchievementData data = m_AchievementCenter.GetData(nAchiID);
			if (info == null || data == null)
			{
				return 0;
			}
			int stepCount = info.GetStepCount();
			for (int i = 0; i < stepCount; i++)
			{
				CAchievementStep step = info.GetStep(i);
				if (step != null && data.nCurValue < step.nStepPurpose)
				{
					return i;
				}
			}
			return stepCount;
		}

		public void AddAchievement(int nAchiType, object[] arrParam = null)
		{
			foreach (CAchievementData achievementDatum in GetAchievementData())
			{
				CAchievementInfo info = m_AchievementCenter.GetInfo(achievementDatum.nID);
				if (info == null || info.nType != nAchiType)
				{
					continue;
				}
				switch (info.nType)
				{
				case 6:
					if (arrParam != null && arrParam.Length == 2)
					{
						int num2 = (int)arrParam[0];
						int num3 = (int)arrParam[1];
						int nValue2 = -1;
						if (info.GetParam(0, ref nValue2) && num2 == nValue2 && info.GetParam(1, ref nValue2) && num3 == nValue2)
						{
							SetAchievementCount(info.nID, achievementDatum.nCurValue + 1);
						}
					}
					break;
				case 7:
					if (arrParam != null && arrParam.Length == 1)
					{
						int num = (int)arrParam[0];
						int nValue = -1;
						if (info.GetParam(0, ref nValue) && num == nValue)
						{
							SetAchievementCount(info.nID, achievementDatum.nCurValue + 1);
						}
					}
					break;
				case 1:
					if (arrParam != null && arrParam.Length == 1)
					{
						int nCount2 = (int)arrParam[0];
						SetAchievementCount(info.nID, nCount2);
					}
					break;
				case 2:
					if (arrParam != null && arrParam.Length == 1)
					{
						int nCount = (int)arrParam[0];
						SetAchievementCount(info.nID, nCount);
					}
					break;
				default:
					SetAchievementCount(info.nID, achievementDatum.nCurValue + 1);
					break;
				}
			}
			if (m_bNeedSave)
			{
				m_bNeedSave = false;
				m_AchievementCenter.SaveData();
			}
		}
	}
}
