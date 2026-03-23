using System.Collections.Generic;
using UnityEngine;

public class PrefabManager
{
	protected static Transform mPreLoadNode;

	protected static Dictionary<int, string> m_dictData;

	protected static Dictionary<int, GameObject> m_dictCache;

	protected static Dictionary<int, CPoolManage> m_dictPool;

	protected static Dictionary<string, GameObject> m_dictCacheByPath;

	public static GameObject Get(int nPrefabPath)
	{
		if (!m_dictCache.ContainsKey(nPrefabPath))
		{
			return Load(nPrefabPath);
		}
		return m_dictCache[nPrefabPath];
	}

	public static GameObject Get(string sPath)
	{
		if (!m_dictCacheByPath.ContainsKey(sPath))
		{
			return Load(sPath);
		}
		return m_dictCacheByPath[sPath];
	}

	public static GameObject GetPoolObject(int nPrefabPath, float fTime = 0f)
	{
		if (!m_dictPool.ContainsKey(nPrefabPath))
		{
			return null;
		}
		return m_dictPool[nPrefabPath].Get(fTime);
	}

	public static string GetPath(int nPrefabPath)
	{
		if (!m_dictData.ContainsKey(nPrefabPath))
		{
			return string.Empty;
		}
		return m_dictData[nPrefabPath];
	}

