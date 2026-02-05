using EventCenter;
using UnityEngine;

public class Scene_Equip : MonoBehaviour
{
	public TUIFade m_fade;

	private float m_fade_in_time;

	private float m_fade_out_time;

	private bool do_fade_in;

	private bool is_fade_out;

	private bool do_fade_out;

	private string next_scene = string.Empty;

	public BtnItem_Item btn_skill01;

	public BtnItem_Item btn_skill02;

	public BtnItem_Item btn_skill03;

	public BtnItem_Item btn_skill04;

	public BtnItem_Item btn_prop01;

	public BtnItem_Item btn_prop02;

	public BtnItem_Item btn_weapon01;

	public BtnItem_Item btn_weapon02;

	public BtnItem_Item btn_weapon03;

	public BtnItem_Item btn_weapon04;

	public BtnItem_Item btn_role;

	public Popup_Show popup_prop;

	public Popup_Show popup_skill01;

	public Popup_Show popup_skill;

	public Popup_Show popup_weapon01;

	public Popup_Show popup_weapon02;

	public Popup_Show popup_weapon03;

	public Popup_Show popup_role;

	public Role_Control go_role;

	public TUILabel label_role_name;

	public Top_Bar top_bar;

	public PopupGoBuy popup_go_buy;

	private Popup_Show popup_weapon_now;

	private bool sfx_open_now = true;

	private bool music_open_now = true;

	private void Awake()
	{
		TUIDataServer.Instance().Initialize();
		global::EventCenter.EventCenter.Instance.Register<TUIEvent.BackEvent_SceneEquip>(TUIEvent_SetUIInfo);
	}

