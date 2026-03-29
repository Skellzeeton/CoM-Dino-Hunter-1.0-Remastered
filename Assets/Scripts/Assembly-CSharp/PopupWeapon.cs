using System.Collections.Generic;
using EventCenter;
using UnityEngine;

public class PopupWeapon : MonoBehaviour
{
	public Top_Bar top_bar;

	public Role_Control role_control;

	public WeaponKindItem weapon_kind_item;

	public LevelStars level_stars;

	public LabelInfo_Weapon label_info_weapon;

	public TUILabel label_title;

	public PopupWeaponUpdate popup_weapon_update;

	public PopupWeaponSupplement popup_weapon_supplement;

	public PopupWeaponBuy popup_weapon_buy;

	public UnlockBlink unlock_blink;

	public PopupGoldToCrystal popup_gold_to_crystal;

	public PopupCrystalNoEnough popup_crystal_no_enough;

	private ScrollList_WeaponItem item_choose;

	private int role_now_id;

	private GoodsNeedItemBuy btn_goods_buy;

	private TUIWeaponInfo weapon_info;

	private ScrollList_Weapon scroll_list_weapon_now;

	public ScrollList_Weapon prefab_scroll_list_weapon01;

	public ScrollList_Weapon prefab_scroll_list_weapon02;

	public ScrollList_Weapon prefab_scroll_list_weapon03;

	public ScrollList_Weapon prefab_scroll_list_weapon04;

	public ScrollList_Weapon prefab_scroll_list_weapon05;

	public ScrollList_Weapon prefab_scroll_list_weapon06;

	public ScrollList_Weapon prefab_scroll_list_weapon07;

	private ScrollList_Weapon scroll_list_weapon01;

	private ScrollList_Weapon scroll_list_weapon02;

	private ScrollList_Weapon scroll_list_weapon03;

	private ScrollList_Weapon scroll_list_weapon04;

	private ScrollList_Weapon scroll_list_weapon05;

	private ScrollList_Weapon scroll_list_weapon06;

	private ScrollList_Weapon scroll_list_weapon07;

	public Scene_Forge scene_forge;

	private TUISupplementInfo supplement_info;

	private TUIPriceInfo supplement_price_info;

	public PopupGoEquip popup_go_equip;

	public PopupTips popup_tips;

	private void Start()
	{
	}

	private void Update()
	{
		CheckScrollChoose();
	}

	public void SetWeaponInfo(TUIWeaponInfo m_weapon_info)
	{
		if (weapon_info == null)
		{
			weapon_info = new TUIWeaponInfo();
		}
		weapon_info = m_weapon_info;
		SetWeaponKindItem(WeaponType.CloseWeapon, null);
		NewMarkType type = CheckNewMark(weapon_info.weapon_list01, weapon_info.new_mark_list);
		weapon_kind_item.SetNewMark(1, type);
		NewMarkType type2 = CheckNewMark(weapon_info.weapon_list02, weapon_info.new_mark_list);
		weapon_kind_item.SetNewMark(2, type2);
		NewMarkType type3 = CheckNewMark(weapon_info.weapon_list03, weapon_info.new_mark_list);
		weapon_kind_item.SetNewMark(3, type3);
		NewMarkType type4 = CheckNewMark(weapon_info.weapon_list04, weapon_info.new_mark_list);
		weapon_kind_item.SetNewMark(4, type4);
		NewMarkType type5 = CheckNewMark(weapon_info.weapon_list05, weapon_info.new_mark_list);
		weapon_kind_item.SetNewMark(5, type5);
		NewMarkType type6 = CheckNewMark(weapon_info.weapon_list06, weapon_info.new_mark_list);
		weapon_kind_item.SetNewMark(6, type6);
		NewMarkType type7 = CheckNewMark(weapon_info.weapon_list07, weapon_info.new_mark_list);
		weapon_kind_item.SetNewMark(7, type7);
		if (m_weapon_info.open_link)
		{
			WeaponType weapon_link_type = m_weapon_info.weapon_link_type;
			int weapon_link_id = m_weapon_info.weapon_link_id;
			weapon_kind_item.SetSelectBtn(weapon_link_type);
			SetWeaponKindItem(weapon_link_type, weapon_kind_item.GetSelectBtn());
			scroll_list_weapon_now.SetItemChoose(weapon_link_id);
		}
	}

