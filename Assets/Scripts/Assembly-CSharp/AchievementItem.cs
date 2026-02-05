using System.Collections.Generic;
using UnityEngine;

public class AchievementItem : MonoBehaviour
{
	public AchievementStars achievement_stars;

	public TUILabel label_name;

	public TUILabel label_introduce;

	public AchievementBar achievement_bar;

	public AchievementRewardText achievement_reward_text_mid;

	public AchievementRewardText achievement_reward_text_right;

	public TUIButton btn_achievement;

	public TUIMeshSprite img_bg;

	public GameObject effect_stars_prefab;

	private TUIOneAchievementInfo chievement_info;

	private AchievementLevelType star_level;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void DoCreate(TUIOneAchievementInfo m_chievement_info, bool m_change_bg = false, GameObject m_go_invoke = null)
	{
		chievement_info = m_chievement_info;
		if (chievement_info == null)
		{
			Debug.Log("no info!");
			return;
		}
		if (btn_achievement != null && m_go_invoke != null)
		{
			btn_achievement.invokeObject = m_go_invoke;
		}
		Dictionary<int, string> name_list = chievement_info.name_list;
		Dictionary<int, string> introduce_list = chievement_info.introduce_list;
		Dictionary<int, TUIAchievementRewardInfo> reward_list = chievement_info.reward_list;
		Dictionary<int, int> progress_list = chievement_info.progress_list;
		Dictionary<int, bool> take_reward_list = chievement_info.take_reward_list;
		if (take_reward_list.ContainsKey(1) && !take_reward_list[1])
		{
			star_level = AchievementLevelType.Level0;
		}
		else if (take_reward_list.ContainsKey(2) && !take_reward_list[2])
		{
			star_level = AchievementLevelType.Level1;
		}
		else if (take_reward_list.ContainsKey(3) && !take_reward_list[3])
		{
			star_level = AchievementLevelType.Level2;
		}
		else if (take_reward_list.ContainsKey(3) && take_reward_list[3])
		{
			star_level = AchievementLevelType.Level3;
		}
		int progress = 0;
		if (chievement_info != null)
		{
			progress = ((star_level != AchievementLevelType.Level3) ? progress_list[(int)(star_level + 1)] : progress_list[(int)star_level]);
		}
		string text = string.Empty;
		if (name_list != null)
		{
			if (star_level == AchievementLevelType.Level3)
			{
				if (name_list.ContainsKey((int)star_level))
				{
					text = name_list[(int)star_level];
				}
			}
			else if (name_list.ContainsKey((int)(star_level + 1)))
			{
				text = name_list[(int)(star_level + 1)];
			}
		}
		string introduce = string.Empty;
		if (introduce_list != null)
		{
			if (star_level == AchievementLevelType.Level3)
			{
				if (introduce_list.ContainsKey((int)star_level))
				{
					introduce = introduce_list[(int)star_level];
				}
			}
			else if (introduce_list.ContainsKey((int)(star_level + 1)))
			{
				introduce = introduce_list[(int)(star_level + 1)];
			}
		}
		TUIAchievementRewardInfo tUIAchievementRewardInfo = null;
		if (reward_list != null)
		{
			if (star_level == AchievementLevelType.Level3)
			{
				if (reward_list.ContainsKey((int)star_level))
				{
					tUIAchievementRewardInfo = reward_list[(int)star_level];
				}
			}
			else if (reward_list.ContainsKey((int)(star_level + 1)))
			{
				tUIAchievementRewardInfo = reward_list[(int)(star_level + 1)];
			}
		}
		bool flag = false;
		bool flag2 = false;
		int reward = 0;
		int reward2 = 0;
		UnitType unit = UnitType.Gold;
		UnitType unit2 = UnitType.Gold;
		if (tUIAchievementRewardInfo != null)
		{
			if (tUIAchievementRewardInfo.open_reward01)
			{
				flag = true;
				reward = tUIAchievementRewardInfo.reward_value01;
				unit = tUIAchievementRewardInfo.reward_unit01;
			}
			if (tUIAchievementRewardInfo.open_reward02)
			{
				flag2 = true;
				reward2 = tUIAchievementRewardInfo.reward_value02;
				unit2 = tUIAchievementRewardInfo.reward_unit02;
			}
		}
		if (flag && !flag2)
		{
			DoCreateEx(star_level, text, introduce, progress, reward, unit);
		}
		else if (flag && flag2)
		{
			DoCreateEx(star_level, text, introduce, progress, reward, unit, reward2, unit2);
		}
		else
		{
			Debug.Log("no reward?!Next Level:" + (int)(star_level + 1));
		}
		if (img_bg != null)
		{
			img_bg.gameObject.SetActiveRecursively((!m_change_bg) ? true : false);
		}
	}

