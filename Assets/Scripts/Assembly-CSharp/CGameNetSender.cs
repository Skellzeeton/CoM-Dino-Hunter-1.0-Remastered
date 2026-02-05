using System.Collections.Generic;
using UnityEngine;

public class CGameNetSender
{
	protected static CGameNetSender m_Instance;

	protected iGameSceneBase m_GameScene;

	public static CGameNetSender GetInstance()
	{
		if (m_Instance == null)
		{
			m_Instance = new CGameNetSender();
			m_Instance.Initialize();
		}
		return m_Instance;
	}

	public void Initialize()
	{
		m_GameScene = iGameApp.GetInstance().m_GameScene;
	}

	public void GamePlayerInfoBroadcast(int nCharID, int nCharLevel)
	{
		nmsg_game_playerinfo nmsg_game_playerinfo2 = new nmsg_game_playerinfo();
		nmsg_game_playerinfo2.msghead = kGameNetEnum.GAME_PLAYERINFO;
		nmsg_game_playerinfo2.nCharID = nCharID;
		nmsg_game_playerinfo2.nCharLevel = nCharLevel;
		TNetManager.GetInstance().BroadcastMessage(nmsg_game_playerinfo2);
	}

	public void GamePlayerInfo(int nReciverUID, int nCharID, int nCharLevel)
	{
		nmsg_game_playerinfo nmsg_game_playerinfo2 = new nmsg_game_playerinfo();
		nmsg_game_playerinfo2.msghead = kGameNetEnum.GAME_PLAYERINFO;
		nmsg_game_playerinfo2.nCharID = nCharID;
		nmsg_game_playerinfo2.nCharLevel = nCharLevel;
		TNetManager.GetInstance().SendMessage(nReciverUID, nmsg_game_playerinfo2);
	}

	public void Game_AddItem(int nUID, int nID, Vector3 v3Pos)
	{
		nmsg_game_additem nmsg_game_additem2 = new nmsg_game_additem();
		nmsg_game_additem2.msghead = kGameNetEnum.GAME_ADDITEM;
		nmsg_game_additem2.nItemUID = nUID;
		nmsg_game_additem2.nItemID = nID;
		nmsg_game_additem2.v3Pos = v3Pos;
		TNetManager.GetInstance().BroadcastMessage(nmsg_game_additem2);
	}

	public void PlayerMove(Vector3 v3Src, Vector3 v3Dst)
	{
		nmsg_playermove nmsg_playermove2 = new nmsg_playermove();
		nmsg_playermove2.msghead = kGameNetEnum.PLAYER_MOVE;
		nmsg_playermove2.v3Src = v3Src;
		nmsg_playermove2.v3Dst = v3Dst;
		TNetManager.GetInstance().BroadcastMessage(nmsg_playermove2);
	}

	public void PlayerMoveStop(Vector3 v3Dst)
	{
		nmsg_playermovestop nmsg_playermovestop2 = new nmsg_playermovestop();
		nmsg_playermovestop2.msghead = kGameNetEnum.PLAYER_MOVESTOP;
		nmsg_playermovestop2.v3Dst = v3Dst;
		TNetManager.GetInstance().BroadcastMessage(nmsg_playermovestop2);
	}

	public void PlayerAim(Vector3 v3AimPoint)
	{
		nmsg_playeraim nmsg_playeraim2 = new nmsg_playeraim();
		nmsg_playeraim2.msghead = kGameNetEnum.PLAYER_AIM;
		nmsg_playeraim2.v3AimPoint = v3AimPoint;
		TNetManager.GetInstance().BroadcastMessage(nmsg_playeraim2);
	}

	public void PlayerSwitchWeapon(int nWeaponID, int nWeaponLevel)
	{
		nmsg_playerswitchweapon nmsg_playerswitchweapon2 = new nmsg_playerswitchweapon();
		nmsg_playerswitchweapon2.msghead = kGameNetEnum.PLAYER_SWITCHWEAPON;
		nmsg_playerswitchweapon2.nWeaponID = nWeaponID;
		nmsg_playerswitchweapon2.nWeaponLevel = nWeaponLevel;
		TNetManager.GetInstance().BroadcastMessage(nmsg_playerswitchweapon2);
	}