	public void UpdateNewMarkInfo(Dictionary<int, NewMarkType> m_new_mark_list)
	{
		if (weapon_info == null)
		{
			return;
		}
		if (weapon_info.new_mark_list == null)
		{
			weapon_info.new_mark_list = m_new_mark_list;
		}
		else
		{
			foreach (KeyValuePair<int, NewMarkType> item in m_new_mark_list)
			{
				weapon_info.new_mark_list[item.Key] = item.Value;
			}
		}
		NewMarkType type = CheckNewMark(weapon_info.weapon_list01, weapon_info.new_mark_list);
		weapon_kind_item.SetNewMark(1, type);
		NewMarkType type2 = CheckNewMark(weapon_info.weapon_list02, weapon_info.new_mark_list);
		weapon_kind_item.SetNewMark(2, type2);
		NewMarkType type3 = CheckNewMark(weapon_info.weapon_list03, weapon_info.new_mark_list);
		weapon_kind_item.SetNewMark(3, type3);
		NewMarkType type4 = CheckNewMark(weapon_info.weapon_list04, weapon_info.new_mark_list);
		weapon_kind_item.SetNewMark(4, type4);
		NewMarkType type5 = CheckNewMark(weapon_info.weapon_list05, weapon_info.new_mark_list);
		weapon_kind_item.SetNewMark(5, type5);
		NewMarkType type6 = CheckNewMark(weapon_info.weapon_list06, weapon_info.new_mark_list);
		weapon_kind_item.SetNewMark(6, type6);
		NewMarkType type7 = CheckNewMark(weapon_info.weapon_list07, weapon_info.new_mark_list);
		weapon_kind_item.SetNewMark(7, type7);
		if (scroll_list_weapon01 != null)
		{
			scroll_list_weapon01.UpdateNewMark(weapon_info.new_mark_list);
		}
		if (scroll_list_weapon02 != null)
		{
			scroll_list_weapon02.UpdateNewMark(weapon_info.new_mark_list);
		}
		if (scroll_list_weapon03 != null)
		{
			scroll_list_weapon03.UpdateNewMark(weapon_info.new_mark_list);
		}
		if (scroll_list_weapon04 != null)
		{
			scroll_list_weapon04.UpdateNewMark(weapon_info.new_mark_list);
		}
		if (scroll_list_weapon05 != null)
		{
			scroll_list_weapon05.UpdateNewMark(weapon_info.new_mark_list);
		}
		if (scroll_list_weapon06 != null)
		{
			scroll_list_weapon06.UpdateNewMark(weapon_info.new_mark_list);
		}
		if (scroll_list_weapon07 != null)
		{
			scroll_list_weapon07.UpdateNewMark(weapon_info.new_mark_list);
		}
	}

	public NewMarkType CheckNewMark(List<TUIWeaponAttributeInfo> m_weapon_list, Dictionary<int, NewMarkType> m_new_mark_list)
	{
		if (m_weapon_list == null || m_new_mark_list == null)
		{
			return NewMarkType.None;
		}
		bool flag = false;
		for (int i = 0; i < m_weapon_list.Count; i++)
		{
			int id = m_weapon_list[i].id;
			if (m_new_mark_list.ContainsKey(id))
			{
				NewMarkType newMarkType = m_new_mark_list[id];
				if (newMarkType == NewMarkType.New)
				{
					return NewMarkType.New;
				}
				if (m_new_mark_list[id] == NewMarkType.Mark)
				{
					flag = true;
				}
			}
		}
		if (flag)
		{
			return NewMarkType.Mark;
		}
		return NewMarkType.None;
	}

