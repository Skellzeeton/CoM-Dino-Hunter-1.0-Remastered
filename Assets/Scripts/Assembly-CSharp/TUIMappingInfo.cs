using System.Collections.Generic;
using UnityEngine;

public class TUIMappingInfo
{
	public delegate void SwitchSceneStr(string m_next_scene);

	public delegate void SwitchSceneInt(int m_id);

	public delegate Transform FireEffect(int m_id);

	public delegate int GoldToCrystal(int m_gold);

	private static TUIMappingInfo instance = null;

	private static Vector3 current_angle = Vector3.zero;

	public Dictionary<int, string> stash_dictionary;

	public Dictionary<int, string> skill_dictionary;

	public Dictionary<int, string> weapon_dictionary;

	public Dictionary<int, string> prop_dictionary;

	public Dictionary<int, string> role_dictionary;

	public Dictionary<int, string> map_dictionary;

	public SwitchSceneStr switch_scene_function_str;

	public SwitchSceneInt switch_scene_function_int;

	public FireEffect fire_effect_function;

	public GoldToCrystal gold_to_crystal_function;

	public TUIMappingInfo()
	{
		stash_dictionary = new Dictionary<int, string>();
		stash_dictionary[20001] = "xunmenglong_putong_tougu2";
		stash_dictionary[30001] = "bawanglong_BOSS_ya1";
		stash_dictionary[30002] = "shuangguanlong_BOSS_duzhi1";
		stash_dictionary[30003] = "shuangguanlong_gongyou_longpi1";
		stash_dictionary[30004] = "shuangguanlong_putong_duzhi1";
		stash_dictionary[30005] = "shuangguanlong_bianyi_duzhi1";
		stash_dictionary[30006] = "yilong_gongyou_longzhua1";
		stash_dictionary[30007] = "yilong_BOSS_yizhua1";
		stash_dictionary[30008] = "yilong_putong_yizhua1";
		stash_dictionary[30009] = "yilong_bianyi_yizhua1";
		stash_dictionary[30010] = "sanjiaolong_gongyou_linke1";
		stash_dictionary[30011] = "sanjiaolong_putong_touke1";
		stash_dictionary[30012] = "sanjiaolong_bianyi_touke1";
		stash_dictionary[30013] = "sanjiaolong_BOSS_touke1";
		stash_dictionary[30014] = "xunmenglong_BOSS_duanwei1";
		stash_dictionary[30015] = "xunmenglong_gongyou_longgu1";
		stash_dictionary[30016] = "xunmenglong_putong_duanwei1";
		stash_dictionary[30017] = "xunmenglong_bianyi_duanwei1";
		stash_dictionary[30018] = "jibeilong_BOSS_weici1";
		stash_dictionary[30019] = "bawanglong_BOSS_ya3";
		stash_dictionary[30020] = "shuangguanlong_BOSS_duzhi3";
		stash_dictionary[30021] = "shuangguanlong_putong_duzhi3";
		stash_dictionary[30022] = "shuangguanlong_bianyi_duzhi3";
		stash_dictionary[30023] = "yilong_BOSS_yizhua";
		stash_dictionary[30024] = "yilong_putong_yizhua3";
		stash_dictionary[30025] = "yilong_bianyi_yizhua3";
		stash_dictionary[30026] = "sanjiaolong_BOSS_touke3";
		stash_dictionary[30027] = "sanjiaolong_putong_touke3";
		stash_dictionary[30028] = "sanjiaolong_bianyi_touke3";
		stash_dictionary[30029] = "xunmenglong_BOSS_duanwei3";
		stash_dictionary[30030] = "xunmenglong_putong_duanwei3";
		stash_dictionary[30031] = "xunmenglong_bianyi_duanwei3";
		stash_dictionary[30032] = "jibeilong_BOSS_weici3";
		stash_dictionary[30033] = "shangu_gongyou_tiekuangshi1";
		stash_dictionary[30034] = "yulin_gongyou_caishuijing1";
		stash_dictionary[30035] = "yanjiang_gongyou_rongyankuai1";
		stash_dictionary[30036] = "bawanglong_BOSS_beijia1";
		stash_dictionary[30037] = "shuangguanlong_gongyou_longpi3";
		stash_dictionary[30038] = "shuangguanlong_BOSS_guan1";
		stash_dictionary[30039] = "shuangguanlong_putong_guan1";
		stash_dictionary[30040] = "shuangguanlong_bianyi_guan1";
		stash_dictionary[30041] = "yilong_gongyou_longzhua3";
		stash_dictionary[30042] = "yilong_BOSS_yimo1";
		stash_dictionary[30043] = "yilong_putong_yimo1";
		stash_dictionary[30044] = "yilong_bianyi_yimo1";
		stash_dictionary[30045] = "sanjiaolong_gongyou_linke3";
		stash_dictionary[30046] = "sanjiaolong_BOSS_jiao1";
		stash_dictionary[30047] = "sanjiaolong_putong_jiao1";
		stash_dictionary[30048] = "sanjiaolong_bianyi_jiao1";
		stash_dictionary[30049] = "xunmenglong_putong_tougu1";
		stash_dictionary[30050] = "xunmenglong_bianyi_tougu1";
		stash_dictionary[30051] = "xunmenglong_BOSS_tougu1";
		stash_dictionary[30052] = "jibeilong_BOSS_xiongmo1";
		stash_dictionary[30053] = "bawanglong_BOSS_beijia3";
		stash_dictionary[30054] = "shuangguanlong_BOSS_guan3";
		stash_dictionary[30055] = "shuangguanlong_putong_guan3";
		stash_dictionary[30056] = "shuangguanlong_bianyi_guan3";
		stash_dictionary[30057] = "yilong_BOSS_yimo3";
		stash_dictionary[30058] = "yilong_putong_yimo3";
		stash_dictionary[30059] = "yilong_bianyi_yimo3";
		stash_dictionary[30060] = "sanjiaolong_BOSS_jiao3";
		stash_dictionary[30061] = "sanjiaolong_putong_jiao3";
		stash_dictionary[30062] = "sanjiaolong_bianyi_jiao3";
		stash_dictionary[30063] = "xunmenglong_gongyou_longgu3";
		stash_dictionary[30064] = "xunmenglong_BOSS_tougu3";
		stash_dictionary[30065] = "xunmenglong_putong_tougu3";
		stash_dictionary[30066] = "xunmenglong_bianyi_tougu3";
		stash_dictionary[30067] = "jibeilong_BOSS_xiongmo3";
		stash_dictionary[30068] = "shangu_gongyou_tiekuangshi3";
		stash_dictionary[30069] = "yulin_gongyou_caishuijing3";
		stash_dictionary[30070] = "yanjiang_gongyou_rongyankuai3";
		stash_dictionary[30071] = "bawanglong_BOSS_touke1";
		stash_dictionary[30072] = "jibeilong_BOSS_gusui1";
		stash_dictionary[30073] = "bawanglong_BOSS_touke3";
		stash_dictionary[30074] = "jibeilong_BOSS_gusui3";
		stash_dictionary[30075] = "bingchuan_gongyou_bingjiejin1";
		stash_dictionary[30076] = "shuangguanlong_gongyou_longpi2";
		stash_dictionary[30077] = "shuangguanlong_gongyou_longpi4";
		stash_dictionary[30078] = "bingchuan_gongyou_bingjiejin3";
		stash_dictionary[30079] = "bawanglong_BOSS_ya2";
		stash_dictionary[30080] = "yilong_BOSS_yizhua2";
		stash_dictionary[30081] = "shuangguanlong_BOSS_duzhi2";
		stash_dictionary[100001] = "duihuanquan_1";
		stash_dictionary[100002] = "duihuanquan_2";
		stash_dictionary[100003] = "duihuanquan_3";
		stash_dictionary[100004] = "duihuanquan_4";
		stash_dictionary[100005] = "duihuanquan_5";
		stash_dictionary[80001] = "crystal1";
		skill_dictionary = new Dictionary<int, string>();
		skill_dictionary[2] = "chongfeng";
		skill_dictionary[4] = "dunxing";
		skill_dictionary[5] = "huti";
		skill_dictionary[1] = "kuangbao";
		skill_dictionary[3] = "zhiliao";
		skill_dictionary[99002] = "chongfeng2";
		skill_dictionary[99004] = "dunxing2";
		skill_dictionary[99005] = "huti2";
		skill_dictionary[99001] = "kuangbao2";
		skill_dictionary[99003] = "zhiliao2";
		skill_dictionary[1001] = "passiveskill_1001";
		skill_dictionary[1002] = "passiveskill_1002";
		skill_dictionary[1003] = "passiveskill_1003";
		skill_dictionary[1004] = "passiveskill_1004";
		skill_dictionary[1005] = "passiveskill_1005";
		skill_dictionary[1006] = "passiveskill_1006";
		skill_dictionary[1007] = "passiveskill_1007";
		skill_dictionary[2001] = "passiveskill_2001";
		skill_dictionary[2002] = "passiveskill_2002";
		skill_dictionary[2003] = "passiveskill_2003";
		skill_dictionary[2004] = "passiveskill_2004";
		skill_dictionary[2005] = "passiveskill_2005";
		skill_dictionary[2006] = "passiveskill_2006";
		skill_dictionary[2007] = "passiveskill_2007";
		skill_dictionary[2008] = "passiveskill_2008";
		skill_dictionary[2009] = "passiveskill_2009";
		skill_dictionary[3001] = "passiveskill_3001";
		skill_dictionary[3002] = "passiveskill_3002";
		skill_dictionary[3003] = "passiveskill_3003";
		skill_dictionary[3004] = "passiveskill_3004";
		skill_dictionary[3005] = "passiveskill_3005";
		skill_dictionary[3006] = "passiveskill_3006";
		skill_dictionary[3007] = "passiveskill_3007";
		skill_dictionary[3008] = "passiveskill_3008";
		skill_dictionary[3009] = "passiveskill_3009";
		skill_dictionary[4001] = "passiveskill_4001";
		skill_dictionary[4002] = "passiveskill_4002";
		skill_dictionary[4003] = "passiveskill_4003";
		skill_dictionary[4004] = "passiveskill_4004";
		skill_dictionary[4005] = "passiveskill_4005";
		skill_dictionary[4006] = "passiveskill_4006";
		skill_dictionary[4007] = "passiveskill_4007";
		skill_dictionary[4008] = "passiveskill_4008";
		skill_dictionary[5001] = "passiveskill_5001";
		skill_dictionary[5002] = "passiveskill_5002";
		skill_dictionary[5003] = "passiveskill_5003";
		skill_dictionary[5004] = "passiveskill_5004";
		skill_dictionary[5005] = "passiveskill_5005";
		skill_dictionary[5006] = "passiveskill_5006";
		skill_dictionary[5007] = "passiveskill_5007";
		skill_dictionary[5008] = "passiveskill_5008";
		skill_dictionary[5009] = "passiveskill_5009";
		weapon_dictionary = new Dictionary<int, string>();
		weapon_dictionary[1] = "Weapon001";
		weapon_dictionary[2] = "Weapon002";
		weapon_dictionary[3] = "Weapon003";
		weapon_dictionary[4] = "Weapon004";
		weapon_dictionary[5] = "Weapon005";
		weapon_dictionary[6] = "Weapon006";
		weapon_dictionary[7] = "Weapon007";
		weapon_dictionary[8] = "Weapon008";
		weapon_dictionary[9] = "Weapon009";
		weapon_dictionary[10] = "Weapon010";
		weapon_dictionary[11] = "Weapon011";
		weapon_dictionary[12] = "Weapon012";
		weapon_dictionary[13] = "Weapon013";
		weapon_dictionary[14] = "Weapon014";
		weapon_dictionary[15] = "Weapon015";
		weapon_dictionary[16] = "Weapon016";
		weapon_dictionary[17] = "Weapon017";
		weapon_dictionary[18] = "Weapon018";
		weapon_dictionary[19] = "Weapon019";
		weapon_dictionary[21] = "Weapon021";
		weapon_dictionary[22] = "Weapon022";
		weapon_dictionary[23] = "Weapon023";
		weapon_dictionary[10001] = "Stoneskin_001";
		weapon_dictionary[10002] = "Stoneskin_002";
		weapon_dictionary[10003] = "Stoneskin_003";
		weapon_dictionary[10004] = "Stoneskin_004";
		weapon_dictionary[10005] = "Stoneskin_005";
		weapon_dictionary[10006] = "Stoneskin_006";
		weapon_dictionary[10007] = "Stoneskin_007";
		prop_dictionary = new Dictionary<int, string>();
		prop_dictionary[1] = "Abundance";
		prop_dictionary[2] = "Fury";
		role_dictionary = new Dictionary<int, string>();
		role_dictionary[1] = "avatar1";
		role_dictionary[2] = "avatar5";
		role_dictionary[3] = "avatar4";
		role_dictionary[4] = "avatar3";
		role_dictionary[5] = "avatar2";
		role_dictionary[6] = "avatar6";
		map_dictionary = new Dictionary<int, string>();
		map_dictionary[1001] = "p7";
		map_dictionary[1002] = "p4";
		map_dictionary[1003] = "p2";
		map_dictionary[1004] = "p3";
		map_dictionary[1005] = "p7";
		map_dictionary[1006] = "p5";
		map_dictionary[1007] = "p8";
		map_dictionary[1008] = "p2";
		map_dictionary[1009] = "p4";
		map_dictionary[1010] = "p9";
		map_dictionary[1011] = "p2";
		map_dictionary[1012] = "p5";
		map_dictionary[1013] = "p7";
		map_dictionary[1014] = "p7";
		map_dictionary[1015] = "p5";
		map_dictionary[1016] = "p4";
		map_dictionary[1017] = "p7";
		map_dictionary[1018] = "p8";
		map_dictionary[1019] = "p7";
		map_dictionary[1020] = "p2";
		map_dictionary[1021] = "p5";
		map_dictionary[1022] = "p9";
		map_dictionary[1023] = "p7";
		map_dictionary[1024] = "p3";
		map_dictionary[5001] = "p6";
		map_dictionary[5002] = "p1";
		map_dictionary[5003] = "p7";
		map_dictionary[5004] = "p3";
		map_dictionary[5005] = "p6";
		map_dictionary[5006] = "p2";
		map_dictionary[5007] = "p7";
		map_dictionary[5008] = "p8";
		map_dictionary[5009] = "p5";
		map_dictionary[5010] = "p1";
		map_dictionary[5011] = "p6";
		map_dictionary[5012] = "p8";
		map_dictionary[5013] = "p1";
		map_dictionary[5014] = "p3";
		map_dictionary[5015] = "p2";
		map_dictionary[5016] = "p1";
		map_dictionary[5017] = "p4";
		map_dictionary[5018] = "p6";
		map_dictionary[5019] = "p1";
		map_dictionary[5020] = "p2";
		map_dictionary[5021] = "p6";
		map_dictionary[5022] = "p5";
		map_dictionary[5023] = "p3";
		map_dictionary[5024] = "p6";
		map_dictionary[5025] = "p2";
		map_dictionary[5026] = "p7";
		map_dictionary[5027] = "p6";
		map_dictionary[5028] = "p7";
		map_dictionary[5029] = "p1";
		map_dictionary[5030] = "p3";
		map_dictionary[5031] = "p6";
		map_dictionary[5032] = "p2";
		current_angle = new Vector3(354.6f, 189.9f, 0f);
		SetSwitchSceneStr(DoSwitchSceneStr);
		SetSwitchSceneInt(DoSwitchSceneInt);
		SetFireEffect(DoFireEffect);
		SetGoldToCrystalFunc(DoGoldToCrystal);
	}