	public void PlayerShoot(bool bShoot)
	{
		nmsg_playershoot nmsg_playershoot2 = new nmsg_playershoot();
		nmsg_playershoot2.msghead = kGameNetEnum.PLAYER_SHOOT;
		nmsg_playershoot2.bShoot = bShoot;
		TNetManager.GetInstance().BroadcastMessage(nmsg_playershoot2);
	}

	public void PlayerSkill(int nSkillID, int nTargetUID)
	{
		nmsg_playerskill nmsg_playerskill2 = new nmsg_playerskill();
		nmsg_playerskill2.msghead = kGameNetEnum.PLAYER_SKILL;
		nmsg_playerskill2.m_nSkillID = nSkillID;
		nmsg_playerskill2.m_nTargetUID = nTargetUID;
		TNetManager.GetInstance().BroadcastMessage(nmsg_playerskill2);
	}

	public void PlayerBeatBack(int nPlayerUID, Vector3 v3Dst)
	{
		nmsg_playerbeatback nmsg_playerbeatback2 = new nmsg_playerbeatback();
		nmsg_playerbeatback2.msghead = kGameNetEnum.PLAYER_BEATBACK;
		nmsg_playerbeatback2.m_nPlayerUID = nPlayerUID;
		nmsg_playerbeatback2.m_v3Dst = v3Dst;
		TNetManager.GetInstance().BroadcastMessage(nmsg_playerbeatback2);
	}

	public void PlayerLevelUp(int nLevel)
	{
		nmsg_playerlevelup nmsg_playerlevelup2 = new nmsg_playerlevelup();
		nmsg_playerlevelup2.msghead = kGameNetEnum.PLAYER_LEVELUP;
		nmsg_playerlevelup2.m_nLevel = nLevel;
		TNetManager.GetInstance().BroadcastMessage(nmsg_playerlevelup2);
	}

	public void PlayerTakeItem(int nItemUID)
	{
		nmsg_playertakeitem nmsg_playertakeitem2 = new nmsg_playertakeitem();
		nmsg_playertakeitem2.msghead = kGameNetEnum.PLAYER_TAKEITEM;
		nmsg_playertakeitem2.nItemUID = nItemUID;
		TNetManager.GetInstance().BroadcastMessage(nmsg_playertakeitem2);
	}

	public void MGManagerAddMob(int nMobID, int nMobUID, int nWaveID, int nSequence, Vector3 v3Pos, Vector3 v3Dir)
	{
		nmsg_mgmanager_addmob nmsg_mgmanager_addmob2 = new nmsg_mgmanager_addmob();
		nmsg_mgmanager_addmob2.msghead = kGameNetEnum.MGMANAGER_ADDMOB;
		nmsg_mgmanager_addmob2.m_nMobID = nMobID;
		nmsg_mgmanager_addmob2.m_nMobUID = nMobUID;
		nmsg_mgmanager_addmob2.m_nWaveID = nWaveID;
		nmsg_mgmanager_addmob2.m_nSequence = nSequence;
		nmsg_mgmanager_addmob2.m_v3Pos = v3Pos;
		nmsg_mgmanager_addmob2.m_v3Dir = v3Dir;
		TNetManager.GetInstance().BroadcastMessage(nmsg_mgmanager_addmob2);
	}

	public void BattleDamageMob(int nMobUID, List<float> ltDamageInfo)
	{
		nmsg_battle_damage_mob nmsg_battle_damage_mob2 = new nmsg_battle_damage_mob();
		nmsg_battle_damage_mob2.msghead = kGameNetEnum.BATTLE_DAMAGE_MOB;
		nmsg_battle_damage_mob2.m_nMobUID = nMobUID;
		for (int i = 0; i < ltDamageInfo.Count; i++)
		{
			nmsg_battle_damage_mob2.m_fDamage += ltDamageInfo[i];
		}
		TNetManager.GetInstance().BroadcastMessage(nmsg_battle_damage_mob2);
	}

