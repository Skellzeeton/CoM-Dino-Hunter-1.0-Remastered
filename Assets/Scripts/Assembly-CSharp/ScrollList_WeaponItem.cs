using System.Collections.Generic;
using UnityEngine;

public class ScrollList_WeaponItem : MonoBehaviour
{
	public TUIMeshSprite img_bg;

	public TUIMeshSprite img_frame;

	public TUIMeshSprite img_frame_choose;

	public TUIMeshSprite img_new;

	private bool be_choose;

	private TUIWeaponAttributeInfo attribute_info;

	private string texture_path = "Artist/Textures/Weapon/";

	private string texture_mark = "new";

	private string texture_new = "new2";

	private NewMarkType new_mark_type;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void DoCreate(TUIWeaponAttributeInfo m_attribute_info, Dictionary<int, NewMarkType> m_new_mark_list)
	{
		if (m_attribute_info == null)
		{
			Debug.Log("error!");
			return;
		}
		attribute_info = m_attribute_info;
		be_choose = true;
		string weaponTexture = TUIMappingInfo.Instance().GetWeaponTexture(m_attribute_info.id);
		if (string.IsNullOrEmpty(weaponTexture))
		{
			Debug.LogWarning("Missing weapon texture mapping for id: " + m_attribute_info.id);
			return;
		}
		if (img_bg != null)
		{
			SetCustomizeTexture(img_bg, texture_path + weaponTexture);
		}
		DoUnChoose();
		UpdateNewMark(m_new_mark_list);
	}

	public void UpdateNewMark(Dictionary<int, NewMarkType> m_new_mark_list)
	{
		if (m_new_mark_list != null && m_new_mark_list.ContainsKey(attribute_info.id))
		{
			SetNewMark(img_new, m_new_mark_list[attribute_info.id]);
		}
		else
		{
			SetNewMark(img_new, NewMarkType.None);
		}
	}

	public void SetCustomizeTexture(TUIMeshSprite m_sprite, string m_path)
	{
		if (m_sprite == null) return;
		Texture tex = Resources.Load(m_path) as Texture;
		if (tex == null)
		{
			Debug.LogWarning("Missing texture: " + m_path);
			return;
		}
		m_sprite.texture = string.Empty;
		m_sprite.UseCustomize = true;
		m_sprite.CustomizeTexture = tex;
		if (m_path.Contains("Stoneskin"))
		{
			m_sprite.CustomizeRect = new Rect(0f, 0f, 96f, 114f);
		}
		else
		{
			m_sprite.CustomizeRect = new Rect(0f, 0f, 200f, 128f);
		}
	}



	public TUIMeshSprite GetCustomizeTexture()
	{
		return img_bg;
	}

	public void DoChoose()
	{
		if (!be_choose)
		{
			be_choose = true;
			img_frame.gameObject.SetActiveRecursively(false);
			img_frame_choose.gameObject.SetActiveRecursively(true);
			if (new_mark_type != NewMarkType.Mark)
			{
				HideNewMark(img_new);
			}
		}
	}

	public void DoUnChoose()
	{
		if (be_choose)
		{
			be_choose = false;
			img_frame.gameObject.SetActiveRecursively(true);
			img_frame_choose.gameObject.SetActiveRecursively(false);
		}
	}

	public TUIWeaponAttributeInfo GetWeaponAttributeInfo()
	{
		return attribute_info;
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
