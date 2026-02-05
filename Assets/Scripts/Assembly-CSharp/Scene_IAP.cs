using EventCenter;
using UnityEngine;

public class Scene_IAP : MonoBehaviour
{
	public TUIFade m_fade;

	private float m_fade_in_time;

	private float m_fade_out_time;

	private bool do_fade_in;

	private bool is_fade_out;

	private bool do_fade_out;

	private string next_scene = string.Empty;

	public Top_Bar top_bar;

	public PopupIAP popup_iap;

	private bool sfx_open_now = true;

	private bool music_open_now = true;

	private void Awake()
	{
		TUIDataServer.Instance().Initialize();
		global::EventCenter.EventCenter.Instance.Register<TUIEvent.BackEvent_SceneIAP>(TUIEvent_SetUIInfo);
	}

	private void Start()
	{
		global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneIAP("TUIEvent_TopBar"));
		if (top_bar != null)
		{
			top_bar.SetBtnCrystalShow(false);
		}
	}

	private void Update()
	{
		if (m_fade == null)
		{
			Debug.Log("error!no found m_fade!");
		}
		m_fade_in_time += Time.deltaTime;
		if (m_fade_in_time >= m_fade.fadeInTime && !do_fade_in)
		{
			do_fade_in = true;
		}
		if (!is_fade_out)
		{
			return;
		}
		m_fade_out_time += Time.deltaTime;
		if (m_fade_out_time >= m_fade.fadeOutTime && !do_fade_out)
		{
			do_fade_out = true;
			m_fade.SetFadeOutEnd();
			TUIMappingInfo.SwitchSceneStr switchSceneStr = TUIMappingInfo.Instance().GetSwitchSceneStr();
			if (switchSceneStr != null)
			{
				switchSceneStr(next_scene);
			}
		}
	}

	private void OnDestroy()
	{
		global::EventCenter.EventCenter.Instance.Unregister<TUIEvent.BackEvent_SceneIAP>(TUIEvent_SetUIInfo);
	}

	public void TUIEvent_SetUIInfo(object sender, TUIEvent.BackEvent_SceneIAP m_event)
	{
		if (m_event.GetEventName() == "TUIEvent_TopBar")
		{
			if (m_event.GetEventInfo() != null && m_event.GetEventInfo().GetPlayerInfo() != null)
			{
				int avatar_id = m_event.GetEventInfo().player_info.avatar_id;
				int level = m_event.GetEventInfo().player_info.level;
				int exp = m_event.GetEventInfo().player_info.exp;
				int level_exp = m_event.GetEventInfo().player_info.level_exp;
				int gold = m_event.GetEventInfo().player_info.gold;
				int crystal = m_event.GetEventInfo().player_info.crystal;
				int avatar_id2 = m_event.GetEventInfo().player_info.avatar_id;
				top_bar.SetAllValue(level, exp, level_exp, gold, crystal, avatar_id2);
			}
			else
			{
				Debug.Log("error!");
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_OptionInfo")
		{
			if (m_event.GetEventInfo() != null && m_event.GetEventInfo().option_info != null)
			{
				bool music_open = m_event.GetEventInfo().option_info.music_open;
				bool sfx_open = m_event.GetEventInfo().option_info.sfx_open;
				sfx_open_now = sfx_open;
				music_open_now = music_open;
			}
			else
			{
				Debug.Log("error!");
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_IAPResult")
		{
			if (m_event.GetControlSuccess())
			{
				popup_iap.Hide();
				popup_iap.ShowPopupWaiting("Please wait while the system is verifying your purphase...");
				return;
			}
			switch ((IAPFailType)m_event.GetWparam())
			{
			case IAPFailType.Cancel:
				popup_iap.Hide();
				break;
			case IAPFailType.Failed:
			case IAPFailType.ServerFaild:
				popup_iap.ShowPopupYes(string.Empty);
				if (sfx_open_now)
				{
					CUISound.GetInstance().Play("UI_Button");
				}
				break;
			default:
				popup_iap.Hide();
				break;
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_ServerResult")
		{
			if (m_event.GetControlSuccess())
			{
				if (sfx_open_now)
				{
					CUISound.GetInstance().Play("UI_Crystal");
				}
				popup_iap.Hide();
				if (m_event.GetEventInfo() != null && m_event.GetEventInfo().GetPlayerInfo() != null)
				{
					int gold2 = m_event.GetEventInfo().player_info.gold;
					int crystal2 = m_event.GetEventInfo().player_info.crystal;
					top_bar.SetGoldValue(gold2);
					top_bar.SetCrystalValue(crystal2);
				}
				else
				{
					Debug.Log("error! no info!");
				}
				return;
			}
			switch ((IAPFailType)m_event.GetWparam())
			{
			case IAPFailType.Cancel:
				popup_iap.Hide();
				break;
			case IAPFailType.Failed:
			case IAPFailType.ServerFaild:
				popup_iap.ShowPopupYes(string.Empty);
				if (sfx_open_now)
				{
					CUISound.GetInstance().Play("UI_Button");
				}
				break;
			default:
				popup_iap.Hide();
				break;
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_Back")
		{
			if (m_event.GetControlSuccess())
			{
				DoSceneChange(m_event.GetWparam(), "Scene_MainMenu");
				return;
			}
			m_fade_in_time = 0f;
			do_fade_in = false;
			m_fade.FadeIn();
		}
		else if (m_event.GetEventName() == "TUIEvent_EnterGold")
		{
			if (m_event.GetControlSuccess())
			{
				DoSceneChange(m_event.GetWparam(), "Scene_Gold");
				return;
			}
			m_fade_in_time = 0f;
			do_fade_in = false;
			m_fade.FadeIn();
		}
	}

	public void TUIEvent_IAPBuy(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Button");
			}
			IAPItem component = control.transform.parent.GetComponent<IAPItem>();
			if (component != null)
			{
				int iD = component.GetID();
				popup_iap.ShowPopupWaiting("Waiting...");
				//global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneIAP("TUIEvent_IAPBuy", iD));
			}
		}
	}

	public void TUIEvent_HidePopup(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Cancle");
			}
			popup_iap.Hide();
		}
	}

	public void TUIEvent_Back(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Button");
			}
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneIAP("TUIEvent_Back"));
		}
	}

	public void TUIEvent_Gold(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Button");
			}
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneIAP("TUIEvent_EnterGold"));
		}
	}

	public void DoSceneChange(int m_scene_id, string m_scene_normal)
	{
		string sceneName = TUIMappingInfo.Instance().GetSceneName(m_scene_id);
		if (sceneName != string.Empty)
		{
			next_scene = sceneName;
		}
		else
		{
			next_scene = m_scene_normal;
		}
		if (!is_fade_out)
		{
			is_fade_out = true;
			m_fade.FadeOut();
		}
	}
}
