using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class iDropGroupCenter
{
	protected Dictionary<int, CDropGroupInfo> m_dictDropGroupInfo;

	public iDropGroupCenter()
	{
		m_dictDropGroupInfo = new Dictionary<int, CDropGroupInfo>();
	}

	public CDropGroupInfo Get(int nID)
	{
		if (!m_dictDropGroupInfo.ContainsKey(nID))
		{
			return null;
		}
		return m_dictDropGroupInfo[nID];
	}

	public bool Load()
	{
		string content = string.Empty;
		if (MyUtils.isWindows)
		{
			if (!Utils.FileGetString("dropgroup.xml", ref content))
			{
				return false;
			}
		}
		else if (MyUtils.isIOS || MyUtils.isAndroid)
		{
			TextAsset textAsset = (TextAsset)Resources.Load(PrefabManager.GetPath(3013), typeof(TextAsset));
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
			if (childNode.Name != "group" || !MyUtils.GetAttribute(childNode, "id", ref value))
			{
				continue;
			}
			CDropGroupInfo cDropGroupInfo = new CDropGroupInfo();
			cDropGroupInfo.nID = int.Parse(value);
			if (MyUtils.GetAttribute(childNode, "item", ref value))
			{
				cDropGroupInfo.ltItem.Clear();
				string[] array = value.Split(',');
				for (int i = 0; i < array.Length; i++)
				{
					cDropGroupInfo.ltItem.Add(new CDropItem(int.Parse(array[i]), 0f));
				}
			}
			if (MyUtils.GetAttribute(childNode, "rate", ref value))
			{
				string[] array = value.Split(',');
				for (int j = 0; j < array.Length && j < cDropGroupInfo.ltItem.Count; j++)
				{
					cDropGroupInfo.ltItem[j].fRate = ServerX.ParseFloat(array[j]);
				}
			}
			m_dictDropGroupInfo.Add(cDropGroupInfo.nID, cDropGroupInfo);
		}
		return true;
	}
}