	public static void Initialize()
	{
		m_dictData = new Dictionary<int, string>();
		m_dictCache = new Dictionary<int, GameObject>();
		m_dictPool = new Dictionary<int, CPoolManage>();
		m_dictCacheByPath = new Dictionary<string, GameObject>();
		m_dictData.Add(1, "Artist/Model/Character/charactor_model1");
		m_dictData.Add(2, "Artist/Model/Character/charactor_model2");
		m_dictData.Add(3, "Artist/Model/Character/charactor_model3");
		m_dictData.Add(4, "Artist/Model/Character/charactor_model4");
		m_dictData.Add(5, "Artist/Model/Character/charactor_model5");
		m_dictData.Add(6, "Artist/Model/Character/charactor_model6");
		m_dictData.Add(21, "Artist/Model/Character/model_pterodactyl1");
		m_dictData.Add(22, "Artist/Model/Character/model_pterodactyl2");
		m_dictData.Add(23, "Artist/Model/Character/model_pterodactyl3");
		m_dictData.Add(31, "Artist/Model/Character/model_velociraptor1");
		m_dictData.Add(32, "Artist/Model/Character/model_velociraptor2");
		m_dictData.Add(33, "Artist/Model/Character/model_velociraptor3");
		m_dictData.Add(41, "Artist/Model/Character/model_dilophosaurus1");
		m_dictData.Add(42, "Artist/Model/Character/model_dilophosaurus2");
		m_dictData.Add(43, "Artist/Model/Character/model_dilophosaurus3");
		m_dictData.Add(51, "Artist/Model/Character/model_triceratops1");
		m_dictData.Add(52, "Artist/Model/Character/model_triceratops2");
		m_dictData.Add(53, "Artist/Model/Character/model_triceratops3");
		m_dictData.Add(60, "Artist/Model/Character/model_tyrannosaurus_boss");
		m_dictData.Add(61, "Artist/Model/Character/model_tyrannosaurus_elite");
		m_dictData.Add(70, "Artist/Model/Character/model_ridgebackdragon_boss");
		m_dictData.Add(101, "Artist/Model/Weapon/weapon_001");
		m_dictData.Add(102, "Artist/Model/Weapon/weapon_002");
		m_dictData.Add(103, "Artist/Model/Weapon/weapon_003");
		m_dictData.Add(104, "Artist/Model/Weapon/weapon_004");
		m_dictData.Add(105, "Artist/Model/Weapon/weapon_005");
		m_dictData.Add(106, "Artist/Model/Weapon/weapon_006");
		m_dictData.Add(107, "Artist/Model/Weapon/weapon_007");
		m_dictData.Add(108, "Artist/Model/Weapon/weapon_008");
		m_dictData.Add(109, "Artist/Model/Weapon/weapon_009");
		m_dictData.Add(110, "Artist/Model/Weapon/weapon_010");
		m_dictData.Add(111, "Artist/Model/Weapon/weapon_011");
		m_dictData.Add(112, "Artist/Model/Weapon/weapon_012");
		m_dictData.Add(113, "Artist/Model/Weapon/weapon_013");
		m_dictData.Add(114, "Artist/Model/Weapon/weapon_014");
		m_dictData.Add(115, "Artist/Model/Weapon/weapon_015");
		m_dictData.Add(116, "Artist/Model/Weapon/weapon_016");
		m_dictData.Add(117, "Artist/Model/Weapon/weapon_017");
		m_dictData.Add(118, "Artist/Model/Weapon/weapon_018");
		m_dictData.Add(119, "Artist/Model/Weapon/weapon_019");
		m_dictData.Add(121, "Artist/Model/Weapon/weapon_021");
		m_dictData.Add(122, "Artist/Model/Weapon/weapon_022");
		m_dictData.Add(123, "Artist/Model/Weapon/weapon_023");
		m_dictData.Add(130, "Artist/Model/Bullet/bullet_003");
		m_dictData.Add(131, "Artist/Model/Bullet/bullet_005");
		m_dictData.Add(132, "Artist/Model/Bullet/bullet_004");
		m_dictData.Add(150, "Artist/Model/Spawn/spawn_bullet_003");
		m_dictData.Add(151, "Artist/Model/Spawn/spawn_bullet_005");
		m_dictData.Add(152, "Artist/Model/Spawn/spawn_bullet_004");
		m_dictData.Add(170, "Artist/Model/Spawn/spawn_rock_large");
		m_dictData.Add(175, "Artist/Model/Spawn/spawn_rock_normal");
		m_dictData.Add(171, "Artist/Model/Spawn/spawn_venom_green");
		m_dictData.Add(172, "Artist/Model/Spawn/spawn_venom_ground_green");
		m_dictData.Add(173, "Artist/Model/Spawn/spawn_venom_yellow");
		m_dictData.Add(174, "Artist/Model/Spawn/spawn_venom_ground_yellow");
		m_dictData.Add(176, "Artist/Model/Spawn/spawn_venom_purple");
		m_dictData.Add(177, "Artist/Model/Spawn/spawn_venom_ground_purple");
		m_dictData.Add(178, "Artist/Model/Spawn/spawn_rock_small");
		m_dictData.Add(250, "Artist/Model/Items/Egg");
		m_dictData.Add(251, "Artist/Model/Items/Gold");
		m_dictData.Add(252, "Artist/Model/Items/Material");
		m_dictData.Add(300, "Artist/Model/BackPack/BackPackFly");
		m_dictData.Add(301, "Artist/Model/BackPack/BackBag");
		m_dictData.Add(302, "Artist/Model/Items/GoldEmitter");
		m_dictData.Add(1000, "Artist/Effect/Weapon/weapon_fire_01/weapon_fire_pfb");
		m_dictData.Add(1001, "Artist/Effect/Weapon/weapon_flash_01/weapon_flash_01_pfb");
		m_dictData.Add(1002, "Artist/Effect/Weapon/weapon_ice_01/weapon_ice_01_pfb");
		m_dictData.Add(1003, "Artist/Effect/Weapon/weapon_fire_01/weapon_fire_02_pfb");
		m_dictData.Add(1004, "Artist/Effect/Weapon/weapon_flash_01/weapon_flash_02_pfb");
		m_dictData.Add(1005, "Artist/Effect/Weapon/weapon_ice_01/weapon_ice_02_pfb");
		m_dictData.Add(1006, "Artist/Effect/Weapon/weapon_019_020/weapon_019_020_01_pfb");
		m_dictData.Add(1007, "Artist/Effect/Weapon/weapon_013_014/weapon_013_014_01_pfb");
		m_dictData.Add(1009, "Artist/Effect/Weapon/weapon_013_014/weapon_013_014_01_02_pfb");
		m_dictData.Add(1010, "Artist/Effect/Weapon/equip_004/equip_004_pfb");
		m_dictData.Add(1011, "Artist/Effect/Weapon/equip_001/equip_001_pfb");
		m_dictData.Add(1012, "Artist/Effect/Weapon/equip_003/equip_003_pfb");
		m_dictData.Add(1013, "Artist/Effect/Weapon/weapon_hellfire/weapon_hellfire_pfb");
		m_dictData.Add(1014, "Artist/Effect/Weapon/weapon_blizzard/weapon_blizzard_pfb");
		m_dictData.Add(1050, "Artist/Effect/Weapon/ammo_fire/ammo_fire_pfb02");
		m_dictData.Add(1051, "Artist/Effect/Weapon/ammo_flash/ammo_flash_pfb02");
		m_dictData.Add(1052, "Artist/Effect/Weapon/ammo_ice/ammo_ice_pfb02");
		m_dictData.Add(1053, "Artist/Effect/Weapon/ammo_common/ammo_common_pfb02");
		m_dictData.Add(1054, "Artist/EffectCustom/ammo_fire");
		m_dictData.Add(1058, "Artist/EffectCustom/ammo_fire_small");
		m_dictData.Add(1055, "Artist/EffectCustom/ammo_flash");
		m_dictData.Add(1056, "Artist/EffectCustom/ammo_ice");
		m_dictData.Add(1057, "Artist/EffectCustom/ammo_normal");
		m_dictData.Add(1060, "Artist/Effect/Weapon/ammo_rpg/ammo_rpg_03_pfb");
		m_dictData.Add(1061, "Artist/Effect/Weapon/ammo_rpg/ammo_rpg_01_pfb");
		m_dictData.Add(1062, "Artist/Effect/Weapon/ammo_rpg/ammo_rpg_02_pfb");
		m_dictData.Add(1100, "Artist/Effect/Weapon/hit_fire_01/hit_fire_pfb");
		m_dictData.Add(1101, "Artist/Effect/Weapon/hit_flash_01/hit_flash_01_pfb");
		m_dictData.Add(1102, "Artist/Effect/Weapon/hit_ice_01/hit_ice_01_pfb");
		m_dictData.Add(1103, "Artist/Effect/Weapon/hit_fire_01/hit_fire_02_pfb");
		m_dictData.Add(1104, "Artist/Effect/Weapon/hit_flash_01/hit_flash_02_pfb");
		m_dictData.Add(1105, "Artist/Effect/Weapon/hit_ice_01/hit_ice_02_pfb");
		m_dictData.Add(1106, "Artist/Effect/Weapon/weapon_019_020/weapon_019_020_03_pfb");
		m_dictData.Add(1107, "Artist/Effect/Weapon/weapon_013_014/weapon_013_014_03_pfb");
		m_dictData.Add(1108, "Artist/Effect/Weapon/hit_staff/hit_staff_pfb");
		m_dictData.Add(1109, "Artist/Effect/Weapon/hit_staff/hit_staff_fire_pfb");
		m_dictData.Add(1110, "Artist/Effect/Weapon/hit_common/hit_common_01_pfb");
		m_dictData.Add(1111, "Artist/Effect/Weapon/hit_common/hit_common_02_pfb");
		m_dictData.Add(1112, "Artist/Effect/Weapon/hit_staff/hit_staff_heavy_pfb");
		m_dictData.Add(1113, "Artist/Effect/Weapon/hit_staff/hit_staff_blood_pfb");
		m_dictData.Add(1115, "Artist/Effect/Weapon/hit_01/hit_01_02_pfb");
		m_dictData.Add(1116, "Artist/Effect/Weapon/hit_01/hit_01_pfb");
		m_dictData.Add(1150, "Artist/Effect/Weapon/staff_fire/staff_fire_pfb");
		m_dictData.Add(1151, "Artist/Effect/Weapon/firecrossbow/fireCrossbow_pfb");
		m_dictData.Add(1200, "Artist/Effect/Skill/HP_UP/HP_up_start");
		m_dictData.Add(1201, "Artist/Effect/Skill/HP_UP/HP_up_keep");
		m_dictData.Add(1202, "Artist/Effect/Skill/power_up/power_up_start");
		m_dictData.Add(1203, "Artist/Effect/Skill/power_up/power_up_keep");
		m_dictData.Add(1204, "Artist/Effect/Skill/Defense_UP/Defense_up_start");
		m_dictData.Add(1205, "Artist/EffectCustom/defence_keep");
		m_dictData.Add(1206, "Artist/Effect/Skill/stealth/stealth_pfb");
		m_dictData.Add(1300, "Artist/Effect/Character/Level_up/Level_up_pfb");
		m_dictData.Add(1301, "Artist/Effect/Item/item/item_04_pfb");
		m_dictData.Add(1302, "Artist/Effect/Character/Level_up/Level_up_female_pfb");
		m_dictData.Add(1351, "Artist/Effect/Character/roar/roar_pfb");
		m_dictData.Add(1400, "Artist/Effect/Skill/atk01/atk01_pfb");
		m_dictData.Add(1401, "Artist/Effect/Skill/atk02/atk02_pfb");
		m_dictData.Add(1402, "Artist/Effect/Skill/rush/rush");
		m_dictData.Add(1403, "Artist/Effect/Skill/shock/shock_01_pfb");
		m_dictData.Add(1404, "Artist/Effect/Character/charge_02/charge_02_01_pfb");
		m_dictData.Add(1405, "Artist/Effect/Character/charge_03/charge_03_01_pfb");
		m_dictData.Add(1406, "Artist/Effect/Skill/blink/blink_01_pfb");
		m_dictData.Add(1407, "Artist/Effect/Custom/velociraptor_blink_disappear");
		m_dictData.Add(1408, "Artist/Effect/Skill/strike/strike01_pfb");
		m_dictData.Add(1409, "Artist/Effect/Skill/stun/stun_pfb");
		m_dictData.Add(1500, "Artist/Effect/Character/anger/anger_eye_01");
		m_dictData.Add(1501, "Artist/Effect/Character/anger/anger_flash_01");
		m_dictData.Add(1900, "Artist/Effect/Skill/atk01_hit/atk01_hit_pfb");
		m_dictData.Add(1901, "Artist/Effect/Skill/atk01_hit/atk01_03_hit_pfb");
		m_dictData.Add(1902, "Artist/Effect/Skill/atk01_hit/atk01_02_hit_pfb");
		m_dictData.Add(1903, "Artist/Effect/Character/charge/charge_pfb");
		m_dictData.Add(1904, "Artist/Effect/Character/death/death_01_pfb");
		m_dictData.Add(1905, "Artist/Effect/Character/death/death_02_pfb");
		m_dictData.Add(1906, "Artist/Effect/Character/tail_gas_01/tail_gas_pfb");
		m_dictData.Add(1907, "Artist/Effect/Skill/venom_yellow/venom02_01_yellow_fire_pfb");
		m_dictData.Add(1908, "Artist/Effect/Skill/venom_yellow/venom02_04_yellow_hitbody_pfb");
		m_dictData.Add(1909, "Artist/Effect/Skill/venom_yellow/venom02_03_yellow_hit_pfb");
		m_dictData.Add(1910, "Artist/Effect/Skill/venom_green/venom01_green_fire_pfb");
		m_dictData.Add(1911, "Artist/Effect/Skill/venom_green/venom04_green_hitbody_pfb");
		m_dictData.Add(1912, "Artist/Effect/Skill/venom_green/venom03_green_hit_pfb");
		m_dictData.Add(1913, "Artist/EffectCustom/snow_walker_step");
		m_dictData.Add(1914, "Artist/EffectCustom/snow_monster_step");
		m_dictData.Add(1915, "Artist/Effect/Skill/fart/fart_pfb");
		m_dictData.Add(1916, "Artist/Effect/Skill/sigh/sigh_pfb");
		m_dictData.Add(1917, "Artist/Effect/Skill/venom_purple/venom01_purple_fire_pfb");
		m_dictData.Add(1918, "Artist/Effect/Skill/venom_purple/venom04_purple_hitbody_pfb");
		m_dictData.Add(1919, "Artist/Effect/Skill/venom_purple/venom03_purple_hit_pfb");
		m_dictData.Add(2000, "Artist/GameUI/Task/NGUITaskTimeLimit");
		m_dictData.Add(2001, "Artist/GameUI/Task/NGUITaskHunter");
		m_dictData.Add(2002, "Artist/GameUI/Task/NGUITaskHunterList");
		m_dictData.Add(2003, "Artist/GameUI/Task/NGUITaskCollect");
		m_dictData.Add(2004, "Artist/GameUI/Task/NGUITaskCollectList");
		m_dictData.Add(2005, "Artist/GameUI/Task/NGUITaskSurvival");
		m_dictData.Add(2006, "Artist/GameUI/Task/NGUITaskDefence");
		m_dictData.Add(2007, "Artist/GameUI/Task/NGUITaskButcher");
		m_dictData.Add(2008, "Artist/GameUI/Task/NGUITaskInfinite");
		m_dictData.Add(2100, "Artist/GameUI/dlgProtratiTeamMate");
		m_dictData.Add(2101, "Artist/GameUI/LifeBar");
		m_dictData.Add(2102, "Artist/GameUI/txtDamageNormal");
		m_dictData.Add(2103, "Artist/GameUI/txtDamageCritical");
		m_dictData.Add(2104, "Artist/GameUI/txtExp");
		m_dictData.Add(2015, "Artist/GameUI/dlgWaveStart");
		m_dictData.Add(2900, "Artist/GameUI/txtMissionCompleted");
		m_dictData.Add(2901, "Artist/GameUI/txtMissionFailed");
		m_dictData.Add(3000, "_Config/ai");
		m_dictData.Add(3001, "_Config/mob");
		m_dictData.Add(3002, "_Config/skillplayer");
		m_dictData.Add(3003, "_Config/skillmonster");
		m_dictData.Add(3004, "_Config/weapon");
		m_dictData.Add(3005, "_Config/buff");
		m_dictData.Add(3006, "_Config/gamewave");
		m_dictData.Add(3007, "_Config/gamelevel");
		m_dictData.Add(3008, "_Config/task");
		m_dictData.Add(3009, "_Config/item");
		m_dictData.Add(3010, "_Config/recipe");
		m_dictData.Add(3011, "_Config/character");
		m_dictData.Add(3012, "_Config/battlegroup");
		m_dictData.Add(3013, "_Config/dropgroup");
		m_dictData.Add(4000, "Artist/Custom/RoadSignPath");
	}

