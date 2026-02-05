using System.Collections.Generic;
using TNetSdk;
using UnityEngine;

public class CRoomManager
{
	public enum kState
	{
		None = 0,
		Search = 1,
		ApplyCreateRoom = 2,
		ApplyJoinRoom = 3,
		ApplyLeaveRoom = 4,
		Done = 5,
		GameConfirm = 6
	}

	protected class SearchRoomInfo
	{
		public int m_nGroupID;

		public float m_fTimeCount;

		public bool m_bExpire;

		public List<TNetRoom> m_ltRoom;
	}

	public class CRoomCharInfo
	{
		public TNetUser m_Player;

		public int m_nLevel;

		public int m_nBattleLevel;

		public int m_nCharID;

		public int m_nWeapon;
	}

	protected static CRoomManager m_Instance;

	protected iGameState m_GameState;

	protected iGameData m_GameData;

	protected kState m_State;

	protected bool m_bInRoom;

	protected List<int> m_ltGetRoomGroup;

	protected int m_nSelfGroupID;

	protected List<SearchRoomInfo> m_ltRoomManager;

	protected List<CBattleRegion> m_ltBattleRegion;

	protected int m_nBattleRegion;

	protected int m_nCurBattleRegion;

	protected int m_nMinBattleRegion;

	protected int m_nMaxBattleRegion;

	protected int m_nApplyGroupID;

	protected int m_nApplyRoomID;

	protected float m_fExpireTime;

	protected float m_fInRoomTime;

	protected float m_fInRoomTimeCount;

	protected CRoomCharInfo[] m_arrRoomChar;

	public kState State
	{
		get
		{
			return m_State;
		}
	}

	public CRoomManager()
	{
		m_GameState = iGameApp.GetInstance().m_GameState;
		m_GameData = iGameApp.GetInstance().m_GameData;
		m_State = kState.None;
		m_ltGetRoomGroup = new List<int>();
		m_ltRoomManager = new List<SearchRoomInfo>();
		iBattleGroupCenter battleGroupCenter = m_GameData.GetBattleGroupCenter();
		if (battleGroupCenter != null)
		{
			m_ltBattleRegion = battleGroupCenter.GetData();
		}
		m_fExpireTime = 2f;
		m_fInRoomTime = 2f;
		m_bInRoom = false;
		m_arrRoomChar = new CRoomCharInfo[1];
		for (int i = 0; i < m_arrRoomChar.Length; i++)
		{
			m_arrRoomChar[i] = new CRoomCharInfo();
		}
	}

	public static CRoomManager GetInstance()
	{
		if (m_Instance == null)
		{
			m_Instance = new CRoomManager();
		}
		return m_Instance;
	}

	public void Initialize()
	{
		TNetManager.GetInstance().AddUpdateFunc(Update);
		m_State = kState.Search;
		TNetObject netObject = TNetManager.GetInstance().NetObject;
		if (netObject != null)
		{
			netObject.AddEventListener(TNetEventRoom.GET_ROOM_LIST, OnRoomList);
			netObject.AddEventListener(TNetEventRoom.ROOM_CREATION, OnCreateRoom);
			netObject.AddEventListener(TNetEventRoom.ROOM_JOIN, OnJoinRoom);
			netObject.AddEventListener(TNetEventRoom.USER_ENTER_ROOM, OnUserEnterRoom);
			netObject.AddEventListener(TNetEventRoom.USER_EXIT_ROOM, OnUserExitRoom);
			netObject.AddEventListener(TNetEventRoom.USER_VARIABLES_UPDATE, OnUserVariableUpdate);
		}
	}

