using System.Collections;
using System.Collections.Generic;
using TNetSdk;
using UnityEngine;
using gyAchievementSystem;
using gyEvent;
using gyTaskSystem;

public class iGameSceneBase
{
	public enum kGameStatus
	{
		None = 0,
		CutScene = 1,
		Pause = 2,
		GameBegin = 3,
		Gameing = 4,
		Gameing_IAP = 5,
		GameOver_Process = 6,
		GameOver_ShowTime = 7,
		GameOver_Revive = 8,
		GameOver = 9,
		GameOver_Material = 10,
		GameOver_LeaveScene = 11,
		GameTutorial = 12
	}

	protected class MonsterNumInfo : MonsterNumLimitInfo
	{
		public int curNum;

		public MonsterNumInfo()
		{
			curNum = 0;
		}

		public bool IsMax()
		{
			return curNum >= nMax;
		}
	}

	protected class CPointScreenTip
	{
		public GameObject agent;

		public gyUIScreenTip screentip;
	}

	protected enum kAssistAimState
	{
		None = 0,
		Normal = 1,
		Proccess = 2
	}

	protected kGameStatus m_LastStatus;

	protected kGameStatus m_Status;

	protected float m_StatusTime;

	protected float m_StatusTimeCount;

	protected int m_StatusStep;
	
	protected int m_nCurTriggerEndIndex = 0;
	
	protected List<CStartPoint> m_ltTriggerEndPoints = new List<CStartPoint>();
	
	protected CPointScreenTip m_curTriggerEndTip;

	protected bool m_bMissionSuccess;

	public kGameSceneEnum m_LeaveGameScene;

	protected iGameState m_GameState;

	protected iGameData m_GameData;

	protected iGameLogic m_GameLogic;

	protected iGameUIBase m_GameUI;

	protected bool m_bPause;

	protected GameLevelInfo m_curGameLevelInfo;

	protected bool m_bCG;

	protected List<int> m_ltCGWave;

	protected CPathWalkerManager m_PathWalkerManager;

	protected bool m_bIsSkyScene;

	protected CControlBase m_Input;

	protected CCharUser m_User;

	protected Vector3 m_v3BirthPos;

	protected iCameraTrail m_CameraTrail;

	protected iCameraReveal m_CameraReveal;

	protected iCameraFocus m_CameraFocus;

	protected Dictionary<int, CCharMob> m_MobMap;

	protected Dictionary<int, int> m_dictWaveMobNumber;

	protected Dictionary<int, int> m_dictStealItem;

	protected Dictionary<int, CCharPlayer> m_PlayerMap;

	protected List<GameObject> m_ltItem;

	protected List<GameObject> m_ltSceneGameObject;

	public float m_fNavPlane;

	protected UnityEngine.AI.NavMeshPath m_NavPath;

	protected List<MonsterNumInfo> m_ltMonsterNumInfo;

	protected CStartPointManager m_curBPManager;

	protected Dictionary<int, CStartPointManager> m_dictSPManager;

	protected Dictionary<int, CStartPointManager> m_dictHPManager;

	protected CStartPointManager m_curSPManagerGround;

	protected CStartPointManager m_curSPManagerSky;

	protected CStartPointManager m_curHPManager;

	protected CStartPointManager m_curTriggerManagerBegin;

	protected CStartPointManager m_curTriggerManagerEnd;

	protected List<CPointScreenTip> m_ltScreenTipTriggerEnd;

	public CMonsterGenerateManager m_MGManager;

	public CEventManager m_EventManager;

	public CTaskManager m_TaskManager;

	public int m_nCurTaskID;

	protected kAssistAimState m_AssistAimState;

	protected CCharBase m_AssistTarget;

	protected Transform m_AssistBone;

	protected float m_fAssistClosest;

	protected Vector2 m_v2AssistAimLastPos;

	protected float m_fAssistAimCount;

	protected float m_fAssistAimRate;

	protected iBuilding m_Building;

	public GameLevelInfo CurGameLevelInfo
	{
		get
		{
			return m_curGameLevelInfo;
		}
	}

	public iBuilding CurBuilding
	{
		get
		{
			return m_Building;
		}
	}

	public kGameStatus GameStatus
	{
		get
		{
			return m_Status;
		}
	}

	public int GameStatusStep
	{
		get
		{
			return m_StatusStep;
		}
		set
		{
			m_StatusStep = value;
		}
	}

	public float GameStatusStepTime
	{
		get
		{
			return m_StatusTime;
		}
		set
		{
			m_StatusTime = value;
			m_StatusTimeCount = 0f;
		}
	}

	public bool isMissionSuccess
	{
		get
		{
			return m_bMissionSuccess;
		}
	}

	public bool isPause
	{
		get
		{
			return m_bPause;
		}
	}

	public virtual void Initialize()
	{
		m_GameState = iGameApp.GetInstance().m_GameState;
		m_GameData = iGameApp.GetInstance().m_GameData;
		if (m_GameUI == null)
		{
			GameObject gameObject = GameObject.Find("Game");
			m_GameUI = gameObject.GetComponent<iGameUIBase>();
			m_GameUI.Initialize();
		}
		if (m_GameLogic == null)
		{
			m_GameLogic = new iGameLogic();
			m_GameLogic.Initialize();
		}
		if (m_CameraTrail == null)
		{
			m_CameraTrail = Camera.main.GetComponent<iCameraTrail>();
			if (m_CameraTrail == null)
			{
				m_CameraTrail = Camera.main.gameObject.AddComponent<iCameraTrail>();
			}
		}
		if (m_CameraReveal == null)
		{
			m_CameraReveal = Camera.main.GetComponent<iCameraReveal>();
			if (m_CameraReveal == null)
			{
				m_CameraReveal = Camera.main.gameObject.AddComponent<iCameraReveal>();
			}
		}
		if (m_CameraFocus == null)
		{
			m_CameraFocus = Camera.main.GetComponent<iCameraFocus>();
			if (m_CameraFocus == null)
			{
				m_CameraFocus = Camera.main.gameObject.AddComponent<iCameraFocus>();
			}
		}
		if (m_MobMap == null)
		{
			m_MobMap = new Dictionary<int, CCharMob>();
		}
		if (m_PathWalkerManager == null)
		{
			m_PathWalkerManager = new CPathWalkerManager();
		}
		if (m_dictWaveMobNumber == null)
		{
			m_dictWaveMobNumber = new Dictionary<int, int>();
		}
		if (m_dictStealItem == null)
		{
			m_dictStealItem = new Dictionary<int, int>();
		}
		if (m_PlayerMap == null)
		{
			m_PlayerMap = new Dictionary<int, CCharPlayer>();
		}
		if (m_ltMonsterNumInfo == null)
		{
			m_ltMonsterNumInfo = new List<MonsterNumInfo>();
		}
		if (m_dictSPManager == null)
		{
			m_dictSPManager = new Dictionary<int, CStartPointManager>();
		}
		if (m_dictHPManager == null)
		{
			m_dictHPManager = new Dictionary<int, CStartPointManager>();
		}
		if (m_EventManager == null)
		{
			m_EventManager = new CEventManager();
		}
		if (m_MGManager == null)
		{
			m_MGManager = new CMonsterGenerateManager();
		}
		if (m_TaskManager == null)
		{
			m_TaskManager = new CTaskManager();
			m_TaskManager.Initialize();
		}
		if (m_ltScreenTipTriggerEnd == null)
		{
			m_ltScreenTipTriggerEnd = new List<CPointScreenTip>();
		}
		if (m_ltItem == null)
		{
			m_ltItem = new List<GameObject>();
		}
		if (m_ltSceneGameObject == null)
		{
			m_ltSceneGameObject = new List<GameObject>();
		}
		if (m_Input == null)
		{
#if UNITY_EDITOR
            m_Input = new CControlWindows();

#elif UNITY_STANDALONE_WIN
			m_Input = new CControlWindows();

#elif UNITY_IOS || UNITY_ANDROID
			m_Input = new CControlIphone();
#endif
        }
        if (TNetManager.GetInstance().Connection != null)
		{
			CGameNetAccepter.GetInstance().Initialize();
		}
		if (m_ltCGWave == null)
		{
			m_ltCGWave = new List<int>();
		}
		if (m_NavPath == null)
		{
			m_NavPath = new UnityEngine.AI.NavMeshPath();
		}
	}

