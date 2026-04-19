using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorTree;
using UnityEngine;
using gyAchievementSystem;
using gyEvent;
using gyTaskSystem;

public class CCharMob : CCharBase
{
	[SerializeField]
	public int MobBehavior;

	public int GenerateWaveID;

	public int GenerateSequence;

	protected CMobInfoLevel m_curMobInfoLevel;

	protected float m_fHardinessCur;

	protected float m_fHardinessMax;

	protected float m_fUpdateMoveTime;

	protected List<SkillComboUserInfo> m_ltSkillList;

	protected kMobBehaviour m_MobBehaviourMode;

	protected int m_nCurAIID;

	protected List<SkillComboUserInfo> m_ltSkillListAI;

	protected List<int> m_ltSkillPassiveAI;

	protected gyLifeBarHUD m_LifeBar;

	protected Dictionary<int, Vector3> m_dictAssistAim;

	public CSkillComboInfo m_pSkillComboInfo;

	public int m_nCurComboIndex;

	[NonSerialized]
	public bool m_bHasPurposePoint;

	[NonSerialized]
	public Vector3 m_v3PurposePoint;

	[NonSerialized]
	public List<Vector3> m_ltPath;

	[NonSerialized]
	public int m_nDstHoverIndex;

	[NonSerialized]
	public Vector3 m_v3DstHoverPoint;

	[NonSerialized]
	public float m_fHoverTime;

	[NonSerialized]
	public kDeadMode m_DeadMode;

	public float m_fDeadDistance;

	public Vector3 m_v3DeadDirection;

	[NonSerialized]
	public Vector3 m_v3BirthPos;

	[NonSerialized]
	public bool m_bShowTime;

	[NonSerialized]
	public bool m_bFreeze;

	[NonSerialized]
	public float m_fFreezeTime;

	public iBuilding m_TargetBuilding;

	protected CDropGroupInfo m_tmpDropGroupInfo;

	[SerializeField]
	public int MobType { get; set; }

	public kMobBehaviour MobBehaviourMode
	{
		get
		{
			return m_MobBehaviourMode;
		}
		set
		{
			m_MobBehaviourMode = value;
		}
	}

	public float HP
	{
		get
		{
			return m_fHP;
		}
	}

	public float HPMAX
	{
		get
		{
			return m_fHPMax;
		}
	}

	public float Hardiness
	{
		get
		{
			return m_fHardinessCur;
		}
	}

	public float HardinessMax
	{
		get
		{
			return m_fHardinessMax;
		}
	}

	public new void Awake()
	{
		base.Awake();
		m_nType = kCharType.Mob;
		base.CampType = kCampType.Monster;
		m_Property = new CProMob();
		m_nCurAIID = -1;
		m_ltSkillListAI = new List<SkillComboUserInfo>();
		m_ltSkillList = new List<SkillComboUserInfo>();
		m_ltSkillPassive = new List<int>();
		m_ltSkillPassiveAI = new List<int>();
		m_MobBehaviourMode = kMobBehaviour.None;
		m_ltPath = new List<Vector3>();
		m_nDstHoverIndex = -1;
		m_bShowTime = false;
		m_DeadMode = kDeadMode.None;
		m_dictAssistAim = new Dictionary<int, Vector3>();
		m_tmpDropGroupInfo = new CDropGroupInfo();
	}

	public new void Start()
	{
		base.Start();
	}

	public new void Update()
	{
		if (!m_bActive)
		{
			return;
		}
		base.Update();
		float num = Time.deltaTime * m_fTimeScale;
		if (m_Behavior != null)
		{
			m_Behavior.Update(this, num);
		}
		if (m_GameState.isNetGame)
		{
			m_fUpdateMoveTime -= num;
			if (m_fUpdateMoveTime <= 0f)
			{
				m_fUpdateMoveTime = 0.1f;
			}
		}
		foreach (SkillComboUserInfo ltSkill in m_ltSkillList)
		{
			ltSkill.Update(num);
		}
		foreach (SkillComboUserInfo item in m_ltSkillListAI)
		{
			item.Update(num);
		}
	}

	public new void LateUpdate()
	{
		if (m_bActive)
		{
			base.LateUpdate();
		}
	}

