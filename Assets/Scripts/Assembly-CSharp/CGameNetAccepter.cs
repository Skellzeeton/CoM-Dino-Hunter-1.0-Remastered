using System.Collections.Generic;
using TNetSdk;
using UnityEngine;
using gyEvent;

public class CGameNetAccepter
{
	protected delegate void OnMsgFunc(TNetUser sender, SFSObject data);

	protected static CGameNetAccepter m_Instance;

	protected iGameSceneBase m_GameScene;

	protected iGameData m_GameData;

	protected Dictionary<kGameNetEnum, OnMsgFunc> m_dictMsgCallBack;

	public static CGameNetAccepter GetInstance()
	{
		if (m_Instance == null)
		{
			m_Instance = new CGameNetAccepter();
		}
		return m_Instance;
	}

	public void Initialize()
	{
		m_GameScene = iGameApp.GetInstance().m_GameScene;
		m_GameData = iGameApp.GetInstance().m_GameData;
		if (m_dictMsgCallBack == null)
		{
			m_dictMsgCallBack = new Dictionary<kGameNetEnum, OnMsgFunc>();
		}
		m_dictMsgCallBack.Clear();
		m_dictMsgCallBack.Add(kGameNetEnum.GAME_ADDITEM, OnGameAddItemMsg);
		m_dictMsgCallBack.Add(kGameNetEnum.PLAYER_MOVE, OnPlayerMoveMsg);
		m_dictMsgCallBack.Add(kGameNetEnum.PLAYER_MOVESTOP, OnPlayerMoveStopMsg);
		m_dictMsgCallBack.Add(kGameNetEnum.PLAYER_AIM, OnPlayerAimMsg);
		m_dictMsgCallBack.Add(kGameNetEnum.PLAYER_SWITCHWEAPON, OnPlayerSwitchWeaponMsg);
		m_dictMsgCallBack.Add(kGameNetEnum.PLAYER_SHOOT, OnPlayerShootMsg);
		m_dictMsgCallBack.Add(kGameNetEnum.PLAYER_SKILL, OnPlayerSkillMsg);
		m_dictMsgCallBack.Add(kGameNetEnum.PLAYER_BEATBACK, OnPlayerBeatbackMsg);
		m_dictMsgCallBack.Add(kGameNetEnum.PLAYER_LEVELUP, OnPlayerLevelupMsg);
		m_dictMsgCallBack.Add(kGameNetEnum.PLAYER_TAKEITEM, OnPlayerTakeItemMsg);
		m_dictMsgCallBack.Add(kGameNetEnum.PLAYER_LEAVEROOM, OnPlayerLeaveRoomMsg);
		m_dictMsgCallBack.Add(kGameNetEnum.MGMANAGER_ADDMOB, OnMGManagerAddMobMsg);
		m_dictMsgCallBack.Add(kGameNetEnum.BATTLE_DAMAGE_MOB, OnBattleDamageMonsterMsg);
		m_dictMsgCallBack.Add(kGameNetEnum.BATTLE_DAMAGE_PLAYER, OnBattleDamagePlayerMsg);
		m_dictMsgCallBack.Add(kGameNetEnum.MOB_DEAD, OnMonsterDeadMsg);
		m_dictMsgCallBack.Add(kGameNetEnum.MOB_HURT, OnMonsterHurtMsg);
		m_dictMsgCallBack.Add(kGameNetEnum.MOB_MOVE, OnMonsterMoveMsg);
		m_dictMsgCallBack.Add(kGameNetEnum.MOB_BEATBACK, OnMonsterBeatbackMsg);
		m_dictMsgCallBack.Add(kGameNetEnum.MOB_ATTACK, OnMonsterAttackMsg);
		m_dictMsgCallBack.Add(kGameNetEnum.TASK_COMPELETE, OnTaskCompeleteMsg);
		m_dictMsgCallBack.Add(kGameNetEnum.TASK_GET_ITEM, OnTaskGetItemMsg);
		m_dictMsgCallBack.Add(kGameNetEnum.TASK_TIME_SYNC, OnTaskTimeSyncMsg);
		TNetManager.GetInstance().AddAcceptMsgFunc(OnGameMsg);
	}

	public void Destroy()
	{
		m_GameScene = null;
		TNetManager.GetInstance().DelAcceptMsgFunc(OnGameMsg);
		m_dictMsgCallBack.Clear();
	}

