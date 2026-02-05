using System.Collections.Generic;
using UnityEngine;

public class BtnSelect_Item : MonoBehaviour
{
	public TUIMeshSprite img_item;

	public TUIMeshSprite img_new;

	private int index;

	private TUIPopupInfo popup_info;

	private string weapon_texture_path = "TUI/Weapon/";

	private string texture_mark = "new";

	private string texture_new = "new2";

	private NewMarkType new_mark_type;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void SetTexture(int texture_id, Popup_Show.PopupType popup_type)
	{
		switch (popup_type)
		{
		case Popup_Show.PopupType.Weapons01:
		case Popup_Show.PopupType.Weapons02:
		case Popup_Show.PopupType.Weapons03:
		{
			string weaponTexture = TUIMappingInfo.Instance().GetWeaponTexture(popup_info.texture_id);
			if (img_item != null)
			{
				img_item.texture = weaponTexture;
			}
			break;
		}
		case Popup_Show.PopupType.Props:
		{
			string propTexture = TUIMappingInfo.Instance().GetPropTexture(texture_id);
			img_item.texture = propTexture;
			break;
		}
		case Popup_Show.PopupType.Skills:
		{
			string skillTexture = TUIMappingInfo.Instance().GetSkillTexture(texture_id);
			img_item.texture = skillTexture;
			break;
		}
		case Popup_Show.PopupType.Roles:
		{
			string roleTexture = TUIMappingInfo.Instance().GetRoleTexture(texture_id);
			img_item.texture = roleTexture;
			break;
		}
		}
	}

	private void SetGoodsCustomizeTexture(TUIMeshSprite m_sprite, string m_path)
	{
		m_sprite.texture = string.Empty;
		m_sprite.UseCustomize = true;
		m_sprite.CustomizeTexture = Resources.Load(m_path) as Texture;
		if (m_sprite.CustomizeTexture == null)
		{
			Debug.Log("lose texture!");
		}
		else
		{
			m_sprite.CustomizeRect = new Rect(0f, 0f, m_sprite.CustomizeTexture.width, m_sprite.CustomizeTexture.height);
		}
	}

	public void SetClipRect(TUIRect rect)
	{
		base.transform.GetComponent<TUIClipBinder>().SetClipRect(rect);
	}

	public void DoCreate(TUIPopupInfo info, TUIRect rect, int m_index, Popup_Show.PopupType popup_type, Dictionary<int, NewMarkType> m_new_mark_list)
	{
		index = m_index;
		popup_info = info;
		SetTexture(popup_info.texture_id, popup_type);
		SetClipRect(rect);
		if (m_new_mark_list != null && m_new_mark_list.ContainsKey(popup_info.texture_id))
		{
			SetNewMark(img_new, m_new_mark_list[popup_info.texture_id]);
		}
		else
		{
			SetNewMark(img_new, NewMarkType.None);
		}
	}

	public void UpdateNewMark(NewMarkType m_new_mark)
	{
		SetNewMark(img_new, m_new_mark);
	}

	public int GetIndex()
	{
		return index;
	}

	public string GetIntroduce()
	{
		return popup_info.introduce;
	}

	public TUIPopupInfo GetInfo()
	{
		return popup_info;
	}

	public void DoChoose()
	{
		if (new_mark_type != NewMarkType.Mark)
		{
			HideNewMark(img_new);
		}
	}

	public void SetNewMark(TUIMeshSprite m_sprite, NewMarkType m_new_mark)
	{
		switch (m_new_mark)
		{
		case NewMarkType.Mark:
			if (m_sprite != null)
			{
				m_sprite.texture = texture_mark;
			}
			break;
		case NewMarkType.New:
			if (m_sprite != null)
			{
				m_sprite.texture = texture_new;
			}
			break;
		case NewMarkType.None:
			if (m_sprite != null)
			{
				m_sprite.texture = string.Empty;
			}
			break;
		}
		new_mark_type = m_new_mark;
	}

	public void HideNewMark(TUIMeshSprite m_sprite)
	{
		if (m_sprite != null)
		{
			m_sprite.texture = string.Empty;
			new_mark_type = NewMarkType.None;
		}
	}

	public NewMarkType GetNewMark()
	{
		return new_mark_type;
	}
}