	public void BattleDamagePlayer(List<float> ltDamageInfo)
	{
		nmsg_battle_damage_player nmsg_battle_damage_player2 = new nmsg_battle_damage_player();
		nmsg_battle_damage_player2.msghead = kGameNetEnum.BATTLE_DAMAGE_PLAYER;
		for (int i = 0; i < ltDamageInfo.Count; i++)
		{
			nmsg_battle_damage_player2.m_fDamage += ltDamageInfo[i];
		}
		TNetManager.GetInstance().BroadcastMessage(nmsg_battle_damage_player2);
	}

	public void MonsterDead(int nMobUID, kDeadMode mode)
	{
		nmsg_monster_dead nmsg_monster_dead2 = new nmsg_monster_dead();
		nmsg_monster_dead2.msghead = kGameNetEnum.MOB_DEAD;
		nmsg_monster_dead2.m_nMobUID = nMobUID;
		nmsg_monster_dead2.m_DeadMode = mode;
		TNetManager.GetInstance().BroadcastMessage(nmsg_monster_dead2);
	}

	public void MonsterHurt(int nMobUID, kAnimEnum anim)
	{
		nmsg_monster_hurt nmsg_monster_hurt2 = new nmsg_monster_hurt();
		nmsg_monster_hurt2.msghead = kGameNetEnum.MOB_HURT;
		nmsg_monster_hurt2.m_nMobUID = nMobUID;
		nmsg_monster_hurt2.m_HurtAnim = anim;
		TNetManager.GetInstance().BroadcastMessage(nmsg_monster_hurt2);
	}

	public void MonsterMove(int nMobUID, Vector3 v3Dst)
	{
		nmsg_monster_move nmsg_monster_move2 = new nmsg_monster_move();
		nmsg_monster_move2.msghead = kGameNetEnum.MOB_MOVE;
		nmsg_monster_move2.m_nMobUID = nMobUID;
		nmsg_monster_move2.m_v3Dst = v3Dst;
		TNetManager.GetInstance().BroadcastMessage(nmsg_monster_move2);
	}

	public void MonsterAttack(int nMobUID, int nTargetUID, int nComboSkillID)
	{
		nmsg_monster_attack nmsg_monster_attack2 = new nmsg_monster_attack();
		nmsg_monster_attack2.msghead = kGameNetEnum.MOB_ATTACK;
		nmsg_monster_attack2.m_nMobUID = nMobUID;
		nmsg_monster_attack2.m_nTargetUID = nTargetUID;
		nmsg_monster_attack2.m_nComboSkillID = nComboSkillID;
		TNetManager.GetInstance().BroadcastMessage(nmsg_monster_attack2);
	}

	public void MonsterBeatback(int nMobUID, Vector3 v3Dst)
	{
		nmsg_monster_beatback nmsg_monster_beatback2 = new nmsg_monster_beatback();
		nmsg_monster_beatback2.msghead = kGameNetEnum.MOB_BEATBACK;
		nmsg_monster_beatback2.m_nMobUID = nMobUID;
		nmsg_monster_beatback2.m_v3Dst = v3Dst;
		TNetManager.GetInstance().BroadcastMessage(nmsg_monster_beatback2);
	}

	public void TaskCompelete(bool isSuccess)
	{
		nmsg_task_compelete nmsg_task_compelete2 = new nmsg_task_compelete();
		nmsg_task_compelete2.msghead = kGameNetEnum.TASK_COMPELETE;
		nmsg_task_compelete2.m_isSuccess = isSuccess;
		TNetManager.GetInstance().BroadcastMessage(nmsg_task_compelete2);
	}

	public void TaskGetItem(int nItemID)
	{
		nmsg_task_getitem nmsg_task_getitem2 = new nmsg_task_getitem();
		nmsg_task_getitem2.msghead = kGameNetEnum.TASK_GET_ITEM;
		nmsg_task_getitem2.m_nItemID = nItemID;
		TNetManager.GetInstance().BroadcastMessage(nmsg_task_getitem2);
	}

	public void TaskTimeSync(float fGameTime)
	{
		nmsg_task_timesync nmsg_task_timesync2 = new nmsg_task_timesync();
		nmsg_task_timesync2.msghead = kGameNetEnum.TASK_TIME_SYNC;
		nmsg_task_timesync2.m_fGameTime = fGameTime;
		TNetManager.GetInstance().BroadcastMessage(nmsg_task_timesync2);
	}
}