	public static TUIMappingInfo Instance()
	{
		if (instance == null)
		{
			instance = new TUIMappingInfo();
		}
		return instance;
	}

	public string GetStashTexture(int id)
	{
		if (stash_dictionary.ContainsKey(id))
		{
			return stash_dictionary[id];
		}
		Debug.Log("error!" + id);
		return string.Empty;
	}

	public string GetSkillTexture(int id, bool m_active_skill_square = false)
	{
		if (m_active_skill_square)
		{
			if (skill_dictionary.ContainsKey(id + 99000))
			{
				return skill_dictionary[id + 99000];
			}
		}
		else if (skill_dictionary.ContainsKey(id))
		{
			return skill_dictionary[id];
		}
		Debug.Log("error!");
		return string.Empty;
	}

	public string GetWeaponTexture(int id)
	{
		if (weapon_dictionary.ContainsKey(id))
		{
			return weapon_dictionary[id];
		}
		Debug.Log("error!" + id);
		return string.Empty;
	}

	public string GetPropTexture(int id)
	{
		if (prop_dictionary.ContainsKey(id))
		{
			return prop_dictionary[id];
		}
		Debug.Log("error!");
		return string.Empty;
	}

	public string GetRoleTexture(int id)
	{
		if (role_dictionary.ContainsKey(id))
		{
			return role_dictionary[id];
		}
		Debug.Log("error!" + id);
		return string.Empty;
	}

