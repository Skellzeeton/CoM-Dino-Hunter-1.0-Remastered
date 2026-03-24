using EventCenter;
using UnityEngine;

public class Scene_Forge : MonoBehaviour
{
	public TUIFade m_fade;

	private float m_fade_in_time;

	private float m_fade_out_time;

	private bool do_fade_in;

	private bool is_fade_out;

	private bool do_fade_out;

	private string next_scene = string.Empty;

	public PopupWeapon popup_weapon;

	private bool sfx_open_now = true;

	private bool music_open_now = true;

	private void Awake()
	{
		TUIDataServer.Instance().Initialize();
		global::EventCenter.EventCenter.Instance.Register<TUIEvent.BackEvent_SceneForge>(TUIEvent_SetUIInfo);
	}

	private void Start()
	{
		global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneForge("TUIEvent_TopBar"));
		global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneForge("TUIEvent_WeaponInfo"));
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
		global::EventCenter.EventCenter.Instance.Unregister<TUIEvent.BackEvent_SceneForge>(TUIEvent_SetUIInfo);
	}

	public void TUIEvent_SetUIInfo(object sender, TUIEvent.BackEvent_SceneForge m_event)
	{
		if (m_event.GetEventName() == "TUIEvent_TopBar")
		{
			if (m_event.GetEventInfo().GetPlayerInfo() != null)
			{
				popup_weapon.SetTopBarInfo(m_event.GetEventInfo().GetPlayerInfo());
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
		else if (m_event.GetEventName() == "TUIEvent_WeaponInfo")
		{
			if (m_event.GetEventInfo() != null)
			{
				popup_weapon.SetWeaponInfo(m_event.GetEventInfo().weapon_info);
			}
			else
			{
				Debug.Log("error!");
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_WeaponGoodsBuy")
		{
			if (m_event.GetControlSuccess())
			{
				popup_weapon.UpdateGoodsBuy(sfx_open_now);
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
				popup_weapon.ShowPopupGoldToCrystal(wparam2, crystal);
				break;
			}
			case BackEventFalseType.NoCrystalEnough:
			{
				int wparam = m_event.GetWparam();
				popup_weapon.ShowPopupCrystalNoEnough(wparam);
				break;
			}
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_WeaponSuppplement")
		{
			if (m_event.GetControlSuccess())
			{
				popup_weapon.UpdateWeaponBuy(sfx_open_now);
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
				popup_weapon.ShowPopupGoldToCrystal(wparam4, crystal2);
				break;
			}
			case BackEventFalseType.NoCrystalEnough:
			{
				int wparam3 = m_event.GetWparam();
				popup_weapon.ShowPopupCrystalNoEnough(wparam3);
				break;
			}
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_WeaponUpdate")
		{
			if (m_event.GetControlSuccess())
			{
				popup_weapon.UpdateWeapon(sfx_open_now);
				popup_weapon.CloseWeaponUpdate();
				return;
			}
			switch (m_event.GetFalseType())
			{
			case BackEventFalseType.NoGoldEnough:
			{
				int wparam6 = m_event.GetWparam();
				int crystal3 = 0;
				TUIMappingInfo.GoldToCrystal goldToCrystalFunc3 = TUIMappingInfo.Instance().GetGoldToCrystalFunc();
				if (goldToCrystalFunc3 != null)
				{
					crystal3 = goldToCrystalFunc3(wparam6);
				}
				popup_weapon.ShowPopupGoldToCrystal(wparam6, crystal3);
				break;
			}
			case BackEventFalseType.NoCrystalEnough:
			{
				int wparam5 = m_event.GetWparam();
				popup_weapon.ShowPopupCrystalNoEnough(wparam5);
				break;
			}
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_NewMarkInfo")
		{
			if (m_event.GetEventInfo() != null && m_event.GetEventInfo().weapon_info != null)
			{
				TUIWeaponInfo weapon_info = m_event.GetEventInfo().weapon_info;
				if (weapon_info != null && weapon_info.new_mark_list != null)
				{
					popup_weapon.UpdateNewMarkInfo(weapon_info.new_mark_list);
				}
			}
			else
			{
				Debug.Log("error!");
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_WeaponSuppplementPrice")
		{
			if (m_event.GetSupplementInfo() != null)
			{
				popup_weapon.SetSupplementBtnInfo(m_event.GetSupplementInfo());
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
				popup_weapon.DoGoldExchange();
				return;
			}
			BackEventFalseType falseType = m_event.GetFalseType();
			if (falseType == BackEventFalseType.NoCrystalEnough)
			{
				int wparam7 = m_event.GetWparam();
				popup_weapon.ShowPopupCrystalNoEnough(wparam7);
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
		else if (m_event.GetEventName() == "TUIEvent_SearchGoodsDrop")
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
	}

	public void TUIEvent_OpenWeaponItem01(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 1)
		{
			if (popup_weapon == null)
			{
				Debug.Log("error!");
			}
			else
			{
				popup_weapon.SetWeaponKindItem(WeaponType.CloseWeapon, control);
			}
		}
	}

	public void TUIEvent_OpenWeaponItem02(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 1)
		{
			if (popup_weapon == null)
			{
				Debug.Log("error!");
			}
			else
			{
				popup_weapon.SetWeaponKindItem(WeaponType.Crossbow, control);
			}
		}
	}

	public void TUIEvent_OpenWeaponItem03(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 1)
		{
			if (popup_weapon == null)
			{
				Debug.Log("error!");
			}
			else
			{
				popup_weapon.SetWeaponKindItem(WeaponType.MachineGun, control);
			}
		}
	}

	public void TUIEvent_OpenWeaponItem04(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 1)
		{
			if (popup_weapon == null)
			{
				Debug.Log("error!");
			}
			else
			{
				popup_weapon.SetWeaponKindItem(WeaponType.ViolenceGun, control);
			}
		}
	}

	public void TUIEvent_OpenWeaponItem05(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 1)
		{
			if (popup_weapon == null)
			{
				Debug.Log("error!");
			}
			else
			{
				popup_weapon.SetWeaponKindItem(WeaponType.LiquidFireGun, control);
			}
		}
	}

	public void TUIEvent_OpenWeaponItem06(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 1)
		{
			if (popup_weapon == null)
			{
				Debug.Log("error!");
			}
			else
			{
				popup_weapon.SetWeaponKindItem(WeaponType.RPG, control);
			}
		}
	}

	public void TUIEvent_OpenWeaponItem07(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 1)
		{
			if (popup_weapon == null)
			{
				Debug.Log("error!");
			}
			else
			{
				popup_weapon.SetWeaponKindItem(WeaponType.Stoneskin, control);
			}
		}
	}

	public void TUIEvent_MoveScreen(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 2)
		{
			popup_weapon.SetRoleRotation(wparam, lparam);
		}
	}

	public void TUIEvent_OpenWeaponUpdate(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Button");
			}
			PopupWeaponBuy.PopupWeaponBuyState state = control.GetComponent<PopupWeaponBuy>().GetState();
			if (state == PopupWeaponBuy.PopupWeaponBuyState.State_Update || state == PopupWeaponBuy.PopupWeaponBuyState.State_Craft)
			{
				popup_weapon.OpenWeaponUpdate();
			}
		}
	}

	public void TUIEvent_CloseWeaponUpdate(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Cancle");
			}
			popup_weapon.CloseWeaponUpdate();
		}
	}

	public void TUIEvent_WeaponGoodsBuy(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Button");
			}
			GoodsNeedItemBuy component = control.GetComponent<GoodsNeedItemBuy>();
			int goodsID = component.GetGoodsID();
			int goodsQuality = (int)component.GetGoodsQuality();
			int goodsLackCount = component.GetGoodsLackCount();
			popup_weapon.SetGoodsNeedItemBuy(component);
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneForge("TUIEvent_WeaponGoodsBuy", goodsID, goodsQuality, goodsLackCount));
		}
	}

	public void TUIEvent_WeaponUpdate(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Button");
			}
			if (!popup_weapon.EnableUpdateWeapon())
			{
				popup_weapon.OpenWeaponSupplement();
				return;
			}
			int weaponID = popup_weapon.GetWeaponID();
			int weaponType = (int)popup_weapon.GetWeaponType();
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneForge("TUIEvent_WeaponUpdate", weaponID, weaponType));
			popup_weapon.CloseWeaponUpdate();
		}
	}

	public void TUIEvent_ClickSupplement(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Button");
			}
			return;
			/*TUISupplementInfo supplementInfo = popup_weapon.GetSupplementInfo();
			if (supplementInfo == null)
			{
				Debug.Log("error!");
				return;
			}
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneForge("TUIEvent_WeaponSuppplement", supplementInfo));
			popup_weapon.CloseWeaponSupplement();*/
		}
	}

	public void TUIEvent_CloseWeaponSupplement(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Button");
			}
			popup_weapon.CloseWeaponSupplement();
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
			popup_weapon.CloseBlink();
			popup_weapon.StarsBlink();
			popup_weapon.OpenValueAnimation();
			popup_weapon.ShowGoEquipAfterBuy(sfx_open_now);
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
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneForge("TUIEvent_Back"));
		}
	}

	public void TUIEvent_SearchGoodsDrop(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Button");
			}
			GoodsNeedItem component = control.transform.parent.GetComponent<GoodsNeedItem>();
			if (component == null)
			{
				Debug.Log("error! no goods need item");
				return;
			}
			int goodsID = component.GetGoodsID();
			int goodsQuality = (int)component.GetGoodsQuality();
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneForge("TUIEvent_SearchGoodsDrop", goodsID, goodsQuality));
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
			//global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneForge("TUIEvent_EnterIAP"));
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
			//global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneForge("TUIEvent_EnterGold"));
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
		if (control.transform.parent == null || control.transform.parent.parent == null)
		{
			Debug.Log("error!");
			return;
		}
		int wparam2 = 0;
		PopupGoldToCrystal component = control.transform.parent.parent.GetComponent<PopupGoldToCrystal>();
		if (component != null)
		{
			wparam2 = component.GetGoldExchangeCount();
		}
		global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneForge("TUIEvent_GoldToCrystal", wparam2));
		popup_weapon.HidePopupGoldToCrystal();
	}

	public void TUIEvent_CloseGoldToCrystal(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 3)
		{
			if (sfx_open_now)
			{
				CUISound.GetInstance().Play("UI_Cancle");
			}
			popup_weapon.HidePopupGoldToCrystal();
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
			popup_weapon.HidePopupCrystalNoEnough();
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
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneForge("TUIEvent_EnterIAPCrystalNoEnough"));
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
			popup_weapon.HideGoEquip();
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
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneForge("TUIEvent_EnterGoEquip"));
			popup_weapon.HideGoEquip();
		}
	}

	public void TUIEvent_ShowGoodsTips(TUIControl control, int event_type, float wparam, float lparam, object data)
	{
		if (event_type == 1)
		{
			popup_weapon.ShowTips(control);
		}
		else
		{
			popup_weapon.HideTips();
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
