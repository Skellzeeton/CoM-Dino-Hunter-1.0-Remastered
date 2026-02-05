using System.Collections.Generic;

public class TUISkillInfo
{
	public int id;

	public string name = string.Empty;

	public int level;

	public bool unlock;

	public bool active_skill;

	public string active_skill_introduce = string.Empty;

	public TUIPriceInfo unlock_price;

	public string skill_introduce_unlock = string.Empty;

	public Dictionary<int, TUIPriceInfo> level_price;

	public Dictionary<int, string> level_introduce;

	public Dictionary<int, string> level_introduce_ex;

	public TUISkillInfo(int m_id, string m_name, int m_level, bool m_unlock, TUIPriceInfo m_unlock_price, Dictionary<int, TUIPriceInfo> m_level_price, Dictionary<int, string> m_level_introduce, Dictionary<int, string> m_level_introduce_ex, string m_skill_introduce_unlock)
	{
		id = m_id;
		name = m_name;
		level = m_level;
		unlock = m_unlock;
		unlock_price = m_unlock_price;
		level_price = m_level_price;
		level_introduce = m_level_introduce;
		level_introduce_ex = m_level_introduce_ex;
		skill_introduce_unlock = m_skill_introduce_unlock;
	}

	public TUISkillInfo(int m_id, string m_name, bool m_active_skill, string m_active_skill_introduce)
	{
		id = m_id;
		name = m_name;
		active_skill = m_active_skill;
		active_skill_introduce = m_active_skill_introduce;
	}
}