	public string GetMapTexture(int id)
	{
		if (map_dictionary.ContainsKey(id))
		{
			return map_dictionary[id];
		}
		Debug.Log("error!" + id);
		return string.Empty;
	}

	public void SetStashTexture(int m_id, string m_name)
	{
		stash_dictionary[m_id] = m_name;
	}

	public void SetSkillTexture(int m_id, string m_name)
	{
		skill_dictionary[m_id] = m_name;
	}

	public void SetWeaponTexture(int m_id, string m_name)
	{
		weapon_dictionary[m_id] = m_name;
	}

	public void SetPropTexture(int m_id, string m_name)
	{
		prop_dictionary[m_id] = m_name;
	}

	public void SetRoleTexture(int m_id, string m_name)
	{
		role_dictionary[m_id] = m_name;
	}

	public void SetMapTexture(int m_id, string m_name)
	{
		map_dictionary[m_id] = m_name;
	}

	public Vector3 GetCurrentAngle()
	{
		return current_angle;
	}

	public void SetCurrentAngle(Vector3 m_angle)
	{
		current_angle = m_angle;
	}

	public string GetSceneName(int m_id)
	{
		string result = string.Empty;
		switch ((TUISceneType)m_id)
		{
		case TUISceneType.Scene_Equip:
			result = "Scene_Equip";
			break;
		case TUISceneType.Scene_Forge:
			result = "Scene_Forge";
			break;
		case TUISceneType.Scene_Gold:
			result = "Scene_Gold";
			break;
		case TUISceneType.Scene_IAP:
			result = "Scene_IAP";
			break;
		case TUISceneType.Scene_Main:
			result = "Scene_Main";
			break;
		case TUISceneType.Scene_MainMenu:
			result = "Scene_MainMenu";
			break;
		case TUISceneType.Scene_Map:
			result = "Scene_Map";
			break;
		case TUISceneType.Scene_Skill:
			result = "Scene_Skill";
			break;
		case TUISceneType.Scene_Stash:
			result = "Scene_Stash";
			break;
		case TUISceneType.Scene_Tavern:
			result = "Scene_Tavern";
			break;
		}
		return result;
	}

