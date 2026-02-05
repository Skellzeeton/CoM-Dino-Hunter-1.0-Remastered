using System.Collections.Generic;
using UnityEngine;

public class iMapUI : MonoBehaviour, TUIHandler
{
	public TUI m_TUI;

	public Transform m_TUIControls;

	protected iGameState m_GameState;

	protected List<TUIButtonClick> m_ltLevelNum;

	protected TUIButtonClick m_btnMutiplyGame;

	private void Awake()
	{
		GameObject gameObject = GameObject.Find("TUI");
		if (!(gameObject == null))
		{
			m_TUI = gameObject.GetComponent<TUI>();
			m_TUIControls = m_TUI.transform.Find("TUIControls");
			m_ltLevelNum = new List<TUIButtonClick>();
		}
	}

	private void Start()
	{
		m_GameState = iGameApp.GetInstance().m_GameState;
		foreach (Transform tUIControl in m_TUIControls)
		{
			TUIButtonClick component = tUIControl.GetComponent<TUIButtonClick>();
			if (component != null)
			{
				m_ltLevelNum.Add(component);
			}
		}
		m_btnMutiplyGame = GetControl<TUIButtonClick>("btnMutiply");
	}

	private void Update()
	{
		if (m_TUI == null)
		{
			return;
		}
		TUIInput[] input = TUIInputManager.GetInput();
		TUIInput[] array = input;
		foreach (TUIInput input2 in array)
		{
			if (m_TUI.HandleInput(input2))
			{
				break;
			}
		}
	}

	public void HandleEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		foreach (TUIButtonClick item in m_ltLevelNum)
		{
			if (item == control && eventType == 3)
			{
				iLevelNum component = item.gameObject.GetComponent<iLevelNum>();
				if (component != null)
				{
					Debug.Log(component.nLevelID);
					m_GameState.GameLevel = component.nLevelID;
					iGameApp.GetInstance().EnterScene(kGameSceneEnum.Game);
					return;
				}
			}
		}
		if (m_btnMutiplyGame == control && eventType != 3)
		{
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
}
