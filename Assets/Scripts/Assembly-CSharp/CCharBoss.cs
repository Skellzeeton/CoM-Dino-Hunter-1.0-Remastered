using System.Collections.Generic;
using UnityEngine;
using gyEvent;

public class CCharBoss : CCharMob
{
	public class CBodyPart
	{
		public float m_fHardinessCur;

		public float m_fHardinessMax;

		public List<kAnimEnum> m_ltAnim;

		public float m_fDmgRate;

		public kAnimEnum GetAnimRadnom()
		{
			if (m_ltAnim == null || m_ltAnim.Count < 1)
			{
				return kAnimEnum.None;
			}
			return m_ltAnim[Random.Range(0, m_ltAnim.Count)];
		}
	}

	protected Dictionary<int, CBodyPart> m_dictBodyPart;

	protected CAIManagerInfo m_curAIManager;

	protected List<CAITriggerInfo> m_curTriggerList;

	protected List<CAITriggerInfo> m_tmpTriggerList;

	protected CAITriggerInfo m_curTrigger;

	protected float m_fLifeTime;

	protected List<GameObject> m_ltBodyEffect;

	public Dictionary<int, CBodyPart> GetBodyPart()
	{
		return m_dictBodyPart;
	}

	public new void Awake()
	{
		base.Awake();
		m_nType = kCharType.Boss;
		m_dictBodyPart = new Dictionary<int, CBodyPart>();
		m_curAIManager = null;
		m_curTriggerList = new List<CAITriggerInfo>();
		m_tmpTriggerList = new List<CAITriggerInfo>();
		m_curTrigger = null;
		m_fLifeTime = 0f;
		m_bShowTime = false;
		m_ltBodyEffect = new List<GameObject>();
	}

	public new void Start()
	{
		base.Start();
	}

	public new void Update()
	{
		base.Update();
		m_fLifeTime += Time.deltaTime;
		UpdateAITrigger(Time.deltaTime);
	}

	public override void InitMob(int nMobID, int nMobLevel)
	{
		base.InitMob(nMobID, nMobLevel);
		if (m_GameScene.CurGameLevelInfo != null && m_GameScene.CurGameLevelInfo.sSceneName == "SceneSnow")
		{
			iStepEffectLeft component = m_ModelEntity.GetComponent<iStepEffectLeft>();
			if (component != null)
			{
				component.nPrefabID = 1914;
			}
			iStepEffectRight component2 = m_ModelEntity.GetComponent<iStepEffectRight>();
			if (component2 != null)
			{
				component2.nPrefabID = 1914;
			}
		}
	}

	public override void Destroy()
	{
		ClearBodyEffect();
		base.Destroy();
	}

	public override void InitHardiness(int nMobID, int nMobLevel)
	{
		CMobInfoLevel mobInfo = m_GameData.GetMobInfo(nMobID, nMobLevel);
		if (mobInfo == null || mobInfo.ltHardinessInfo == null)
		{
			return;
		}
		foreach (CHardinessInfo item in mobInfo.ltHardinessInfo)
		{
			CBodyPart cBodyPart = new CBodyPart();
			cBodyPart.m_fHardinessMax = item.fHardiness;
			cBodyPart.m_fHardinessCur = cBodyPart.m_fHardinessMax;
			cBodyPart.m_ltAnim = new List<kAnimEnum>();
			cBodyPart.m_ltAnim.Add((kAnimEnum)item.nAnimEnum);
			cBodyPart.m_fDmgRate = item.fDmgRate;
			m_dictBodyPart.Add(item.nPartID, cBodyPart);
		}
	}

	public override void InitAnimData()
	{
		base.InitAnimData();
	}

	public override bool IsMobCanThink()
	{
		return base.IsMobCanThink();
	}

	public override void AddHP(float fHP)
	{
		base.AddHP(fHP);
	}

	public override void OnDead(kDeadMode nDeathMode)
	{
		base.OnDead(nDeathMode);
	}

	protected bool AddHardinessValue(CBodyPart info, float fValue)
	{
		if (info == null)
		{
			return false;
		}
		info.m_fHardinessCur += fValue * (info.m_fDmgRate / 100f);
		if (info.m_fHardinessCur <= 0f)
		{
			info.m_fHardinessCur = info.m_fHardinessMax;
			m_HurtAnim = info.GetAnimRadnom();
			return true;
		}
		if (info.m_fHardinessCur > info.m_fHardinessMax)
		{
			info.m_fHardinessCur = info.m_fHardinessMax;
		}
		return false;
	}

