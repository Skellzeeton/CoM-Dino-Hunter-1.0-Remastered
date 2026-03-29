using EventCenter;
using UnityEngine;

public class Scene_MainMenu : MonoBehaviour
{
	public TUIFade m_fade;

	public Camera_Village camera_village;

	public Popup_Option popup_option;

	public Popup_Achievement popup_achievement;

	public Top_Bar top_bar;

	public Transform go_forge;

	public Transform go_forge_name;

	public Transform go_forge_new;

	public Transform go_forge_mark;

	public Transform go_tavern;

	public Transform go_tavern_name;

	public Transform go_tavern_new;

	public Transform go_tavern_mark;

	public Transform go_skill;

	public Transform go_skill_name;

	public Transform go_skill_new;

	public Transform go_skill_mark;

	public Transform go_stash;

	public Transform go_stash_name;

	public Transform go_stash_new;

	public Transform go_stash_mark;

	public Transform go_camp;

	public Transform go_camp_name;

	public Transform go_camp_new;

	public Transform go_camp_mark;

	public UnlockBlink unlock_blink;

	public TUILabel label_finished;

	public TUIMeshSprite img_map_bg;

	public TUIMeshSprite img_achievement_bg;

	public TUIMeshSprite img_arrow_left;

	public TUIMeshSprite img_arrow_right;

	public GameObject popup_help;

	public Popup_Credits popup_credits;

	public Popup_Review popup_review;

	private Transform go_control;

	private bool is_click;

	private float m_fade_in_time;

	private float m_fade_out_time;

	private bool do_fade_in;

	private bool is_fade_out;

	private bool do_fade_out;

	private string next_scene = string.Empty;

	private bool sfx_open_now = true;

	private bool music_open_now = true;

	private void Awake()
	{
		TUIDataServer.Instance().Initialize();
		global::EventCenter.EventCenter.Instance.Register<TUIEvent.BackEvent_SceneMainMenu>(TUIEvent_SetUIInfo);
		camera_village.SetCurrentAngle(TUIMappingInfo.Instance().GetCurrentAngle());
		OpenMapBlink();
		if (img_arrow_left != null && img_arrow_left.GetComponent<Animation>() != null)
		{
			img_arrow_left.GetComponent<Animation>().wrapMode = WrapMode.Loop;
			img_arrow_left.GetComponent<Animation>().Play();
		}
		if (img_arrow_right != null && img_arrow_right.GetComponent<Animation>() != null)
		{
			img_arrow_right.GetComponent<Animation>().wrapMode = WrapMode.Loop;
			img_arrow_right.GetComponent<Animation>().Play();
		}
		CUISound.GetInstance().Play("Amb_MapMenu");
	}

