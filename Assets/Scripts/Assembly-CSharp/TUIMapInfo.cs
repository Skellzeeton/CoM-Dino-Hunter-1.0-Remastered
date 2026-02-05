public class TUIMapInfo
{
	public MapEnterType map_enter_type;

	public int now_level;

	public int next_level;

	public int[] level_open_list;

	public int[] level_no_open_list;

	public int[] level_goods_drop_list;

	public int[] level_pass_list;

	public TUILevelInfo level_info;

	public TUIMapInfo(MapEnterType m_map_enter_type, int m_now_level, int[] m_level_open_list, int[] m_level_no_open_list, int[] m_level_pass_list)
	{
		map_enter_type = m_map_enter_type;
		now_level = m_now_level;
		level_open_list = m_level_open_list;
		level_no_open_list = m_level_no_open_list;
		level_pass_list = m_level_pass_list;
	}

	public TUIMapInfo(MapEnterType m_map_enter_type, int m_now_level, int m_next_level, int[] m_level_open_list, int[] m_level_no_open_list, int[] m_level_pass_list)
	{
		map_enter_type = m_map_enter_type;
		now_level = m_now_level;
		next_level = m_next_level;
		level_open_list = m_level_open_list;
		level_no_open_list = m_level_no_open_list;
		level_pass_list = m_level_pass_list;
	}

	public TUIMapInfo(MapEnterType m_map_enter_type, int m_now_level, int[] m_level_open_list, int[] m_level_no_open_list, int[] m_goods_drop_level_list, int[] m_level_pass_list)
	{
		map_enter_type = m_map_enter_type;
		now_level = m_now_level;
		level_goods_drop_list = m_goods_drop_level_list;
		level_open_list = m_level_open_list;
		level_no_open_list = m_level_no_open_list;
		level_pass_list = m_level_pass_list;
	}

	public TUIMapInfo(TUILevelInfo m_level_info)
	{
		level_info = m_level_info;
	}
}