	public void OnGameMsg(TNetUser sender, kGameNetEnum nmsg, SFSObject data)
	{
		Debug.Log("OnGameMsg " + nmsg);
		if (m_dictMsgCallBack != null && m_dictMsgCallBack.ContainsKey(nmsg))
		{
			m_dictMsgCallBack[nmsg](sender, data);
		}
	}

	protected void OnGameAddItemMsg(TNetUser sender, SFSObject data)
	{
		if (!sender.IsItMe)
		{
			nmsg_game_additem nmsg_game_additem2 = new nmsg_game_additem();
			nmsg_game_additem2.UnPack(data);
			m_GameScene.AddItem(nmsg_game_additem2.nItemID, nmsg_game_additem2.v3Pos, Vector3.forward, -1f, nmsg_game_additem2.nItemUID);
		}
	}

	protected void OnPlayerMoveMsg(TNetUser sender, SFSObject data)
	{
		if (!sender.IsItMe)
		{
			CCharPlayer player = m_GameScene.GetPlayer(sender.Id);
			if (!(player == null))
			{
				nmsg_playermove nmsg_playermove2 = new nmsg_playermove();
				nmsg_playermove2.UnPack(data);
				player.MoveTo(nmsg_playermove2.v3Dst);
			}
		}
	}

	protected void OnPlayerMoveStopMsg(TNetUser sender, SFSObject data)
	{
		if (!sender.IsItMe)
		{
			CCharPlayer player = m_GameScene.GetPlayer(sender.Id);
			if (!(player == null))
			{
				nmsg_playermovestop nmsg_playermovestop2 = new nmsg_playermovestop();
				nmsg_playermovestop2.UnPack(data);
				player.MoveStop(nmsg_playermovestop2.v3Dst);
			}
		}
	}

	protected void OnPlayerAimMsg(TNetUser sender, SFSObject data)
	{
		if (!sender.IsItMe)
		{
			CCharPlayer player = m_GameScene.GetPlayer(sender.Id);
			if (!(player == null))
			{
				nmsg_playeraim nmsg_playeraim2 = new nmsg_playeraim();
				nmsg_playeraim2.UnPack(data);
				player.AimTo(nmsg_playeraim2.v3AimPoint);
			}
		}
	}

	protected void OnPlayerSwitchWeaponMsg(TNetUser sender, SFSObject data)
	{
		if (!sender.IsItMe)
		{
			CCharPlayer player = m_GameScene.GetPlayer(sender.Id);
			if (!(player == null))
			{
				nmsg_playerswitchweapon nmsg_playerswitchweapon2 = new nmsg_playerswitchweapon();
				nmsg_playerswitchweapon2.UnPack(data);
				player.UnEquipWeapon();
				player.EquipWeapon(nmsg_playerswitchweapon2.nWeaponID, nmsg_playerswitchweapon2.nWeaponLevel);
			}
		}
	}

	protected void OnPlayerShootMsg(TNetUser sender, SFSObject data)
	{
		if (!sender.IsItMe)
		{
			CCharPlayer player = m_GameScene.GetPlayer(sender.Id);
			if (!(player == null))
			{
				nmsg_playershoot nmsg_playershoot2 = new nmsg_playershoot();
				nmsg_playershoot2.UnPack(data);
				player.Shoot(nmsg_playershoot2.bShoot);
			}
		}
	}

	protected void OnPlayerSkillMsg(TNetUser sender, SFSObject data)
	{
		if (!sender.IsItMe)
		{
			CCharPlayer player = m_GameScene.GetPlayer(sender.Id);
			if (!(player == null))
			{
				nmsg_playerskill nmsg_playerskill2 = new nmsg_playerskill();
				nmsg_playerskill2.UnPack(data);
				player.UseSkill(nmsg_playerskill2.m_nSkillID, nmsg_playerskill2.m_nTargetUID);
			}
		}
	}

	protected void OnPlayerBeatbackMsg(TNetUser sender, SFSObject data)
	{
		if (!sender.IsItMe)
		{
			nmsg_playerbeatback nmsg_playerbeatback2 = new nmsg_playerbeatback();
			nmsg_playerbeatback2.UnPack(data);
			CCharPlayer player = m_GameScene.GetPlayer(nmsg_playerbeatback2.m_nPlayerUID);
			if (!(player == null))
			{
				player.BeatBack(nmsg_playerbeatback2.m_v3Dst);
			}
		}
	}

	protected void OnPlayerLevelupMsg(TNetUser sender, SFSObject data)
	{
		if (!sender.IsItMe)
		{
			CCharPlayer player = m_GameScene.GetPlayer(sender.Id);
			if (!(player == null))
			{
				nmsg_playerlevelup nmsg_playerlevelup2 = new nmsg_playerlevelup();
				nmsg_playerlevelup2.UnPack(data);
				player.LevelUp(nmsg_playerlevelup2.m_nLevel);
			}
		}
	}

