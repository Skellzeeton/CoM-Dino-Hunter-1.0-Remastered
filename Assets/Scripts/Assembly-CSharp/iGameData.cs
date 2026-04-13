using System;
using BehaviorTree;

public class iGameData
{
	protected iWeaponCenter m_WeaponCenter;

	protected iBuffCenter m_BuffCenter;

	protected iSkillCenter m_SkillCenter;

	protected iMobCenter m_MobCenter;

	protected iMGCenter m_MGCenter;

	protected iGameLevelCenter m_GameLevelCenter;

	protected iBehaviorCenter m_BehaviorCenter;

	protected iAICenter m_AICenter;

	protected iTaskCenter m_TaskCenter;

	protected iItemCenter m_ItemCenter;
	
	protected iCharacterCenter m_CharacterCenter;
	
	protected iDropGroupCenter m_DropGroupCenter;

	protected iLoadTipCenter m_LoadTipCenter;

	protected iStashCapacityCenter m_StashCapacityCenter;

	protected iDataCenter m_DataCenter;

	public iGameData()
	{
		m_WeaponCenter = new iWeaponCenter();
		m_BuffCenter = new iBuffCenter();
		m_SkillCenter = new iSkillCenter();
		m_MobCenter = new iMobCenter();
		m_MGCenter = new iMGCenter();
		m_GameLevelCenter = new iGameLevelCenter();
		m_BehaviorCenter = new iBehaviorCenter();
		m_AICenter = new iAICenter();
		m_TaskCenter = new iTaskCenter();
		m_ItemCenter = new iItemCenter();
		m_CharacterCenter = new iCharacterCenter();
		m_DropGroupCenter = new iDropGroupCenter();
		m_LoadTipCenter = new iLoadTipCenter();
		m_StashCapacityCenter = new iStashCapacityCenter();
		m_DataCenter = new iDataCenter();
	}

	public bool Load()
	{
		try
		{
			iGameApp.GetInstance().ScreenLog("Loading m_WeaponCenter");
			m_WeaponCenter.Load();
			iGameApp.GetInstance().ScreenLog("Loading m_BuffCenter");
			m_BuffCenter.Load();
			iGameApp.GetInstance().ScreenLog("Loading m_SkillCenter Monster");
			m_SkillCenter.Load_Monster();
			iGameApp.GetInstance().ScreenLog("Loading m_SkillCenter Player");
			m_SkillCenter.Load_Player();
			iGameApp.GetInstance().ScreenLog("Loading m_MobCenter");
			m_MobCenter.Load();
			iGameApp.GetInstance().ScreenLog("Loading m_MGCenter");
			m_MGCenter.Load();
			iGameApp.GetInstance().ScreenLog("Loading m_GameLevelCenter");
			m_GameLevelCenter.Load();
			iGameApp.GetInstance().ScreenLog("Loading m_BehaviorCenter");
			m_BehaviorCenter.Load();
			iGameApp.GetInstance().ScreenLog("Loading m_AICenter");
			m_AICenter.Load();
			iGameApp.GetInstance().ScreenLog("Loading m_TaskCenter");
			m_TaskCenter.Load();
			iGameApp.GetInstance().ScreenLog("Loading m_ItemCenter");
			m_ItemCenter.Load();
			iGameApp.GetInstance().ScreenLog("Loading m_CharacterCenter");
			m_CharacterCenter.Load();
			iGameApp.GetInstance().ScreenLog("Loading m_DropGroupCenter");
			m_DropGroupCenter.Load();
			iGameApp.GetInstance().ScreenLog("Loading m_LoadTipCenter");
			m_LoadTipCenter.Load();
			iGameApp.GetInstance().ScreenLog("Loading m_StashCapacityCenter");
			m_StashCapacityCenter.Load();
			iGameApp.GetInstance().ScreenLog("Loading m_DataCenter");
			m_DataCenter.Load();
		}
		catch (Exception ex)
		{
			iGameApp.GetInstance().ScreenLog("error msg: " + ex.Message);
		}
		iGameApp.GetInstance().ClearScreenLog();
		return true;
	}

	public iTaskCenter GetTaskCenter()
	{
		return m_TaskCenter;
	}

	public iDataCenter GetDataCenter()
	{
		return m_DataCenter;
	}

	public iCharacterCenter GetCharacterCenter()
	{
		return m_CharacterCenter;
	}

	public iWeaponCenter GetWeaponCenter()
	{
		return m_WeaponCenter;
	}

	public iSkillCenter GetSkillCenter()
	{
		return m_SkillCenter;
	}

