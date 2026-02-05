using System.Collections.Generic;
using UnityEngine;

public class PopupWeaponUpdate : MonoBehaviour
{
	public TUILabel label_title;

	public TUILabel label_introduce;

	public LevelStars level_stars;

	public PopupWeaponUpdateBuy btn_buy;

	public GoodsNeedItem goods_need_item01;

	public GoodsNeedItem goods_need_item02;

	public GoodsNeedItem goods_need_item03;

	private int goods_kind;

	private void Start()
	{
		goods_need_item01.SetIndex(1);
		goods_need_item02.SetIndex(2);
		goods_need_item03.SetIndex(3);
	}

	private void Update()
	{
	}

	public void ShowWeaponUpdate()
	{
		base.gameObject.transform.localPosition = new Vector3(0f, 0f, base.gameObject.transform.localPosition.z);
		base.gameObject.GetComponent<Animation>().Play();
	}

	public void HideWeaponUpdate()
	{
		base.gameObject.transform.localPosition = new Vector3(0f, -1000f, base.gameObject.transform.localPosition.z);
	}

	public void SetInfo(ScrollList_WeaponItem m_item)
	{
		if (m_item == null)
		{
			Debug.Log("error! no item");
			return;
		}
		int level = m_item.GetWeaponAttributeInfo().level;
		if (level >= 5)
		{
			Debug.Log("error!you reach max level!");
			return;
		}
		label_title.Text = m_item.GetWeaponAttributeInfo().name;
		label_introduce.Text = m_item.GetWeaponAttributeInfo().weapon_update_info.level_introduce[level + 1];
		int price = m_item.GetWeaponAttributeInfo().weapon_update_info.level_price[level + 1].price;
		UnitType unit_type = m_item.GetWeaponAttributeInfo().weapon_update_info.level_price[level + 1].unit_type;
		btn_buy.SetBtnText(price, unit_type);
		float x = label_title.CalculateBounds(label_title.Text).size.x;
		Vector3 position = new Vector3(label_title.transform.localPosition.x + x - 4f, level_stars.transform.localPosition.y, label_title.transform.localPosition.z);
		level_stars.SetStars(level, position);
		TUILevelGoodsNeedInfo tUILevelGoodsNeedInfo = null;
		List<TUIGoodsNeedInfo> list = null;
		Dictionary<int, TUIGoodsInfo> dictionary = null;
		dictionary = m_item.GetWeaponAttributeInfo().goods_list;
		tUILevelGoodsNeedInfo = m_item.GetWeaponAttributeInfo().level_goods_need_info;
		if (tUILevelGoodsNeedInfo == null)
		{
			Debug.Log("error! no goods need info");
			return;
		}
		switch (level)
		{
		case 0:
			list = tUILevelGoodsNeedInfo.level_goods_need01;
			break;
		case 1:
			list = tUILevelGoodsNeedInfo.level_goods_need02;
			break;
		case 2:
			list = tUILevelGoodsNeedInfo.level_goods_need03;
			break;
		case 3:
			list = tUILevelGoodsNeedInfo.level_goods_need04;
			break;
		case 4:
			list = tUILevelGoodsNeedInfo.level_goods_need05;
			break;
		case 5:
			list = null;
			break;
		}
		if (list == null)
		{
			goods_need_item01.HideGoodsNeedItem();
			goods_need_item02.HideGoodsNeedItem();
			goods_need_item03.HideGoodsNeedItem();
			return;
		}
		goods_kind = list.Count;
		switch (goods_kind)
		{
		case 0:
			Debug.Log("you sure update weapon without material?");
			goods_need_item01.HideGoodsNeedItem();
			goods_need_item02.HideGoodsNeedItem();
			goods_need_item03.HideGoodsNeedItem();
			break;
		case 1:
		{
			int need_count4 = list[0].need_count;
			GoodsQualityType goods_quality4 = list[0].goods_quality;
			TUIGoodsInfo tUIGoodsInfo4 = null;
			if (dictionary.ContainsKey(list[0].goods_id))
			{
				tUIGoodsInfo4 = dictionary[list[0].goods_id];
				int goods_now_count = 0;
				if (goods_quality4 == tUIGoodsInfo4.quality)
				{
					goods_now_count = tUIGoodsInfo4.count;
				}
				int price5 = tUIGoodsInfo4.price_info.price;
				UnitType unit_type5 = tUIGoodsInfo4.price_info.unit_type;
				int goods_id4 = list[0].goods_id;
				string goods_name4 = list[0].goods_name;
				goods_need_item01.ShowGoodsNeedItem(goods_now_count, goods_quality4, need_count4, price5, goods_id4, unit_type5, goods_name4);
				goods_need_item02.HideGoodsNeedItem();
				goods_need_item03.HideGoodsNeedItem();
			}
			else
			{
				Debug.Log("error! no goods info!");
			}
			break;
		}
		case 2:
		{
			int need_count5 = list[0].need_count;
			GoodsQualityType goods_quality5 = list[0].goods_quality;
			TUIGoodsInfo tUIGoodsInfo5 = null;
			if (dictionary.ContainsKey(list[0].goods_id))
			{
				tUIGoodsInfo5 = dictionary[list[0].goods_id];
				int goods_now_count2 = 0;
				if (goods_quality5 == tUIGoodsInfo5.quality)
				{
					goods_now_count2 = tUIGoodsInfo5.count;
				}
				int price6 = tUIGoodsInfo5.price_info.price;
				UnitType unit_type6 = tUIGoodsInfo5.price_info.unit_type;
				int goods_id5 = list[0].goods_id;
				string goods_name5 = list[0].goods_name;
				int need_count6 = list[1].need_count;
				GoodsQualityType goods_quality6 = list[1].goods_quality;
				TUIGoodsInfo tUIGoodsInfo6 = null;
				if (dictionary.ContainsKey(list[1].goods_id))
				{
					tUIGoodsInfo6 = dictionary[list[1].goods_id];
					int goods_now_count3 = 0;
					if (goods_quality6 == tUIGoodsInfo6.quality)
					{
						goods_now_count3 = tUIGoodsInfo6.count;
					}
					int price7 = tUIGoodsInfo6.price_info.price;
					UnitType unit_type7 = tUIGoodsInfo6.price_info.unit_type;
					int goods_id6 = list[1].goods_id;
					string goods_name6 = list[1].goods_name;
					goods_need_item01.ShowGoodsNeedItem(goods_now_count2, goods_quality5, need_count5, price6, goods_id5, unit_type6, goods_name5);
					goods_need_item02.ShowGoodsNeedItem(goods_now_count3, goods_quality6, need_count6, price7, goods_id6, unit_type7, goods_name6);
					goods_need_item03.HideGoodsNeedItem();
				}
				else
				{
					Debug.Log("error! no goods info!");
				}
			}
			else
			{
				Debug.Log("error! no goods info!");
			}
			break;
		}
		case 3:
		{
			int need_count = list[0].need_count;
			GoodsQualityType goods_quality = list[0].goods_quality;
			TUIGoodsInfo tUIGoodsInfo = null;
			if (dictionary.ContainsKey(list[0].goods_id))
			{
				tUIGoodsInfo = dictionary[list[0].goods_id];
				int num = 0;
				if (goods_quality == tUIGoodsInfo.quality)
				{
					num = tUIGoodsInfo.count;
				}
				int price2 = tUIGoodsInfo.price_info.price;
				UnitType unit_type2 = tUIGoodsInfo.price_info.unit_type;
				int goods_id = list[0].goods_id;
				string goods_name = list[0].goods_name;
				int need_count2 = list[1].need_count;
				GoodsQualityType goods_quality2 = list[1].goods_quality;
				TUIGoodsInfo tUIGoodsInfo2 = null;
				if (dictionary.ContainsKey(list[1].goods_id))
				{
					tUIGoodsInfo2 = dictionary[list[1].goods_id];
					int num2 = 0;
					if (goods_quality2 == tUIGoodsInfo2.quality)
					{
						num2 = tUIGoodsInfo2.count;
					}
					int price3 = tUIGoodsInfo2.price_info.price;
					UnitType unit_type3 = tUIGoodsInfo2.price_info.unit_type;
					int goods_id2 = list[1].goods_id;
					string goods_name2 = list[1].goods_name;
					int need_count3 = list[2].need_count;
					GoodsQualityType goods_quality3 = list[2].goods_quality;
					TUIGoodsInfo tUIGoodsInfo3 = null;
					if (dictionary.ContainsKey(list[2].goods_id))
					{
						tUIGoodsInfo3 = dictionary[list[2].goods_id];
						int num3 = 0;
						if (goods_quality3 == tUIGoodsInfo3.quality)
						{
							num3 = tUIGoodsInfo3.count;
						}
						int price4 = tUIGoodsInfo3.price_info.price;
						UnitType unit_type4 = tUIGoodsInfo3.price_info.unit_type;
						int goods_id3 = list[2].goods_id;
						string goods_name3 = list[2].goods_name;
						goods_need_item01.ShowGoodsNeedItem(num, goods_quality, need_count, price2, goods_id, unit_type2, goods_name);
						goods_need_item02.ShowGoodsNeedItem(num2, goods_quality2, need_count2, price3, goods_id2, unit_type3, goods_name2);
						goods_need_item03.ShowGoodsNeedItem(num3, goods_quality3, need_count3, price4, goods_id3, unit_type4, goods_name3);
						Debug.Log("now_count01:" + num + " now_count02:" + num2 + " now_count03:" + num3);
					}
					else
					{
						Debug.Log("error! no goods info!");
					}
				}
				else
				{
					Debug.Log("error! no goods info!");
				}
			}
			else
			{
				Debug.Log("error! no goods info!" + list[0].goods_id);
			}
			break;
		}
		}
	}