	private void Start()
	{
		global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneEquip("TUIEvent_TopBar"));
		global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneEquip("TUIEvent_RoleSign"));
		global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneEquip("TUIEvent_SkillSign"));
		global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneEquip("TUIEvent_WeaponSign"));
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
		global::EventCenter.EventCenter.Instance.Unregister<TUIEvent.BackEvent_SceneEquip>(TUIEvent_SetUIInfo);
	}

	public void TUIEvent_SetUIInfo(object sender, TUIEvent.BackEvent_SceneEquip m_event)
	{
		if (m_event.GetEventName() == "TUIEvent_TopBar")
		{
			if (m_event.GetEventInfo() != null && m_event.GetEventInfo().player_info != null)
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
				sfx_open_now = sfx_open;
				music_open_now = music_open;
			}
			else
			{
				Debug.Log("error!");
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_RoleSign")
		{
			if (m_event.GetEventInfo() != null && m_event.GetEventInfo().equip_info != null)
			{
				if (m_event.GetEventInfo().equip_info.role != null)
				{
					go_role.ChangeRole(m_event.GetEventInfo().equip_info.role.texture_id);
					go_role.SetRoleFixedRotation(new Vector3(0f, -40f, 0f));
					label_role_name.Text = m_event.GetEventInfo().equip_info.role.name;
				}
				btn_role.SetInfo(m_event.GetEventInfo().equip_info.role);
				popup_role.SetInfo(m_event.GetEventInfo().equip_info.roles_list, base.gameObject, Popup_Show.PopupType.Roles, m_event.GetEventInfo().equip_info.roles_new_mark_list);
				popup_role.SetBtnInfo(1, btn_role, m_event.GetEventInfo().equip_info.roles_list, m_event.GetEventInfo().equip_info.roles_new_mark_list);
			}
			else
			{
				Debug.Log("error!");
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_SkillSign")
		{
			if (m_event.GetEventInfo() != null && m_event.GetEventInfo().equip_info != null)
			{
				btn_skill01.SetInfo(m_event.GetEventInfo().equip_info.skill01, Popup_Show.PopupType.Skills);
				btn_skill02.SetInfo(m_event.GetEventInfo().equip_info.skill02, Popup_Show.PopupType.Skills);
				btn_skill03.SetInfo(m_event.GetEventInfo().equip_info.skill03, Popup_Show.PopupType.Skills);
				btn_skill04.SetInfo(m_event.GetEventInfo().equip_info.skill04, Popup_Show.PopupType.Skills);
				popup_skill.SetInfo(m_event.GetEventInfo().equip_info.skill_list, base.gameObject, Popup_Show.PopupType.Skills, m_event.GetEventInfo().equip_info.skill_new_mark_list);
				popup_skill.SetBtnInfo(1, btn_skill01);
				popup_skill.SetBtnInfo(2, btn_skill02, m_event.GetEventInfo().equip_info.skill_list, m_event.GetEventInfo().equip_info.skill_new_mark_list);
				popup_skill.SetBtnInfo(3, btn_skill03, m_event.GetEventInfo().equip_info.skill_list, m_event.GetEventInfo().equip_info.skill_new_mark_list);
				popup_skill.SetBtnInfo(4, btn_skill04, m_event.GetEventInfo().equip_info.skill_list, m_event.GetEventInfo().equip_info.skill_new_mark_list);
			}
			else
			{
				Debug.Log("error!");
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_PropSign")
		{
			if (m_event.GetEventInfo() != null && m_event.GetEventInfo().equip_info != null)
			{
				btn_prop01.SetInfo(m_event.GetEventInfo().equip_info.prop01, Popup_Show.PopupType.Props);
				btn_prop02.SetInfo(m_event.GetEventInfo().equip_info.prop02, Popup_Show.PopupType.Props);
			}
			else
			{
				Debug.Log("error!");
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_WeaponSign")
		{
			if (m_event.GetEventInfo() != null && m_event.GetEventInfo().equip_info != null)
			{
				if (m_event.GetEventInfo().equip_info.weapon01 != null)
				{
					go_role.ChangeWeapon(m_event.GetEventInfo().equip_info.weapon01.texture_id);
				}
				btn_weapon01.SetInfo(m_event.GetEventInfo().equip_info.weapon01, Popup_Show.PopupType.Weapons01);
				btn_weapon02.SetInfo(m_event.GetEventInfo().equip_info.weapon02, Popup_Show.PopupType.Weapons02);
				btn_weapon03.SetInfo(m_event.GetEventInfo().equip_info.weapon03, Popup_Show.PopupType.Weapons02);
				btn_weapon04.SetInfo(m_event.GetEventInfo().equip_info.weapon04, Popup_Show.PopupType.Weapons03);
				popup_weapon01.SetInfo(m_event.GetEventInfo().equip_info.weapon_list01, base.gameObject, Popup_Show.PopupType.Weapons01, m_event.GetEventInfo().equip_info.weapon_new_mark_list);
				popup_weapon02.SetInfo(m_event.GetEventInfo().equip_info.weapon_list02, base.gameObject, Popup_Show.PopupType.Weapons02, m_event.GetEventInfo().equip_info.weapon_new_mark_list);
				popup_weapon03.SetInfo(m_event.GetEventInfo().equip_info.weapon_list03, base.gameObject, Popup_Show.PopupType.Weapons03, m_event.GetEventInfo().equip_info.weapon_new_mark_list);
				popup_weapon01.SetBtnInfo(1, btn_weapon01, m_event.GetEventInfo().equip_info.weapon_list01, m_event.GetEventInfo().equip_info.weapon_new_mark_list);
				popup_weapon02.SetBtnInfo(2, btn_weapon02, m_event.GetEventInfo().equip_info.weapon_list02, m_event.GetEventInfo().equip_info.weapon_new_mark_list);
				popup_weapon02.SetBtnInfo(3, btn_weapon03, m_event.GetEventInfo().equip_info.weapon_list02, m_event.GetEventInfo().equip_info.weapon_new_mark_list);
				popup_weapon03.SetBtnInfo(4, btn_weapon04, m_event.GetEventInfo().equip_info.weapon_list03, m_event.GetEventInfo().equip_info.weapon_new_mark_list);
			}
			else
			{
				Debug.Log("error!");
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_RoleEquip")
		{
			if (m_event.GetControlSuccess())
			{
				if (popup_role.GetItemSelectInfo() != null)
				{
					int texture_id = popup_role.GetItemSelectInfo().texture_id;
					string text = popup_role.GetItemSelectInfo().name;
					go_role.ChangeRole(texture_id);
					go_role.SetRoleFixedRotation(new Vector3(0f, -40f, 0f));
					label_role_name.Text = text;
					if (m_event.GetEventInfo() != null && m_event.GetEventInfo().player_info != null)
					{
						int level2 = m_event.GetEventInfo().player_info.level;
						int exp2 = m_event.GetEventInfo().player_info.exp;
						int level_exp2 = m_event.GetEventInfo().player_info.level_exp;
						int gold2 = m_event.GetEventInfo().player_info.gold;
						int crystal2 = m_event.GetEventInfo().player_info.crystal;
						int avatar_id2 = m_event.GetEventInfo().player_info.avatar_id;
						top_bar.SetAllValue(level2, exp2, level_exp2, gold2, crystal2, avatar_id2);
						if (m_event.GetEventInfo().equip_info != null)
						{
							btn_skill01.SetInfo(m_event.GetEventInfo().equip_info.skill01, Popup_Show.PopupType.Skills);
							btn_skill02.SetInfo(m_event.GetEventInfo().equip_info.skill02, Popup_Show.PopupType.Skills);
							btn_skill03.SetInfo(m_event.GetEventInfo().equip_info.skill03, Popup_Show.PopupType.Skills);
							btn_skill04.SetInfo(m_event.GetEventInfo().equip_info.skill04, Popup_Show.PopupType.Skills);
							popup_skill.ClearScrollInfo();
							popup_skill.SetInfo(m_event.GetEventInfo().equip_info.skill_list, base.gameObject, Popup_Show.PopupType.Skills, m_event.GetEventInfo().equip_info.skill_new_mark_list);
							popup_skill.SetBtnInfo(1, btn_skill01);
							popup_skill.SetBtnInfo(2, btn_skill02, m_event.GetEventInfo().equip_info.skill_list, m_event.GetEventInfo().equip_info.skill_new_mark_list);
							popup_skill.SetBtnInfo(3, btn_skill03, m_event.GetEventInfo().equip_info.skill_list, m_event.GetEventInfo().equip_info.skill_new_mark_list);
							popup_skill.SetBtnInfo(4, btn_skill04, m_event.GetEventInfo().equip_info.skill_list, m_event.GetEventInfo().equip_info.skill_new_mark_list);
						}
						else
						{
							Debug.Log("no skill!");
						}
					}
					else
					{
						Debug.Log("error! no player info");
					}
				}
				else
				{
					Debug.Log("error!");
				}
				popup_role.EquipItem();
			}
			popup_role.SetItemSelectInfo(null);
			popup_role.SetItemNowInfo(null);
		}
		else if (m_event.GetEventName() == "TUIEvent_SkillEquip")
		{
			if (m_event.GetControlSuccess())
			{
				popup_skill.EquipItem();
			}
			popup_skill.SetItemSelectInfo(null);
			popup_skill.SetItemNowInfo(null);
		}
		else if (m_event.GetEventName() == "TUIEvent_SkillUnEquip")
		{
			if (m_event.GetControlSuccess())
			{
				popup_skill.UnEquipItem();
			}
			popup_skill.SetItemSelectInfo(null);
			popup_skill.SetItemNowInfo(null);
		}
		else if (m_event.GetEventName() == "TUIEvent_SkillExchange")
		{
			if (m_event.GetControlSuccess())
			{
				popup_skill.ExchangeSkill();
			}
			popup_skill.SetItemSelectInfo(null);
			popup_skill.SetItemNowInfo(null);
		}
		else if (m_event.GetEventName() == "TUIEvent_WeaponEquip")
		{
			if (m_event.GetControlSuccess())
			{
				int texture_id2 = popup_weapon_now.GetItemSelectInfo().texture_id;
				go_role.ChangeWeapon(texture_id2);
				popup_weapon_now.EquipItem();
			}
			popup_weapon_now.SetItemSelectInfo(null);
			popup_weapon_now.SetItemNowInfo(null);
		}
		else if (m_event.GetEventName() == "TUIEvent_WeaponExchange")
		{
			if (m_event.GetControlSuccess())
			{
				popup_weapon_now.ExchangeWeapon();
				int texture_id3 = popup_weapon_now.GetItemSelectInfo().texture_id;
				go_role.ChangeWeapon(texture_id3);
			}
			popup_weapon_now.SetItemSelectInfo(null);
			popup_weapon_now.SetItemNowInfo(null);
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
		else if (m_event.GetEventName() == "TUIEvent_RoleNewMarkInfo")
		{
			if (m_event.GetEventInfo() != null && m_event.GetEventInfo().equip_info != null)
			{
				popup_role.UpdateNewMark(m_event.GetEventInfo().equip_info.roles_new_mark_list);
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_SkillNewMarkInfo")
		{
			if (m_event.GetEventInfo() != null && m_event.GetEventInfo().equip_info != null)
			{
				popup_skill.UpdateNewMark(m_event.GetEventInfo().equip_info.skill_new_mark_list);
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_WeaponNewMarkInfo")
		{
			if (m_event.GetEventInfo() != null && m_event.GetEventInfo().equip_info != null)
			{
				popup_weapon01.UpdateNewMark(m_event.GetEventInfo().equip_info.weapon_new_mark_list);
				popup_weapon02.UpdateNewMark(m_event.GetEventInfo().equip_info.weapon_new_mark_list);
				popup_weapon03.UpdateNewMark(m_event.GetEventInfo().equip_info.weapon_new_mark_list);
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
		else if (m_event.GetEventName() == "TUIEvent_EnterGoBuyWeapon")
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
		else if (m_event.GetEventName() == "TUIEvent_EnterGoBuySkill")
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
	}

	public void TUIEvent_PopupRole(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			Debug.Log("you click role");
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Button");
			}
			popup_role.Show();
			popup_role.SetItemNowInfo(control.GetComponent<BtnItem_Item>());
			int num = popup_role.BeforeItemSelect(control.GetComponent<BtnItem_Item>().GetInfo());
			if (num != 0)
			{
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneEquip("TUIEvent_RolesChoose", num));
			}
		}
	}

	public void TUIEvent_PopupProp(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Button");
			}
			popup_prop.Show();
			popup_prop.SetSimpleInfo(control.GetComponent<BtnItem_Item>().GetInfo());
		}
	}

	public void TUIEvent_PopupSkill01(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Button");
			}
			popup_skill01.Show();
			popup_skill01.SetSimpleInfo(control.GetComponent<BtnItem_Item>().GetInfo(), Popup_Show.PopupType.Skills01);
		}
	}

	public void TUIEvent_PopupSkill(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type != 3)
		{
			return;
		}
		Debug.Log("You click skill");
		if (sfx_open_now)
		{
			CUISound.GetInstance().Play("UI_Button");
		}
		if (popup_skill.IsEmpty())
		{
			popup_go_buy.Show(PopupGoBuy.GoBuyType.Skill);
			return;
		}
		popup_skill.Show();
		popup_skill.SetItemNowInfo(control.GetComponent<BtnItem_Item>());
		int num = popup_skill.BeforeItemSelect(control.GetComponent<BtnItem_Item>().GetInfo());
		if (num != 0)
		{
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneEquip("TUIEvent_SkillChoose", num));
		}
	}

	public void TUIEvent_PopupWeapon(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type != 3)
		{
			return;
		}
		Debug.Log("You click weapon");
		if (sfx_open_now)
		{
			CUISound.GetInstance().Play("UI_Button");
		}
		BtnItem_Item component = control.GetComponent<BtnItem_Item>();
		switch (component.GetIndex())
		{
		case 1:
		{
			if (popup_weapon01.IsEmpty())
			{
				popup_go_buy.Show(PopupGoBuy.GoBuyType.Weapon);
				break;
			}
			popup_weapon01.Show();
			popup_weapon01.SetItemNowInfo(component);
			int num2 = popup_weapon01.BeforeItemSelect(component.GetInfo());
			if (num2 != 0)
			{
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneEquip("TUIEvent_WeaponChoose", num2));
			}
			popup_weapon_now = popup_weapon01;
			break;
		}
		case 2:
		case 3:
		{
			if (popup_weapon02.IsEmpty())
			{
				popup_go_buy.Show(PopupGoBuy.GoBuyType.Weapon);
				break;
			}
			popup_weapon02.Show();
			popup_weapon02.SetItemNowInfo(component);
			int num3 = popup_weapon02.BeforeItemSelect(component.GetInfo());
			if (num3 != 0)
			{
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneEquip("TUIEvent_WeaponChoose", num3));
			}
			popup_weapon_now = popup_weapon02;
			break;
		}
		case 4:
		{
			if (popup_weapon03.IsEmpty())
			{
				popup_go_buy.Show(PopupGoBuy.GoBuyType.Weapon);
				break;
			}
			popup_weapon03.Show();
			popup_weapon03.SetItemNowInfo(component);
			int num = popup_weapon03.BeforeItemSelect(component.GetInfo());
			if (num != 0)
			{
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneEquip("TUIEvent_WeaponChoose", num));
			}
			popup_weapon_now = popup_weapon03;
			break;
		}
		}
	}

	public void TUIEvent_PopupRoleSelect(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 1)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Drag");
			}
			BtnSelect_Item component = control.GetComponent<BtnSelect_Item>();
			popup_role.SetItemSelectInfo(component.GetInfo());
			component.DoChoose();
			popup_role.CheckNewMark();
			int texture_id = component.GetInfo().texture_id;
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneEquip("TUIEvent_RolesChoose", texture_id));
		}
	}

	public void TUIEvent_PopupSkillSelect(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 1)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Drag");
			}
			BtnSelect_Item component = control.GetComponent<BtnSelect_Item>();
			popup_skill.SetItemSelectInfo(component.GetInfo());
			component.DoChoose();
			popup_skill.CheckNewMark();
			int texture_id = component.GetInfo().texture_id;
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneEquip("TUIEvent_SkillChoose", texture_id));
		}
	}

	public void TUIEvent_PopupWeaponSelect(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 1)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Drag");
			}
			BtnSelect_Item component = control.GetComponent<BtnSelect_Item>();
			popup_weapon_now.SetItemSelectInfo(component.GetInfo());
			component.DoChoose();
			popup_weapon_now.CheckNewMark();
			int texture_id = component.GetInfo().texture_id;
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneEquip("TUIEvent_WeaponChoose", texture_id));
		}
	}

	public void TUIEvent_PopupRoleEquip(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Button");
				CUISound.GetInstance().Play("UI_Use");
			}
			if (popup_role.GetItemSelectInfo() != null)
			{
				int texture_id = popup_role.GetItemSelectInfo().texture_id;
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneEquip("TUIEvent_RoleEquip", texture_id));
			}
			popup_role.Hide();
		}
	}

	public void TUIEvent_PopupSkillEquip(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type != 3)
		{
			return;
		}
		if (sfx_open_now)
		{
			CUISound.GetInstance().Play("UI_Equip");
		}
		if (popup_skill.GetItemSelectInfo() != null)
		{
			if (popup_skill.IsExchangeSkill())
			{
				int index = popup_skill.GetExchangeItem01().GetIndex();
				int index2 = popup_skill.GetExchangeItem02().GetIndex();
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneEquip("TUIEvent_SkillExchange", index, index2));
			}
			else
			{
				int index3 = popup_skill.GetItemNowInfo().GetIndex();
				int texture_id = popup_skill.GetItemSelectInfo().texture_id;
				if (popup_skill.GetItemNowInfo().GetInfo() != null && popup_skill.GetItemNowInfo().GetInfo().texture_id == popup_skill.GetItemSelectInfo().texture_id)
				{
					global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneEquip("TUIEvent_SkillUnEquip", index3, texture_id));
				}
				else
				{
					global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneEquip("TUIEvent_SkillEquip", index3, texture_id));
				}
			}
		}
		popup_skill.Hide();
	}

	public void TUIEvent_PopupWeaponEquip(TUIControl control, int event_type, float wapram, float lparam, object data)
	{
		if (event_type != 3)
		{
			return;
		}
		if (sfx_open_now)
		{
			CUISound.GetInstance().Play("UI_Equip");
		}
		if (popup_weapon_now.GetItemSelectInfo() != null)
		{
			if (popup_weapon_now.IsExchangeWeapon())
			{
				int index = popup_weapon_now.GetExchangeItem01().GetIndex();
				int index2 = popup_weapon_now.GetExchangeItem02().GetIndex();
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneEquip("TUIEvent_WeaponExchange", index, index2));
			}
			else
			{
				int index3 = popup_weapon_now.GetItemNowInfo().GetIndex();
				int texture_id = popup_weapon_now.GetItemSelectInfo().texture_id;
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneEquip("TUIEvent_WeaponEquip", index3, texture_id));
			}
		}
		popup_weapon_now.Hide();
		popup_weapon_now = null;
	}

	public void TUIEvent_ClosePopupRole(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Cancle");
			}
			popup_role.Hide();
			popup_role.SetItemSelectInfo(null);
			popup_role.SetItemNowInfo(null);
		}
	}

	public void TUIEvent_ClosePopupProp(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Cancle");
			}
			popup_prop.Hide();
		}
	}

	public void TUIEvent_ClosePopupSkill01(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Cancle");
			}
			popup_skill01.Hide();
		}
	}

	public void TUIEvent_ClosePopupSkill(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Cancle");
			}
			popup_skill.Hide();
			popup_skill.SetItemSelectInfo(null);
			popup_skill.SetItemNowInfo(null);
		}
	}

	public void TUIEvent_ClosePopupWeapon(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Cancle");
			}
			popup_weapon_now.Hide();
			popup_weapon_now.SetItemSelectInfo(null);
			popup_weapon_now.SetItemNowInfo(null);
			popup_weapon_now = null;
		}
	}

	public void TUIEvent_MoveScreen(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 2)
		{
			go_role.SetRotation(wparam, lparam);
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
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneEquip("TUIEvent_Back"));
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
			//global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneEquip("TUIEvent_EnterIAP"));
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
			//global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneEquip("TUIEvent_EnterGold"));
		}
	}

	public void TUIEvent_CloseGoBuy(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Cancle");
			}
			popup_go_buy.Hide();
		}
	}

	public void TUIEvent_ClickGoBuy(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type != 3)
		{
			return;
		}
		if (sfx_open_now)
		{
			CUISound.GetInstance().Play("UI_Button");
		}
		if (control.transform.parent == null || control.transform.parent.parent == null)
		{
			Debug.Log("error!");
			return;
		}
		PopupGoBuy component = control.transform.parent.parent.GetComponent<PopupGoBuy>();
		if (component == null)
		{
			Debug.Log("error!");
		}
		else if (component.GetGoBuyType() == PopupGoBuy.GoBuyType.Weapon)
		{
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneEquip("TUIEvent_EnterGoBuyWeapon"));
		}
		else if (component.GetGoBuyType() == PopupGoBuy.GoBuyType.Skill)
		{
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneEquip("TUIEvent_EnterGoBuySkill"));
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