	public override void Destroy()
	{
		if (m_LifeBar != null && m_GameScene != null)
		{
			iGameUIBase gameUI = m_GameScene.GetGameUI();
			if (gameUI != null)
			{
				gameUI.RemoveLifeBar(base.UID);
			}
			m_LifeBar = null;
		}
		base.Destroy();
	}

	public CMobInfoLevel GetMobInfo()
	{
		return m_curMobInfoLevel;
	}

	public override void InitAnimData()
	{
		m_AnimData.Add(new CAnimInfo(kAnimEnum.Idle, "idle"));
		m_AnimData.Add(new CAnimInfo(kAnimEnum.MoveForward, "fly"));
		m_AnimData.Add(new CAnimInfo(kAnimEnum.Mob_Attack, "attack01"));
		m_AnimData.Add(new CAnimInfo(kAnimEnum.Mob_Dead, "death"));
		m_AnimData.Add(new CAnimInfo(kAnimEnum.Mob_Hurt, "damage"));
	}

	public virtual bool IsMobCanThink()
	{
		return !base.isDead;
	}

	public virtual bool AddHardiness(float fDamage, string sBoneName = "")
	{
		m_fHardinessCur += fDamage;
		if (m_fHardinessCur > 0f)
		{
			return false;
		}
		m_fHardinessCur = m_fHardinessMax;
		m_HurtAnim = kAnimEnum.Mob_Hurt;
		return true;
	}

	public override void AddHP(float fHP)
	{
		m_fHP += fHP;
		if (m_fHP > m_fHPMax)
		{
			m_fHP = m_fHPMax;
		}
		else if (m_fHP <= 0f)
		{
			m_fHP = 0f;
		}
		if (m_LifeBar != null)
		{
			m_LifeBar.SetLife(m_fHP / m_fHPMax);
		}
	}

