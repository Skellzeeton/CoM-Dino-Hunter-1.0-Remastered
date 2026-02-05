using System.Collections;
using LitJson;
using UnityEngine;

public class iServerSaveData : MonoBehaviour
{
	public delegate void OnEvent();

	public delegate void OnSuccessEvent();

	protected static iServerSaveData m_Instance;

	protected OnSuccessEvent m_OnSuccess;

	protected OnEvent m_OnFailed;

	public static iServerSaveData GetInstance()
	{
		if (m_Instance == null)
		{
			GameObject gameObject = new GameObject("_ServerSaveData");
			Object.DontDestroyOnLoad(gameObject);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			m_Instance = gameObject.AddComponent<iServerSaveData>();
		}
		return m_Instance;
	}

	private void Awake()
	{
		HttpClient.Instance().AddServer("DataServer", "http://192.168.0.190:8090/gameapi/gp.do", -1f, null);
	}

	private void Start()
	{
	}

	private void Update()
	{
		HttpClient.Instance().HandleResponse();
	}

	public void SendFetchSaveData(string userid, Hashtable data)
	{
		string text = JsonMapper.ToJson(data);
		Debug.Log(text);
		HttpClient.Instance().SendRequest("DataServer", "userHandler.loadProfile&json={\"userId\":\"" + userid + "\"}", text, "_ServerSaveData", "iServerSaveData", "OnFetchSaveDataResult", null);
	}

	protected void OnFetchSaveDataResult(int taskId, int result, string server, string action, string response, string param)
	{
		Debug.Log("OnFetchSaveDataResult " + action + " " + response);
		try
		{
			JsonData jsonData = JsonMapper.ToObject(response);
			if ((int)jsonData["code"] == 0)
			{
				string message = (string)jsonData["profile"];
				Debug.Log(message);
			}
		}
		catch
		{
		}
	}

	public void SendUploadSaveData(string userid, string data)
	{
		Debug.Log(data);
		HttpClient.Instance().SendRequest("DataServer", "userHandler.saveProfile&json={\"userId\":\"" + userid + "\"}", data, "_ServerSaveData", "iServerSaveData", "OnUploadSaveDataResult", null);
	}

	protected void OnUploadSaveDataResult(int taskId, int result, string server, string action, string response, string param)
	{
		Debug.Log("OnUploadSaveDataResult " + action + " " + response);
		try
		{
			JsonData jsonData = JsonMapper.ToObject(response);
			if ((int)jsonData["code"] != 0)
			{
			}
		}
		catch
		{
		}
	}
}
