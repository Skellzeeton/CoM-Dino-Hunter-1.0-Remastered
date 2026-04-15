using System.Collections.Generic;
using EventCenter;
using UnityEngine;
using gyAchievementSystem;
using gyIAPSystem;
using GUPS.AntiCheat.Protected;

public class TUIDataServer
{
	private static TUIDataServer instance;

	public static TUIDataServer Instance()
	{
		if (instance == null)
		{
			instance = new TUIDataServer();
		}
		return instance;
	}

	public void Initialize()
	{
		global::EventCenter.EventCenter.Instance.Register<TUIEvent.SendEvent_SceneMain>(TUIEvent_BackInfo_SceneMain);
		global::EventCenter.EventCenter.Instance.Register<TUIEvent.SendEvent_SceneMainMenu>(TUIEvent_BackInfo_SceneMainMenu);
		global::EventCenter.EventCenter.Instance.Register<TUIEvent.SendEvent_SceneEquip>(TUIEvent_BackInfo_SceneEquip);
		global::EventCenter.EventCenter.Instance.Register<TUIEvent.SendEvent_SceneStash>(TUIEvent_BackInfo_SceneStash);
		global::EventCenter.EventCenter.Instance.Register<TUIEvent.SendEvent_SceneSkill>(TUIEvent_BackInfo_SceneSkill);
		global::EventCenter.EventCenter.Instance.Register<TUIEvent.SendEvent_SceneForge>(TUIEvent_BackInfo_SceneForge);
		global::EventCenter.EventCenter.Instance.Register<TUIEvent.SendEvent_SceneTavern>(TUIEvent_BackInfo_SceneTavern);
		global::EventCenter.EventCenter.Instance.Register<TUIEvent.SendEvent_SceneMap>(TUIEvent_BackInfo_SceneMap);
		global::EventCenter.EventCenter.Instance.Register<TUIEvent.SendEvent_SceneIAP>(TUIEvent_BackInfo_SceneIAP);
		global::EventCenter.EventCenter.Instance.Register<TUIEvent.SendEvent_SceneGold>(TUIEvent_BackInfo_SceneGold);
	}