	public void SetSwitchSceneStr(SwitchSceneStr m_function)
	{
		switch_scene_function_str = m_function;
	}

	public void SetSwitchSceneInt(SwitchSceneInt m_function)
	{
		switch_scene_function_int = m_function;
	}

	public SwitchSceneStr GetSwitchSceneStr()
	{
		return switch_scene_function_str;
	}

	public SwitchSceneInt GetSwitchSceneInt()
	{
		return switch_scene_function_int;
	}

	public void DoSwitchSceneStr(string m_next_scene)
	{
		iGameApp.GetInstance().EnterScene(m_next_scene);
	}

	public void DoSwitchSceneInt(int m_scene_id)
	{
		iGameApp.GetInstance().EnterScene((kGameSceneEnum)m_scene_id);
	}

	public void SetFireEffect(FireEffect m_function)
	{
		fire_effect_function = m_function;
	}

	public FireEffect GetFireEffect()
	{
		return fire_effect_function;
	}

	public Transform DoFireEffect(int m_id)
	{
		iGameData gameData = iGameApp.GetInstance().m_GameData;
		if (gameData == null)
		{
			return null;
		}
		CWeaponInfoLevel weaponInfo = gameData.GetWeaponInfo(m_id, 1);
		if (weaponInfo == null)
		{
			return null;
		}
		GameObject gameObject = PrefabManager.Get(weaponInfo.nFire);
		if (gameObject == null)
		{
			return null;
		}
		GameObject gameObject2 = Object.Instantiate(gameObject) as GameObject;
		if (gameObject2 == null)
		{
			return null;
		}
		return gameObject2.transform;
	}

	public void SetGoldToCrystalFunc(GoldToCrystal m_function)
	{
		gold_to_crystal_function = m_function;
	}

	public GoldToCrystal GetGoldToCrystalFunc()
	{
		return gold_to_crystal_function;
	}

	public int DoGoldToCrystal(int m_gold)
	{
		float num = 0.01904f;
		float p = 0.8f;
		float num2 = -3f;
		int num3 = Mathf.CeilToInt(num * Mathf.Pow(m_gold * 10, p) + num2);
		if (num3 < 1)
		{
			num3 = 1;
		}
		return num3;
	}
}
