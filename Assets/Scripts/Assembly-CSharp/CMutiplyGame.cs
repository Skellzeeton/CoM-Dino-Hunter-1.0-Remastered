using TNetSdk;
using UnityEngine;

public class CMutiplyGame
{
	public enum kNetState
	{
		None = 0,
		Connect = 1,
		Login = 2
	}

	public enum kNetGameState
	{
		None = 0,
		SearchRoom = 1,
		JoinRoom = 2,
		WaitingForPlayer = 3,
		WaitingForSearch = 4,
		Gaming = 5
	}

	protected static CMutiplyGame m_Instance;

	protected iDataCenter m_DataCenter;

	protected iGameState m_GameState;

	protected kNetState m_NetState;

	protected kNetGameState m_NetGameState;

	public kNetState NetState
	{
		get
		{
			return m_NetState;
		}
	}

	public kNetGameState NetGameState
	{
		get
		{
			return m_NetGameState;
		}
	}

	public CMutiplyGame()
	{
		m_NetState = kNetState.None;
		m_NetGameState = kNetGameState.None;
	}

	public static CMutiplyGame GetInstance()
	{
		if (m_Instance == null)
		{
			m_Instance = new CMutiplyGame();
		}
		return m_Instance;
	}

	public void Initialize()
	{
		m_GameState = iGameApp.GetInstance().m_GameState;
		iGameData gameData = iGameApp.GetInstance().m_GameData;
		if (gameData != null)
		{
			m_DataCenter = gameData.GetDataCenter();
		}
	}

	public void OnConnectSuccess(TNetEventData tEvent)
	{
		iGameApp.GetInstance().ScreenLog("connect successed!");
		TNetManager.GetInstance().Login(m_GameState.UserName, string.Empty);
	}

	public void OnLogin(TNetEventData tEvent)
	{
		SysLoginResCmd.Result result = (SysLoginResCmd.Result)(int)tEvent.data["result"];
		iGameApp.GetInstance().ScreenLog("login result " + result);
		if (result != 0)
		{
			DisConnect();
			return;
		}
		iGameApp.GetInstance().EnterScene(kGameSceneEnum.Room);
		m_GameState.BattleLevel = Random.Range(0, 150);
		CRoomManager.CRoomCharInfo cRoomCharInfo = new CRoomManager.CRoomCharInfo();
		if (cRoomCharInfo != null)
		{
			cRoomCharInfo.m_nCharID = m_DataCenter.CurCharID;
			cRoomCharInfo.m_nBattleLevel = m_GameState.BattleLevel;
			CRoomManager.GetInstance().SendUserVariable(cRoomCharInfo);
		}
		CRoomManager.GetInstance().Initialize();
		CRoomManager.GetInstance().SearchRoom(m_GameState.BattleLevel);
	}

	public void Connect(string sIP, int nPort)
	{
		TNetManager.GetInstance().Connect(sIP, nPort);
		TNetObject netObject = TNetManager.GetInstance().NetObject;
		if (netObject != null)
		{
			netObject.AddEventListener(TNetEventSystem.CONNECTION, OnConnectSuccess);
			netObject.AddEventListener(TNetEventSystem.LOGIN, OnLogin);
		}
	}

	public void DisConnect()
	{
		TNetManager.GetInstance().DisConnect();
		UnRegisterEvent();
	}

	public void UnRegisterEvent()
	{
		TNetObject netObject = TNetManager.GetInstance().NetObject;
		if (netObject != null)
		{
			netObject.RemoveEventListener(TNetEventSystem.CONNECTION, OnConnectSuccess);
			netObject.RemoveEventListener(TNetEventSystem.LOGIN, OnLogin);
		}
	}
}
