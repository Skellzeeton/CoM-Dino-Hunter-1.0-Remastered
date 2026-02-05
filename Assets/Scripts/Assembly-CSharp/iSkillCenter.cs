using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class iSkillCenter
{
	protected Dictionary<int, CSkillInfo> m_dictSkillInfo;

	protected Dictionary<int, CSkillComboInfo> m_dictSkillComboInfo;

	public iSkillCenter()
	{
		m_dictSkillInfo = new Dictionary<int, CSkillInfo>();
		m_dictSkillComboInfo = new Dictionary<int, CSkillComboInfo>();
	}

	public Dictionary<int, CSkillInfo> GetDataSkillInfo()
	{
		return m_dictSkillInfo;
	}

	public CSkillInfo GetSkillInfo(int nID)
	{
		if (!m_dictSkillInfo.ContainsKey(nID))
		{
			return null;
		}
		return m_dictSkillInfo[nID];
	}

	public CSkillInfoLevel GetSkillInfo(int nID, int nLevel)
	{
		CSkillInfo skillInfo = GetSkillInfo(nID);
		if (skillInfo == null)
		{
			return null;
		}
		return skillInfo.Get(nLevel);
	}

	public CSkillComboInfo GetSkillComboInfo(int nComboID)
	{
		if (!m_dictSkillComboInfo.ContainsKey(nComboID))
		{
			return null;
		}
		return m_dictSkillComboInfo[nComboID];
	}

	public bool Load_Monster()
	{
		string content = string.Empty;
		if (MyUtils.isWindows)
		{
			if (!Utils.FileGetString("skillmonster.xml", ref content))
			{
				return false;
			}
		}
		else if (MyUtils.isIOS || MyUtils.isAndroid)
		{
			TextAsset textAsset = (TextAsset)Resources.Load(PrefabManager.GetPath(3003), typeof(TextAsset));
			if (textAsset == null)
			{
				return false;
			}
			content = textAsset.ToString();
		}
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(content);
		XmlNode documentElement = xmlDocument.DocumentElement;
		foreach (XmlNode childNode in documentElement.ChildNodes)
		{
			if (childNode.Name == "skillinfo")
			{
				LoadSkillInfo(childNode);
			}
			else if (childNode.Name == "comboinfo")
			{
				LoadSkillComboInfo(childNode);
			}
		}
		return true;
	}

	public bool Load_Player()
	{
		string content = string.Empty;
		if (MyUtils.isWindows)
		{
			if (!Utils.FileGetString("skillplayer.xml", ref content))
			{
				return false;
			}
		}
		else if (MyUtils.isIOS || MyUtils.isAndroid)
		{
			TextAsset textAsset = (TextAsset)Resources.Load(PrefabManager.GetPath(3002), typeof(TextAsset));
			if (textAsset == null)
			{
				return false;
			}
			content = textAsset.ToString();
		}
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(content);
		XmlNode documentElement = xmlDocument.DocumentElement;
		foreach (XmlNode childNode in documentElement.ChildNodes)
		{
			if (childNode.Name == "skillinfo")
			{
				LoadSkillInfo(childNode);
			}
		}
		return true;
	}

	protected void LoadSkillInfo(XmlNode root)
	{
		string value = string.Empty;
		foreach (XmlNode childNode in root.ChildNodes)
		{
			if (childNode.Name != "skill" || !MyUtils.GetAttribute(childNode, "id", ref value))
			{
				continue;
			}
			int num = int.Parse(value);
			if (!MyUtils.GetAttribute(childNode, "lvl", ref value))
			{
				continue;
			}
			int nLevel = int.Parse(value);
			CSkillInfo cSkillInfo = GetSkillInfo(num);
			if (cSkillInfo == null)
			{
				cSkillInfo = new CSkillInfo();
				cSkillInfo.nID = num;
				m_dictSkillInfo.Add(num, cSkillInfo);
			}
			CSkillInfoLevel cSkillInfoLevel = cSkillInfo.Get(nLevel);
			if (cSkillInfoLevel == null)
			{
				cSkillInfoLevel = new CSkillInfoLevel();
				cSkillInfoLevel.nID = num;
				cSkillInfoLevel.nLevel = nLevel;
				cSkillInfo.Add(nLevel, cSkillInfoLevel);
			}
			if (MyUtils.GetAttribute(childNode, "type", ref value))
			{
				cSkillInfoLevel.nType = int.Parse(value);
			}
			if (MyUtils.GetAttribute(childNode, "rangetype", ref value))
			{
				cSkillInfoLevel.nRangeType = int.Parse(value);
			}
			if (MyUtils.GetAttribute(childNode, "rangetypevalue", ref value))
			{
				cSkillInfoLevel.ltRangeValue.Clear();
				string[] array = value.Split(',');
				for (int i = 0; i < array.Length; i++)
				{
					cSkillInfoLevel.ltRangeValue.Add(ServerX.ParseFloat(array[i]));
				}
			}
			if (MyUtils.GetAttribute(childNode, "targetlimit", ref value))
			{
				cSkillInfoLevel.nTargetLimit = int.Parse(value);
			}
			if (MyUtils.GetAttribute(childNode, "skillmode", ref value))
			{
				cSkillInfoLevel.nSkillMode = int.Parse(value);
			}
			if (MyUtils.GetAttribute(childNode, "skillmodevalue", ref value))
			{
				cSkillInfoLevel.ltSkillModeValue.Clear();
				string[] array = value.Split(',');
				for (int j = 0; j < array.Length; j++)
				{
					cSkillInfoLevel.ltSkillModeValue.Add(ServerX.ParseFloat(array[j]));
				}
			}
			if (MyUtils.GetAttribute(childNode, "name", ref value))
			{
				cSkillInfoLevel.sName = value;
			}
			else
			{
				cSkillInfoLevel.sName = "Skill " + cSkillInfoLevel.nID;
			}
			if (MyUtils.GetAttribute(childNode, "desc", ref value))
			{
				cSkillInfoLevel.sDesc = value;
			}
			else
			{
				cSkillInfoLevel.sDesc = "This is desc of Skill " + cSkillInfoLevel.nID;
			}
			if (MyUtils.GetAttribute(childNode, "icon", ref value))
			{
				cSkillInfoLevel.sIcon = value;
			}
			if (MyUtils.GetAttribute(childNode, "targetmax", ref value))
			{
				cSkillInfoLevel.nTargetMax = int.Parse(value);
			}
			if (MyUtils.GetAttribute(childNode, "func", ref value))
			{
				string[] array = value.Split(',');
				for (int k = 0; k < array.Length && k < cSkillInfoLevel.arrFunc.Length; k++)
				{
					cSkillInfoLevel.arrFunc[k] = int.Parse(array[k]);
				}
			}
			if (MyUtils.GetAttribute(childNode, "valuex", ref value))
			{
				string[] array = value.Split(',');
				for (int l = 0; l < array.Length && l < cSkillInfoLevel.arrValueX.Length; l++)
				{
					cSkillInfoLevel.arrValueX[l] = int.Parse(array[l]);
				}
			}
			if (MyUtils.GetAttribute(childNode, "valuey", ref value))
			{
				string[] array = value.Split(',');
				for (int m = 0; m < array.Length && m < cSkillInfoLevel.arrValueY.Length; m++)
				{
					cSkillInfoLevel.arrValueY[m] = int.Parse(array[m]);
				}
			}
			if (MyUtils.GetAttribute(childNode, "action", ref value))
			{
				cSkillInfoLevel.nAnim = int.Parse(value);
			}
			if (MyUtils.GetAttribute(childNode, "useaudio", ref value))
			{
				cSkillInfoLevel.sUseAudio = value;
			}
			if (MyUtils.GetAttribute(childNode, "unlocklevel", ref value))
			{
				cSkillInfo.nUnlockLevel = int.Parse(value);
			}
			if (MyUtils.GetAttribute(childNode, "iscrystalunlock", ref value))
			{
				cSkillInfo.isCrystalUnlock = bool.Parse(value);
			}
			if (MyUtils.GetAttribute(childNode, "unlockprice", ref value))
			{
				cSkillInfo.nUnlockPrice = int.Parse(value);
			}
			if (MyUtils.GetAttribute(childNode, "iscrystalpurchase", ref value))
			{
				cSkillInfoLevel.isCrystalPurchase = bool.Parse(value);
			}
			if (MyUtils.GetAttribute(childNode, "purchaseprice", ref value))
			{
				cSkillInfoLevel.nPurchasePrice = int.Parse(value);
			}
			if (MyUtils.GetAttribute(childNode, "levelupdesc", ref value))
			{
				cSkillInfoLevel.sLevelUpDesc = value;
			}
		}
	}

	protected void LoadSkillComboInfo(XmlNode root)
	{
		string value = string.Empty;
		foreach (XmlNode childNode in root.ChildNodes)
		{
			if (childNode.Name != "combo" || !MyUtils.GetAttribute(childNode, "id", ref value))
			{
				continue;
			}
			CSkillComboInfo cSkillComboInfo = new CSkillComboInfo();
			cSkillComboInfo.nID = int.Parse(value);
			if (MyUtils.GetAttribute(childNode, "ignorecombolimit", ref value))
			{
				cSkillComboInfo.isIgnoreComboLimit = bool.Parse(value);
			}
			if (MyUtils.GetAttribute(childNode, "coldown", ref value))
			{
				cSkillComboInfo.fCoolDown = ServerX.ParseFloat(value);
			}
			if (MyUtils.GetAttribute(childNode, "skilllist", ref value))
			{
				string[] array = value.Split(',');
				for (int i = 0; i < array.Length; i++)
				{
					cSkillComboInfo.ltSkill.Add(int.Parse(array[i]));
				}
			}
			if (MyUtils.GetAttribute(childNode, "freezetime", ref value))
			{
				cSkillComboInfo.fFreezeTime = ServerX.ParseFloat(value);
			}
			m_dictSkillComboInfo.Add(cSkillComboInfo.nID, cSkillComboInfo);
		}
	}
}
