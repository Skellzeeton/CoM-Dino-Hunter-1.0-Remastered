using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class iLoadTipCenter
{
	protected List<CLoadTipInfo> m_ltLoadTipInfo;

	public iLoadTipCenter()
	{
		m_ltLoadTipInfo = new List<CLoadTipInfo>();
	}

	public CLoadTipInfo GetRandom()
	{
		if (m_ltLoadTipInfo == null)
		{
			return null;
		}
		return m_ltLoadTipInfo[Random.Range(0, m_ltLoadTipInfo.Count)];
	}

	public bool Load()
	{
		string content = string.Empty;
		if (MyUtils.isWindows)
		{
			if (!Utils.FileGetString("loadtip.xml", ref content))
			{
				return false;
			}
		}
		else if (MyUtils.isIOS || MyUtils.isAndroid)
		{
			TextAsset textAsset = (TextAsset)Resources.Load("_Config/loadtip", typeof(TextAsset));
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
			if (!(childNode.Name != "tip"))
			{
				CLoadTipInfo cLoadTipInfo = new CLoadTipInfo();
				if (MyUtils.GetAttribute(childNode, "icon", ref value))
				{
					cLoadTipInfo.sIcon = value;
				}
				if (MyUtils.GetAttribute(childNode, "desc", ref value))
				{
					cLoadTipInfo.sDesc = value;
				}
				m_ltLoadTipInfo.Add(cLoadTipInfo);
			}
		}
		return true;
	}
}