	public void Destroy()
	{
		TNetManager.GetInstance().RemoveUpdateFunc(Update);
		m_ltRoomManager.Clear();
		m_ltGetRoomGroup.Clear();
		for (int i = 0; i < m_arrRoomChar.Length; i++)
		{
			if (m_arrRoomChar[i] != null)
			{
				m_arrRoomChar[i].m_Player = null;
			}
		}
		TNetObject netObject = TNetManager.GetInstance().NetObject;
		if (netObject != null)
		{
			netObject.RemoveEventListener(TNetEventRoom.GET_ROOM_LIST, OnRoomList);
			netObject.RemoveEventListener(TNetEventRoom.ROOM_CREATION, OnCreateRoom);
			netObject.RemoveEventListener(TNetEventRoom.ROOM_JOIN, OnJoinRoom);
			netObject.RemoveEventListener(TNetEventRoom.USER_ENTER_ROOM, OnUserEnterRoom);
			netObject.RemoveEventListener(TNetEventRoom.USER_EXIT_ROOM, OnUserExitRoom);
			netObject.RemoveEventListener(TNetEventRoom.USER_VARIABLES_UPDATE, OnUserVariableUpdate);
		}
	}

	public void Update(float deltaTime)
	{
		if (m_State == kState.GameConfirm)
		{
			return;
		}
		if (m_bInRoom && m_State == kState.Done)
		{
			m_fInRoomTimeCount += deltaTime;
			if (m_fInRoomTimeCount < m_fInRoomTime)
			{
				return;
			}
			m_State = kState.Search;
		}
		foreach (SearchRoomInfo item in m_ltRoomManager)
		{
			if (item.m_bExpire)
			{
				continue;
			}
			item.m_fTimeCount += deltaTime;
			if (item.m_fTimeCount >= m_fExpireTime)
			{
				item.m_fTimeCount = 0f;
				item.m_bExpire = true;
				if (TNetManager.GetInstance().Connection != null)
				{
					m_ltGetRoomGroup.Add(item.m_nGroupID);
					GetRoomListRequest request = new GetRoomListRequest(item.m_nGroupID, 0, 10, RoomDragListCmd.ListType.not_full_not_game);
					TNetManager.GetInstance().Connection.Send(request);
				}
			}
		}
	}

	public void SearchRoom(float fBattleValue)
	{
		iGameApp.GetInstance().ScreenLog("battlelvl = " + fBattleValue);
		for (int i = 0; i < m_ltBattleRegion.Count; i++)
		{
			if (m_ltBattleRegion[i].IsMatch(fBattleValue))
			{
				m_nBattleRegion = i;
				m_nCurBattleRegion = m_nBattleRegion;
				m_nMinBattleRegion = m_nCurBattleRegion;
				m_nMaxBattleRegion = m_nCurBattleRegion;
				m_nSelfGroupID = m_ltBattleRegion[i].nGroupID;
				AddGroup(m_ltBattleRegion[i].nGroupID);
				break;
			}
		}
	}

	public void OnRoomList(TNetEventData tEvent)
	{
		ushort num = (ushort)tEvent.data["curPage"];
		ushort num2 = (ushort)tEvent.data["pageSum"];
		RoomDragListCmd.ListType listType = (RoomDragListCmd.ListType)(int)tEvent.data["roomListType"];
		List<TNetRoom> ltRoom = (List<TNetRoom>)tEvent.data["roomList"];
		Debug.Log("OnRoomList page = " + num + " pagenum = " + num2 + "  listtype = " + listType);
		if (m_ltGetRoomGroup.Count < 1)
		{
			return;
		}
		int nGroupID = m_ltGetRoomGroup[0];
		m_ltGetRoomGroup.RemoveAt(0);
		foreach (SearchRoomInfo item in m_ltRoomManager)
		{
			if (item.m_nGroupID == nGroupID)
			{
				item.m_bExpire = false;
				item.m_fTimeCount = 0f;
				item.m_ltRoom = ltRoom;
				break;
			}
		}
		int nRoomID = 0;
		if (GetCanJoinRoom(ref nGroupID, ref nRoomID))
		{
			ApplyJoinRoom(nGroupID, nRoomID);
		}
		else if (IsAllEmpty())
		{
			if (m_nSelfGroupID == nGroupID)
			{
				ApplyCreateRoom();
			}
			if (m_State == kState.Search)
			{
				BiggerRegion();
			}
		}
	}

	public void OnCreateRoom(TNetEventData tEvent)
	{
		Debug.Log("OnCreateRoom");
		if ((int)tEvent.data["result"] != 0)
		{
			Debug.LogError("cant create room!");
			ApplyCreateRoomFailed();
		}
		else
		{
			ushort nRoomID = (ushort)tEvent.data["roomId"];
			ApplyCreateRoomSuccess(nRoomID);
		}
	}

