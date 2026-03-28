using UnityEngine;

public class CMonsterGenerate
{
	public enum GenerateState
	{
		None = 0,
		Destroy = 1,
		Delay = 2,
		Process = 3,
		Waiting = 4
	}

	public int nNextWave = -1;

	protected iGameSceneBase m_GameScene;

	protected iGameData m_GameData;

	protected GenerateState m_State;

	protected WaveInfo m_curWaveInfo;

	protected int m_nCurIndex;

	protected int m_nSequence;

	protected float m_fTimeCount;

	public int WaveID
	{
		get
		{
			if (m_curWaveInfo == null)
			{
				return -1;
			}
			return m_curWaveInfo.nID;
		}
	}

	public GenerateState State
	{
		get
		{
			return m_State;
		}
	}

	public CMonsterGenerate()
	{
		m_GameScene = iGameApp.GetInstance().m_GameScene;
		m_GameData = iGameApp.GetInstance().m_GameData;
		m_State = GenerateState.None;
	}

	public void Initialize(int nWaveID)
	{
		m_curWaveInfo = m_GameData.GetWaveInfo(nWaveID);
		if (m_curWaveInfo != null)
		{
			m_nCurIndex = 0;
			m_nSequence = 0;
			if (m_curWaveInfo.m_fDelayTime > 0f)
			{
				m_State = GenerateState.Delay;
				m_fTimeCount = 0f;
			}
			else
			{
				m_State = GenerateState.Process;
				m_fTimeCount = m_curWaveInfo.m_fInterval;
			}
		}
	}

	public void Update(float deltaTime)
	{
		if (m_State == GenerateState.None || m_State == GenerateState.Destroy)
		{
			return;
		}
		switch (m_State)
		{
		case GenerateState.Delay:
		{
			m_fTimeCount += deltaTime;
			if (m_fTimeCount < m_curWaveInfo.m_fDelayTime)
			{
				return;
			}
			m_State = GenerateState.Process;
			m_fTimeCount = m_curWaveInfo.m_fInterval;
			CTaskInfo taskInfo = m_GameData.GetTaskInfo(m_GameScene.m_nCurTaskID);
			if (taskInfo != null && taskInfo.nType == 3)
			{
				CPathWalkerManager pathWalkerManager = m_GameScene.GetPathWalkerManager();
				if (pathWalkerManager != null)
				{
					pathWalkerManager.Stop(m_curWaveInfo.nID, 5f);
				}
				iGameUIBase gameUI = m_GameScene.GetGameUI();
				if (gameUI != null)
				{
					gameUI.ShowTip("G O !");
				}
			}
			break;
		}
		case GenerateState.Process:
			m_fTimeCount += deltaTime;
			if (m_fTimeCount < m_curWaveInfo.m_fInterval)
			{
				return;
			}
			m_fTimeCount = 0f;
			break;
		}
		for (int num = m_curWaveInfo.m_nNumAtOnce; num > 0; num--)
		{
			if (!GenerateMob(m_nCurIndex))
			{
				if (m_curWaveInfo.bForceWave)
				{
					m_State = GenerateState.Waiting;
					return;
				}
			}
			else
			{
				m_nCurIndex++;
				m_nSequence++;
			}
		}
		if (m_nCurIndex < m_curWaveInfo.GetWaveMobCount())
		{
			return;
		}
		if (m_curWaveInfo.m_nLoop == -1)
		{
			m_State = GenerateState.Destroy;
		}
		else if (m_curWaveInfo.m_nLoop > 0)
		{
			m_State = GenerateState.Destroy;
			WaveInfo waveInfo = m_GameData.GetWaveInfo(m_curWaveInfo.m_nLoop);
			if (waveInfo != null)
			{
				nNextWave = m_curWaveInfo.m_nLoop;
			}
		}
		else
		{
			m_nCurIndex = 0;
		}
	}
	
	public void OnMobDied()
	{
		if (m_State == GenerateState.Waiting && m_curWaveInfo != null && m_curWaveInfo.bForceWave)
		{
			m_State = GenerateState.Process;
			m_fTimeCount = m_curWaveInfo.m_fInterval;
		}
	}

	public bool GenerateMob(int nIndex)
	{
		WaveMobInfo waveMobInfo = m_curWaveInfo.GetWaveMobInfo(nIndex);
		if (waveMobInfo == null)
		{
			return false;
		}
		Vector3 v3Pos = Vector3.zero;
		Vector3 onUnitSphere = Random.onUnitSphere;
		onUnitSphere.y = 0f;
		CMobInfoLevel mobInfo = m_GameData.GetMobInfo(waveMobInfo.nID, waveMobInfo.nLevel);
		if (mobInfo == null)
		{
			return false;
		}
		CStartPointManager cStartPointManager = null;
		int num = -1;
		CAIManagerInfo aIManagerInfo = m_GameData.GetAIManagerInfo(mobInfo.nAIManagerID);
		if (aIManagerInfo != null)
		{
			CAIInfo aIInfo = m_GameData.GetAIInfo(aIManagerInfo.nAI);
			if (aIInfo != null)
			{
				num = aIInfo.nBehavior;
			}
		}
		int num2 = num;
		cStartPointManager = ((num2 != 2) ? m_GameScene.GetSPManagerGround() : m_GameScene.GetSPManagerSky());
		if (cStartPointManager != null)
		{
			CStartPoint cStartPoint = null;
			cStartPoint = ((waveMobInfo.nStartPoint != 0) ? cStartPointManager.Get(waveMobInfo.nStartPoint) : cStartPointManager.GetRandom());
			if (cStartPoint != null)
			{
				v3Pos = cStartPoint.GetRandom();
			}
		}
		int uID = MyUtils.GetUID();
		CCharMob cCharMob = m_GameScene.AddMobByWave(waveMobInfo.nID, waveMobInfo.nLevel, uID, m_curWaveInfo.nID, m_nSequence, v3Pos, onUnitSphere);
		if (cCharMob == null)
		{
			return false;
		}
		CGameNetSender.GetInstance().MGManagerAddMob(cCharMob.ID, cCharMob.UID, cCharMob.GenerateWaveID, cCharMob.GenerateSequence, cCharMob.Pos, cCharMob.Dir2D);
		return true;
	}
}