	protected void OnPlayerTakeItemMsg(TNetUser sender, SFSObject data)
	{
		if (sender.IsItMe)
		{
			return;
		}
		CCharPlayer player = m_GameScene.GetPlayer(sender.Id);
		if (player == null)
		{
			return;
		}
		nmsg_playertakeitem nmsg_playertakeitem2 = new nmsg_playertakeitem();
		nmsg_playertakeitem2.UnPack(data);
		if (nmsg_playertakeitem2.nItemUID == -1)
		{
			player.DropItem();
			return;
		}
		List<GameObject> sceneItemList = m_GameScene.GetSceneItemList();
		if (sceneItemList == null)
		{
			return;
		}
		foreach (GameObject item in sceneItemList)
		{
			if (item == null)
			{
				continue;
			}
			iItem component = item.GetComponent<iItem>();
			if (!(component != null) || component.UID != nmsg_playertakeitem2.nItemUID)
			{
				continue;
			}
			if (player.IsTakenItem())
			{
				player.DropItem();
			}
			player.TakeItem(component.ID, item);
			break;
		}
	}

	protected void OnPlayerLeaveRoomMsg(TNetUser sender, SFSObject data)
	{
		if (!sender.IsItMe && m_GameScene != null)
		{
			m_GameScene.RemovePlayer(sender.Id);
		}
	}

	protected void OnMGManagerAddMobMsg(TNetUser sender, SFSObject data)
	{
		if (!sender.IsItMe)
		{
			CCharPlayer player = m_GameScene.GetPlayer(sender.Id);
			if (!(player == null))
			{
				nmsg_mgmanager_addmob nmsg_mgmanager_addmob2 = new nmsg_mgmanager_addmob();
				nmsg_mgmanager_addmob2.UnPack(data);
				m_GameScene.AddMobByWave(nmsg_mgmanager_addmob2.m_nMobID, nmsg_mgmanager_addmob2.m_nMobLevel, nmsg_mgmanager_addmob2.m_nMobUID, nmsg_mgmanager_addmob2.m_nWaveID, nmsg_mgmanager_addmob2.m_nSequence, nmsg_mgmanager_addmob2.m_v3Pos, nmsg_mgmanager_addmob2.m_v3Dir);
			}
		}
	}

	protected void OnBattleDamageMonsterMsg(TNetUser sender, SFSObject data)
	{
		if (sender.IsItMe)
		{
			return;
		}
		CCharPlayer player = m_GameScene.GetPlayer(sender.Id);
		if (!(player == null))
		{
			nmsg_battle_damage_mob nmsg_battle_damage_mob2 = new nmsg_battle_damage_mob();
			nmsg_battle_damage_mob2.UnPack(data);
			CCharMob mob = m_GameScene.GetMob(nmsg_battle_damage_mob2.m_nMobUID);
			if (!(mob == null))
			{
				Debug.Log("OnBattleDamageMonsterMsg " + nmsg_battle_damage_mob2.m_fDamage);
				mob.SetLifeBarParam(3f);
				mob.OnHit(0f - nmsg_battle_damage_mob2.m_fDamage, null, string.Empty);
			}
		}
	}

	protected void OnBattleDamagePlayerMsg(TNetUser sender, SFSObject data)
	{
		if (!sender.IsItMe)
		{
			CCharPlayer player = m_GameScene.GetPlayer(sender.Id);
			if (!(player == null))
			{
				nmsg_battle_damage_player nmsg_battle_damage_player2 = new nmsg_battle_damage_player();
				nmsg_battle_damage_player2.UnPack(data);
				Debug.Log("OnBattleDamagePlayerMsg " + nmsg_battle_damage_player2.m_fDamage);
				player.OnHit(0f - nmsg_battle_damage_player2.m_fDamage, null, string.Empty);
			}
		}
	}

	protected void OnMonsterDeadMsg(TNetUser sender, SFSObject data)
	{
		if (!sender.IsItMe)
		{
			nmsg_monster_dead nmsg_monster_dead2 = new nmsg_monster_dead();
			nmsg_monster_dead2.UnPack(data);
			CCharMob mob = m_GameScene.GetMob(nmsg_monster_dead2.m_nMobUID);
			if (!(mob == null) && !mob.isDead)
			{
				mob.OnDead(nmsg_monster_dead2.m_DeadMode);
			}
		}
	}

