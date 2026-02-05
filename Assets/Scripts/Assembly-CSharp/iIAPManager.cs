using System.Collections;
using LitJson;
using UnityEngine;
using gyIAPSystem;

public class iIAPManager : MonoBehaviour
{
	protected enum kPingState
	{
		None = 0,
		Pinging = 1,
		Success = 2,
		Fail = 3
	}

	protected enum kPurchaseState
	{
		None = 0,
		Ping = 1,
		Purchase = 2,
		ServerVerify = 3,
		ServerVerifyRequest = 4
	}

	public delegate void OnEvent();

	protected static iIAPManager m_Instance;

	protected OnEvent m_OnSuccess;

	protected OnEvent m_OnFailed;

	protected OnEvent m_OnCancel;

	protected OnEvent m_OnNetError;

	protected OnEvent m_OnSendVerify;

	protected OnEvent m_OnVerifyFailed;

	protected CIAPCenter m_IAPCenter;

	protected int m_nCurPurchase;

	protected string m_sCurTID;

	protected string m_sCurReceipt;

	protected string m_sRandom;

	protected int m_nRat;

	protected int m_nRatA;

	protected int m_nRatB;

	protected kPingState m_PingState;

	protected kPurchaseState m_PurchaseState;

	protected float m_fVerifyTime = 1f;

	protected float m_fVerifyTimeCount;

	public static iIAPManager GetInstance()
	{
		if (m_Instance == null)
		{
			GameObject gameObject = new GameObject("_IAPManager");
			gameObject.transform.position = Vector3.zero;
			gameObject.transform.rotation = Quaternion.identity;
			Object.DontDestroyOnLoad(gameObject);
			m_Instance = gameObject.AddComponent<iIAPManager>();
		}
		return m_Instance;
	}

	private void Awake()
	{
		m_IAPCenter = new CIAPCenter();
		m_IAPCenter.Load();
		m_IAPCenter.LoadCrystal2Gold();
		m_nCurPurchase = -1;
		m_PingState = kPingState.None;
		m_PurchaseState = kPurchaseState.None;
		HttpClient.Instance().AddServer("IAPServer", "http://iap.trinitigame.com:7600/gameapi/GameCommon.do", -1f, "abcd@@##980[]L>.");
	}

	private void Start()
	{
	}

	private void Update()
	{
		HttpClient.Instance().HandleResponse();
		Update(Time.deltaTime);
	}

	protected void Update(float deltaTime)
	{
		if (m_PurchaseState == kPurchaseState.None)
		{
			return;
		}
		if (m_PurchaseState == kPurchaseState.Ping)
		{
			if (m_PingState == kPingState.Pinging)
			{
				return;
			}
			if (m_PingState == kPingState.Success)
			{
				m_PingState = kPingState.None;
				CIAPInfo cIAPInfo = m_IAPCenter.Get(m_nCurPurchase);
				if (cIAPInfo != null)
				{
					IAPPlugin.NowPurchaseProduct(cIAPInfo.sKey, "1");
					m_PurchaseState = kPurchaseState.Purchase;
				}
			}
			else if (m_PingState == kPingState.Fail)
			{
				m_PingState = kPingState.None;
				m_PurchaseState = kPurchaseState.None;
				OnPurchaseNetError();
			}
		}
		else if (m_PurchaseState == kPurchaseState.Purchase)
		{
			int purchaseStatus = IAPPlugin.GetPurchaseStatus();
			if (purchaseStatus != 0)
			{
				if (purchaseStatus == 1)
				{
					OnPurchaseSuccess(m_nCurPurchase);
				}
				else if (purchaseStatus == -2)
				{
					m_PurchaseState = kPurchaseState.None;
					OnPurchaseCancel();
				}
				else if (purchaseStatus < 0)
				{
					m_PurchaseState = kPurchaseState.None;
					OnPurchaseFailed(m_nCurPurchase);
				}
			}
		}
		else if (m_PurchaseState == kPurchaseState.ServerVerifyRequest)
		{
			m_fVerifyTimeCount += deltaTime;
			if (!(m_fVerifyTimeCount < m_fVerifyTime))
			{
				m_fVerifyTimeCount = 0f;
				SendPurchaseVerify(m_sCurTID, m_sCurReceipt);
			}
		}
	}

