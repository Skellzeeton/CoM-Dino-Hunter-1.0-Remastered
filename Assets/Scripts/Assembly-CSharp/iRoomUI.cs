using TNetSdk;
using UnityEngine;

public class iRoomUI : MonoBehaviour, TUIHandler
{
	public TUI m_TUI;

	protected Transform m_TUIControls;

	public Camera m_TUICamera;

	protected iGameState m_GameState;

	protected iGameData m_GameData;

	protected bool m_bUpdate;

	protected bool m_bEnterGame;

	protected bool m_bLeftTime;

	protected float m_fLeftTime;

	protected TUILabel m_UserLabel;

	protected TUILabel[] m_arrLabel;

	protected TUILabel m_Title;

	protected TUILabel m_Time;

	private void Awake()
	{
		TNetManager.GetInstance().AddAcceptMsgFunc(OnAcceptMsg);
		m_GameState = iGameApp.GetInstance().m_GameState;
		m_GameData = iGameApp.GetInstance().m_GameData;
		GameObject gameObject = GameObject.Find("TUI");
		if (!(gameObject == null))
		{
			m_TUI = gameObject.GetComponent<TUI>();
			m_TUIControls = m_TUI.transform.Find("TUIControls");
			Transform transform = m_TUI.transform.Find("TUICamera");
			if (transform != null)
			{
				m_TUICamera = transform.GetComponent<Camera>();
			}
			m_UserLabel = GetControl<TUILabel>("MainUser");
			m_arrLabel = new TUILabel[2];
			for (int i = 0; i < 2; i++)
			{
				m_arrLabel[i] = GetControl<TUILabel>("Player" + (i + 1));
			}
			m_Title = GetControl<TUILabel>("dlgReady/Title");
			m_Time = GetControl<TUILabel>("dlgReady/Time");
			m_bEnterGame = false;
		}
	}

	private void Update()
	{
		if (TNetManager.GetInstance().Connection != null && m_UserLabel != null)
		{
			m_UserLabel.Text = TNetManager.GetInstance().Connection.Myself.Name + " [" + m_GameState.BattleLevel + "]";
		}
		if (CRoomManager.GetInstance().State == CRoomManager.kState.GameConfirm)
		{
			if (!m_bLeftTime)
			{
				m_bLeftTime = true;
				m_fLeftTime = 5f;
				m_Title.gameObject.SetActiveRecursively(true);
				m_Time.gameObject.SetActiveRecursively(true);
			}
		}
		else
		{
			m_bLeftTime = false;
			m_Title.gameObject.SetActiveRecursively(false);
			m_Time.gameObject.SetActiveRecursively(false);
		}
		if (m_bLeftTime)
		{
			if (m_fLeftTime <= 0f)
			{
				if (!m_bEnterGame)
				{
					CRoomManager.GetInstance().Destroy();
					SendStartGameMsg();
					m_bEnterGame = true;
				}
			}
			else
			{
				m_fLeftTime -= Time.deltaTime;
				m_Time.Text = m_fLeftTime.ToString();
			}
		}
		CRoomManager.CRoomCharInfo[] roomCharInfoArr = CRoomManager.GetInstance().GetRoomCharInfoArr();
		if (roomCharInfoArr == null)
		{
			return;
		}
		for (int i = 0; i < roomCharInfoArr.Length; i++)
		{
			if (roomCharInfoArr[i].m_Player == null)
			{
				SetName(i, "None");
				continue;
			}
			SetName(i, roomCharInfoArr[i].m_Player.Name + " [" + roomCharInfoArr[i].m_nBattleLevel + "]");
		}
	}

	public void HandleEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
	}

	protected void SetName(int nIndex, string sName)
	{
		if (nIndex >= 0 && nIndex < m_arrLabel.Length)
		{
			m_arrLabel[nIndex].Text = sName;
		}
	}

	public T GetControl<T>(string path) where T : MonoBehaviour
	{
		Transform transform = m_TUI.transform.Find("TUIControls/" + path);
		if (transform == null)
		{
			return (T)null;
		}
		return transform.GetComponent<T>();
	}

	public GameObject GetControl(string path)
	{
		Transform transform = m_TUI.transform.Find("TUIControls/" + path);
		if (transform == null)
		{
			return null;
		}
		return transform.gameObject;
	}

	public void SendStartGameMsg()
	{
		TNetObject connection = TNetManager.GetInstance().Connection;
		if (connection == null || connection.CurRoom == null || connection.Myself.Id != connection.CurRoom.RoomMasterID)
		{
			return;
		}
		int[] array = new int[6] { 10001, 10002, 10003, 10004, 10005, 10006 };
		m_GameState.GameLevel = array[Random.Range(0, array.Length)];
		GameLevelInfo gameLevelInfo = m_GameData.GetGameLevelInfo(m_GameState.GameLevel);
		if (gameLevelInfo == null)
		{
			return;
		}
		CStartPointManager cStartPointManager = new CStartPointManager();
		cStartPointManager.Load("_Config/_StartPoint/StartPoint_" + gameLevelInfo.nBirthPos);
		nmsg_startgame nmsg_startgame2 = new nmsg_startgame();
		nmsg_startgame2.msghead = kGameNetEnum.GAME_START;
		nmsg_startgame2.nGameLevel = m_GameState.GameLevel;
		int userCount = connection.CurRoom.UserCount;
		for (int i = 0; i < userCount; i++)
		{
			TNetUser tNetUser = connection.CurRoom.UserList[i];
			CStartPoint random = cStartPointManager.GetRandom();
			if (random != null)
			{
				nmsg_startgame.CPlayerPos cPlayerPos = new nmsg_startgame.CPlayerPos();
				cPlayerPos.nUID = tNetUser.Id;
				cPlayerPos.v3Pos = random.GetRandom2D();
				nmsg_startgame2.ltPlayerPos.Add(cPlayerPos);
			}
		}
		TNetManager.GetInstance().BroadcastMessage(nmsg_startgame2);
	}

	public void OnAcceptMsg(TNetUser netuser, kGameNetEnum nmsg, SFSObject data)
	{
		if (nmsg == kGameNetEnum.GAME_START)
		{
			OnGameStartMsg(netuser, data);
			TNetManager.GetInstance().DelAcceptMsgFunc(OnAcceptMsg);
		}
	}

	protected void OnGameStartMsg(TNetUser sender, SFSObject data)
	{
		nmsg_startgame nmsg_startgame2 = new nmsg_startgame();
		nmsg_startgame2.UnPack(data);
		m_GameState.GameLevel = nmsg_startgame2.nGameLevel;
		iGameApp.GetInstance().ScreenLog("GameLevel = " + m_GameState.GameLevel);
		foreach (nmsg_startgame.CPlayerPos ltPlayerPo in nmsg_startgame2.ltPlayerPos)
		{
			iGameState.CPlayerInitInfo cPlayerInitInfo = m_GameState.GetNetPlayerInitInfo(ltPlayerPo.nUID);
			if (cPlayerInitInfo == null)
			{
				cPlayerInitInfo = new iGameState.CPlayerInitInfo();
				cPlayerInitInfo.nUID = ltPlayerPo.nUID;
				cPlayerInitInfo.nCharID = 1;
				m_GameState.SetNetPlayerInitInfo(cPlayerInitInfo);
			}
			cPlayerInitInfo.v3Pos = ltPlayerPo.v3Pos;
			iGameApp.GetInstance().ScreenLog(ltPlayerPo.nUID + " " + ltPlayerPo.v3Pos);
		}
		m_GameState.isNetGame = true;
		iGameApp.GetInstance().EnterScene(kGameSceneEnum.Game);
	}
}