	public void OnJoinRoom(TNetEventData tEvent)
	{
		RoomJoinResCmd.Result result = (RoomJoinResCmd.Result)(int)tEvent.data["result"];
		Debug.Log("OnJoinRoom result = " + result);
		if (result != 0)
		{
			ApplyJoinRoomFailed();
		}
		else
		{
			ApplyJoinRoomSuccess();
		}
	}

	public void OnUserEnterRoom(TNetEventData tEvent)
	{
		Debug.Log("OnUserEnterRoom");
		TNetUser tNetUser = (TNetUser)tEvent.data["user"];
		if (tNetUser != null)
		{
			iGameApp.GetInstance().ScreenLog("cur room players = " + TNetManager.GetInstance().Connection.CurRoom.UserCount + " max = " + TNetManager.GetInstance().Connection.CurRoom.MaxUsers);
			if (TNetManager.GetInstance().Connection.CurRoom.UserCount >= TNetManager.GetInstance().Connection.CurRoom.MaxUsers)
			{
				m_State = kState.GameConfirm;
			}
			m_fInRoomTimeCount = 0f - (float)TNetManager.GetInstance().Connection.CurRoom.UserCount * 2f;
			CRoomCharInfo freeRoomCharInfo = GetFreeRoomCharInfo();
			if (freeRoomCharInfo != null)
			{
				freeRoomCharInfo.m_Player = tNetUser;
			}
		}
	}

	public void OnUserExitRoom(TNetEventData tEvent)
	{
		Debug.Log("OnUserExitRoom");
		TNetUser tNetUser = (TNetUser)tEvent.data["user"];
		if (tNetUser.IsItMe)
		{
			ApplyLeaveRoomResult();
			int nGroupID = 0;
			int nRoomID = 0;
			if (GetCanJoinRoom(ref nGroupID, ref nRoomID))
			{
				ApplyJoinRoom(nGroupID, nRoomID);
			}
			else if (IsAllEmpty())
			{
				BiggerRegion();
			}
			ClearRoomCharInfo();
		}
		else
		{
			iGameApp.GetInstance().ScreenLog("cur room players = " + TNetManager.GetInstance().Connection.CurRoom.UserCount);
			if (TNetManager.GetInstance().Connection.CurRoom.UserCount < TNetManager.GetInstance().Connection.CurRoom.MaxUsers)
			{
				m_State = kState.Done;
				m_fInRoomTimeCount = 0f;
			}
			CRoomCharInfo roomCharInfo = GetRoomCharInfo(tNetUser.Id);
			if (roomCharInfo != null)
			{
				roomCharInfo.m_Player = null;
			}
		}
	}

	public void OnUserVariableUpdate(TNetEventData tEvent)
	{
		TNetUser tNetUser = (TNetUser)tEvent.data["user"];
		Debug.Log("OnUserVariableUpdate " + tNetUser.Id);
		if (!tNetUser.IsItMe)
		{
			SFSObject variable = tNetUser.GetVariable(TNetUserVarType.PlayerSetting);
			CRoomCharInfo roomCharInfo = GetRoomCharInfo(tNetUser.Id);
			if (roomCharInfo != null)
			{
				roomCharInfo.m_nCharID = variable.GetInt("charid");
				roomCharInfo.m_nBattleLevel = variable.GetInt("battlelevel");
			}
		}
	}