	public override void OnDead(kDeadMode nDeathMode)
	{
		base.isDead = true;
		m_DeadMode = nDeathMode;
		ResetAI();
		if (m_curMobInfoLevel != null && m_tmpDropGroupInfo != null)
		{
			int dropItemCount = m_curMobInfoLevel.GetDropItemCount();
			if (dropItemCount > 0)
			{
				for (int i = 0; i < dropItemCount; i++)
				{
					int dropItem = m_tmpDropGroupInfo.GetDropItem();
					if (dropItem > 0)
					{
						CUISound.GetInstance().Play("UI_Material_appear");
						Vector3 onUnitSphere = UnityEngine.Random.onUnitSphere;
						onUnitSphere.y = 1f;
						m_GameScene.AddItem(dropItem, GetBone(0).position, onUnitSphere * UnityEngine.Random.Range(300f, 500f), -1f);
					}
				}
			}
			int crystalDropCount = m_curMobInfoLevel.GetCrystalDropCount();
			if (crystalDropCount > 0)
			{
				CDropGroupInfo crystalGroup = m_GameData.GetDropGrouInfo(1);
				if (crystalGroup != null && crystalGroup.ltItem != null && crystalGroup.ltItem.Count > 0)
				{
					CDropGroupInfo tmpCrystalDrop = new CDropGroupInfo();
					for (int ci = 0; ci < crystalGroup.ltItem.Count; ci++)
					{
						tmpCrystalDrop.Add(new CDropItem(
							crystalGroup.ltItem[ci].nItemID,
							crystalGroup.ltItem[ci].fRate));
					}
					for (int ci = 0; ci < crystalDropCount; ci++)
					{
						int crystalItem = tmpCrystalDrop.GetDropItem();
						if (crystalItem > 0)
						{
							CUISound.GetInstance().Play("UI_Crystal_appear");
							Vector3 onUnitSphere = UnityEngine.Random.onUnitSphere;
							onUnitSphere.y = 1f;
							m_GameScene.AddItem(
								crystalItem,
								GetBone(0).position,
								onUnitSphere * UnityEngine.Random.Range(300f, 500f),
								-1f);
						}
					}
				}
			}
			if (UnityEngine.Random.value <= (m_curMobInfoLevel.fGoldRate / 100f))
			{
				GameObject poolObject = PrefabManager.GetPoolObject(302, 0f);
				if (poolObject != null)
				{
					iGoldEmitter component = poolObject.GetComponent<iGoldEmitter>();
					if (component != null)
					{
						component.Initialize(m_curMobInfoLevel.nGold);
						component.transform.position = GetBone(0).position;
					}
				}
			}
		}
		m_GameScene.AddMonsterNumLimit(base.ID, MobType, MobBehavior, -1);
		m_GameScene.AddWaveMobNumber(GenerateWaveID, -1);
		if (m_GameScene.m_MGManager != null)
		{
			CEventManager eventManager = m_GameScene.m_MGManager.GetEventManager();
			if (eventManager != null)
			{
				eventManager.Trigger(new EventCondition_MobByWave(GenerateWaveID, GenerateSequence, 1));
				eventManager.Trigger(new EventCondition_MobByID(base.ID, 1));
				int generateWaveID = GenerateWaveID;
				if (!m_GameScene.m_MGManager.IsWaveProcess(generateWaveID))
				{
					int waveMobNumber = m_GameScene.GetWaveMobNumber(GenerateWaveID);
					eventManager.Trigger(new EventCondition_WaveNumberLeft(generateWaveID, waveMobNumber));
					if (m_GameScene.m_TaskManager != null)
					{
						CTaskBase task = m_GameScene.m_TaskManager.GetTask();
						if (task != null)
						{
							CTaskInfo taskInfo = task.GetTaskInfo();
							if (taskInfo != null && taskInfo.nType == 3 && waveMobNumber == 0)
							{
								CAchievementManager.GetInstance().AddAchievement(8);
							}
						}
					}
				}
			}
		}
		if (m_GameScene.m_TaskManager != null)
		{
			m_GameScene.m_TaskManager.OnKillMonster(base.ID);
			if (IsBoss())
			{
				m_GameState.LastKillBoss = base.UID;
			}
			if (m_GameScene.m_MGManager != null && m_GameScene.m_MGManager.IsWaveCompleted() && m_GameScene.GetMobAliveCount() == 0)
			{
				m_GameScene.m_TaskManager.OnKillAllMonsters();
			}
		}
		CAchievementManager.GetInstance().AddAchievement(5);
		if (m_curMobInfoLevel != null)
		{
			CAchievementManager.GetInstance().AddAchievement(6, new object[2] { m_curMobInfoLevel.nRareType, m_curMobInfoLevel.nType });
		}
	}

	public int GetSkillNum()
	{
		if (m_ltSkillList == null)
		{
			return 0;
		}
		return m_ltSkillList.Count;
	}

	public SkillComboUserInfo GetSkill(int nIndex)
	{
		if (nIndex < 0 || nIndex >= m_ltSkillList.Count)
		{
			return null;
		}
		return m_ltSkillList[nIndex];
	}

	public IEnumerable GetSkillEnumerator()
	{
		foreach (SkillComboUserInfo ltSkill in m_ltSkillList)
		{
			yield return ltSkill;
		}
	}

	public int GetAISkillNum()
	{
		if (m_ltSkillListAI == null)
		{
			return 0;
		}
		return m_ltSkillListAI.Count;
	}

	public SkillComboUserInfo GetAISkill(int nIndex)
	{
		if (nIndex < 0 || nIndex >= m_ltSkillList.Count)
		{
			return null;
		}
		return m_ltSkillListAI[nIndex];
	}

	public IEnumerable GetAISkillEnumerator()
	{
		foreach (SkillComboUserInfo item in m_ltSkillListAI)
		{
			yield return item;
		}
	}

	public override bool GetSkillPassiveList(ref List<int> ltSkillPassive)
	{
		bool result = base.GetSkillPassiveList(ref ltSkillPassive);
		if (m_ltSkillPassiveAI != null)
		{
			result = true;
			foreach (int item in m_ltSkillPassiveAI)
			{
				ltSkillPassive.Add(item);
			}
		}
		return result;
	}

	public void SetBehaviourMode(kMobBehaviour mode)
	{
		MobBehaviourMode = mode;
	}

