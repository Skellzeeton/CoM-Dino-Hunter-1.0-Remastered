using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class iGameLevelCenter
{
	protected Dictionary<int, GameLevelInfo> m_dictGameLevelInfo;

	public iGameLevelCenter()
	{
		m_dictGameLevelInfo = new Dictionary<int, GameLevelInfo>();
	}

	public bool Load()
	{
		string content = string.Empty;
		if (MyUtils.isWindows)
		{
			if (!Utils.FileGetString("gamelevel.xml", ref content))
			{
				return false;
			}
		}
		else if (MyUtils.isIOS || MyUtils.isAndroid)
		{
			TextAsset textAsset = (TextAsset)Resources.Load(PrefabManager.GetPath(3007), typeof(TextAsset));
			if (textAsset == null)
			{
				return false;
			}
			content = textAsset.ToString();
		}
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(content);
		string value = string.Empty;
		XmlNode documentElement = xmlDocument.DocumentElement;
		foreach (XmlNode childNode in documentElement.ChildNodes)
		{
			if (childNode.Name != "gamelevel" || !MyUtils.GetAttribute(childNode, "id", ref value))
			{
				continue;
			}
			GameLevelInfo gameLevelInfo = new GameLevelInfo();
			gameLevelInfo.nID = int.Parse(value);
			if (MyUtils.GetAttribute(childNode, "scenename", ref value))
			{
				gameLevelInfo.sSceneName = value;
			}
			if (MyUtils.GetAttribute(childNode, "isskyscene", ref value))
			{
				gameLevelInfo.bIsSkyScene = bool.Parse(value);
			}
			if (MyUtils.GetAttribute(childNode, "name", ref value))
			{
				gameLevelInfo.sLevelName = value;
			}
			else
			{
				gameLevelInfo.sLevelName = "Level " + gameLevelInfo.nID;
			}
			if (MyUtils.GetAttribute(childNode, "desc", ref value))
			{
				gameLevelInfo.sLevelDesc = value;
			}
			else
			{
				gameLevelInfo.sLevelDesc = "This is desc of Level " + gameLevelInfo.nID;
			}
			if (MyUtils.GetAttribute(childNode, "icon", ref value))
			{
				gameLevelInfo.sIcon = value;
			}
			if (MyUtils.GetAttribute(childNode, "nav_plane", ref value))
			{
				gameLevelInfo.fNavPlane = ServerX.ParseFloat(value);
			}
			if (MyUtils.GetAttribute(childNode, "bp_cfg", ref value))
			{
				gameLevelInfo.nBirthPos = int.Parse(value);
			}
			if (MyUtils.GetAttribute(childNode, "gamewave", ref value))
			{
				string[] array = value.Split(',');
				for (int i = 0; i < array.Length; i++)
				{
					gameLevelInfo.ltGameWave.Add(int.Parse(array[i]));
				}
			}
			if (MyUtils.GetAttribute(childNode, "sp_cfg_sky", ref value))
			{
				gameLevelInfo.nDefaultSPSky = int.Parse(value);
			}
			if (MyUtils.GetAttribute(childNode, "sp_cfg_ground", ref value))
			{
				gameLevelInfo.nDefaultSPGround = int.Parse(value);
			}
			if (MyUtils.GetAttribute(childNode, "hp_cfg_def", ref value))
			{
				gameLevelInfo.nDefaultHoverPoint = int.Parse(value);
			}
			if (MyUtils.GetAttribute(childNode, "tpbegin_cfg", ref value))
			{
				gameLevelInfo.nTPBeginCfg = int.Parse(value);
			}
			if (MyUtils.GetAttribute(childNode, "tpend_cfg", ref value))
			{
				gameLevelInfo.nTPEndCfg = int.Parse(value);
			}
			if (MyUtils.GetAttribute(childNode, "task", ref value))
			{
				gameLevelInfo.nTaskID = int.Parse(value);
			}
			if (MyUtils.GetAttribute(childNode, "exp", ref value))
			{
				gameLevelInfo.nRewardExp = int.Parse(value);
			}
			if (MyUtils.GetAttribute(childNode, "gold", ref value))
			{
				gameLevelInfo.nRewardGold = int.Parse(value);
			}
			gameLevelInfo.ltRewardMaterial.Clear();
			if (MyUtils.GetAttribute(childNode, "cutscene", ref value))
			{
				gameLevelInfo.sCutScene = value;
			}
			if (MyUtils.GetAttribute(childNode, "cutscenecontent", ref value))
			{
				gameLevelInfo.sCutSceneContent = value;
			}
			if (MyUtils.GetAttribute(childNode, "cutscene_ambience", ref value))
			{
				gameLevelInfo.sCutSceneAmbience = value;
			}
			if (MyUtils.GetAttribute(childNode, "BGM", ref value))
			{
				gameLevelInfo.sBGM = value;
			}
			if (MyUtils.GetAttribute(childNode, "BGM_ambience", ref value))
			{
				gameLevelInfo.sBGMAmbience = value;
			}
			if (MyUtils.GetAttribute(childNode, "proccess", ref value))
			{
				gameLevelInfo.m_nProccess = int.Parse(value);
			}
			if (MyUtils.GetAttribute(childNode, "limitmelee", ref value))
			{
				gameLevelInfo.m_bLimitMelee = bool.Parse(value);
			}
			if (MyUtils.GetAttribute(childNode, "issidelevel", ref value))
			{
				gameLevelInfo.bIsSideLevel = bool.Parse(value);
			}
			else
			{
				gameLevelInfo.bIsSideLevel = true;
			}
			foreach (XmlNode childNode2 in childNode.ChildNodes)
			{
				if (childNode2.Name == "MonsterNumLimit")
				{
					foreach (XmlNode childNode3 in childNode2.ChildNodes)
					{
						if (!(childNode3.Name != "limit"))
						{
							MonsterNumLimitInfo monsterNumLimitInfo = new MonsterNumLimitInfo();
							if (MyUtils.GetAttribute(childNode3, "type", ref value))
							{
								monsterNumLimitInfo.nLimitType = int.Parse(value);
							}
							if (MyUtils.GetAttribute(childNode3, "value", ref value))
							{
								monsterNumLimitInfo.nLimitValue = int.Parse(value);
							}
							if (MyUtils.GetAttribute(childNode3, "maxnumber", ref value))
							{
								monsterNumLimitInfo.nMax = int.Parse(value);
							}
							gameLevelInfo.ltMonsterNumLimit.Add(monsterNumLimitInfo);
						}
					}
				}
				else if (childNode2.Name == "trigger_sp")
				{
					if (!MyUtils.GetAttribute(childNode2, "trigger", ref value))
					{
						continue;
					}
					StartPointTrigger startPointTrigger = new StartPointTrigger();
					startPointTrigger.m_Trigger = new TriggerInfo();
					startPointTrigger.m_Trigger.nEventType = int.Parse(value);
					if (MyUtils.GetAttribute(childNode2, "triggervalue", ref value))
					{
						string[] array = value.Split(',');
						for (int j = 0; j < array.Length; j++)
						{
							startPointTrigger.m_Trigger.ltEventParam.Add(int.Parse(array[j]));
						}
					}
					if (MyUtils.GetAttribute(childNode2, "triggerloop", ref value))
					{
						startPointTrigger.m_Trigger.bEventLoop = bool.Parse(value);
					}
					if (MyUtils.GetAttribute(childNode2, "sp_cfg", ref value))
					{
						startPointTrigger.m_nStartPointCfg = int.Parse(value);
					}
				}
				else if (childNode2.Name == "trigger_hp")
				{
					if (!MyUtils.GetAttribute(childNode2, "trigger", ref value))
					{
						continue;
					}
					StartPointTrigger startPointTrigger2 = new StartPointTrigger();
					startPointTrigger2.m_Trigger = new TriggerInfo();
					startPointTrigger2.m_Trigger.nEventType = int.Parse(value);
					if (MyUtils.GetAttribute(childNode2, "triggervalue", ref value))
					{
						string[] array = value.Split(',');
						for (int k = 0; k < array.Length; k++)
						{
							startPointTrigger2.m_Trigger.ltEventParam.Add(int.Parse(array[k]));
						}
					}
					if (MyUtils.GetAttribute(childNode2, "triggerloop", ref value))
					{
						startPointTrigger2.m_Trigger.bEventLoop = bool.Parse(value);
					}
					if (MyUtils.GetAttribute(childNode2, "hp_cfg", ref value))
					{
						startPointTrigger2.m_nStartPointCfg = int.Parse(value);
					}
				}
				else if (childNode2.Name == "RewardMaterial")
				{
					CRewardMaterial cRewardMaterial = new CRewardMaterial();
					if (MyUtils.GetAttribute(childNode2, "material", ref value))
					{
						cRewardMaterial.nID = int.Parse(value);
					}
					if (MyUtils.GetAttribute(childNode2, "countlist", ref value))
					{
						string[] array = value.Split(',');
						for (int l = 0; l < array.Length; l++)
						{
							cRewardMaterial.ltCount.Add(int.Parse(array[l]));
						}
					}
					if (MyUtils.GetAttribute(childNode2, "countratelist", ref value))
					{
						string[] array = value.Split(',');
						for (int m = 0; m < array.Length; m++)
						{
							cRewardMaterial.ltCountRate.Add(int.Parse(array[m]));
						}
					}
					gameLevelInfo.ltRewardMaterial.Add(cRewardMaterial);
				}
				else if (childNode2.Name == "recommand")
				{
					if (MyUtils.GetAttribute(childNode2, "recommandtype", ref value))
					{
						gameLevelInfo.m_nRecommandType = int.Parse(value);
					}
					if (MyUtils.GetAttribute(childNode2, "recommandid", ref value))
					{
						gameLevelInfo.m_nRecommandID = int.Parse(value);
					}
					if (MyUtils.GetAttribute(childNode2, "recommandlevel", ref value))
					{
						gameLevelInfo.m_nRecommandLevel = int.Parse(value);
					}
					if (MyUtils.GetAttribute(childNode2, "islimit", ref value))
					{
						gameLevelInfo.m_bRecommandLimit = bool.Parse(value);
					}
				}
			}
			m_dictGameLevelInfo.Add(gameLevelInfo.nID, gameLevelInfo);
		}
		return true;
	}

	public Dictionary<int, GameLevelInfo> GetData()
	{
		return m_dictGameLevelInfo;
	}

	public GameLevelInfo Get(int nID)
	{
		if (!m_dictGameLevelInfo.ContainsKey(nID))
		{
			return null;
		}
		return m_dictGameLevelInfo[nID];
	}
}