	public void CheckScrollChoose()
	{
		if (scroll_list_weapon_now == null)
		{
			return;
		}
		ScrollList_WeaponItem itemChoose = scroll_list_weapon_now.GetItemChoose();
		if (!(item_choose != itemChoose))
		{
			return;
		}
		if (scene_forge != null)
		{
			if (scene_forge.GetSFXOpen())
			{
				CUISound.GetInstance().Play("UI_Drag");
			}
		}
		else
		{
			Debug.Log("error!");
		}
		item_choose = itemChoose;
		if (item_choose != null)
		{
			SetInfo(item_choose.GetWeaponAttributeInfo());
			SetRoleWeapon(item_choose.GetWeaponAttributeInfo().id);
			int id = item_choose.GetWeaponAttributeInfo().id;
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneForge("TUIEvent_WeaponChoose", id));
		}
	}

	public void SetInfo(TUIWeaponAttributeInfo m_info)
	{
		if (m_info == null)
		{
			Debug.Log("error! no info!");
			return;
		}
		label_title.Text = m_info.name;
		float x = label_title.CalculateBounds(label_title.Text).size.x;
		Vector3 position = new Vector3(label_title.transform.localPosition.x + x - 4f, label_title.transform.localPosition.y, label_title.transform.localPosition.z);
		int level = m_info.level;
		TUIWeaponUpdateInfo weapon_update_info = m_info.weapon_update_info;
		if (weapon_update_info == null)
		{
			Debug.Log("error! no update info");
			return;
		}
		if (m_info.kind == WeaponType.Stoneskin)
		{
			string introduce = string.Empty;
			int hp = 0;
			if (weapon_update_info.level_hp != null && weapon_update_info.level_hp.ContainsKey((level == 0) ? 1 : level))
			{
				hp = weapon_update_info.level_hp[(level == 0) ? 1 : level];
				introduce = weapon_update_info.level_introduce_ex[(level == 0) ? 1 : level];
			}
			label_info_weapon.SetStoneskinInfo(introduce, hp);
		}
		else
		{
			int damage = 0;
			float fire_rate = 0f;
			int blast_radius = 0;
			int knockback = 0;
			int ammo = 0;
			if (weapon_update_info.level_damage != null && weapon_update_info.level_damage.ContainsKey((level == 0) ? 1 : level))
			{
				damage = weapon_update_info.level_damage[(level == 0) ? 1 : level];
			}
			if (weapon_update_info.level_fire_rate != null && weapon_update_info.level_fire_rate.ContainsKey((level == 0) ? 1 : level))
			{
				fire_rate = weapon_update_info.level_fire_rate[(level == 0) ? 1 : level];
			}
			if (weapon_update_info.level_blast_radius != null && weapon_update_info.level_blast_radius.ContainsKey((level == 0) ? 1 : level))
			{
				blast_radius = weapon_update_info.level_blast_radius[(level == 0) ? 1 : level];
			}
			if (weapon_update_info.level_knockback != null && weapon_update_info.level_knockback.ContainsKey((level == 0) ? 1 : level))
			{
				knockback = weapon_update_info.level_knockback[(level == 0) ? 1 : level];
			}
			if (weapon_update_info.level_ammo != null && weapon_update_info.level_ammo.ContainsKey((level == 0) ? 1 : level))
			{
				ammo = weapon_update_info.level_ammo[(level == 0) ? 1 : level];
			}
			label_info_weapon.SetWeaponInfo(damage, fire_rate, blast_radius, knockback, ammo);
		}
		level_stars.SetStars(level, position);
		if (level >= 5)
		{
			popup_weapon_buy.SetStateDisable();
		}
		else if (level <= 0)
		{
			popup_weapon_buy.SetStateCraft();
		}
		else
		{
			popup_weapon_buy.SetStateUpdate();
		}
	}

	public void OpenWeaponUpdate()
	{
		if (item_choose == null)
		{
			Debug.Log("error! no item_choose");
			return;
		}
		popup_weapon_update.ShowWeaponUpdate();
		popup_weapon_update.SetInfo(item_choose);
	}

	public void CloseWeaponUpdate()
	{
		popup_weapon_update.HideWeaponUpdate();
	}

	public void OpenWeaponSupplement()
	{
		if (popup_weapon_supplement == null)
		{
			Debug.Log("error!");
		}
		else
		{
			popup_weapon_supplement.Show();
		}
	}

	public void CloseWeaponSupplement()
	{
		if (popup_weapon_supplement == null)
		{
			Debug.Log("error!");
			return;
		}
		popup_weapon_supplement.Hide();
		supplement_info = null;
		supplement_price_info = null;
	}

	public void SetRoleID(int id)
	{
		role_now_id = id;
		role_control.ChangeRole(id);
		role_control.SetRoleFixedRotation(new Vector3(0f, -40f, 0f));
	}

	public void SetRoleWeapon(int id)
	{
		role_control.ChangeWeapon(id);
	}

	public void UpdateGoodsBuy(bool m_open_sfx)
	{
		int index = btn_goods_buy.GetIndex();
		int goodsID = btn_goods_buy.GetGoodsID();
		int goodsLackCount = btn_goods_buy.GetGoodsLackCount();
		int goodsPrice = btn_goods_buy.GetGoodsPrice();
		switch (btn_goods_buy.GetGoodsUnitType())
		{
		case UnitType.Gold:
		{
			int goldValue = top_bar.GetGoldValue();
			goldValue -= goodsPrice * goodsLackCount;
			if (goldValue < 0)
			{
				Debug.Log("error!you have no gold enough!");
				return;
			}
			top_bar.SetGoldValue(goldValue);
			if (m_open_sfx)
			{
				CUISound.GetInstance().Play("UI_Trade");
			}
			break;
		}
		case UnitType.Crystal:
		{
			int crystalValue = top_bar.GetCrystalValue();
			crystalValue -= goodsPrice * goodsLackCount;
			if (crystalValue < 0)
			{
				Debug.Log("error!you have no crystal enough!");
				return;
			}
			top_bar.SetCrystalValue(crystalValue);
			if (m_open_sfx)
			{
				CUISound.GetInstance().Play("UI_Crystal");
			}
			break;
		}
		default:
			Debug.Log("error!");
			return;
		}
		int count = item_choose.GetWeaponAttributeInfo().goods_list[goodsID].count;
		count += goodsLackCount;
		item_choose.GetWeaponAttributeInfo().goods_list[goodsID].SetCount(count);
		popup_weapon_update.UpdateGoodsBuy(index);
		Debug.Log("add goods:" + goodsID + " count:" + goodsLackCount + " remain:" + count);
	}

	public void UpdateWeaponBuy(bool m_open_sfx)
	{
		if (supplement_info == null || supplement_price_info == null)
		{
			Debug.Log("error!");
			return;
		}
		int price = supplement_price_info.price;
		switch (supplement_price_info.unit_type)
		{
		case UnitType.Gold:
		{
			int goldValue = top_bar.GetGoldValue();
			goldValue -= price;
			if (goldValue < 0)
			{
				Debug.Log("error!you have no gold enough!");
				return;
			}
			top_bar.SetGoldValue(goldValue);
			if (m_open_sfx)
			{
				CUISound.GetInstance().Play("UI_Trade");
			}
			break;
		}
		case UnitType.Crystal:
		{
			int crystalValue = top_bar.GetCrystalValue();
			crystalValue -= price;
			if (crystalValue < 0)
			{
				Debug.Log("error!you have no crystal enough!");
				return;
			}
			top_bar.SetCrystalValue(crystalValue);
			if (m_open_sfx)
			{
				CUISound.GetInstance().Play("UI_Crystal");
			}
			break;
		}
		}
		int price_value = supplement_info.price_value;
		UnitType price_unit = supplement_info.price_unit;
		if (price_value > 0)
		{
			switch (price_unit)
			{
			case UnitType.Gold:
			{
				int goldValue2 = top_bar.GetGoldValue();
				goldValue2 += price_value;
				top_bar.SetGoldValue(goldValue2);
				break;
			}
			case UnitType.Crystal:
			{
				int crystalValue2 = top_bar.GetCrystalValue();
				crystalValue2 += price_value;
				top_bar.SetCrystalValue(crystalValue2);
				break;
			}
			}
		}
		List<TUIGoodsSupplementInfo> goods_list = supplement_info.goods_list;
		if (goods_list != null)
		{
			for (int i = 0; i < goods_list.Count; i++)
			{
				TUIGoodsSupplementInfo tUIGoodsSupplementInfo = goods_list[i];
				if (tUIGoodsSupplementInfo != null)
				{
					TUIGoodsInfo tUIGoodsInfo = item_choose.GetWeaponAttributeInfo().goods_list[tUIGoodsSupplementInfo.id];
					if (tUIGoodsInfo != null)
					{
						int count = tUIGoodsInfo.count;
						count += tUIGoodsSupplementInfo.count;
						tUIGoodsInfo.SetCount(count);
					}
				}
			}
		}
		popup_weapon_update.SetInfo(item_choose);
	}

	public int GetWeaponID()
	{
		if (item_choose == null || item_choose.GetWeaponAttributeInfo() == null)
		{
			Debug.Log("error! no info!");
			return 0;
		}
		return item_choose.GetWeaponAttributeInfo().id;
	}

	public WeaponType GetWeaponType()
	{
		if (item_choose == null || item_choose.GetWeaponAttributeInfo() == null)
		{
			Debug.Log("error! no info!");
			return WeaponType.None;
		}
		return item_choose.GetWeaponAttributeInfo().kind;
	}

	public int GetWeaponLevel()
	{
		return item_choose.GetWeaponAttributeInfo().level;
	}

	public void UpdateWeapon(bool m_open_sfx)
	{
		TUIWeaponAttributeInfo weaponAttributeInfo = item_choose.GetWeaponAttributeInfo();
		int level = weaponAttributeInfo.level;
		int count = weaponAttributeInfo.weapon_update_info.level_price.Count;
		if (level >= count)
		{
			return;
		}
		int price = weaponAttributeInfo.weapon_update_info.level_price[level + 1].price;
		UnitType unit_type = weaponAttributeInfo.weapon_update_info.level_price[level + 1].unit_type;
		int num = 0;
		switch (unit_type)
		{
		case UnitType.Gold:
			num = top_bar.GetGoldValue();
			num -= price;
			if (num < 0)
			{
				Debug.Log("you have no gold enough!");
				return;
			}
			break;
		case UnitType.Crystal:
			num = top_bar.GetCrystalValue();
			num -= price;
			if (num < 0)
			{
				Debug.Log("you have no crystal enough!");
				return;
			}
			break;
		}
		List<TUIGoodsNeedInfo> goodsNeedInfo = weaponAttributeInfo.level_goods_need_info.GetGoodsNeedInfo(weaponAttributeInfo.level + 1);
		if (goodsNeedInfo != null)
		{
			for (int i = 0; i < goodsNeedInfo.Count; i++)
			{
				int goods_id = goodsNeedInfo[i].goods_id;
				int need_count = goodsNeedInfo[i].need_count;
				GoodsQualityType goods_quality = goodsNeedInfo[i].goods_quality;
				int num2 = 0;
				if (goods_quality == weaponAttributeInfo.goods_list[goods_id].quality)
				{
					num2 = weaponAttributeInfo.goods_list[goods_id].count;
				}
				num2 -= need_count;
				if (num2 < 0)
				{
					Debug.Log("you have no goods enough!");
					return;
				}
			}
			for (int j = 0; j < goodsNeedInfo.Count; j++)
			{
				int goods_id2 = goodsNeedInfo[j].goods_id;
				int need_count2 = goodsNeedInfo[j].need_count;
				GoodsQualityType goods_quality2 = goodsNeedInfo[j].goods_quality;
				int num3 = 0;
				if (goods_quality2 == weaponAttributeInfo.goods_list[goods_id2].quality)
				{
					num3 = weaponAttributeInfo.goods_list[goods_id2].count;
				}
				num3 -= need_count2;
				Debug.Log("cost goods:" + goods_id2 + " count:" + need_count2 + " remain:" + num3);
				weaponAttributeInfo.goods_list[goods_id2].SetCount(num3);
			}
		}
		weaponAttributeInfo.level++;
		if (weaponAttributeInfo == null)
		{
			Debug.Log("error! no attribute info");
			return;
		}
		int level2 = weaponAttributeInfo.level;
		TUIWeaponUpdateInfo weapon_update_info = weaponAttributeInfo.weapon_update_info;
		if (weaponAttributeInfo.kind == WeaponType.Stoneskin)
		{
			string introduce = string.Empty;
			int hp = 0;
			if (weapon_update_info.level_hp != null && weapon_update_info.level_hp.ContainsKey((level2 == 0) ? 1 : level2))
			{
				hp = weapon_update_info.level_hp[(level2 == 0) ? 1 : level2];
				introduce = weapon_update_info.level_introduce_ex[(level2 == 0) ? 1 : level2];
			}
			label_info_weapon.SetStoneskinInfo(introduce, hp);
		}
		else
		{
			int damage = 0;
			float fire_rate = 0f;
			int blast_radius = 0;
			int knockback = 0;
			int ammo = 0;
			if (weapon_update_info.level_damage != null && weapon_update_info.level_damage.ContainsKey((level2 == 0) ? 1 : level2))
			{
				damage = weapon_update_info.level_damage[(level2 == 0) ? 1 : level2];
			}
			if (weapon_update_info.level_fire_rate != null && weapon_update_info.level_fire_rate.ContainsKey((level2 == 0) ? 1 : level2))
			{
				fire_rate = weapon_update_info.level_fire_rate[(level2 == 0) ? 1 : level2];
			}
			if (weapon_update_info.level_blast_radius != null && weapon_update_info.level_blast_radius.ContainsKey((level2 == 0) ? 1 : level2))
			{
				blast_radius = weapon_update_info.level_blast_radius[(level2 == 0) ? 1 : level2];
			}
			if (weapon_update_info.level_knockback != null && weapon_update_info.level_knockback.ContainsKey((level2 == 0) ? 1 : level2))
			{
				knockback = weapon_update_info.level_knockback[(level2 == 0) ? 1 : level2];
			}
			if (weapon_update_info.level_ammo != null && weapon_update_info.level_ammo.ContainsKey((level2 == 0) ? 1 : level2))
			{
				ammo = weapon_update_info.level_ammo[(level2 == 0) ? 1 : level2];
			}
			if (level2 == 1)
			{
				label_info_weapon.SetWeaponInfo(damage, fire_rate, blast_radius, knockback, ammo);
			}
			else
			{
				label_info_weapon.SetWeaponInfo(fire_rate, blast_radius, knockback, ammo);
			}
		}
		if (level2 >= count)
		{
			popup_weapon_buy.SetStateDisable();
		}
		if (level2 == 1)
		{
			popup_weapon_buy.SetStateUpdate();
			unlock_blink.OpenBlinkWeapon(weaponAttributeInfo.id, "Purchase complete!");
		}
		else if (level2 < count)
		{
			popup_weapon_buy.SetStateUpdate();
			unlock_blink.OpenBlinkWeapon(weaponAttributeInfo.id, "Upgrade complete!");
		}
		else if (level2 == count)
		{
			popup_weapon_buy.SetStateDisable();
			unlock_blink.OpenBlinkWeapon(weaponAttributeInfo.id, "Upgrade complete!");
		}
		else
		{
			Debug.Log("error!" + level2);
		}
		if (scene_forge != null)
		{
			if (scene_forge.GetSFXOpen())
			{
				CUISound.GetInstance().Play("UI_Unlocked_weapon");
				CUISound.GetInstance().Play("UI_Forge_craft");
			}
		}
		else
		{
			Debug.Log("error!");
		}
		switch (unit_type)
		{
		case UnitType.Gold:
			top_bar.SetGoldValue(num);
			if (m_open_sfx)
			{
				CUISound.GetInstance().Play("UI_Trade");
			}
			break;
		case UnitType.Crystal:
			top_bar.SetCrystalValue(num);
			if (m_open_sfx)
			{
				CUISound.GetInstance().Play("UI_Crystal");
			}
			break;
		}
	}

	public bool EnableUpdateWeapon()
	{
		bool flag = true;
		if (supplement_info == null)
		{
			supplement_info = new TUISupplementInfo();
		}
		TUIWeaponAttributeInfo weaponAttributeInfo = item_choose.GetWeaponAttributeInfo();
		int level = weaponAttributeInfo.level;
		int count = weaponAttributeInfo.weapon_update_info.level_price.Count;
		if (level < count)
		{
			int price = weaponAttributeInfo.weapon_update_info.level_price[level + 1].price;
			UnitType unit_type = weaponAttributeInfo.weapon_update_info.level_price[level + 1].unit_type;
			int num = 0;
			switch (unit_type)
			{
			case UnitType.Gold:
				num = top_bar.GetGoldValue();
				num -= price;
				if (num < 0)
				{
					Debug.Log("you have no gold enough!");
					flag = false;
					supplement_info.SetPriceInfo(Mathf.Abs(num), UnitType.Gold);
				}
				break;
			case UnitType.Crystal:
				num = top_bar.GetCrystalValue();
				num -= price;
				if (num < 0)
				{
					Debug.Log("you have no crystal enough!");
					flag = false;
					supplement_info.SetPriceInfo(Mathf.Abs(num), UnitType.Crystal);
				}
				break;
			}
			List<TUIGoodsNeedInfo> goodsNeedInfo = weaponAttributeInfo.level_goods_need_info.GetGoodsNeedInfo(weaponAttributeInfo.level + 1);
			if (goodsNeedInfo != null)
			{
				for (int i = 0; i < goodsNeedInfo.Count; i++)
				{
					int goods_id = goodsNeedInfo[i].goods_id;
					int need_count = goodsNeedInfo[i].need_count;
					GoodsQualityType goods_quality = goodsNeedInfo[i].goods_quality;
					int num2 = 0;
					if (goods_quality == weaponAttributeInfo.goods_list[goods_id].quality)
					{
						num2 = weaponAttributeInfo.goods_list[goods_id].count;
					}
					num2 -= need_count;
					if (num2 < 0)
					{
						Debug.Log("you have no goods enough!");
						flag = false;
						supplement_info.AddSupplementGoods(new TUIGoodsSupplementInfo(goods_id, Mathf.Abs(num2), goods_quality));
					}
				}
			}
		}
		if (!flag && popup_weapon_supplement != null)
		{
			popup_weapon_supplement.SetSupplementInfo(supplement_info);
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneForge("TUIEvent_WeaponSuppplementPrice", supplement_info));
		}
		return flag;
	}

	public TUISupplementInfo GetSupplementInfo()
	{
		return supplement_info;
	}

	public void SetSupplementBtnInfo(TUIPriceInfo m_info)
	{
		popup_weapon_supplement.SetSupplementBtnInfo(m_info);
		supplement_price_info = m_info;
	}

	public void SetTopBarInfo(TUIPlayerInfo m_player_info)
	{
		if (m_player_info == null)
		{
			Debug.Log("error! no found info");
			return;
		}
		int avatar_id = m_player_info.avatar_id;
		int level = m_player_info.level;
		int exp = m_player_info.exp;
		int level_exp = m_player_info.level_exp;
		int gold = m_player_info.gold;
		int crystal = m_player_info.crystal;
		int avatar_id2 = m_player_info.avatar_id;
		top_bar.SetAllValue(level, exp, level_exp, gold, crystal, avatar_id2);
		SetRoleID(avatar_id);
	}

	public void SetGoodsNeedItemBuy(GoodsNeedItemBuy m_item)
	{
		btn_goods_buy = m_item;
	}

	public GoodsNeedItemBuy GetGoodsNeedItemBuy()
	{
		return btn_goods_buy;
	}

	public void CloseBlink()
	{
		unlock_blink.CloseBlink();
	}

	public void StarsBlink()
	{
		if (item_choose == null)
		{
			Debug.Log("error!");
			return;
		}
		TUIWeaponAttributeInfo weaponAttributeInfo = item_choose.GetWeaponAttributeInfo();
		if (weaponAttributeInfo == null)
		{
			Debug.Log("error!");
			return;
		}
		float x = label_title.CalculateBounds(label_title.Text).size.x;
		Vector3 position = new Vector3(label_title.transform.localPosition.x + x - 4f, label_title.transform.localPosition.y, label_title.transform.localPosition.z);
		level_stars.SetStars(weaponAttributeInfo.level, position, weaponAttributeInfo.level);
		if (scene_forge != null)
		{
			if (scene_forge.GetSFXOpen())
			{
				CUISound.GetInstance().Play("UI_Levelup");
			}
		}
		else
		{
			Debug.Log("error!");
		}
	}

	public void OpenValueAnimation()
	{
		TUIWeaponAttributeInfo weaponAttributeInfo = item_choose.GetWeaponAttributeInfo();
		if (weaponAttributeInfo == null || label_info_weapon == null)
		{
			Debug.Log("error!");
			return;
		}
		int level = weaponAttributeInfo.level;
		TUIWeaponUpdateInfo weapon_update_info = weaponAttributeInfo.weapon_update_info;
		if (weapon_update_info == null)
		{
			Debug.Log("error!");
		}
		else if (weaponAttributeInfo.kind == WeaponType.Stoneskin)
		{
			int hP = weapon_update_info.level_hp[(level == 0) ? 1 : level];
			label_info_weapon.SetHP(hP);
			label_info_weapon.OpenHPAnimation();
		}
		else
		{
			int damage = weapon_update_info.level_damage[(level == 0) ? 1 : level];
			label_info_weapon.SetDamage(damage);
			label_info_weapon.OpenDamageAnimation();
		}
	}

	public void SetRoleRotation(float wparam, float lparam)
	{
		role_control.SetRotation(wparam, lparam);
	}

	public void SetWeaponKindItem(WeaponType m_type, TUIControl m_control)
	{
		weapon_kind_item.SetSelectBtn(m_type);
		if (scroll_list_weapon_now != null)
		{
			scroll_list_weapon_now.Hide();
			item_choose = null;
		}
		if (weapon_info == null)
		{
			Debug.Log("no weapon_info!");
			return;
		}
		if (m_type == WeaponType.CloseWeapon && m_control == null)
		{
			m_control = weapon_kind_item.GetSelectBtn();
		}
		Vector3 normal_scroll_pos = new Vector3(84f, 15f, -3f);
		switch (m_type)
		{
		case WeaponType.CloseWeapon:
			SetWeaponKindItemEx(ref scroll_list_weapon01, prefab_scroll_list_weapon01, weapon_info.weapon_list01, normal_scroll_pos, weapon_info.new_mark_list, m_control);
			break;
		case WeaponType.Crossbow:
			SetWeaponKindItemEx(ref scroll_list_weapon02, prefab_scroll_list_weapon02, weapon_info.weapon_list02, normal_scroll_pos, weapon_info.new_mark_list, m_control);
			break;
		case WeaponType.MachineGun:
			SetWeaponKindItemEx(ref scroll_list_weapon03, prefab_scroll_list_weapon03, weapon_info.weapon_list03, normal_scroll_pos, weapon_info.new_mark_list, m_control);
			break;
		case WeaponType.ViolenceGun:
			SetWeaponKindItemEx(ref scroll_list_weapon04, prefab_scroll_list_weapon04, weapon_info.weapon_list04, normal_scroll_pos, weapon_info.new_mark_list, m_control);
			break;
		case WeaponType.LiquidFireGun:
			SetWeaponKindItemEx(ref scroll_list_weapon05, prefab_scroll_list_weapon05, weapon_info.weapon_list05, normal_scroll_pos, weapon_info.new_mark_list, m_control);
			break;
		case WeaponType.RPG:
			SetWeaponKindItemEx(ref scroll_list_weapon06, prefab_scroll_list_weapon06, weapon_info.weapon_list06, normal_scroll_pos, weapon_info.new_mark_list, m_control);
			break;
		case WeaponType.Stoneskin:
			SetWeaponKindItemEx(ref scroll_list_weapon07, prefab_scroll_list_weapon07, weapon_info.weapon_list07, normal_scroll_pos, weapon_info.new_mark_list, m_control);
			break;
		}
	}

	private void SetWeaponKindItemEx(ref ScrollList_Weapon m_scrolllist_weapon, ScrollList_Weapon m_prefab_scrolllist_weapon, List<TUIWeaponAttributeInfo> m_weapon_list, Vector3 m_normal_scroll_pos, Dictionary<int, NewMarkType> m_new_mark_list, TUIControl m_control)
	{
		if (m_scrolllist_weapon == null && m_weapon_list != null)
		{
			if (m_prefab_scrolllist_weapon == null)
			{
				Debug.Log("error!");
				return;
			}
			GameObject gameObject = (GameObject)Object.Instantiate(m_prefab_scrolllist_weapon.gameObject);
			m_scrolllist_weapon = gameObject.GetComponent<ScrollList_Weapon>();
			m_scrolllist_weapon.transform.parent = base.transform.parent;
			m_scrolllist_weapon.transform.localPosition = m_normal_scroll_pos;
			m_scrolllist_weapon.AddScrollListItem(m_weapon_list, m_new_mark_list, m_control);
		}
		scroll_list_weapon_now = m_scrolllist_weapon;
		if (scroll_list_weapon_now != null)
		{
			scroll_list_weapon_now.Show();
		}
	}

	public void ShowPopupGoldToCrystal(int m_gold, int m_crystal)
	{
		if (popup_gold_to_crystal != null)
		{
			string title = "You need more gold";
			string introduce = "Buy the missing " + m_gold + " Gold?";
			popup_gold_to_crystal.Show();
			popup_gold_to_crystal.SetInfo(title, introduce, m_gold, m_crystal, UnitType.Crystal);
		}
	}

	public void HidePopupGoldToCrystal()
	{
		if (popup_gold_to_crystal != null)
		{
			popup_gold_to_crystal.Hide();
		}
	}

	public int GetGoldExchangeCount()
	{
		if (popup_gold_to_crystal != null)
		{
			return popup_gold_to_crystal.GetGoldExchangeCount();
		}
		return 0;
	}

	public int GetCrystalExchangeCount()
	{
		if (popup_gold_to_crystal != null)
		{
			return popup_gold_to_crystal.GetCrystalExchangeCount();
		}
		return 0;
	}

	public void DoGoldExchange()
	{
		if (popup_gold_to_crystal == null || top_bar == null)
		{
			Debug.Log("error!");
			return;
		}
		int goldExchangeCount = popup_gold_to_crystal.GetGoldExchangeCount();
		int crystalExchangeCount = popup_gold_to_crystal.GetCrystalExchangeCount();
		int goldValue = top_bar.GetGoldValue();
		int crystalValue = top_bar.GetCrystalValue();
		goldValue += goldExchangeCount;
		crystalValue -= crystalExchangeCount;
		if (crystalValue < 0)
		{
			Debug.Log("error!");
			return;
		}
		top_bar.SetGoldValue(goldValue);
		top_bar.SetCrystalValue(crystalValue);
	}

	public void ShowPopupCrystalNoEnough(int m_crystal)
	{
		if (popup_crystal_no_enough != null)
		{
			string title = "you're " + m_crystal + " crystals short";
			string introduce = "Get more now?";
			//popup_crystal_no_enough.Show();
			//popup_crystal_no_enough.SetInfo(title, introduce, m_crystal, "OK");
		}
	}

	public void HidePopupCrystalNoEnough()
	{
		if (popup_crystal_no_enough != null)
		{
			popup_crystal_no_enough.Hide();
		}
	}

	public int GetCrystalNoEnoughCount()
	{
		if (popup_crystal_no_enough != null)
		{
			return popup_crystal_no_enough.GetCrystalNoEnoughCount();
		}
		return 0;
	}

	public void ShowGoEquip()
	{
		if (popup_go_equip != null)
		{
			popup_go_equip.Show();
		}
	}

	public void HideGoEquip()
	{
		if (popup_go_equip != null)
		{
			popup_go_equip.Hide();
		}
	}

	public void ShowGoEquipAfterBuy(bool m_sfx_open_now)
	{
		TUIWeaponAttributeInfo weaponAttributeInfo = item_choose.GetWeaponAttributeInfo();
		if (weaponAttributeInfo == null)
		{
			Debug.Log("error!");
			return;
		}
		if (m_sfx_open_now)
		{
			CUISound.GetInstance().Play("UI_Button");
		}
		if (weaponAttributeInfo.level == 1)
		{
			ShowGoEquip();
		}
	}

	public void ShowTips(TUIControl m_control)
	{
		if (popup_tips == null || m_control == null)
		{
			Debug.Log("error!");
			return;
		}
		GoodsNeedItemImg component = m_control.GetComponent<GoodsNeedItemImg>();
		if (component != null)
		{
			string goodsName = component.GetGoodsName();
			popup_tips.SetInfo(goodsName, m_control.transform.position, PopupTips.TipsPivot.TopRight);
		}
	}

	public void HideTips()
	{
		if (popup_tips == null)
		{
			Debug.Log("error!");
		}
		else
		{
			popup_tips.Hide();
		}
	}
}