	public override bool OnHit(float fDmg, CWeaponInfoLevel pWeaponLvlInfo = null, string sBodyPart = "")
	{
		if (!base.OnHit(fDmg, pWeaponLvlInfo, sBodyPart))
		{
			return false;
		}
		if (!m_GameScene.IsRoomMaster())
		{
			return false;
		}
		if (m_fHP <= 0f)
		{
			kDeadMode kDeadMode2 = kDeadMode.Normal;
			if (pWeaponLvlInfo != null && !IsBoss())
			{
				switch (pWeaponLvlInfo.nAttackMode)
				{
				case 1:
				case 3:
				case 4:
				case 6:
					kDeadMode2 = kDeadMode.HitFly;
					m_fDeadDistance = 10f;
					break;
				}
			}
			if (kDeadMode2 == kDeadMode.None && MobType == 1 && !(m_curTask is doUseSkillTask) && !(m_curTask is doHurtTask))
			{
				kDeadMode2 = kDeadMode.FlyDead;
			}
			OnDead(kDeadMode2);
			CGameNetSender.GetInstance().MonsterDead(base.UID, kDeadMode2);
		}
		else if (IsCanBeatHardniess() && AddHardiness(fDmg, sBodyPart))
		{
			m_bHurting = true;
			ResetAI();
			CGameNetSender.GetInstance().MonsterHurt(base.UID, m_HurtAnim);
		}
		return true;
	}

	public virtual void InitMob(int nMobID, int nMobLevel)
	{
		base.ID = nMobID;
		base.Level = nMobLevel;
		m_curMobInfoLevel = m_GameData.GetMobInfo(nMobID, nMobLevel);
		if (m_curMobInfoLevel == null)
		{
			return;
		}
		InitAnimData();
		InitAudioData();
		m_ltSkillList.Clear();
		if (m_curMobInfoLevel.ltSkill != null)
		{
			for (int i = 0; i < m_curMobInfoLevel.ltSkill.Count; i++)
			{
				int nID = m_curMobInfoLevel.ltSkill[i].m_nID;
				CSkillComboInfo skillComboInfo = m_GameData.GetSkillComboInfo(nID);
				if (skillComboInfo != null)
				{
					SkillComboUserInfo item = new SkillComboUserInfo(nID, m_curMobInfoLevel.ltSkill[i].m_fRate, skillComboInfo.fCoolDown);
					m_ltSkillList.Add(item);
				}
			}
		}
		m_ltSkillPassive.Clear();
		if (m_curMobInfoLevel.ltSkillPassive != null)
		{
			for (int j = 0; j < m_curMobInfoLevel.ltSkillPassive.Count; j++)
			{
				int num = m_curMobInfoLevel.ltSkillPassive[j];
				CSkillInfoLevel skillInfo = m_GameData.GetSkillInfo(num, 1);
				if (skillInfo != null && skillInfo.nType == 1)
				{
					m_ltSkillPassive.Add(num);
				}
			}
		}
		if (m_tmpDropGroupInfo == null)
		{
			m_tmpDropGroupInfo = new CDropGroupInfo();
		}
		if (m_tmpDropGroupInfo != null)
		{
			m_tmpDropGroupInfo.Clear();
			if (m_curMobInfoLevel != null)
			{
				CDropGroupInfo dropGrouInfo = m_GameData.GetDropGrouInfo(m_curMobInfoLevel.nDropGroup);
				if (dropGrouInfo != null && dropGrouInfo.ltItem != null)
				{
					for (int k = 0; k < dropGrouInfo.ltItem.Count; k++)
					{
						m_tmpDropGroupInfo.Add(new CDropItem(dropGrouInfo.ltItem[k].nItemID, dropGrouInfo.ltItem[k].fRate));
					}
				}
			}
		}
		m_Property.Initialize(base.ID, base.Level);
		m_Property.UpdateSkill(this);
		m_fHPMax = m_Property.GetValue(kProEnum.HPMax);
		m_fHP = m_fHPMax;
		InitHardiness(base.ID, base.Level);
		iGameUIBase gameUI = m_GameScene.GetGameUI();
		if (gameUI != null)
		{
			m_LifeBar = gameUI.CreateLifeBar(this);
		}
		InitAssistAimInfo();
	}

