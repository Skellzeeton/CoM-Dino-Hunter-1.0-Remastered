using UnityEngine;

public class BtnItem_Item : MonoBehaviour
{
	public int index;

	public TUIMeshSprite img_normal;

	public TUIMeshSprite img_pressed;

	public TUILabel label_value;

	public TUIMeshSprite img_new;

	public TUIMeshSprite img_bg;

	private TUIPopupInfo popup_info;

	private string weapon_texture_path = "TUI/Weapon/";

	private string skill_texture_path = "TUI/Skill/";

	private string goods_texture_path = "TUI/Goods/";

	private string texture_mark = "new";

	private string texture_new = "new2";

	private NewMarkType new_mark_type;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void SetInfo(TUIPopupInfo m_popup_info, Popup_Show.PopupType popup_type = Popup_Show.PopupType.Roles, bool play_animation = false, bool m_use_customize = false)
	{
		popup_info = m_popup_info;
		if (popup_info == null)
		{
			img_normal.texture = string.Empty;
			img_normal.CustomizeTexture = null;
			img_pressed.texture = string.Empty;
			img_pressed.CustomizeTexture = null;
			if (label_value != null)
			{
				label_value.Text = string.Empty;
			}
			if (play_animation && base.GetComponent<Animation>() != null)
			{
				base.GetComponent<Animation>().Play();
			}
			if (img_bg != null)
			{
				img_bg.gameObject.SetActiveRecursively(true);
			}
			return;
		}
		switch (popup_type)
		{
		case Popup_Show.PopupType.Roles:
		{
			string roleTexture = TUIMappingInfo.Instance().GetRoleTexture(popup_info.texture_id);
			img_normal.texture = roleTexture;
			img_pressed.texture = roleTexture;
			break;
		}
		case Popup_Show.PopupType.Skills01:
		case Popup_Show.PopupType.Skills:
		{
			string skillTexture = TUIMappingInfo.Instance().GetSkillTexture(popup_info.texture_id);
			if (m_use_customize)
			{
				SetCustomizeTexture(img_normal, skill_texture_path + skillTexture);
				SetCustomizeTexture(img_pressed, skill_texture_path + skillTexture);
			}
			else
			{
				img_normal.texture = skillTexture;
				img_pressed.texture = skillTexture;
			}
			break;
		}
		case Popup_Show.PopupType.Weapons01:
		case Popup_Show.PopupType.Weapons02:
		case Popup_Show.PopupType.Weapons03:
		{
			string weaponTexture = TUIMappingInfo.Instance().GetWeaponTexture(popup_info.texture_id);
			if (m_use_customize)
			{
				SetCustomizeTexture(img_normal, weapon_texture_path + weaponTexture);
				SetCustomizeTexture(img_pressed, weapon_texture_path + weaponTexture);
			}
			else
			{
				if (img_normal != null)
				{
					img_normal.texture = weaponTexture;
				}
				if (img_pressed != null)
				{
					img_pressed.texture = weaponTexture;
				}
			}
			if (img_bg != null)
			{
				img_bg.gameObject.SetActiveRecursively(false);
			}
			break;
		}
		case Popup_Show.PopupType.Props:
		{
			string stashTexture = TUIMappingInfo.Instance().GetStashTexture(popup_info.texture_id);
			if (m_use_customize)
			{
				SetCustomizeTexture(img_normal, goods_texture_path + stashTexture);
				SetCustomizeTexture(img_pressed, goods_texture_path + stashTexture);
			}
			else
			{
				img_normal.texture = stashTexture;
				img_pressed.texture = stashTexture;
			}
			if (label_value != null)
			{
				label_value.Text = popup_info.value.ToString();
			}
			break;
		}
		default:
			Debug.Log("error!" + popup_type);
			break;
		}
		if (play_animation && base.GetComponent<Animation>() != null)
		{
			base.GetComponent<Animation>().Play();
		}
	}

	public void SetCustomizeTexture(TUIMeshSprite m_sprite, string m_path)
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

	public void PlayAnimation()
	{
		base.GetComponent<Animation>().Play();
	}

	public int GetIndex()
	{
		return index;
	}

	public int GetNowEquipID()
	{
		if (popup_info != null)
		{
			return popup_info.texture_id;
		}
		return 0;
	}

	public TUIPopupInfo GetInfo()
	{
		return popup_info;
	}

	public void ShowNewMark(NewMarkType m_new_mark)
	{
		switch (m_new_mark)
		{
		case NewMarkType.Mark:
			if (img_new != null)
			{
				img_new.texture = texture_mark;
			}
			break;
		case NewMarkType.New:
			if (img_new != null)
			{
				img_new.texture = texture_new;
			}
			break;
		case NewMarkType.None:
			if (img_new != null)
			{
				img_new.texture = string.Empty;
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
