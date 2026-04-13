using System.Collections.Generic;
using UnityEngine;
using gyAchievementSystem;
using gyIAPSystem;

public class iGameApp
{
	public static iGameApp m_Instance;

	public DebugGUI m_Debug;

	public iGizmos m_Gizmos;

	public iGameSceneBase m_GameScene;

	public iGameState m_GameState;

	public iGameData m_GameData;

	public iClearMemory m_ClearMemory;

	public static iGameApp GetInstance()
	{
		if (m_Instance == null)
		{
			m_Instance = new iGameApp();
			m_Instance.Initialize();
		}
		return m_Instance;
	}

	public void Initialize()
	{
		MyUtils.SimulatePlatform = PlatformEnum.IOS;
		GameObject gameObject = new GameObject("_GizmosManager");
		if (gameObject != null)
		{
			gameObject.transform.position = Vector3.zero;
			gameObject.transform.rotation = Quaternion.identity;
			m_Gizmos = gameObject.AddComponent<iGizmos>();
		}
		gameObject = new GameObject("_DebugGUI");
		if (gameObject != null)
		{
			gameObject.transform.position = Vector3.zero;
			gameObject.transform.rotation = Quaternion.identity;
			m_Debug = gameObject.AddComponent<DebugGUI>();
		}
		gameObject = new GameObject("_ClearMemoryObject");
		if (gameObject != null)
		{
			gameObject.transform.position = Vector3.zero;
			gameObject.transform.rotation = Quaternion.identity;
			m_ClearMemory = gameObject.AddComponent<iClearMemory>();
		}
		PrefabManager.Initialize();
		m_GameData = new iGameData();
		m_GameData.Load();
		m_GameState = new iGameState();
		m_GameState.Initialize();
		CUISound.GetInstance().PreloadAlwaysSounds();
		CheckUnLock(true);
		Screen.autorotateToLandscapeLeft = true;
		Screen.autorotateToLandscapeRight = true;
		//CFlurryManager.GetInstance().Initialize("TWV7XMZQQKTHZ38ZH93V");
		//CFlurryManager.GetInstance().EnterApp();
		CLocalNotification.GetInstance().AddLocalNotification("Grab your weapon and hunt dinosaurs for rewards!", iMacroDefine.SecondsOneDay * 3);
		CLocalNotification.GetInstance().AddLocalNotification("Grab your weapon and hunt dinosaurs for rewards!", iMacroDefine.SecondsOneDay * 7);
		CLocalNotification.GetInstance().AddLocalNotification("Your villagers need you to help them forge weapons against dinosaurs!", iMacroDefine.SecondsOneDay * 14);
		CLocalNotification.GetInstance().AddLocalNotification("Dinosaurs are running wild! Return to tame those fierce beasts!", iMacroDefine.SecondsOneDay * 21);
		//OpenClikPlugin.Initialize("9B11A42C-3F34-4897-8D16-F6A49E5CC750");
	}

	public void Destroy()
	{
	}

	public string GetKey()
	{
		return "fuckbreakitandbethesuperman";
	}

	public void EnterScene(string sName)
	{
		m_GameState.CurScene = kGameSceneEnum.OutOfGame;
		m_GameState.m_sLoadScene = sName;
		Application.LoadLevel("SceneLoad");
		CUISound.GetInstance().ScheduleUnloadAfterSceneChange(5f);
	}

	public void EnterScene(kGameSceneEnum gotoscene)
	{
		Time.timeScale = 1f;
		if (m_GameState.CurScene == kGameSceneEnum.Game)
		{
			DestroyScene();
		}
		CUISound.GetInstance().ScheduleUnloadAfterSceneChange(5f);
		Debug.Log("play theme " + gotoscene);
		switch (gotoscene)
		{
		case kGameSceneEnum.Game:
		{
			CUISound.GetInstance().Stop("BGM_theme");
			GameLevelInfo gameLevelInfo = m_GameData.GetGameLevelInfo(m_GameState.GameLevel);
			if (gameLevelInfo != null)
			{
				m_GameState.CurScene = kGameSceneEnum.Game;
				m_GameState.m_sLoadScene = gameLevelInfo.sSceneName;
				Application.LoadLevel("SceneLoad");
				CAchievementManager.GetInstance().Save();
			}
			break;
		}
		case kGameSceneEnum.Map:
			CUISound.GetInstance().Play("BGM_theme");
			m_GameState.CurScene = kGameSceneEnum.Map;
			m_GameState.m_sLoadScene = "Scene_Map";
			Application.LoadLevel("SceneLoad");
			break;
		case kGameSceneEnum.Room:
			m_GameState.CurScene = kGameSceneEnum.Room;
			Application.LoadLevelAsync("SceneRoom");
			break;
		case kGameSceneEnum.Home:
			CUISound.GetInstance().Play("BGM_theme");
			m_GameState.CurScene = kGameSceneEnum.Home;
			m_GameState.m_sLoadScene = "Scene_MainMenu";
			Application.LoadLevel("SceneLoad");
			CAchievementManager.GetInstance().Save();
			break;
		}
	}

	public void CreateScene()
	{
		iGameData gameData = GetInstance().m_GameData;
		if (gameData == null)
		{
			return;
		}
		iDataCenter dataCenter = gameData.GetDataCenter();
		if (dataCenter != null)
		{
			m_GameState.Reset();
			for (int i = 0; i < 3; i++)
			{
				CarryWeapon(i, dataCenter.GetSelectWeapon(i));
			}
			int gameLevel = m_GameState.GameLevel;
			switch (gameLevel)
			{
			case 0:
				m_GameScene = new iGameScene0();
				break;
			case 1:
				m_GameScene = new iGameScene1();
				break;
			case 2:
				m_GameScene = new iGameScene2();
				break;
			default:
				m_GameScene = new iGameSceneBase();
				break;
			}
			if (m_GameScene != null)
			{
				m_GameScene.Initialize();
				m_GameScene.InitializeGameLevel(gameLevel);
				m_GameScene.StartGame();
				PrefabManager.PreLoad();
			}
		}
	}

	public void DestroyScene()
	{
		if (m_GameScene != null)
		{
			m_GameScene.Destroy();
			m_GameScene = null;
			PrefabManager.DestroyPreLoad();
			PrefabManager.DestroyAll();
		}
	}

	public void ResetScene()
	{
	}

	public void Update(float deltaTime)
	{
		if (m_GameScene != null)
		{
			m_GameScene.Update(deltaTime);
		}
	}

	public void FixedUpdate(float deltaTime)
	{
		if (m_GameScene != null)
		{
			m_GameScene.FixedUpdate(deltaTime);
		}
	}

	public void LateUpdate(float deltaTime)
	{
		if (m_GameScene != null)
		{
			m_GameScene.LateUpdate(deltaTime);
		}
	}

	public void CarryWeapon(int nIndex, int nWeaponID)
	{
		iDataCenter dataCenter = m_GameData.GetDataCenter();
		if (dataCenter == null)
		{
			return;
		}
		int weaponLevel = dataCenter.GetWeaponLevel(nWeaponID);
		if (weaponLevel < 1)
		{
			return;
		}
		CWeaponInfoLevel weaponInfo = m_GameData.GetWeaponInfo(nWeaponID, weaponLevel);
		if (weaponInfo != null)
		{
			CWeaponBase cWeaponBase = null;
			switch (weaponInfo.nAttackMode)
			{
			case 1:
				cWeaponBase = new CWeaponMelee();
				break;
			case 2:
				cWeaponBase = new CWeaponShoot();
				break;
			case 3:
				cWeaponBase = new CWeaponSpawn();
				break;
			case 4:
				cWeaponBase = new CWeaponSpawnWithHead();
				break;
			case 5:
				cWeaponBase = new CWeaponHoldy();
				break;
			case 6:
				cWeaponBase = new CWeaponShotgun();
				break;
			}
			if (cWeaponBase != null)
			{
				cWeaponBase.Initialize(nWeaponID, weaponLevel);
				m_GameState.CarryWeapon(nIndex, cWeaponBase);
			}
		}
	}