	public void InitializeGameLevel(int nLevel)
	{
		m_curGameLevelInfo = m_GameData.GetGameLevelInfo(nLevel);
		if (m_curGameLevelInfo == null)
		{
			return;
		}
		m_bIsSkyScene = m_curGameLevelInfo.bIsSkyScene;
		m_fNavPlane = m_curGameLevelInfo.fNavPlane;
		m_curBPManager = new CStartPointManager();
		m_curBPManager.Load("_Config/_StartPoint/StartPoint_" + m_curGameLevelInfo.nBirthPos);
		CStartPoint random = m_curBPManager.GetRandom();
		if (random != null)
		{
			m_v3BirthPos = random.GetRandom2D();
		}
		int nDefaultSPSky = m_curGameLevelInfo.nDefaultSPSky;
		CStartPointManager cStartPointManager = new CStartPointManager();
		if (cStartPointManager.Load("_Config/_StartPoint/StartPoint_" + nDefaultSPSky))
		{
			m_curSPManagerSky = cStartPointManager;
			if (!m_dictSPManager.ContainsKey(nDefaultSPSky))
			{
				m_dictSPManager.Add(nDefaultSPSky, cStartPointManager);
			}
		}
		nDefaultSPSky = m_curGameLevelInfo.nDefaultSPGround;
		cStartPointManager = new CStartPointManager();
		if (cStartPointManager.Load("_Config/_StartPoint/StartPoint_" + nDefaultSPSky))
		{
			m_curSPManagerGround = cStartPointManager;
			if (!m_dictSPManager.ContainsKey(nDefaultSPSky))
			{
				m_dictSPManager.Add(nDefaultSPSky, cStartPointManager);
			}
		}
		int nDefaultHoverPoint = m_curGameLevelInfo.nDefaultHoverPoint;
		CStartPointManager cStartPointManager2 = new CStartPointManager();
		cStartPointManager2.Load("_Config/_HoverPoint/HoverPoint_" + nDefaultHoverPoint);
		m_curHPManager = cStartPointManager2;
		if (!m_dictHPManager.ContainsKey(nDefaultHoverPoint))
		{
			m_dictHPManager.Add(nDefaultHoverPoint, cStartPointManager2);
		}
		if (m_curGameLevelInfo.ltGameWave.Count > 0)
		{
			m_MGManager.RegisterWaveID(m_curGameLevelInfo.ltGameWave.ToArray());
		}
		if (m_curGameLevelInfo.nTPBeginCfg > 0)
		{
			m_curTriggerManagerBegin = new CStartPointManager();
			m_curTriggerManagerBegin.Load("_Config/_TriggerPoint/TriggerPoint_" + m_curGameLevelInfo.nTPBeginCfg);
		}
		if (m_curGameLevelInfo.nTPEndCfg > 0)
		{
			m_curTriggerManagerEnd = new CStartPointManager();
			m_curTriggerManagerEnd.Load("_Config/_TriggerPoint/TriggerPoint_" + m_curGameLevelInfo.nTPEndCfg);
		}
		if (m_curGameLevelInfo.ltMonsterNumLimit.Count > 0)
		{
			foreach (MonsterNumLimitInfo item in m_curGameLevelInfo.ltMonsterNumLimit)
			{
				MonsterNumInfo monsterNumInfo = new MonsterNumInfo();
				monsterNumInfo.nLimitType = item.nLimitType;
				monsterNumInfo.nLimitValue = item.nLimitValue;
				monsterNumInfo.nMax = item.nMax;
				Debug.Log(monsterNumInfo.nLimitType + " " + monsterNumInfo.nMax);
				m_ltMonsterNumInfo.Add(monsterNumInfo);
			}
		}
		m_TaskManager.AddTask(m_curGameLevelInfo.nTaskID);
		m_nCurTaskID = m_curGameLevelInfo.nTaskID;
		m_GameUI.InitTaskUI();
		if (m_curGameLevelInfo.sCutScene.Length > 0)
		{
			CCameraRoam.GetInstance().PreLoadCG(m_curGameLevelInfo.sCutScene);
		}
		if (m_curGameLevelInfo.sCutSceneContent.Length > 0)
		{
			CCameraRoam.GetInstance().PreLoadCG(m_curGameLevelInfo.sCutSceneContent);
		}
		if (m_curGameLevelInfo.ltGameWave.Count <= 0)
		{
			return;
		}
		WaveInfo waveInfo = null;
		foreach (int item2 in m_curGameLevelInfo.ltGameWave)
		{
			waveInfo = m_GameData.GetWaveInfo(item2);
			if (waveInfo != null)
			{
				if (waveInfo.sCutScene.Length > 0)
				{
					CCameraRoam.GetInstance().PreLoadCG(waveInfo.sCutScene);
				}
				if (waveInfo.sCutSceneContent.Length > 0)
				{
					CCameraRoam.GetInstance().PreLoadCG(waveInfo.sCutSceneContent);
				}
			}
		}
	}

	public virtual void Reset()
	{
		ClearNPC();
		ClearPlayer();
		if (m_TaskManager != null)
		{
			m_TaskManager.Reset();
		}
		if (m_MGManager != null)
		{
			m_MGManager.Reset();
		}
		if (m_GameUI != null)
		{
			m_GameUI.Reset();
		}
		foreach (GameObject item in m_ltItem)
		{
			if (item != null)
			{
				Object.Destroy(item);
			}
		}
		m_ltItem.Clear();
		foreach (GameObject item2 in m_ltSceneGameObject)
		{
			if (item2 != null)
			{
				Object.Destroy(item2);
			}
		}
		m_ltSceneGameObject.Clear();
	}

	public virtual void Destroy()
	{
		if (m_User != null)
		{
			m_User.Destroy();
			m_User = null;
		}
		ClearNPC();
		ClearPlayer();
		if (m_GameUI != null)
		{
			m_GameUI.Destroy();
		}
		if (m_ltScreenTipTriggerEnd != null)
		{
			foreach (CPointScreenTip item in m_ltScreenTipTriggerEnd)
			{
				if (item.agent != null)
				{
					Object.Destroy(item.agent);
				}
				if (item.screentip != null)
				{
					item.screentip.gameObject.SetActiveRecursively(false);
				}
			}
			m_ltScreenTipTriggerEnd.Clear();
		}
		CSoundScene.GetInstance().Destroy();
		if (TNetManager.GetInstance().Connection != null)
		{
			CGameNetAccepter.GetInstance().Destroy();
		}
	}

	public virtual void ClearNPC()
	{
		if (m_MobMap == null)
		{
			return;
		}
		foreach (CCharMob value in m_MobMap.Values)
		{
			value.Destroy();
		}
		m_MobMap.Clear();
	}

	public virtual void ClearPlayer()
	{
		if (m_PlayerMap == null)
		{
			return;
		}
		foreach (CCharPlayer value in m_PlayerMap.Values)
		{
			if (!(value == m_User))
			{
				value.Destroy();
			}
		}
		m_PlayerMap.Clear();
	}

	protected void StartCG()
	{
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        m_GameUI.MovieUIIn(1f);
		m_GameUI.HideGameUI();
		m_GameUI.ShowSkipUI(true);
	}

	protected void FinishCG()
	{
		m_GameUI.MovieUIOut(1f);
		m_GameUI.ShowGameUI();
		m_GameUI.ShowSkipUI(false);
		StartGame();
	}

	public virtual void StartGame()
	{
		if (m_curGameLevelInfo == null)
		{
			return;
		}
		iDataCenter dataCenter = m_GameData.GetDataCenter();
		if (dataCenter == null)
		{
			return;
		}
		if (!dataCenter.IsLevelIgnoreCG(m_curGameLevelInfo.nID))
		{
			dataCenter.SetLevelIgnoreCG(m_curGameLevelInfo.nID, true);
			if (!m_bCG)
			{
				m_bCG = true;
				CCameraRoam.CCGInfo cCGInfo = new CCameraRoam.CCGInfo();
				cCGInfo.sCG = m_curGameLevelInfo.sCutScene;
				cCGInfo.sCGContent = m_curGameLevelInfo.sCutSceneContent;
				cCGInfo.sCGAmbience = m_curGameLevelInfo.sCutSceneAmbience;
				cCGInfo.sCGBGM = string.Empty;
				if (CCameraRoam.GetInstance().Start(Camera.main, cCGInfo, StartCG, FinishCG))
				{
					m_Status = kGameStatus.CutScene;
					return;
				}
			}
		}
		CCharSaveInfo character = dataCenter.GetCharacter(dataCenter.CurCharID);
		if (character == null || character.nLevel < 0)
		{
			return;
		}
		CCharacterInfoLevel characterInfo = m_GameData.GetCharacterInfo(character.nID, character.nLevel);
		if (characterInfo == null)
		{
			return;
		}
		if (m_User == null)
		{
			m_User = AddUser(characterInfo.nModel, m_v3BirthPos, Vector3.forward);
			if (m_User == null)
			{
				return;
			}
		}
		m_User.isDead = false;
		if (TNetManager.GetInstance().Connection != null)
		{
			iGameState.CPlayerInitInfo netPlayerInitInfo = m_GameState.GetNetPlayerInitInfo(TNetManager.GetInstance().Connection.Myself.Id);
			if (netPlayerInitInfo != null)
			{
				m_v3BirthPos = netPlayerInitInfo.v3Pos;
			}
		}
		m_User.Pos = m_v3BirthPos;
		m_User.Dir2D = Vector3.zero - m_User.Pos;
		m_User.SetRotateLimit(-10f, 60f, 0f, 0f);
		m_User.SetMoveMode(m_bIsSkyScene ? kCharMoveMode.Fly : kCharMoveMode.Ground);
		m_User.SetBehavior(0);
		Debug.Log("user level = " + character.nLevel);
		for (int i = 0; i < 3; i++)
		{
			int nID = 0;
			int nLevel = 0;
			if (m_GameState.GetCarryPassiveSkill(i, ref nID, ref nLevel))
			{
				m_User.CarryPassiveSkill(i, nID, nLevel);
			}
		}
		m_User.InitChar(character.nID, character.nLevel, character.nExp);
		if (m_User.Property != null)
		{
			if (m_bIsSkyScene)
			{
				m_User.Property.SetValueBase(kProEnum.MoveSpeed, 10f);
				m_User.Property.SetValueBase(kProEnum.MoveSpeedAcc, 10f);
			}
			else
			{
				m_User.Property.SetValueBase(kProEnum.MoveSpeed, 6f);
				m_User.Property.SetValueBase(kProEnum.MoveSpeedAcc, 6f);
			}
		}
		bool flag = false;
		if (CurGameLevelInfo.m_bLimitMelee)
		{
			for (int j = 0; j < 3; j++)
			{
				CWeaponBase weapon = m_GameState.GetWeapon(j);
				if (weapon != null &&
				    weapon.CurWeaponLvlInfo != null &&
				    weapon.CurWeaponLvlInfo.nType == 1)
				{
					m_User.SwitchWeapon(j);
					flag = true;
					break;
				}
			}
		}
		else
		{
			for (int j = 0; j < 3; j++)
			{
				CWeaponBase weapon = m_GameState.GetWeapon(j);
				if (weapon != null &&
				    weapon.CurWeaponLvlInfo != null &&
				    weapon.CurWeaponLvlInfo.nType != 1)
				{
					m_User.SwitchWeapon(j);
					flag = true;
					break;
				}
			}
		}

		if (!flag)
		{
			CWeaponBase weapon = m_GameState.GetWeapon(0);
			if (weapon != null && weapon.CurWeaponLvlInfo != null)
				m_User.SwitchWeapon(0);
		}
		if (m_GameUI != null)
		{
			m_GameUI.SetProtraitLevel(character.nLevel);
			m_GameUI.SetProtraitExp(0, (float)character.nExp / (float)characterInfo.nExp);
		}
		if (TNetManager.GetInstance().Connection != null)
		{
			TNetObject connection = TNetManager.GetInstance().Connection;
			for (int k = 0; k < connection.CurRoom.UserList.Count; k++)
			{
				TNetUser tNetUser = connection.CurRoom.UserList[k];
				if (tNetUser.IsItMe)
				{
					continue;
				}
				Vector3 v3Pos = Vector3.zero;
				int num = 1;
				int nLevel2 = 1;
				iGameState.CPlayerInitInfo netPlayerInitInfo2 = m_GameState.GetNetPlayerInitInfo(tNetUser.Id);
				if (netPlayerInitInfo2 != null)
				{
					v3Pos = netPlayerInitInfo2.v3Pos;
					num = netPlayerInitInfo2.nCharID;
					nLevel2 = netPlayerInitInfo2.nCharLevel;
				}
				CCharPlayer cCharPlayer = AddPlayer(num, tNetUser.Id, v3Pos, Vector3.forward);
				if (!(cCharPlayer == null))
				{
					cCharPlayer.EquipWeapon(1, 1);
					cCharPlayer.SetMoveMode(m_bIsSkyScene ? kCharMoveMode.Fly : kCharMoveMode.Ground);
					cCharPlayer.SetBehavior(100);
					cCharPlayer.InitChar(num, nLevel2, 1);
					if (m_GameUI != null)
					{
						m_GameUI.SetProtraitLevel(nLevel2, tNetUser.Id);
					}
				}
			}
		}
		CSoundScene.GetInstance().PlayBGM(m_curGameLevelInfo.sBGM);
		CSoundScene.GetInstance().PlayAmbienceBGM(m_curGameLevelInfo.sBGMAmbience);
		m_CameraTrail.Initialize(m_User, !flag);
		m_CameraTrail.SetRotateLimit(-10f, 60f, 0f, 0f);
		m_CameraTrail.Active = true;
		m_GameState.nLastLevel = character.nLevel;
		m_Input.Initialize();
		if (IsRoomMaster())
		{
			InitTask(m_nCurTaskID);
			m_TaskManager.Start();
			m_MGManager.Start();
		}
		m_Status = kGameStatus.GameBegin;
		m_StatusTime = 1f;
		m_StatusTimeCount = 0f;
		m_bMissionSuccess = false;
		PrefabLoadResource();
		iGameApp.GetInstance().Flurry_EnterStage(m_curGameLevelInfo.nID);
		m_GameUI.HideGameUI();
		m_GameUI.ShowGameUI();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (!dataCenter.isTutorial)
		{
			StartTutorial();
			return;
		}
		m_GameUI.FadeIn(1f);
		SetPause(false);
    }

