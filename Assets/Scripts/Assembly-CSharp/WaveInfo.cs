using System.Collections.Generic;

public class WaveInfo
{
	public int nID;

	public int nEventType;

	public List<int> ltEventParam;

	public bool bEventLoop;

	public float m_fDelayTime;

	public float m_fInterval;

	public int m_nNumAtOnce;

	public int m_nLoop;

	public List<WaveMobInfo> m_ltWaveMobInfo;

	public string sCutScene;

	public string sCutSceneContent;

	public string sCutSceneAmbience;

	public string sCutSceneBGM;

	public WaveInfo()
	{
		m_fDelayTime = 0f;
		m_fInterval = 0.1f;
		m_nNumAtOnce = 1;
		m_nLoop = -1;
		m_ltWaveMobInfo = new List<WaveMobInfo>();
		sCutScene = string.Empty;
		sCutSceneContent = string.Empty;
		sCutSceneBGM = string.Empty;
		sCutSceneAmbience = string.Empty;
	}

	public WaveMobInfo GetWaveMobInfo(int nIndex)
	{
		if (m_ltWaveMobInfo == null || nIndex < 0 || nIndex >= m_ltWaveMobInfo.Count)
		{
			return null;
		}
		return m_ltWaveMobInfo[nIndex];
	}

	public int GetWaveMobCount()
	{
		if (m_ltWaveMobInfo == null)
		{
			return 0;
		}
		return m_ltWaveMobInfo.Count;
	}
}