	protected void SendPurchaseRequest(string iapkey, string tid, string receipt)
	{
		Debug.Log("SendPurchaseRequest " + iapkey + " " + tid + " " + receipt);
		Hashtable hashtable = new Hashtable();
		hashtable["cmd"] = "purchase/UserPurchaseBuy";
		hashtable["aid"] = iMacroDefine.BundleID;
		hashtable["uuid"] = MiscPlugin.GetMacAddr();
		hashtable["pid"] = iapkey;
		m_sCurTID = tid;
		hashtable["tid"] = m_sCurTID;
		hashtable["receipt"] = receipt;
		m_sRandom = Random.Range(1, 10).ToString();
		hashtable["rand"] = m_sRandom;
		m_nRat = Random.Range(1, 10);
		hashtable["rat"] = m_nRat;
		string text = JsonMapper.ToJson(hashtable);
		Debug.Log(text);
		HttpClient.Instance().SendRequest("IAPServer", "groovy", text, "_IAPManager", "iIAPManager", "OnPurchaseRequest", null);
	}

	protected void OnPurchaseRequest(int taskId, int result, string server, string action, string response, string param)
	{
		if (m_PurchaseState != kPurchaseState.ServerVerify)
		{
			return;
		}
		try
		{
			JsonData jsonData = JsonMapper.ToObject(response);
			if ((int)jsonData["code"] != 0)
			{
				m_PurchaseState = kPurchaseState.None;
				OnPurchaseFailed(-1);
				return;
			}
			m_nRatA = (int)jsonData["rata"];
			m_nRatB = (int)jsonData["ratb"];
			m_PurchaseState = kPurchaseState.ServerVerifyRequest;
			m_fVerifyTimeCount = 0f;
			SendPurchaseVerify(m_sCurTID, m_sCurReceipt);
		}
		catch
		{
			Debug.Log("OnPurchaseRequest error " + action + " " + response);
			m_PurchaseState = kPurchaseState.None;
			OnPurchaseFailed(-1);
		}
	}

	protected void SendPurchaseVerify(string tid, string random)
	{
		Debug.Log("SendPurchaseVerify " + tid + " " + random);
		Hashtable hashtable = new Hashtable();
		hashtable["cmd"] = "purchase/GetPurchaseVerify";
		hashtable["transactionId"] = m_sCurTID;
		hashtable["randPara"] = m_sRandom;
		string text = JsonMapper.ToJson(hashtable);
		Debug.Log(text);
		HttpClient.Instance().SendRequest("IAPServer", "groovy", text, "_IAPManager", "iIAPManager", "OnPurcahseVerify", null);
	}

	protected void OnPurcahseVerify(int taskId, int result, string server, string action, string response, string param)
	{
		iGameData gameData = iGameApp.GetInstance().m_GameData;
		if (gameData == null)
		{
			return;
		}
		iDataCenter dataCenter = gameData.GetDataCenter();
		if (dataCenter == null || m_PurchaseState != kPurchaseState.ServerVerifyRequest)
		{
			return;
		}
		Debug.Log("OnPurcahseVerify result = " + result);
		try
		{
			JsonData jsonData = JsonMapper.ToObject(response);
			int num = (int)jsonData["code"];
			Debug.Log("OnPurcahseVerify code = " + num);
			if (num != 0)
			{
				m_PurchaseState = kPurchaseState.None;
				OnPurchaseFailed(-1);
				return;
			}
			int num2 = (int)jsonData["sta"];
			Debug.Log("OnPurcahseVerify sta = " + num2);
			if (num2 == -1)
			{
				return;
			}
			m_PurchaseState = kPurchaseState.None;
			if (num2 == 0)
			{
				string key = (string)jsonData["pid"];
				int num3 = (int)jsonData["ratresult"];
				string text = (string)jsonData["aid"];
				if (text != iMacroDefine.BundleID)
				{
					return;
				}
				Debug.Log("ratersult = " + num3);
				if (num3 != m_nRat * m_nRatA / 9 + m_nRatB - 3)
				{
					return;
				}
				CIAPInfo byKey = m_IAPCenter.GetByKey(key);
				if (byKey != null)
				{
					if (byKey.isCrystal)
					{
						dataCenter.AddCrystal(byKey.nValue);
					}
					else
					{
						dataCenter.AddGold(byKey.nValue);
					}
					dataCenter.Save();
					//iGameApp.GetInstance().Flurry_PurchaseIAP(byKey.nID);
					if (m_OnSuccess != null)
					{
						m_OnSuccess();
					}
				}
			}
			else
			{
				m_PurchaseState = kPurchaseState.None;
				OnVerifyFailed();
			}
		}
		catch
		{
			Debug.Log("OnPurcahseVerify error " + action + " " + response);
			m_PurchaseState = kPurchaseState.None;
			OnPurchaseFailed(-1);
		}
	}

