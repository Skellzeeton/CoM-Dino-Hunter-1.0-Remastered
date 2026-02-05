using System.Collections.Generic;
using UnityEngine;

public class PopupWeaponSupplement : MonoBehaviour
{
	public GameObject go_popup;

	public PopupSkillUpdateBuy btn_buy;

	public PopupWeaponSupplementGoods supplement_goods;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void SetSupplementInfo(TUISupplementInfo m_supplement_info)
	{
		if (m_supplement_info == null || supplement_goods == null)
		{
			Debug.Log("error!");
			return;
		}
		supplement_goods.ClearInfo();
		List<TUIGoodsSupplementInfo> goods_list = m_supplement_info.goods_list;
		if (goods_list != null)
		{
			for (int i = 0; i < goods_list.Count; i++)
			{
				supplement_goods.SetGoodsInfo(i + 1, goods_list[i].id, goods_list[i].count, goods_list[i].quality_type);
			}
		}
		int price_value = m_supplement_info.price_value;
		UnitType price_unit = m_supplement_info.price_unit;
		if (price_value != 0)
		{
			if (goods_list == null || goods_list.Count == 0)
			{
				supplement_goods.SetOnlyPriceInfo(price_value, price_unit);
			}
			else
			{
				supplement_goods.SetPriceInfo(price_value, price_unit);
			}
		}
	}

	public void SetSupplementBtnInfo(TUIPriceInfo m_info)
	{
		if (m_info == null)
		{
			Debug.Log("error!");
		}
		else
		{
			btn_buy.SetBtnText(m_info.price, m_info.unit_type);
		}
	}

	public void Show()
	{
		base.gameObject.transform.localPosition = new Vector3(0f, 0f, base.gameObject.transform.localPosition.z);
		if (go_popup != null && go_popup.GetComponent<Animation>() != null)
		{
			go_popup.GetComponent<Animation>().Play();
		}
	}

	public void Hide()
	{
		base.gameObject.transform.localPosition = new Vector3(0f, -1000f, base.gameObject.transform.localPosition.z);
	}
}
