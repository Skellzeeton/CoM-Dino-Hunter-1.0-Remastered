using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace gyIAPSystem
{
	public class CIAPCenter
	{
		protected Dictionary<int, CIAPInfo> m_dictIAPInfo;

		protected Dictionary<int, CCrystal2GoldInfo> m_dictCrystal2GoldInfo;

		public CIAPCenter()
		{
			m_dictIAPInfo = new Dictionary<int, CIAPInfo>();
			m_dictCrystal2GoldInfo = new Dictionary<int, CCrystal2GoldInfo>();
		}

		public Dictionary<int, CIAPInfo> GetData()
		{
			return m_dictIAPInfo;
		}

		public CIAPInfo Get(int nID)
		{
			if (!m_dictIAPInfo.ContainsKey(nID))
			{
				return null;
			}
			return m_dictIAPInfo[nID];
		}

		public CIAPInfo GetByKey(string key)
		{
			foreach (CIAPInfo value in m_dictIAPInfo.Values)
			{
				if (value.sKey == key)
				{
					return value;
				}
			}
			return null;
		}

		public CCrystal2GoldInfo GetCrystal2GoldInfo(int nID)
		{
			if (!m_dictCrystal2GoldInfo.ContainsKey(nID))
			{
				return null;
			}
			return m_dictCrystal2GoldInfo[nID];
		}

		public bool Load()
		{
			string content = string.Empty;
			if (MyUtils.isWindows)
			{
				if (!Utils.FileGetString("iap.xml", ref content))
				{
					return false;
				}
			}
			else if (MyUtils.isIOS || MyUtils.isAndroid)
			{
				TextAsset textAsset = (TextAsset)Resources.Load("_Config/iap", typeof(TextAsset));
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
				if (!(childNode.Name != "iap") && GetAttribute(childNode, "id", ref value))
				{
					CIAPInfo cIAPInfo = new CIAPInfo();
					cIAPInfo.nID = int.Parse(value);
					if (GetAttribute(childNode, "key", ref value))
					{
						cIAPInfo.sKey = value;
					}
					if (GetAttribute(childNode, "iscrystal", ref value))
					{
						cIAPInfo.isCrystal = bool.Parse(value);
					}
					if (GetAttribute(childNode, "value", ref value))
					{
						cIAPInfo.nValue = int.Parse(value);
					}
					m_dictIAPInfo.Add(cIAPInfo.nID, cIAPInfo);
				}
			}
			return true;
		}

		public bool LoadCrystal2Gold()
		{
			CCrystal2GoldInfo cCrystal2GoldInfo = null;
			cCrystal2GoldInfo = new CCrystal2GoldInfo();
			cCrystal2GoldInfo.nID = 1;
			cCrystal2GoldInfo.nGold = 2000;
			cCrystal2GoldInfo.nCrystal = 50;
			m_dictCrystal2GoldInfo.Add(cCrystal2GoldInfo.nID, cCrystal2GoldInfo);
			cCrystal2GoldInfo = new CCrystal2GoldInfo();
			cCrystal2GoldInfo.nID = 2;
			cCrystal2GoldInfo.nGold = 6000;
			cCrystal2GoldInfo.nCrystal = 124;
			m_dictCrystal2GoldInfo.Add(cCrystal2GoldInfo.nID, cCrystal2GoldInfo);
			cCrystal2GoldInfo = new CCrystal2GoldInfo();
			cCrystal2GoldInfo.nID = 3;
			cCrystal2GoldInfo.nGold = 14000;
			cCrystal2GoldInfo.nCrystal = 247;
			m_dictCrystal2GoldInfo.Add(cCrystal2GoldInfo.nID, cCrystal2GoldInfo);
			cCrystal2GoldInfo = new CCrystal2GoldInfo();
			cCrystal2GoldInfo.nID = 4;
			cCrystal2GoldInfo.nGold = 32000;
			cCrystal2GoldInfo.nCrystal = 480;
			m_dictCrystal2GoldInfo.Add(cCrystal2GoldInfo.nID, cCrystal2GoldInfo);
			cCrystal2GoldInfo = new CCrystal2GoldInfo();
			cCrystal2GoldInfo.nID = 5;
			cCrystal2GoldInfo.nGold = 100000;
			cCrystal2GoldInfo.nCrystal = 1199;
			m_dictCrystal2GoldInfo.Add(cCrystal2GoldInfo.nID, cCrystal2GoldInfo);
			return true;
		}

		protected bool GetAttribute(XmlNode node, string name, ref string value)
		{
			if (node == null || node.Attributes[name] == null)
			{
				return false;
			}
			value = node.Attributes[name].Value.Trim();
			if (value.Length < 1)
			{
				return false;
			}
			return true;
		}
	}
}