	protected void OnMonsterHurtMsg(TNetUser sender, SFSObject data)
	{
		if (!sender.IsItMe)
		{
			nmsg_monster_hurt nmsg_monster_hurt2 = new nmsg_monster_hurt();
			nmsg_monster_hurt2.UnPack(data);
			CCharMob mob = m_GameScene.GetMob(nmsg_monster_hurt2.m_nMobUID);
			if (!(mob == null) && !mob.isDead)
			{
				mob.m_bHurting = true;
				mob.m_HurtAnim = nmsg_monster_hurt2.m_HurtAnim;
				mob.ResetAI();
			}
		}
	}

	protected void OnMonsterMoveMsg(TNetUser sender, SFSObject data)
	{
		if (!sender.IsItMe)
		{
			nmsg_monster_move nmsg_monster_move2 = new nmsg_monster_move();
			nmsg_monster_move2.UnPack(data);
			CCharMob mob = m_GameScene.GetMob(nmsg_monster_move2.m_nMobUID);
			if (!(mob == null) && !mob.isDead)
			{
				mob.MoveTo(nmsg_monster_move2.m_v3Dst);
			}
		}
	}

	protected void OnMonsterAttackMsg(TNetUser sender, SFSObject data)
	{
		if (sender.IsItMe)
		{
			return;
		}
		nmsg_monster_attack nmsg_monster_attack2 = new nmsg_monster_attack();
		nmsg_monster_attack2.UnPack(data);
		CCharMob mob = m_GameScene.GetMob(nmsg_monster_attack2.m_nMobUID);
		if (!(mob == null) && !mob.isDead && mob.m_pSkillComboInfo == null)
		{
			CCharBase player = m_GameScene.GetPlayer(nmsg_monster_attack2.m_nTargetUID);
			if (player != null)
			{
				mob.m_Target = player;
			}
			mob.m_pSkillComboInfo = m_GameData.GetSkillComboInfo(nmsg_monster_attack2.m_nComboSkillID);
			mob.m_nCurComboIndex = 0;
			mob.ResetAI();
			Debug.Log(mob.UID + " hit " + player.UID + " combo = " + nmsg_monster_attack2.m_nComboSkillID);
		}
	}

	protected void OnMonsterBeatbackMsg(TNetUser sender, SFSObject data)
	{
		if (!sender.IsItMe)
		{
			nmsg_monster_beatback nmsg_monster_beatback2 = new nmsg_monster_beatback();
			nmsg_monster_beatback2.UnPack(data);
			CCharMob mob = m_GameScene.GetMob(nmsg_monster_beatback2.m_nMobUID);
			if (!(mob == null) && !mob.isDead)
			{
				mob.BeatBack(nmsg_monster_beatback2.m_v3Dst);
			}
		}
	}

	protected void OnTaskCompeleteMsg(TNetUser sender, SFSObject data)
	{
		if (!sender.IsItMe)
		{
			nmsg_task_compelete nmsg_task_compelete2 = new nmsg_task_compelete();
			nmsg_task_compelete2.UnPack(data);
			if (m_GameScene.m_TaskManager != null)
			{
				m_GameScene.m_TaskManager.OnAllTaskCompelete(nmsg_task_compelete2.m_isSuccess);
			}
		}
	}

	protected void OnTaskGetItemMsg(TNetUser sender, SFSObject data)
	{
		if (sender.IsItMe)
		{
			return;
		}
		nmsg_task_getitem nmsg_task_getitem2 = new nmsg_task_getitem();
		nmsg_task_getitem2.UnPack(data);
		if (m_GameScene.m_TaskManager != null)
		{
			m_GameScene.m_TaskManager.OnGetItem(nmsg_task_getitem2.m_nItemID);
		}
		m_GameScene.AddStealItem(nmsg_task_getitem2.m_nItemID, 1);
		if (m_GameScene.IsRoomMaster() && m_GameScene.m_MGManager != null)
		{
			CEventManager eventManager = m_GameScene.m_MGManager.GetEventManager();
			if (eventManager != null)
			{
				eventManager.Trigger(new EventCondition_StealEgg_Home(nmsg_task_getitem2.m_nItemID, m_GameScene.GetStealItem(nmsg_task_getitem2.m_nItemID)));
			}
		}
	}

	protected void OnTaskTimeSyncMsg(TNetUser sender, SFSObject data)
	{
		if (!sender.IsItMe)
		{
			nmsg_task_timesync nmsg_task_timesync2 = new nmsg_task_timesync();
			nmsg_task_timesync2.UnPack(data);
		}
	}
}