	protected void BiggerRegion()
	{
		if (m_nMinBattleRegion != 0 || m_nMaxBattleRegion != m_ltBattleRegion.Count - 1)
		{
			iGameApp.GetInstance().ScreenLog("????????????");
			if (m_nMinBattleRegion == 0)
			{
				m_nMaxBattleRegion++;
				m_nCurBattleRegion = m_nMaxBattleRegion;
				AddGroup(m_ltBattleRegion[m_nCurBattleRegion].nGroupID);
			}
			else if (m_nMaxBattleRegion == m_ltBattleRegion.Count - 1)
			{
				m_nMinBattleRegion--;
				m_nCurBattleRegion = m_nMinBattleRegion;
				AddGroup(m_ltBattleRegion[m_nCurBattleRegion].nGroupID);
			}
			else if (m_nMinBattleRegion == m_nMaxBattleRegion || m_nCurBattleRegion == m_nMaxBattleRegion)
			{
				m_nMinBattleRegion--;
				m_nCurBattleRegion = m_nMinBattleRegion;
				AddGroup(m_ltBattleRegion[m_nCurBattleRegion].nGroupID);
			}
			else if (m_nCurBattleRegion == m_nMinBattleRegion)
			{
				m_nMaxBattleRegion++;
				m_nCurBattleRegion = m_nMaxBattleRegion;
				AddGroup(m_ltBattleRegion[m_nCurBattleRegion].nGroupID);
			}
		}
	}

	protected bool IsGroupExist(int nGroupID)
	{
		foreach (SearchRoomInfo item in m_ltRoomManager)
		{
			if (item.m_nGroupID == nGroupID)
			{
				return true;
			}
		}
		return false;
	}

	protected void AddGroup(int nGroupID)
	{
		if (IsGroupExist(nGroupID))
		{
			return;
		}
		SearchRoomInfo searchRoomInfo = new SearchRoomInfo();
		searchRoomInfo.m_nGroupID = nGroupID;
		searchRoomInfo.m_bExpire = false;
		searchRoomInfo.m_fTimeCount = 0f;
		searchRoomInfo.m_ltRoom = null;
		m_ltRoomManager.Add(searchRoomInfo);
		string text = string.Empty;
		foreach (SearchRoomInfo item in m_ltRoomManager)
		{
			text = text + " " + item.m_nGroupID;
		}
		iGameApp.GetInstance().ScreenLog("????????: " + text);
	}

	protected void ClearGroup()
	{
		m_ltRoomManager.Clear();
	}

	protected bool IsAllEmpty()
	{
		foreach (SearchRoomInfo item in m_ltRoomManager)
		{
			if (item.m_bExpire)
			{
				return false;
			}
			if (item.m_ltRoom != null && item.m_ltRoom.Count > 0)
			{
				return false;
			}
		}
		return true;
	}

	protected bool GetCanJoinRoom(ref int nGroupID, ref int nRoomID)
	{
		foreach (SearchRoomInfo item in m_ltRoomManager)
		{
			if (item.m_bExpire || item.m_ltRoom == null || item.m_ltRoom.Count < 1)
			{
				continue;
			}
			if (TNetManager.GetInstance().Connection != null && TNetManager.GetInstance().Connection.CurRoom != null)
			{
				foreach (TNetRoom item2 in item.m_ltRoom)
				{
					if (TNetManager.GetInstance().Connection.CurRoom.Id == item2.Id)
					{
						item.m_ltRoom.Remove(item2);
						break;
					}
				}
			}
			if (item.m_ltRoom.Count < 1)
			{
				continue;
			}
			nGroupID = item.m_nGroupID;
			nRoomID = item.m_ltRoom[0].Id;
			return true;
		}
		return false;
	}

	protected void ApplyCreateRoom()
	{
		if (!m_bInRoom)
		{
			iGameApp.GetInstance().ScreenLog("ApplyCreateRoom");
			m_State = kState.ApplyCreateRoom;
			TNetManager.GetInstance().CreateRoom(m_GameState.UserName + "'s Room", string.Empty, m_nSelfGroupID, 2, RoomCreateCmd.RoomType.limit, RoomCreateCmd.RoomSwitchMasterType.Auto, string.Empty);
		}
	}

	protected void ApplyCreateRoomSuccess(int nRoomID)
	{
		if (m_State == kState.ApplyCreateRoom)
		{
			iGameApp.GetInstance().ScreenLog("ApplyCreateRoomSuccess " + nRoomID);
			m_State = kState.Search;
			ApplyJoinRoom(m_nSelfGroupID, nRoomID);
		}
	}