	private static GameObject Load(int nPrefabPath)
	{
		if (!m_dictData.ContainsKey(nPrefabPath))
		{
			return null;
		}
		GameObject gameObject = (GameObject)Resources.Load(m_dictData[nPrefabPath]);
		if (gameObject == null)
		{
			return null;
		}
		m_dictCache.Add(nPrefabPath, gameObject);
		return gameObject;
	}

	private static GameObject Load(string sPath)
	{
		GameObject gameObject = (GameObject)Resources.Load(sPath);
		if (gameObject == null)
		{
			return null;
		}
		m_dictCacheByPath.Add(sPath, gameObject);
		return gameObject;
	}

	public static void Destroy(int nPrefabPath)
	{
		if (m_dictCache.ContainsKey(nPrefabPath))
		{
			Object.Destroy(m_dictCache[nPrefabPath]);
			m_dictCache.Remove(nPrefabPath);
		}
	}

	public static void DestroyAll()
	{
		m_dictCache.Clear();
		m_dictCacheByPath.Clear();
	}

	public static void PreLoad()
	{
		Debug.Log("PreLoadGameEffect");
		AddPool(302, 5);
		AddPool(251, 10);
		AddPool(252, 10);
		AddPool(1301, 10);
		AddPool(1351);
	}

	public static void DestroyPreLoad()
	{
		foreach (CPoolManage value in m_dictPool.Values)
		{
			value.Destroy();
		}
		m_dictPool.Clear();
	}

	public static void AddPool(int nPrefabPath, int nCount = 0)
	{
		if (mPreLoadNode == null)
		{
			GameObject gameObject = GameObject.Find("GamePreLoad");
			if (gameObject == null)
			{
				gameObject = new GameObject("GamePreLoad");
			}
			if (gameObject != null)
			{
				mPreLoadNode = gameObject.transform;
			}
		}
		CPoolManage cPoolManage = null;
		if (!m_dictPool.ContainsKey(nPrefabPath))
		{
			cPoolManage = new CPoolManage();
			m_dictPool.Add(nPrefabPath, cPoolManage);
		}
		else
		{
			cPoolManage = m_dictPool[nPrefabPath];
		}
		if (cPoolManage != null)
		{
			cPoolManage.Initialize(GetPath(nPrefabPath), mPreLoadNode, null, nCount);
		}
	}
}