	public void SetGizmosPoint(string sKey, Vector3 p, Color color)
	{
		if (!(m_Gizmos == null))
		{
			m_Gizmos.SetPoint(sKey, p, color);
		}
	}

	public void SetGizmosLine(string sKey, Vector3 p1, Vector3 p2, Color color)
	{
		if (!(m_Gizmos == null))
		{
			m_Gizmos.SetLine(sKey, p1, p2, color);
		}
	}

	public void SetGizmosRay(string sKey, Vector3 p, Vector3 dir, Color color)
	{
		if (!(m_Gizmos == null))
		{
			m_Gizmos.SetRay(sKey, p, dir, color);
		}
	}

	public void ScreenLog(string str)
	{
	}

	public void ClearScreenLog()
	{
		if (!(m_Debug == null))
		{
			m_Debug.Clear();
		}
	}

	public void ClearMemory()
	{
		if (!(m_ClearMemory == null))
		{
			m_ClearMemory.ClearMemory();
		}
	}

	public void CheckUnLock(bool bFirst = false)
	{
		if (m_GameData == null || m_GameState == null)
		{
			return;
		}
		iDataCenter dataCenter = m_GameData.GetDataCenter();
		if (dataCenter == null || !m_GameState.isCheckUnLock)
		{
			return;
		}
		m_GameState.isCheckUnLock = false;
		bool flag = false;
		iCharacterCenter characterCenter = m_GameData.GetCharacterCenter();
		if (characterCenter != null)
		{
			Dictionary<int, CCharacterInfo> data = characterCenter.GetData();
			if (data != null)
			{
				foreach (CCharacterInfo value in data.Values)
				{
					if (dataCenter.GetCharacter(value.nID) == null && dataCenter.IsLevelPassed(value.nUnLockLevel))
					{
						dataCenter.SetCharacterSign(value.nID, 1);
						dataCenter.UnlockCharacter(value.nID);
						if (!bFirst)
						{
							dataCenter.UnLockSignType = 1;
							dataCenter.UnLockSignID = value.nID;
						}
						flag = true;
						ScreenLog("unlock character " + value.nID);
					}
				}
			}
		}
		int[] array = new int[5] { 1, 2, 3, 4, 5 };
		for (int i = 0; i < array.Length; i++)
		{
			CCharSaveInfo character = dataCenter.GetCharacter(array[i]);
			if (character == null || character.nLevel == -1)
			{
				continue;
			}
			CCharacterInfo characterInfo = m_GameData.GetCharacterInfo(array[i]);
			if (characterInfo == null || characterInfo.ltCharacterPassiveSkill == null)
			{
				continue;
			}
			foreach (int item in characterInfo.ltCharacterPassiveSkill)
			{
				CSkillInfo skillInfo = m_GameData.GetSkillInfo(item);
				if (skillInfo == null)
				{
					continue;
				}
				int nSkillLevel = 0;
				if (!dataCenter.GetPassiveSkill(skillInfo.nID, ref nSkillLevel) && character.nLevel >= skillInfo.nUnlockLevel)
				{
					dataCenter.SetSkillSign(skillInfo.nID, 1);
					dataCenter.UnlockPassiveSkill(skillInfo.nID);
					if (!bFirst && dataCenter.UnLockSignType == 0)
					{
						dataCenter.UnLockSignType = 2;
						dataCenter.UnLockSignID = skillInfo.nID;
					}
					flag = true;
					ScreenLog("unlock skill " + skillInfo.nID);
				}
			}
		}
		iWeaponCenter weaponCenter = m_GameData.GetWeaponCenter();
		if (weaponCenter != null)
		{
			Dictionary<int, CWeaponInfo> data2 = weaponCenter.GetData();
			if (data2 != null)
			{
				foreach (CWeaponInfo value2 in data2.Values)
				{
					if (dataCenter.GetWeaponLevel(value2.nID) > 0)
					{
						continue;
					}
					int nSignState = 0;
					if (dataCenter.GetWeaponSign(value2.nID, ref nSignState) && nSignState != 0)
					{
						continue;
					}
					CWeaponInfoLevel cWeaponInfoLevel = value2.Get(1);
					if (cWeaponInfoLevel == null)
					{
						continue;
					}
					for (int j = 0; j < cWeaponInfoLevel.ltMaterials.Count; j++)
					{
						if (dataCenter.GetMaterialNum(cWeaponInfoLevel.ltMaterials[j]) > 0)
						{
							dataCenter.SetWeaponSign(value2.nID, 1);
							if (!bFirst)
							{
							}
							flag = true;
							ScreenLog("sign weapon " + value2.nID);
							break;
						}
					}
				}
			}
		}
		iItemCenter itemCenter = m_GameData.GetItemCenter();
		if (itemCenter != null)
		{
			Dictionary<int, CItemInfo> data3 = itemCenter.GetData();
			if (data3 != null)
			{
				foreach (CItemInfo value3 in data3.Values)
				{
					CItemInfoLevel cItemInfoLevel = value3.Get(1);
					if (cItemInfoLevel == null || cItemInfoLevel.nType != 1)
					{
						continue;
					}
					int nItemLevel = 0;
					if (dataCenter.GetEquipStone(value3.nID, ref nItemLevel))
					{
						continue;
					}
					int nSignState2 = 0;
					if (dataCenter.GetEquipStoneSign(value3.nID, ref nSignState2) && nSignState2 != 0)
					{
						continue;
					}
					for (int k = 0; k < cItemInfoLevel.ltMaterials.Count; k++)
					{
						if (dataCenter.GetMaterialNum(cItemInfoLevel.ltMaterials[k]) > 0)
						{
							dataCenter.SetEquipStoneSign(value3.nID, 1);
							if (!bFirst)
							{
							}
							flag = true;
							ScreenLog("sign equipstone " + value3.nID);
							break;
						}
					}
				}
			}
		}
		if (flag)
		{
			ScreenLog("===============================");
			dataCenter.Save();
		}
	}

