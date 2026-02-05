using System.Collections.Generic;
using UnityEngine;

public class ScrollList_Weapon : MonoBehaviour
{
	public ScrollList_WeaponItem item_prefab;

	public TUIGrid grid;

	private TUIScrollListCircle scroll_list_ex;

	private List<ScrollList_WeaponItem> scroll_list_weapon_item_list;

	private ScrollList_WeaponItem item_choose;

	private Vector3 normal_position = Vector3.zero;

	private TUIButtonSelect btn_select;

	private void Awak()
	{
	}

	private void Start()
	{
		scroll_list_ex = base.gameObject.GetComponent<TUIScrollListCircle>();
	}

	private void Update()
	{
		CheckItemChoose();
	}

	private void CheckItemChoose()
	{
		GameObject nowItem = scroll_list_ex.GetNowItem();
		if (nowItem == null)
		{
			return;
		}
		ScrollList_WeaponItem component = nowItem.GetComponent<ScrollList_WeaponItem>();
		if (item_choose == null)
		{
			if (component != null)
			{
				item_choose = component;
				item_choose.DoChoose();
				CheckNewMark();
			}
		}
		else if (item_choose != component)
		{
			item_choose.DoUnChoose();
			item_choose = component;
			item_choose.DoChoose();
			CheckNewMark();
		}
	}

	public void AddScrollListItem(List<TUIWeaponAttributeInfo> m_attribute_info, Dictionary<int, NewMarkType> m_new_mark_list, TUIControl m_control)
	{
		if (m_control == null)
		{
			Debug.Log("no m_control!");
			return;
		}
		btn_select = m_control.GetComponent<TUIButtonSelect>();
		if (btn_select == null)
		{
			Debug.Log("no btn_select!");
			return;
		}
		if (m_attribute_info == null)
		{
			Debug.Log("no find m_attribute_info!");
			return;
		}
		if (scroll_list_ex == null)
		{
			scroll_list_ex = base.gameObject.GetComponent<TUIScrollListCircle>();
		}
		for (int i = 0; i < m_attribute_info.Count; i++)
		{
			ScrollList_WeaponItem scrollList_WeaponItem = (ScrollList_WeaponItem)Object.Instantiate(item_prefab);
			scrollList_WeaponItem.transform.parent = grid.transform;
			scrollList_WeaponItem.DoCreate(m_attribute_info[i], m_new_mark_list);
			scroll_list_ex.Add(scrollList_WeaponItem.gameObject);
			if (scroll_list_weapon_item_list == null)
			{
				scroll_list_weapon_item_list = new List<ScrollList_WeaponItem>();
			}
			scroll_list_weapon_item_list.Add(scrollList_WeaponItem);
		}
		scroll_list_ex.ResetGrid();
		scroll_list_ex.SetItemList();
		normal_position = base.transform.localPosition;
	}

	public void UpdateNewMark(Dictionary<int, NewMarkType> m_new_mark_list)
	{
		if (m_new_mark_list == null || scroll_list_weapon_item_list == null)
		{
			return;
		}
		for (int i = 0; i < scroll_list_weapon_item_list.Count; i++)
		{
			ScrollList_WeaponItem scrollList_WeaponItem = scroll_list_weapon_item_list[i];
			if (scrollList_WeaponItem != null)
			{
				scrollList_WeaponItem.UpdateNewMark(m_new_mark_list);
			}
		}
	}

	public void ResetPosition()
	{
		grid.repositionNow = true;
	}

	public void ResetPositionNow()
	{
		grid.Reposition();
	}

	public ScrollList_WeaponItem GetItemChoose()
	{
		return item_choose;
	}

	public void Show()
	{
		base.transform.localPosition = normal_position;
	}

	public void Hide()
	{
		ResetPositionNow();
		base.transform.localPosition = normal_position + new Vector3(0f, 1000f, 0f);
	}

	public void CheckNewMark()
	{
		if (btn_select == null)
		{
			Debug.Log("no btn_select!");
			return;
		}
		WeaponKindItemBtn component = btn_select.GetComponent<WeaponKindItemBtn>();
		if (component == null)
		{
			Debug.Log("no btn_weapon_kind_item");
			return;
		}
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i < scroll_list_weapon_item_list.Count; i++)
		{
			switch (scroll_list_weapon_item_list[i].GetNewMark())
			{
			case NewMarkType.New:
				flag = true;
				break;
			case NewMarkType.Mark:
				flag2 = true;
				break;
			}
		}
		if (flag)
		{
			component.ShowNewMark(NewMarkType.New);
		}
		else if (flag2)
		{
			component.ShowNewMark(NewMarkType.Mark);
		}
		else
		{
			component.ShowNewMark(NewMarkType.None);
		}
	}

	public void SetItemChoose(int m_weapon_id)
	{
		if (scroll_list_weapon_item_list == null)
		{
			return;
		}
		int nowItem = 0;
		for (int i = 0; i < scroll_list_weapon_item_list.Count; i++)
		{
			ScrollList_WeaponItem scrollList_WeaponItem = scroll_list_weapon_item_list[i];
			TUIWeaponAttributeInfo weaponAttributeInfo = scrollList_WeaponItem.GetWeaponAttributeInfo();
			if (weaponAttributeInfo != null && weaponAttributeInfo.id == m_weapon_id)
			{
				nowItem = i;
			}
		}
		grid.Reposition();
		grid.repositionStart = false;
		scroll_list_ex.SetNowItem(nowItem);
	}
}
