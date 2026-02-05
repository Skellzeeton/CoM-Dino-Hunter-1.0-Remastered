using System.Collections.Generic;

public class TUIEquipInfo
{
	public TUIPopupInfo role;

	public List<TUIPopupInfo> roles_list;

	public Dictionary<int, NewMarkType> roles_new_mark_list;

	public TUIPopupInfo prop01;

	public TUIPopupInfo prop02;

	public TUIPopupInfo prop03;

	public List<TUIPopupInfo> prop_list;

	public Dictionary<int, NewMarkType> prop_new_mark_list;

	public TUIPopupInfo skill01;

	public TUIPopupInfo skill02;

	public TUIPopupInfo skill03;

	public TUIPopupInfo skill04;

	public List<TUIPopupInfo> skill_list;

	public Dictionary<int, NewMarkType> skill_new_mark_list;

	public TUIPopupInfo weapon01;

	public TUIPopupInfo weapon02;

	public TUIPopupInfo weapon03;

	public TUIPopupInfo weapon04;

	public List<TUIPopupInfo> weapon_list01;

	public List<TUIPopupInfo> weapon_list02;

	public List<TUIPopupInfo> weapon_list03;

	public Dictionary<int, NewMarkType> weapon_new_mark_list;

	public void AddRolesNewMark(int m_id, NewMarkType m_type)
	{
		if (roles_new_mark_list == null)
		{
			roles_new_mark_list = new Dictionary<int, NewMarkType>();
		}
		roles_new_mark_list[m_id] = m_type;
	}

	public void AddPropNewMark(int m_id, NewMarkType m_type)
	{
		if (prop_new_mark_list == null)
		{
			prop_new_mark_list = new Dictionary<int, NewMarkType>();
		}
		prop_new_mark_list[m_id] = m_type;
	}

	public void AddSkillNewMark(int m_id, NewMarkType m_type)
	{
		if (skill_new_mark_list == null)
		{
			skill_new_mark_list = new Dictionary<int, NewMarkType>();
		}
		skill_new_mark_list[m_id] = m_type;
	}

	public void AddWeaponNewMark(int m_id, NewMarkType m_type)
	{
		if (weapon_new_mark_list == null)
		{
			weapon_new_mark_list = new Dictionary<int, NewMarkType>();
		}
		weapon_new_mark_list[m_id] = m_type;
	}
}