	public bool CheckAchieveReward()
	{
		CAchievementCenter achievementCenter = CAchievementManager.GetInstance().GetAchievementCenter();
		if (achievementCenter == null)
		{
			return false;
		}
		Dictionary<int, CAchievementData> dataData = achievementCenter.GetDataData();
		if (dataData == null)
		{
			return false;
		}
		foreach (CAchievementData value in dataData.Values)
		{
			int achiStar = CAchievementManager.GetInstance().GetAchiStar(value.nID);
			if (achiStar != 0)
			{
				Debug.Log(value.nID + " has " + achiStar + " stars");
				if (!value.IsGotReward(achiStar - 1))
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool CheckWeaponMaterialEnough(int nWeaponID = -1)
	{
		iDataCenter dataCenter = m_GameData.GetDataCenter();
		if (dataCenter == null)
		{
			return false;
		}
		if (nWeaponID != -1)
		{
			CWeaponInfo weaponInfo = m_GameData.GetWeaponInfo(nWeaponID);
			if (weaponInfo == null)
			{
				return false;
			}
			CLevelUpWeapon cLevelUpWeapon = new CLevelUpWeapon();
			if (cLevelUpWeapon == null)
			{
				return false;
			}
			cLevelUpWeapon.Initialize(nWeaponID);
			if (!cLevelUpWeapon.IsMaterialsMatch())
			{
				return false;
			}
			return true;
		}
		iWeaponCenter weaponCenter = m_GameData.GetWeaponCenter();
		if (weaponCenter == null)
		{
			return false;
		}
		Dictionary<int, CWeaponInfo> data = weaponCenter.GetData();
		if (data == null)
		{
			return false;
		}
		CLevelUpWeapon cLevelUpWeapon2 = new CLevelUpWeapon();
		if (cLevelUpWeapon2 == null)
		{
			return false;
		}
		foreach (CWeaponInfo value in data.Values)
		{
			cLevelUpWeapon2.Initialize(value.nID);
			if (cLevelUpWeapon2.IsMaterialsMatch())
			{
				return true;
			}
		}
		return false;
	}

	public bool CheckEquipStoneMaterialEnough(int nEquip = -1)
	{
		iDataCenter dataCenter = m_GameData.GetDataCenter();
		if (dataCenter == null)
		{
			return false;
		}
		if (nEquip != -1)
		{
			CItemInfo itemInfo = m_GameData.GetItemInfo(nEquip);
			if (itemInfo == null)
			{
				return false;
			}
			CLevelUpEquip cLevelUpEquip = new CLevelUpEquip();
			if (cLevelUpEquip == null)
			{
				return false;
			}
			cLevelUpEquip.Initialize(nEquip);
			if (!cLevelUpEquip.IsMaterialsMatch())
			{
				return false;
			}
			return true;
		}
		iItemCenter itemCenter = m_GameData.GetItemCenter();
		if (itemCenter == null)
		{
			return false;
		}
		Dictionary<int, CItemInfo> data = itemCenter.GetData();
		if (data == null)
		{
			return false;
		}
		CLevelUpEquip cLevelUpEquip2 = new CLevelUpEquip();
		if (cLevelUpEquip2 == null)
		{
			return false;
		}
		foreach (CItemInfo value in data.Values)
		{
			cLevelUpEquip2.Initialize(value.nID);
			if (cLevelUpEquip2.IsMaterialsMatch())
			{
				return true;
			}
		}
		return false;
	}

	public bool CheckCharacterMaterialEnough(int nCharacterID = -1)
	{
		iDataCenter dataCenter = m_GameData.GetDataCenter();
		if (dataCenter == null)
		{
			return false;
		}
		if (nCharacterID != -1)
		{
			CCharacterInfo characterInfo = m_GameData.GetCharacterInfo(nCharacterID);
			if (characterInfo == null)
			{
				return false;
			}
			CCharSaveInfo character = dataCenter.GetCharacter(nCharacterID);
			if (character == null || character.nLevel != -1)
			{
				return false;
			}
			if ((characterInfo.isCrystalPurchase && dataCenter.Crystal < characterInfo.nPurchasePrice) || (!characterInfo.isCrystalPurchase && dataCenter.Gold < characterInfo.nPurchasePrice))
			{
				return false;
			}
			return true;
		}
		iCharacterCenter characterCenter = m_GameData.GetCharacterCenter();
		if (characterCenter == null)
		{
			return false;
		}
		Dictionary<int, CCharacterInfo> data = characterCenter.GetData();
		if (data == null)
		{
			return false;
		}
		foreach (CCharacterInfo value in data.Values)
		{
			CCharSaveInfo character2 = dataCenter.GetCharacter(value.nID);
			if (character2 == null || character2.nLevel != -1 || ((!value.isCrystalPurchase || dataCenter.Crystal < value.nPurchasePrice) && (value.isCrystalPurchase || dataCenter.Gold < value.nPurchasePrice)))
			{
				continue;
			}
			return true;
		}
		return false;
	}

	public bool CheckSkillMaterialEnough(int nSkillID = -1)
	{
		iDataCenter dataCenter = m_GameData.GetDataCenter();
		if (dataCenter == null)
		{
			return false;
		}
		if (nSkillID != -1)
		{
			CSkillInfo skillInfo = m_GameData.GetSkillInfo(nSkillID);
			if (skillInfo == null)
			{
				return false;
			}
			int nSkillLevel = 0;
			if (!dataCenter.GetPassiveSkill(nSkillID, ref nSkillLevel))
			{
				return false;
			}
			int nLevel = ((nSkillLevel == -1) ? 1 : (nSkillLevel + 1));
			CSkillInfoLevel cSkillInfoLevel = skillInfo.Get(nLevel);
			if (cSkillInfoLevel == null)
			{
				return false;
			}
			if ((cSkillInfoLevel.isCrystalPurchase && dataCenter.Crystal < cSkillInfoLevel.nPurchasePrice) || (!cSkillInfoLevel.isCrystalPurchase && dataCenter.Gold < cSkillInfoLevel.nPurchasePrice))
			{
				return false;
			}
			return true;
		}
		iSkillCenter skillCenter = m_GameData.GetSkillCenter();
		if (skillCenter == null)
		{
			return false;
		}
		Dictionary<int, CSkillInfo> dataSkillInfo = skillCenter.GetDataSkillInfo();
		if (dataSkillInfo == null)
		{
			return false;
		}
		foreach (CSkillInfo value in dataSkillInfo.Values)
		{
			int nSkillLevel2 = 0;
			if (dataCenter.GetPassiveSkill(value.nID, ref nSkillLevel2))
			{
				int nLevel2 = ((nSkillLevel2 == -1) ? 1 : (nSkillLevel2 + 1));
				CSkillInfoLevel cSkillInfoLevel2 = value.Get(nLevel2);
				if (cSkillInfoLevel2 != null && ((cSkillInfoLevel2.isCrystalPurchase && dataCenter.Crystal >= cSkillInfoLevel2.nPurchasePrice) || (!cSkillInfoLevel2.isCrystalPurchase && dataCenter.Gold >= cSkillInfoLevel2.nPurchasePrice)))
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool CheckWeaponSignState(int nSignState, int nWeaponID = -1)
	{
		iDataCenter dataCenter = m_GameData.GetDataCenter();
		if (dataCenter == null)
		{
			return false;
		}
		if (nWeaponID != -1)
		{
			int nSignState2 = 0;
			if (!dataCenter.GetWeaponSign(nWeaponID, ref nSignState2) || nSignState2 != nSignState)
			{
				return false;
			}
			return true;
		}
		iWeaponCenter weaponCenter = m_GameData.GetWeaponCenter();
		if (weaponCenter == null)
		{
			return false;
		}
		Dictionary<int, CWeaponInfo> data = weaponCenter.GetData();
		if (data == null)
		{
			return false;
		}
		foreach (CWeaponInfo value in data.Values)
		{
			int nSignState3 = 0;
			if (dataCenter.GetWeaponSign(value.nID, ref nSignState3) && nSignState3 == nSignState)
			{
				return true;
			}
		}
		return false;
	}

	public bool CheckEquipStoneSignState(int nSignState, int nEquip = -1)
	{
		iDataCenter dataCenter = m_GameData.GetDataCenter();
		if (dataCenter == null)
		{
			return false;
		}
		if (nEquip != -1)
		{
			int nSignState2 = 0;
			if (!dataCenter.GetEquipStoneSign(nEquip, ref nSignState2) || nSignState2 != nSignState)
			{
				return false;
			}
			return true;
		}
		iItemCenter itemCenter = m_GameData.GetItemCenter();
		if (itemCenter == null)
		{
			return false;
		}
		Dictionary<int, CItemInfo> data = itemCenter.GetData();
		if (data == null)
		{
			return false;
		}
		foreach (CItemInfo value in data.Values)
		{
			int nSignState3 = 0;
			if (dataCenter.GetEquipStoneSign(value.nID, ref nSignState3) && nSignState3 == nSignState)
			{
				return true;
			}
		}
		return false;
	}

	public bool CheckCharacterSignState(int nSignState, int nCharacterID = -1)
	{
		iDataCenter dataCenter = m_GameData.GetDataCenter();
		if (dataCenter == null)
		{
			return false;
		}
		if (nCharacterID != -1)
		{
			int nSignState2 = 0;
			if (!dataCenter.GetCharacterSign(nCharacterID, ref nSignState2) || nSignState2 != nSignState)
			{
				return false;
			}
			return true;
		}
		iCharacterCenter characterCenter = m_GameData.GetCharacterCenter();
		if (characterCenter == null)
		{
			return false;
		}
		Dictionary<int, CCharacterInfo> data = characterCenter.GetData();
		if (data == null)
		{
			return false;
		}
		foreach (CCharacterInfo value in data.Values)
		{
			int nSignState3 = 0;
			if (dataCenter.GetCharacterSign(value.nID, ref nSignState3) && nSignState3 == nSignState)
			{
				return true;
			}
		}
		return false;
	}

	public bool CheckSkillSignState(int nSignState, int nSkillID = -1)
	{
		iDataCenter dataCenter = m_GameData.GetDataCenter();
		if (dataCenter == null)
		{
			return false;
		}
		if (nSkillID != -1)
		{
			int nSignState2 = 0;
			if (!dataCenter.GetSkillSign(nSkillID, ref nSignState2) || nSignState2 != nSignState)
			{
				return false;
			}
			return true;
		}
		int[] array = new int[5] { 1, 2, 3, 4, 5 };
		for (int i = 0; i < array.Length; i++)
		{
			CCharSaveInfo character = dataCenter.GetCharacter(array[i]);
			if (character == null)
			{
				continue;
			}
			CCharacterInfo characterInfo = m_GameData.GetCharacterInfo(array[i]);
			if (characterInfo == null || characterInfo.ltCharacterPassiveSkill == null)
			{
				continue;
			}
			foreach (int item in characterInfo.ltCharacterPassiveSkill)
			{
				CSkillInfo skillInfo = m_GameData.GetSkillInfo(item);
				if (skillInfo == null)
				{
					continue;
				}
				CSkillInfoLevel cSkillInfoLevel = skillInfo.Get(1);
				if (cSkillInfoLevel != null && cSkillInfoLevel.nType == 1)
				{
					int nSignState3 = 0;
					if (dataCenter.GetSkillSign(skillInfo.nID, ref nSignState3) && nSignState3 == nSignState)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public void Flurry_EnterStage(int nLevelID)
	{
		/*iDataCenter dataCenter = m_GameData.GetDataCenter();
		if (dataCenter == null)
		{
			return;
		}
		GameLevelInfo gameLevelInfo = m_GameData.GetGameLevelInfo(nLevelID);
		if (gameLevelInfo == null)
		{
			return;
		}
		CCharacterInfo characterInfo = m_GameData.GetCharacterInfo(dataCenter.CurCharID);
		if (characterInfo == null)
		{
			return;
		}
		CCharSaveInfo character = dataCenter.GetCharacter(dataCenter.CurCharID);
		if (character == null || character.nLevel == -1)
		{
			return;
		}
		CCharacterInfoLevel cCharacterInfoLevel = characterInfo.Get(character.nLevel);
		if (cCharacterInfoLevel == null)
		{
			return;
		}
		CFlurryManager.CEnterStageInfo cEnterStageInfo = new CFlurryManager.CEnterStageInfo();
		if (cEnterStageInfo == null)
		{
			return;
		}
		cEnterStageInfo.sCharID = characterInfo.nID + "_" + cCharacterInfoLevel.sName;
		cEnterStageInfo.sCharLevel = character.nLevel.ToString();
		cEnterStageInfo.arrWeaponID = new string[3];
		for (int i = 0; i < 3; i++)
		{
			int selectWeapon = dataCenter.GetSelectWeapon(i);
			int weaponLevel = dataCenter.GetWeaponLevel(selectWeapon);
			CWeaponInfoLevel weaponInfo = m_GameData.GetWeaponInfo(selectWeapon, weaponLevel);
			if (weaponInfo != null)
			{
				cEnterStageInfo.arrWeaponID[i] = selectWeapon + "_" + weaponInfo.sName;
			}
			else
			{
				cEnterStageInfo.arrWeaponID[i] = "Empty";
			}
		}
		cEnterStageInfo.arrSkillID = new string[3];
		for (int j = 0; j < 3; j++)
		{
			int selectPassiveSkill = dataCenter.GetSelectPassiveSkill(characterInfo.nID, j);
			int nSkillLevel = 0;
			dataCenter.GetPassiveSkill(selectPassiveSkill, ref nSkillLevel);
			CSkillInfoLevel skillInfo = m_GameData.GetSkillInfo(selectPassiveSkill, nSkillLevel);
			if (skillInfo != null)
			{
				cEnterStageInfo.arrSkillID[j] = selectPassiveSkill + "_" + skillInfo.sName;
			}
			else
			{
				cEnterStageInfo.arrSkillID[j] = "Empty";
			}
		}
		int curEquipStone = dataCenter.CurEquipStone;
		int nItemLevel = 0;
		dataCenter.GetEquipStone(curEquipStone, ref nItemLevel);
		CItemInfoLevel itemInfo = m_GameData.GetItemInfo(curEquipStone, nItemLevel);
		if (itemInfo != null)
		{
			cEnterStageInfo.sEquipStoneID = curEquipStone + "_" + itemInfo.sName;
		}
		else
		{
			cEnterStageInfo.sEquipStoneID = "Empty";
		}
		cEnterStageInfo.sLevelID = gameLevelInfo.nID + "_" + gameLevelInfo.sLevelName;
		cEnterStageInfo.nLevelProccess = dataCenter.SceneProccess;
		CFlurryManager.GetInstance().EnterStage(cEnterStageInfo.sLevelID, cEnterStageInfo);
		CFlurryManager.GetInstance().EnterStage("ALL Stage", cEnterStageInfo);
	}

	public void Flurry_LoseStage(int nLevelID)
	{
		iDataCenter dataCenter = m_GameData.GetDataCenter();
		if (dataCenter == null)
		{
			return;
		}
		GameLevelInfo gameLevelInfo = m_GameData.GetGameLevelInfo(nLevelID);
		if (gameLevelInfo == null)
		{
			return;
		}
		CCharacterInfo characterInfo = m_GameData.GetCharacterInfo(dataCenter.CurCharID);
		if (characterInfo == null)
		{
			return;
		}
		CCharSaveInfo character = dataCenter.GetCharacter(dataCenter.CurCharID);
		if (character == null || character.nLevel == -1)
		{
			return;
		}
		CCharacterInfoLevel cCharacterInfoLevel = characterInfo.Get(character.nLevel);
		if (cCharacterInfoLevel == null)
		{
			return;
		}
		CFlurryManager.CEnterStageInfo cEnterStageInfo = new CFlurryManager.CEnterStageInfo();
		if (cEnterStageInfo == null)
		{
			return;
		}
		cEnterStageInfo.sCharID = characterInfo.nID + "_" + cCharacterInfoLevel.sName;
		cEnterStageInfo.sCharLevel = character.nLevel.ToString();
		cEnterStageInfo.arrWeaponID = new string[3];
		for (int i = 0; i < 3; i++)
		{
			int selectWeapon = dataCenter.GetSelectWeapon(i);
			int weaponLevel = dataCenter.GetWeaponLevel(selectWeapon);
			CWeaponInfoLevel weaponInfo = m_GameData.GetWeaponInfo(selectWeapon, weaponLevel);
			if (weaponInfo != null)
			{
				cEnterStageInfo.arrWeaponID[i] = selectWeapon + "_" + weaponInfo.sName;
			}
			else
			{
				cEnterStageInfo.arrWeaponID[i] = "Empty";
			}
		}
		cEnterStageInfo.arrSkillID = new string[3];
		for (int j = 0; j < 3; j++)
		{
			int selectPassiveSkill = dataCenter.GetSelectPassiveSkill(characterInfo.nID, j);
			int nSkillLevel = 0;
			dataCenter.GetPassiveSkill(selectPassiveSkill, ref nSkillLevel);
			CSkillInfoLevel skillInfo = m_GameData.GetSkillInfo(selectPassiveSkill, nSkillLevel);
			if (skillInfo != null)
			{
				cEnterStageInfo.arrSkillID[j] = selectPassiveSkill + "_" + skillInfo.sName;
			}
			else
			{
				cEnterStageInfo.arrSkillID[j] = "Empty";
			}
		}
		int curEquipStone = dataCenter.CurEquipStone;
		int nItemLevel = 0;
		dataCenter.GetEquipStone(curEquipStone, ref nItemLevel);
		CItemInfoLevel itemInfo = m_GameData.GetItemInfo(curEquipStone, nItemLevel);
		if (itemInfo != null)
		{
			cEnterStageInfo.sEquipStoneID = curEquipStone + "_" + itemInfo.sName;
		}
		else
		{
			cEnterStageInfo.sEquipStoneID = "Empty";
		}
		cEnterStageInfo.sLevelID = gameLevelInfo.nID + "_" + gameLevelInfo.sLevelName;
		cEnterStageInfo.nLevelProccess = dataCenter.SceneProccess;
		CFlurryManager.GetInstance().LoseStage(cEnterStageInfo.sLevelID, cEnterStageInfo);
		CFlurryManager.GetInstance().LoseStage("ALL Stage", cEnterStageInfo);
	}

	public void Flurry_WinStage(int nLevelID)
	{
		iDataCenter dataCenter = m_GameData.GetDataCenter();
		if (dataCenter == null)
		{
			return;
		}
		GameLevelInfo gameLevelInfo = m_GameData.GetGameLevelInfo(nLevelID);
		if (gameLevelInfo == null)
		{
			return;
		}
		CCharacterInfo characterInfo = m_GameData.GetCharacterInfo(dataCenter.CurCharID);
		if (characterInfo == null)
		{
			return;
		}
		CCharSaveInfo character = dataCenter.GetCharacter(dataCenter.CurCharID);
		if (character == null || character.nLevel == -1)
		{
			return;
		}
		CCharacterInfoLevel cCharacterInfoLevel = characterInfo.Get(character.nLevel);
		if (cCharacterInfoLevel == null)
		{
			return;
		}
		CFlurryManager.CEnterStageInfo cEnterStageInfo = new CFlurryManager.CEnterStageInfo();
		if (cEnterStageInfo == null)
		{
			return;
		}
		cEnterStageInfo.sCharID = characterInfo.nID + "_" + cCharacterInfoLevel.sName;
		cEnterStageInfo.sCharLevel = character.nLevel.ToString();
		cEnterStageInfo.arrWeaponID = new string[3];
		for (int i = 0; i < 3; i++)
		{
			int selectWeapon = dataCenter.GetSelectWeapon(i);
			int weaponLevel = dataCenter.GetWeaponLevel(selectWeapon);
			CWeaponInfoLevel weaponInfo = m_GameData.GetWeaponInfo(selectWeapon, weaponLevel);
			if (weaponInfo != null)
			{
				cEnterStageInfo.arrWeaponID[i] = selectWeapon + "_" + weaponInfo.sName;
			}
			else
			{
				cEnterStageInfo.arrWeaponID[i] = "Empty";
			}
		}
		cEnterStageInfo.arrSkillID = new string[3];
		for (int j = 0; j < 3; j++)
		{
			int selectPassiveSkill = dataCenter.GetSelectPassiveSkill(characterInfo.nID, j);
			int nSkillLevel = 0;
			dataCenter.GetPassiveSkill(selectPassiveSkill, ref nSkillLevel);
			CSkillInfoLevel skillInfo = m_GameData.GetSkillInfo(selectPassiveSkill, nSkillLevel);
			if (skillInfo != null)
			{
				cEnterStageInfo.arrSkillID[j] = selectPassiveSkill + "_" + skillInfo.sName;
			}
			else
			{
				cEnterStageInfo.arrSkillID[j] = "Empty";
			}
		}
		int curEquipStone = dataCenter.CurEquipStone;
		int nItemLevel = 0;
		dataCenter.GetEquipStone(curEquipStone, ref nItemLevel);
		CItemInfoLevel itemInfo = m_GameData.GetItemInfo(curEquipStone, nItemLevel);
		if (itemInfo != null)
		{
			cEnterStageInfo.sEquipStoneID = curEquipStone + "_" + itemInfo.sName;
		}
		else
		{
			cEnterStageInfo.sEquipStoneID = "Empty";
		}
		cEnterStageInfo.sLevelID = gameLevelInfo.nID + "_" + gameLevelInfo.sLevelName;
		cEnterStageInfo.nLevelProccess = dataCenter.SceneProccess;
		CFlurryManager.GetInstance().WinStage(cEnterStageInfo.sLevelID, cEnterStageInfo);
		CFlurryManager.GetInstance().WinStage("ALL Stage", cEnterStageInfo);
	}

	public void Flurry_QuitStage(int nLevelID)
	{
		iDataCenter dataCenter = m_GameData.GetDataCenter();
		if (dataCenter == null)
		{
			return;
		}
		GameLevelInfo gameLevelInfo = m_GameData.GetGameLevelInfo(nLevelID);
		if (gameLevelInfo == null)
		{
			return;
		}
		CCharacterInfo characterInfo = m_GameData.GetCharacterInfo(dataCenter.CurCharID);
		if (characterInfo == null)
		{
			return;
		}
		CCharSaveInfo character = dataCenter.GetCharacter(dataCenter.CurCharID);
		if (character == null || character.nLevel == -1)
		{
			return;
		}
		CCharacterInfoLevel cCharacterInfoLevel = characterInfo.Get(character.nLevel);
		if (cCharacterInfoLevel == null)
		{
			return;
		}
		CFlurryManager.CEnterStageInfo cEnterStageInfo = new CFlurryManager.CEnterStageInfo();
		if (cEnterStageInfo == null)
		{
			return;
		}
		cEnterStageInfo.sCharID = characterInfo.nID + "_" + cCharacterInfoLevel.sName;
		cEnterStageInfo.sCharLevel = character.nLevel.ToString();
		cEnterStageInfo.arrWeaponID = new string[3];
		for (int i = 0; i < 3; i++)
		{
			int selectWeapon = dataCenter.GetSelectWeapon(i);
			int weaponLevel = dataCenter.GetWeaponLevel(selectWeapon);
			CWeaponInfoLevel weaponInfo = m_GameData.GetWeaponInfo(selectWeapon, weaponLevel);
			if (weaponInfo != null)
			{
				cEnterStageInfo.arrWeaponID[i] = selectWeapon + "_" + weaponInfo.sName;
			}
			else
			{
				cEnterStageInfo.arrWeaponID[i] = "Empty";
			}
		}
		cEnterStageInfo.arrSkillID = new string[3];
		for (int j = 0; j < 3; j++)
		{
			int selectPassiveSkill = dataCenter.GetSelectPassiveSkill(characterInfo.nID, j);
			int nSkillLevel = 0;
			dataCenter.GetPassiveSkill(selectPassiveSkill, ref nSkillLevel);
			CSkillInfoLevel skillInfo = m_GameData.GetSkillInfo(selectPassiveSkill, nSkillLevel);
			if (skillInfo != null)
			{
				cEnterStageInfo.arrSkillID[j] = selectPassiveSkill + "_" + skillInfo.sName;
			}
			else
			{
				cEnterStageInfo.arrSkillID[j] = "Empty";
			}
		}
		int curEquipStone = dataCenter.CurEquipStone;
		int nItemLevel = 0;
		dataCenter.GetEquipStone(curEquipStone, ref nItemLevel);
		CItemInfoLevel itemInfo = m_GameData.GetItemInfo(curEquipStone, nItemLevel);
		if (itemInfo != null)
		{
			cEnterStageInfo.sEquipStoneID = curEquipStone + "_" + itemInfo.sName;
		}
		else
		{
			cEnterStageInfo.sEquipStoneID = "Empty";
		}
		cEnterStageInfo.sLevelID = gameLevelInfo.nID + "_" + gameLevelInfo.sLevelName;
		cEnterStageInfo.nLevelProccess = dataCenter.SceneProccess;
		CFlurryManager.GetInstance().QuitStage(cEnterStageInfo.sLevelID, cEnterStageInfo);
		CFlurryManager.GetInstance().QuitStage("ALL Stage", cEnterStageInfo);
	}

	public void Flurry_PurchaseSkill(int nSkillID)
	{
		iDataCenter dataCenter = m_GameData.GetDataCenter();
		if (dataCenter == null)
		{
			return;
		}
		CCharacterInfo characterInfo = m_GameData.GetCharacterInfo(dataCenter.CurCharID);
		if (characterInfo == null)
		{
			return;
		}
		CCharSaveInfo character = dataCenter.GetCharacter(dataCenter.CurCharID);
		if (character == null || character.nLevel == -1)
		{
			return;
		}
		CCharacterInfoLevel cCharacterInfoLevel = characterInfo.Get(character.nLevel);
		if (cCharacterInfoLevel == null)
		{
			return;
		}
		int nSkillLevel = 0;
		dataCenter.GetPassiveSkill(nSkillID, ref nSkillLevel);
		CSkillInfoLevel skillInfo = m_GameData.GetSkillInfo(nSkillID, nSkillLevel);
		if (skillInfo != null)
		{
			CFlurryManager.CPurchaseSkillInfo cPurchaseSkillInfo = new CFlurryManager.CPurchaseSkillInfo();
			if (cPurchaseSkillInfo != null)
			{
				cPurchaseSkillInfo.sCharID = characterInfo.nID + "_" + cCharacterInfoLevel.sName;
				cPurchaseSkillInfo.sCharLevel = character.nLevel.ToString();
				cPurchaseSkillInfo.sSkillID = nSkillID + "_" + skillInfo.sName;
				CFlurryManager.GetInstance().PurchaseSkill(cPurchaseSkillInfo.sSkillID, cPurchaseSkillInfo);
				CFlurryManager.GetInstance().PurchaseSkill("ALL Skill", cPurchaseSkillInfo);
			}
		}
	}

	public void Flurry_UpgradeSkill(int nSkillID)
	{
		iDataCenter dataCenter = m_GameData.GetDataCenter();
		if (dataCenter == null)
		{
			return;
		}
		CCharacterInfo characterInfo = m_GameData.GetCharacterInfo(dataCenter.CurCharID);
		if (characterInfo == null)
		{
			return;
		}
		CCharSaveInfo character = dataCenter.GetCharacter(dataCenter.CurCharID);
		if (character == null || character.nLevel == -1)
		{
			return;
		}
		CCharacterInfoLevel cCharacterInfoLevel = characterInfo.Get(character.nLevel);
		if (cCharacterInfoLevel == null)
		{
			return;
		}
		int nSkillLevel = 0;
		dataCenter.GetPassiveSkill(nSkillID, ref nSkillLevel);
		CSkillInfoLevel skillInfo = m_GameData.GetSkillInfo(nSkillID, nSkillLevel);
		if (skillInfo != null)
		{
			CFlurryManager.CUpgradeSkillInfo cUpgradeSkillInfo = new CFlurryManager.CUpgradeSkillInfo();
			if (cUpgradeSkillInfo != null)
			{
				cUpgradeSkillInfo.sCharID = characterInfo.nID + "_" + cCharacterInfoLevel.sName;
				cUpgradeSkillInfo.sCharLevel = character.nLevel.ToString();
				cUpgradeSkillInfo.sSkillID = nSkillID + "_" + skillInfo.sName;
				cUpgradeSkillInfo.sSkillLevel = nSkillLevel.ToString();
				CFlurryManager.GetInstance().UpgradeSkill(cUpgradeSkillInfo.sSkillID, cUpgradeSkillInfo);
				CFlurryManager.GetInstance().UpgradeSkill("ALL Skill", cUpgradeSkillInfo);
			}
		}
	}

	public void Flurry_PurchaseWeapon(int nWeaponID)
	{
		iDataCenter dataCenter = m_GameData.GetDataCenter();
		if (dataCenter == null)
		{
			return;
		}
		CCharacterInfo characterInfo = m_GameData.GetCharacterInfo(dataCenter.CurCharID);
		if (characterInfo == null)
		{
			return;
		}
		CCharSaveInfo character = dataCenter.GetCharacter(dataCenter.CurCharID);
		if (character == null || character.nLevel == -1)
		{
			return;
		}
		CCharacterInfoLevel cCharacterInfoLevel = characterInfo.Get(character.nLevel);
		if (cCharacterInfoLevel == null)
		{
			return;
		}
		int weaponLevel = dataCenter.GetWeaponLevel(nWeaponID);
		CWeaponInfoLevel weaponInfo = m_GameData.GetWeaponInfo(nWeaponID, weaponLevel);
		if (weaponInfo != null)
		{
			CFlurryManager.CPurchaseWeaponInfo cPurchaseWeaponInfo = new CFlurryManager.CPurchaseWeaponInfo();
			if (cPurchaseWeaponInfo != null)
			{
				cPurchaseWeaponInfo.sCharID = characterInfo.nID + "_" + cCharacterInfoLevel.sName;
				cPurchaseWeaponInfo.sCharLevel = character.nLevel.ToString();
				cPurchaseWeaponInfo.sWeaponID = nWeaponID + "_" + weaponInfo.sName;
				cPurchaseWeaponInfo.nLevelProccess = dataCenter.SceneProccess;
				CFlurryManager.GetInstance().PurchaseWeapon(cPurchaseWeaponInfo.sWeaponID, cPurchaseWeaponInfo);
				CFlurryManager.GetInstance().PurchaseWeapon("ALL Weapon", cPurchaseWeaponInfo);
			}
		}
	}

	public void Flurry_UpgradeWeapon(int nWeaponID)
	{
		iDataCenter dataCenter = m_GameData.GetDataCenter();
		if (dataCenter == null)
		{
			return;
		}
		CCharacterInfo characterInfo = m_GameData.GetCharacterInfo(dataCenter.CurCharID);
		if (characterInfo == null)
		{
			return;
		}
		CCharSaveInfo character = dataCenter.GetCharacter(dataCenter.CurCharID);
		if (character == null || character.nLevel == -1)
		{
			return;
		}
		CCharacterInfoLevel cCharacterInfoLevel = characterInfo.Get(character.nLevel);
		if (cCharacterInfoLevel == null)
		{
			return;
		}
		int weaponLevel = dataCenter.GetWeaponLevel(nWeaponID);
		CWeaponInfoLevel weaponInfo = m_GameData.GetWeaponInfo(nWeaponID, weaponLevel);
		if (weaponInfo != null)
		{
			CFlurryManager.CUpgradeWeaponInfo cUpgradeWeaponInfo = new CFlurryManager.CUpgradeWeaponInfo();
			if (cUpgradeWeaponInfo != null)
			{
				cUpgradeWeaponInfo.sCharID = characterInfo.nID + "_" + cCharacterInfoLevel.sName;
				cUpgradeWeaponInfo.sCharLevel = character.nLevel.ToString();
				cUpgradeWeaponInfo.sWeaponID = nWeaponID + "_" + weaponInfo.sName;
				cUpgradeWeaponInfo.sWeaponLevel = weaponLevel.ToString();
				cUpgradeWeaponInfo.nLevelProccess = dataCenter.SceneProccess;
				CFlurryManager.GetInstance().UpgradeWeapon(cUpgradeWeaponInfo.sWeaponID, cUpgradeWeaponInfo);
				CFlurryManager.GetInstance().UpgradeWeapon("ALL Weapon", cUpgradeWeaponInfo);
			}
		}
	}

	public void Flurry_PurchaseStone(int nStoneID)
	{
		iDataCenter dataCenter = m_GameData.GetDataCenter();
		if (dataCenter == null)
		{
			return;
		}
		CCharacterInfo characterInfo = m_GameData.GetCharacterInfo(dataCenter.CurCharID);
		if (characterInfo == null)
		{
			return;
		}
		CCharSaveInfo character = dataCenter.GetCharacter(dataCenter.CurCharID);
		if (character == null || character.nLevel == -1)
		{
			return;
		}
		CCharacterInfoLevel cCharacterInfoLevel = characterInfo.Get(character.nLevel);
		if (cCharacterInfoLevel == null)
		{
			return;
		}
		int nItemLevel = 0;
		dataCenter.GetEquipStone(nStoneID, ref nItemLevel);
		CItemInfoLevel itemInfo = m_GameData.GetItemInfo(nStoneID, nItemLevel);
		if (itemInfo != null)
		{
			CFlurryManager.CPurchaseStoneInfo cPurchaseStoneInfo = new CFlurryManager.CPurchaseStoneInfo();
			if (cPurchaseStoneInfo != null)
			{
				cPurchaseStoneInfo.sCharID = characterInfo.nID + "_" + cCharacterInfoLevel.sName;
				cPurchaseStoneInfo.sCharLevel = character.nLevel.ToString();
				cPurchaseStoneInfo.sStoneID = nStoneID + "_" + itemInfo.sName;
				cPurchaseStoneInfo.nLevelProccess = dataCenter.SceneProccess;
				CFlurryManager.GetInstance().PurchaseStone(cPurchaseStoneInfo.sStoneID, cPurchaseStoneInfo);
				CFlurryManager.GetInstance().PurchaseStone("ALL Stone", cPurchaseStoneInfo);
			}
		}
	}

	public void Flurry_UpgradeStone(int nStoneID)
	{
		iDataCenter dataCenter = m_GameData.GetDataCenter();
		if (dataCenter == null)
		{
			return;
		}
		CCharacterInfo characterInfo = m_GameData.GetCharacterInfo(dataCenter.CurCharID);
		if (characterInfo == null)
		{
			return;
		}
		CCharSaveInfo character = dataCenter.GetCharacter(dataCenter.CurCharID);
		if (character == null || character.nLevel == -1)
		{
			return;
		}
		CCharacterInfoLevel cCharacterInfoLevel = characterInfo.Get(character.nLevel);
		if (cCharacterInfoLevel == null)
		{
			return;
		}
		int nItemLevel = 0;
		dataCenter.GetEquipStone(nStoneID, ref nItemLevel);
		CItemInfoLevel itemInfo = m_GameData.GetItemInfo(nStoneID, nItemLevel);
		if (itemInfo != null)
		{
			CFlurryManager.CUpgradeStoneInfo cUpgradeStoneInfo = new CFlurryManager.CUpgradeStoneInfo();
			if (cUpgradeStoneInfo != null)
			{
				cUpgradeStoneInfo.sCharID = characterInfo.nID + "_" + cCharacterInfoLevel.sName;
				cUpgradeStoneInfo.sCharLevel = character.nLevel.ToString();
				cUpgradeStoneInfo.sStoneID = nStoneID + "_" + itemInfo.sName;
				cUpgradeStoneInfo.sStoneLevel = nItemLevel.ToString();
				cUpgradeStoneInfo.nLevelProccess = dataCenter.SceneProccess;
				CFlurryManager.GetInstance().UpgradeStone(cUpgradeStoneInfo.sStoneID, cUpgradeStoneInfo);
				CFlurryManager.GetInstance().UpgradeStone("ALL Stone", cUpgradeStoneInfo);
			}
		}
	}

	public void Flurry_PurchaseChar(int nCharID)
	{
		iDataCenter dataCenter = m_GameData.GetDataCenter();
		if (dataCenter != null)
		{
			CCharacterInfoLevel characterInfo = m_GameData.GetCharacterInfo(nCharID, 1);
			if (characterInfo != null)
			{
				CFlurryManager.CPurchaseCharInfo cPurchaseCharInfo = new CFlurryManager.CPurchaseCharInfo();
				cPurchaseCharInfo.sCharID = nCharID + "_" + characterInfo.sName;
				cPurchaseCharInfo.nLevelProccess = dataCenter.SceneProccess;
				CFlurryManager.GetInstance().PurchaseChar(cPurchaseCharInfo.sCharID, cPurchaseCharInfo);
				CFlurryManager.GetInstance().PurchaseChar("ALL Char", cPurchaseCharInfo);
			}
		}
	}

	public void Flurry_PurchaseBullet(int nLevelID)
	{
		GameLevelInfo gameLevelInfo = m_GameData.GetGameLevelInfo(nLevelID);
		if (gameLevelInfo == null)
		{
			return;
		}
		iDataCenter dataCenter = m_GameData.GetDataCenter();
		if (dataCenter == null)
		{
			return;
		}
		CCharacterInfo characterInfo = m_GameData.GetCharacterInfo(dataCenter.CurCharID);
		if (characterInfo == null)
		{
			return;
		}
		CCharSaveInfo character = dataCenter.GetCharacter(dataCenter.CurCharID);
		if (character != null && character.nLevel != -1)
		{
			CCharacterInfoLevel cCharacterInfoLevel = characterInfo.Get(character.nLevel);
			if (cCharacterInfoLevel != null)
			{
				CFlurryManager.CPurchaseBulletInfo cPurchaseBulletInfo = new CFlurryManager.CPurchaseBulletInfo();
				cPurchaseBulletInfo.sCharID = characterInfo.nID + "_" + cCharacterInfoLevel.sName;
				cPurchaseBulletInfo.sCharLevel = character.nLevel.ToString();
				cPurchaseBulletInfo.sLevelID = nLevelID + "_" + gameLevelInfo.sLevelName;
				CFlurryManager.GetInstance().PurchaseBullet(cPurchaseBulletInfo.sLevelID, cPurchaseBulletInfo);
				CFlurryManager.GetInstance().PurchaseBullet("ALL Level", cPurchaseBulletInfo);
			}
		}
	}

	public void Flurry_PurchaseIAP(int nIAPID)
	{
		CIAPInfo iAPInfo = iIAPManager.GetInstance().GetIAPInfo(nIAPID);
		if (iAPInfo == null)
		{
			return;
		}
		iDataCenter dataCenter = m_GameData.GetDataCenter();
		if (dataCenter == null)
		{
			return;
		}
		CCharacterInfo characterInfo = m_GameData.GetCharacterInfo(dataCenter.CurCharID);
		if (characterInfo == null)
		{
			return;
		}
		CCharSaveInfo character = dataCenter.GetCharacter(dataCenter.CurCharID);
		if (character == null || character.nLevel == -1)
		{
			return;
		}
		CCharacterInfoLevel cCharacterInfoLevel = characterInfo.Get(character.nLevel);
		if (cCharacterInfoLevel != null)
		{
			GameLevelInfo gameLevelInfo = m_GameData.GetGameLevelInfo(dataCenter.LatestLevel);
			if (gameLevelInfo != null)
			{
				CFlurryManager.CPurchaseIAPInfo cPurchaseIAPInfo = new CFlurryManager.CPurchaseIAPInfo();
				cPurchaseIAPInfo.sCharID = characterInfo.nID + "_" + cCharacterInfoLevel.sName;
				cPurchaseIAPInfo.sCharLevel = character.nLevel.ToString();
				cPurchaseIAPInfo.sIAP = nIAPID + "_" + iAPInfo.sKey;
				cPurchaseIAPInfo.sLevelID = gameLevelInfo.nID + "_" + gameLevelInfo.sLevelName;
				CFlurryManager.GetInstance().PurchaseIAP(cPurchaseIAPInfo.sIAP, cPurchaseIAPInfo);
				CFlurryManager.GetInstance().PurchaseIAP("ALL IAP", cPurchaseIAPInfo);
			}
		}
	}

	public void Flurry_CharRevive(int nLevelID)
	{
		GameLevelInfo gameLevelInfo = m_GameData.GetGameLevelInfo(nLevelID);
		if (gameLevelInfo == null)
		{
			return;
		}
		iDataCenter dataCenter = m_GameData.GetDataCenter();
		if (dataCenter == null)
		{
			return;
		}
		CCharacterInfo characterInfo = m_GameData.GetCharacterInfo(dataCenter.CurCharID);
		if (characterInfo == null)
		{
			return;
		}
		CCharSaveInfo character = dataCenter.GetCharacter(dataCenter.CurCharID);
		if (character == null || character.nLevel == -1)
		{
			return;
		}
		CCharacterInfoLevel cCharacterInfoLevel = characterInfo.Get(character.nLevel);
		if (cCharacterInfoLevel == null)
		{
			return;
		}
		CFlurryManager.CReviveInfo cReviveInfo = new CFlurryManager.CReviveInfo();
		cReviveInfo.sCharID = characterInfo.nID + "_" + cCharacterInfoLevel.sName;
		cReviveInfo.sCharLevel = character.nLevel.ToString();
		cReviveInfo.arrWeaponID = new string[3];
		for (int i = 0; i < 3; i++)
		{
			int selectWeapon = dataCenter.GetSelectWeapon(i);
			int weaponLevel = dataCenter.GetWeaponLevel(selectWeapon);
			CWeaponInfoLevel weaponInfo = m_GameData.GetWeaponInfo(selectWeapon, weaponLevel);
			if (weaponInfo != null)
			{
				cReviveInfo.arrWeaponID[i] = selectWeapon + "_" + weaponInfo.sName;
			}
			else
			{
				cReviveInfo.arrWeaponID[i] = "Empty";
			}
		}
		cReviveInfo.arrSkillID = new string[3];
		for (int j = 0; j < 3; j++)
		{
			int selectPassiveSkill = dataCenter.GetSelectPassiveSkill(characterInfo.nID, j);
			int nSkillLevel = 0;
			dataCenter.GetPassiveSkill(selectPassiveSkill, ref nSkillLevel);
			CSkillInfoLevel skillInfo = m_GameData.GetSkillInfo(selectPassiveSkill, nSkillLevel);
			if (skillInfo != null)
			{
				cReviveInfo.arrSkillID[j] = selectPassiveSkill + "_" + skillInfo.sName;
			}
			else
			{
				cReviveInfo.arrSkillID[j] = "Empty";
			}
		}
		int curEquipStone = dataCenter.CurEquipStone;
		int nItemLevel = 0;
		dataCenter.GetEquipStone(curEquipStone, ref nItemLevel);
		CItemInfoLevel itemInfo = m_GameData.GetItemInfo(curEquipStone, nItemLevel);
		if (itemInfo != null)
		{
			cReviveInfo.sEquipStoneID = curEquipStone + "_" + itemInfo.sName;
		}
		else
		{
			cReviveInfo.sEquipStoneID = "Empty";
		}
		cReviveInfo.sLevelID = gameLevelInfo.nID + "_" + gameLevelInfo.sLevelName;
		cReviveInfo.nLevelProccess = dataCenter.SceneProccess;
		CFlurryManager.GetInstance().CharRevive(cReviveInfo.sLevelID, cReviveInfo);
		CFlurryManager.GetInstance().CharRevive("ALL Level", cReviveInfo);
	}

	public void Flurry_GainAchi(int nAchiID, int nStep)
	{
		CAchievementCenter achievementCenter = CAchievementManager.GetInstance().GetAchievementCenter();
		if (achievementCenter == null)
		{
			return;
		}
		CAchievementInfo info = achievementCenter.GetInfo(nAchiID);
		if (info == null)
		{
			return;
		}
		CAchievementData data = achievementCenter.GetData(nAchiID);
		if (data == null)
		{
			return;
		}
		iDataCenter dataCenter = m_GameData.GetDataCenter();
		if (dataCenter == null)
		{
			return;
		}
		CCharacterInfo characterInfo = m_GameData.GetCharacterInfo(dataCenter.CurCharID);
		if (characterInfo == null)
		{
			return;
		}
		CCharSaveInfo character = dataCenter.GetCharacter(dataCenter.CurCharID);
		if (character != null && character.nLevel != -1)
		{
			CCharacterInfoLevel cCharacterInfoLevel = characterInfo.Get(character.nLevel);
			if (cCharacterInfoLevel != null)
			{
				CFlurryManager.CAchiInfo cAchiInfo = new CFlurryManager.CAchiInfo();
				cAchiInfo.sCharID = characterInfo.nID + "_" + cCharacterInfoLevel.sName;
				cAchiInfo.sCharLevel = character.nLevel.ToString();
				cAchiInfo.sAchiID = nAchiID + "_" + info.sName;
				cAchiInfo.sAchiLevel = nStep.ToString();
				CFlurryManager.GetInstance().GainAchi(cAchiInfo.sAchiID, cAchiInfo);
				CFlurryManager.GetInstance().GainAchi("ALL Achi", cAchiInfo);
			}
		}*/
	}
}
