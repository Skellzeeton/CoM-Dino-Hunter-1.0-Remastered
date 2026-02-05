using System.Collections.Generic;
using UnityEngine;

public class LevelPoint : MonoBehaviour
{
	public enum LevelPointState
	{
		Passed = 0,
		Open = 1,
		Disable = 2,
		Hide = 3
	}

	public int level_id;

	public List<GameObject> way_points_list;

	public TUIButtonClick btn_level;

	public bool open_aniamtion;

	public bool open_way_points_show;

	public LevelPointBottom img_bottom;

	public LevelPointEx[] level_ex_list;

	public TUIMeshSprite img_btn_normal;

	public TUIMeshSprite img_btn_press;

	public TUIMeshSprite img_btn_disable;

	public TUIMeshSprite img_lock;

	private LevelPointState level_point_state = LevelPointState.Hide;

	private float m_time;

	private int way_points_count;

	private int way_points_index;

	private float time_gap = 0.14f;

	private TUILevelInfo level_info;

	private LevelPoint next_level;

	private string texture_btn_normal = "zhurenwu_1";

	private string texture_btn_hui = "zhurenwu_1hui";

	private void Awake()
	{
		way_points_count = way_points_list.Count;
		if (open_aniamtion)
		{
			open_aniamtion = false;
			btn_level.GetComponent<Animation>().wrapMode = WrapMode.Loop;
			btn_level.GetComponent<Animation>().Play();
		}
		SetLevelPointState(LevelPointState.Hide);
		HideWayPoint();
		HideWayEx();
	}

	private void Start()
	{
	}

	private void Update()
	{
		UpdateWayPointAni(Time.deltaTime);
	}

	public void SetLevelPointState(LevelPointState m_level_point_state, bool change_level_point_ex = true)
	{
		level_point_state = m_level_point_state;
		if (level_point_state == LevelPointState.Passed)
		{
			btn_level.gameObject.SetActiveRecursively(true);
			btn_level.Reset();
			img_bottom.OpenChoose(false);
			if (img_lock != null)
			{
				img_lock.gameObject.SetActiveRecursively(false);
			}
			if (img_btn_normal != null)
			{
				img_btn_normal.texture = texture_btn_normal;
			}
			if (img_btn_press != null)
			{
				img_btn_press.texture = texture_btn_normal;
			}
		}
		else if (level_point_state == LevelPointState.Open)
		{
			btn_level.gameObject.SetActiveRecursively(true);
			btn_level.Reset();
			img_bottom.OpenChoose(false);
			if (level_ex_list != null && change_level_point_ex)
			{
				for (int i = 0; i < level_ex_list.Length; i++)
				{
					level_ex_list[i].SetLevelPointState(LevelPointEx.LevelPointExState.Open);
				}
			}
			if (img_lock != null)
			{
				img_lock.gameObject.SetActiveRecursively(false);
			}
			if (img_btn_normal != null)
			{
				img_btn_normal.texture = texture_btn_hui;
			}
			if (img_btn_press != null)
			{
				img_btn_press.texture = texture_btn_hui;
			}
		}
		else if (level_point_state == LevelPointState.Disable)
		{
			btn_level.Disable(true);
			btn_level.gameObject.SetActiveRecursively(false);
			img_bottom.OpenChoose(false);
			if (level_ex_list != null && change_level_point_ex)
			{
				for (int j = 0; j < level_ex_list.Length; j++)
				{
					level_ex_list[j].SetLevelPointState(LevelPointEx.LevelPointExState.Disable);
				}
			}
			if (img_lock != null)
			{
				img_lock.gameObject.SetActiveRecursively(true);
			}
			if (img_btn_normal != null)
			{
				img_btn_normal.texture = texture_btn_hui;
			}
			if (img_btn_press != null)
			{
				img_btn_press.texture = texture_btn_hui;
			}
		}
		else
		{
			if (level_point_state != LevelPointState.Hide)
			{
				return;
			}
			btn_level.Disable(true);
			btn_level.gameObject.SetActiveRecursively(false);
			img_bottom.Hide();
			if (level_ex_list != null && change_level_point_ex)
			{
				for (int k = 0; k < level_ex_list.Length; k++)
				{
					level_ex_list[k].SetLevelPointState(LevelPointEx.LevelPointExState.Hide);
				}
			}
			if (img_lock != null)
			{
				img_lock.gameObject.SetActiveRecursively(false);
			}
			if (img_btn_normal != null)
			{
				img_btn_normal.texture = texture_btn_hui;
			}
			if (img_btn_press != null)
			{
				img_btn_press.texture = texture_btn_hui;
			}
		}
	}

