using System;
using System.Collections;
using System.IO;
using System.Xml;
using UnityEngine;

public class iServerVerify : MonoBehaviour
{
	public class CServerInfo
	{
		public string m_sVersion;

		public CServerInfo()
		{
			Clear();
		}

		public void Clear()
		{
			m_sVersion = string.Empty;
		}
	}

	protected enum kPingState
	{
		None = 0,
		Delay = 1,
		Pinging = 2,
		Success = 3,
		Fail = 4
	}

	public delegate void OnEvent();

	//protected static iServerVerify m_Instance;

	protected OnEvent m_OnSuccess;

	protected OnEvent m_OnFailed;

	protected OnEvent m_OnNetError;

	protected CServerInfo m_ServerInfo;

	protected kPingState m_PingState;

	protected float m_fTimeOut;

	protected float m_fTimeOutCount;

	protected float m_fTimeDelay;

	protected float m_fTimeDelayCount;

	protected string m_sUrl = "http://www.apple.com";

	protected string m_sVersion = "1.0.1";

	protected string m_sServerInfoKey = "trinitigame_comdh";

	public string Version
	{
		get
		{
			return m_sVersion;
		}
		set
		{
			m_sVersion = value;
		}
	}

	/*public static iServerVerify GetInstance()
	{
		if (m_Instance == null)
		{
			GameObject gameObject = new GameObject("_ServerVerify");
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			m_Instance = gameObject.AddComponent<iServerVerify>();
		}
		return m_Instance;
	}*/

	public bool IsSuccess()
	{
		return m_PingState == kPingState.Success;
	}

	public bool IsFailed()
	{
		return m_PingState == kPingState.Fail;
	}

	private void Awake()
	{
		m_ServerInfo = new CServerInfo();
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (m_PingState == kPingState.Delay)
		{
			m_fTimeDelayCount += Time.deltaTime;
			if (m_fTimeDelayCount >= m_fTimeDelay)
			{
				m_fTimeDelayCount = 0f;
				StartCoroutine(Connect());
			}
		}
		else
		{
			if (m_PingState != kPingState.Pinging)
			{
				return;
			}
			m_fTimeOutCount += Time.deltaTime;
			if (m_fTimeOutCount >= m_fTimeOut)
			{
				m_fTimeOutCount = 0f;
				Debug.Log("test ping time out ");
				m_PingState = kPingState.Fail;
				if (m_OnNetError != null)
				{
					m_OnNetError();
				}
			}
		}
	}

	public void ConnectServer(string sVersion, float timeout = 10f, float delaytime = 0f)
	{
		m_sVersion = sVersion;
		m_fTimeOut = timeout;
		m_fTimeOutCount = 0f;
		if (delaytime <= 0f)
		{
			StartCoroutine(Connect());
			return;
		}
		m_PingState = kPingState.Delay;
		m_fTimeDelay = delaytime;
		m_fTimeDelayCount = 0f;
	}

	public void SetSuccessFunc(OnEvent func)
	{
		m_OnSuccess = func;
	}

	public void SetFailedFunc(OnEvent func)
	{
		m_OnFailed = func;
	}

	public void SetNetErrorFunc(OnEvent func)
	{
		m_OnNetError = func;
	}

	protected IEnumerator Connect()
	{
		m_PingState = kPingState.Pinging;
		WWW www = new WWW(m_sUrl + "?rand=" + UnityEngine.Random.Range(10, 99999));
		Debug.Log(www.url);
		yield return www;
		if (m_PingState != kPingState.Pinging)
		{
			yield return 0;
		}
		if (www.error != null)
		{
			Debug.Log("test ping failed " + www.error);
			m_PingState = kPingState.Fail;
			if (m_OnFailed != null)
			{
				m_OnFailed();
			}
		}
		else
		{
			Debug.Log("test ping successed ");
			m_PingState = kPingState.Success;
			if (m_OnSuccess != null)
			{
				m_OnSuccess();
			}
		}
	}

	protected void LoadServerData(string input)
	{
		Debug.Log("LoadServerData " + input);
		m_ServerInfo.Clear();
		string empty = string.Empty;
		try
		{
			empty = XXTEAUtils.Decrypt(input, m_sServerInfoKey);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(empty);
			XmlElement documentElement = xmlDocument.DocumentElement;
			if (documentElement.Attributes["version"] != null)
			{
				m_ServerInfo.m_sVersion = documentElement.Attributes["version"].Value;
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("LoadServerData Error " + ex);
		}
	}

	protected void TransformXML2TXT(string srcpath, string dstpath, string key)
	{
		if (srcpath.Length < 1 || dstpath.Length < 1)
		{
			return;
		}
		string text = string.Empty;
		Debug.Log(srcpath);
		if (File.Exists(srcpath))
		{
			StreamReader streamReader = null;
			try
			{
				streamReader = new StreamReader(srcpath);
				text = streamReader.ReadToEnd();
			}
			catch
			{
				Debug.Log("ERROR - Encrypt()!!!");
			}
			finally
			{
				if (streamReader != null)
				{
					streamReader.Close();
				}
			}
		}
		if (text != null && text.Length > 0)
		{
			string value = XXTEAUtils.Encrypt(text, key);
			StreamWriter streamWriter = new StreamWriter(dstpath, false);
			streamWriter.Write(value);
			streamWriter.Flush();
			streamWriter.Close();
		}
	}
}