	protected void SendServerVerify()
	{
		Debug.Log("SendServerVerify");
		m_PingState = kPingState.Pinging;
		Hashtable hashtable = new Hashtable();
		hashtable["cmd"] = "GetServerTime";
		string text = JsonMapper.ToJson(hashtable);
		Debug.Log(text);
		HttpClient.Instance().SendRequest("IAPServer", "groovy", text, "_IAPManager", "iIAPManager", "OnServerVerify", null);
	}

	protected void OnServerVerify(int taskId, int result, string server, string action, string response, string param)
	{
		Debug.Log("OnServerVerify " + action + " " + response);
		try
		{
			JsonData jsonData = JsonMapper.ToObject(response);
			if ((int)jsonData["code"] == 0)
			{
				m_PingState = kPingState.Success;
			}
			else
			{
				m_PingState = kPingState.Fail;
			}
		}
		catch
		{
			m_PingState = kPingState.Fail;
		}
	}

	public CIAPInfo GetIAPInfo(int nIAPID)
	{
		if (m_IAPCenter == null)
		{
			return null;
		}
		return m_IAPCenter.Get(nIAPID);
	}

	public CCrystal2GoldInfo GetCrystal2GoldInfo(int nGoldID)
	{
		if (m_IAPCenter == null)
		{
			return null;
		}
		return m_IAPCenter.GetCrystal2GoldInfo(nGoldID);
	}

	public IEnumerator TestPingApple()
	{
		m_PingState = kPingState.Pinging;
		WWW www = new WWW("http://www.apple.com/?rand=" + Random.Range(10, 99999));
		Debug.Log(www.url);
		yield return www;
		if (www.error != null)
		{
			Debug.Log("test ping failed " + www.error);
			m_PingState = kPingState.Fail;
		}
		else
		{
			Debug.Log("test ping successed ");
			m_PingState = kPingState.Success;
		}
	}

	public bool Purchase(int nID)
	{
		if (m_PurchaseState != 0)
		{
			return false;
		}
		CIAPInfo cIAPInfo = m_IAPCenter.Get(nID);
		if (cIAPInfo == null)
		{
			return false;
		}
		m_PurchaseState = kPurchaseState.Ping;
		m_nCurPurchase = nID;
		StartCoroutine(TestPingApple());
		return true;
	}

	public void OnPurchaseSuccess(int nID)
	{
		iGameData gameData = iGameApp.GetInstance().m_GameData;
		if (gameData == null)
		{
			return;
		}
		iDataCenter dataCenter = gameData.GetDataCenter();
		if (dataCenter == null)
		{
			return;
		}
		CIAPInfo cIAPInfo = m_IAPCenter.Get(nID);
		if (cIAPInfo != null)
		{
			m_PurchaseState = kPurchaseState.None;
			if (cIAPInfo.isCrystal)
			{
				dataCenter.AddCrystal(cIAPInfo.nValue);
			}
			else
			{
				dataCenter.AddGold(cIAPInfo.nValue);
			}
			dataCenter.Save();
			//iGameApp.GetInstance().Flurry_PurchaseIAP(cIAPInfo.nID);
			if (m_OnSuccess != null)
			{
				m_OnSuccess();
			}
		}
	}

	public void OnPurchaseFailed(int nID)
	{
		if (m_OnFailed != null)
		{
			m_OnFailed();
		}
	}

	public void OnPurchaseCancel()
	{
		if (m_OnCancel != null)
		{
			m_OnCancel();
		}
	}

	public void OnPurchaseNetError()
	{
		if (m_OnNetError != null)
		{
			m_OnNetError();
		}
	}

	public void OnSendVerify()
	{
		if (m_OnSendVerify != null)
		{
			m_OnSendVerify();
		}
	}

	public void OnVerifyFailed()
	{
		if (m_OnVerifyFailed != null)
		{
			m_OnVerifyFailed();
		}
	}

	public void SetSuccessFunc(OnEvent func)
	{
		m_OnSuccess = func;
	}

	public void SetFailedFunc(OnEvent func)
	{
		m_OnFailed = func;
	}

	public void SetCancelFunc(OnEvent func)
	{
		m_OnCancel = func;
	}

	public void SetNetErrorFunc(OnEvent func)
	{
		m_OnNetError = func;
	}

	public void SetOnSendVerifyFunc(OnEvent func)
	{
		m_OnSendVerify = func;
	}

	public void SetOnVerifyFailed(OnEvent func)
	{
		m_OnVerifyFailed = func;
	}
}