	protected void ApplyCreateRoomFailed()
	{
		if (m_State == kState.ApplyCreateRoom)
		{
			iGameApp.GetInstance().ScreenLog("ApplyCreateRoomFailed");
			m_State = kState.Search;
			BiggerRegion();
		}
	}

	protected void ApplyJoinRoom(int nGroupID, int nRoomID)
	{
		if (m_State == kState.Search && TNetManager.GetInstance().Connection != null)
		{
			if (TNetManager.GetInstance().Connection.CurRoom != null)
			{
				ApplyLeaveRoom();
				return;
			}
			TNetManager.GetInstance().JoinRoom(nRoomID, string.Empty);
			m_State = kState.ApplyJoinRoom;
			m_nApplyGroupID = nGroupID;
			m_nApplyRoomID = nRoomID;
			iGameApp.GetInstance().ScreenLog("ApplyJoinRoom " + nGroupID + " " + nRoomID);
		}
	}

	protected void ApplyJoinRoomSuccess()
	{
		iGameApp.GetInstance().ScreenLog("ApplyJoinRoomSuccess");
		m_State = kState.Done;
		m_fInRoomTimeCount = 0f;
		m_bInRoom = true;
	}

	protected void ApplyJoinRoomFailed()
	{
		iGameApp.GetInstance().ScreenLog("ApplyJoinRoomFailed");
		foreach (SearchRoomInfo item in m_ltRoomManager)
		{
			if (item.m_nGroupID != m_nApplyGroupID)
			{
				continue;
			}
			foreach (TNetRoom item2 in item.m_ltRoom)
			{
				if (item2.Id != m_nApplyRoomID)
				{
					continue;
				}
				item.m_ltRoom.Remove(item2);
				break;
			}
			break;
		}
		m_State = kState.Search;
		int nGroupID = 0;
		int nRoomID = 0;
		if (GetCanJoinRoom(ref nGroupID, ref nRoomID))
		{
			ApplyJoinRoom(nGroupID, nRoomID);
		}
		else if (IsAllEmpty())
		{
			BiggerRegion();
		}
	}

	protected void ApplyLeaveRoom()
	{
		if (TNetManager.GetInstance().Connection != null)
		{
			TNetManager.GetInstance().LeaveRoom();
			m_State = kState.ApplyLeaveRoom;
			iGameApp.GetInstance().ScreenLog("AppyLeaveRoom");
		}
	}

	protected void ApplyLeaveRoomResult()
	{
		if (m_State == kState.ApplyLeaveRoom)
		{
			m_bInRoom = false;
			m_State = kState.Search;
			iGameApp.GetInstance().ScreenLog("AppyLeaveRoomResult");
		}
	}

	protected CRoomCharInfo GetFreeRoomCharInfo()
	{
		for (int i = 0; i < m_arrRoomChar.Length; i++)
		{
			if (m_arrRoomChar[i].m_Player == null)
			{
				return m_arrRoomChar[i];
			}
		}
		return null;
	}

	public CRoomCharInfo[] GetRoomCharInfoArr()
	{
		return m_arrRoomChar;
	}

	protected CRoomCharInfo GetRoomCharInfo(int nID)
	{
		for (int i = 0; i < m_arrRoomChar.Length; i++)
		{
			if (m_arrRoomChar[i].m_Player != null && m_arrRoomChar[i].m_Player.Id == nID)
			{
				return m_arrRoomChar[i];
			}
		}
		return null;
	}

	protected void ClearRoomCharInfo()
	{
		for (int i = 0; i < m_arrRoomChar.Length; i++)
		{
			m_arrRoomChar[i].m_Player = null;
		}
	}

	public void SendUserVariable(CRoomCharInfo info)
	{
		if (TNetManager.GetInstance().Connection != null)
		{
			SFSObject sFSObject = new SFSObject();
			sFSObject.PutInt("charid", info.m_nCharID);
			sFSObject.PutInt("level", info.m_nLevel);
			sFSObject.PutInt("battlelevel", info.m_nBattleLevel);
			SetUserVariableRequest request = new SetUserVariableRequest(TNetUserVarType.PlayerSetting, sFSObject);
			TNetManager.GetInstance().Connection.Send(request);
		}
	}
}