	private void TUIEvent_BackInfo_SceneMain(object sender, TUIEvent.SendEvent_SceneMain m_event)
	{
		if (m_event.GetEventName() == "TUIEvent_EnterInfo")
		{
			/*iServerVerify.GetInstance().SetSuccessFunc(OnServerVerifySuccess);
			iServerVerify.GetInstance().SetFailedFunc(OnServerVerifyFailed);
			iServerVerify.GetInstance().SetNetErrorFunc(OnServerVerifyNetError);
			iServerVerify.GetInstance().ConnectServer("1.0.2", 10f, 2f);*/
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneMain(m_event.GetEventName()));
		}
		else if (m_event.GetEventName() == "TUIEvent_ConnectAgain")
		{
			/*iServerVerify.GetInstance().SetSuccessFunc(OnServerVerifySuccess);
			iServerVerify.GetInstance().SetFailedFunc(OnServerVerifyFailed);
			iServerVerify.GetInstance().SetNetErrorFunc(OnServerVerifyNetError);
			iServerVerify.GetInstance().ConnectServer("1.0.2", 10f, 0f);*/
		}
		else if (m_event.GetEventName() == "TUIEvent_GotoUpdate")
		{
			Application.OpenURL(iMacroDefine.AddressForItunes);
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneMain(m_event.GetEventName()));
		}
		else
		{
			if (!(m_event.GetEventName() == "TUIEvent_EnterLevel"))
			{
				return;
			}
			iGameData gameData = iGameApp.GetInstance().m_GameData;
			if (gameData == null)
			{
				return;
			}
			iGameState gameState = iGameApp.GetInstance().m_GameState;
			if (gameState == null)
			{
				return;
			}
			iDataCenter dataCenter = gameData.GetDataCenter();
			if (dataCenter == null)
			{
				return;
			}
            Debug.Log("FIRST TIME? => " + dataCenter.isFirstTimePlay);
            if (dataCenter.isFirstTimePlay)
			{
				gameState.GameLevel = 1001;
				int wparam = 2;
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneMain(m_event.GetEventName(), true, wparam));
				return;
			}
			/*if (OpenClikPlugin.IsAdReady())
			{
				OpenClikPlugin.Show(true);
			}*/
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneMain(m_event.GetEventName(), true));
		}
	}

	private void TUIEvent_BackInfo_SceneMainMenu(object sender, TUIEvent.SendEvent_SceneMainMenu m_event)
	{
		if (m_event.GetEventName() == "TUIEvent_TopBar")
		{
			iGameData gameData = iGameApp.GetInstance().m_GameData;
			if (gameData == null)
			{
				return;
			}
			iDataCenter dataCenter = gameData.GetDataCenter();
			if (dataCenter == null)
			{
				return;
			}
			CCharSaveInfo character = dataCenter.GetCharacter(dataCenter.CurCharID);
			if (character == null)
			{
				return;
			}
			CCharacterInfoLevel characterInfo = gameData.GetCharacterInfo(character.nID, character.nLevel);
			if (characterInfo != null)
			{
				TUIGameInfo tUIGameInfo = new TUIGameInfo();
				tUIGameInfo.player_info = new TUIPlayerInfo();
				tUIGameInfo.player_info.avatar_id = character.nID;
				tUIGameInfo.player_info.level = character.nLevel;
				tUIGameInfo.player_info.level_exp = characterInfo.nExp;
				tUIGameInfo.player_info.exp = character.nExp;
				tUIGameInfo.player_info.gold = dataCenter.Gold;
				tUIGameInfo.player_info.crystal = dataCenter.Crystal;
				if (!dataCenter.isTutorialVillage)
				{
					dataCenter.isTutorialVillage = true;
					dataCenter.Save();
					global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneMainMenu("TUIEvent_ShowHelp"));
				}
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneMainMenu(m_event.GetEventName(), tUIGameInfo));
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_OptionInfo")
		{
			iGameData gameData2 = iGameApp.GetInstance().m_GameData;
			if (gameData2 != null)
			{
				iDataCenter dataCenter2 = gameData2.GetDataCenter();
				if (dataCenter2 != null)
				{
					TUIGameInfo tUIGameInfo2 = new TUIGameInfo();
					tUIGameInfo2.option_info = new TUIOptionInfo();
					tUIGameInfo2.option_info.music_open = dataCenter2.MusicSwitch;
					tUIGameInfo2.option_info.sfx_open = dataCenter2.SoundSwitch;
					tUIGameInfo2.option_info.ambience_open = dataCenter2.AmbienceSwitch;
					tUIGameInfo2.option_info.start_cutscene_replay = dataCenter2.StartCutsceneReplay;
					global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneMainMenu(m_event.GetEventName(), tUIGameInfo2));
				}
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_AcheviementInfo")
		{
			CAchievementCenter achievementCenter = CAchievementManager.GetInstance().GetAchievementCenter();
			if (achievementCenter == null)
			{
				return;
			}
			Dictionary<int, CAchievementInfo> dataInfo = achievementCenter.GetDataInfo();
			if (dataInfo == null)
			{
				return;
			}
			TUIGameInfo tUIGameInfo3 = new TUIGameInfo();
			tUIGameInfo3.achievement_info = new TUIAchievementInfo();
			foreach (CAchievementInfo value in dataInfo.Values)
			{
				CAchievementData data = achievementCenter.GetData(value.nID);
				Dictionary<int, string> dictionary = new Dictionary<int, string>();
				Dictionary<int, string> dictionary2 = new Dictionary<int, string>();
				Dictionary<int, TUIAchievementRewardInfo> dictionary3 = new Dictionary<int, TUIAchievementRewardInfo>();
				Dictionary<int, int> dictionary4 = new Dictionary<int, int>();
				Dictionary<int, bool> dictionary5 = new Dictionary<int, bool>();
				for (int i = 0; i < 3; i++)
				{
					CAchievementStep step = value.GetStep(i);
					if (step != null)
					{
						int key = i + 1;
						dictionary.Add(key, value.sName);
						dictionary2.Add(key, string.Format(value.sDesc, step.nStepPurpose));
						TUIAchievementRewardInfo tUIAchievementRewardInfo = new TUIAchievementRewardInfo();
						tUIAchievementRewardInfo.SetRewardInfo01(step.nRewardNumber, (step.nRewardType == 2) ? UnitType.Crystal : UnitType.Gold);
						dictionary3.Add(key, tUIAchievementRewardInfo);
						if (data != null)
						{
							dictionary4.Add(key, (int)(Mathf.Clamp01((float)data.nCurValue / (float)step.nStepPurpose) * 100f));
							dictionary5.Add(key, data.IsGotReward(i));
						}
						else
						{
							dictionary4.Add(key, 0);
							dictionary5.Add(key, false);
						}
					}
				}
				tUIGameInfo3.achievement_info.AddAchievementInfo(new TUIOneAchievementInfo(value.nID, dictionary, dictionary2, dictionary3, dictionary4, dictionary5));
			}
			if (iGameApp.GetInstance().CheckAchieveReward())
			{
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneMainMenu("TUIEvent_HadAchievementReward", true));
			}
			else
			{
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneMainMenu("TUIEvent_HadAchievementReward"));
			}
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneMainMenu(m_event.GetEventName(), tUIGameInfo3));
		}
		else if (m_event.GetEventName() == "TUIEvent_ChangeMusic")
		{
			iGameData gameData3 = iGameApp.GetInstance().m_GameData;
			if (gameData3 != null)
			{
				iDataCenter dataCenter3 = gameData3.GetDataCenter();
				if (dataCenter3 != null)
				{
					dataCenter3.MusicSwitch = !dataCenter3.MusicSwitch;
					dataCenter3.Save();
					TAudioManager.instance.isMusicOn = dataCenter3.MusicSwitch;
					global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneMainMenu(m_event.GetEventName(), true));
				}
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_ChangeSFX")
		{
			iGameData gameData4 = iGameApp.GetInstance().m_GameData;
			if (gameData4 != null)
			{
				iDataCenter dataCenter4 = gameData4.GetDataCenter();
				if (dataCenter4 != null)
				{
					dataCenter4.SoundSwitch = !dataCenter4.SoundSwitch;
					dataCenter4.Save();
					TAudioManager.instance.isSoundOn = dataCenter4.SoundSwitch;
					global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneMainMenu(m_event.GetEventName(), true));
				}
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_ChangeAmb")
		{
			iGameData gameData4 = iGameApp.GetInstance().m_GameData;
			if (gameData4 != null)
			{
				iDataCenter dataCenter4 = gameData4.GetDataCenter();
				if (dataCenter4 != null)
				{
					dataCenter4.AmbienceSwitch = !dataCenter4.AmbienceSwitch;
					dataCenter4.Save();
					TAudioManager.instance.isAmbienceOn = dataCenter4.AmbienceSwitch;
					global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneMainMenu(m_event.GetEventName(), true));
				}
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_ChangeStartCutsceneReplay")
		{
			iGameData gameData8 = iGameApp.GetInstance().m_GameData;
			if (gameData8 != null)
			{
				iDataCenter dataCenter8 = gameData8.GetDataCenter();
				if (dataCenter8 != null)
				{
					dataCenter8.StartCutsceneReplay = !dataCenter8.StartCutsceneReplay;
					dataCenter8.Save();
					global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneMainMenu(m_event.GetEventName(), true));
				}
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_TakeAchievement")
		{
			bool flag = false;
			iGameData gameData5 = iGameApp.GetInstance().m_GameData;
			if (gameData5 != null)
			{
				iDataCenter dataCenter5 = gameData5.GetDataCenter();
				CAchievementCenter achievementCenter2 = CAchievementManager.GetInstance().GetAchievementCenter();
				if (dataCenter5 != null && achievementCenter2 != null)
				{
					int wParam = m_event.GetWParam();
					int lparam = m_event.GetLparam();
					CAchievementInfo info = achievementCenter2.GetInfo(wParam);
					CAchievementData data2 = achievementCenter2.GetData(wParam);
					if (info != null && data2 != null && !data2.IsGotReward(lparam - 1))
					{
						CAchievementStep step2 = info.GetStep(lparam - 1);
						if (step2 != null)
						{
							if (step2.nRewardType == 2)
							{
								dataCenter5.AddCrystal(step2.nRewardNumber);
								dataCenter5.Save();
							}
							else if (step2.nRewardType == 1)
							{
								dataCenter5.AddGold(step2.nRewardNumber);
								dataCenter5.Save();
							}
							data2.SetGotReward(lparam - 1, true);
							dataCenter5.Save();
							flag = true;
						}
					}
				}
			}
			if (flag)
			{
				if (iGameApp.GetInstance().CheckAchieveReward())
				{
					global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneMainMenu("TUIEvent_HadAchievementReward", true));
				}
				else
				{
					global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneMainMenu("TUIEvent_HadAchievementReward"));
				}
			}
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneMainMenu(m_event.GetEventName(), flag));
		}
		else if (m_event.GetEventName() == "TUIEvent_EnterInfo")
		{
			iGameData gameData6 = iGameApp.GetInstance().m_GameData;
			if (gameData6 == null)
			{
				return;
			}
			iDataCenter dataCenter6 = gameData6.GetDataCenter();
			if (dataCenter6 == null)
			{
				return;
			}
			if (!dataCenter6.isEvaluate)
			{
				dataCenter6.EnterAppCount++;
				if (dataCenter6.EnterAppCount >= 3)
				{
					dataCenter6.isEvaluate = true;
					dataCenter6.Save();
					global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneMainMenu("TUIEvent_ShowReview"));
				}
			}
			iGameApp.GetInstance().CheckUnLock();
			TUIGameInfo tUIGameInfo4 = new TUIGameInfo();
			tUIGameInfo4.villiage_enter_info = new TUIVilliageEnterInfo();
			tUIGameInfo4.villiage_enter_info.finished_text = dataCenter6.SceneProccess + "%";
			switch (dataCenter6.UnLockSignType)
			{
			case 2:
				tUIGameInfo4.villiage_enter_info.unlock_type = UnlockType.Skill;
				tUIGameInfo4.villiage_enter_info.unlock_skill_id = dataCenter6.UnLockSignID;
				break;
			case 1:
				tUIGameInfo4.villiage_enter_info.unlock_type = UnlockType.Role;
				tUIGameInfo4.villiage_enter_info.unlock_role_id = dataCenter6.UnLockSignID;
				break;
			default:
				tUIGameInfo4.villiage_enter_info.unlock_type = UnlockType.None;
				break;
			}
			dataCenter6.UnLockSignType = 0;
			dataCenter6.UnLockSignID = -1;
			dataCenter6.Save();
			if (iGameApp.GetInstance().CheckWeaponSignState(1) || iGameApp.GetInstance().CheckEquipStoneSignState(1))
			{
				tUIGameInfo4.villiage_enter_info.forge_sign = NewMarkType.New;
			}
			else if (iGameApp.GetInstance().CheckWeaponMaterialEnough() || iGameApp.GetInstance().CheckEquipStoneMaterialEnough())
			{
				tUIGameInfo4.villiage_enter_info.forge_sign = NewMarkType.Mark;
			}
			else
			{
				tUIGameInfo4.villiage_enter_info.forge_sign = NewMarkType.None;
			}
			int curCharID = dataCenter6.CurCharID;
			CCharacterInfo characterInfo2 = gameData6.GetCharacterInfo(curCharID);
			if (characterInfo2 != null)
			{
				NewMarkType skill_sign = NewMarkType.None;
				if (characterInfo2.ltCharacterPassiveSkill != null)
				{
					foreach (int item in characterInfo2.ltCharacterPassiveSkill)
					{
						if (iGameApp.GetInstance().CheckSkillSignState(1, item))
						{
							skill_sign = NewMarkType.New;
							break;
						}
						if (iGameApp.GetInstance().CheckSkillMaterialEnough(item))
						{
							skill_sign = NewMarkType.Mark;
						}
					}
				}
				tUIGameInfo4.villiage_enter_info.skill_sign = skill_sign;
			}
			if (iGameApp.GetInstance().CheckCharacterSignState(1))
			{
				tUIGameInfo4.villiage_enter_info.tavern_sign = NewMarkType.New;
			}
			else if (iGameApp.GetInstance().CheckCharacterMaterialEnough())
			{
				tUIGameInfo4.villiage_enter_info.tavern_sign = NewMarkType.Mark;
			}
			else
			{
				tUIGameInfo4.villiage_enter_info.tavern_sign = NewMarkType.None;
			}
			if (iGameApp.GetInstance().CheckWeaponSignState(3) || iGameApp.GetInstance().CheckEquipStoneSignState(3) || iGameApp.GetInstance().CheckCharacterSignState(3))
			{
				tUIGameInfo4.villiage_enter_info.equip_sign = NewMarkType.New;
			}
			else
			{
				NewMarkType equip_sign = NewMarkType.None;
				if (characterInfo2 != null && characterInfo2.ltCharacterPassiveSkill != null)
				{
					foreach (int item2 in characterInfo2.ltCharacterPassiveSkill)
					{
						if (iGameApp.GetInstance().CheckSkillSignState(3, item2))
						{
							equip_sign = NewMarkType.New;
							break;
						}
					}
				}
				tUIGameInfo4.villiage_enter_info.equip_sign = equip_sign;
			}
			tUIGameInfo4.villiage_enter_info.stash_sign = NewMarkType.None;
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneMainMenu(m_event.GetEventName(), tUIGameInfo4));
		}
		else if (m_event.GetEventName() == "TUIEvent_EnterIAP")
		{
			iGameState gameState = iGameApp.GetInstance().m_GameState;
			if (gameState != null)
			{
				gameState.m_lstScene4IAP = TUISceneType.Scene_MainMenu;
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneMainMenu(m_event.GetEventName(), true));
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_EnterGold")
		{
			iGameState gameState2 = iGameApp.GetInstance().m_GameState;
			if (gameState2 != null)
			{
				gameState2.m_lstScene4IAP = TUISceneType.Scene_MainMenu;
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneMainMenu(m_event.GetEventName(), true));
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_EnterEquip")
		{
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneMainMenu(m_event.GetEventName(), true));
		}
		else if (m_event.GetEventName() == "TUIEvent_EnterForge")
		{
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneMainMenu(m_event.GetEventName(), true));
		}
		else if (m_event.GetEventName() == "TUIEvent_EnterTavern")
		{
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneMainMenu(m_event.GetEventName(), true));
		}
		else if (m_event.GetEventName() == "TUIEvent_EnterSkill")
		{
			iGameData gameData7 = iGameApp.GetInstance().m_GameData;
			if (gameData7 == null)
			{
				return;
			}
			iDataCenter dataCenter7 = gameData7.GetDataCenter();
			if (dataCenter7 == null)
			{
				return;
			}
			iGameState gameState3 = iGameApp.GetInstance().m_GameState;
			if (gameState3 != null)
			{
				gameState3.m_nLinkSkillRole = dataCenter7.CurCharID;
				CCharacterInfo characterInfo3 = gameData7.GetCharacterInfo(dataCenter7.CurCharID);
				if (characterInfo3 != null && characterInfo3.ltCharacterPassiveSkill != null && characterInfo3.ltCharacterPassiveSkill.Count > 0)
				{
					gameState3.m_nLinkSkill = characterInfo3.ltCharacterPassiveSkill[0];
				}
			}
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneMainMenu(m_event.GetEventName(), true));
		}
		else if (m_event.GetEventName() == "TUIEvent_EnterStash")
		{
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneMainMenu(m_event.GetEventName(), true));
		}
		else if (m_event.GetEventName() == "TUIEvent_EnterMap")
		{
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneMainMenu(m_event.GetEventName(), true));
		}
		else if (m_event.GetEventName() == "TUIEvent_OpenSupportURL")
		{
			Application.OpenURL("https://discord.gg/gv7Ebnmg7u");
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneMainMenu(m_event.GetEventName()));
		}
		else if (m_event.GetEventName() == "TUIEvent_OpenReviewURL")
		{
			Application.OpenURL(iMacroDefine.AddressForItunes);
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneMainMenu(m_event.GetEventName()));
		}
	}

	private void TUIEvent_BackInfo_SceneEquip(object sender, TUIEvent.SendEvent_SceneEquip m_event)
	{
		if (m_event.GetEventName() == "TUIEvent_TopBar")
		{
			iGameData gameData = iGameApp.GetInstance().m_GameData;
			if (gameData == null)
			{
				return;
			}
			iDataCenter dataCenter = gameData.GetDataCenter();
			if (dataCenter == null)
			{
				return;
			}
			CCharSaveInfo character = dataCenter.GetCharacter(dataCenter.CurCharID);
			if (character == null)
			{
				return;
			}
			CCharacterInfoLevel characterInfo = gameData.GetCharacterInfo(character.nID, character.nLevel);
			if (characterInfo != null)
			{
				TUIGameInfo tUIGameInfo = new TUIGameInfo();
				tUIGameInfo.player_info = new TUIPlayerInfo();
				tUIGameInfo.player_info.avatar_id = character.nID;
				tUIGameInfo.player_info.level = character.nLevel;
				tUIGameInfo.player_info.level_exp = characterInfo.nExp;
				tUIGameInfo.player_info.exp = character.nExp;
				tUIGameInfo.player_info.gold = dataCenter.Gold;
				tUIGameInfo.player_info.crystal = dataCenter.Crystal;
				iGameState gameState = iGameApp.GetInstance().m_GameState;
				if (gameState != null)
				{
					gameState.m_curScene4Recommand = gameState.m_lstScene4Recommand;
					gameState.m_lstScene4Recommand = TUISceneType.None;
				}
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneEquip(m_event.GetEventName(), tUIGameInfo));
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_RoleSign")
		{
			iGameData gameData2 = iGameApp.GetInstance().m_GameData;
			if (gameData2 == null)
			{
				return;
			}
			iDataCenter dataCenter2 = gameData2.GetDataCenter();
			if (dataCenter2 == null)
			{
				return;
			}
			CCharSaveInfo character2 = dataCenter2.GetCharacter(dataCenter2.CurCharID);
			if (character2 == null)
			{
				return;
			}
			TUIGameInfo tUIGameInfo2 = new TUIGameInfo();
			tUIGameInfo2.equip_info = new TUIEquipInfo();
			string name = "No Name";
			string introduce = "No Desc";
			CCharacterInfoLevel cCharacterInfoLevel = null;
			cCharacterInfoLevel = gameData2.GetCharacterInfo(dataCenter2.CurCharID, 1);
			if (cCharacterInfoLevel != null)
			{
				name = cCharacterInfoLevel.sName;
				introduce = cCharacterInfoLevel.sDesc;
			}
			tUIGameInfo2.equip_info.role = new TUIPopupInfo(dataCenter2.CurCharID, name, introduce);
			int[] array = new int[6] { 1, 6, 2, 3, 4, 5 };
			tUIGameInfo2.equip_info.roles_list = new List<TUIPopupInfo>();
			for (int i = 0; i < array.Length; i++)
			{
				CCharSaveInfo character3 = dataCenter2.GetCharacter(array[i]);
				if (character3 != null && character3.nLevel != -1)
				{
					cCharacterInfoLevel = gameData2.GetCharacterInfo(array[i], 1);
					if (cCharacterInfoLevel != null)
					{
						name = cCharacterInfoLevel.sName;
						introduce = cCharacterInfoLevel.sDesc;
					}
					tUIGameInfo2.equip_info.roles_list.Add(new TUIPopupInfo(array[i], name, introduce));
					if (iGameApp.GetInstance().CheckCharacterSignState(3, array[i]))
					{
						tUIGameInfo2.equip_info.AddRolesNewMark(array[i], NewMarkType.New);
					}
					else
					{
						tUIGameInfo2.equip_info.AddRolesNewMark(array[i], NewMarkType.None);
					}
				}
			}
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneEquip(m_event.GetEventName(), tUIGameInfo2));
		}
		else if (m_event.GetEventName() == "TUIEvent_SkillSign")
		{
			iGameData gameData3 = iGameApp.GetInstance().m_GameData;
			if (gameData3 == null)
			{
				return;
			}
			iDataCenter dataCenter3 = gameData3.GetDataCenter();
			if (dataCenter3 == null)
			{
				return;
			}
			iSkillCenter skillCenter = gameData3.GetSkillCenter();
			if (skillCenter == null)
			{
				return;
			}
			CCharSaveInfo character4 = dataCenter3.GetCharacter(dataCenter3.CurCharID);
			if (character4 == null)
			{
				return;
			}
			CCharacterInfoLevel characterInfo2 = gameData3.GetCharacterInfo(character4.nID, character4.nLevel);
			if (characterInfo2 == null)
			{
				return;
			}
			TUIGameInfo tUIGameInfo3 = new TUIGameInfo();
			tUIGameInfo3.equip_info = new TUIEquipInfo();
			int num = -1;
			int nSkillLevel = 0;
			CSkillInfoLevel cSkillInfoLevel = null;
			num = characterInfo2.nSkill;
			cSkillInfoLevel = gameData3.GetSkillInfo(num, 1);
			if (cSkillInfoLevel != null)
			{
				tUIGameInfo3.equip_info.skill01 = new TUIPopupInfo(num, cSkillInfoLevel.sName, cSkillInfoLevel.sDesc);
				tUIGameInfo3.equip_info.AddSkillNewMark(num, NewMarkType.None);
			}
			num = dataCenter3.GetSelectPassiveSkill(dataCenter3.CurCharID, 0);
			if (dataCenter3.GetPassiveSkill(num, ref nSkillLevel))
			{
				cSkillInfoLevel = gameData3.GetSkillInfo(num, nSkillLevel);
				if (cSkillInfoLevel != null)
				{
					tUIGameInfo3.equip_info.skill02 = new TUIPopupInfo(num, cSkillInfoLevel.sName, cSkillInfoLevel.sDesc);
				}
			}
			num = dataCenter3.GetSelectPassiveSkill(dataCenter3.CurCharID, 1);
			if (dataCenter3.GetPassiveSkill(num, ref nSkillLevel))
			{
				cSkillInfoLevel = gameData3.GetSkillInfo(num, nSkillLevel);
				if (cSkillInfoLevel != null)
				{
					tUIGameInfo3.equip_info.skill03 = new TUIPopupInfo(num, cSkillInfoLevel.sName, cSkillInfoLevel.sDesc);
				}
			}
			num = dataCenter3.GetSelectPassiveSkill(dataCenter3.CurCharID, 2);
			if (dataCenter3.GetPassiveSkill(num, ref nSkillLevel))
			{
				cSkillInfoLevel = gameData3.GetSkillInfo(num, nSkillLevel);
				if (cSkillInfoLevel != null)
				{
					tUIGameInfo3.equip_info.skill04 = new TUIPopupInfo(num, cSkillInfoLevel.sName, cSkillInfoLevel.sDesc);
				}
			}
			tUIGameInfo3.equip_info.skill_list = new List<TUIPopupInfo>();
			int curCharID = dataCenter3.CurCharID;
			CCharacterInfo characterInfo3 = gameData3.GetCharacterInfo(curCharID);
			if (characterInfo3 != null && characterInfo3.ltCharacterPassiveSkill != null)
			{
				for (int j = 0; j < characterInfo3.ltCharacterPassiveSkill.Count; j++)
				{
					CSkillInfo skillInfo = gameData3.GetSkillInfo(characterInfo3.ltCharacterPassiveSkill[j]);
					if (skillInfo == null)
					{
						continue;
					}
					int nSkillLevel2 = 0;
					if (!dataCenter3.GetPassiveSkill(skillInfo.nID, ref nSkillLevel2))
					{
						continue;
					}
					cSkillInfoLevel = skillInfo.Get(nSkillLevel2);
					if (cSkillInfoLevel != null && cSkillInfoLevel.nType == 1)
					{
						tUIGameInfo3.equip_info.skill_list.Add(new TUIPopupInfo(skillInfo.nID, cSkillInfoLevel.sName, cSkillInfoLevel.sDesc));
						if (iGameApp.GetInstance().CheckSkillSignState(3, skillInfo.nID))
						{
							tUIGameInfo3.equip_info.AddSkillNewMark(skillInfo.nID, NewMarkType.New);
						}
						else
						{
							tUIGameInfo3.equip_info.AddSkillNewMark(skillInfo.nID, NewMarkType.None);
						}
					}
				}
			}
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneEquip(m_event.GetEventName(), tUIGameInfo3));
		}
		else if (m_event.GetEventName() == "TUIEvent_WeaponSign")
		{
			iGameData gameData4 = iGameApp.GetInstance().m_GameData;
			if (gameData4 == null)
			{
				return;
			}
			iDataCenter dataCenter4 = gameData4.GetDataCenter();
			if (dataCenter4 == null)
			{
				return;
			}
			iWeaponCenter weaponCenter = gameData4.GetWeaponCenter();
			if (weaponCenter == null)
			{
				return;
			}
			iItemCenter itemCenter = gameData4.GetItemCenter();
			if (itemCenter == null)
			{
				return;
			}
			TUIGameInfo tUIGameInfo4 = new TUIGameInfo();
			tUIGameInfo4.equip_info = new TUIEquipInfo();
			int num2 = -1;
			int num3 = -1;
			CWeaponInfoLevel cWeaponInfoLevel = null;
			num2 = dataCenter4.GetSelectWeapon(0);
			num3 = dataCenter4.GetWeaponLevel(num2);
			cWeaponInfoLevel = gameData4.GetWeaponInfo(num2, num3);
			if (cWeaponInfoLevel != null)
			{
				TUIWeaponAttribute tUIWeaponAttribute = new TUIWeaponAttribute(0f, 0f, 0f, 0f, 0f);
				tUIWeaponAttribute.ammo = cWeaponInfoLevel.nCapacity;
				if (cWeaponInfoLevel.nType == 5)
				{
					tUIWeaponAttribute.blast_radius = 20f;
				}
				tUIWeaponAttribute.damage = (int)cWeaponInfoLevel.fDamage;
				tUIWeaponAttribute.fire_rate = cWeaponInfoLevel.fShootSpeed;
				for (int k = 0; k < 3; k++)
				{
					if (cWeaponInfoLevel.arrFunc[k] == 4)
					{
						tUIWeaponAttribute.knockback = cWeaponInfoLevel.arrValueY[k];
						break;
					}
				}
				tUIGameInfo4.equip_info.weapon01 = new TUIPopupInfo(num2, cWeaponInfoLevel.sName, string.Empty, tUIWeaponAttribute);
			}
			num2 = dataCenter4.GetSelectWeapon(1);
			num3 = dataCenter4.GetWeaponLevel(num2);
			cWeaponInfoLevel = gameData4.GetWeaponInfo(num2, num3);
			if (cWeaponInfoLevel != null)
			{
				TUIWeaponAttribute tUIWeaponAttribute2 = new TUIWeaponAttribute(0f, 0f, 0f, 0f, 0f);
				tUIWeaponAttribute2.ammo = cWeaponInfoLevel.nCapacity;
				if (cWeaponInfoLevel.nType == 5)
				{
					tUIWeaponAttribute2.blast_radius = 20f;
				}
				tUIWeaponAttribute2.damage = (int)cWeaponInfoLevel.fDamage;
				tUIWeaponAttribute2.fire_rate = cWeaponInfoLevel.fShootSpeed;
				for (int l = 0; l < 3; l++)
				{
					if (cWeaponInfoLevel.arrFunc[l] == 4)
					{
						tUIWeaponAttribute2.knockback = cWeaponInfoLevel.arrValueY[l];
						break;
					}
				}
				tUIGameInfo4.equip_info.weapon02 = new TUIPopupInfo(num2, cWeaponInfoLevel.sName, string.Empty, tUIWeaponAttribute2);
			}
			num2 = dataCenter4.GetSelectWeapon(2);
			num3 = dataCenter4.GetWeaponLevel(num2);
			cWeaponInfoLevel = gameData4.GetWeaponInfo(num2, num3);
			if (cWeaponInfoLevel != null)
			{
				TUIWeaponAttribute tUIWeaponAttribute3 = new TUIWeaponAttribute(0f, 0f, 0f, 0f, 0f);
				tUIWeaponAttribute3.ammo = cWeaponInfoLevel.nCapacity;
				if (cWeaponInfoLevel.nType == 5)
				{
					tUIWeaponAttribute3.blast_radius = 20f;
				}
				tUIWeaponAttribute3.damage = (int)cWeaponInfoLevel.fDamage;
				tUIWeaponAttribute3.fire_rate = cWeaponInfoLevel.fShootSpeed;
				for (int m = 0; m < 3; m++)
				{
					if (cWeaponInfoLevel.arrFunc[m] == 4)
					{
						tUIWeaponAttribute3.knockback = cWeaponInfoLevel.arrValueY[m];
						break;
					}
				}
				tUIGameInfo4.equip_info.weapon03 = new TUIPopupInfo(num2, cWeaponInfoLevel.sName, string.Empty, tUIWeaponAttribute3);
			}
			int nItemLevel = 0;
			if (dataCenter4.GetEquipStone(dataCenter4.CurEquipStone, ref nItemLevel))
			{
				CItemInfoLevel itemInfo = gameData4.GetItemInfo(dataCenter4.CurEquipStone, nItemLevel);
				if (itemInfo != null && itemInfo.nType == 1)
				{
					TUIStoneskinAttribute tUIStoneskinAttribute = new TUIStoneskinAttribute(0f);
					for (int n = 0; n < 3; n++)
					{
						if (itemInfo.arrFunc[n] == 1)
						{
							kProEnum kProEnum2 = (kProEnum)MyUtils.Low32(itemInfo.arrValueX[n]);
							if (kProEnum2 == kProEnum.HPMax)
							{
								tUIStoneskinAttribute.hp = itemInfo.arrValueY[n];
								break;
							}
						}
					}
					tUIGameInfo4.equip_info.weapon04 = new TUIPopupInfo(itemInfo.nID, itemInfo.sName, itemInfo.sDesc, tUIStoneskinAttribute);
				}
			}
			tUIGameInfo4.equip_info.weapon_list01 = new List<TUIPopupInfo>();
			tUIGameInfo4.equip_info.weapon_list02 = new List<TUIPopupInfo>();
			Dictionary<int, CWeaponInfo> data = weaponCenter.GetData();
			if (data != null)
			{
				foreach (CWeaponInfo value in data.Values)
				{
					int weaponLevel = dataCenter4.GetWeaponLevel(value.nID);
					if (weaponLevel == -1)
					{
						continue;
					}
					cWeaponInfoLevel = value.Get(weaponLevel);
					if (cWeaponInfoLevel == null)
					{
						continue;
					}
					TUIWeaponAttribute tUIWeaponAttribute4 = new TUIWeaponAttribute(0f, 0f, 0f, 0f, 0f);
					tUIWeaponAttribute4.ammo = cWeaponInfoLevel.nCapacity;
					if (cWeaponInfoLevel.nType == 5)
					{
						tUIWeaponAttribute4.blast_radius = 20f;
					}
					tUIWeaponAttribute4.damage = (int)cWeaponInfoLevel.fDamage;
					tUIWeaponAttribute4.fire_rate = cWeaponInfoLevel.fShootSpeed;
					for (int num4 = 0; num4 < 3; num4++)
					{
						if (cWeaponInfoLevel.arrFunc[num4] == 4)
						{
							tUIWeaponAttribute4.knockback = cWeaponInfoLevel.arrValueY[num4];
							break;
						}
					}
					if (cWeaponInfoLevel.nType == 1)
					{
						tUIGameInfo4.equip_info.weapon_list01.Add(new TUIPopupInfo(value.nID, cWeaponInfoLevel.sName, string.Empty, tUIWeaponAttribute4));
					}
					else
					{
						tUIGameInfo4.equip_info.weapon_list02.Add(new TUIPopupInfo(value.nID, cWeaponInfoLevel.sName, string.Empty, tUIWeaponAttribute4));
					}
					if (iGameApp.GetInstance().CheckWeaponSignState(3, value.nID))
					{
						tUIGameInfo4.equip_info.AddWeaponNewMark(value.nID, NewMarkType.New);
					}
					else
					{
						tUIGameInfo4.equip_info.AddWeaponNewMark(value.nID, NewMarkType.None);
					}
				}
			}
			tUIGameInfo4.equip_info.weapon_list03 = new List<TUIPopupInfo>();
			Dictionary<int, CItemInfo> data2 = itemCenter.GetData();
			if (data2 != null)
			{
				foreach (CItemInfo value2 in data2.Values)
				{
					int nItemLevel2 = 0;
					if (!dataCenter4.GetEquipStone(value2.nID, ref nItemLevel2))
					{
						continue;
					}
					CItemInfoLevel cItemInfoLevel = value2.Get(nItemLevel2);
					if (cItemInfoLevel == null || cItemInfoLevel.nType != 1)
					{
						continue;
					}
					TUIStoneskinAttribute tUIStoneskinAttribute2 = new TUIStoneskinAttribute(0f);
					for (int num5 = 0; num5 < 3; num5++)
					{
						if (cItemInfoLevel.arrFunc[num5] == 1)
						{
							kProEnum kProEnum3 = (kProEnum)MyUtils.Low32(cItemInfoLevel.arrValueX[num5]);
							if (kProEnum3 == kProEnum.HPMax)
							{
								tUIStoneskinAttribute2.hp = cItemInfoLevel.arrValueY[num5];
								break;
							}
						}
					}
					tUIGameInfo4.equip_info.weapon_list03.Add(new TUIPopupInfo(cItemInfoLevel.nID, cItemInfoLevel.sName, cItemInfoLevel.sDesc, tUIStoneskinAttribute2));
					if (iGameApp.GetInstance().CheckEquipStoneSignState(3, value2.nID))
					{
						tUIGameInfo4.equip_info.AddWeaponNewMark(value2.nID, NewMarkType.New);
					}
					else
					{
						tUIGameInfo4.equip_info.AddWeaponNewMark(value2.nID, NewMarkType.None);
					}
				}
			}
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneEquip(m_event.GetEventName(), tUIGameInfo4));
		}
		else if (m_event.GetEventName() == "TUIEvent_RoleEquip")
		{
			iGameData gameData5 = iGameApp.GetInstance().m_GameData;
			if (gameData5 == null)
			{
				return;
			}
			iDataCenter dataCenter5 = gameData5.GetDataCenter();
			if (dataCenter5 == null)
			{
				return;
			}
			int wParam = m_event.GetWParam();
			if (dataCenter5.GetCharacter(wParam) == null)
			{
				return;
			}
			dataCenter5.CurCharID = wParam;
			TUIGameInfo tUIGameInfo5 = new TUIGameInfo();
			tUIGameInfo5.player_info = new TUIPlayerInfo();
			tUIGameInfo5.equip_info = new TUIEquipInfo();
			CCharSaveInfo character5 = dataCenter5.GetCharacter(dataCenter5.CurCharID);
			CCharacterInfoLevel characterInfo4 = gameData5.GetCharacterInfo(character5.nID, character5.nLevel);
			if (character5 != null && characterInfo4 != null)
			{
				tUIGameInfo5.player_info.avatar_id = character5.nID;
				tUIGameInfo5.player_info.level = character5.nLevel;
				tUIGameInfo5.player_info.level_exp = characterInfo4.nExp;
				tUIGameInfo5.player_info.exp = character5.nExp;
				tUIGameInfo5.player_info.gold = dataCenter5.Gold;
				tUIGameInfo5.player_info.crystal = dataCenter5.Crystal;
				int num6 = -1;
				int nSkillLevel3 = 0;
				CSkillInfoLevel cSkillInfoLevel2 = null;
				num6 = characterInfo4.nSkill;
				cSkillInfoLevel2 = gameData5.GetSkillInfo(num6, 1);
				if (cSkillInfoLevel2 != null)
				{
					tUIGameInfo5.equip_info.skill01 = new TUIPopupInfo(num6, cSkillInfoLevel2.sName, cSkillInfoLevel2.sDesc);
					tUIGameInfo5.equip_info.AddSkillNewMark(num6, NewMarkType.None);
				}
				num6 = dataCenter5.GetSelectPassiveSkill(dataCenter5.CurCharID, 0);
				if (dataCenter5.GetPassiveSkill(num6, ref nSkillLevel3))
				{
					cSkillInfoLevel2 = gameData5.GetSkillInfo(num6, nSkillLevel3);
					if (cSkillInfoLevel2 != null)
					{
						tUIGameInfo5.equip_info.skill02 = new TUIPopupInfo(num6, cSkillInfoLevel2.sName, cSkillInfoLevel2.sDesc);
					}
				}
				num6 = dataCenter5.GetSelectPassiveSkill(dataCenter5.CurCharID, 1);
				if (dataCenter5.GetPassiveSkill(num6, ref nSkillLevel3))
				{
					cSkillInfoLevel2 = gameData5.GetSkillInfo(num6, nSkillLevel3);
					if (cSkillInfoLevel2 != null)
					{
						tUIGameInfo5.equip_info.skill03 = new TUIPopupInfo(num6, cSkillInfoLevel2.sName, cSkillInfoLevel2.sDesc);
					}
				}
				num6 = dataCenter5.GetSelectPassiveSkill(dataCenter5.CurCharID, 2);
				if (dataCenter5.GetPassiveSkill(num6, ref nSkillLevel3))
				{
					cSkillInfoLevel2 = gameData5.GetSkillInfo(num6, nSkillLevel3);
					if (cSkillInfoLevel2 != null)
					{
						tUIGameInfo5.equip_info.skill04 = new TUIPopupInfo(num6, cSkillInfoLevel2.sName, cSkillInfoLevel2.sDesc);
					}
				}
				tUIGameInfo5.equip_info.skill_list = new List<TUIPopupInfo>();
				int curCharID2 = dataCenter5.CurCharID;
				CCharacterInfo characterInfo5 = gameData5.GetCharacterInfo(curCharID2);
				if (characterInfo5 != null && characterInfo5.ltCharacterPassiveSkill != null)
				{
					for (int num7 = 0; num7 < characterInfo5.ltCharacterPassiveSkill.Count; num7++)
					{
						CSkillInfo skillInfo2 = gameData5.GetSkillInfo(characterInfo5.ltCharacterPassiveSkill[num7]);
						if (skillInfo2 == null)
						{
							continue;
						}
						int nSkillLevel4 = 0;
						if (!dataCenter5.GetPassiveSkill(skillInfo2.nID, ref nSkillLevel4))
						{
							continue;
						}
						cSkillInfoLevel2 = skillInfo2.Get(nSkillLevel4);
						if (cSkillInfoLevel2 != null && cSkillInfoLevel2.nType == 1)
						{
							tUIGameInfo5.equip_info.skill_list.Add(new TUIPopupInfo(skillInfo2.nID, cSkillInfoLevel2.sName, cSkillInfoLevel2.sDesc));
							if (iGameApp.GetInstance().CheckSkillSignState(3, skillInfo2.nID))
							{
								tUIGameInfo5.equip_info.AddSkillNewMark(skillInfo2.nID, NewMarkType.New);
							}
							else
							{
								tUIGameInfo5.equip_info.AddSkillNewMark(skillInfo2.nID, NewMarkType.None);
							}
						}
					}
				}
			}
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneEquip(m_event.GetEventName(), tUIGameInfo5, true));
		}
		else if (m_event.GetEventName() == "TUIEvent_SkillEquip")
		{
			iGameData gameData6 = iGameApp.GetInstance().m_GameData;
			if (gameData6 != null)
			{
				iDataCenter dataCenter6 = gameData6.GetDataCenter();
				if (dataCenter6 != null)
				{
					int wParam2 = m_event.GetWParam();
					int lparam = m_event.GetLparam();
					dataCenter6.SetSelectPassiveSkill(dataCenter6.CurCharID, wParam2 - 2, lparam);
					global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneEquip(m_event.GetEventName(), true));
				}
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_SkillUnEquip")
		{
			iGameData gameData7 = iGameApp.GetInstance().m_GameData;
			if (gameData7 != null)
			{
				iDataCenter dataCenter7 = gameData7.GetDataCenter();
				if (dataCenter7 != null)
				{
					int wParam3 = m_event.GetWParam();
					int lparam2 = m_event.GetLparam();
					dataCenter7.SetSelectPassiveSkill(dataCenter7.CurCharID, wParam3 - 2, -1);
					global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneEquip(m_event.GetEventName(), true));
				}
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_SkillExchange")
		{
			iGameData gameData8 = iGameApp.GetInstance().m_GameData;
			if (gameData8 != null)
			{
				iDataCenter dataCenter8 = gameData8.GetDataCenter();
				if (dataCenter8 != null)
				{
					int wParam4 = m_event.GetWParam();
					int lparam3 = m_event.GetLparam();
					int selectPassiveSkill = dataCenter8.GetSelectPassiveSkill(dataCenter8.CurCharID, wParam4 - 1);
					int selectPassiveSkill2 = dataCenter8.GetSelectPassiveSkill(dataCenter8.CurCharID, lparam3 - 1);
					dataCenter8.SetSelectPassiveSkill(dataCenter8.CurCharID, wParam4 - 1, selectPassiveSkill2);
					dataCenter8.SetSelectPassiveSkill(dataCenter8.CurCharID, lparam3 - 1, selectPassiveSkill);
					global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneEquip(m_event.GetEventName(), true));
				}
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_WeaponEquip")
		{
			iGameData gameData9 = iGameApp.GetInstance().m_GameData;
			if (gameData9 == null)
			{
				return;
			}
			iDataCenter dataCenter9 = gameData9.GetDataCenter();
			if (dataCenter9 != null)
			{
				int wParam5 = m_event.GetWParam();
				int lparam4 = m_event.GetLparam();
				if (dataCenter9.GetWeaponLevel(lparam4) != -1)
				{
					dataCenter9.SetSelectWeapon(wParam5 - 1, lparam4);
				}
				int nItemLevel3 = 0;
				if (dataCenter9.GetEquipStone(lparam4, ref nItemLevel3))
				{
					dataCenter9.CurEquipStone = lparam4;
				}
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneEquip(m_event.GetEventName(), true));
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_WeaponExchange")
		{
			iGameData gameData10 = iGameApp.GetInstance().m_GameData;
			if (gameData10 != null)
			{
				iDataCenter dataCenter10 = gameData10.GetDataCenter();
				if (dataCenter10 != null)
				{
					int wParam6 = m_event.GetWParam();
					int lparam5 = m_event.GetLparam();
					int selectWeapon = dataCenter10.GetSelectWeapon(wParam6 - 1);
					int selectWeapon2 = dataCenter10.GetSelectWeapon(lparam5 - 1);
					dataCenter10.SetSelectWeapon(wParam6 - 1, selectWeapon2);
					dataCenter10.SetSelectWeapon(lparam5 - 1, selectWeapon);
					global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneEquip(m_event.GetEventName(), true));
				}
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_Back")
		{
			iGameData gameData11 = iGameApp.GetInstance().m_GameData;
			if (gameData11 == null)
			{
				return;
			}
			iDataCenter dataCenter11 = gameData11.GetDataCenter();
			if (dataCenter11 == null)
			{
				return;
			}
			dataCenter11.Save();
			iGameState gameState2 = iGameApp.GetInstance().m_GameState;
			if (gameState2 != null)
			{
				if (gameState2.m_curScene4Recommand != 0)
				{
					global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneEquip(m_event.GetEventName(), true, (int)gameState2.m_curScene4Recommand));
					gameState2.m_curScene4Recommand = TUISceneType.None;
				}
				else
				{
					global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneEquip(m_event.GetEventName(), true));
				}
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_RolesChoose")
		{
			int wParam7 = m_event.GetWParam();
			iGameData gameData12 = iGameApp.GetInstance().m_GameData;
			if (gameData12 == null)
			{
				return;
			}
			iDataCenter dataCenter12 = gameData12.GetDataCenter();
			if (dataCenter12 == null)
			{
				return;
			}
			int nSignState = 0;
			if (dataCenter12.GetCharacterSign(wParam7, ref nSignState))
			{
				if (nSignState == 3)
				{
					dataCenter12.SetCharacterSign(wParam7, 4);
					dataCenter12.Save();
				}
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneEquip(m_event.GetEventName()));
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_WeaponChoose")
		{
			int wParam8 = m_event.GetWParam();
			iGameData gameData13 = iGameApp.GetInstance().m_GameData;
			if (gameData13 == null)
			{
				return;
			}
			iDataCenter dataCenter13 = gameData13.GetDataCenter();
			if (dataCenter13 != null)
			{
				int nSignState2 = 0;
				if (dataCenter13.GetWeaponSign(wParam8, ref nSignState2) && nSignState2 == 3)
				{
					dataCenter13.SetWeaponSign(wParam8, 4);
					dataCenter13.Save();
				}
				if (dataCenter13.GetEquipStoneSign(wParam8, ref nSignState2) && nSignState2 == 3)
				{
					dataCenter13.SetEquipStoneSign(wParam8, 4);
					dataCenter13.Save();
				}
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneEquip(m_event.GetEventName()));
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_SkillChoose")
		{
			int wParam9 = m_event.GetWParam();
			iGameData gameData14 = iGameApp.GetInstance().m_GameData;
			if (gameData14 == null)
			{
				return;
			}
			iDataCenter dataCenter14 = gameData14.GetDataCenter();
			if (dataCenter14 == null)
			{
				return;
			}
			int nSignState3 = 0;
			if (dataCenter14.GetSkillSign(wParam9, ref nSignState3))
			{
				if (nSignState3 == 3)
				{
					dataCenter14.SetSkillSign(wParam9, 4);
					dataCenter14.Save();
				}
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneEquip(m_event.GetEventName()));
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_EnterIAP")
		{
			iGameState gameState3 = iGameApp.GetInstance().m_GameState;
			if (gameState3 != null)
			{
				gameState3.m_lstScene4IAP = TUISceneType.Scene_Equip;
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneEquip(m_event.GetEventName(), true));
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_EnterGold")
		{
			iGameState gameState4 = iGameApp.GetInstance().m_GameState;
			if (gameState4 != null)
			{
				gameState4.m_lstScene4IAP = TUISceneType.Scene_Equip;
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneEquip(m_event.GetEventName(), true));
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_EnterGoBuyWeapon")
		{
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneEquip(m_event.GetEventName(), true));
		}
		else if (m_event.GetEventName() == "TUIEvent_EnterGoBuySkill")
		{
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneEquip(m_event.GetEventName(), true));
		}
	}

	private void TUIEvent_BackInfo_SceneStash(object sender, TUIEvent.SendEvent_SceneStash m_event)
	{
		if (m_event.GetEventName() == "TUIEvent_TopBar")
		{
			iGameData gameData = iGameApp.GetInstance().m_GameData;
			if (gameData == null)
			{
				return;
			}
			iDataCenter dataCenter = gameData.GetDataCenter();
			if (dataCenter == null)
			{
				return;
			}
			CCharSaveInfo character = dataCenter.GetCharacter(dataCenter.CurCharID);
			if (character != null)
			{
				CCharacterInfoLevel characterInfo = gameData.GetCharacterInfo(character.nID, character.nLevel);
				if (characterInfo != null)
				{
					TUIGameInfo tUIGameInfo = new TUIGameInfo();
					tUIGameInfo.player_info = new TUIPlayerInfo();
					tUIGameInfo.player_info.avatar_id = character.nID;
					tUIGameInfo.player_info.level = character.nLevel;
					tUIGameInfo.player_info.level_exp = characterInfo.nExp;
					tUIGameInfo.player_info.exp = character.nExp;
					tUIGameInfo.player_info.gold = dataCenter.Gold;
					tUIGameInfo.player_info.crystal = dataCenter.Crystal;
					global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneStash(m_event.GetEventName(), tUIGameInfo));
				}
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_StashInfo")
		{
			iGameData gameData2 = iGameApp.GetInstance().m_GameData;
			if (gameData2 == null)
			{
				return;
			}
			iDataCenter dataCenter2 = gameData2.GetDataCenter();
			if (dataCenter2 == null)
			{
				return;
			}
			iItemCenter itemCenter = gameData2.GetItemCenter();
			if (itemCenter == null)
			{
				return;
			}
			iStashCapacityCenter stashCapacityCenter = gameData2.GetStashCapacityCenter();
			if (stashCapacityCenter == null)
			{
				return;
			}
			TUIGameInfo tUIGameInfo2 = new TUIGameInfo();
			tUIGameInfo2.stash_info = new TUIStashInfo();
			List<TUIStashUpdateInfo> list = new List<TUIStashUpdateInfo>();
			Dictionary<ProtectedInt32, CStashCapacity> data = stashCapacityCenter.GetData();
			if (data != null)
			{
				foreach (CStashCapacity value in data.Values)
				{
					list.Add(new TUIStashUpdateInfo(value.nLevel, new TUIPriceInfo(value.nPrice, value.isCrystalPurchase ? UnitType.Crystal : UnitType.Gold), value.nCapacity, value.sLevelUpDesc));
				}
			}
			tUIGameInfo2.stash_info.goods_info_list = new List<TUIGoodsInfo>();
			Dictionary<int, CItemInfo> data2 = itemCenter.GetData();
			if (data2 != null)
			{
				CItemInfoLevel cItemInfoLevel = null;
				int num = 0;
				foreach (CItemInfo value2 in data2.Values)
				{
					cItemInfoLevel = value2.Get(1);
					if (cItemInfoLevel != null && cItemInfoLevel.nType == 3)
					{
						num = dataCenter2.GetMaterialNum(value2.nID);
						if (num == -1)
						{
							num = 0;
						}
						GoodsQualityType quality = GoodsQualityType.Quality01;
						switch (cItemInfoLevel.nRare)
						{
						case 1:
							quality = GoodsQualityType.Quality01;
							break;
						case 2:
							quality = GoodsQualityType.Quality02;
							break;
						case 3:
							quality = GoodsQualityType.Quality03;
							break;
						case 4:
							quality = GoodsQualityType.Quality04;
							break;
						case 5:
							quality = GoodsQualityType.Quality05;
							break;
						case 6:
							quality = GoodsQualityType.Quality06;
							break;
						}
						tUIGameInfo2.stash_info.goods_info_list.Add(new TUIGoodsInfo(value2.nID, quality, cItemInfoLevel.sName, num, new TUIPriceInfo(cItemInfoLevel.nSellPrice, cItemInfoLevel.isCrystalSell ? UnitType.Crystal : UnitType.Gold)));
					}
				}
			}
			Debug.Log(dataCenter2.StashLevel);
			tUIGameInfo2.stash_info = new TUIStashInfo(dataCenter2.StashLevel, list.ToArray(), tUIGameInfo2.stash_info.goods_info_list);
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneStash(m_event.GetEventName(), tUIGameInfo2));
		}
		else if (m_event.GetEventName() == "TUIEvent_AddCapacity")
		{
			bool success = false;
			iGameData gameData3 = iGameApp.GetInstance().m_GameData;
			if (gameData3 != null)
			{
				iDataCenter dataCenter3 = gameData3.GetDataCenter();
				if (dataCenter3 != null)
				{
					CStashCapacity stashCapacity = gameData3.GetStashCapacity(dataCenter3.StashLevel);
					CStashCapacity stashCapacity2 = gameData3.GetStashCapacity(dataCenter3.StashLevel + 1);
					if (stashCapacity != null && stashCapacity2 != null)
					{
						if (stashCapacity.isCrystalPurchase)
						{
							if (dataCenter3.Crystal < stashCapacity.nPrice)
							{
								global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneStash(m_event.GetEventName(), false, BackEventFalseType.NoCrystalEnough, stashCapacity.nPrice - dataCenter3.Crystal));
								return;
							}
							dataCenter3.AddCrystal(-stashCapacity.nPrice);
							CAchievementManager.GetInstance().AddAchievement(13);
							dataCenter3.StashLevel++;
							dataCenter3.Save();
							success = true;
							//CFlurryManager.GetInstance().ConsumeCrystal(CFlurryManager.kConsumeType.StashSize);
						}
						else
						{
							if (dataCenter3.Gold < stashCapacity.nPrice)
							{
								global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneStash(m_event.GetEventName(), false, BackEventFalseType.NoGoldEnough, stashCapacity.nPrice - dataCenter3.Gold));
								return;
							}
							dataCenter3.AddGold(-stashCapacity.nPrice);
							dataCenter3.StashLevel++;
							dataCenter3.Save();
							success = true;
							//CFlurryManager.GetInstance().ConsumeGold(CFlurryManager.kConsumeType.StashSize);
						}
					}
				}
			}
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneStash(m_event.GetEventName(), success));
		}
		else if (m_event.GetEventName() == "TUIEvent_SellGoods")
		{
			iGameData gameData4 = iGameApp.GetInstance().m_GameData;
			if (gameData4 == null)
			{
				return;
			}
			iDataCenter dataCenter4 = gameData4.GetDataCenter();
			if (dataCenter4 == null)
			{
				return;
			}
			bool flag = false;
			int wParam = m_event.GetWParam();
			int lparam = m_event.GetLparam();
			int rparam = m_event.GetRparam();
			CItemInfoLevel itemInfo = gameData4.GetItemInfo(wParam, 1);
			if (itemInfo == null || itemInfo.nType != 3)
			{
				return;
			}
			int materialNum = dataCenter4.GetMaterialNum(wParam);
			if (materialNum != -1)
			{
				materialNum = ((rparam <= materialNum) ? (materialNum - rparam) : 0);
				dataCenter4.SetMaterialNum(wParam, materialNum);
				if (itemInfo.isCrystalSell)
				{
					dataCenter4.AddCrystal(itemInfo.nSellPrice * rparam);
				}
				else
				{
					dataCenter4.AddGold(itemInfo.nSellPrice * rparam);
				}
				flag = true;
				dataCenter4.Save();
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneStash(m_event.GetEventName(), flag));
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_Back")
		{
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneStash(m_event.GetEventName(), true));
		}
		else if (m_event.GetEventName() == "TUIEvent_SearchGoodsDrop")
		{
			int wParam2 = m_event.GetWParam();
			iGameState gameState = iGameApp.GetInstance().m_GameState;
			if (gameState != null)
			{
				gameState.m_nMaterialIDFromEquip = wParam2;
				gameState.m_lstScene4SearchMaterial = TUISceneType.Scene_Stash;
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneStash(m_event.GetEventName()));
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_GoldToCrystal")
		{
			iGameData gameData5 = iGameApp.GetInstance().m_GameData;
			if (gameData5 == null)
			{
				return;
			}
			iDataCenter dataCenter5 = gameData5.GetDataCenter();
			if (dataCenter5 != null)
			{
				int wParam3 = m_event.GetWParam();
				int num2 = MyUtils.Formula_Gold2Crystal(wParam3);
				if (dataCenter5.Crystal < num2)
				{
					global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneStash(m_event.GetEventName(), false, BackEventFalseType.NoCrystalEnough, num2 - dataCenter5.Crystal));
					return;
				}
				dataCenter5.AddCrystal(-num2);
				dataCenter5.AddGold(wParam3);
				dataCenter5.Save();
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneStash(m_event.GetEventName(), true));
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_EnterIAP")
		{
			iGameState gameState2 = iGameApp.GetInstance().m_GameState;
			if (gameState2 != null)
			{
				gameState2.m_lstScene4IAP = TUISceneType.Scene_Stash;
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneStash(m_event.GetEventName(), true));
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_EnterGold")
		{
			iGameState gameState3 = iGameApp.GetInstance().m_GameState;
			if (gameState3 != null)
			{
				gameState3.m_lstScene4IAP = TUISceneType.Scene_Stash;
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneStash(m_event.GetEventName(), true));
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_EnterIAPCrystalNoEnough")
		{
			iGameState gameState4 = iGameApp.GetInstance().m_GameState;
			if (gameState4 != null)
			{
				gameState4.m_lstScene4IAP = TUISceneType.Scene_Stash;
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneStash(m_event.GetEventName(), true));
			}
		}
	}

	private void TUIEvent_BackInfo_SceneSkill(object sender, TUIEvent.SendEvent_SceneSkill m_event)
	{
		if (m_event.GetEventName() == "TUIEvent_TopBar")
		{
			iGameData gameData = iGameApp.GetInstance().m_GameData;
			if (gameData == null)
			{
				return;
			}
			iDataCenter dataCenter = gameData.GetDataCenter();
			if (dataCenter == null)
			{
				return;
			}
			CCharSaveInfo character = dataCenter.GetCharacter(dataCenter.CurCharID);
			if (character != null)
			{
				CCharacterInfoLevel characterInfo = gameData.GetCharacterInfo(character.nID, character.nLevel);
				if (characterInfo != null)
				{
					TUIGameInfo tUIGameInfo = new TUIGameInfo();
					tUIGameInfo.player_info = new TUIPlayerInfo();
					tUIGameInfo.player_info.avatar_id = character.nID;
					tUIGameInfo.player_info.level = character.nLevel;
					tUIGameInfo.player_info.level_exp = characterInfo.nExp;
					tUIGameInfo.player_info.exp = character.nExp;
					tUIGameInfo.player_info.gold = dataCenter.Gold;
					tUIGameInfo.player_info.crystal = dataCenter.Crystal;
					global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneSkill(m_event.GetEventName(), tUIGameInfo));
				}
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_SkillInfo")
		{
			iGameData gameData2 = iGameApp.GetInstance().m_GameData;
			if (gameData2 == null)
			{
				return;
			}
			iDataCenter dataCenter2 = gameData2.GetDataCenter();
			if (dataCenter2 == null)
			{
				return;
			}
			CCharacterInfo cCharacterInfo = null;
			CSkillInfo cSkillInfo = null;
			CSkillInfoLevel cSkillInfoLevel = null;
			int[] array = new int[6] { 1, 6, 4, 3, 2, 5 };
			TUISkillListInfo[] array2 = new TUISkillListInfo[array.Length];
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = new TUISkillListInfo(array[i], null);
				List<TUISkillInfo> list = new List<TUISkillInfo>();
				cCharacterInfo = gameData2.GetCharacterInfo(array[i]);
				if (cCharacterInfo != null)
				{
					for (int j = 0; j < cCharacterInfo.ltCharacterPassiveSkill.Count; j++)
					{
						cSkillInfo = gameData2.GetSkillInfo(cCharacterInfo.ltCharacterPassiveSkill[j]);
						if (cSkillInfo == null)
						{
							continue;
						}
						Dictionary<int, TUIPriceInfo> dictionary = new Dictionary<int, TUIPriceInfo>();
						for (int k = 1; k <= 5; k++)
						{
							cSkillInfoLevel = cSkillInfo.Get(k);
							if (cSkillInfoLevel != null)
							{
								dictionary.Add(k, new TUIPriceInfo(cSkillInfoLevel.nPurchasePrice, cSkillInfoLevel.isCrystalPurchase ? UnitType.Crystal : UnitType.Gold));
							}
							else
							{
								dictionary.Add(k, new TUIPriceInfo(999999, UnitType.Crystal));
							}
						}
						Dictionary<int, string> dictionary2 = new Dictionary<int, string>();
						Dictionary<int, string> dictionary3 = new Dictionary<int, string>();
						for (int l = 1; l <= 5; l++)
						{
							cSkillInfoLevel = cSkillInfo.Get(l);
							if (cSkillInfoLevel != null)
							{
								dictionary2.Add(l, cSkillInfoLevel.sLevelUpDesc);
								dictionary3.Add(l, cSkillInfoLevel.sDesc);
							}
						}
						cSkillInfoLevel = cSkillInfo.Get(1);
						if (cSkillInfoLevel != null)
						{
							int nSkillLevel = 0;
							dataCenter2.GetPassiveSkill(cSkillInfo.nID, ref nSkillLevel);
							string skill_introduce_unlock = "Unlock at Lv " + cSkillInfo.nUnlockLevel;
							list.Add(new TUISkillInfo(cSkillInfo.nID, cSkillInfoLevel.sName, (nSkillLevel != -1) ? nSkillLevel : 0, nSkillLevel != 0, new TUIPriceInfo(cSkillInfo.nUnlockPrice, cSkillInfo.isCrystalUnlock ? UnitType.Crystal : UnitType.Gold), dictionary, dictionary2, dictionary3, skill_introduce_unlock));
							if (iGameApp.GetInstance().CheckSkillSignState(1, cSkillInfo.nID))
							{
								array2[i].AddNewMark(cSkillInfo.nID, NewMarkType.New);
							}
							else if (iGameApp.GetInstance().CheckSkillMaterialEnough(cSkillInfo.nID))
							{
								array2[i].AddNewMark(cSkillInfo.nID, NewMarkType.Mark);
							}
							else
							{
								array2[i].AddNewMark(cSkillInfo.nID, NewMarkType.None);
							}
						}
					}
				}
				array2[i].skill_list_info = list.ToArray();
			}
			TUIGameInfo tUIGameInfo2 = new TUIGameInfo();
			tUIGameInfo2.all_skill_info = new TUIAllSkillInfo(array2);
			iGameState gameState = iGameApp.GetInstance().m_GameState;
			if (gameState != null && gameState.m_nLinkSkillRole > 0 && gameState.m_nLinkSkill > 0)
			{
				cCharacterInfo = gameData2.GetCharacterInfo(gameState.m_nLinkSkillRole);
				if (cCharacterInfo != null)
				{
					for (int m = 0; m < cCharacterInfo.ltCharacterPassiveSkill.Count; m++)
					{
						if (gameState.m_nLinkSkill == cCharacterInfo.ltCharacterPassiveSkill[m])
						{
							Debug.Log(gameState.m_nLinkSkillRole + " " + gameState.m_nLinkSkill);
							tUIGameInfo2.all_skill_info.SetLinkInfo(gameState.m_nLinkSkillRole, gameState.m_nLinkSkill);
							break;
						}
					}
				}
			}
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneSkill(m_event.GetEventName(), tUIGameInfo2));
		}
		else if (m_event.GetEventName() == "TUIEvent_SkillUnlcok")
		{
			int lparam = m_event.GetLparam();
			bool flag = false;
			iGameData gameData3 = iGameApp.GetInstance().m_GameData;
			if (gameData3 != null)
			{
				CSkillInfo skillInfo = gameData3.GetSkillInfo(lparam);
				iDataCenter dataCenter3 = gameData3.GetDataCenter();
				if (skillInfo != null && dataCenter3 != null)
				{
					int nSkillLevel2 = 0;
					if (!dataCenter3.GetPassiveSkill(skillInfo.nID, ref nSkillLevel2))
					{
						if (skillInfo.isCrystalUnlock)
						{
							if (dataCenter3.Crystal < skillInfo.nUnlockPrice)
							{
								global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneSkill(m_event.GetEventName(), false, BackEventFalseType.NoCrystalEnough, skillInfo.nUnlockPrice - dataCenter3.Crystal));
								return;
							}
							dataCenter3.AddCrystal(-skillInfo.nUnlockPrice);
							dataCenter3.UnlockPassiveSkill(skillInfo.nID);
							dataCenter3.Save();
							flag = true;
							//CFlurryManager.GetInstance().ConsumeCrystal(CFlurryManager.kConsumeType.Skill);
						}
						else
						{
							if (dataCenter3.Gold < skillInfo.nUnlockPrice)
							{
								global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneSkill(m_event.GetEventName(), false, BackEventFalseType.NoGoldEnough, skillInfo.nUnlockPrice - dataCenter3.Gold));
								return;
							}
							dataCenter3.AddGold(-skillInfo.nUnlockPrice);
							dataCenter3.UnlockPassiveSkill(skillInfo.nID);
							dataCenter3.Save();
							flag = true;
							//CFlurryManager.GetInstance().ConsumeGold(CFlurryManager.kConsumeType.Skill);
						}
						if (flag)
						{
							iSkillCenter skillCenter = gameData3.GetSkillCenter();
							if (skillCenter != null)
							{
								Dictionary<int, CSkillInfo> dataSkillInfo = skillCenter.GetDataSkillInfo();
								if (dataSkillInfo != null)
								{
									int[] array3 = new int[6] { 1, 6, 4, 3, 2, 5 };
									TUISkillListInfo[] array4 = new TUISkillListInfo[array3.Length];
									for (int n = 0; n < array4.Length; n++)
									{
										array4[n] = new TUISkillListInfo(array3[n], null);
										List<TUISkillInfo> list2 = new List<TUISkillInfo>();
										CCharacterInfo characterInfo2 = gameData3.GetCharacterInfo(array3[n]);
										if (characterInfo2 == null)
										{
											continue;
										}
										for (int num = 0; num < characterInfo2.ltCharacterPassiveSkill.Count; num++)
										{
											int num2 = characterInfo2.ltCharacterPassiveSkill[num];
											if (!iGameApp.GetInstance().CheckSkillSignState(1, num2))
											{
												if (iGameApp.GetInstance().CheckSkillMaterialEnough(num2))
												{
													array4[n].AddNewMark(num2, NewMarkType.Mark);
												}
												else
												{
													array4[n].AddNewMark(num2, NewMarkType.None);
												}
											}
										}
									}
									TUIGameInfo tUIGameInfo3 = new TUIGameInfo();
									tUIGameInfo3.all_skill_info = new TUIAllSkillInfo(array4);
									global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneSkill("TUIEvent_NewMarkInfo", tUIGameInfo3));
								}
							}
						}
					}
				}
			}
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneSkill(m_event.GetEventName(), flag));
		}
		else if (m_event.GetEventName() == "TUIEvent_SkillBuy")
		{
			int lparam2 = m_event.GetLparam();
			bool flag2 = false;
			iGameData gameData4 = iGameApp.GetInstance().m_GameData;
			if (gameData4 != null)
			{
				CSkillInfo skillInfo2 = gameData4.GetSkillInfo(lparam2);
				iDataCenter dataCenter4 = gameData4.GetDataCenter();
				if (skillInfo2 != null && dataCenter4 != null)
				{
					int nSkillLevel3 = 0;
					if (dataCenter4.GetPassiveSkill(skillInfo2.nID, ref nSkillLevel3) && nSkillLevel3 == -1)
					{
						CSkillInfoLevel cSkillInfoLevel2 = skillInfo2.Get(1);
						if (cSkillInfoLevel2 != null)
						{
							if (cSkillInfoLevel2.isCrystalPurchase)
							{
								if (dataCenter4.Crystal < cSkillInfoLevel2.nPurchasePrice)
								{
									global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneSkill(m_event.GetEventName(), false, BackEventFalseType.NoCrystalEnough, cSkillInfoLevel2.nPurchasePrice - dataCenter4.Crystal));
									return;
								}
								dataCenter4.AddCrystal(-cSkillInfoLevel2.nPurchasePrice);
								dataCenter4.SetPassiveSkill(skillInfo2.nID, 1);
								if (!dataCenter4.HasSelectPassiveSkill(dataCenter4.CurCharID, skillInfo2.nID))
								{
									dataCenter4.SetSkillSign(skillInfo2.nID, 3);
								}
								dataCenter4.Save();
								flag2 = true;
								/*iGameApp.GetInstance().Flurry_PurchaseSkill(skillInfo2.nID);
								CFlurryManager.GetInstance().ConsumeCrystal(CFlurryManager.kConsumeType.Skill);*/
							}
							else
							{
								if (dataCenter4.Gold < cSkillInfoLevel2.nPurchasePrice)
								{
									global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneSkill(m_event.GetEventName(), false, BackEventFalseType.NoGoldEnough, cSkillInfoLevel2.nPurchasePrice - dataCenter4.Gold));
									return;
								}
								dataCenter4.AddGold(-cSkillInfoLevel2.nPurchasePrice);
								dataCenter4.SetPassiveSkill(skillInfo2.nID, 1);
								if (!dataCenter4.HasSelectPassiveSkill(dataCenter4.CurCharID, skillInfo2.nID))
								{
									dataCenter4.SetSkillSign(skillInfo2.nID, 3);
								}
								dataCenter4.Save();
								flag2 = true;
								/*iGameApp.GetInstance().Flurry_PurchaseSkill(skillInfo2.nID);
								CFlurryManager.GetInstance().ConsumeGold(CFlurryManager.kConsumeType.Skill);*/
							}
							if (flag2)
							{
								iSkillCenter skillCenter2 = gameData4.GetSkillCenter();
								if (skillCenter2 != null)
								{
									Dictionary<int, CSkillInfo> dataSkillInfo2 = skillCenter2.GetDataSkillInfo();
									if (dataSkillInfo2 != null)
									{
										int[] array5 = new int[6] { 1, 6, 4, 3, 2, 5 };
										TUISkillListInfo[] array6 = new TUISkillListInfo[array5.Length];
										for (int num3 = 0; num3 < array6.Length; num3++)
										{
											array6[num3] = new TUISkillListInfo(array5[num3], null);
											List<TUISkillInfo> list3 = new List<TUISkillInfo>();
											CCharacterInfo characterInfo3 = gameData4.GetCharacterInfo(array5[num3]);
											if (characterInfo3 == null)
											{
												continue;
											}
											for (int num4 = 0; num4 < characterInfo3.ltCharacterPassiveSkill.Count; num4++)
											{
												int num5 = characterInfo3.ltCharacterPassiveSkill[num4];
												if (!iGameApp.GetInstance().CheckSkillSignState(1, num5))
												{
													if (iGameApp.GetInstance().CheckSkillMaterialEnough(num5))
													{
														array6[num3].AddNewMark(num5, NewMarkType.Mark);
													}
													else
													{
														array6[num3].AddNewMark(num5, NewMarkType.None);
													}
												}
											}
										}
										TUIGameInfo tUIGameInfo4 = new TUIGameInfo();
										tUIGameInfo4.all_skill_info = new TUIAllSkillInfo(array6);
										global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneSkill("TUIEvent_NewMarkInfo", tUIGameInfo4));
									}
								}
								CAchievementManager.GetInstance().AddAchievement(3);
								CAchievementManager.GetInstance().Save();
							}
						}
					}
				}
			}
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneSkill(m_event.GetEventName(), flag2));
		}
		else if (m_event.GetEventName() == "TUIEvent_SkillUpdate")
		{
			int lparam3 = m_event.GetLparam();
			bool flag3 = false;
			iGameData gameData5 = iGameApp.GetInstance().m_GameData;
			if (gameData5 != null)
			{
				CSkillInfo skillInfo3 = gameData5.GetSkillInfo(lparam3);
				iDataCenter dataCenter5 = gameData5.GetDataCenter();
				if (skillInfo3 != null && dataCenter5 != null)
				{
					int nSkillLevel4 = 0;
					if (dataCenter5.GetPassiveSkill(skillInfo3.nID, ref nSkillLevel4) && nSkillLevel4 > 0)
					{
						CSkillInfoLevel skillInfo4 = gameData5.GetSkillInfo(skillInfo3.nID, nSkillLevel4);
						CSkillInfoLevel skillInfo5 = gameData5.GetSkillInfo(skillInfo3.nID, nSkillLevel4 + 1);
						if (skillInfo4 != null && skillInfo5 != null)
						{
							if (skillInfo5.isCrystalPurchase)
							{
								if (dataCenter5.Crystal < skillInfo5.nPurchasePrice)
								{
									global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneSkill(m_event.GetEventName(), false, BackEventFalseType.NoCrystalEnough, skillInfo5.nPurchasePrice - dataCenter5.Crystal));
									return;
								}
								dataCenter5.AddCrystal(-skillInfo5.nPurchasePrice);
								CAchievementManager.GetInstance().AddAchievement(13);
								dataCenter5.SetPassiveSkill(skillInfo3.nID, skillInfo5.nLevel);
								dataCenter5.Save();
								flag3 = true;
								/*iGameApp.GetInstance().Flurry_UpgradeSkill(skillInfo3.nID);
								CFlurryManager.GetInstance().ConsumeCrystal(CFlurryManager.kConsumeType.Skill);*/
								CAchievementManager.GetInstance().AddAchievement(9);
							}
							else
							{
								if (dataCenter5.Gold < skillInfo5.nPurchasePrice)
								{
									Debug.Log(skillInfo4.nPurchasePrice - dataCenter5.Gold);
									global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneSkill(m_event.GetEventName(), false, BackEventFalseType.NoGoldEnough, skillInfo5.nPurchasePrice - dataCenter5.Gold));
									return;
								}
								dataCenter5.AddGold(-skillInfo5.nPurchasePrice);
								dataCenter5.SetPassiveSkill(skillInfo3.nID, skillInfo5.nLevel);
								dataCenter5.Save();
								flag3 = true;
								/*iGameApp.GetInstance().Flurry_UpgradeSkill(skillInfo3.nID);
								CFlurryManager.GetInstance().ConsumeGold(CFlurryManager.kConsumeType.Skill);*/
								CAchievementManager.GetInstance().AddAchievement(9);
							}
							if (flag3)
							{
								iSkillCenter skillCenter3 = gameData5.GetSkillCenter();
								if (skillCenter3 != null)
								{
									Dictionary<int, CSkillInfo> dataSkillInfo3 = skillCenter3.GetDataSkillInfo();
									if (dataSkillInfo3 != null)
									{
										int[] array7 = new int[6] { 1, 6, 4, 3, 2, 5 };
										TUISkillListInfo[] array8 = new TUISkillListInfo[array7.Length];
										for (int num6 = 0; num6 < array8.Length; num6++)
										{
											array8[num6] = new TUISkillListInfo(array7[num6], null);
											List<TUISkillInfo> list4 = new List<TUISkillInfo>();
											CCharacterInfo characterInfo4 = gameData5.GetCharacterInfo(array7[num6]);
											if (characterInfo4 == null)
											{
												continue;
											}
											for (int num7 = 0; num7 < characterInfo4.ltCharacterPassiveSkill.Count; num7++)
											{
												int num8 = characterInfo4.ltCharacterPassiveSkill[num7];
												if (!iGameApp.GetInstance().CheckSkillSignState(1, num8))
												{
													if (iGameApp.GetInstance().CheckSkillMaterialEnough(num8))
													{
														array8[num6].AddNewMark(num8, NewMarkType.Mark);
													}
													else
													{
														array8[num6].AddNewMark(num8, NewMarkType.None);
													}
												}
											}
										}
										TUIGameInfo tUIGameInfo5 = new TUIGameInfo();
										tUIGameInfo5.all_skill_info = new TUIAllSkillInfo(array8);
										global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneSkill("TUIEvent_NewMarkInfo", tUIGameInfo5));
									}
								}
							}
						}
					}
				}
			}
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneSkill(m_event.GetEventName(), flag3));
		}
		else if (m_event.GetEventName() == "TUIEvent_Back")
		{
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneSkill(m_event.GetEventName(), true));
		}
		else if (m_event.GetEventName() == "TUIEvent_SkillChoose")
		{
			int wParam = m_event.GetWParam();
			int lparam4 = m_event.GetLparam();
			iGameData gameData6 = iGameApp.GetInstance().m_GameData;
			if (gameData6 == null)
			{
				return;
			}
			iGameState gameState2 = iGameApp.GetInstance().m_GameState;
			if (gameState2 == null)
			{
				return;
			}
			iDataCenter dataCenter6 = gameData6.GetDataCenter();
			if (dataCenter6 == null)
			{
				return;
			}
			gameState2.m_nLinkSkillRole = wParam;
			gameState2.m_nLinkSkill = lparam4;
			int nSignState = 0;
			if (!dataCenter6.GetSkillSign(lparam4, ref nSignState))
			{
				return;
			}
			if (nSignState == 1)
			{
				Debug.Log("choose " + lparam4);
				dataCenter6.SetSkillSign(lparam4, 2);
				dataCenter6.Save();
				if (iGameApp.GetInstance().CheckSkillMaterialEnough(lparam4))
				{
					TUISkillListInfo[] array9 = new TUISkillListInfo[1]
					{
						new TUISkillListInfo(wParam, null)
					};
					array9[0].AddNewMark(lparam4, NewMarkType.Mark);
					TUIGameInfo tUIGameInfo6 = new TUIGameInfo();
					tUIGameInfo6.all_skill_info = new TUIAllSkillInfo(array9);
					global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneSkill("TUIEvent_NewMarkInfo", tUIGameInfo6));
				}
			}
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneSkill(m_event.GetEventName()));
		}
		else if (m_event.GetEventName() == "TUIEvent_GoldToCrystal")
		{
			iGameData gameData7 = iGameApp.GetInstance().m_GameData;
			if (gameData7 == null)
			{
				return;
			}
			iDataCenter dataCenter7 = gameData7.GetDataCenter();
			if (dataCenter7 != null)
			{
				int wParam2 = m_event.GetWParam();
				int num9 = MyUtils.Formula_Gold2Crystal(wParam2);
				if (dataCenter7.Crystal < num9)
				{
					global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneSkill(m_event.GetEventName(), false, BackEventFalseType.NoCrystalEnough, num9 - dataCenter7.Crystal));
					return;
				}
				dataCenter7.AddCrystal(-num9);
				dataCenter7.AddGold(wParam2);
				dataCenter7.Save();
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneSkill(m_event.GetEventName(), true));
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_EnterIAP")
		{
			iGameState gameState3 = iGameApp.GetInstance().m_GameState;
			if (gameState3 != null)
			{
				gameState3.m_lstScene4IAP = TUISceneType.Scene_Skill;
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneSkill(m_event.GetEventName(), true));
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_EnterGold")
		{
			iGameState gameState4 = iGameApp.GetInstance().m_GameState;
			if (gameState4 != null)
			{
				gameState4.m_lstScene4IAP = TUISceneType.Scene_Skill;
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneSkill(m_event.GetEventName(), true));
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_EnterIAPCrystalNoEnough")
		{
			iGameState gameState5 = iGameApp.GetInstance().m_GameState;
			if (gameState5 != null)
			{
				gameState5.m_lstScene4IAP = TUISceneType.Scene_Skill;
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneSkill(m_event.GetEventName(), true));
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_EnterGoEquip")
		{
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneSkill(m_event.GetEventName(), true));
		}
	}

	private void TUIEvent_BackInfo_SceneForge(object sender, TUIEvent.SendEvent_SceneForge m_event)
	{
		if (m_event.GetEventName() == "TUIEvent_TopBar")
		{
			iGameData gameData = iGameApp.GetInstance().m_GameData;
			if (gameData == null)
			{
				return;
			}
			iDataCenter dataCenter = gameData.GetDataCenter();
			if (dataCenter == null)
			{
				return;
			}
			CCharSaveInfo character = dataCenter.GetCharacter(dataCenter.CurCharID);
			if (character == null)
			{
				return;
			}
			CCharacterInfoLevel characterInfo = gameData.GetCharacterInfo(character.nID, character.nLevel);
			if (characterInfo != null)
			{
				TUIGameInfo tUIGameInfo = new TUIGameInfo();
				tUIGameInfo.player_info = new TUIPlayerInfo();
				tUIGameInfo.player_info.avatar_id = character.nID;
				tUIGameInfo.player_info.level = character.nLevel;
				tUIGameInfo.player_info.level_exp = characterInfo.nExp;
				tUIGameInfo.player_info.exp = character.nExp;
				tUIGameInfo.player_info.gold = dataCenter.Gold;
				tUIGameInfo.player_info.crystal = dataCenter.Crystal;
				iGameState gameState = iGameApp.GetInstance().m_GameState;
				if (gameState != null)
				{
					gameState.m_curScene4Recommand = gameState.m_lstScene4Recommand;
					gameState.m_lstScene4Recommand = TUISceneType.None;
				}
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneForge(m_event.GetEventName(), tUIGameInfo));
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_WeaponInfo")
		{
			iGameData gameData2 = iGameApp.GetInstance().m_GameData;
			if (gameData2 == null)
			{
				return;
			}
			iDataCenter dataCenter2 = gameData2.GetDataCenter();
			if (dataCenter2 == null)
			{
				return;
			}
			iItemCenter itemCenter = gameData2.GetItemCenter();
			if (itemCenter == null)
			{
				return;
			}
			Dictionary<int, CItemInfo> data = itemCenter.GetData();
			if (data == null)
			{
				return;
			}
			iWeaponCenter weaponCenter = gameData2.GetWeaponCenter();
			if (weaponCenter == null)
			{
				return;
			}
			Dictionary<int, CWeaponInfo> data2 = weaponCenter.GetData();
			if (data2 == null)
			{
				return;
			}
			Dictionary<int, TUIGoodsInfo> dictionary = new Dictionary<int, TUIGoodsInfo>();
			if (data != null)
			{
				CItemInfoLevel cItemInfoLevel = null;
				foreach (CItemInfo value in data.Values)
				{
					cItemInfoLevel = gameData2.GetItemInfo(value.nID, 1);
					if (cItemInfoLevel != null)
					{
						int num = dataCenter2.GetMaterialNum(value.nID);
						if (num == -1)
						{
							num = 0;
						}
						GoodsQualityType quality = GoodsQualityType.Quality01;
						switch (cItemInfoLevel.nRare)
						{
						case 1:
							quality = GoodsQualityType.Quality01;
							break;
						case 2:
							quality = GoodsQualityType.Quality02;
							break;
						case 3:
							quality = GoodsQualityType.Quality03;
							break;
						case 4:
							quality = GoodsQualityType.Quality04;
							break;
						case 5:
							quality = GoodsQualityType.Quality05;
							break;
						case 6:
							quality = GoodsQualityType.Quality06;
							break;
						}
						dictionary.Add(value.nID, new TUIGoodsInfo(value.nID, quality, cItemInfoLevel.sName, num, new TUIPriceInfo(cItemInfoLevel.nPurchasePrice, cItemInfoLevel.isCrystalPurchase ? UnitType.Crystal : UnitType.Gold)));
					}
				}
			}
			TUIGameInfo tUIGameInfo2 = new TUIGameInfo();
			tUIGameInfo2.weapon_info = new TUIWeaponInfo();
			// Add weapons to shop
			int[] array = new int[22]
			{
				1, 21, 9, 10, 4, 16, 15, 18, 3, 11,
				12, 17, 5, 13, 19, 2, 6, 7, 8, 14, 22,
				23
			};
			CWeaponInfoLevel[] array2 = new CWeaponInfoLevel[5];
			int[] array3 = array;
			foreach (int nID in array3)
			{
				CWeaponInfo weaponInfo = gameData2.GetWeaponInfo(nID);
				if (weaponInfo == null)
				{
					return;
				}
				bool flag = false;
				for (int j = 0; j < array2.Length; j++)
				{
					array2[j] = weaponInfo.Get(j + 1);
					if (array2[j] == null)
					{
						Debug.LogWarning("id " + weaponInfo.nID + " lvl " + (j + 1) + " does not exist!");
						flag = true;
					}
				}
				if (flag)
				{
					continue;
				}
				Dictionary<int, TUIPriceInfo> dictionary2 = new Dictionary<int, TUIPriceInfo>();
				for (int k = 0; k < array2.Length; k++)
				{
					dictionary2.Add(k + 1, new TUIPriceInfo(array2[k].nPurchasePrice, array2[k].isCrystalPurchase ? UnitType.Crystal : UnitType.Gold));
				}
				Dictionary<int, int> dictionary3 = new Dictionary<int, int>();
				for (int l = 0; l < array2.Length; l++)
				{
					dictionary3.Add(l + 1, (int)array2[l].fDamage);
				}
				Dictionary<int, float> dictionary4 = new Dictionary<int, float>();
				for (int m = 0; m < array2.Length; m++)
				{
					dictionary4.Add(m + 1, array2[m].fShootSpeed);
				}
				Dictionary<int, int> dictionary5 = new Dictionary<int, int>();
				for (int n = 0; n < array2.Length; n++)
				{
					if (array2[n] != null && array2[n].nType == 5)
					{
						dictionary5.Add(n + 1, 20);
					}
				}
				Dictionary<int, int> dictionary6 = new Dictionary<int, int>();
				for (int num2 = 0; num2 < array2.Length; num2++)
				{
					for (int num3 = 0; num3 < 3; num3++)
					{
						if (array2[num2].arrFunc[num3] == 4)
						{
							dictionary6.Add(num2 + 1, array2[num2].arrValueY[num3]);
							break;
						}
					}
				}
				Dictionary<int, int> dictionary7 = new Dictionary<int, int>();
				for (int num4 = 0; num4 < array2.Length; num4++)
				{
					dictionary7.Add(num4 + 1, array2[num4].nCapacity);
				}
				Dictionary<int, string> dictionary8 = new Dictionary<int, string>();
				dictionary8.Add(1, "DMG: " + (int)array2[0].fDamage);
				for (int num5 = 1; num5 < array2.Length; num5++)
				{
					int level   = num5 + 1;
					int curDmg  = (int)array2[num5 - 1].fDamage;
					int nextDmg = (int)array2[num5].fDamage;
					int delta   = nextDmg - curDmg;
					if (num5 + 1 < array2.Length && array2[num5 + 1] != null)
					{
						dictionary8.Add(level, "Next Upgrade:\n" + nextDmg
						                                         + "({color:1eff0000}+" + delta + "{color})damage");
					}
					else
					{
						dictionary8.Add(level, "Next Upgrade:\n" + nextDmg
						                                         + "({color:1eff0000}+" + delta + "{color})damage"
						                                         + "\nThis is the final level.");
					}
				}
				TUIWeaponUpdateInfo weapon_update_info = new TUIWeaponUpdateInfo(dictionary2, dictionary3, dictionary4, dictionary5, dictionary6, dictionary7, dictionary8);
				List<TUIGoodsNeedInfo>[] array4 = new List<TUIGoodsNeedInfo>[5];
				for (int num6 = 0; num6 < array4.Length; num6++)
				{
					array4[num6] = new List<TUIGoodsNeedInfo>();
					for (int num7 = 0; num7 < array2[num6].ltMaterials.Count && num7 < array2[num6].ltMaterialsCount.Count; num7++)
					{
						CItemInfoLevel itemInfo = gameData2.GetItemInfo(array2[num6].ltMaterials[num7], 1);
						if (itemInfo != null)
						{
							GoodsQualityType goods_quality = GoodsQualityType.Quality01;
							switch (itemInfo.nRare)
							{
							case 1:
								goods_quality = GoodsQualityType.Quality01;
								break;
							case 2:
								goods_quality = GoodsQualityType.Quality02;
								break;
							case 3:
								goods_quality = GoodsQualityType.Quality03;
								break;
							case 4:
								goods_quality = GoodsQualityType.Quality04;
								break;
							case 5:
								goods_quality = GoodsQualityType.Quality05;
								break;
							case 6:
								goods_quality = GoodsQualityType.Quality06;
								break;
							}
							array4[num6].Add(new TUIGoodsNeedInfo(array2[num6].ltMaterials[num7], goods_quality, array2[num6].ltMaterialsCount[num7], itemInfo.sName));
						}
					}
				}
				TUILevelGoodsNeedInfo level_goods_need_info = new TUILevelGoodsNeedInfo(array4[0], array4[1], array4[2], array4[3], array4[4]);
				WeaponType type = WeaponType.CloseWeapon;
				switch (array2[0].nType)
				{
				case 1:
					type = WeaponType.CloseWeapon;
					break;
				case 2:
					type = WeaponType.MachineGun;
					break;
				case 0:
					type = WeaponType.Crossbow;
					break;
				case 4:
					type = WeaponType.LiquidFireGun;
					break;
				case 5:
					type = WeaponType.RPG;
					break;
				case 3:
					type = WeaponType.ViolenceGun;
					break;
				}
				int num8 = dataCenter2.GetWeaponLevel(weaponInfo.nID);
				if (num8 == -1)
				{
					num8 = 0;
				}
				tUIGameInfo2.weapon_info.AddItem(new TUIWeaponAttributeInfo(type, weaponInfo.nID, array2[0].sName, num8, weapon_update_info, level_goods_need_info, dictionary));
				iGameState gameState2 = iGameApp.GetInstance().m_GameState;
				if (gameState2 != null && gameState2.m_nLinkWeapon == weaponInfo.nID)
				{
					Debug.Log(gameState2.m_nLinkWeapon);
					tUIGameInfo2.weapon_info.SetLinkInfo(type, gameState2.m_nLinkWeapon);
				}
				if (iGameApp.GetInstance().CheckWeaponSignState(1, weaponInfo.nID))
				{
					tUIGameInfo2.weapon_info.AddNewMark(weaponInfo.nID, NewMarkType.New);
				}
				else if (iGameApp.GetInstance().CheckWeaponMaterialEnough(weaponInfo.nID))
				{
					tUIGameInfo2.weapon_info.AddNewMark(weaponInfo.nID, NewMarkType.Mark);
				}
				else
				{
					tUIGameInfo2.weapon_info.AddNewMark(weaponInfo.nID, NewMarkType.None);
				}
			}
			if (data != null)
			{
				CItemInfoLevel[] array5 = new CItemInfoLevel[5];
				foreach (CItemInfo value2 in data.Values)
				{
					bool flag2 = false;
					for (int num9 = 0; num9 < array5.Length; num9++)
					{
						array5[num9] = value2.Get(num9 + 1);
						if (array5[num9] == null)
						{
							flag2 = true;
						}
						else if (array5[num9].nType != 1)
						{
							flag2 = true;
						}
					}
					if (flag2)
					{
						continue;
					}
					Dictionary<int, int> dictionary9 = new Dictionary<int, int>();
					for (int num10 = 0; num10 < array5.Length; num10++)
					{
						for (int num11 = 0; num11 < 3; num11++)
						{
							if (array5[num10].arrFunc[num11] == 1)
							{
								kProEnum kProEnum2 = (kProEnum)MyUtils.Low32(array5[num10].arrValueX[num11]);
								if (kProEnum2 == kProEnum.HPMax)
								{
									dictionary9.Add(num10 + 1, MyUtils.Low32(array5[num10].arrValueY[num11]));
									break;
								}
							}
						}
					}
					Dictionary<int, TUIPriceInfo> dictionary10 = new Dictionary<int, TUIPriceInfo>();
					for (int num12 = 0; num12 < array5.Length; num12++)
					{
						dictionary10.Add(num12 + 1, new TUIPriceInfo(array5[num12].nPurchasePrice, array5[num12].isCrystalPurchase ? UnitType.Crystal : UnitType.Gold));
					}
					Dictionary<int, string> dictionary11 = new Dictionary<int, string>();
					Dictionary<int, string> dictionary12 = new Dictionary<int, string>();
					for (int num13 = 0; num13 < array5.Length; num13++)
					{
						int hp = 0;
						for (int fi = 0; fi < 3; fi++)
						{
							if (array5[num13].arrFunc[fi] == 1)
							{
								kProEnum kp = (kProEnum)MyUtils.Low32(array5[num13].arrValueX[fi]);
								if (kp == kProEnum.HPMax)
								{
									hp = MyUtils.Low32(array5[num13].arrValueY[fi]);
									break;
								}
							}
						}

						string levelDesc;
						if (num13 == 0)
						{
							levelDesc = "HP: {color:1eff0000}" + hp + "{color}";
						}
						else
						{
							int prevHp = 0;
							for (int fi = 0; fi < 3; fi++)
							{
								if (array5[num13 - 1].arrFunc[fi] == 1)
								{
									kProEnum kp = (kProEnum)MyUtils.Low32(array5[num13 - 1].arrValueX[fi]);
									if (kp == kProEnum.HPMax)
									{
										prevHp = MyUtils.Low32(array5[num13 - 1].arrValueY[fi]);
										break;
									}
								}
							}
							int delta = hp - prevHp;
							levelDesc = "Next Upgrade:\nHP: " + hp + "({color:1eff0000}+" + delta + "{color})";
							if (num13 + 1 >= array5.Length)
							{
								levelDesc += "\nThis stone cannot be upgraded further after this.";
							}
						}

						dictionary11.Add(num13 + 1, levelDesc);
						dictionary12.Add(num13 + 1, array5[num13].sDesc);
					}
					List<TUIGoodsNeedInfo>[] array6 = new List<TUIGoodsNeedInfo>[5];
					for (int num14 = 0; num14 < array6.Length; num14++)
					{
						array6[num14] = new List<TUIGoodsNeedInfo>();
						for (int num15 = 0; num15 < array5[num14].ltMaterials.Count && num15 < array5[num14].ltMaterialsCount.Count; num15++)
						{
							CItemInfoLevel itemInfo2 = gameData2.GetItemInfo(array5[num14].ltMaterials[num15], 1);
							if (itemInfo2 != null)
							{
								GoodsQualityType goods_quality2 = GoodsQualityType.Quality01;
								switch (itemInfo2.nRare)
								{
								case 1:
									goods_quality2 = GoodsQualityType.Quality01;
									break;
								case 2:
									goods_quality2 = GoodsQualityType.Quality02;
									break;
								case 3:
									goods_quality2 = GoodsQualityType.Quality03;
									break;
								case 4:
									goods_quality2 = GoodsQualityType.Quality04;
									break;
								case 5:
									goods_quality2 = GoodsQualityType.Quality05;
									break;
								case 6:
									goods_quality2 = GoodsQualityType.Quality06;
									break;
								}
								array6[num14].Add(new TUIGoodsNeedInfo(array5[num14].ltMaterials[num15], goods_quality2, array5[num14].ltMaterialsCount[num15], itemInfo2.sName));
							}
						}
					}
					TUILevelGoodsNeedInfo level_goods_need_info2 = new TUILevelGoodsNeedInfo(array6[0], array6[1], array6[2], array6[3], array6[4]);
					TUIWeaponUpdateInfo weapon_update_info2 = new TUIWeaponUpdateInfo(dictionary10, dictionary11, dictionary9, dictionary12);
					int nItemLevel = 0;
					dataCenter2.GetEquipStone(value2.nID, ref nItemLevel);
					tUIGameInfo2.weapon_info.AddItem(new TUIWeaponAttributeInfo(WeaponType.Stoneskin, value2.nID, array5[0].sName, nItemLevel, weapon_update_info2, level_goods_need_info2, dictionary));
					iGameState gameState3 = iGameApp.GetInstance().m_GameState;
					if (gameState3 != null && gameState3.m_nLinkWeapon > 0 && gameData2.GetItemInfo(gameState3.m_nLinkWeapon) != null)
					{
						tUIGameInfo2.weapon_info.SetLinkInfo(WeaponType.Stoneskin, gameState3.m_nLinkWeapon);
					}
					if (iGameApp.GetInstance().CheckEquipStoneSignState(1, value2.nID))
					{
						tUIGameInfo2.weapon_info.AddNewMark(value2.nID, NewMarkType.New);
					}
					else if (iGameApp.GetInstance().CheckEquipStoneMaterialEnough(value2.nID))
					{
						tUIGameInfo2.weapon_info.AddNewMark(value2.nID, NewMarkType.Mark);
					}
					else
					{
						tUIGameInfo2.weapon_info.AddNewMark(value2.nID, NewMarkType.None);
					}
				}
			}
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneForge(m_event.GetEventName(), tUIGameInfo2));
		}
		else if (m_event.GetEventName() == "TUIEvent_WeaponGoodsBuy")
		{
			bool flag3 = false;
			iGameData gameData3 = iGameApp.GetInstance().m_GameData;
			if (gameData3 != null)
			{
				iDataCenter dataCenter3 = gameData3.GetDataCenter();
				if (dataCenter3 != null)
				{
					int wParam = m_event.GetWParam();
					int rparam = m_event.GetRparam();
					int lparam = m_event.GetLparam();
					Debug.Log(rparam);
					CItemInfoLevel itemInfo3 = gameData3.GetItemInfo(wParam, 1);
					if (itemInfo3 != null && itemInfo3.nType == 3)
					{
						if (itemInfo3.isCrystalPurchase)
						{
							int num16 = itemInfo3.nPurchasePrice * rparam;
							if (dataCenter3.Crystal < num16)
							{
								global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneForge(m_event.GetEventName(), false, BackEventFalseType.NoCrystalEnough, num16 - dataCenter3.Crystal));
								return;
							}
							dataCenter3.AddCrystal(-num16);
							dataCenter3.AddMaterialNum(wParam, rparam);
							dataCenter3.Save();
							flag3 = true;
							//CFlurryManager.GetInstance().ConsumeCrystal(CFlurryManager.kConsumeType.Material);
						}
						else
						{
							int num17 = itemInfo3.nPurchasePrice * rparam;
							if (dataCenter3.Gold < num17)
							{
								global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneForge(m_event.GetEventName(), false, BackEventFalseType.NoGoldEnough, num17 - dataCenter3.Gold));
								return;
							}
							dataCenter3.AddGold(-num17);
							dataCenter3.AddMaterialNum(wParam, rparam);
							dataCenter3.Save();
							flag3 = true;
							//CFlurryManager.GetInstance().ConsumeGold(CFlurryManager.kConsumeType.Material);
						}
						if (flag3)
						{
							TUIGameInfo tUIGameInfo3 = new TUIGameInfo();
							tUIGameInfo3.weapon_info = new TUIWeaponInfo();
							iWeaponCenter weaponCenter2 = gameData3.GetWeaponCenter();
							if (weaponCenter2 != null)
							{
								Dictionary<int, CWeaponInfo> data3 = weaponCenter2.GetData();
								if (data3 != null)
								{
									foreach (CWeaponInfo value3 in data3.Values)
									{
										if (!iGameApp.GetInstance().CheckWeaponSignState(1, value3.nID))
										{
											if (iGameApp.GetInstance().CheckWeaponMaterialEnough(value3.nID))
											{
												tUIGameInfo3.weapon_info.AddNewMark(value3.nID, NewMarkType.Mark);
											}
											else
											{
												tUIGameInfo3.weapon_info.AddNewMark(value3.nID, NewMarkType.None);
											}
										}
									}
								}
							}
							iItemCenter itemCenter2 = gameData3.GetItemCenter();
							if (itemCenter2 != null)
							{
								Dictionary<int, CItemInfo> data4 = itemCenter2.GetData();
								if (data4 != null)
								{
									foreach (CItemInfo value4 in data4.Values)
									{
										CItemInfoLevel cItemInfoLevel2 = value4.Get(1);
										if (cItemInfoLevel2 != null && cItemInfoLevel2.nType == 1 && !iGameApp.GetInstance().CheckEquipStoneSignState(1, value4.nID))
										{
											if (iGameApp.GetInstance().CheckEquipStoneMaterialEnough(value4.nID))
											{
												tUIGameInfo3.weapon_info.AddNewMark(value4.nID, NewMarkType.Mark);
											}
											else
											{
												tUIGameInfo3.weapon_info.AddNewMark(value4.nID, NewMarkType.None);
											}
										}
									}
								}
							}
							global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneForge("TUIEvent_NewMarkInfo", tUIGameInfo3));
						}
					}
				}
			}
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneForge(m_event.GetEventName(), flag3));
		}
		else if (m_event.GetEventName() == "TUIEvent_WeaponUpdate")
		{
			bool success = false;
			int wparam = 0;
			iGameData gameData4 = iGameApp.GetInstance().m_GameData;
			if (gameData4 != null)
			{
				iDataCenter dataCenter4 = gameData4.GetDataCenter();
				if (dataCenter4 != null)
				{
					int wParam2 = m_event.GetWParam();
					CLevelUpWeapon cLevelUpWeapon = new CLevelUpWeapon();
					if (cLevelUpWeapon != null && cLevelUpWeapon.Initialize(wParam2))
					{
						if (!cLevelUpWeapon.LevelUp())
						{
							global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneForge(m_event.GetEventName(), false, (!cLevelUpWeapon.isCrystalTrade) ? BackEventFalseType.NoGoldEnough : BackEventFalseType.NoCrystalEnough, cLevelUpWeapon.nNeedValue));
							return;
						}
						wparam = ((!cLevelUpWeapon.isCrystalTrade) ? 1 : 2);
						if (!dataCenter4.HasSelectWeapon(wParam2))
						{
							dataCenter4.SetWeaponSign(wParam2, 3);
						}
						success = true;
						if (cLevelUpWeapon.AfterLevel == 1)
						{
							//iGameApp.GetInstance().Flurry_PurchaseWeapon(wParam2);
							CAchievementManager.GetInstance().AddAchievement(4);
							CAchievementManager.GetInstance().Save();
						}
						else
						{
							//iGameApp.GetInstance().Flurry_UpgradeWeapon(wParam2);
							CAchievementManager.GetInstance().AddAchievement(10);
						}
						if (cLevelUpWeapon.isCrystalTrade)
						{
							//CFlurryManager.GetInstance().ConsumeCrystal(CFlurryManager.kConsumeType.Weapon);
							CAchievementManager.GetInstance().AddAchievement(13);
						}
						else
						{
							//CFlurryManager.GetInstance().ConsumeGold(CFlurryManager.kConsumeType.Weapon);
						}
						iWeaponCenter weaponCenter3 = gameData4.GetWeaponCenter();
						if (weaponCenter3 != null)
						{
							Dictionary<int, CWeaponInfo> data5 = weaponCenter3.GetData();
							if (data5 != null)
							{
								TUIGameInfo tUIGameInfo4 = new TUIGameInfo();
								tUIGameInfo4.weapon_info = new TUIWeaponInfo();
								foreach (CWeaponInfo value5 in data5.Values)
								{
									if (!iGameApp.GetInstance().CheckWeaponSignState(1, value5.nID))
									{
										if (iGameApp.GetInstance().CheckWeaponMaterialEnough(value5.nID))
										{
											tUIGameInfo4.weapon_info.AddNewMark(value5.nID, NewMarkType.Mark);
										}
										else
										{
											tUIGameInfo4.weapon_info.AddNewMark(value5.nID, NewMarkType.None);
										}
									}
								}
								global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneForge("TUIEvent_NewMarkInfo", tUIGameInfo4));
							}
						}
					}
					CLevelUpEquip cLevelUpEquip = new CLevelUpEquip();
					if (cLevelUpEquip != null && cLevelUpEquip.Initialize(wParam2))
					{
						if (!cLevelUpEquip.LevelUp())
						{
							global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneForge(m_event.GetEventName(), false, (!cLevelUpEquip.isCrystalTrade) ? BackEventFalseType.NoGoldEnough : BackEventFalseType.NoCrystalEnough, cLevelUpEquip.nNeedValue));
							return;
						}
						wparam = ((!cLevelUpEquip.isCrystalTrade) ? 1 : 2);
						if (dataCenter4.CurEquipStone != wParam2)
						{
							dataCenter4.SetWeaponSign(wParam2, 3);
						}
						success = true;
						if (cLevelUpEquip.AfterLevel == 1)
						{
							//iGameApp.GetInstance().Flurry_PurchaseStone(wParam2);
						}
						else
						{
							//iGameApp.GetInstance().Flurry_UpgradeStone(wParam2);
						}
						if (cLevelUpEquip.isCrystalTrade)
						{
							//CFlurryManager.GetInstance().ConsumeCrystal(CFlurryManager.kConsumeType.Stone);
						}
						else
						{
							//CFlurryManager.GetInstance().ConsumeGold(CFlurryManager.kConsumeType.Stone);
						}
						iItemCenter itemCenter3 = gameData4.GetItemCenter();
						if (itemCenter3 != null)
						{
							Dictionary<int, CItemInfo> data6 = itemCenter3.GetData();
							if (data6 != null)
							{
								TUIGameInfo tUIGameInfo5 = new TUIGameInfo();
								tUIGameInfo5.weapon_info = new TUIWeaponInfo();
								foreach (CItemInfo value6 in data6.Values)
								{
									CItemInfoLevel cItemInfoLevel3 = value6.Get(1);
									if (cItemInfoLevel3 != null && cItemInfoLevel3.nType == 1 && !iGameApp.GetInstance().CheckEquipStoneSignState(1, value6.nID))
									{
										if (iGameApp.GetInstance().CheckEquipStoneMaterialEnough(value6.nID))
										{
											tUIGameInfo5.weapon_info.AddNewMark(value6.nID, NewMarkType.Mark);
										}
										else
										{
											tUIGameInfo5.weapon_info.AddNewMark(value6.nID, NewMarkType.None);
										}
									}
								}
								global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneForge("TUIEvent_NewMarkInfo", tUIGameInfo5));
							}
						}
					}
				}
			}
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneForge(m_event.GetEventName(), success, wparam));
		}
		else if (m_event.GetEventName() == "TUIEvent_Back")
		{
			iGameState gameState4 = iGameApp.GetInstance().m_GameState;
			if (gameState4 != null)
			{
				if (gameState4.m_curScene4Recommand != 0)
				{
					global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneForge(m_event.GetEventName(), true, (int)gameState4.m_curScene4Recommand));
					gameState4.m_curScene4Recommand = TUISceneType.None;
				}
				else
				{
					global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneForge(m_event.GetEventName(), true));
				}
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_SearchGoodsDrop")
		{
			int wParam3 = m_event.GetWParam();
			int lparam2 = m_event.GetLparam();
			iGameState gameState5 = iGameApp.GetInstance().m_GameState;
			if (gameState5 != null)
			{
				gameState5.m_nMaterialIDFromEquip = wParam3;
				gameState5.m_lstScene4SearchMaterial = TUISceneType.Scene_Forge;
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneForge(m_event.GetEventName(), true));
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_WeaponChoose")
		{
			int wParam4 = m_event.GetWParam();
			iGameData gameData5 = iGameApp.GetInstance().m_GameData;
			if (gameData5 == null)
			{
				return;
			}
			iGameState gameState6 = iGameApp.GetInstance().m_GameState;
			if (gameState6 == null)
			{
				return;
			}
			iDataCenter dataCenter5 = gameData5.GetDataCenter();
			if (dataCenter5 == null)
			{
				return;
			}
			int nSignState = 0;
			gameState6.m_nLinkWeapon = wParam4;
			if (dataCenter5.GetWeaponSign(wParam4, ref nSignState) && nSignState == 1)
			{
				dataCenter5.SetWeaponSign(wParam4, 2);
				dataCenter5.Save();
				if (iGameApp.GetInstance().CheckWeaponMaterialEnough(wParam4))
				{
					TUIGameInfo tUIGameInfo6 = new TUIGameInfo();
					tUIGameInfo6.weapon_info = new TUIWeaponInfo();
					tUIGameInfo6.weapon_info.AddNewMark(wParam4, NewMarkType.Mark);
					global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneForge("TUIEvent_NewMarkInfo", tUIGameInfo6));
				}
			}
			if (dataCenter5.GetEquipStoneSign(wParam4, ref nSignState) && nSignState == 1)
			{
				dataCenter5.SetEquipStoneSign(wParam4, 2);
				dataCenter5.Save();
				if (iGameApp.GetInstance().CheckEquipStoneMaterialEnough(wParam4))
				{
					TUIGameInfo tUIGameInfo7 = new TUIGameInfo();
					tUIGameInfo7.weapon_info = new TUIWeaponInfo();
					tUIGameInfo7.weapon_info.AddNewMark(wParam4, NewMarkType.Mark);
					global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneForge("TUIEvent_NewMarkInfo", tUIGameInfo7));
				}
			}
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneForge(m_event.GetEventName()));
		}
		else if (m_event.GetEventName() == "TUIEvent_WeaponSuppplementPrice")
		{
			iGameData gameData6 = iGameApp.GetInstance().m_GameData;
			if (gameData6 == null)
			{
				return;
			}
			iDataCenter dataCenter6 = gameData6.GetDataCenter();
			if (dataCenter6 == null)
			{
				return;
			}
			int num18 = 0;
			int num19 = 0;
			int num20 = 0;
			TUISupplementInfo supplementInfo = m_event.GetSupplementInfo();
			if (supplementInfo.goods_list != null)
			{
				foreach (TUIGoodsSupplementInfo item in supplementInfo.goods_list)
				{
					CItemInfoLevel itemInfo4 = gameData6.GetItemInfo(item.id, 1);
					if (itemInfo4 != null && itemInfo4.isCrystalPurchase)
					{
						num18 += itemInfo4.nPurchasePrice * item.count;
					}
				}
			}
			Debug.Log("m_supplement.price_value = " + supplementInfo.price_value);
			if (supplementInfo.price_unit == UnitType.Gold)
			{
				if (supplementInfo.price_value > 0)
				{
					num19 = MyUtils.Formula_Gold2Crystal(supplementInfo.price_value);
				}
			}
			else if (supplementInfo.price_unit == UnitType.Crystal)
			{
				num20 = supplementInfo.price_value;
			}
			TUIPriceInfo supplement_info = new TUIPriceInfo(num18 + num19 + num20, UnitType.Crystal);
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneForge(m_event.GetEventName(), supplement_info));
		}
		else if (m_event.GetEventName() == "TUIEvent_WeaponSuppplement")
		{
			iGameData gameData7 = iGameApp.GetInstance().m_GameData;
			if (gameData7 == null)
			{
				return;
			}
			iDataCenter dataCenter7 = gameData7.GetDataCenter();
			if (dataCenter7 == null)
			{
				return;
			}
			int num21 = 0;
			int num22 = 0;
			int num23 = 0;
			TUISupplementInfo supplementInfo2 = m_event.GetSupplementInfo();
			if (supplementInfo2.goods_list != null)
			{
				foreach (TUIGoodsSupplementInfo item2 in supplementInfo2.goods_list)
				{
					CItemInfoLevel itemInfo5 = gameData7.GetItemInfo(item2.id, 1);
					if (itemInfo5 != null && itemInfo5.isCrystalPurchase)
					{
						num21 += itemInfo5.nPurchasePrice * item2.count;
					}
				}
			}
			Debug.Log("m_supplement.price_value = " + supplementInfo2.price_value);
			if (supplementInfo2.price_unit == UnitType.Gold)
			{
				if (supplementInfo2.price_value > 0)
				{
					num22 = MyUtils.Formula_Gold2Crystal(supplementInfo2.price_value);
				}
			}
			else if (supplementInfo2.price_unit == UnitType.Crystal)
			{
				num23 = supplementInfo2.price_value + dataCenter7.Crystal;
			}
			int num24 = num21 + num22 + num23;
			Debug.Log("total = " + num24 + " cur = " + dataCenter7.Crystal);
			if (dataCenter7.Crystal >= num24)
			{
				if (supplementInfo2.goods_list != null)
				{
					foreach (TUIGoodsSupplementInfo item3 in supplementInfo2.goods_list)
					{
						CItemInfoLevel itemInfo6 = gameData7.GetItemInfo(item3.id, 1);
						if (itemInfo6 != null && itemInfo6.isCrystalPurchase)
						{
							dataCenter7.AddMaterialNum(item3.id, item3.count);
						}
					}
					dataCenter7.AddCrystal(-num21);
				}
				if (supplementInfo2.price_unit == UnitType.Gold)
				{
					dataCenter7.AddCrystal(-num22);
					dataCenter7.AddGold(supplementInfo2.price_value);
				}
				dataCenter7.Save();
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneForge(m_event.GetEventName(), true));
			}
			else
			{
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneForge(m_event.GetEventName(), false, BackEventFalseType.NoCrystalEnough, num24 - dataCenter7.Crystal));
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_GoldToCrystal")
		{
			iGameData gameData8 = iGameApp.GetInstance().m_GameData;
			if (gameData8 == null)
			{
				return;
			}
			iDataCenter dataCenter8 = gameData8.GetDataCenter();
			if (dataCenter8 != null)
			{
				int wParam5 = m_event.GetWParam();
				int num25 = MyUtils.Formula_Gold2Crystal(wParam5);
				if (dataCenter8.Crystal < num25)
				{
					global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneForge(m_event.GetEventName(), false, BackEventFalseType.NoCrystalEnough, num25 - dataCenter8.Crystal));
					return;
				}
				dataCenter8.AddCrystal(-num25);
				dataCenter8.AddGold(wParam5);
				dataCenter8.Save();
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneForge(m_event.GetEventName(), true));
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_EnterIAP")
		{
			iGameState gameState7 = iGameApp.GetInstance().m_GameState;
			if (gameState7 != null)
			{
				gameState7.m_lstScene4IAP = TUISceneType.Scene_Forge;
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneForge(m_event.GetEventName(), true));
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_EnterGold")
		{
			iGameState gameState8 = iGameApp.GetInstance().m_GameState;
			if (gameState8 != null)
			{
				gameState8.m_lstScene4IAP = TUISceneType.Scene_Forge;
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneForge(m_event.GetEventName(), true));
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_EnterIAPCrystalNoEnough")
		{
			iGameState gameState9 = iGameApp.GetInstance().m_GameState;
			if (gameState9 != null)
			{
				gameState9.m_lstScene4IAP = TUISceneType.Scene_Forge;
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneForge(m_event.GetEventName(), true));
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_EnterGoEquip")
		{
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneForge(m_event.GetEventName(), true));
		}
	}

	private void TUIEvent_BackInfo_SceneTavern(object sender, TUIEvent.SendEvent_SceneTavern m_event)
	{
		if (m_event.GetEventName() == "TUIEvent_TopBar")
		{
			iGameData gameData = iGameApp.GetInstance().m_GameData;
			if (gameData == null)
			{
				return;
			}
			iDataCenter dataCenter = gameData.GetDataCenter();
			if (dataCenter == null)
			{
				return;
			}
			CCharSaveInfo character = dataCenter.GetCharacter(dataCenter.CurCharID);
			if (character != null)
			{
				CCharacterInfoLevel characterInfo = gameData.GetCharacterInfo(character.nID, character.nLevel);
				if (characterInfo != null)
				{
					TUIGameInfo tUIGameInfo = new TUIGameInfo();
					tUIGameInfo.player_info = new TUIPlayerInfo();
					tUIGameInfo.player_info.avatar_id = character.nID;
					tUIGameInfo.player_info.level = character.nLevel;
					tUIGameInfo.player_info.level_exp = characterInfo.nExp;
					tUIGameInfo.player_info.exp = character.nExp;
					tUIGameInfo.player_info.gold = dataCenter.Gold;
					tUIGameInfo.player_info.crystal = dataCenter.Crystal;
					global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneTavern(m_event.GetEventName(), tUIGameInfo));
				}
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_AllRoleInfo")
		{
			iGameData gameData2 = iGameApp.GetInstance().m_GameData;
			if (gameData2 == null)
			{
				return;
			}
			iCharacterCenter characterCenter = gameData2.GetCharacterCenter();
			if (characterCenter == null)
			{
				return;
			}
			iDataCenter dataCenter2 = gameData2.GetDataCenter();
			if (dataCenter2 == null)
			{
				return;
			}
			TUIGameInfo tUIGameInfo2 = new TUIGameInfo();
			tUIGameInfo2.all_role_info = new TUIAllRoleInfo();
			// Add new characters here
			tUIGameInfo2.all_role_info.role_list = new TUIRoleInfo[6];
			int[] array = new int[6] { 1, 6, 4, 3, 2, 5 };
			for (int i = 0; i < array.Length; i++)
			{
				CCharacterInfo characterInfo2 = gameData2.GetCharacterInfo(array[i]);
				if (characterInfo2 != null)
				{
					CCharacterInfoLevel cCharacterInfoLevel = characterInfo2.Get(1);
					if (i >= tUIGameInfo2.all_role_info.role_list.Length)
					{
						break;
					}
					CCharSaveInfo character2 = dataCenter2.GetCharacter(characterInfo2.nID);
					string introduce_unlock = (characterInfo2.nUnLockLevel > 0)
						? "Complete Stage " + (characterInfo2.nUnLockLevel - 1000) + " to unlock"
						: string.Empty;
					List<TUIPopupInfo> list = new List<TUIPopupInfo>();
					CSkillInfoLevel skillInfo = gameData2.GetSkillInfo(cCharacterInfoLevel.nSkill, 1);
					if (skillInfo != null)
					{
						list.Add(new TUIPopupInfo(cCharacterInfoLevel.nSkill, skillInfo.sName, skillInfo.sDesc));
					}
					tUIGameInfo2.all_role_info.role_list[i] = new TUIRoleInfo(characterInfo2.nID, cCharacterInfoLevel.sName, cCharacterInfoLevel.sDesc, character2 != null, new TUIPriceInfo(characterInfo2.nUnLockPrice, characterInfo2.isCrystalUnLock ? UnitType.Crystal : UnitType.Gold), character2 != null && character2.nLevel >= 1, new TUIPriceInfo(characterInfo2.nPurchasePrice, characterInfo2.isCrystalPurchase ? UnitType.Crystal : UnitType.Gold), introduce_unlock, list);
					if (iGameApp.GetInstance().CheckCharacterSignState(1, characterInfo2.nID))
					{
						tUIGameInfo2.all_role_info.AddNewMark(characterInfo2.nID, NewMarkType.New);
					}
					else if (iGameApp.GetInstance().CheckCharacterMaterialEnough(characterInfo2.nID))
					{
						tUIGameInfo2.all_role_info.AddNewMark(characterInfo2.nID, NewMarkType.Mark);
					}
					else
					{
						tUIGameInfo2.all_role_info.AddNewMark(characterInfo2.nID, NewMarkType.None);
					}
				}
			}
			iGameState gameState = iGameApp.GetInstance().m_GameState;
			if (gameState != null && gameState.m_nLinkCharacter > 0)
			{
				tUIGameInfo2.all_role_info.SetLinkInfo(gameState.m_nLinkCharacter);
			}
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneTavern(m_event.GetEventName(), tUIGameInfo2));
		}
		else if (m_event.GetEventName() == "TUIEvent_RoleUnlock")
		{
			bool flag = false;
			iGameData gameData3 = iGameApp.GetInstance().m_GameData;
			if (gameData3 != null)
			{
				iDataCenter dataCenter3 = gameData3.GetDataCenter();
				if (dataCenter3 != null)
				{
					int wParam = m_event.GetWParam();
					CCharSaveInfo character3 = dataCenter3.GetCharacter(wParam);
					if (character3 == null)
					{
						CCharacterInfo characterInfo3 = gameData3.GetCharacterInfo(wParam);
						if (characterInfo3 != null)
						{
							if (characterInfo3.isCrystalUnLock)
							{
								if (dataCenter3.Crystal < characterInfo3.nUnLockPrice)
								{
									global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneTavern(m_event.GetEventName(), false, BackEventFalseType.NoCrystalEnough, characterInfo3.nUnLockPrice - dataCenter3.Crystal));
									return;
								}
								dataCenter3.AddCrystal(-characterInfo3.nUnLockPrice);
								dataCenter3.UnlockCharacter(wParam);
								dataCenter3.Save();
								flag = true;
								//CFlurryManager.GetInstance().ConsumeCrystal(CFlurryManager.kConsumeType.Char);
							}
							else
							{
								if (dataCenter3.Gold < characterInfo3.nUnLockPrice)
								{
									global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneTavern(m_event.GetEventName(), false, BackEventFalseType.NoGoldEnough, characterInfo3.nUnLockPrice - dataCenter3.Gold));
									return;
								}
								dataCenter3.AddGold(-characterInfo3.nUnLockPrice);
								dataCenter3.UnlockCharacter(wParam);
								dataCenter3.Save();
								flag = true;
								//CFlurryManager.GetInstance().ConsumeGold(CFlurryManager.kConsumeType.Char);
							}
							if (flag)
							{
								iCharacterCenter characterCenter2 = gameData3.GetCharacterCenter();
								if (characterCenter2 != null)
								{
									Dictionary<int, CCharacterInfo> data = characterCenter2.GetData();
									if (data != null)
									{
										TUIGameInfo tUIGameInfo3 = new TUIGameInfo();
										tUIGameInfo3.all_role_info = new TUIAllRoleInfo();
										foreach (CCharacterInfo value in data.Values)
										{
											if (!iGameApp.GetInstance().CheckCharacterSignState(1, value.nID))
											{
												if (iGameApp.GetInstance().CheckCharacterMaterialEnough(value.nID))
												{
													tUIGameInfo3.all_role_info.AddNewMark(value.nID, NewMarkType.Mark);
												}
												else
												{
													tUIGameInfo3.all_role_info.AddNewMark(value.nID, NewMarkType.None);
												}
											}
										}
										global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneTavern("TUIEvent_NewMarkInfo", tUIGameInfo3));
									}
								}
							}
						}
					}
				}
			}
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneTavern(m_event.GetEventName(), flag));
		}
		else if (m_event.GetEventName() == "TUIEvent_RoleBuy")
		{
			bool flag2 = false;
			iGameData gameData4 = iGameApp.GetInstance().m_GameData;
			if (gameData4 != null)
			{
				iDataCenter dataCenter4 = gameData4.GetDataCenter();
				if (dataCenter4 != null)
				{
					int wParam2 = m_event.GetWParam();
					CCharSaveInfo character4 = dataCenter4.GetCharacter(wParam2);
					if (character4 != null && character4.nLevel < 0)
					{
						CCharacterInfo characterInfo4 = gameData4.GetCharacterInfo(wParam2);
						if (characterInfo4 != null)
						{
							if (characterInfo4.isCrystalPurchase)
							{
								if (dataCenter4.Crystal < characterInfo4.nPurchasePrice)
								{
									global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneTavern(m_event.GetEventName(), false, BackEventFalseType.NoCrystalEnough, characterInfo4.nPurchasePrice - dataCenter4.Crystal));
									return;
								}
								dataCenter4.AddCrystal(-characterInfo4.nPurchasePrice);
								dataCenter4.SetCharacter(wParam2, 1, 0);
								if (dataCenter4.CurCharID != wParam2)
								{
									dataCenter4.SetCharacterSign(wParam2, 3);
								}
								dataCenter4.Save();
								flag2 = true;
								//CFlurryManager.GetInstance().ConsumeCrystal(CFlurryManager.kConsumeType.Char);
								//iGameApp.GetInstance().Flurry_PurchaseChar(wParam2);
							}
							else
							{
								if (dataCenter4.Gold < characterInfo4.nPurchasePrice)
								{
									global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneTavern(m_event.GetEventName(), false, BackEventFalseType.NoGoldEnough, characterInfo4.nPurchasePrice - dataCenter4.Gold));
									return;
								}
								dataCenter4.AddGold(-characterInfo4.nPurchasePrice);
								dataCenter4.SetCharacter(wParam2, 1, 0);
								if (dataCenter4.CurCharID != wParam2)
								{
									dataCenter4.SetCharacterSign(wParam2, 3);
								}
								dataCenter4.Save();
								flag2 = true;
								//CFlurryManager.GetInstance().ConsumeGold(CFlurryManager.kConsumeType.Char);
								//iGameApp.GetInstance().Flurry_PurchaseChar(wParam2);
							}
							if (flag2)
							{
								iCharacterCenter characterCenter3 = gameData4.GetCharacterCenter();
								if (characterCenter3 != null)
								{
									Dictionary<int, CCharacterInfo> data2 = characterCenter3.GetData();
									if (data2 != null)
									{
										TUIGameInfo tUIGameInfo4 = new TUIGameInfo();
										tUIGameInfo4.all_role_info = new TUIAllRoleInfo();
										foreach (CCharacterInfo value2 in data2.Values)
										{
											if (!iGameApp.GetInstance().CheckCharacterSignState(1, value2.nID))
											{
												if (iGameApp.GetInstance().CheckCharacterMaterialEnough(value2.nID))
												{
													tUIGameInfo4.all_role_info.AddNewMark(value2.nID, NewMarkType.Mark);
												}
												else
												{
													tUIGameInfo4.all_role_info.AddNewMark(value2.nID, NewMarkType.None);
												}
											}
										}
										global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneTavern("TUIEvent_NewMarkInfo", tUIGameInfo4));
									}
								}
								if (iGameApp.GetInstance().m_GameState != null)
								{
									iGameApp.GetInstance().m_GameState.isCheckUnLock = true;
								}
							}
						}
					}
				}
			}
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneTavern(m_event.GetEventName(), flag2));
		}
		else if (m_event.GetEventName() == "TUIEvent_RoleChange")
		{
			bool success = false;
			TUIGameInfo tUIGameInfo5 = new TUIGameInfo();
			tUIGameInfo5.player_info = new TUIPlayerInfo();
			iGameData gameData5 = iGameApp.GetInstance().m_GameData;
			if (gameData5 != null)
			{
				iDataCenter dataCenter5 = gameData5.GetDataCenter();
				if (dataCenter5 != null)
				{
					int wParam3 = m_event.GetWParam();
					CCharSaveInfo character5 = dataCenter5.GetCharacter(wParam3);
					if (character5 != null && character5.nLevel >= 1)
					{
						CCharacterInfoLevel characterInfo5 = gameData5.GetCharacterInfo(character5.nID, character5.nLevel);
						if (characterInfo5 != null)
						{
							dataCenter5.CurCharID = wParam3;
							dataCenter5.Save();
							success = true;
							tUIGameInfo5.player_info.avatar_id = character5.nID;
							tUIGameInfo5.player_info.level = character5.nLevel;
							tUIGameInfo5.player_info.level_exp = characterInfo5.nExp;
							tUIGameInfo5.player_info.exp = character5.nExp;
							tUIGameInfo5.player_info.gold = dataCenter5.Gold;
							tUIGameInfo5.player_info.crystal = dataCenter5.Crystal;
						}
					}
				}
			}
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneTavern(m_event.GetEventName(), tUIGameInfo5, success));
		}
		else if (m_event.GetEventName() == "TUIEvent_Back")
		{
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneTavern(m_event.GetEventName(), true));
		}
		else if (m_event.GetEventName() == "TUIEvent_RolesChoose")
		{
			iGameData gameData6 = iGameApp.GetInstance().m_GameData;
			if (gameData6 == null)
			{
				return;
			}
			iGameState gameState2 = iGameApp.GetInstance().m_GameState;
			if (gameState2 == null)
			{
				return;
			}
			iDataCenter dataCenter6 = gameData6.GetDataCenter();
			if (dataCenter6 == null)
			{
				return;
			}
			int nSignState = 0;
			int num = (gameState2.m_nLinkCharacter = m_event.GetWParam());
			if (!dataCenter6.GetCharacterSign(num, ref nSignState))
			{
				return;
			}
			if (nSignState == 1)
			{
				dataCenter6.SetCharacterSign(num, 2);
				dataCenter6.Save();
				if (iGameApp.GetInstance().CheckCharacterMaterialEnough(num))
				{
					TUIGameInfo tUIGameInfo6 = new TUIGameInfo();
					tUIGameInfo6.all_role_info = new TUIAllRoleInfo();
					tUIGameInfo6.all_role_info.AddNewMark(num, NewMarkType.Mark);
					global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneTavern("TUIEvent_NewMarkInfo", tUIGameInfo6));
				}
			}
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneTavern(m_event.GetEventName()));
		}
		else if (m_event.GetEventName() == "TUIEvent_GoldToCrystal")
		{
			iGameData gameData7 = iGameApp.GetInstance().m_GameData;
			if (gameData7 == null)
			{
				return;
			}
			iDataCenter dataCenter7 = gameData7.GetDataCenter();
			if (dataCenter7 != null)
			{
				int wParam4 = m_event.GetWParam();
				int num2 = MyUtils.Formula_Gold2Crystal(wParam4);
				if (dataCenter7.Crystal < num2)
				{
					global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneTavern(m_event.GetEventName(), false, BackEventFalseType.NoCrystalEnough, num2 - dataCenter7.Crystal));
					return;
				}
				dataCenter7.AddCrystal(-num2);
				dataCenter7.AddGold(wParam4);
				dataCenter7.Save();
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneTavern(m_event.GetEventName(), true));
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_EnterIAP")
		{
			iGameState gameState3 = iGameApp.GetInstance().m_GameState;
			if (gameState3 != null)
			{
				gameState3.m_lstScene4IAP = TUISceneType.Scene_Tavern;
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneTavern(m_event.GetEventName(), true));
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_EnterGold")
		{
			iGameState gameState4 = iGameApp.GetInstance().m_GameState;
			if (gameState4 != null)
			{
				gameState4.m_lstScene4IAP = TUISceneType.Scene_Tavern;
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneTavern(m_event.GetEventName(), true));
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_EnterIAPCrystalNoEnough")
		{
			iGameState gameState5 = iGameApp.GetInstance().m_GameState;
			if (gameState5 != null)
			{
				gameState5.m_lstScene4IAP = TUISceneType.Scene_Tavern;
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneTavern(m_event.GetEventName(), true));
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_EnterGoEquip")
		{
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneTavern(m_event.GetEventName(), true));
		}
	}

	private void TUIEvent_BackInfo_SceneMap(object sender, TUIEvent.SendEvent_SceneMap m_event)
	{
		if (m_event.GetEventName() == "TUIEvent_TopBar")
		{
			iGameData gameData = iGameApp.GetInstance().m_GameData;
			if (gameData == null)
			{
				return;
			}
			iDataCenter dataCenter = gameData.GetDataCenter();
			if (dataCenter == null)
			{
				return;
			}
			CCharSaveInfo character = dataCenter.GetCharacter(dataCenter.CurCharID);
			if (character == null)
			{
				return;
			}
			CCharacterInfoLevel characterInfo = gameData.GetCharacterInfo(character.nID, character.nLevel);
			if (characterInfo != null)
			{
				TUIGameInfo tUIGameInfo = new TUIGameInfo();
				tUIGameInfo.player_info = new TUIPlayerInfo();
				tUIGameInfo.player_info.avatar_id = character.nID;
				tUIGameInfo.player_info.level = character.nLevel;
				tUIGameInfo.player_info.level_exp = characterInfo.nExp;
				tUIGameInfo.player_info.exp = character.nExp;
				tUIGameInfo.player_info.gold = dataCenter.Gold;
				tUIGameInfo.player_info.crystal = dataCenter.Crystal;
				iGameState gameState = iGameApp.GetInstance().m_GameState;
				if (gameState != null)
				{
					gameState.m_curScene4SearchMaterial = gameState.m_lstScene4SearchMaterial;
					gameState.m_lstScene4SearchMaterial = TUISceneType.None;
				}
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneMap(m_event.GetEventName(), tUIGameInfo));
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_MapEnterInfo")
		{
			iGameState gameState2 = iGameApp.GetInstance().m_GameState;
			if (gameState2 == null)
			{
				return;
			}
			iGameData gameData2 = iGameApp.GetInstance().m_GameData;
			if (gameData2 == null)
			{
				return;
			}
			iDataCenter dataCenter2 = gameData2.GetDataCenter();
			if (dataCenter2 == null)
			{
				return;
			}
			iGameLevelCenter gameLevelCenter = gameData2.GetGameLevelCenter();
			if (gameLevelCenter == null)
			{
				return;
			}
			List<int> list = new List<int>();
			List<int> list2 = new List<int>();
			List<int> levelList = dataCenter2.GetLevelList();
			bool flag = false;
			for (int i = 0; i < levelList.Count; i++)
			{
				if (flag)
				{
					list2.Add(levelList[i]);
				}
				else
				{
					list.Add(levelList[i]);
				}
				if (dataCenter2.LatestLevel == levelList[i])
				{
					flag = true;
				}
			}
			List<int> list3 = new List<int>();
			List<CLevelSaveInfo> levelSaveInfoData = dataCenter2.GetLevelSaveInfoData();
			if (levelSaveInfoData != null)
			{
				foreach (CLevelSaveInfo item in levelSaveInfoData)
				{
					list3.Add(item.nID);
				}
			}
			TUIGameInfo tUIGameInfo2 = new TUIGameInfo();
			if (gameState2.m_nMaterialIDFromEquip == -1)
			{
				int nNewLevel = -1;
				if (dataCenter2.GetNewLevel(ref nNewLevel))
				{
					tUIGameInfo2.map_info = new TUIMapInfo(MapEnterType.OpenNewLevel, dataCenter2.LatestLevel, nNewLevel, list.ToArray(), list2.ToArray(), list3.ToArray());
					dataCenter2.UnlockNewLevelConfirm(nNewLevel);
					dataCenter2.Save();
				}
				else
				{
					tUIGameInfo2.map_info = new TUIMapInfo(MapEnterType.Normal, list[list.Count - 1], list.ToArray(), list2.ToArray(), list3.ToArray());
				}
			}
			else
			{
				int nNewLevel2 = -1;
				if (dataCenter2.GetNewLevel(ref nNewLevel2))
				{
					dataCenter2.UnlockNewLevelConfirm(nNewLevel2);
					dataCenter2.Save();
					list2.Remove(nNewLevel2);
					list.Add(nNewLevel2);
				}
				List<int> list4 = new List<int>();
				int nMaterialIDFromEquip = gameState2.m_nMaterialIDFromEquip;
				gameState2.m_nMaterialIDFromEquip = -1;
				Dictionary<int, GameLevelInfo> data = gameLevelCenter.GetData();
				if (data != null)
				{
					foreach (GameLevelInfo value in data.Values)
					{
						if (value.ltRewardMaterial == null)
						{
							continue;
						}
						foreach (CRewardMaterial item2 in value.ltRewardMaterial)
						{
							if (nMaterialIDFromEquip == item2.nID)
							{
								list4.Add(value.nID);
								break;
							}
						}
					}
				}
				tUIGameInfo2.map_info = new TUIMapInfo(MapEnterType.SearchGoods, list[list.Count - 1], list.ToArray(), list2.ToArray(), list4.ToArray(), list3.ToArray());
			}
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneMap(m_event.GetEventName(), tUIGameInfo2));
		}
		else if (m_event.GetEventName() == "TUIEvent_LevelInfo")
		{
			iGameData gameData3 = iGameApp.GetInstance().m_GameData;
			if (gameData3 == null)
			{
				return;
			}
			iDataCenter dataCenter3 = gameData3.GetDataCenter();
			if (dataCenter3 == null)
			{
				return;
			}
			int wParam = m_event.GetWParam();
			GameLevelInfo gameLevelInfo = gameData3.GetGameLevelInfo(wParam);
			if (gameLevelInfo == null)
			{
				return;
			}
			TUIRecommendRoleInfo recommend_role_info = null;
			TUIRecommendWeaponInfo recommend_weapon_info = null;
			if (gameLevelInfo.m_nRecommandType == 1)
			{
				CWeaponInfoLevel weaponInfo = gameData3.GetWeaponInfo(gameLevelInfo.m_nRecommandID, gameLevelInfo.m_nRecommandLevel);
				if (weaponInfo != null)
				{
					bool have_equip = false;
					int num = dataCenter3.GetWeaponLevel(gameLevelInfo.m_nRecommandID);
					if (num <= 0)
					{
						num = 0;
					}
					else
					{
						for (int j = 0; j < 3; j++)
						{
							if (dataCenter3.GetSelectWeapon(j) == gameLevelInfo.m_nRecommandID)
							{
								have_equip = true;
								break;
							}
						}
					}
					recommend_weapon_info = new TUIRecommendWeaponInfo(gameLevelInfo.m_nRecommandID, num, gameLevelInfo.m_nRecommandLevel, have_equip, gameLevelInfo.m_bRecommandLimit);
				}
			}
			else if (gameLevelInfo.m_nRecommandType == 2)
			{
				CCharacterInfoLevel characterInfo2 = gameData3.GetCharacterInfo(gameLevelInfo.m_nRecommandID, gameLevelInfo.m_nRecommandLevel);
				if (characterInfo2 != null)
				{
					bool have_equip2 = false;
					bool have_buy = false;
					CCharSaveInfo character2 = dataCenter3.GetCharacter(gameLevelInfo.m_nRecommandID);
					if (character2 != null)
					{
						if (character2.nLevel > 0)
						{
							have_buy = true;
						}
						if (dataCenter3.CurCharID == gameLevelInfo.m_nRecommandID)
						{
							have_equip2 = true;
						}
					}
					recommend_role_info = new TUIRecommendRoleInfo(gameLevelInfo.m_nRecommandID, have_buy, have_equip2, gameLevelInfo.m_bRecommandLimit);
				}
			}
			else if (gameLevelInfo.m_nRecommandType == 3)
			{
				CItemInfoLevel itemInfo = gameData3.GetItemInfo(gameLevelInfo.m_nRecommandID, gameLevelInfo.m_nRecommandLevel);
				if (itemInfo != null)
				{
					bool have_equip3 = false;
					int nItemLevel = 0;
					dataCenter3.GetEquipStone(gameLevelInfo.m_nRecommandID, ref nItemLevel);
					if (nItemLevel <= 0)
					{
						nItemLevel = 0;
					}
					else if (dataCenter3.CurEquipStone == gameLevelInfo.m_nRecommandID)
					{
						have_equip3 = true;
					}
					recommend_weapon_info = new TUIRecommendWeaponInfo(gameLevelInfo.m_nRecommandID, nItemLevel, gameLevelInfo.m_nRecommandLevel, have_equip3, gameLevelInfo.m_bRecommandLimit);
				}
			}
			string sLevelDesc = gameLevelInfo.sLevelDesc;
			string introduce = "Exp: " + gameLevelInfo.nRewardExp + "\nGold: " + gameLevelInfo.nRewardGold;
			List<TUIGoodsInfo> list5 = new List<TUIGoodsInfo>();
			if (gameLevelInfo.ltRewardMaterial != null)
			{
				foreach (CRewardMaterial item3 in gameLevelInfo.ltRewardMaterial)
				{
					if (item3.nID == 0)
					{
						continue;
					}
					CItemInfoLevel itemInfo2 = gameData3.GetItemInfo(item3.nID, 1);
					if (itemInfo2 != null)
					{
						GoodsQualityType quality = GoodsQualityType.Quality01;
						switch (itemInfo2.nRare)
						{
						case 1:
							quality = GoodsQualityType.Quality01;
							break;
						case 2:
							quality = GoodsQualityType.Quality02;
							break;
						case 3:
							quality = GoodsQualityType.Quality03;
							break;
						case 4:
							quality = GoodsQualityType.Quality04;
							break;
						case 5:
							quality = GoodsQualityType.Quality05;
							break;
						case 6:
							quality = GoodsQualityType.Quality06;
							break;
						}
						list5.Add(new TUIGoodsInfo(item3.nID, quality, itemInfo2.sName));
					}
				}
			}
			TUILevelInfo level_info = new TUILevelInfo(wParam, sLevelDesc, introduce, list5, recommend_role_info, recommend_weapon_info, gameLevelInfo.sLevelName);
			TUIGameInfo tUIGameInfo3 = new TUIGameInfo();
			tUIGameInfo3.map_info = new TUIMapInfo(level_info);
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneMap(m_event.GetEventName(), tUIGameInfo3));
		}
		else if (m_event.GetEventName() == "TUIEvent_EnterLevel")
		{
			iGameState gameState3 = iGameApp.GetInstance().m_GameState;
			if (gameState3 != null)
			{
				int wParam2 = m_event.GetWParam();
				Debug.Log(wParam2);
				string text = "Scene_Main";
				gameState3.GameLevel = wParam2;
				iGameApp.GetInstance().EnterScene(kGameSceneEnum.Game);
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_Back")
		{
			iGameState gameState4 = iGameApp.GetInstance().m_GameState;
			if (gameState4 != null)
			{
				if (gameState4.m_curScene4SearchMaterial != 0)
				{
					global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneMap(m_event.GetEventName(), true, (int)gameState4.m_curScene4SearchMaterial));
					gameState4.m_curScene4SearchMaterial = TUISceneType.None;
				}
				else
				{
					global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneMap(m_event.GetEventName(), true));
				}
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_EnterWeaponBuy")
		{
			int wParam3 = m_event.GetWParam();
			iGameState gameState5 = iGameApp.GetInstance().m_GameState;
			if (gameState5 != null)
			{
				gameState5.m_nLinkWeapon = wParam3;
				gameState5.m_lstScene4Recommand = TUISceneType.Scene_Map;
			}
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneMap(m_event.GetEventName(), true));
		}
		else if (m_event.GetEventName() == "TUIEvent_EnterRoleBuy")
		{
			int wParam4 = m_event.GetWParam();
			iGameState gameState6 = iGameApp.GetInstance().m_GameState;
			if (gameState6 != null)
			{
				gameState6.m_nLinkCharacter = wParam4;
				gameState6.m_lstScene4Recommand = TUISceneType.Scene_Map;
			}
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneMap(m_event.GetEventName(), true));
		}
		else if (m_event.GetEventName() == "TUIEvent_EnterEquip")
		{
			iGameState gameState7 = iGameApp.GetInstance().m_GameState;
			if (gameState7 != null)
			{
				gameState7.m_lstScene4Recommand = TUISceneType.Scene_Map;
			}
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneMap(m_event.GetEventName(), true));
		}
		else if (m_event.GetEventName() == "TUIEvent_EnterIAP")
		{
			iGameState gameState8 = iGameApp.GetInstance().m_GameState;
			if (gameState8 != null)
			{
				gameState8.m_lstScene4IAP = TUISceneType.Scene_Map;
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneMap(m_event.GetEventName(), true));
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_EnterGold")
		{
			iGameState gameState9 = iGameApp.GetInstance().m_GameState;
			if (gameState9 != null)
			{
				gameState9.m_lstScene4IAP = TUISceneType.Scene_Map;
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneMap(m_event.GetEventName(), true));
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_EnterVilliage")
		{
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneMap(m_event.GetEventName(), true));
		}
	}

	private void TUIEvent_BackInfo_SceneIAP(object sender, TUIEvent.SendEvent_SceneIAP m_event)
	{
		if (m_event.GetEventName() == "TUIEvent_TopBar")
		{
			iGameData gameData = iGameApp.GetInstance().m_GameData;
			if (gameData == null)
			{
				return;
			}
			iDataCenter dataCenter = gameData.GetDataCenter();
			if (dataCenter == null)
			{
				return;
			}
			CCharSaveInfo character = dataCenter.GetCharacter(dataCenter.CurCharID);
			if (character == null)
			{
				return;
			}
			CCharacterInfoLevel characterInfo = gameData.GetCharacterInfo(character.nID, character.nLevel);
			if (characterInfo != null)
			{
				TUIGameInfo tUIGameInfo = new TUIGameInfo();
				tUIGameInfo.player_info = new TUIPlayerInfo();
				tUIGameInfo.player_info.avatar_id = character.nID;
				tUIGameInfo.player_info.level = character.nLevel;
				tUIGameInfo.player_info.level_exp = characterInfo.nExp;
				tUIGameInfo.player_info.exp = character.nExp;
				tUIGameInfo.player_info.gold = dataCenter.Gold;
				tUIGameInfo.player_info.crystal = dataCenter.Crystal;
				iGameState gameState = iGameApp.GetInstance().m_GameState;
				if (gameState != null)
				{
					gameState.m_curScene4IAP = gameState.m_lstScene4IAP;
					gameState.m_lstScene4IAP = TUISceneType.None;
				}
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneIAP(m_event.GetEventName(), tUIGameInfo));
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_IAPBuy")
		{
			int wParam = m_event.GetWParam();
			iIAPManager.GetInstance().Purchase(wParam);
			iIAPManager.GetInstance().SetSuccessFunc(OnPurchaseIAPSuccess);
			iIAPManager.GetInstance().SetFailedFunc(OnPurchaseIAPFailed);
			iIAPManager.GetInstance().SetCancelFunc(OnPurchaseIAPCancel);
			iIAPManager.GetInstance().SetNetErrorFunc(OnPurchaseIAPFailed);
			iIAPManager.GetInstance().SetOnSendVerifyFunc(OnPurchaseIAPSendVerify);
			iIAPManager.GetInstance().SetOnVerifyFailed(OnPurchaseIAPVerifyFailed);
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneIAP(m_event.GetEventName()));
		}
		else if (m_event.GetEventName() == "TUIEvent_Back")
		{
			iGameState gameState2 = iGameApp.GetInstance().m_GameState;
			if (gameState2 != null)
			{
				if (gameState2.m_curScene4IAP != 0)
				{
					Debug.Log(gameState2.m_curScene4IAP);
					global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneIAP(m_event.GetEventName(), true, (int)gameState2.m_curScene4IAP));
					gameState2.m_curScene4IAP = TUISceneType.None;
				}
				else
				{
					global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneIAP(m_event.GetEventName(), true));
				}
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_EnterGold")
		{
			iGameState gameState3 = iGameApp.GetInstance().m_GameState;
			if (gameState3 != null)
			{
				gameState3.m_lstScene4IAP = gameState3.m_curScene4IAP;
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneIAP(m_event.GetEventName(), true));
			}
		}
	}

	private void TUIEvent_BackInfo_SceneGold(object sender, TUIEvent.SendEvent_SceneGold m_event)
	{
		if (m_event.GetEventName() == "TUIEvent_TopBar")
		{
			iGameData gameData = iGameApp.GetInstance().m_GameData;
			if (gameData == null)
			{
				return;
			}
			iDataCenter dataCenter = gameData.GetDataCenter();
			if (dataCenter == null)
			{
				return;
			}
			CCharSaveInfo character = dataCenter.GetCharacter(dataCenter.CurCharID);
			if (character == null)
			{
				return;
			}
			CCharacterInfoLevel characterInfo = gameData.GetCharacterInfo(character.nID, character.nLevel);
			if (characterInfo != null)
			{
				TUIGameInfo tUIGameInfo = new TUIGameInfo();
				tUIGameInfo.player_info = new TUIPlayerInfo();
				tUIGameInfo.player_info.avatar_id = character.nID;
				tUIGameInfo.player_info.level = character.nLevel;
				tUIGameInfo.player_info.level_exp = characterInfo.nExp;
				tUIGameInfo.player_info.exp = character.nExp;
				tUIGameInfo.player_info.gold = dataCenter.Gold;
				tUIGameInfo.player_info.crystal = dataCenter.Crystal;
				iGameState gameState = iGameApp.GetInstance().m_GameState;
				if (gameState != null)
				{
					gameState.m_curScene4IAP = gameState.m_lstScene4IAP;
					gameState.m_lstScene4IAP = TUISceneType.None;
				}
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneGold(m_event.GetEventName(), tUIGameInfo));
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_GoldBuy")
		{
			iGameData gameData2 = iGameApp.GetInstance().m_GameData;
			if (gameData2 == null)
			{
				return;
			}
			iDataCenter dataCenter2 = gameData2.GetDataCenter();
			if (dataCenter2 == null)
			{
				return;
			}
			int wParam = m_event.GetWParam();
			CCrystal2GoldInfo crystal2GoldInfo = iIAPManager.GetInstance().GetCrystal2GoldInfo(wParam);
			if (crystal2GoldInfo != null)
			{
				if (dataCenter2.Crystal >= crystal2GoldInfo.nCrystal)
				{
					dataCenter2.AddGold(crystal2GoldInfo.nGold);
					dataCenter2.AddCrystal(-crystal2GoldInfo.nCrystal);
					dataCenter2.Save();
					//CFlurryManager.GetInstance().ConsumeCrystal(CFlurryManager.kConsumeType.Gold);
					TUIGameInfo tUIGameInfo2 = new TUIGameInfo();
					tUIGameInfo2.player_info = new TUIPlayerInfo();
					tUIGameInfo2.player_info.gold = dataCenter2.Gold;
					tUIGameInfo2.player_info.crystal = dataCenter2.Crystal;
					global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneGold("TUIEvent_GoldResult", tUIGameInfo2, true));
				}
				else
				{
					global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneGold("TUIEvent_GoldResult", false, BackEventFalseType.NoCrystalEnough, crystal2GoldInfo.nCrystal - dataCenter2.Crystal));
				}
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_Back")
		{
			iGameState gameState2 = iGameApp.GetInstance().m_GameState;
			if (gameState2 != null)
			{
				if (gameState2.m_curScene4IAP != 0)
				{
					Debug.Log(gameState2.m_curScene4IAP);
					global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneGold(m_event.GetEventName(), true, (int)gameState2.m_curScene4IAP));
					gameState2.m_curScene4IAP = TUISceneType.None;
				}
				else
				{
					global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneGold(m_event.GetEventName(), true));
				}
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_EnterIAP")
		{
			iGameState gameState3 = iGameApp.GetInstance().m_GameState;
			if (gameState3 != null)
			{
				gameState3.m_lstScene4IAP = gameState3.m_curScene4IAP;
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneGold(m_event.GetEventName(), true));
			}
		}
		else if (m_event.GetEventName() == "TUIEvent_EnterIAPCrystalNoEnough")
		{
			global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneGold(m_event.GetEventName(), true));
		}
	}

	protected void OnPurchaseIAPSuccess()
	{
		iGameData gameData = iGameApp.GetInstance().m_GameData;
		if (gameData != null)
		{
			iDataCenter dataCenter = gameData.GetDataCenter();
			if (dataCenter != null)
			{
				TUIGameInfo tUIGameInfo = new TUIGameInfo();
				tUIGameInfo.player_info = new TUIPlayerInfo();
				tUIGameInfo.player_info.gold = dataCenter.Gold;
				tUIGameInfo.player_info.crystal = dataCenter.Crystal;
				global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneIAP("TUIEvent_ServerResult", tUIGameInfo, true));
			}
		}
	}

	protected void OnPurchaseIAPFailed()
	{
		global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneIAP("TUIEvent_IAPResult", false, 2));
	}

	protected void OnPurchaseIAPCancel()
	{
		global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneIAP("TUIEvent_IAPResult", false, 1));
	}

	protected void OnPurchaseIAPSendVerify()
	{
		global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneIAP("TUIEvent_IAPResult", true));
	}

	protected void OnPurchaseIAPVerifyFailed()
	{
		global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneIAP("TUIEvent_ServerResult", false, 3));
	}

	protected void OnServerVerifySuccess()
	{
		global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneMain("TUIEvent_ConnectResult", true));
	}

	protected void OnServerVerifyFailed()
	{
		global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneMain("TUIEvent_ConnectResult", false, 2));
	}

	protected void OnServerVerifyNetError()
	{
		global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.BackEvent_SceneMain("TUIEvent_ConnectResult", false, 1));
	}
}
