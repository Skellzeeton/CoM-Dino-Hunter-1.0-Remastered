using System.Collections.Generic;
using UnityEngine;

public class LevelMap : MonoBehaviour
{
	public LevelPoint[] level_point_list;

	public Transform[] mask_list;

	public Transform[] sign_list;

	public Transform left_border;

	public Transform right_border;

	private float map_width;

	protected float map_width_total = 1700f;

	protected Camera m_Camera;

	private void Awake()
	{
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void MoveScreen(float wparam, float lparam)
	{
		base.transform.localPosition = base.transform.localPosition + new Vector3(wparam, lparam);
		float num = base.transform.localPosition.x - 240f;
		float num2 = base.transform.localPosition.x + 240f + 1224f;
		if (num > left_border.localPosition.x)
		{
			base.transform.localPosition = new Vector3(left_border.localPosition.x + 240f, base.transform.localPosition.y, base.transform.localPosition.z);
		}
		if (num2 < right_border.localPosition.x)
		{
			base.transform.localPosition = new Vector3(right_border.localPosition.x - 240f - 1224f, base.transform.localPosition.y, base.transform.localPosition.z);
		}
	}

	public void SetScreenPos(Vector3 m_pos)
	{
		base.transform.localPosition = base.transform.localPosition - m_pos;
		float num = base.transform.localPosition.x - 240f;
		float num2 = base.transform.localPosition.x + 240f + 1224f;
		if (num > left_border.localPosition.x)
		{
			base.transform.localPosition = new Vector3(left_border.localPosition.x + 240f, base.transform.localPosition.y, base.transform.localPosition.z);
		}
		if (num2 < right_border.localPosition.x)
		{
			base.transform.localPosition = new Vector3(right_border.localPosition.x - 240f - 1224f, base.transform.localPosition.y, base.transform.localPosition.z);
		}
	}

	public void SetMapEnterInfo(TUIMapInfo m_map_info)
	{
		if (m_map_info == null)
		{
			Debug.Log("error!no map info");
			return;
		}
		MapEnterType map_enter_type = m_map_info.map_enter_type;
		List<int> list = new List<int>();
		List<int> list2 = new List<int>();
		List<int> list3 = new List<int>();
		List<int> list4 = new List<int>();
		if (m_map_info.level_open_list != null)
		{
			for (int i = 0; i < m_map_info.level_open_list.Length; i++)
			{
				list.Add(m_map_info.level_open_list[i]);
			}
			if (m_map_info.level_no_open_list != null)
			{
				for (int j = 0; j < m_map_info.level_no_open_list.Length; j++)
				{
					list2.Add(m_map_info.level_no_open_list[j]);
				}
			}
			if (m_map_info.level_goods_drop_list != null)
			{
				for (int k = 0; k < m_map_info.level_goods_drop_list.Length; k++)
				{
					list3.Add(m_map_info.level_goods_drop_list[k]);
				}
			}
			if (m_map_info.level_pass_list != null)
			{
				for (int l = 0; l < m_map_info.level_pass_list.Length; l++)
				{
					list4.Add(m_map_info.level_pass_list[l]);
				}
			}
			int now_level = m_map_info.now_level;
			int next_level = m_map_info.next_level;
			int count = list.Count;
			Vector3 m_move_pos = Vector3.zero;
			if (count < 1)
			{
				Debug.Log("error! you have no open level!!");
				return;
			}
			int num = FindLevelInMap(now_level, map_enter_type);
			switch (num)
			{
			case 1:
			{
				for (int num3 = 0; num3 < list2.Count; num3++)
				{
					if (list2[num3] >= 1004)
					{
						list2.RemoveRange(num3, list2.Count - num3);
						break;
					}
				}
				break;
			}
			case 2:
			{
				for (int n = 0; n < list2.Count; n++)
				{
					if (list2[n] >= 1010)
					{
						list2.RemoveRange(n, list2.Count - n);
						break;
					}
				}
				break;
			}
			case 3:
			{
				for (int num2 = 0; num2 < list2.Count; num2++)
				{
					if (list2[num2] >= 1016)
					{
						list2.RemoveRange(num2, list2.Count - num2);
						break;
					}
				}
				break;
			}
			case 4:
			{
				for (int m = 0; m < list2.Count; m++)
				{
					if (list2[m] >= 1022)
					{
						list2.RemoveRange(m, list2.Count - m);
						break;
					}
				}
				break;
			}
			}
			switch (map_enter_type)
			{
			case MapEnterType.Normal:
				if (level_point_list != null)
				{
					for (int num14 = 0; num14 < level_point_list.Length; num14++)
					{
						if (list != null)
						{
							for (int num15 = 0; num15 < list.Count; num15++)
							{
								if (level_point_list[num14].GetLevelID() == list[num15])
								{
									level_point_list[num14].SetLevelOpen();
									level_point_list[num14].ShowWayPoint();
									level_point_list[num14].ShowWayEx();
								}
							}
							count = list.Count;
						}
						else
						{
							Debug.Log("warning! no level open list!");
						}
						if (list2 != null)
						{
							for (int num16 = 0; num16 < list2.Count; num16++)
							{
								if (level_point_list[num14].GetLevelID() == list2[num16])
								{
									level_point_list[num14].SetLevelDisable();
									level_point_list[num14].HideWayPoint();
									level_point_list[num14].HideWayEx();
								}
							}
						}
						if (list4 != null)
						{
							for (int num17 = 0; num17 < list4.Count; num17++)
							{
								if (level_point_list[num14].GetLevelID() == list4[num17])
								{
									level_point_list[num14].SetLevelPass();
									level_point_list[num14].ShowWayPoint();
									level_point_list[num14].ShowWayEx();
									continue;
								}
								LevelPointEx levelPointEx3 = level_point_list[num14].FindLevelEx(list4[num17]);
								if (levelPointEx3 != null)
								{
									levelPointEx3.SetLevelPointState(LevelPointEx.LevelPointExState.Passed);
								}
							}
						}
						if (level_point_list[num14].GetLevelID() == now_level)
						{
							level_point_list[num14].OpenLevelAnimation();
							level_point_list[num14].HideWayPoint();
							level_point_list[num14].ShowWayEx();
							m_move_pos.x = level_point_list[num14].transform.position.x - base.transform.position.x;
						}
					}
				}
				else
				{
					Debug.Log("no info found!");
				}
				break;
			case MapEnterType.OpenNewLevel:
			{
				LevelPoint levelPoint = null;
				LevelPoint newLevelOpen = null;
				if (level_point_list != null)
				{
					for (int num10 = 0; num10 < level_point_list.Length; num10++)
					{
						if (list != null)
						{
							for (int num11 = 0; num11 < list.Count; num11++)
							{
								if (level_point_list[num10].GetLevelID() == list[num11])
								{
									level_point_list[num10].SetLevelOpen();
									level_point_list[num10].ShowWayPoint();
									level_point_list[num10].ShowWayEx();
								}
							}
							count = list.Count + 1;
						}
						else
						{
							Debug.Log("warning! no level open list!");
						}
						if (list2 != null)
						{
							for (int num12 = 0; num12 < list2.Count; num12++)
							{
								if (level_point_list[num10].GetLevelID() == list2[num12])
								{
									level_point_list[num10].SetLevelDisable();
									level_point_list[num10].HideWayPoint();
									level_point_list[num10].HideWayEx();
								}
							}
						}
						else
						{
							Debug.Log("warning! no level no open list!");
						}
						if (list4 != null)
						{
							for (int num13 = 0; num13 < list4.Count; num13++)
							{
								if (level_point_list[num10].GetLevelID() == list4[num13])
								{
									level_point_list[num10].SetLevelPass();
									level_point_list[num10].ShowWayPoint();
									level_point_list[num10].ShowWayEx();
									continue;
								}
								LevelPointEx levelPointEx2 = level_point_list[num10].FindLevelEx(list4[num13]);
								if (levelPointEx2 != null)
								{
									levelPointEx2.SetLevelPointState(LevelPointEx.LevelPointExState.Passed);
								}
							}
						}
						if (level_point_list[num10].GetLevelID() == now_level)
						{
							levelPoint = level_point_list[num10];
							m_move_pos.x = level_point_list[num10].transform.position.x - base.transform.position.x;
						}
						if (level_point_list[num10].GetLevelID() == next_level)
						{
							newLevelOpen = level_point_list[num10];
						}
					}
					levelPoint.SetNewLevelOpen(newLevelOpen);
				}
				else
				{
					Debug.Log("no info found!");
				}
				break;
			}
			case MapEnterType.SearchGoods:
				if (level_point_list != null)
				{
					int level = 0;
					for (int num4 = 0; num4 < level_point_list.Length; num4++)
					{
						if (list != null)
						{
							for (int num5 = 0; num5 < list.Count; num5++)
							{
								if (level_point_list[num4].GetLevelID() == list[num5])
								{
									level_point_list[num4].SetLevelOpen();
									level_point_list[num4].ShowWayPoint();
									level_point_list[num4].ShowWayEx();
								}
							}
							count = list.Count;
						}
						else
						{
							Debug.Log("warning! no level open list!");
						}
						if (list2 != null)
						{
							for (int num6 = 0; num6 < list2.Count; num6++)
							{
								if (level_point_list[num4].GetLevelID() == list2[num6])
								{
									level_point_list[num4].SetLevelDisable();
									level_point_list[num4].HideWayPoint();
									level_point_list[num4].HideWayEx();
								}
							}
						}
						else
						{
							Debug.Log("warning! no level no open list!");
						}
						if (list4 != null)
						{
							for (int num7 = 0; num7 < list4.Count; num7++)
							{
								if (level_point_list[num4].GetLevelID() == list4[num7])
								{
									level_point_list[num4].SetLevelPass();
									level_point_list[num4].ShowWayPoint();
									level_point_list[num4].ShowWayEx();
									continue;
								}
								LevelPointEx levelPointEx = level_point_list[num4].FindLevelEx(list4[num7]);
								if (levelPointEx != null)
								{
									levelPointEx.SetLevelPointState(LevelPointEx.LevelPointExState.Passed);
								}
							}
						}
						if (list3 != null)
						{
							for (int num8 = 0; num8 < list3.Count; num8++)
							{
								if (level_point_list[num4].GetLevelID() == list3[num8])
								{
									level = list3[num8];
									level_point_list[num4].OpenLevelAnimation();
									m_move_pos.x = level_point_list[num4].transform.position.x - base.transform.position.x;
								}
								else if (level_point_list[num4].FindGoodsDropLevelEx(list3[num8]))
								{
									level = list3[num8];
									m_move_pos.x = level_point_list[num4].transform.position.x - base.transform.position.x;
								}
							}
						}
						else
						{
							Debug.Log("warning! no level goods drop list!");
						}
						if (level_point_list[num4].GetLevelID() == now_level)
						{
							level_point_list[num4].HideWayPoint();
							level_point_list[num4].ShowWayEx();
						}
					}
					int num9 = 0;
					num9 = FindLevelInMap(level);
					if (num9 != 0 && num9 > num)
					{
						ShowSign(num9, base.transform.position, ref m_move_pos);
					}
				}
				else
				{
					Debug.Log("error! no info found!");
				}
				break;
			}
			ShowMask(num);
			SetScreenPos(m_move_pos);
		}
		else
		{
			Debug.Log("warning! you have no open level!!");
		}
	}

	public void SetLevelInfo(TUILevelInfo m_level_info)
	{
		if (m_level_info == null)
		{
			Debug.Log("error! no level info!");
			return;
		}
		if (level_point_list == null)
		{
			Debug.Log("error! no level list!");
			return;
		}
		for (int i = 0; i < level_point_list.Length; i++)
		{
			if (level_point_list[i].GetLevelID() == m_level_info.id)
			{
				level_point_list[i].SetLevelInfo(m_level_info);
			}
		}
	}

	public int FindLevelInMap(int m_level, MapEnterType m_enter_type = MapEnterType.Normal)
	{
		int result = 0;
		if (m_level >= 1001 && m_level <= 1003)
		{
			result = 1;
			if (m_level == 1003 && m_enter_type == MapEnterType.OpenNewLevel)
			{
				result = 2;
			}
		}
		else if (m_level >= 1004 && m_level <= 1009)
		{
			result = 2;
			if (m_level == 1009 && m_enter_type == MapEnterType.OpenNewLevel)
			{
				result = 3;
			}
		}
		else if (m_level >= 1010 && m_level <= 1015)
		{
			result = 3;
			if (m_level == 1015 && m_enter_type == MapEnterType.OpenNewLevel)
			{
				result = 4;
			}
		}
		else if (m_level >= 1016 && m_level <= 1021)
		{
			result = 4;
			if (m_level == 1021 && m_enter_type == MapEnterType.OpenNewLevel)
			{
				result = 5;
			}
		}
		else if (m_level >= 1022)
		{
			result = 5;
		}
		if (m_level >= 5001 && m_level <= 5009)
		{
			result = 2;
		}
		else if (m_level >= 5010 && m_level <= 5018)
		{
			result = 3;
		}
		else if (m_level >= 5019 && m_level <= 5028)
		{
			result = 4;
		}
		else if (m_level >= 5028 && m_level <= 5032)
		{
			result = 5;
		}
		return result;
	}

	public void ShowMask(int m_id)
	{
		if (mask_list == null || mask_list.Length < m_id - 1 || m_id < 1)
		{
			Debug.Log("error!");
			return;
		}
		if (m_id == 5)
		{
			for (int i = 0; i < mask_list.Length; i++)
			{
				mask_list[i].gameObject.SetActiveRecursively(false);
			}
			return;
		}
		for (int j = 0; j < mask_list.Length; j++)
		{
			if (j == m_id - 1)
			{
				mask_list[j].gameObject.SetActiveRecursively(true);
			}
			else
			{
				mask_list[j].gameObject.SetActiveRecursively(false);
			}
		}
	}

	public void ShowSign(int m_id, Vector3 m_pos, ref Vector3 m_move_pos)
	{
		if (sign_list == null || sign_list.Length < m_id || m_id < 1)
		{
			Debug.Log("error!");
		}
		else if (m_id >= 1 && m_id <= 5)
		{
			for (int i = 0; i < sign_list.Length; i++)
			{
				if (i == m_id - 1)
				{
					sign_list[i].gameObject.SetActiveRecursively(true);
					m_move_pos.x = sign_list[i].position.x - m_pos.x;
					LevelMapSign component = sign_list[i].GetComponent<LevelMapSign>();
					if (component != null)
					{
						component.PlaySignAnimation();
					}
				}
				else
				{
					sign_list[i].gameObject.SetActiveRecursively(false);
				}
			}
		}
		else
		{
			for (int j = 0; j < sign_list.Length; j++)
			{
				sign_list[j].gameObject.SetActiveRecursively(false);
			}
		}
	}
}