	public void StartTutorial()
	{
		m_LastStatus = m_Status;
		m_Status = kGameStatus.GameTutorial;
		m_GameUI.FadeIn(0f);
		m_GameUI.ShowTutorials(true);
		SetPause(true);
	}

	public void FinishTutorial()
	{
		if (m_LastStatus != 0)
		{
			m_Status = m_LastStatus;
		}
		m_GameUI.FadeIn(1f);
		m_GameUI.ShowTutorials(false);
		SetPause(false);
		iDataCenter dataCenter = m_GameData.GetDataCenter();
		if (dataCenter != null)
		{
			dataCenter.isTutorial = true;
			dataCenter.Save();
		}
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

	public void PrefabLoadResource()
	{
		List<WaveInfo> waveList = m_MGManager.GetWaveList();
		if (waveList != null)
		{
			int num = 0;
			WaveMobInfo waveMobInfo = null;
			CMobInfoLevel cMobInfoLevel = null;
			foreach (WaveInfo item in waveList)
			{
				num = item.GetWaveMobCount();
				for (int i = 0; i < num; i++)
				{
					waveMobInfo = item.GetWaveMobInfo(i);
					if (waveMobInfo != null)
					{
						cMobInfoLevel = m_GameData.GetMobInfo(waveMobInfo.nID, waveMobInfo.nLevel);
						if (cMobInfoLevel != null)
						{
							PrefabManager.Get(cMobInfoLevel.nModel);
						}
					}
				}
			}
		}
		CWeaponBase cWeaponBase = null;
		for (int j = 0; j < 3; j++)
		{
			cWeaponBase = m_GameState.GetWeapon(j);
			if (cWeaponBase != null && cWeaponBase.CurWeaponLvlInfo != null)
			{
				if (cWeaponBase.CurWeaponLvlInfo.nFire != -1)
				{
					PrefabManager.AddPool(cWeaponBase.CurWeaponLvlInfo.nFire, 5);
				}
				if (cWeaponBase.CurWeaponLvlInfo.nHit != -1)
				{
					PrefabManager.AddPool(cWeaponBase.CurWeaponLvlInfo.nHit, 5);
				}
				if (cWeaponBase.CurWeaponLvlInfo.nBullet != -1)
				{
					PrefabManager.AddPool(cWeaponBase.CurWeaponLvlInfo.nBullet, 5);
				}
			}
		}
		if (!(m_User != null))
		{
			return;
		}
		CSkillInfoLevel skillInfo = m_GameData.GetSkillInfo(m_User.SkillID, 1);
		if (skillInfo == null)
		{
			return;
		}
		CBuffInfo cBuffInfo = null;
		for (int k = 0; k < 3; k++)
		{
			if (skillInfo.arrFunc[k] == 3)
			{
				cBuffInfo = m_GameData.GetBuffInfo(skillInfo.arrValueX[k]);
				if (cBuffInfo != null)
				{
					PrefabManager.Get(cBuffInfo.arrEffAdd[0]);
					PrefabManager.Get(cBuffInfo.arrEffDel[0]);
					PrefabManager.Get(cBuffInfo.arrEffHold[0]);
				}
			}
		}
		PrefabManager.AddPool(1115, 1);
		PrefabManager.AddPool(1116, 1);
	}

	public virtual void FinishGame()
	{
		m_Status = kGameStatus.GameOver_Process;
		m_StatusTime = 2f;
		m_StatusTimeCount = 0f;
		m_CameraTrail.Active = false;
		m_CameraReveal.Active = false;
		m_CameraFocus.Active = false;
		CSoundScene.GetInstance().StopBGM();
		if (m_TaskManager.isAllCompleted)
		{
			CTaskBase task = m_TaskManager.GetTask();
			if (task != null)
			{
				CTaskInfo taskInfo = task.GetTaskInfo();
				if (taskInfo != null && taskInfo.nType == 2 && m_GameState.LastKillBoss != -1)
				{
					CCharMob mob = GetMob(m_GameState.LastKillBoss);
					Debug.Log(m_GameState.LastKillBoss + " " + mob);
					if (mob != null)
					{
						m_CameraFocus.Go(mob.GetBone(1).gameObject, 8f, 1.5f);
						m_StatusTime = mob.GetActionLen(kAnimEnum.Mob_Dead);
						Time.timeScale = 0.4f;
					}
					CUISound.GetInstance().Play("UI_Game_status");
					CSoundScene.GetInstance().PlayBGM("Amb_Cheer_Crowd");
					m_GameState.LastKillBoss = -1;
				}
			}
		}
		else
		{
			m_CameraReveal.Go(m_User.GetBone(1).gameObject, 70f, 5f, 5f);
			Time.timeScale = 0.4f;
		}
		m_User.SetFire(false);
		m_User.MoveStop();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

	public virtual void GameOver(bool bSuccess)
	{
		if (m_User.isDead)
		{
			m_Status = kGameStatus.GameOver_Revive;
			m_StatusStep = 0;
			m_StatusTime = 10f;
			m_StatusTimeCount = 0f;
			CSoundScene.GetInstance().StopBGM();
			CSoundScene.GetInstance().StopAmbienceBGM();
			m_GameUI.ShowRevive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
		else
		{
			m_Status = kGameStatus.GameOver;
			m_StatusStep = 0;
			m_StatusTime = 5f;
			m_StatusTimeCount = 0f;
		}
		m_bMissionSuccess = bSuccess;
		m_CameraTrail.Active = false;
		m_CameraFocus.Active = false;
		m_CameraReveal.Active = false;
		foreach (CCharMob value2 in m_MobMap.Values)
		{
			value2.SetActive(false);
		}
		m_GameUI.HideGameUI();
		if (bSuccess)
		{
			m_CameraReveal.Go(m_User.GetBone(1).gameObject, -20f, 2f, 10f);
			iDataCenter dataCenter = m_GameData.GetDataCenter();
			iItemDynamic[] allDrops = Object.FindObjectsOfType(typeof(iItemDynamic)) as iItemDynamic[];
			if (allDrops != null)
			{
				foreach (iItemDynamic drop in allDrops)
				{
					if (drop != null)
					{
						drop.ForceAbsorb(50f);
					}
				}
			}
			if (dataCenter != null)
			{
				m_GameState.m_nLevelRewardGold = CurGameLevelInfo.nRewardGold;
				m_GameState.m_nLevelRewardExp = CurGameLevelInfo.nRewardExp;
				float value = m_User.Property.GetValue(kProEnum.Char_IncreaseGold);
				if (value > 0f)
				{
					m_GameState.m_nLevelRewardGold = (int)((float)m_GameState.m_nLevelRewardGold * (1f + value / 100f));
				}
				dataCenter.AddGold(m_GameState.m_nLevelRewardGold + m_GameState.GainGoldInGame);
				value = m_User.Property.GetValue(kProEnum.Char_IncreaseExp);
				if (value > 0f)
				{
					m_GameState.m_nLevelRewardExp = (int)((float)m_GameState.m_nLevelRewardExp * (1f + value / 100f));
				}
				m_User.AddExp(m_GameState.m_nLevelRewardExp);
				if (CurGameLevelInfo.nID == dataCenter.LatestLevel)
				{
					dataCenter.UnlockNewLevelPrepare();
				}
				if (!dataCenter.IsLevelPassed(CurGameLevelInfo.nID))
				{
					dataCenter.SetPassedLevel(CurGameLevelInfo.nID);
					dataCenter.AddSceneProccess(CurGameLevelInfo.m_nProccess);
					CAchievementManager.GetInstance().AddAchievement(2, new object[1] { dataCenter.SceneProccess });
				}
				dataCenter.Save();
				m_GameState.isCheckUnLock = true;
				if (CurGameLevelInfo != null)
				{
					if (CurGameLevelInfo.bIsSideLevel)
					{
						CAchievementManager.GetInstance().AddAchievement(12);
					}
					else
					{
						CAchievementManager.GetInstance().AddAchievement(11);
					}
				}
				if (CurGameLevelInfo != null && CurGameLevelInfo.ltRewardMaterial != null)
				{
					int num = 0;
					foreach (CRewardMaterial item in CurGameLevelInfo.ltRewardMaterial)
					{
						num = item.GetDropCount();
						if (num > 0)
						{
							m_GameState.AddMaterial(item.nID, num);
						}
					}
				}
			}
			m_User.CrossAnim(kAnimEnum.VictoryIdle, WrapMode.Loop, 0.3f, 1f, 0f);
			m_User.SetActionLayer(kAnimEnum.VictoryIdle, -1);
			m_User.CrossAnim(kAnimEnum.Victory, WrapMode.Once, 0.3f, 1f, 0f);
			CSoundScene.GetInstance().PlayBGM("BGM_Victory");
			CUISound.GetInstance().Play("UI_Game_status");
			//iGameApp.GetInstance().Flurry_WinStage(m_curGameLevelInfo.nID);
		}
		else
		{
			if (!m_User.isDead)
			{
				m_CameraReveal.Go(m_User.GetBone(1).gameObject, -20f, 2f, 10f);
				m_User.CrossAnim(kAnimEnum.FailIdle, WrapMode.Loop, 0.3f, 1f, 0f);
				m_User.SetActionLayer(kAnimEnum.FailIdle, -1);
				m_User.CrossAnim(kAnimEnum.Fail, WrapMode.Once, 0.3f, 1f, 0f);
			}
			else
			{
				m_CameraReveal.Go(m_User.GetBone(1).gameObject, 70f, 5f, 10f);
			}
			CSoundScene.GetInstance().PlayBGM("BGM_Fail");
			//iGameApp.GetInstance().Flurry_LoseStage(m_curGameLevelInfo.nID);
		}
		m_User.SetFire(false);
		m_User.MoveStop();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

	public virtual void GameOver_TakeMaterial()
	{
		m_Status = kGameStatus.GameOver_Material;
		m_GameUI.ShowMaterial(true);
		//OpenClikPlugin.Hide();
	}

	public virtual void ReviveGame()
	{
		m_Status = kGameStatus.Gameing;
		m_bMissionSuccess = false;
		Time.timeScale = 1f;
		m_GameUI.ShowRevive(false);
		m_GameUI.ShowGameUI();
		m_User.Revive(m_User.Property.GetValue(kProEnum.HPMax));
		m_User.AddBuff(10, 4f);
		m_User.ResetSkillCD();
		if (m_bIsSkyScene && CurGameLevelInfo != null)
		{
			Vector3 pos = m_User.Pos;
			pos.y = CurGameLevelInfo.fNavPlane;
			m_User.Pos = pos;
		}
		m_TaskManager.ResetState();
		m_CameraTrail.Active = true;
		m_CameraFocus.Active = false;
		m_CameraReveal.Active = false;
		foreach (CCharMob value in m_MobMap.Values)
		{
			value.SetActive(true);
		}
		CSoundScene.GetInstance().PlayBGM(m_curGameLevelInfo.sBGM);
		CSoundScene.GetInstance().PlayAmbienceBGM(m_curGameLevelInfo.sBGMAmbience);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

	public virtual void LeaveGame(float fDelayTime = 0f, kGameSceneEnum leavegamescene = kGameSceneEnum.None)
	{
		m_Status = kGameStatus.GameOver_LeaveScene;
		m_StatusTime = fDelayTime;
		m_StatusTimeCount = 0f;
		m_LeaveGameScene = leavegamescene;
		CCameraRoam.GetInstance().UnLoad();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        //OpenClikPlugin.Hide();
    }

	public virtual void Update(float deltaTime)
	{
		if (m_Input != null)
		{
			m_Input.Update(deltaTime);
		}
		switch (m_Status)
		{
		case kGameStatus.GameBegin:
			UpdateStatus_GameBegin(deltaTime);
			break;
		case kGameStatus.GameOver:
			UpdateStatus_GameOver(deltaTime);
			break;
		case kGameStatus.Gameing:
			UpdateStatus_Gaming(deltaTime);
			break;
		case kGameStatus.CutScene:
			UpdateStatus_CutScene(deltaTime);
			break;
		case kGameStatus.Pause:
			break;
		case kGameStatus.GameOver_Process:
			UpdateStatus_GameOver_Process(deltaTime);
			break;
		case kGameStatus.GameOver_ShowTime:
			UpdateStatus_GameOver_ShowTime(deltaTime);
			break;
		case kGameStatus.GameOver_Revive:
			UpdateStatus_GameOver_Revive(deltaTime);
			break;
		case kGameStatus.GameOver_LeaveScene:
			UpdateStatus_GameLeave(deltaTime);
			break;
		case kGameStatus.Gameing_IAP:
		case kGameStatus.GameOver_Material:
			break;
		}
	}

	public virtual void FixedUpdate(float deltaTime)
	{
	}

	public virtual void LateUpdate(float deltaTime)
	{
		if (m_Input != null)
		{
			m_Input.LateUpdate(deltaTime);
		}
	}

	protected virtual void UpdateStatus_Movie(float deltaTime)
	{
	}

	protected virtual void UpdateStatus_GameBegin(float deltaTime)
	{
		m_StatusTimeCount += deltaTime;
		if (!(m_StatusTimeCount < m_StatusTime))
		{
			m_StatusTimeCount = 0f;
			m_Status = kGameStatus.Gameing;
		}
	}

	protected virtual void UpdateStatus_GameOver_Process(float deltaTime)
	{
		m_StatusTimeCount += deltaTime;
		if (m_StatusTimeCount < m_StatusTime)
		{
			return;
		}
		Time.timeScale = 1f;
		CTaskBase task = m_TaskManager.GetTask();
		if (task != null)
		{
			CTaskInfo taskInfo = task.GetTaskInfo();
			if (taskInfo != null && taskInfo.nType == 2 && m_TaskManager.isAllCompleted)
			{
				m_GameUI.ShowTip("Returning to the village in 10 seconds");
				m_Status = kGameStatus.GameOver_ShowTime;
				m_StatusTime = 10f;
				m_StatusTimeCount = 0f;
				m_CameraTrail.Active = true;
				m_CameraFocus.Active = false;
				m_CameraReveal.Active = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                return;
			}
		}
		GameOver(m_TaskManager.isAllCompleted);
	}

	protected virtual void UpdateStatus_GameOver_ShowTime(float deltaTime)
	{
		m_StatusTimeCount += deltaTime;
		if (m_StatusTimeCount >= m_StatusTime)
		{
			GameOver(true);
			return;
		}
		m_GameState.AddGameTime(deltaTime);
		UpdateAssistAim(deltaTime);
		List<CCharMob> list = new List<CCharMob>();
		foreach (CCharMob value in m_MobMap.Values)
		{
			if (value.isNeedDestroy)
			{
				list.Add(value);
			}
		}
		foreach (CCharMob item in list)
		{
			RemoveMob(item);
		}
	}

	protected virtual void UpdateStatus_GameOver_Revive(float deltaTime)
	{
		m_StatusTimeCount += deltaTime;
		if (m_StatusTimeCount >= m_StatusTime)
		{
			m_Status = kGameStatus.GameOver;
			m_StatusStep = 0;
			m_StatusTime = 1f;
			m_StatusTimeCount = 0f;
			m_GameUI.ShowRevive(false);
		}
	}

	protected virtual void UpdateStatus_GameOver(float deltaTime)
	{
		if (m_bMissionSuccess)
		{
			UpdateStatus_GameOver_Win(deltaTime);
		}
		else
		{
			UpdateStatus_GameOver_Lose(deltaTime);
		}
	}

	protected virtual void UpdateStatus_GameOver_Win(float deltaTime)
	{
		switch (m_StatusStep)
		{
		case 0:
			m_StatusTimeCount += deltaTime;
			if (m_StatusTimeCount >= m_StatusTime)
			{
				CSoundScene.GetInstance().StopAmbienceBGM();
				m_GameUI.ShowSuccess(true);
				m_StatusStep = 1;
				m_StatusTime = 5f;
				m_StatusTimeCount = 0f;
				/*if (OpenClikPlugin.IsAdReady())
				{
					OpenClikPlugin.Show(false);
				}*/
			}
			break;
		case 1:
			m_StatusTimeCount += deltaTime;
			if (m_StatusTimeCount >= m_StatusTime)
			{
				m_StatusStep = 2;
			}
			break;
		case 2:
			break;
		case 3:
			m_StatusTimeCount += deltaTime;
			if (m_StatusTimeCount >= m_StatusTime)
			{
				m_StatusStep = 4;
				m_StatusTime = 5f;
				m_StatusTimeCount = 0f;
			}
			break;
		case 4:
			break;
		}
	}

	protected virtual void UpdateStatus_GameOver_Lose(float deltaTime)
	{
		switch (m_StatusStep)
		{
		case 0:
			m_StatusTimeCount += deltaTime;
			if (m_StatusTimeCount >= m_StatusTime)
			{
				CSoundScene.GetInstance().StopAmbienceBGM();
				m_GameUI.ShowFailed(true);
				m_StatusStep = 1;
				/*if (OpenClikPlugin.IsAdReady())
				{
					OpenClikPlugin.Show(false);
				}*/
			}
			break;
		}
	}

	protected virtual void UpdateStatus_GameLeave(float deltaTime)
	{
		m_StatusTimeCount += deltaTime;
		if (!(m_StatusTimeCount < m_StatusTime))
		{
			m_Status = kGameStatus.None;
			m_StatusTimeCount = 0f;
			if (m_LeaveGameScene == kGameSceneEnum.None)
			{
				m_LeaveGameScene = kGameSceneEnum.Home;
			}
			iGameApp.GetInstance().EnterScene(m_LeaveGameScene);
		}
	}

	protected virtual void UpdateStatus_Gaming(float deltaTime)
	{
		m_GameState.AddGameTime(deltaTime);
		UpdateAssistAim(deltaTime);
		List<CCharMob> list = new List<CCharMob>();
		foreach (CCharMob value in m_MobMap.Values)
		{
			if (value.isNeedDestroy)
			{
				list.Add(value);
			}
		}
		foreach (CCharMob item in list)
		{
			RemoveMob(item);
		}
		if (m_MGManager != null)
		{
			m_MGManager.Update(deltaTime);
		}
		if (m_EventManager != null)
		{
			m_EventManager.Update(deltaTime);
		}
		if (m_TaskManager == null)
		{
			return;
		}
		m_TaskManager.Update(deltaTime);
		if (m_TaskManager.isAllCompleted || m_TaskManager.isFailed)
		{
			FinishGame();
			if (IsRoomMaster())
			{
				if (m_TaskManager.isAllCompleted)
				{
					CGameNetSender.GetInstance().TaskCompelete(true);
				}
				else if (m_TaskManager.isFailed)
				{
					CGameNetSender.GetInstance().TaskCompelete(false);
				}
			}
			return;
		}
		List<CTaskBase> taskList = m_TaskManager.GetTaskList();
		if (taskList == null || m_curTriggerManagerEnd == null)
		{
			return;
		}
		foreach (CTaskBase item2 in taskList)
		{
			CTaskInfo taskInfo = item2.GetTaskInfo();
			if (taskInfo == null)
			{
				continue;
			}
			switch (taskInfo.nType)
			{
			case 1:
				if (!(m_User == null) && m_User.IsTakenItem() && IsInsideActiveTrigger(m_User.Pos))
				{
					int carryItem = m_User.GetCarryItem();
					m_TaskManager.OnGetItem(carryItem);
					m_User.DropItem();
					AdvanceTriggerEnd();
					AddStealItem(carryItem, 1);
					CAchievementManager.GetInstance().AddAchievement(7, new object[1] { carryItem });
					CGameNetSender.GetInstance().PlayerTakeItem(-1);
					CGameNetSender.GetInstance().TaskGetItem(carryItem);
					CEventManager eventManager = m_MGManager.GetEventManager();
					if (eventManager != null)
					{
						eventManager.Trigger(new EventCondition_StealEgg_Home(carryItem, GetStealItem(carryItem)));
					}
				}
				break;
			case 3:
				foreach (CCharMob value2 in m_MobMap.Values)
				{
					if (m_curTriggerManagerEnd.IsInside2D(value2.Pos))
					{
						m_TaskManager.OnMonsterEnter(value2.ID);
					}
				}
				break;
			}
		}
	}
	
	private bool IsInsideActiveTrigger(Vector3 pos)
	{
		if (m_nCurTriggerEndIndex >= m_ltTriggerEndPoints.Count)
			return false;

		return m_ltTriggerEndPoints[m_nCurTriggerEndIndex].IsInside2D(pos);
	}

	protected virtual void UpdateStatus_CutScene(float deltaTime)
	{
		m_GameState.AddGameTime(deltaTime);
		if (m_MGManager != null)
		{
			m_MGManager.Update(deltaTime);
		}
		if (m_EventManager != null)
		{
			m_EventManager.Update(deltaTime);
		}
	}

	public CControlBase GetInput()
	{
		return m_Input;
	}

	public void StartMobCG()
	{
		foreach (CCharMob value in m_MobMap.Values)
		{
			value.SetActive(false);
		}
		m_GameUI.MovieUIIn(1f);
		m_GameUI.HideGameUI();
		m_GameUI.ShowSkipUI(true);
		m_User.SetFire(false);
		m_User.MoveStop();
        Screen.lockCursor = false;
    }

	public void FinishMobCG()
	{
		foreach (CCharMob value in m_MobMap.Values)
		{
			value.SetActive(true);
		}
		m_Status = kGameStatus.GameBegin;
		m_StatusTime = 1f;
		m_StatusTimeCount = 0f;
		m_GameUI.FadeIn(1f);
		m_GameUI.MovieUIOut(1f);
		m_GameUI.ShowGameUI();
		m_GameUI.ShowSkipUI(false);
		Screen.lockCursor = true;
    }

	public void StartWaveCG(int nWaveID)
	{
		if (m_Status != kGameStatus.Gameing || m_ltCGWave.Contains(nWaveID))
		{
			return;
		}
		WaveInfo waveInfo = m_GameData.GetWaveInfo(nWaveID);
		if (waveInfo == null)
		{
			return;
		}
		if (waveInfo.sCutScene.Length > 0)
		{
			CCameraRoam.CCGInfo cCGInfo = new CCameraRoam.CCGInfo();
			cCGInfo.sCG = waveInfo.sCutScene;
			cCGInfo.sCGContent = waveInfo.sCutSceneContent;
			cCGInfo.sCGAmbience = waveInfo.sCutSceneAmbience;
			cCGInfo.sCGBGM = waveInfo.sCutSceneBGM;
			if (CCameraRoam.GetInstance().Start(Camera.main, cCGInfo, StartMobCG, FinishMobCG))
			{
				m_Status = kGameStatus.CutScene;
				m_ltCGWave.Add(nWaveID);
			}
		}
		else
		{
			CSoundScene.GetInstance().PlayBGM(waveInfo.sCutSceneBGM);
		}
	}

	public CCharMob AddMobByWave(int nMobID, int nMobLevel, int nMobUID, int nWaveID, int nSequence, Vector3 v3Pos, Vector3 v3Dir)
	{
		CMobInfoLevel mobInfo = m_GameData.GetMobInfo(nMobID, nMobLevel);
		if (mobInfo == null)
		{
			return null;
		}
		CCharMob cCharMob = AddMob(nMobID, nMobLevel, nMobUID, v3Pos, v3Dir);
		if (cCharMob == null)
		{
			return null;
		}
		cCharMob.GenerateWaveID = nWaveID;
		cCharMob.GenerateSequence = nSequence;
		AddWaveMobNumber(nWaveID, 1);
		CEventManager eventManager = m_MGManager.GetEventManager();
		if (eventManager != null)
		{
			eventManager.Trigger(new EventCondition_MobByWave(nWaveID, nSequence, 0));
			eventManager.Trigger(new EventCondition_MobByID(nMobID, 0));
		}
		return cCharMob;
	}

	public CCharMob AddMob(int nMobID, int nMobLevel, int nMobUID, Vector3 v3Pos, Vector3 v3Dir)
	{
		CMobInfoLevel mobInfo = m_GameData.GetMobInfo(nMobID, nMobLevel);
		if (mobInfo == null)
		{
			return null;
		}
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
		if (IsMonsterNumFull(nMobID, mobInfo.nType, num))
		{
			return null;
		}
		GameObject gameObject = PrefabManager.Get(mobInfo.nModel);
		if (gameObject == null)
		{
			return null;
		}
		GameObject gameObject2 = (GameObject)Object.Instantiate(gameObject);
		if (gameObject2 == null)
		{
			return null;
		}
		CCharMob component = gameObject2.GetComponent<CCharMob>();
		if (component == null)
		{
			return null;
		}
		component.UID = nMobUID;
		component.gameObject.name = "mob_" + component.UID;
		component.InitAI(mobInfo.nAIManagerID);
		component.InitMob(nMobID, nMobLevel);
		component.MobType = mobInfo.nType;
		component.MobBehavior = num;
		component.name = "mob_" + component.UID;
		RaycastHit hitInfo;
		if (Physics.Raycast(new Ray(v3Pos + new Vector3(0f, 10f, 0f), Vector3.down), out hitInfo, 20f, 536870912))
		{
			v3Pos = hitInfo.point;
		}
		component.Pos = v3Pos;
		if (m_User != null)
		{
			component.Dir2D = m_User.Pos - component.Pos;
		}
		else
		{
			component.Dir2D = v3Dir;
		}
		m_MobMap.Add(component.UID, component);
		AddMonsterNumLimit(nMobID, mobInfo.nType, num);
		return component;
	}

	public CCharUser AddUser(int nID, Vector3 v3Pos, Vector3 v3Dir)
	{
		GameObject gameObject = PrefabManager.Get(nID);
		if (gameObject == null)
		{
			return null;
		}
		GameObject gameObject2 = (GameObject)Object.Instantiate(gameObject);
		if (gameObject2 == null)
		{
			return null;
		}
		CCharUser cCharUser = gameObject2.AddComponent<CCharUser>();
		if (cCharUser == null)
		{
			return null;
		}
		cCharUser.Pos = v3Pos;
		cCharUser.Dir2D = v3Dir;
		if (TNetManager.GetInstance().Connection != null)
		{
			cCharUser.UID = TNetManager.GetInstance().Connection.Myself.Id;
		}
		else
		{
			cCharUser.UID = 0;
		}
		cCharUser.name = "main_player";
		m_PlayerMap.Add(cCharUser.UID, cCharUser);
		return cCharUser;
	}

	public CCharPlayer AddPlayer(int nID, int nUID, Vector3 v3Pos, Vector3 v3Dir)
	{
		GameObject gameObject = PrefabManager.Get(nID);
		if (gameObject == null)
		{
			return null;
		}
		GameObject gameObject2 = (GameObject)Object.Instantiate(gameObject);
		if (gameObject2 == null)
		{
			return null;
		}
		CCharPlayer cCharPlayer = gameObject2.AddComponent<CCharPlayer>();
		if (cCharPlayer == null)
		{
			return null;
		}
		cCharPlayer.UID = nUID;
		cCharPlayer.gameObject.name = "player_" + cCharPlayer.UID;
		cCharPlayer.Pos = v3Pos;
		cCharPlayer.Dir2D = v3Dir;
		cCharPlayer.AimTo(cCharPlayer.Pos + cCharPlayer.Dir2D * 1000f);
		m_GameUI.AddTeamMateProtrait(cCharPlayer.UID);
		m_PlayerMap.Add(cCharPlayer.UID, cCharPlayer);
		return cCharPlayer;
	}

	public void RemoveMob(CCharMob charmob)
	{
		if (!(charmob == null))
		{
			m_MobMap.Remove(charmob.UID);
			charmob.Destroy();
			if (m_MGManager != null)
			{
				m_MGManager.NotifyMobDied();
			}
		}
	}

	public void RemoveMob(int nUID)
	{
		if (m_MobMap.ContainsKey(nUID))
		{
			RemoveMob(m_MobMap[nUID]);
		}
	}

	public void RemovePlayer(int nUID)
	{
		if (m_PlayerMap.ContainsKey(nUID))
		{
			m_PlayerMap[nUID].Destroy();
			m_PlayerMap.Remove(nUID);
			m_GameUI.DelTeamMateProtrait(nUID);
		}
	}

	public void AddWaveMobNumber(int nWaveID, int nNum)
	{
		if (!m_dictWaveMobNumber.ContainsKey(nWaveID))
		{
			m_dictWaveMobNumber.Add(nWaveID, 0);
		}
		Dictionary<int, int> dictWaveMobNumber;
		Dictionary<int, int> dictionary = (dictWaveMobNumber = m_dictWaveMobNumber);
		int key;
		int key2 = (key = nWaveID);
		key = dictWaveMobNumber[key];
		dictionary[key2] = key + nNum;
	}

	public int GetWaveMobNumber(int nWaveID)
	{
		if (!m_dictWaveMobNumber.ContainsKey(nWaveID))
		{
			return 0;
		}
		return m_dictWaveMobNumber[nWaveID];
	}

	public void AddStealItem(int nItemID, int nNum)
	{
		if (!m_dictStealItem.ContainsKey(nItemID))
		{
			m_dictStealItem.Add(nItemID, 0);
		}
		Dictionary<int, int> dictStealItem;
		Dictionary<int, int> dictionary = (dictStealItem = m_dictStealItem);
		int key;
		int key2 = (key = nItemID);
		key = dictStealItem[key];
		dictionary[key2] = key + nNum;
	}

	public int GetStealItem(int nItemID)
	{
		if (!m_dictStealItem.ContainsKey(nItemID))
		{
			return 0;
		}
		return m_dictStealItem[nItemID];
	}

	public void DelPlayer(int nUID)
	{
		if (m_PlayerMap.ContainsKey(nUID))
		{
			Object.Destroy(m_PlayerMap[nUID].gameObject);
			m_PlayerMap.Remove(nUID);
		}
	}

	public void AddBulletTrack(Vector3 v3Src, Vector3 v3Dst, int nPrefab)
	{
		GameObject poolObject = PrefabManager.GetPoolObject(nPrefab, 0f);
		if (!(poolObject == null))
		{
			iBulletTrack component = poolObject.GetComponent<iBulletTrack>();
			if (!(component == null))
			{
				component.Initialize(v3Src, v3Dst, 200f);
				poolObject.SetActiveRecursively(true);
			}
		}
	}

	public void AddHitEffect(Vector3 v3Pos, Vector3 v3Dir, int nPrefab)
	{
		GameObject poolObject = PrefabManager.GetPoolObject(nPrefab, 2f);
		if (!(poolObject == null))
		{
			poolObject.transform.position = v3Pos;
			poolObject.transform.forward = v3Dir;
			poolObject.SetActiveRecursively(true);
		}
	}

	public GameObject AddFireEffect(Transform transform, Vector3 v3Dir, int nPrefab, float fTime = 2f)
	{
		if (transform == null)
		{
			return null;
		}
		GameObject poolObject = PrefabManager.GetPoolObject(nPrefab, fTime);
		if (poolObject == null)
		{
			return null;
		}
		poolObject.transform.parent = transform;
		poolObject.transform.localPosition = Vector3.zero;
		poolObject.transform.forward = v3Dir;
		poolObject.SetActiveRecursively(true);
		if (fTime > 0f)
		{
		}
		return poolObject;
	}

	public iSpawnBullet AddSpawn(int nUID, int nID, Vector3 v3Pos, Vector3 v3Force, int[] arrFunc, int[] arrValueX, int[] arrValueY)
	{
		GameObject gameObject = PrefabManager.Get(nID);
		if (gameObject == null)
		{
			return null;
		}
		GameObject gameObject2 = (GameObject)Object.Instantiate(gameObject);
		if (gameObject2 == null)
		{
			return null;
		}
		iSpawnBullet component = gameObject2.GetComponent<iSpawnBullet>();
		if (component == null)
		{
			return null;
		}
		component.Initialize(nUID, nID, v3Pos, v3Force, arrFunc, arrValueX, arrValueY);
		return component;
	}

	public GameObject AddEffect(Vector3 v3Pos, Vector3 v3Dir, float fTime, int nPrefab)
	{
		GameObject gameObject = PrefabManager.Get(nPrefab);
		if (gameObject == null)
		{
			return null;
		}
		GameObject gameObject2 = (GameObject)Object.Instantiate(gameObject);
		if (gameObject2 == null)
		{
			return null;
		}
		gameObject2.transform.position = v3Pos;
		gameObject2.transform.forward = v3Dir;
		Object.Destroy(gameObject2, fTime);
		return gameObject2;
	}

	public GameObject AddSceneGameObject(int nPrefab, Vector3 v3Pos, Vector3 v3Dir, float fDisappearTime = -1f)
	{
		Object @object = PrefabManager.Get(nPrefab);
		if (@object == null)
		{
			return null;
		}
		GameObject gameObject = (GameObject)Object.Instantiate(@object, v3Pos, Quaternion.LookRotation(v3Dir));
		if (gameObject == null)
		{
			return null;
		}
		if (fDisappearTime > 0f)
		{
			Object.Destroy(gameObject, fDisappearTime);
		}
		m_ltSceneGameObject.Add(gameObject);
		return gameObject;
	}

	public void AddItem(int nItemID, Vector3 v3Pos, Vector3 v3Dir, float fDisappearTime = -1f, int nItemUID = -1)
	{
		CItemInfoLevel itemInfo = m_GameData.GetItemInfo(nItemID, 1);
		if (itemInfo == null)
		{
			return;
		}
		GameObject gameObject = PrefabManager.Get(itemInfo.nModel);
		if (gameObject == null)
		{
			return;
		}
		GameObject gameObject2 = (GameObject)Object.Instantiate(gameObject);
		if (gameObject2 == null)
		{
			return;
		}
		gameObject2.transform.position = v3Pos;
		gameObject2.transform.forward = v3Dir;
		iItem component = gameObject2.GetComponent<iItem>();
		if (component != null)
		{
			component.Initialize(nItemID);
			component.UID = nItemUID;
			component.AddForce(v3Dir);
			if (component.isHasScreenTip)
			{
				component.m_ScreenTip = m_GameUI.CreateScreenTip(m_User.gameObject, gameObject2);
				component.m_ScreenTip.SetIcon("dan");
			}
		}
		if (fDisappearTime > 0f)
		{
			Object.Destroy(gameObject2, fDisappearTime);
		}
		m_ltItem.Add(gameObject2);
	}

	public void AddGold(int nGold, Vector3 v3Pos, Vector3 v3Dir, float fScaleRate, int nGroundEffectPrefab = -1)
	{
		GameObject gameObject = PrefabManager.Get(251);
		if (gameObject == null)
			return;
		GameObject gameObject2 = (GameObject)Object.Instantiate(gameObject);
		if (gameObject2 == null)
			return;
		gameObject2.transform.position = v3Pos;
		gameObject2.transform.forward  = v3Dir;
		gameObject2.transform.localScale *= fScaleRate;
		iItem component = gameObject2.GetComponent<iItem>();
		if (component != null)
		{
			component.Initialize(50001);
			component.UID = -1;
			component.AddForce(v3Dir);
			component.UpdateFunc(0, 100, nGold, 0);
			if (component.isHasScreenTip)
			{
				component.m_ScreenTip = m_GameUI.CreateScreenTip(m_User.gameObject, gameObject2);
				component.m_ScreenTip.SetIcon("dan");
			}
		}
		if (nGroundEffectPrefab > 0)
		{
			iItemDynamic dynItem = gameObject2.GetComponent<iItemDynamic>();
			if (dynItem != null)
			{
				GameObject effectPrefab = PrefabManager.Get(nGroundEffectPrefab);
				if (effectPrefab != null)
				{
					GameObject effectInst = (GameObject)Object.Instantiate(effectPrefab);
					if (effectInst != null)
					{
						effectInst.transform.parent        = gameObject2.transform;
						effectInst.transform.localPosition = Vector3.zero;
						effectInst.transform.localRotation = Quaternion.identity;
						effectInst.SetActiveRecursively(false);
						dynItem.m_GroundEffect = effectInst;
					}
				}
			}
		}
 
		m_ltItem.Add(gameObject2);
	}

	public List<GameObject> GetSceneItemList()
	{
		return m_ltItem;
	}

	public iCameraTrail GetCamera()
	{
		return m_CameraTrail;
	}

	public iCameraReveal GetCameraReveal()
	{
		return m_CameraReveal;
	}

	public CCharUser GetUser()
	{
		return m_User;
	}

	public IEnumerable GetMobEnumerator()
	{
		foreach (CCharMob value in m_MobMap.Values)
		{
			yield return value;
		}
	}

	public CCharMob GetMob(int uid)
	{
		if (!m_MobMap.ContainsKey(uid))
		{
			return null;
		}
		return m_MobMap[uid];
	}

	public CCharPlayer GetPlayer(int uid)
	{
		if (!m_PlayerMap.ContainsKey(uid))
		{
			return null;
		}
		return m_PlayerMap[uid];
	}

	public IEnumerable GetPlayerEnumerator()
	{
		foreach (CCharPlayer value in m_PlayerMap.Values)
		{
			yield return value;
		}
	}

	public int GetPlayerCount()
	{
		if (m_PlayerMap == null)
		{
			return 0;
		}
		return m_PlayerMap.Count;
	}

	public List<CCharBase> GetUnitList()
	{
		List<CCharBase> list = new List<CCharBase>();
		foreach (CCharMob value in m_MobMap.Values)
		{
			list.Add(value);
		}
		foreach (CCharPlayer value2 in m_PlayerMap.Values)
		{
			list.Add(value2);
		}
		return list;
	}

	public bool IsSkyScene()
	{
		return m_bIsSkyScene;
	}

	public void SendAttack(int uid_atk, int uid_def, int skillid)
	{
		if (!(m_User == null))
		{
			CCharMob mob = GetMob(uid_def);
			if (!(mob == null))
			{
			}
		}
	}

	public void OnAttackMsg(int uid_atk, int uid_def, int skillid)
	{
	}

	public bool WorldToScreenPoint(Vector3 v3WorldPos, ref Vector2 v2ScreenPos)
	{
		v3WorldPos = Camera.main.WorldToScreenPoint(v3WorldPos);
		if (v3WorldPos.z < 0f)
		{
			return false;
		}
		v2ScreenPos.x = v3WorldPos.x;
		v2ScreenPos.y = v3WorldPos.y;
		return true;
	}

	public bool WorldToScreenPointNGUI(Vector3 v3World, ref Vector3 v3Screen)
	{
		if (m_CameraTrail == null)
		{
			return false;
		}
		v3Screen = m_CameraTrail.GetComponent<Camera>().WorldToScreenPoint(v3World);
		v3Screen -= m_GameState.GetScreenCenterV3();
		return true;
	}

	public bool WorldToScreenPointTUI(Vector3 v3World, ref Vector3 v3Screen)
	{
		return false;
	}

	public iGameLogic GetGameLogic()
	{
		return m_GameLogic;
	}

	public iGameUIBase GetGameUI()
	{
		return m_GameUI;
	}

	public CStartPointManager GetSPManagerGround()
	{
		return m_curSPManagerGround;
	}

	public CStartPointManager GetSPManagerSky()
	{
		return m_curSPManagerSky;
	}

	public CStartPointManager GetHPManager()
	{
		return m_curHPManager;
	}

	public CPathWalkerManager GetPathWalkerManager()
	{
		return m_PathWalkerManager;
	}

	public void SetUserUID(int nUID)
	{
		if (m_User.UID == -1)
		{
			m_User.UID = nUID;
		}
	}

	public void AddMonsterNumLimit(int nMobID, int nMobType, int nBehavior, int nNum = 1)
	{
		foreach (MonsterNumInfo item in m_ltMonsterNumInfo)
		{
			switch (item.nLimitType)
			{
			case 1:
				item.curNum += nNum;
				break;
			case 2:
				if (nMobID == item.nLimitValue)
				{
					item.curNum += nNum;
				}
				break;
			case 3:
				if (nMobType == item.nLimitValue)
				{
					item.curNum += nNum;
				}
				break;
			case 0:
				if (nBehavior == item.nLimitValue)
				{
					item.curNum += nNum;
				}
				break;
			}
			if (item.curNum < 0)
			{
				item.curNum = 0;
			}
		}
	}

	protected bool IsMonsterNumFull(int nMobID, int nMobType, int nBehavior)
	{
		foreach (MonsterNumInfo item in m_ltMonsterNumInfo)
		{
			switch (item.nLimitType)
			{
			case 1:
				return item.IsMax();
			case 2:
				if (nMobID == item.nLimitValue && item.IsMax())
				{
					return true;
				}
				break;
			case 3:
				if (nMobType == item.nLimitValue && item.IsMax())
				{
					return true;
				}
				break;
			case 0:
				if (nBehavior == item.nLimitValue && item.IsMax())
				{
					return true;
				}
				break;
			}
		}
		return false;
	}
	
	protected bool IsInsideCurrentTriggerEnd(Vector3 v3Pos)
	{
		if (m_ltScreenTipTriggerEnd.Count == 0)
			return m_curTriggerManagerEnd != null && m_curTriggerManagerEnd.IsInside2D(v3Pos);
		if (m_nCurTriggerEndIndex >= m_ltScreenTipTriggerEnd.Count)
			return false;
		return m_curTriggerManagerEnd != null && m_curTriggerManagerEnd.IsInside2D(v3Pos);
	}

	public void AddDamageText(float fDamage, Vector3 v3Pos, bool bCritical = false)
	{
		if (!(m_GameUI == null))
		{
			WorldToScreenPointNGUI(v3Pos, ref v3Pos);
			if (!bCritical)
			{
				m_GameUI.AddDmgUI(fDamage, v3Pos, Color.white, gyUILabelDmg.kMode.Mode2);
			}
			else
			{
				m_GameUI.AddDmgUI(fDamage, v3Pos, Color.yellow, gyUILabelDmg.kMode.Mode1);
				CUISound.GetInstance().Play("Fx_Impact_crit");
			}
		}
	}

	public void AddDamageText(float fDamage, Vector3 v3Pos, Color color, bool bCritical = false)
	{
		if (!(m_GameUI == null))
		{
			WorldToScreenPointNGUI(v3Pos, ref v3Pos);
			if (!bCritical)
			{
				m_GameUI.AddDmgUI(fDamage, v3Pos, color, gyUILabelDmg.kMode.Mode2);
			}
			else
			{
				m_GameUI.AddDmgUI(fDamage, v3Pos, color, gyUILabelDmg.kMode.Mode1);
			}
		}
	}

	public void AddExpText(int nExp, Vector3 v3Pos)
	{
		if (!(m_GameUI == null))
		{
			WorldToScreenPointNGUI(v3Pos, ref v3Pos);
			m_GameUI.AddExpText(v3Pos, nExp);
		}
	}

	public void AddGoldText(float fValue, Vector3 v3Pos)
	{
		if (!(m_GameUI == null))
		{
			WorldToScreenPointNGUI(v3Pos, ref v3Pos);
			m_GameUI.AddGoldUI(fValue, v3Pos);
		}
	}

	public void AddMaterial(Vector3 v3Pos, string sIcon, int nCount)
	{
		if (!(m_GameUI == null))
		{
			WorldToScreenPointNGUI(v3Pos, ref v3Pos);
			m_GameUI.AddMaterialUI(v3Pos, sIcon, nCount);
		}
	}

	public void AddHealText(float fHeal, Vector3 v3Pos, bool bCritical = false)
	{
		if (!(m_GameUI == null))
		{
			WorldToScreenPointNGUI(v3Pos, ref v3Pos);
			m_GameUI.AddDmgUI(fHeal, v3Pos, Color.green, gyUILabelDmg.kMode.Mode2);
		}
	}

	public Dictionary<int, CCharMob> GetMobData()
	{
		return m_MobMap;
	}

	public Dictionary<int, CCharPlayer> GetPlayerData()
	{
		return m_PlayerMap;
	}

	public int GetMobAliveCount()
	{
		if (m_MobMap.Count == 0)
		{
			return 0;
		}
		int num = 0;
		foreach (CCharMob value in m_MobMap.Values)
		{
			if (!value.isDead)
			{
				num++;
			}
		}
		return num;
	}

	public void ShakeCamera(float fShakeTime = 0.5f, float fShakeRange = 0.05f)
	{
		iCameraController cameraController = m_CameraTrail.GetCameraController();
		if (cameraController != null)
		{
			cameraController.Shake(fShakeTime, fShakeRange);
		}
	}

	public CStartPointManager GetTPManagerBegin()
	{
		return m_curTriggerManagerBegin;
	}

	public CStartPointManager GetTPManagerEnd()
	{
		return m_curTriggerManagerEnd;
	}

	public void InitTask(int nTaskID)
	{
		CTaskInfo taskInfo = m_GameData.GetTaskInfo(nTaskID);
		if (taskInfo == null)
		{
			return;
		}
		switch (taskInfo.nType)
		{
		case 1:
		{
			if (IsRoomMaster())
			{
				CTaskInfoCollect cTaskInfoCollect = taskInfo as CTaskInfoCollect;
				if (cTaskInfoCollect == null || m_curTriggerManagerBegin == null)
				{
					break;
				}
				foreach (CTaskInfoCollect.CCollectInfo item in cTaskInfoCollect.ltCollectInfo)
				{
					for (int i = 0; i < item.nMaxCount; i++)
					{
						CStartPoint random = m_curTriggerManagerBegin.GetRandom();
						if (random != null)
						{
							int uID = MyUtils.GetUID();
							Vector3 random2 = random.GetRandom();
							AddItem(item.nItemID, random2, Vector3.forward, -1f, uID);
							CGameNetSender.GetInstance().Game_AddItem(uID, item.nItemID, random2);
						}
					}
				}
			}
			Dictionary<int, CStartPoint> data = m_curTriggerManagerEnd.GetData();
			if (data != null)
			{
				m_ltTriggerEndPoints.Clear();

				foreach (var kvp in data)
				{
					m_ltTriggerEndPoints.Add(kvp.Value);
				}
				m_nCurTriggerEndIndex = 0;
				CreateCurrentTriggerEnd();
			}
			break;
		}
		case 3:
		{
			CTaskInfoDefence cTaskInfoDefence = taskInfo as CTaskInfoDefence;
			if (cTaskInfoDefence == null)
			{
				break;
			}
			GameObject gameObject = GameObject.Find("Building");
			if (gameObject != null)
			{
				m_Building = gameObject.GetComponent<iBuilding>();
				if (m_Building != null)
				{
					m_Building.Initialize(cTaskInfoDefence.fLife, cTaskInfoDefence.fLife);
				}
			}
			break;
		}
		case 2:
			break;
		}
	}

	public bool IsRoomMaster()
	{
		if (TNetManager.GetInstance().Connection != null && TNetManager.GetInstance().Connection.CurRoom != null && TNetManager.GetInstance().Connection.CurRoom.RoomMasterID != TNetManager.GetInstance().Connection.Myself.Id)
		{
			return false;
		}
		return true;
	}

	public bool IsMyself(CCharBase target)
	{
		if (target == null || target != m_User)
		{
			return false;
		}
		return true;
	}

	public bool IsPlayerAllDead()
	{
		foreach (CCharPlayer value in m_PlayerMap.Values)
		{
			if (!value.isDead)
			{
				return false;
			}
		}
		return true;
	}

	public List<Vector3> GetNavMeshPath(Vector3 point1, Vector3 point2)
	{
		List<Vector3> list = new List<Vector3>();
		if (!UnityEngine.AI.NavMesh.CalculatePath(point1, point2, -1, m_NavPath))
		{
			return list;
		}
		for (int i = 0; i < m_NavPath.corners.Length; i++)
		{
			list.Add(m_NavPath.corners[i]);
		}
		return list;
	}

	public bool IsAssistAim()
	{
		return m_AssistAimState != kAssistAimState.None;
	}

	public void AssistAim_Start(float fDelayTime = 0.5f)
	{
	}

	public void AssistAim_Stop()
	{
	}

	protected void UpdateAssistAim(float deltaTime)
	{
	}

	public void ShowItemScreenTip(bool bShow)
	{
		foreach (GameObject item in m_ltItem)
		{
			if (!(item == null))
			{
				iItem component = item.GetComponent<iItem>();
				if (!(component == null) && !(component.m_ScreenTip == null))
				{
					component.m_ScreenTip.isActive = bShow;
				}
			}
		}
	}

	public void ShowTriggerEndScreenTip(bool bShow)
	{
		if (m_curTriggerEndTip != null && m_curTriggerEndTip.screentip != null)
		{
			m_curTriggerEndTip.screentip.isActive = bShow;
		}
	}

	public void AdvanceTriggerEnd()
	{
		DestroyCurrentTriggerEnd();
		m_nCurTriggerEndIndex++;
		if (m_nCurTriggerEndIndex < m_ltTriggerEndPoints.Count)
		{
			CreateCurrentTriggerEnd();
		}
	}

	
	public void CreateCurrentTriggerEnd()
	{
		DestroyCurrentTriggerEnd();
		if (m_nCurTriggerEndIndex >= m_ltTriggerEndPoints.Count)
			return;
		CStartPoint point = m_ltTriggerEndPoints[m_nCurTriggerEndIndex];
		m_curTriggerEndTip = new CPointScreenTip();
		m_curTriggerEndTip.agent = new GameObject("triggerend_" + m_nCurTriggerEndIndex);
		if (m_curTriggerEndTip.agent == null)
			return;
		m_curTriggerEndTip.agent.transform.position = point.GetCenter();
		m_curTriggerEndTip.screentip =
			m_GameUI.CreateScreenTip(m_User.gameObject, m_curTriggerEndTip.agent);
		if (m_curTriggerEndTip.screentip != null)
		{
			m_curTriggerEndTip.screentip.SetIcon("baoxiang");
			m_curTriggerEndTip.screentip.isActive = m_User != null && m_User.IsTakenItem();
		}
	}
	
	void DestroyCurrentTriggerEnd()
	{
		if (m_curTriggerEndTip != null)
		{
			if (m_curTriggerEndTip.screentip != null)
				Object.Destroy(m_curTriggerEndTip.screentip.gameObject);
			if (m_curTriggerEndTip.agent != null)
				Object.Destroy(m_curTriggerEndTip.agent);
			m_curTriggerEndTip = null;
		}
	}

	public bool IsLevelUp(int nExp)
	{
		CCharacterInfoLevel curCharInfoLevel = m_User.CurCharInfoLevel;
		if (curCharInfoLevel == null)
		{
			return false;
		}
		if (curCharInfoLevel.nExp > m_User.EXP + nExp)
		{
			return false;
		}
		return true;
	}

	public void PlayAudio(Vector3 v3Pos, string sAuido)
	{
		GameObject gameObject = new GameObject("tempsound");
		if (!(gameObject == null))
		{
			gameObject.transform.position = v3Pos;
			TAudioController tAudioController = gameObject.AddComponent<TAudioController>();
			if (!(tAudioController == null))
			{
				tAudioController.PlayAudio(sAuido);
				Object.Destroy(gameObject, 15f);
			}
		}
	}

	public void SetGamePause(bool bPause)
	{
		if (bPause && m_Status != kGameStatus.Gameing)
		{
			return;
		}
		if (bPause)
		{
			m_LastStatus = m_Status;
			m_Status = kGameStatus.Pause;
			if (m_User != null)
			{
				m_User.SetFire(false);
				m_User.MoveStop();
			}
			/*if (OpenClikPlugin.IsAdReady())
			{
				OpenClikPlugin.Show(false);
			}*/
			CSoundScene.GetInstance().StopBGM();
		}
		else
		{
			m_Status = m_LastStatus;
			//OpenClikPlugin.Hide();
			CSoundScene.GetInstance().PlayBGM(string.Empty);
		}
		SetPause(bPause);
		m_GameUI.ShowPauseUI(bPause);
	}

	public void SetPause(bool bPause)
	{
		if (m_bPause == bPause)
		{
			return;
		}
		m_bPause = bPause;
		foreach (CCharMob value in m_MobMap.Values)
		{
			value.SetActive(!bPause);
		}
		foreach (CCharPlayer value2 in m_PlayerMap.Values)
		{
			value2.SetActive(!bPause);
		}
	}

	public void FullWeaponBullet()
	{
		if (!(m_User == null))
		{
			CWeaponBase weapon = m_User.GetWeapon();
			if (weapon != null && weapon.BulletNum != weapon.BulletNumMax)
			{
				weapon.FullBullet();
			}
		}
	}

	public void StartIAPPurchase(int nNeed)
	{
		Debug.Log("StartIAPPurchase " + m_Status);
	}

	public void FinishIAPPurchase()
	{
	}

	protected void IAPPurchaseSuccess()
	{
	}

	protected void IAPPurchaseFailed()
	{
	}

	public void PurchaseIAP(int nIAP)
	{
		iIAPManager.GetInstance().Purchase(nIAP);
		iIAPManager.GetInstance().SetSuccessFunc(OnPurchaseIAPSuccess);
		iIAPManager.GetInstance().SetFailedFunc(OnPurchaseIAPFailed);
		iIAPManager.GetInstance().SetCancelFunc(OnPurchaseIAPCancel);
		iIAPManager.GetInstance().SetNetErrorFunc(OnPurchaseIAPNetError);
		iIAPManager.GetInstance().SetOnSendVerifyFunc(OnPurchaseIAPSendVerify);
		iIAPManager.GetInstance().SetOnVerifyFailed(OnPurchaseIAPFailed);
	}

	public void OnPurchaseIAPSuccess()
	{
		CUISound.GetInstance().Play("UI_Crystal");
		m_GameUI.ShowMessageBox("Purchase Successed!", gyUIMessageBox.kMessageBoxType.OK, IAPPurchaseSuccess);
	}

	public void OnPurchaseIAPFailed()
	{
		CUISound.GetInstance().Play("UI_Error");
		m_GameUI.ShowMessageBox("Purchase Failed!", gyUIMessageBox.kMessageBoxType.OK, IAPPurchaseFailed);
	}

	public void OnPurchaseIAPCancel()
	{
		CUISound.GetInstance().Play("UI_Error");
		IAPPurchaseFailed();
		m_GameUI.HideMessageBox();
	}

	public void OnPurchaseIAPNetError()
	{
		CUISound.GetInstance().Play("UI_Error");
		m_GameUI.ShowMessageBox("Connect Error!", gyUIMessageBox.kMessageBoxType.OK, IAPPurchaseFailed);
	}

	public void OnPurchaseIAPSendVerify()
	{
		m_GameUI.ShowMessageBox("Please wait while the system is verifying your purphase...", gyUIMessageBox.kMessageBoxType.None);
	}
}