	private void Start()
	{
		global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneMainMenu("TUIEvent_TopBar"));
		global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneMainMenu("TUIEvent_OptionInfo"));
		global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneMainMenu("TUIEvent_AcheviementInfo"));
		global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneMainMenu("TUIEvent_EnterInfo"));
	}

	private void Update()
	{
		LookAtCamera();
		UpdateArrowControl();
		if (m_fade == null)
		{
			Debug.Log("error! no found m_fade!");
			return;
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
			TUIMappingInfo.Instance().SetCurrentAngle(camera_village.GetCurrentAngle());
			do_fade_out = true;
			m_fade.SetFadeOutEnd();
			TUIMappingInfo.SwitchSceneStr switchSceneStr = TUIMappingInfo.Instance().GetSwitchSceneStr();
			if (switchSceneStr != null)
			{
				switchSceneStr(next_scene);
				CUISound.GetInstance().Stop("Amb_MapMenu");
			}
		}
	}

	private void OnDestroy()
	{
		global::EventCenter.EventCenter.Instance.Unregister<TUIEvent.BackEvent_SceneMainMenu>(TUIEvent_SetUIInfo);
	}

	public void TUIEvent_SetUIInfo(object sender, TUIEvent.BackEvent_SceneMainMenu m_event)
	{
		if (m_event.GetEventName() == "TUIEvent_TopBar")
		{
			if (m_event.GetEventInfo().player_info != null)
			{
				int level = m_event.GetEventInfo().player_info.level;
				int exp = m_event.GetEventInfo().player_info.exp;
				int level_exp = m_event.GetEventInfo().player_info.level_exp;
				int gold = m_event.GetEventInfo().player_info.gold;
				int crystal = m_event.GetEventInfo().player_info.crystal;
				int avatar_id = m_event.GetEventInfo().player_info.avatar_id;
				top_bar.SetAllValue(level, exp, level_exp, gold, crystal, avatar_id);
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
				popup_option.SetOption(music_open, sfx_open);
				sfx_open_now = sfx_open;
				music_open_now = music_open;
			}
			else
			{
				Debug.Log("error!");
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_AcheviementInfo")
		{
			if (m_event.GetEventInfo() != null && popup_achievement != null)
			{
				popup_achievement.DoCreate(m_event.GetEventInfo().achievement_info, base.gameObject);
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_TakeAchievement")
		{
			if (m_event.GetControlSuccess())
			{
				if (sfx_open_now)
				{
					CUISound.GetInstance().Play("UI_Levelup");
				}
				TUIAchievementRewardInfo tUIAchievementRewardInfo = popup_achievement.TakeAchievement();
				if (tUIAchievementRewardInfo == null)
				{
					Debug.Log("error!");
					return;
				}
				TakeAchievement(tUIAchievementRewardInfo, top_bar);
				popup_achievement.AfterTakeAchievement();
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_EnterInfo")
		{
			if (m_event.GetEventInfo() == null || m_event.GetEventInfo().villiage_enter_info == null)
			{
				return;
			}
			TUIVilliageEnterInfo villiage_enter_info = m_event.GetEventInfo().villiage_enter_info;
			UnlockType unlock_type = villiage_enter_info.unlock_type;
			if (label_finished != null)
			{
				label_finished.Text = villiage_enter_info.finished_text;
			}
			switch (unlock_type)
			{
			case UnlockType.Weapon:
				if (sfx_open_now)
				{
					CUISound.GetInstance().Play("UI_Unlocked_weapon");
				}
				unlock_blink.OpenBlinkWeapon(villiage_enter_info.unlock_weapon_id, "New Equip Unlocked For Purchase!", true);
				break;
			case UnlockType.Skill:
				if (sfx_open_now)
				{
					CUISound.GetInstance().Play("UI_Unlocked_character");
				}
				unlock_blink.OpenBlinkSkill(villiage_enter_info.unlock_skill_id, "New Skill Unlocked For Purchase!", true);
				break;
			case UnlockType.Role:
				if (sfx_open_now)
				{
					CUISound.GetInstance().Play("UI_Unlocked_character");
				}
				unlock_blink.OpenBlinkRole(villiage_enter_info.unlock_role_id, "New Character Unlocked For Purchase!");
				break;
			}
			if (villiage_enter_info.equip_sign == NewMarkType.New)
			{
				go_camp_new.gameObject.SetActiveRecursively(true);
				go_camp_mark.gameObject.SetActiveRecursively(false);
			}
			else if (villiage_enter_info.equip_sign == NewMarkType.Mark)
			{
				go_camp_new.gameObject.SetActiveRecursively(false);
				go_camp_mark.gameObject.SetActiveRecursively(true);
			}
			else
			{
				go_camp_new.gameObject.SetActiveRecursively(false);
				go_camp_mark.gameObject.SetActiveRecursively(false);
			}
			if (villiage_enter_info.forge_sign == NewMarkType.New)
			{
				go_forge_new.gameObject.SetActiveRecursively(true);
				go_forge_mark.gameObject.SetActiveRecursively(false);
			}
			else if (villiage_enter_info.forge_sign == NewMarkType.Mark)
			{
				go_forge_new.gameObject.SetActiveRecursively(false);
				go_forge_mark.gameObject.SetActiveRecursively(true);
			}
			else
			{
				go_forge_new.gameObject.SetActiveRecursively(false);
				go_forge_mark.gameObject.SetActiveRecursively(false);
			}
			if (villiage_enter_info.skill_sign == NewMarkType.New)
			{
				go_skill_new.gameObject.SetActiveRecursively(true);
				go_skill_mark.gameObject.SetActiveRecursively(false);
			}
			else if (villiage_enter_info.skill_sign == NewMarkType.Mark)
			{
				go_skill_new.gameObject.SetActiveRecursively(false);
				go_skill_mark.gameObject.SetActiveRecursively(true);
			}
			else
			{
				go_skill_new.gameObject.SetActiveRecursively(false);
				go_skill_mark.gameObject.SetActiveRecursively(false);
			}
			if (villiage_enter_info.tavern_sign == NewMarkType.New)
			{
				go_tavern_new.gameObject.SetActiveRecursively(true);
				go_tavern_mark.gameObject.SetActiveRecursively(false);
			}
			else if (villiage_enter_info.tavern_sign == NewMarkType.Mark)
			{
				go_tavern_new.gameObject.SetActiveRecursively(false);
				go_tavern_mark.gameObject.SetActiveRecursively(true);
			}
			else
			{
				go_tavern_new.gameObject.SetActiveRecursively(false);
				go_tavern_mark.gameObject.SetActiveRecursively(false);
			}
			if (villiage_enter_info.stash_sign == NewMarkType.New)
			{
				go_stash_new.gameObject.SetActiveRecursively(true);
				go_stash_mark.gameObject.SetActiveRecursively(false);
			}
			else if (villiage_enter_info.stash_sign == NewMarkType.Mark)
			{
				go_stash_new.gameObject.SetActiveRecursively(false);
				go_stash_mark.gameObject.SetActiveRecursively(true);
			}
			else
			{
				go_stash_new.gameObject.SetActiveRecursively(false);
				go_stash_mark.gameObject.SetActiveRecursively(false);
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_ChangeMusic")
		{
			if (m_event.GetControlSuccess())
			{
				popup_option.SetMusicNow();
				music_open_now = popup_option.GetMusicNow();
				CUISound.GetInstance().Stop("BGM_theme");
				if (music_open_now)
				{
					CUISound.GetInstance().Play("BGM_theme");
				}
			}
			else
			{
				popup_option.RestoreOption();
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_ChangeSFX")
		{
			if (m_event.GetControlSuccess())
			{
				popup_option.SetSFXNow();
				sfx_open_now = popup_option.GetSFXNow();
			}
			else
			{
				popup_option.RestoreOption();
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
		else if (m_event.GetEventName() == "TUIEvent_EnterForge")
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
		else if (m_event.GetEventName() == "TUIEvent_EnterTavern")
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
		else if (m_event.GetEventName() == "TUIEvent_EnterSkill")
		{
			if (m_event.GetControlSuccess())
			{
				DoSceneChange(m_event.GetWparam(), "Scene_Skill");
				return;
			}
			m_fade_in_time = 0f;
			do_fade_in = false;
			m_fade.FadeIn();
		}
		else if (m_event.GetEventName() == "TUIEvent_EnterStash")
		{
			if (m_event.GetControlSuccess())
			{
				DoSceneChange(m_event.GetWparam(), "Scene_Stash");
				return;
			}
			m_fade_in_time = 0f;
			do_fade_in = false;
			m_fade.FadeIn();
		}
		else if (m_event.GetEventName() == "TUIEvent_EnterMap")
		{
			if (m_event.GetControlSuccess())
			{
				DoSceneChange(m_event.GetWparam(), "Scene_Map");
				return;
			}
			m_fade_in_time = 0f;
			do_fade_in = false;
			m_fade.FadeIn();
		}
		else if (m_event.GetEventName() == "TUIEvent_ShowHelp")
		{
			if (popup_help != null)
			{
				popup_help.transform.localPosition = new Vector3(0f, 0f, popup_help.transform.localPosition.z);
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_ShowReview")
		{
			if (popup_review != null)
			{
				popup_review.Show();
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_HadAchievementReward")
		{
			if (m_event.GetControlSuccess())
			{
				OpenAchievementBlink();
			}
			else
			{
				CloseAchievementBlink();
			}
		}
	}

	public void TUIEvent_CameraMove(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		switch (event_type)
		{
		case 1:
		{
			if (go_control != null)
			{
				break;
			}
			camera_village.DoBegin();
			Vector2 clickPosition2 = control.GetComponent<TUIMoveEx>().GetClickPosition();
			Ray ray2 = camera_village.GetComponent<Camera>().ScreenPointToRay(new Vector3(clickPosition2.x, clickPosition2.y, 0f));
			Debug.DrawRay(ray2.origin, ray2.direction * 600f, Color.green);
			RaycastHit hitInfo2;
			if (Physics.Raycast(ray2, out hitInfo2))
			{
				Debug.Log("you hit: " + hitInfo2.transform.name);
				if (hitInfo2.transform == go_camp)
				{
					next_scene = "Scene_Equip";
					go_control = go_camp_name;
					PlayForwardAnimation(go_camp_name);
					CUISound.GetInstance().Play("UI_Equip_enter");
				}
				else if (hitInfo2.transform == go_forge)
				{
					next_scene = "Scene_Forge";
					go_control = go_forge_name;
					PlayForwardAnimation(go_forge_name);
					CUISound.GetInstance().Play("UI_Forge_enter");
				}
				else if (hitInfo2.transform == go_tavern)
				{
					next_scene = "Scene_Tavern";
					go_control = go_tavern_name;
					PlayForwardAnimation(go_tavern_name);
					CUISound.GetInstance().Play("UI_Tavern_enter");
				}
				else if (hitInfo2.transform == go_skill)
				{
					next_scene = "Scene_Skill";
					go_control = go_skill_name;
					PlayForwardAnimation(go_skill_name);
					CUISound.GetInstance().Play("UI_Skill_enter");
				}
				else if (hitInfo2.transform == go_stash)
				{
					next_scene = "Scene_Stash";
					go_control = go_stash_name;
					PlayForwardAnimation(go_stash_name);
					CUISound.GetInstance().Play("UI_Stash_enter");
				}
			}
			break;
		}
		case 2:
			if (!is_click)
			{
				PlayBackwardAnimation(go_control);
				go_control = null;
				camera_village.DoMoveBegin();
			}
			break;
		case 3:
			if (!is_click)
			{
				camera_village.DoMove(wparam);
			}
			break;
		case 4:
			if (!is_click)
			{
				camera_village.DoMoveEnd();
			}
			break;
		case 5:
		{
			PlayBackwardAnimation(go_control);
			Vector2 clickPosition = control.GetComponent<TUIMoveEx>().GetClickPosition();
			Ray ray = camera_village.GetComponent<Camera>().ScreenPointToRay(new Vector3(clickPosition.x, clickPosition.y, 0f));
			Debug.DrawRay(ray.origin, ray.direction * 300f, Color.green);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo))
			{
				if (hitInfo.transform.name == "shop_camp")
				{
					camera_village.SetCloser(go_camp);
					is_click = true;
					global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneMainMenu("TUIEvent_EnterEquip"));
				}
				else if (hitInfo.transform.name == "shop_forge")
				{
					camera_village.SetCloser(go_forge);
					is_click = true;
					global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneMainMenu("TUIEvent_EnterForge"));
				}
				else if (hitInfo.transform.name == "shop_tavern")
				{
					camera_village.SetCloser(go_tavern);
					is_click = true;
					global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneMainMenu("TUIEvent_EnterTavern"));
				}
				else if (hitInfo.transform.name == "shop_get skills")
				{
					camera_village.SetCloser(go_skill);
					is_click = true;
					global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneMainMenu("TUIEvent_EnterSkill"));
				}
				else if (hitInfo.transform.name == "shop_stash")
				{
					camera_village.SetCloser(go_stash);
					is_click = true;
					global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneMainMenu("TUIEvent_EnterStash"));
				}
			}
			break;
		}
		}
	}

	public void TUIEvent_Acheviement(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Button");
			}
			popup_achievement.Show();
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneMainMenu("TUIEvent_SetAcheviement"));
		}
	}

	public void TUIEvent_Option(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Button");
			}
			popup_option.Show();
		}
	}

	public void TUIEvent_BtnMusic(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 1 || event_type == 2)
		{
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneMainMenu("TUIEvent_ChangeMusic"));
		}
	}

	public void TUIEvent_BtnSFX(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 1 || event_type == 2)
		{
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneMainMenu("TUIEvent_ChangeSFX"));
		}
	}

	public void TUIEvent_TakeAchievement(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type != 3)
		{
			return;
		}
		if (sfx_open_now)
		{
			CUISound.GetInstance().Play("UI_Button");
			CUISound.GetInstance().Play("UI_Coin_get");
			CUISound.GetInstance().Play("UI_Collection");
		}
		if (control.transform.parent == null)
		{
			Debug.Log("error!");
			return;
		}
		AchievementItem component = control.transform.parent.GetComponent<AchievementItem>();
		if (component == null)
		{
			Debug.Log("error!");
			return;
		}
		popup_achievement.SetTakeAchievementBtn(control);
		int iD = component.GetID();
		int achievementLevel = (int)component.GetAchievementLevel();
		global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneMainMenu("TUIEvent_TakeAchievement", iD, achievementLevel));
	}

	public void TUIEvent_CloseAchievement(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Cancle");
			}
			popup_achievement.Hide();
		}
	}

	public void TUIEvent_CloseOption(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Cancle");
			}
			popup_option.Hide();
		}
	}

	public void TUIEvent_OpenCredits(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Button");
			}
			if (popup_credits != null)
			{
				popup_credits.Show();
			}
		}
	}

	public void TUIEvent_CloseCredits(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Cancle");
			}
			if (popup_credits != null)
			{
				popup_credits.Hide();
			}
		}
	}

	public void TUIEvent_OpenSupport(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Button");
			}
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneMainMenu("TUIEvent_OpenSupportURL"));
		}
	}

	public void TUIEvent_CloseReview(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Cancle");
			}
			if (popup_review != null)
			{
				popup_review.Hide();
			}
		}
	}

	public void TUIEvent_ClickReview(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Button");
			}
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneMainMenu("TUIEvent_OpenReviewURL"));
			popup_review.Hide();
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
			//global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneMainMenu("TUIEvent_EnterIAP"));
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
			//global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneMainMenu("TUIEvent_EnterGold"));
		}
	}

	public void TUIEvent_Map(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Entergame");
			}
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneMainMenu("TUIEvent_EnterMap"));
		}
	}

	public void TUIEvent_HideUnlockBlink(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Cancle");
			}
			unlock_blink.CloseBlink();
		}
	}

	public void TUIEvent_HideHelp(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Cancle");
			}
			if (popup_help != null)
			{
				popup_help.transform.localPosition = new Vector3(0f, -1000f, popup_help.transform.localPosition.z);
			}
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

	private void PlayForwardAnimation(Transform go)
	{
		if (!(go == null))
		{
			AnimationState animationState = go.GetComponent<Animation>()[go.GetComponent<Animation>().clip.name];
			animationState.speed = 1f;
			animationState.normalizedTime = 0f;
			go.GetComponent<Animation>().Play(animationState.name, PlayMode.StopAll);
		}
	}

	private void PlayBackwardAnimation(Transform go)
	{
		if (!(go == null))
		{
			AnimationState animationState = go.GetComponent<Animation>()[go.GetComponent<Animation>().clip.name];
			animationState.speed = -1f;
			animationState.normalizedTime = 1f;
			go.GetComponent<Animation>().Play(animationState.name, PlayMode.StopAll);
		}
	}

	private void LookAtCamera()
	{
		if (camera_village == null)
		{
			Debug.Log("error!");
			return;
		}
		Vector3 eulerAngles = camera_village.transform.eulerAngles + new Vector3(-90f, 0f, 180f);
		if (go_forge_name != null)
		{
			go_forge_name.transform.eulerAngles = eulerAngles;
		}
		if (go_tavern_name != null)
		{
			go_tavern_name.transform.eulerAngles = eulerAngles;
		}
		if (go_skill_name != null)
		{
			go_skill_name.transform.eulerAngles = eulerAngles;
		}
		if (go_stash_name != null)
		{
			go_stash_name.transform.eulerAngles = eulerAngles;
		}
		if (go_camp_name != null)
		{
			go_camp_name.transform.eulerAngles = eulerAngles;
		}
	}

	private void TakeAchievement(TUIAchievementRewardInfo m_reward_info, Top_Bar m_top_bar)
	{
		if (m_reward_info == null)
		{
			Debug.Log("error!");
			return;
		}
		int num = 0;
		int num2 = 0;
		if (m_reward_info.open_reward01)
		{
			if (m_reward_info.reward_unit01 == UnitType.Gold)
			{
				num += m_reward_info.reward_value01;
			}
			else if (m_reward_info.reward_unit01 == UnitType.Crystal)
			{
				num2 += m_reward_info.reward_value01;
			}
		}
		if (m_reward_info.open_reward02)
		{
			if (m_reward_info.reward_unit02 == UnitType.Gold)
			{
				num += m_reward_info.reward_value02;
			}
			else if (m_reward_info.reward_unit02 == UnitType.Crystal)
			{
				num2 += m_reward_info.reward_value02;
			}
		}
		m_top_bar.SetGoldValue(m_top_bar.GetGoldValue() + num);
		m_top_bar.SetCrystalValue(m_top_bar.GetCrystalValue() + num2);
	}

	private void UpdateArrowControl()
	{
		if (img_arrow_left == null || img_arrow_right == null || camera_village == null)
		{
			Debug.Log("error!");
			return;
		}
		float num = 0.37f;
		float num2 = 0.65f;
		float persentAngle = camera_village.GetPersentAngle();
		MeshRenderer leftRenderer = img_arrow_left.GetComponent<MeshRenderer>();
		if (leftRenderer != null)
		{
			leftRenderer.enabled = (persentAngle > num);
		}
		MeshRenderer rightRenderer = img_arrow_right.GetComponent<MeshRenderer>();
		if (rightRenderer != null)
		{
			rightRenderer.enabled = (persentAngle < num2);
		}
	}

	private void OpenMapBlink()
	{
		if (img_map_bg != null && img_map_bg.GetComponent<Animation>() != null)
		{
			img_map_bg.GetComponent<Animation>().wrapMode = WrapMode.Loop;
			img_map_bg.GetComponent<Animation>().Play();
		}
	}

	private void OpenAchievementBlink()
	{
		if (img_achievement_bg != null && img_achievement_bg.GetComponent<Animation>() != null)
		{
			img_achievement_bg.gameObject.SetActiveRecursively(true);
			img_achievement_bg.GetComponent<Animation>().wrapMode = WrapMode.Loop;
			img_achievement_bg.GetComponent<Animation>().Play();
		}
	}

	private void CloseAchievementBlink()
	{
		if (img_achievement_bg != null && img_achievement_bg.GetComponent<Animation>() != null)
		{
			img_achievement_bg.GetComponent<Animation>().Stop();
			img_achievement_bg.gameObject.SetActiveRecursively(false);
		}
	}
}
