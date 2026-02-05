using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class iBuffCenter
{
	protected Dictionary<int, CBuffInfo> m_dictBuff;

	public iBuffCenter()
	{
		m_dictBuff = new Dictionary<int, CBuffInfo>();
	}

	public CBuffInfo GetBuffInfo(int nID)
	{
		if (!m_dictBuff.ContainsKey(nID))
		{
			return null;
		}
		return m_dictBuff[nID];
	}

	public bool Load()
	{
		string content = string.Empty;
		if (MyUtils.isWindows)
		{
			if (!Utils.FileGetString("buff.xml", ref content))
			{
				return false;
			}
		}
		else if (MyUtils.isIOS || MyUtils.isAndroid)
		{
			TextAsset textAsset = (TextAsset)Resources.Load(PrefabManager.GetPath(3005), typeof(TextAsset));
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
			if (childNode.Name != "buff" || !MyUtils.GetAttribute(childNode, "id", ref value))
			{
				continue;
			}
			CBuffInfo cBuffInfo = new CBuffInfo();
			cBuffInfo.nID = int.Parse(value);
			if (MyUtils.GetAttribute(childNode, "type", ref value))
			{
				cBuffInfo.nType = int.Parse(value);
			}
			if (MyUtils.GetAttribute(childNode, "slot", ref value))
			{
				cBuffInfo.nSlot = int.Parse(value);
			}
			if (MyUtils.GetAttribute(childNode, "priority", ref value))
			{
				cBuffInfo.nPriority = int.Parse(value);
			}
			if (MyUtils.GetAttribute(childNode, "functime", ref value))
			{
				cBuffInfo.fEffectTime = ServerX.ParseFloat(value);
			}
			if (MyUtils.GetAttribute(childNode, "effhold", ref value))
			{
				string[] array = value.Split(',');
				if (array.Length == 2)
				{
					cBuffInfo.arrEffHold[0] = int.Parse(array[0]);
					cBuffInfo.arrEffHold[1] = int.Parse(array[1]);
				}
			}
			if (MyUtils.GetAttribute(childNode, "effadd", ref value))
			{
				string[] array = value.Split(',');
				if (array.Length == 2)
				{
					cBuffInfo.arrEffAdd[0] = int.Parse(array[0]);
					cBuffInfo.arrEffAdd[1] = int.Parse(array[1]);
				}
			}
			if (MyUtils.GetAttribute(childNode, "effdel", ref value))
			{
				string[] array = value.Split(',');
				if (array.Length == 2)
				{
					cBuffInfo.arrEffDel[0] = int.Parse(array[0]);
					cBuffInfo.arrEffDel[1] = int.Parse(array[1]);
				}
			}
			if (MyUtils.GetAttribute(childNode, "audioeffhold", ref value))
			{
				cBuffInfo.sAudioEffHold = value;
			}
			if (MyUtils.GetAttribute(childNode, "audioeffadd", ref value))
			{
				cBuffInfo.sAudioEffAdd = value;
			}
			if (MyUtils.GetAttribute(childNode, "audioeffdel", ref value))
			{
				cBuffInfo.sAudioEffDel = value;
			}
			if (MyUtils.GetAttribute(childNode, "func", ref value))
			{
				string[] array = value.Split(',');
				for (int i = 0; i < array.Length && i < cBuffInfo.arrFunc.Length; i++)
				{
					cBuffInfo.arrFunc[i] = int.Parse(array[i]);
				}
			}
			if (MyUtils.GetAttribute(childNode, "valuex", ref value))
			{
				string[] array = value.Split(',');
				for (int j = 0; j < array.Length && j < cBuffInfo.arrValueX.Length; j++)
				{
					cBuffInfo.arrValueX[j] = int.Parse(array[j]);
				}
			}
			if (MyUtils.GetAttribute(childNode, "valuey", ref value))
			{
				string[] array = value.Split(',');
				for (int k = 0; k < array.Length && k < cBuffInfo.arrValueY.Length; k++)
				{
					cBuffInfo.arrValueY[k] = int.Parse(array[k]);
				}
			}
			m_dictBuff.Add(cBuffInfo.nID, cBuffInfo);
		}
		return true;
	}
}