	public virtual void InitHardiness(int nMobID, int nMobLevel)
	{
		CMobInfoLevel mobInfo = m_GameData.GetMobInfo(nMobID, nMobLevel);
		if (mobInfo != null)
		{
			m_fHardinessMax = mobInfo.fHardiness;
			m_fHardinessCur = m_fHardinessMax;
		}
	}

	public virtual void InitAI(int nAIManager)
	{
		CAIManagerInfo aIManagerInfo = m_GameData.GetAIManagerInfo(nAIManager);
		if (aIManagerInfo != null)
		{
			SetAI(aIManagerInfo.nAI);
		}
	}

	public void SetAI(int nAI)
	{
		CAIInfo aIInfo = m_GameData.GetAIInfo(nAI);
		if (aIInfo == null)
		{
			return;
		}
		int nCurAIID = m_nCurAIID;
		m_nCurAIID = nAI;
		m_ltSkillListAI.Clear();
		foreach (SkillComboRateInfo item2 in aIInfo.ltSkill)
		{
			CSkillComboInfo skillComboInfo = m_GameData.GetSkillComboInfo(item2.m_nID);
			if (skillComboInfo != null)
			{
				SkillComboUserInfo item = new SkillComboUserInfo(item2.m_nID, item2.m_fRate, skillComboInfo.fCoolDown);
				m_ltSkillListAI.Add(item);
			}
		}
		m_ltSkillPassiveAI.Clear();
		foreach (int item3 in aIInfo.ltSkillPassive)
		{
			m_ltSkillPassiveAI.Add(item3);
		}
		m_Property.UpdateSkill(this);
		Node node = null;
		node = ((!m_GameScene.IsRoomMaster()) ? m_GameData.GetBehavior(aIInfo.nBehavior + 100) : m_GameData.GetBehavior(aIInfo.nBehavior));
		if (node != null)
		{
			if (m_Behavior == null)
			{
				m_Behavior = new Behavior();
			}
			if (m_Behavior.HasInstalled())
			{
				m_Behavior.Uninstall();
			}
			m_Behavior.Install(node);
			OnEnterAI(nCurAIID, nAI);
		}
	}

	protected virtual void OnEnterAI(int nLastAI, int nAI)
	{
	}

	public void SetLifeBarParam(float fHoldTime, float fFadeTime = 0.5f)
	{
		if (!(m_LifeBar == null))
		{
			m_LifeBar.SetTime(fHoldTime, fFadeTime);
		}
	}

	public bool IsCanBeatHardniess()
	{
		if (m_bHurting)
		{
			return false;
		}
		return true;
	}

	public void SetFreeze(bool bFreeze, float fTime = 0f)
	{
		if (ID == 1303 || ID == 903 || ID == 1304 || ID == 1403 || ID == 1202 || ID == 1203 || ID == 1404 || ID == 304) 
		{
			return;
		}
		m_bFreeze = bFreeze;
		m_fFreezeTime = fTime;
		ResetAI();
	}

	public virtual void InitAssistAimInfo()
	{
		m_dictAssistAim.Add(1, Vector2.zero);
		m_dictAssistAim.Add(2, Vector2.zero);
	}

	public bool CheckAssistAimInfo(ref CAssistAimInfo assistaiminfo)
	{
		if (base.isDead)
		{
			return false;
		}
		assistaiminfo.m_Target = this;
		foreach (int key in m_dictAssistAim.Keys)
		{
			assistaiminfo.m_ltBone.Add(GetBone(key));
		}
		return true;
	}

	public void MoveTo(Vector3 v3Dst)
	{
		Vector3 sourcePosition = base.Pos;
		if (m_ltPath.Count > 0)
		{
			sourcePosition = m_ltPath[m_ltPath.Count - 1];
		}
		UnityEngine.AI.NavMeshPath navMeshPath = new UnityEngine.AI.NavMeshPath();
		if (!UnityEngine.AI.NavMesh.CalculatePath(sourcePosition, v3Dst, -1, navMeshPath))
		{
			m_ltPath.Add(v3Dst);
			return;
		}
		m_ltPath.Clear();
		for (int i = 0; i < navMeshPath.corners.Length; i++)
		{
			m_ltPath.Add(navMeshPath.corners[i]);
		}
	}
}
