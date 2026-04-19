using EventCenter;
using UnityEngine;

public class Scene_Tavern : MonoBehaviour
{
	public TUIFade m_fade;

	private float m_fade_in_time;

	private float m_fade_out_time;

	private bool do_fade_in;

	private bool is_fade_out;

	private bool do_fade_out;

	private string next_scene = string.Empty;

	public PopupRole popup_role;

	private bool sfx_open_now = true;

	private bool music_open_now = true;

	private void Awake()
	{
		TUIDataServer.Instance().Initialize();
		global::EventCenter.EventCenter.Instance.Register<TUIEvent.BackEvent_SceneTavern>(TUIEvent_SetUIInfo);
	}

	private void Start()
	{
		global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneTavern("TUIEvent_TopBar"));
		global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneTavern("TUIEvent_AllRoleInfo"));
	}

	private void Update()
	{
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
		global::EventCenter.EventCenter.Instance.Unregister<TUIEvent.BackEvent_SceneTavern>(TUIEvent_SetUIInfo);
	}

	public void TUIEvent_SetUIInfo(object sender, TUIEvent.BackEvent_SceneTavern m_event)
	{
		if (m_event.GetEventName() == "TUIEvent_TopBar")
		{
			if (m_event.GetEventInfo().GetPlayerInfo() != null)
			{
				popup_role.SetTopBarInfo(m_event.GetEventInfo().player_info);
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
		else if (m_event.GetEventName() == "TUIEvent_AllRoleInfo")
		{
			if (m_event.GetEventInfo() != null)
			{
				popup_role.AddScrollListItem(m_event.GetEventInfo().all_role_info);
			}
			else
			{
				Debug.Log("error!");
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_RoleUnlock")
		{
			if (m_event.GetControlSuccess())
			{
				popup_role.SetRoleUnlock(sfx_open_now);
				return;
			}
			switch (m_event.GetFalseType())
			{
			case BackEventFalseType.NoGoldEnough:
			{
				int wparam2 = m_event.GetWparam();
				int crystal = 0;
				TUIMappingInfo.GoldToCrystal goldToCrystalFunc = TUIMappingInfo.Instance().GetGoldToCrystalFunc();
				if (goldToCrystalFunc != null)
				{
					crystal = goldToCrystalFunc(wparam2);
				}
				popup_role.ShowPopupGoldToCrystal(wparam2, crystal);
				break;
			}
			case BackEventFalseType.NoCrystalEnough:
			{
				int wparam = m_event.GetWparam();
				popup_role.ShowPopupCrystalNoEnough(wparam);
				break;
			}
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_RoleBuy")
		{
			if (m_event.GetControlSuccess())
			{
				popup_role.SetRoleBuy(sfx_open_now);
				return;
			}
			switch (m_event.GetFalseType())
			{
			case BackEventFalseType.NoGoldEnough:
			{
				int wparam4 = m_event.GetWparam();
				int crystal2 = 0;
				TUIMappingInfo.GoldToCrystal goldToCrystalFunc2 = TUIMappingInfo.Instance().GetGoldToCrystalFunc();
				if (goldToCrystalFunc2 != null)
				{
					crystal2 = goldToCrystalFunc2(wparam4);
				}
				popup_role.ShowPopupGoldToCrystal(wparam4, crystal2);
				break;
			}
			case BackEventFalseType.NoCrystalEnough:
			{
				int wparam3 = m_event.GetWparam();
				popup_role.ShowPopupCrystalNoEnough(wparam3);
				break;
			}
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
		else if (m_event.GetEventName() == "TUIEvent_NewMarkInfo")
		{
			if (m_event.GetEventInfo() != null && m_event.GetEventInfo().all_role_info != null)
			{
				TUIAllRoleInfo all_role_info = m_event.GetEventInfo().all_role_info;
				if (all_role_info != null)
				{
					popup_role.UpdateNewMark(all_role_info.new_mark_list);
				}
			}
			else
			{
				Debug.Log("error!");
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_GoldToCrystal")
		{
			if (m_event.GetControlSuccess())
			{
				popup_role.DoGoldExchange();
				return;
			}
			BackEventFalseType falseType = m_event.GetFalseType();
			if (falseType == BackEventFalseType.NoCrystalEnough)
			{
				int wparam5 = m_event.GetWparam();
				popup_role.ShowPopupCrystalNoEnough(wparam5);
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_EnterIAP")
		{
			if (m_event.GetControlSuccess())
			{
				//DoSceneChange(m_event.GetWparam(), "Scene_IAP");
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
		else if (m_event.GetEventName() == "TUIEvent_EnterIAPCrystalNoEnough")
		{
			if (m_event.GetControlSuccess())
			{
				//DoSceneChange(m_event.GetWparam(), "Scene_IAP");
				return;
			}
			m_fade_in_time = 0f;
			do_fade_in = false;
			m_fade.FadeIn();
		}
		else if (m_event.GetEventName() == "TUIEvent_EnterGoEquip")
		{
			if (m_event.GetControlSuccess())
			{
				DoSceneChange(m_event.GetWparam(), "Scene_Equip");
				return;
			}
			m_fade_in_time = 0f;
			do_fade_in = false;
			m_fade.FadeIn();
		}
		else if (m_event.GetEventName() == "TUIEvent_RoleChange")
		{
			if (m_event.GetControlSuccess())
			{
				if (m_event.GetEventInfo() != null && m_event.GetEventInfo().GetPlayerInfo() != null)
				{
					popup_role.SetTopBarInfo(m_event.GetEventInfo().player_info);
				}
				else
				{
					Debug.Log("error!");
				}
			}
		}
	}

	public void TUIEvent_MoveScreen(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 2)
		{
			popup_role.SetRoleRotation(wparam, lparam);
		}
	}

	public void TUIEvent_BtnBuy(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Button");
			}
			switch (popup_role.GetRoleBuyState())
			{
			case PopupRoleBtnBuy.PopupRoleBuyState.State_Unlock:
				popup_role.ShowPopupUnlock();
				break;
			case PopupRoleBtnBuy.PopupRoleBuyState.State_Buy:
				popup_role.ShowPopupBuy();
				break;
			case PopupRoleBtnBuy.PopupRoleBuyState.State_Use:
			{
				if (sfx_open_now)
				{
					CUISound.GetInstance().Play("UI_Use");
				}
				int roleChooseID = popup_role.GetRoleChooseID();
				global::EventCenter.EventCenter.Instance.Publish(
					this,
					new TUIEvent.SendEvent_SceneTavern("TUIEvent_RoleChange", roleChooseID)
				);
				break;
			}
			default:
				Debug.Log("error!");
				break;
			}
		}
	}

	public void TUIEvent_RoleUnlock(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Button");
			}
			int roleChooseID = popup_role.GetRoleChooseID();
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneTavern("TUIEvent_RoleUnlock", roleChooseID));
			popup_role.ClosePopupUnlock();
		}
	}

	public void TUIEvent_RoleBuy(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Button");
			}
			int roleChooseID = popup_role.GetRoleChooseID();
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneTavern("TUIEvent_RoleBuy", roleChooseID));
			popup_role.ClosePopupBuy();
		}
	}

	public void TUIEvent_CloseRoleUnlock(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Cancle");
			}
			popup_role.ClosePopupUnlock();
		}
	}

	public void TUIEvent_CloseRoleBuy(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Cancle");
			}
			popup_role.ClosePopupBuy();
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
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneTavern("TUIEvent_Back"));
		}
	}

	public void TUIEvent_IAP(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Button");
			}
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneTavern("TUIEvent_EnterIAP"));
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
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneTavern("TUIEvent_EnterGold"));
		}
	}

	public void TUIEvent_BtnGoldToCrystal(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type != 3)
		{
			return;
		}
		if (sfx_open_now)
		{
			CUISound.GetInstance().Play("UI_Button");
		}
		popup_role.HidePopupGoldToCrystal();
	}

	public void TUIEvent_HideUnlockBlink(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Cancle");
			}
			popup_role.CloseBlink();
		}
	}

	public void TUIEvent_CloseGoldToCrystal(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Cancle");
			}
			popup_role.HidePopupGoldToCrystal();
		}
	}

	public void TUIEvent_CloseCrystalNoEnough(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Cancle");
			}
			popup_role.HidePopupCrystalNoEnough();
		}
	}

	public void TUIEvent_BtnCrystalNoEnough(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Button");
			}
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneTavern("TUIEvent_EnterIAPCrystalNoEnough"));
		}
	}

	public void TUIEvent_ClickActiveSkill(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Button");
			}
			popup_role.ShowPopupActiveSkill(control);
		}
	}

	public void TUIEvent_CloseActiveSkill(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Cancle");
			}
			popup_role.HidePopupActiveSkill();
		}
	}

	public void TUIEvent_CloseGoEquip(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Cancle");
			}
			popup_role.HideGoEquip();
		}
	}

	public void TUIEvent_ClickGoEquip(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Button");
			}
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneTavern("TUIEvent_EnterGoEquip"));
			popup_role.HideGoEquip();
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

	public bool GetMusicOpen()
	{
		return music_open_now;
	}

	public bool GetSFXOpen()
	{
		return sfx_open_now;
	}
}