	public void DoCreateEx(AchievementLevelType m_level, string m_name, string m_introduce, int m_progress, int m_reward01, UnitType m_unit01)
	{
		if (achievement_stars != null)
		{
			achievement_stars.SetInfo(m_level);
		}
		if (label_name != null)
		{
			label_name.Text = m_name;
		}
		if (label_introduce != null)
		{
			label_introduce.Text = m_introduce;
		}
		switch (m_progress)
		{
		case 100:
			if (achievement_reward_text_mid != null && btn_achievement != null && achievement_bar != null)
			{
				achievement_reward_text_mid.Show(m_reward01, m_unit01);
				achievement_reward_text_right.Hide();
				btn_achievement.gameObject.SetActiveRecursively(true);
				btn_achievement.Show();
				achievement_bar.Hide();
			}
			break;
		case 0:
		case 1:
		case 2:
		case 3:
		case 4:
		case 5:
		case 6:
		case 7:
		case 8:
		case 9:
		case 10:
		case 11:
		case 12:
		case 13:
		case 14:
		case 15:
		case 16:
		case 17:
		case 18:
		case 19:
		case 20:
		case 21:
		case 22:
		case 23:
		case 24:
		case 25:
		case 26:
		case 27:
		case 28:
		case 29:
		case 30:
		case 31:
		case 32:
		case 33:
		case 34:
		case 35:
		case 36:
		case 37:
		case 38:
		case 39:
		case 40:
		case 41:
		case 42:
		case 43:
		case 44:
		case 45:
		case 46:
		case 47:
		case 48:
		case 49:
		case 50:
		case 51:
		case 52:
		case 53:
		case 54:
		case 55:
		case 56:
		case 57:
		case 58:
		case 59:
		case 60:
		case 61:
		case 62:
		case 63:
		case 64:
		case 65:
		case 66:
		case 67:
		case 68:
		case 69:
		case 70:
		case 71:
		case 72:
		case 73:
		case 74:
		case 75:
		case 76:
		case 77:
		case 78:
		case 79:
		case 80:
		case 81:
		case 82:
		case 83:
		case 84:
		case 85:
		case 86:
		case 87:
		case 88:
		case 89:
		case 90:
		case 91:
		case 92:
		case 93:
		case 94:
		case 95:
		case 96:
		case 97:
		case 98:
		case 99:
			if (achievement_reward_text_right != null && btn_achievement != null && achievement_bar != null)
			{
				achievement_reward_text_mid.Hide();
				achievement_reward_text_right.Show(m_reward01, m_unit01);
				btn_achievement.gameObject.SetActiveRecursively(false);
				achievement_bar.Show(m_progress);
			}
			break;
		default:
			Debug.Log("error!");
			break;
		}
		if (btn_achievement != null && m_level == AchievementLevelType.Level3)
		{
			btn_achievement.gameObject.SetActiveRecursively(false);
		}
	}

	public void DoCreateEx(AchievementLevelType m_level, string m_name, string m_introduce, int m_progress, int m_reward01, UnitType m_unit01, int m_reward02, UnitType m_unit02)
	{
		if (achievement_stars != null)
		{
			achievement_stars.SetInfo(m_level);
		}
		if (label_name != null)
		{
			label_name.Text = m_name;
		}
		if (label_introduce != null)
		{
			label_introduce.Text = m_introduce;
		}
		switch (m_progress)
		{
		case 100:
			if (achievement_reward_text_mid != null && achievement_reward_text_right != null && achievement_bar != null)
			{
				achievement_reward_text_mid.Show(m_reward01, m_unit01, m_reward02, m_unit02);
				achievement_reward_text_right.Hide();
				btn_achievement.gameObject.SetActiveRecursively(true);
				btn_achievement.Show();
				achievement_bar.Hide();
			}
			break;
		case 0:
		case 1:
		case 2:
		case 3:
		case 4:
		case 5:
		case 6:
		case 7:
		case 8:
		case 9:
		case 10:
		case 11:
		case 12:
		case 13:
		case 14:
		case 15:
		case 16:
		case 17:
		case 18:
		case 19:
		case 20:
		case 21:
		case 22:
		case 23:
		case 24:
		case 25:
		case 26:
		case 27:
		case 28:
		case 29:
		case 30:
		case 31:
		case 32:
		case 33:
		case 34:
		case 35:
		case 36:
		case 37:
		case 38:
		case 39:
		case 40:
		case 41:
		case 42:
		case 43:
		case 44:
		case 45:
		case 46:
		case 47:
		case 48:
		case 49:
		case 50:
		case 51:
		case 52:
		case 53:
		case 54:
		case 55:
		case 56:
		case 57:
		case 58:
		case 59:
		case 60:
		case 61:
		case 62:
		case 63:
		case 64:
		case 65:
		case 66:
		case 67:
		case 68:
		case 69:
		case 70:
		case 71:
		case 72:
		case 73:
		case 74:
		case 75:
		case 76:
		case 77:
		case 78:
		case 79:
		case 80:
		case 81:
		case 82:
		case 83:
		case 84:
		case 85:
		case 86:
		case 87:
		case 88:
		case 89:
		case 90:
		case 91:
		case 92:
		case 93:
		case 94:
		case 95:
		case 96:
		case 97:
		case 98:
		case 99:
			if (achievement_reward_text_mid != null && achievement_reward_text_right != null && achievement_bar != null)
			{
				achievement_reward_text_mid.Hide();
				achievement_reward_text_right.Show(m_reward01, m_unit01, m_reward02, m_unit02);
				btn_achievement.gameObject.SetActiveRecursively(false);
				achievement_bar.Show(m_progress);
			}
			break;
		default:
			Debug.Log("error!");
			break;
		}
		if (btn_achievement != null && m_level == AchievementLevelType.Level3)
		{
			btn_achievement.gameObject.SetActiveRecursively(false);
		}
	}