	public void UpdateWayPointAni(float delta_time)
	{
		if (!open_way_points_show)
		{
			return;
		}
		m_time += delta_time;
		if (!(m_time >= time_gap))
		{
			return;
		}
		m_time = 0f;
		way_points_list[way_points_index].SetActiveRecursively(true);
		way_points_index++;
		if (way_points_index >= way_points_count)
		{
			way_points_index = 0;
			open_way_points_show = false;
			CloseLevelAnimation();
			if (next_level != null)
			{
				next_level.SetLevelPointState(LevelPointState.Open, false);
				next_level.OpenLevelAnimation();
				next_level.ShowWayExAffterTime(1f);
			}
		}
	}

	public int GetLevelID()
	{
		return level_id;
	}

	public void SetLevelInfo(TUILevelInfo m_info)
	{
		level_info = m_info;
	}

	public TUILevelInfo GetLevelInfo()
	{
		return level_info;
	}

	public void OpenLevelAnimation()
	{
		btn_level.GetComponent<Animation>().wrapMode = WrapMode.Loop;
		btn_level.GetComponent<Animation>().Play();
		img_bottom.OpenChoose(true);
	}

	public void CloseLevelAnimation()
	{
		btn_level.GetComponent<Animation>().Stop();
		img_bottom.OpenChoose(false);
	}

	public void HideWayPoint()
	{
		if (way_points_list == null)
		{
			Debug.Log("error! no level way!");
			return;
		}
		for (int i = 0; i < way_points_list.Count; i++)
		{
			way_points_list[i].SetActiveRecursively(false);
		}
	}

	public void ShowWayPoint()
	{
		if (way_points_list == null)
		{
			Debug.Log("error! no level way!");
			return;
		}
		for (int i = 0; i < way_points_list.Count; i++)
		{
			way_points_list[i].SetActiveRecursively(true);
		}
	}

	public void HideWayEx()
	{
		if (level_ex_list == null)
		{
			Debug.Log("error! no level way ex!");
			return;
		}
		for (int i = 0; i < level_ex_list.Length; i++)
		{
			level_ex_list[i].HideWay();
		}
	}

	public void ShowWayEx()
	{
		if (level_ex_list == null)
		{
			Debug.Log("error! no level way ex!");
			return;
		}
		for (int i = 0; i < level_ex_list.Length; i++)
		{
			level_ex_list[i].ShowWay();
		}
	}

	public void ShowWayExAffterTime(float m_time_gap)
	{
		if (level_ex_list == null)
		{
			Debug.Log("error! no level way ex!");
			return;
		}
		for (int i = 0; i < level_ex_list.Length; i++)
		{
			level_ex_list[i].ShowWayAffterTime(m_time_gap);
		}
	}

	public void SetNewLevelOpen(LevelPoint m_next_level)
	{
		if (m_next_level == null)
		{
			Debug.Log("error! no next level info!");
			return;
		}
		open_way_points_show = true;
		next_level = m_next_level;
		HideWayPoint();
		OpenLevelAnimation();
	}

	public void SetLevelDisable()
	{
		SetLevelPointState(LevelPointState.Disable);
	}

	public void SetLevelOpen()
	{
		SetLevelPointState(LevelPointState.Open);
	}

	public void SetLevelPass()
	{
		SetLevelPointState(LevelPointState.Passed);
	}

	public bool FindGoodsDropLevelEx(int m_id)
	{
		if (level_ex_list == null)
		{
			return false;
		}
		for (int i = 0; i < level_ex_list.Length; i++)
		{
			if (level_ex_list[i].GetLevelID() == m_id)
			{
				level_ex_list[i].OpenLevelAnimation();
				return true;
			}
		}
		return false;
	}

	public LevelPointEx FindLevelEx(int m_id)
	{
		if (level_ex_list == null)
		{
			return null;
		}
		for (int i = 0; i < level_ex_list.Length; i++)
		{
			if (level_ex_list[i].GetLevelID() == m_id)
			{
				return level_ex_list[i];
			}
		}
		return null;
	}
}
