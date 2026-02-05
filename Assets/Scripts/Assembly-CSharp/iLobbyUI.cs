using TNetSdk;
using UnityEngine;

public class iLobbyUI : MonoBehaviour, TUIHandler
{
	protected iGameState m_GameState;

	protected iGameData m_GameData;

	public void HandleEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
	}

	private void Awake()
	{
		TNetManager.GetInstance().AddAcceptMsgFunc(OnAcceptMsg);
		m_GameState = iGameApp.GetInstance().m_GameState;
		m_GameData = iGameApp.GetInstance().m_GameData;
	}

	private void OnGUI()
	{
		GUILayout.BeginArea(new Rect(0f, 0f, Screen.width / 2, Screen.height));
		GUILayout.BeginHorizontal();
		if (TNetManager.GetInstance().Connection == null)
		{
			if (GUILayout.Button("Connect"))
			{
				TNetManager.GetInstance().Connect("192.168.0.190", 7000);
			}
		}
		else if (!TNetManager.GetInstance().isLogin)
		{
			if (GUILayout.Button("Login"))
			{
				TNetManager.GetInstance().Login(m_GameState.UserName, string.Empty);
			}
		}
		else
		{
			GUILayout.BeginVertical();
			GUILayout.Label("Hi!  [" + m_GameState.UserName + "]");
			if (GUILayout.Button("Logout"))
			{
				TNetManager.GetInstance().DisConnect();
			}
			GUILayout.EndVertical();
		}
		GUILayout.EndHorizontal();
		if (TNetManager.GetInstance().isLogin)
		{
			if (TNetManager.GetInstance().Connection.CurRoom == null)
			{
				if (GUILayout.Button("CreateRoom"))
				{
					TNetManager.GetInstance().CreateRoom("GGYY's room", string.Empty, 1, 4, RoomCreateCmd.RoomType.limit, RoomCreateCmd.RoomSwitchMasterType.Auto, string.Empty);
				}
			}
			else
			{
				GUILayout.BeginVertical();
				TNetRoom curRoom = TNetManager.GetInstance().Connection.CurRoom;
				GUILayout.Label("room id = " + curRoom.Id);
				GUILayout.Label("room group = " + curRoom.GroupId);
				GUILayout.Label("room name = " + curRoom.Name);
				GUILayout.Label("room master = " + curRoom.CreaterName);
				GUILayout.Label("room user count = " + curRoom.UserCount);
				if (GUILayout.Button("LeaveRoom"))
				{
					TNetManager.GetInstance().LeaveRoom();
				}
				if (curRoom.CreaterName == m_GameState.UserName && GUILayout.Button("StartGame"))
				{
					SendStartGameMsg();
				}
				GUILayout.EndVertical();
			}
		}
		GUILayout.EndArea();
		GUILayout.BeginArea(new Rect(Screen.width / 2, 0f, Screen.width / 2, Screen.height));
		if (TNetManager.GetInstance().Connection != null)
		{
			GUILayout.BeginVertical();
			if (GUILayout.Button("Refresh"))
			{
				TNetManager.GetInstance().ApplyRoomList(1, 0, 10, RoomDragListCmd.ListType.all);
			}
			GUILayout.BeginHorizontal();
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		}
		GUILayout.EndArea();
	}

	public void SendStartGameMsg()
	{
	}

	public void OnAcceptMsg(TNetUser netuser, kGameNetEnum nmsg, SFSObject data)
	{
		switch (nmsg)
		{
		case kGameNetEnum.GAME_START:
			OnGameStartMsg(netuser, data);
			TNetManager.GetInstance().DelAcceptMsgFunc(OnAcceptMsg);
			break;
		case kGameNetEnum.GAME_PLAYERINFO:
			OnGamePlayerInfoMsg(netuser, data);
			break;
		case kGameNetEnum.GAME_OVER:
		case kGameNetEnum.GAME_PAUSE:
			break;
		}
	}

	protected void OnGameStartMsg(TNetUser sender, SFSObject data)
	{
		nmsg_startgame nmsg_startgame2 = new nmsg_startgame();
		nmsg_startgame2.UnPack(data);
		m_GameState.GameLevel = nmsg_startgame2.nGameLevel;
		foreach (nmsg_startgame.CPlayerPos ltPlayerPo in nmsg_startgame2.ltPlayerPos)
		{
			iGameState.CPlayerInitInfo cPlayerInitInfo = m_GameState.GetNetPlayerInitInfo(ltPlayerPo.nUID);
			if (cPlayerInitInfo == null)
			{
				cPlayerInitInfo = new iGameState.CPlayerInitInfo();
				cPlayerInitInfo.nUID = ltPlayerPo.nUID;
				m_GameState.SetNetPlayerInitInfo(cPlayerInitInfo);
			}
			cPlayerInitInfo.v3Pos = ltPlayerPo.v3Pos;
		}
		m_GameState.isNetGame = true;
		iGameApp.GetInstance().EnterScene(kGameSceneEnum.Game);
	}

	protected void OnGamePlayerInfoMsg(TNetUser sender, SFSObject data)
	{
		if (!sender.IsItMe)
		{
			nmsg_game_playerinfo nmsg_game_playerinfo2 = new nmsg_game_playerinfo();
			nmsg_game_playerinfo2.UnPack(data);
			iGameState.CPlayerInitInfo cPlayerInitInfo = new iGameState.CPlayerInitInfo();
			cPlayerInitInfo.nUID = sender.Id;
			cPlayerInitInfo.nCharID = nmsg_game_playerinfo2.nCharID;
			cPlayerInitInfo.nCharLevel = nmsg_game_playerinfo2.nCharLevel;
			m_GameState.SetNetPlayerInitInfo(cPlayerInitInfo);
		}
	}
}
