using System;
using System.Collections.Generic;
using TNetSdk;
using UnityEngine;

public class TNetManager : MonoBehaviour
{
	public delegate void UpdateFunc(float deltaTime);

	public delegate void OnAcceptMsgFunc(TNetUser netuser, kGameNetEnum nmsg, SFSObject data);

	protected static TNetManager m_Instance;

	protected TNetObject m_Connection;

	protected bool m_bLogin;

	protected bool m_bGaming;

	protected OnAcceptMsgFunc m_Func;

	protected UpdateFunc m_UpdateFunc;

	public TNetObject Connection
	{
		get
		{
			if (m_Connection == null || !m_Connection.IsContected())
			{
				return null;
			}
			return m_Connection;
		}
	}

	public TNetObject NetObject
	{
		get
		{
			return m_Connection;
		}
	}

	public bool isGaming
	{
		get
		{
			return m_bGaming;
		}
		set
		{
			m_bGaming = value;
		}
	}

	public bool isLogin
	{
		get
		{
			return m_bLogin;
		}
		set
		{
			m_bLogin = value;
		}
	}

	public static TNetManager GetInstance()
	{
		if (m_Instance == null)
		{
			GameObject gameObject = new GameObject("TNetManager");
			if (gameObject == null)
			{
				return null;
			}
			m_Instance = gameObject.AddComponent<TNetManager>();
		}
		return m_Instance;
	}