	public void AfterTakeAchievement()
	{
		if (chievement_info == null)
		{
			Debug.Log("no info!");
			return;
		}
		Dictionary<int, string> name_list = chievement_info.name_list;
		Dictionary<int, string> introduce_list = chievement_info.introduce_list;
		Dictionary<int, TUIAchievementRewardInfo> reward_list = chievement_info.reward_list;
		Dictionary<int, int> progress_list = chievement_info.progress_list;
		Dictionary<int, bool> take_reward_list = chievement_info.take_reward_list;
		if (star_level + 1 > AchievementLevelType.Level3)
		{
			Debug.Log("error!");
			return;
		}
		star_level++;
		int progress = 0;
		if (chievement_info != null)
		{
			progress = ((star_level != AchievementLevelType.Level3) ? progress_list[(int)(star_level + 1)] : progress_list[(int)star_level]);
		}
		string text = string.Empty;
		if (name_list != null)
		{
			if (star_level == AchievementLevelType.Level3)
			{
				if (name_list.ContainsKey((int)star_level))
				{
					text = name_list[(int)star_level];
				}
			}
			else if (name_list.ContainsKey((int)(star_level + 1)))
			{
				text = name_list[(int)(star_level + 1)];
			}
		}
		string introduce = string.Empty;
		if (introduce_list != null)
		{
			if (star_level == AchievementLevelType.Level3)
			{
				if (introduce_list.ContainsKey((int)star_level))
				{
					introduce = introduce_list[(int)star_level];
				}
			}
			else if (introduce_list.ContainsKey((int)(star_level + 1)))
			{
				introduce = introduce_list[(int)(star_level + 1)];
			}
		}
		TUIAchievementRewardInfo tUIAchievementRewardInfo = null;
		if (reward_list != null)
		{
			if (star_level == AchievementLevelType.Level3)
			{
				if (reward_list.ContainsKey((int)star_level))
				{
					tUIAchievementRewardInfo = reward_list[(int)star_level];
				}
			}
			else if (reward_list.ContainsKey((int)(star_level + 1)))
			{
				tUIAchievementRewardInfo = reward_list[(int)(star_level + 1)];
			}
		}
		bool flag = false;
		bool flag2 = false;
		int reward = 0;
		int reward2 = 0;
		UnitType unit = UnitType.Gold;
		UnitType unit2 = UnitType.Gold;
		if (tUIAchievementRewardInfo != null)
		{
			if (tUIAchievementRewardInfo.open_reward01)
			{
				flag = true;
				reward = tUIAchievementRewardInfo.reward_value01;
				unit = tUIAchievementRewardInfo.reward_unit01;
			}
			if (tUIAchievementRewardInfo.open_reward02)
			{
				flag2 = true;
				reward2 = tUIAchievementRewardInfo.reward_value02;
				unit2 = tUIAchievementRewardInfo.reward_unit02;
			}
		}
		if (flag && !flag2)
		{
			DoCreateEx(star_level, text, introduce, progress, reward, unit);
		}
		else if (flag && flag2)
		{
			DoCreateEx(star_level, text, introduce, progress, reward, unit, reward2, unit2);
		}
		else
		{
			Debug.Log("no reward?!Next Level:" + (int)(star_level + 1));
		}
		if (effect_stars_prefab != null)
		{
			GameObject gameObject = (GameObject)Object.Instantiate(effect_stars_prefab);
			gameObject.transform.parent = achievement_stars.transform;
			gameObject.transform.localPosition = achievement_stars.GetStarPos(star_level) + new Vector3(0f, 0f, 0.5f);
			Object.Destroy(gameObject, 1f);
		}
	}

	public TUIAchievementRewardInfo TakeAchievement()
	{
		if (chievement_info == null)
		{
			Debug.Log("no info!");
			return null;
		}
		Dictionary<int, TUIAchievementRewardInfo> reward_list = chievement_info.reward_list;
		if (reward_list.ContainsKey((int)GetAchievementLevel()))
		{
			return reward_list[(int)GetAchievementLevel()];
		}
		return null;
	}

	public int GetID()
	{
		if (chievement_info != null)
		{
			return chievement_info.id;
		}
		return 0;
	}

	public AchievementLevelType GetStarLevel()
	{
		return star_level;
	}

	public AchievementLevelType GetAchievementLevel()
	{
		if (star_level + 1 <= AchievementLevelType.Level3)
		{
			return star_level + 1;
		}
		return AchievementLevelType.Level3;
	}
}