	public iItemCenter GetItemCenter()
	{
		return m_ItemCenter;
	}
	

	public iGameLevelCenter GetGameLevelCenter()
	{
		return m_GameLevelCenter;
	}

	public iStashCapacityCenter GetStashCapacityCenter()
	{
		return m_StashCapacityCenter;
	}

	public CCharacterInfo GetCharacterInfo(int nID)
	{
		if (m_CharacterCenter == null)
		{
			return null;
		}
		return m_CharacterCenter.Get(nID);
	}

	public CCharacterInfoLevel GetCharacterInfo(int nID, int nLevel)
	{
		if (m_CharacterCenter == null)
		{
			return null;
		}
		return m_CharacterCenter.Get(nID, nLevel);
	}

	public CWeaponInfo GetWeaponInfo(int nID)
	{
		if (m_WeaponCenter == null)
		{
			return null;
		}
		return m_WeaponCenter.Get(nID);
	}

	public CWeaponInfoLevel GetWeaponInfo(int nID, int nLevel)
	{
		if (m_WeaponCenter == null)
		{
			return null;
		}
		return m_WeaponCenter.Get(nID, nLevel);
	}

	public CBuffInfo GetBuffInfo(int nID)
	{
		if (m_BuffCenter == null)
		{
			return null;
		}
		return m_BuffCenter.GetBuffInfo(nID);
	}

	public CSkillInfo GetSkillInfo(int nID)
	{
		if (m_SkillCenter == null)
		{
			return null;
		}
		return m_SkillCenter.GetSkillInfo(nID);
	}

	public CSkillInfoLevel GetSkillInfo(int nID, int nLevel)
	{
		if (m_SkillCenter == null)
		{
			return null;
		}
		return m_SkillCenter.GetSkillInfo(nID, nLevel);
	}

	public CSkillComboInfo GetSkillComboInfo(int nComboID)
	{
		if (m_SkillCenter == null)
		{
			return null;
		}
		return m_SkillCenter.GetSkillComboInfo(nComboID);
	}

	public CMobInfoLevel GetMobInfo(int nID, int nLevel)
	{
		if (m_MobCenter == null)
		{
			return null;
		}
		return m_MobCenter.Get(nID, nLevel);
	}

	public Node GetBehavior(int nID)
	{
		if (m_BehaviorCenter == null)
		{
			return null;
		}
		return m_BehaviorCenter.GetBehavior(nID);
	}

	public CAIInfo GetAIInfo(int nID)
	{
		if (m_AICenter == null)
		{
			return null;
		}
		return m_AICenter.GetAIInfo(nID);
	}

	public CAIManagerInfo GetAIManagerInfo(int nID)
	{
		if (m_AICenter == null)
		{
			return null;
		}
		return m_AICenter.GetAIManagerInfo(nID);
	}

	public WaveInfo GetWaveInfo(int nID)
	{
		if (m_MGCenter == null)
		{
			return null;
		}
		return m_MGCenter.Get(nID);
	}

	public GameLevelInfo GetGameLevelInfo(int nID)
	{
		if (m_GameLevelCenter == null)
		{
			return null;
		}
		return m_GameLevelCenter.Get(nID);
	}

	public CTaskInfo GetTaskInfo(int nID)
	{
		if (m_TaskCenter == null)
		{
			return null;
		}
		return m_TaskCenter.Get(nID);
	}

	public CItemInfo GetItemInfo(int nID)
	{
		if (m_ItemCenter == null)
		{
			return null;
		}
		return m_ItemCenter.Get(nID);
	}

	public CItemInfoLevel GetItemInfo(int nID, int nLevel)
	{
		if (m_ItemCenter == null)
		{
			return null;
		}
		return m_ItemCenter.Get(nID, nLevel);
	}

	public CDropGroupInfo GetDropGrouInfo(int nID)
	{
		if (m_DropGroupCenter == null)
		{
			return null;
		}
		return m_DropGroupCenter.Get(nID);
	}

	public CLoadTipInfo GetLoadTipInfoRandom()
	{
		if (m_LoadTipCenter == null)
		{
			return null;
		}
		return m_LoadTipCenter.GetRandom();
	}

	public CStashCapacity GetStashCapacity(int nLevel)
	{
		if (m_StashCapacityCenter == null)
		{
			return null;
		}
		return m_StashCapacityCenter.Get(nLevel);
	}

	public int GetStashCapacityCount(int nLevel)
	{
		if (m_StashCapacityCenter == null)
		{
			return 0;
		}
		return m_StashCapacityCenter.GetCapacity(nLevel);
	}
}
