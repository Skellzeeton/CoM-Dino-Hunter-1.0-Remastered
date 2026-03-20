using EventCenter;
using UnityEngine;

public class Scene_Map : MonoBehaviour
{
	public TUIFade m_fade;

	private float m_fade_in_time;

	private float m_fade_out_time;

	private bool do_fade_in;

	private bool is_fade_out;

	private bool do_fade_out;

	private string next_scene = "Scene_MainMenu";

	private int next_scene_id;

	public Top_Bar top_bar;

	public LevelMap level_map;

	public PopupLevel popup_level_map;

	public Camera tui_camera;

	public TUIButtonClick btn_villiage;

	private Transform level_point;

	private bool sfx_open_now = true;

	private bool music_open_now = true;

	private void Awake()
	{
		TUIDataServer.Instance().Initialize();
		global::EventCenter.EventCenter.Instance.Register<TUIEvent.BackEvent_SceneMap>(TUIEvent_SetUIInfo);
		TUISelfAdaptiveAnchorGroup component = base.transform.GetComponent<TUISelfAdaptiveAnchorGroup>();
		if (component != null)
		{
			component.Anchor();
		}
	}

	private void Start()
	{
		global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneMap("TUIEvent_TopBar"));
		global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneMap("TUIEvent_MapEnterInfo"));
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
		if (!(m_fade_out_time >= m_fade.fadeOutTime) || do_fade_out)
		{
			return;
		}
		do_fade_out = true;
		m_fade.SetFadeOutEnd();
		if (next_scene_id != 0)
		{
			TUIMappingInfo.SwitchSceneInt switchSceneInt = TUIMappingInfo.Instance().GetSwitchSceneInt();
			if (switchSceneInt != null)
			{
				switchSceneInt(next_scene_id);
			}
			CUISound.GetInstance().Stop("BGM_theme");
		}
		else
		{
			TUIMappingInfo.SwitchSceneStr switchSceneStr = TUIMappingInfo.Instance().GetSwitchSceneStr();
			if (switchSceneStr != null)
			{
				switchSceneStr(next_scene);
			}
		}
	}

	private void OnDestroy()
	{
		global::EventCenter.EventCenter.Instance.Unregister<TUIEvent.BackEvent_SceneMap>(TUIEvent_SetUIInfo);
	}

	public void TUIEvent_SetUIInfo(object sender, TUIEvent.BackEvent_SceneMap m_event)
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
		else if (m_event.GetEventName() == "TUIEvent_MapEnterInfo")
		{
			if (m_event.GetEventInfo() == null)
			{
				return;
			}
			if (m_event.GetEventInfo().map_info == null || top_bar == null)
			{
				Debug.Log("error!");
				return;
			}
			level_map.SetMapEnterInfo(m_event.GetEventInfo().map_info);
			MapEnterType map_enter_type = m_event.GetEventInfo().map_info.map_enter_type;
			if (map_enter_type == MapEnterType.SearchGoods)
			{
				top_bar.SetBtnBackShow(true);
				if (btn_villiage != null)
				{
					btn_villiage.gameObject.SetActiveRecursively(false);
				}
				return;
			}
			top_bar.SetBtnBackShow(false);
			if (btn_villiage != null)
			{
				btn_villiage.gameObject.SetActiveRecursively(true);
				btn_villiage.Show();
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_LevelInfo")
		{
			if (m_event.GetEventInfo() == null)
			{
				return;
			}
			TUIMapInfo map_info = m_event.GetEventInfo().map_info;
			if (map_info != null)
			{
				TUILevelInfo level_info = map_info.level_info;
				if (map_info != null)
				{
					level_map.SetLevelInfo(level_info);
					popup_level_map.Show(level_info);
				}
				else
				{
					Debug.Log("error! no map info!");
				}
			}
			else
			{
				Debug.Log("error! no map info!");
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_EnterLevel")
		{
			if (m_event.GetControlSuccess())
			{
				int wparam = m_event.GetWparam();
				if (wparam != 0)
				{
					next_scene_id = wparam;
				}
				else
				{
					next_scene = "Scene_MainMenu";
				}
				if (!is_fade_out)
				{
					is_fade_out = true;
					m_fade.FadeOut();
				}
			}
			else
			{
				m_fade_in_time = 0f;
				do_fade_in = false;
				m_fade.FadeIn();
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
		else if (m_event.GetEventName() == "TUIEvent_EnterEquip")
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
		else if (m_event.GetEventName() == "TUIEvent_EnterWeaponBuy")
		{
			if (m_event.GetControlSuccess())
			{
				DoSceneChange(m_event.GetWparam(), "Scene_Forge");
				return;
			}
			m_fade_in_time = 0f;
			do_fade_in = false;
			m_fade.FadeIn();
		}
		else if (m_event.GetEventName() == "TUIEvent_EnterRoleBuy")
		{
			if (m_event.GetControlSuccess())
			{
				DoSceneChange(m_event.GetWparam(), "Scene_Tavern");
				return;
			}
			m_fade_in_time = 0f;
			do_fade_in = false;
			m_fade.FadeIn();
		}
		else if (m_event.GetEventName() == "TUIEvent_EnterVilliage")
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
	}

	public void TUIEvent_MoveScreen(TUIControl control, int event_type, float wparam, float lparam, object obj)
	{
		level_map.MoveScreen(wparam, 0f);
	}

	public void TUIEvent_ShowPopup(TUIControl control, int event_type, float wparam, float lparam, object obj)
	{
		if (event_type != 3)
		{
			return;
		}
		if (sfx_open_now)
		{
			CUISound.GetInstance().Play("UI_Button");
		}
		level_point = control.transform.parent.transform;
		LevelPoint component = level_point.GetComponent<LevelPoint>();
		LevelPointEx component2 = level_point.GetComponent<LevelPointEx>();
		if (component != null)
		{
			if (component.GetLevelInfo() == null)
			{
				int levelID = component.GetLevelID();
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneMap("TUIEvent_LevelInfo", levelID));
			}
			else
			{
				popup_level_map.Show(component.GetLevelInfo());
			}
		}
		else if (component2 != null)
		{
			if (component2.GetLevelInfo() == null)
			{
				int levelID2 = component2.GetLevelID();
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneMap("TUIEvent_LevelInfo", levelID2));
			}
			else
			{
				popup_level_map.Show(component2.GetLevelInfo());
			}
		}
		else
		{
			Debug.Log("error!");
		}
	}

	public void TUIEvent_EnterLevel(TUIControl control, int event_type, float wparam, float lparam, object obj)
	{
		if (event_type != 3)
		{
			return;
		}
		if (sfx_open_now)
		{
			CUISound.GetInstance().Play("UI_Button");
		}
		popup_level_map.Hide();
		if (level_point == null)
		{
			Debug.Log("error!");
			return;
		}
		LevelPoint component = level_point.GetComponent<LevelPoint>();
		LevelPointEx component2 = level_point.GetComponent<LevelPointEx>();
		if (component != null)
		{
			int levelID = component.GetLevelID();
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneMap("TUIEvent_EnterLevel", levelID));
		}
		else if (component2 != null)
		{
			int levelID2 = component2.GetLevelID();
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneMap("TUIEvent_EnterLevel", levelID2));
		}
		else
		{
			Debug.Log("error!");
		}
	}

	public void TUIEvent_ClickRecommend(TUIControl control, int event_type, float wparam, float lparam, object obj)
	{
		if (event_type != 3)
		{
			return;
		}
		if (sfx_open_now)
		{
			CUISound.GetInstance().Play("UI_Button");
		}
		PopupLevel_Recommend component = control.transform.parent.GetComponent<PopupLevel_Recommend>();
		PopupLevel_Recommend.RecommendBtnState recommendBtnState = component.GetRecommendBtnState();
		Debug.Log("m_btn_state:" + recommendBtnState);
		switch (recommendBtnState)
		{
		case PopupLevel_Recommend.RecommendBtnState.RoleBuy:
		{
			TUIRecommendRoleInfo recommendRoleInfo = component.GetRecommendRoleInfo();
			if (recommendRoleInfo == null)
			{
				Debug.Log("error!");
			}
			int id2 = recommendRoleInfo.id;
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneMap("TUIEvent_EnterRoleBuy", id2));
			break;
		}
		case PopupLevel_Recommend.RecommendBtnState.WeaponBuy:
		{
			TUIRecommendWeaponInfo recommendWeaponInfo = component.GetRecommendWeaponInfo();
			if (recommendWeaponInfo == null)
			{
				Debug.Log("error!");
			}
			int id = recommendWeaponInfo.id;
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneMap("TUIEvent_EnterWeaponBuy", id));
			break;
		}
		case PopupLevel_Recommend.RecommendBtnState.RoleEquip:
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Use");
			}
			TUIRecommendRoleInfo recommendRoleInfo = component.GetRecommendRoleInfo();
			if (recommendRoleInfo == null)
			{
				Debug.Log("error!");
				return;
			}
			int id = recommendRoleInfo.id;
			global::EventCenter.EventCenter.Instance.Publish(
				this,
				new TUIEvent.SendEvent_SceneTavern("TUIEvent_RoleChange", id)
			);
			global::EventCenter.EventCenter.Instance.Publish(
				this,
				new TUIEvent.SendEvent_SceneMap("TUIEvent_TopBar")
			);
			popup_level_map.Hide();
			return;
		}
		case PopupLevel_Recommend.RecommendBtnState.WeaponEquip:
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneMap("TUIEvent_EnterEquip"));
			break;
		}
		if (!is_fade_out)
		{
			is_fade_out = true;
			m_fade.FadeOut();
		}
	}

	public void TUIEvent_ClosePopup(TUIControl control, int event_type, float wparam, float lparam, object obj)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Cancle");
			}
			popup_level_map.Hide();
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
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneMap("TUIEvent_Back"));
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
			//global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneMap("TUIEvent_EnterIAP"));
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
			//global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneMap("TUIEvent_EnterGold"));
		}
	}

	public void TUIEvent_EnterVilliage(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Button");
			}
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneMap("TUIEvent_EnterVilliage"));
		}
	}

	public void TUIEvent_ShowGoodsTips(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 1)
		{
			popup_level_map.ShowTips(control);
		}
		else
		{
			popup_level_map.HideTips();
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
