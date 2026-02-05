using UnityEngine;
using UnityEngine.SocialPlatforms;

public class iGameCenter : MonoBehaviour
{
	public delegate void OnEvent();

	public delegate void OnLoginSuccess(IUserProfile user);

	protected static iGameCenter m_Instance;

	protected OnLoginSuccess m_OnLoginSuccess;

	protected OnEvent m_OnLoginFail;

	public static iGameCenter GetInstance()
	{
		if (m_Instance == null)
		{
			GameObject gameObject = new GameObject("_GameCenter");
			Object.DontDestroyOnLoad(gameObject);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			m_Instance = gameObject.AddComponent<iGameCenter>();
		}
		return m_Instance;
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public bool IsLogin()
	{
		return Social.localUser != null && Social.localUser.authenticated;
	}

	public void Login()
	{
		Social.localUser.Authenticate(OnLogin);
	}

	public void SetLoginSuccess(OnLoginSuccess func)
	{
		m_OnLoginSuccess = func;
	}

	public void SetLoginFail(OnEvent func)
	{
		m_OnLoginFail = func;
	}

	protected void OnLogin(bool bSuccess)
	{
		if (bSuccess)
		{
			Debug.Log("GameCenter Login successed");
			if (m_OnLoginSuccess != null)
			{
				m_OnLoginSuccess(Social.localUser);
			}
		}
		else
		{
			Debug.Log("GameCenter Login failed");
			if (m_OnLoginFail != null)
			{
				m_OnLoginFail();
			}
		}
	}
}
