using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class iMGCenter
{
	protected Dictionary<int, WaveInfo> m_dictWaveInfo;

	public iMGCenter()
	{
		m_dictWaveInfo = new Dictionary<int, WaveInfo>();
	}

	public bool Load()
	{
		string content = string.Empty;
		if (MyUtils.isWindows)
		{
			if (!Utils.FileGetString("gamewave.xml", ref content))
			{
				return false;
			}
		}
		else if (MyUtils.isIOS || MyUtils.isAndroid)
		{
			TextAsset textAsset = (TextAsset)Resources.Load(PrefabManager.GetPath(3006), typeof(TextAsset));
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
			if (childNode.Name != "gamewave" || !MyUtils.GetAttribute(childNode, "id", ref value))
			{
				continue;
			}
			WaveInfo waveInfo = new WaveInfo();
			waveInfo.nID = int.Parse(value);
			if (MyUtils.GetAttribute(childNode, "trigger", ref value))
			{
				waveInfo.nEventType = int.Parse(value);
			}
			if (MyUtils.GetAttribute(childNode, "triggervalue", ref value))
			{
				waveInfo.ltEventParam = new List<int>();
				string[] array = value.Split(',');
				for (int i = 0; i < array.Length; i++)
				{
					waveInfo.ltEventParam.Add(int.Parse(array[i]));
				}
			}
			if (MyUtils.GetAttribute(childNode, "triggerloop", ref value))
			{
				waveInfo.bEventLoop = bool.Parse(value);
			}
			if (MyUtils.GetAttribute(childNode, "delay", ref value))
			{
				waveInfo.m_fDelayTime = ServerX.ParseFloat(value);
			}
			if (MyUtils.GetAttribute(childNode, "interval", ref value))
			{
				waveInfo.m_fInterval = ServerX.ParseFloat(value);
			}
			if (MyUtils.GetAttribute(childNode, "number", ref value))
			{
				waveInfo.m_nNumAtOnce = int.Parse(value);
			}
			if (MyUtils.GetAttribute(childNode, "loop", ref value))
			{
				waveInfo.m_nLoop = int.Parse(value);
			}
			if (MyUtils.GetAttribute(childNode, "forcewave", ref value))
			{
				waveInfo.bForceWave = bool.Parse(value);
			}
			if (MyUtils.GetAttribute(childNode, "cutscene", ref value))
			{
				waveInfo.sCutScene = value;
			}
			if (MyUtils.GetAttribute(childNode, "cutscenecontent", ref value))
			{
				waveInfo.sCutSceneContent = value;
			}
			if (MyUtils.GetAttribute(childNode, "cutscene_BGM", ref value))
			{
				waveInfo.sCutSceneBGM = value;
			}
			if (MyUtils.GetAttribute(childNode, "cutscene_ambience", ref value))
			{
				waveInfo.sCutSceneAmbience = value;
			}
			foreach (XmlNode childNode2 in childNode.ChildNodes)
			{
				if (!(childNode2.Name != "mob") && MyUtils.GetAttribute(childNode2, "id", ref value))
				{
					WaveMobInfo waveMobInfo = new WaveMobInfo();
					waveMobInfo.nID = int.Parse(value);
					if (MyUtils.GetAttribute(childNode2, "level", ref value))
					{
						waveMobInfo.nLevel = int.Parse(value);
					}
					if (MyUtils.GetAttribute(childNode2, "spawnmode", ref value))
					{
						waveMobInfo.SpawnMode = int.Parse(value);
					}
					if (MyUtils.GetAttribute(childNode2, "startpoint", ref value))
					{
						waveMobInfo.nStartPoint = int.Parse(value);
					}
					waveInfo.m_ltWaveMobInfo.Add(waveMobInfo);
				}
			}
			m_dictWaveInfo.Add(waveInfo.nID, waveInfo);
		}
		return true;
	}

	public Dictionary<int, WaveInfo> GetData()
	{
		return m_dictWaveInfo;
	}

	public WaveInfo Get(int nID)
	{
		if (!m_dictWaveInfo.ContainsKey(nID))
		{
			return null;
		}
		return m_dictWaveInfo[nID];
	}
}
