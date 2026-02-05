using System.Collections.Generic;

public class TUIRoleInfo
{
	public int id;

	public string name = string.Empty;

	public string introduce = string.Empty;

	public string introduce_unlock = string.Empty;

	public bool unlock;

	public TUIPriceInfo unlock_price;

	public bool do_buy;

	public TUIPriceInfo do_buy_price;

	public List<TUIPopupInfo> active_skill_list;

	public TUIRoleInfo()
	{
	}

	public TUIRoleInfo(int m_id, string m_name, string m_introduce, bool m_unlock, TUIPriceInfo m_unlock_price, bool m_do_buy, TUIPriceInfo m_do_buy_price, string m_introduce_unlock, List<TUIPopupInfo> m_active_skill_list = null)
	{
		id = m_id;
		name = m_name;
		introduce = m_introduce;
		unlock = m_unlock;
		unlock_price = m_unlock_price;
		do_buy = m_do_buy;
		do_buy_price = m_do_buy_price;
		introduce_unlock = m_introduce_unlock;
		active_skill_list = m_active_skill_list;
	}
}