	protected void UpdateAITrigger(float deltaTime)
	{
		if (m_curTriggerList == null)
		{
			return;
		}
		m_tmpTriggerList.Clear();
		foreach (CAITriggerInfo curTrigger in m_curTriggerList)
		{
			if (curTrigger.nAI == m_nCurAIID)
			{
				continue;
			}
			switch (curTrigger.nType)
			{
			case 1:
				if (MyUtils.Compare(curTrigger.nValue, curTrigger.nOprate, m_fLifeTime, 0f) && (m_curTrigger == null || m_curTrigger.nPriority < curTrigger.nPriority))
				{
					m_tmpTriggerList.Add(curTrigger);
				}
				break;
			case 2:
				if (MyUtils.Compare(curTrigger.nValue, curTrigger.nOprate, m_fHP, m_fHPMax) && (m_curTrigger == null || m_curTrigger.nPriority < curTrigger.nPriority))
				{
					m_tmpTriggerList.Add(curTrigger);
				}
				break;
			}
		}
		if (m_tmpTriggerList.Count == 0)
		{
			return;
		}
		if (m_tmpTriggerList.Count == 1)
		{
			m_curTrigger = m_tmpTriggerList[0];
		}
		else
		{
			for (int i = 0; i < m_tmpTriggerList.Count; i++)
			{
				if (i == 0)
				{
					m_curTrigger = m_tmpTriggerList[i];
				}
				else if (m_curTrigger.nPriority < m_tmpTriggerList[i].nPriority)
				{
					m_curTrigger = m_tmpTriggerList[i];
				}
			}
		}
		if (m_curTrigger == null)
		{
			return;
		}
		int nAI = m_curTrigger.nAI;
		Debug.Log("Boss???????? AI:" + nAI);
		m_bShowTime = false;
		SetAI(nAI);
		if (m_GameScene.m_MGManager != null)
		{
			CEventManager eventManager = m_GameScene.m_MGManager.GetEventManager();
			if (eventManager != null)
			{
				eventManager.Trigger(new EventCondition_MobByWave(GenerateWaveID, GenerateSequence, 2, nAI));
				eventManager.Trigger(new EventCondition_MobByID(base.ID, 2, nAI));
			}
		}
	}

	public override void InitAI(int nAIManager)
	{
		base.InitAI(nAIManager);
		CAIManagerInfo aIManagerInfo = m_GameData.GetAIManagerInfo(nAIManager);
		if (aIManagerInfo == null)
		{
			return;
		}
		m_curTriggerList.Clear();
		foreach (CAITriggerInfo item in aIManagerInfo.ltAITrigger)
		{
			m_curTriggerList.Add(item);
		}
	}

	protected override void OnEnterAI(int nLastAI, int nAI)
	{
		CAIInfo aIInfo = m_GameData.GetAIInfo(nAI);
		if (aIInfo == null)
		{
			return;
		}
		ClearBodyEffect();
		for (int i = 0; i < aIInfo.ltEffect.Count; i++)
		{
			switch (aIInfo.ltEffect[i])
			{
			case 1500:
				AddBodyEffect(1500, GetBone(8));
				AddBodyEffect(1500, GetBone(9));
				break;
			case 1501:
				AddBodyEffect(1501, GetBone(2));
				break;
			}
		}
	}

	protected void ClearBodyEffect()
	{
		foreach (GameObject item in m_ltBodyEffect)
		{
			Object.Destroy(item);
		}
		m_ltBodyEffect.Clear();
	}

	protected void AddBodyEffect(int nPrefabID, Transform node)
	{
		if (node == null)
		{
			return;
		}
		GameObject gameObject = PrefabManager.Get(nPrefabID);
		if (!(gameObject == null))
		{
			GameObject gameObject2 = (GameObject)Object.Instantiate(gameObject);
			if (!(gameObject2 == null))
			{
				gameObject2.transform.parent = node;
				gameObject2.transform.localPosition = Vector3.zero;
				gameObject2.transform.localRotation = Quaternion.identity;
				m_ltBodyEffect.Add(gameObject2);
			}
		}
	}
}