	public void UpdateGoodsBuy(int index)
	{
		switch (index)
		{
		case 1:
			goods_need_item01.UpdateGoodsBuy();
			break;
		case 2:
			goods_need_item02.UpdateGoodsBuy();
			break;
		case 3:
			goods_need_item03.UpdateGoodsBuy();
			break;
		default:
			Debug.Log("error! index:" + index);
			break;
		}
	}

	public int GetGoodsID(int m_index)
	{
		switch (m_index)
		{
		case 1:
			return goods_need_item01.GetGoodsID();
		case 2:
			return goods_need_item02.GetGoodsID();
		case 3:
			return goods_need_item03.GetGoodsID();
		default:
			return 0;
		}
	}

	public int GetGoodsNeedCount(int m_index)
	{
		switch (m_index)
		{
		case 1:
			return goods_need_item01.GetGoodsNeedCount();
		case 2:
			return goods_need_item02.GetGoodsNeedCount();
		case 3:
			return goods_need_item03.GetGoodsNeedCount();
		default:
			return 0;
		}
	}

	public void SetDafaultParam()
	{
		goods_need_item01.SetDefaultParam();
		goods_need_item02.SetDefaultParam();
		goods_need_item03.SetDefaultParam();
	}

	public int GetGoodsKind()
	{
		return goods_kind;
	}
}