	private void Awake()
	{
		m_Connection = null;
		m_bLogin = false;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Update()
	{
		if (m_UpdateFunc != null)
		{
			m_UpdateFunc(Time.deltaTime);
		}
	}

	private void FixedUpdate()
	{
		UpdateNetManager(Time.deltaTime);
	}

	public void AddAcceptMsgFunc(OnAcceptMsgFunc func)
	{
		Debug.Log("AddAcceptMsgFunc " + func.ToString());
		m_Func = (OnAcceptMsgFunc)Delegate.Combine(m_Func, func);
	}

	public void DelAcceptMsgFunc(OnAcceptMsgFunc func)
	{
		Debug.Log("DelAcceptMsgFunc " + func.ToString());
		m_Func = (OnAcceptMsgFunc)Delegate.Remove(m_Func, func);
	}

	public void ClearBroadcastMsgFunc()
	{
		m_Func = null;
	}

	public void AddUpdateFunc(UpdateFunc func)
	{
		m_UpdateFunc = (UpdateFunc)Delegate.Combine(m_UpdateFunc, func);
	}

	public void RemoveUpdateFunc(UpdateFunc func)
	{
		m_UpdateFunc = (UpdateFunc)Delegate.Remove(m_UpdateFunc, func);
	}

	public void ClearUpdateFunc(UpdateFunc func)
	{
		m_UpdateFunc = null;
	}

	public void OnConnectSuccess(TNetEventData tEvent)
	{
	}

	public void OnConnectKilled(TNetEventData tEvent)
	{
		Clear();
	}

	public void OnConnectTimeout(TNetEventData tEvent)
	{
	}

	public void OnDisConnect(TNetEventData tEvent)
	{
		Clear();
	}

	public void OnLoginSuccess(TNetEventData tEvent)
	{
		m_bLogin = true;
	}

	public void OnLoginFailed(TNetEventData tEvent)
	{
		m_bLogin = false;
	}

	public void OnHeartTimeout(TNetEventData tEvent)
	{
	}

	public void OnHeartWaiting(TNetEventData tEvent)
	{
	}

	public void OnHearRenew(TNetEventData tEvent)
	{
	}

	public void OnHearBeat(TNetEventData tEvent)
	{
	}

	public void OnRoomList(TNetEventData tEvent)
	{
		ushort num = (ushort)tEvent.data["curPage"];
		ushort num2 = (ushort)tEvent.data["pageSum"];
		RoomDragListCmd.ListType listType = (RoomDragListCmd.ListType)(int)tEvent.data["roomListType"];
		List<TNetRoom> list = (List<TNetRoom>)tEvent.data["roomList"];
	}

	public void OnJoinRoom(TNetEventData tEvent)
	{
		Debug.Log("OnJoinRoom");
		RoomJoinResCmd.Result result = (RoomJoinResCmd.Result)(int)tEvent.data["result"];
		if (result != 0)
		{
			Debug.LogError("OnJoinRoom failed result = " + result);
		}
		else
		{
			TNetRoom tNetRoom = (TNetRoom)tEvent.data["room"];
		}
	}

	public void OnUserEnterRoom(TNetEventData tEvent)
	{
		Debug.Log("OnUserEnterRoom");
		TNetUser tNetUser = (TNetUser)tEvent.data["user"];
		if (tNetUser != null)
		{
		}
	}

	public void OnUserExitRoom(TNetEventData tEvent)
	{
		Debug.Log("OnUserExitRoom");
		TNetUser netuser = (TNetUser)tEvent.data["user"];
		if (m_Func != null)
		{
			m_Func(netuser, kGameNetEnum.PLAYER_LEAVEROOM, new SFSObject());
		}
	}

	public void OnCreateRoom(TNetEventData tEvent)
	{
		Debug.Log("OnCreateRoom");
		if ((int)tEvent.data["result"] != 0)
		{
			Debug.LogError("cant create room!");
		}
		else
		{
			ushort num = (ushort)tEvent.data["roomId"];
		}
	}

	public void OnLeaveRoom(TNetEventData tEvent)
	{
	}

	public void OnAcceptMsg(TNetEventData tEvent)
	{
		TNetUser netuser = (TNetUser)tEvent.data["user"];
		SFSObject sFSObject = (SFSObject)tEvent.data["message"];
		kGameNetEnum @int = (kGameNetEnum)sFSObject.GetInt("msghead");
		SFSObject data = (SFSObject)sFSObject.GetSFSObject("data");
		if (m_Func != null)
		{
			m_Func(netuser, @int, data);
		}
	}

	public void UpdateNetManager(float deltaTime)
	{
		if (m_Connection != null)
		{
			m_Connection.Update(deltaTime);
		}
	}

	public void Clear()
	{
		m_Connection.RemoveAllEventListeners();
	}

	public void Connect(string sIP, int nPort)
	{
		if (m_Connection == null)
		{
			m_Connection = new TNetObject();
		}
		if (m_Connection.IsContected())
		{
			Debug.Log("connect is alreay worked");
			return;
		}
		m_Connection.AddEventListener(TNetEventSystem.CONNECTION, OnConnectSuccess);
		m_Connection.AddEventListener(TNetEventSystem.CONNECTION_KILLED, OnConnectKilled);
		m_Connection.AddEventListener(TNetEventSystem.CONNECTION_TIMEOUT, OnConnectTimeout);
		m_Connection.AddEventListener(TNetEventSystem.DISCONNECT, OnDisConnect);
		m_Connection.AddEventListener(TNetEventSystem.LOGIN, OnLoginSuccess);
		m_Connection.AddEventListener(TNetEventSystem.REVERSE_HEART_TIMEOUT, OnHeartTimeout);
		m_Connection.AddEventListener(TNetEventSystem.REVERSE_HEART_WAITING, OnHeartWaiting);
		m_Connection.AddEventListener(TNetEventSystem.REVERSE_HEART_RENEW, OnHearRenew);
		m_Connection.AddEventListener(TNetEventSystem.HEARTBEAT, OnHearBeat);
		m_Connection.AddEventListener(TNetEventRoom.GET_ROOM_LIST, OnRoomList);
		m_Connection.AddEventListener(TNetEventRoom.ROOM_CREATION, OnCreateRoom);
		m_Connection.AddEventListener(TNetEventRoom.ROOM_JOIN, OnJoinRoom);
		m_Connection.AddEventListener(TNetEventRoom.USER_ENTER_ROOM, OnUserEnterRoom);
		m_Connection.AddEventListener(TNetEventRoom.USER_EXIT_ROOM, OnUserExitRoom);
		m_Connection.AddEventListener(TNetEventRoom.OBJECT_MESSAGE, OnAcceptMsg);
		Debug.Log("Connect " + sIP + " port " + nPort);
		m_Connection.Connect(sIP, nPort);
	}

	public void DisConnect()
	{
		if (m_Connection != null && m_Connection.IsContected())
		{
			m_Connection.Close();
			m_bLogin = false;
		}
	}

	public void Login(string username, string password = "")
	{
		if (!m_bLogin && m_Connection != null)
		{
			m_Connection.Send(new LoginRequest(username));
		}
	}

	public void ApplyRoomList(int nGroupID, int nPage, int nPageNum, RoomDragListCmd.ListType ListType)
	{
		if (Connection != null)
		{
			GetRoomListRequest request = new GetRoomListRequest(nGroupID, nPage, nPageNum, ListType);
			Connection.Send(request);
		}
	}

	public void JoinRoom(int nRoomID, string password = "")
	{
		if (Connection != null)
		{
			JoinRoomRequest request = new JoinRoomRequest(nRoomID, password);
			Connection.Send(request);
		}
	}

	public void CreateRoom(string sRoomName, string password, int nGroupID, int nMaxPlayer, RoomCreateCmd.RoomType roomlimit, RoomCreateCmd.RoomSwitchMasterType mastertype, string comment = "")
	{
		if (Connection != null)
		{
			CreateRoomRequest request = new CreateRoomRequest(sRoomName, password, nGroupID, nMaxPlayer, roomlimit, mastertype, comment);
			Connection.Send(request);
		}
	}

	public void LeaveRoom()
	{
		if (Connection != null)
		{
			LeaveRoomRequest request = new LeaveRoomRequest();
			Connection.Send(request);
		}
	}

	public void BroadcastMessage(nmsg_struct msg)
	{
		if (Connection != null)
		{
			SFSObject sFSObject = new SFSObject();
			sFSObject.PutInt("msghead", (int)msg.msghead);
			sFSObject.PutSFSObject("data", msg.Pack());
			BroadcastMessageRequest request = new BroadcastMessageRequest(sFSObject);
			Connection.Send(request);
		}
	}

	public void SendMessage(int nUserID, nmsg_struct msg)
	{
		if (Connection != null)
		{
			SFSObject sFSObject = new SFSObject();
			sFSObject.PutInt("msghead", (int)msg.msghead);
			sFSObject.PutSFSObject("data", msg.Pack());
			ObjectMessageRequest request = new ObjectMessageRequest(nUserID, sFSObject);
			Connection.Send(request);
		}
	}

	public void SendMessage(TNetUser user, nmsg_struct msg)
	{
		SendMessage(user.Id, msg);
	}

	public void OnApplicationQuit()
	{
		DisConnect();
	}
}
