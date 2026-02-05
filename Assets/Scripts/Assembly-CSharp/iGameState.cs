using System.Collections.Generic;
using UnityEngine;

public class iGameState
{
	public class CPlayerInitInfo
	{
		public int nUID;

		public int nCharID;

		public int nCharLevel;

		public Vector3 v3Pos;
	}

	public class CDamageInfo
	{
		public bool bCritical;

		public float fDamage;

		public CDamageInfo()
		{
		}

		public CDamageInfo(float fDamage, bool bCritical)
		{
			this.fDamage = fDamage;
			this.bCritical = bCritical;
		}
	}

	public int m_HD;

	public int nLastLevel;

	public int nNowLevel;

	public int nLastHP;

	public int nNowHP;

	public bool isCheckUnLock = true;

	public string m_sLoadScene = string.Empty;

	public int m_nMaterialIDFromEquip = -1;

	public TUISceneType m_lstScene4SearchMaterial;

	public TUISceneType m_curScene4SearchMaterial;

	public TUISceneType m_lstScene4IAP;

	public TUISceneType m_curScene4IAP;

	public TUISceneType m_lstScene4Recommand;

	public TUISceneType m_curScene4Recommand;

	public int m_nLinkCharacter = -1;

	public int m_nLinkWeapon = -1;

	public int m_nLinkSkillRole = -1;

	public int m_nLinkSkill = -1;

	public int m_nLevelRewardGold;

	public int m_nLevelRewardExp;

	protected Vector2 m_v2ScreenCenter;

	protected kGameSceneEnum m_curScene;

	protected int[] m_arrCarryPassiveSkill;

	protected int[] m_arrCarryPassiveSkillLevel;

	protected int m_nCurWeaponIndex;

	protected CWeaponBase[] m_arrWeapon;

	protected int m_nCurGameLevel;

	protected int m_nCurBattleLevel;

	protected int m_nLastKillBoss;

	protected int m_nGainGoldInGame;

	protected CMaterialInfo[] m_arrGainMaterialInGame;

	protected string m_sUserName;

	protected bool m_bNetGame;

	protected Dictionary<int, CPlayerInitInfo> m_dictPlayerInitInfo;

	protected float m_fGameTime;

	public Vector2 ScreenCenter
	{
		get
		{
			return new Vector2(Screen.width / 2, Screen.height / 2);
		}
		set
		{
			m_v2ScreenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
		}
	}

	public int GameLevel
	{
		get
		{
			return m_nCurGameLevel;
		}
		set
		{
			m_nCurGameLevel = value;
		}
	}

	public int BattleLevel
	{
		get
		{
			return m_nCurBattleLevel;
		}
		set
		{
			m_nCurBattleLevel = value;
		}
	}

	public kGameSceneEnum CurScene
	{
		get
		{
			return m_curScene;
		}
		set
		{
			m_curScene = value;
		}
	}

	public bool isNetGame
	{
		get
		{
			return m_bNetGame;
		}
		set
		{
			m_bNetGame = value;
		}
	}

	public string UserName
	{
		get
		{
			return m_sUserName;
		}
		set
		{
			m_sUserName = value;
		}
	}

	public int LastKillBoss
	{
		get
		{
			return m_nLastKillBoss;
		}
		set
		{
			m_nLastKillBoss = value;
		}
	}

	public int GainGoldInGame
	{
		get
		{
			return m_nGainGoldInGame;
		}
		set
		{
			m_nGainGoldInGame = value;
		}
	}

	public Vector3 GetScreenCenterV3()
	{
		return new Vector3(Screen.width / 2, Screen.height / 2, 0f);
	}

	public void Initialize()
	{
		m_HD = ((!Utils.IsRetina()) ? 1 : 2);
		m_v2ScreenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
		m_arrWeapon = new CWeaponBase[3];
		m_nCurGameLevel = 1001;
		m_dictPlayerInitInfo = new Dictionary<int, CPlayerInitInfo>();
		UserName = "GGYY_" + Random.Range(0, 101);
		m_arrCarryPassiveSkill = new int[3];
		m_arrCarryPassiveSkillLevel = new int[3];
		m_arrGainMaterialInGame = new CMaterialInfo[iMacroDefine.GainMaterialFromGameMax + iMacroDefine.GainMaterialFromTaskMax];
		for (int i = 0; i < m_arrGainMaterialInGame.Length; i++)
		{
			m_arrGainMaterialInGame[i] = new CMaterialInfo(-1, 0);
		}
	}

	public void Reset()
	{
		m_fGameTime = 0f;
		m_nGainGoldInGame = 0;
		m_nLevelRewardGold = 0;
		m_nLevelRewardExp = 0;
		CMaterialInfo[] arrGainMaterialInGame = m_arrGainMaterialInGame;
		foreach (CMaterialInfo cMaterialInfo in arrGainMaterialInGame)
		{
			cMaterialInfo.nItemID = -1;
			cMaterialInfo.nItemCount = 0;
		}
		for (int j = 0; j < m_arrWeapon.Length; j++)
		{
			m_arrWeapon[j] = null;
		}
	}

	public void ClearNetPlayerInitPos()
	{
		m_dictPlayerInitInfo.Clear();
	}

	public void SetNetPlayerInitInfo(CPlayerInitInfo info)
	{
		if (!m_dictPlayerInitInfo.ContainsKey(info.nUID))
		{
			m_dictPlayerInitInfo.Add(info.nUID, info);
		}
		else
		{
			m_dictPlayerInitInfo[info.nUID] = info;
		}
	}

	public CPlayerInitInfo GetNetPlayerInitInfo(int nUID)
	{
		if (!m_dictPlayerInitInfo.ContainsKey(nUID))
		{
			return null;
		}
		return m_dictPlayerInitInfo[nUID];
	}

	public void AddGameTime(float fTime)
	{
		m_fGameTime += fTime;
	}

	public float GetGameTime()
	{
		return m_fGameTime;
	}

	public CWeaponBase GetCurrWeapon()
	{
		if (m_nCurWeaponIndex < 0 || m_nCurWeaponIndex >= m_arrWeapon.Length)
		{
			return null;
		}
		return m_arrWeapon[m_nCurWeaponIndex];
	}

	public CWeaponBase GetWeapon(int nIndex)
	{
		if (nIndex < 0 || nIndex >= m_arrWeapon.Length)
		{
			return null;
		}
		return m_arrWeapon[nIndex];
	}

	public void CarryWeapon(int nIndex, CWeaponBase weapon)
	{
		if (nIndex >= 0 && nIndex < m_arrWeapon.Length)
		{
			m_arrWeapon[nIndex] = weapon;
		}
	}

	public void SwitchWeapon(int nIndex)
	{
		if (nIndex >= 0 && nIndex < m_arrWeapon.Length)
		{
			m_nCurWeaponIndex = nIndex;
		}
	}

	public void CarryPassiveSkill(int nIndex, int nPassiveSkillID, int nPassiveSkillLevel)
	{
		if (nIndex >= 0 && nIndex < m_arrCarryPassiveSkill.Length)
		{
			m_arrCarryPassiveSkill[nIndex] = nPassiveSkillID;
			m_arrCarryPassiveSkillLevel[nIndex] = nPassiveSkillLevel;
		}
	}

	public bool GetCarryPassiveSkill(int nIndex, ref int nID, ref int nLevel)
	{
		if (nIndex < 0 || nIndex >= m_arrCarryPassiveSkill.Length)
		{
			return false;
		}
		nID = m_arrCarryPassiveSkill[nIndex];
		nLevel = m_arrCarryPassiveSkillLevel[nIndex];
		return true;
	}

	public void AddGold(int nGold)
	{
		m_nGainGoldInGame += nGold;
	}

	public void AddMaterial(int nID, int nCount, bool isInGame = true)
	{
		int num = 0;
		int num2 = 0;
		if (isInGame)
		{
			num = 0;
			num2 = iMacroDefine.GainMaterialFromGameMax;
		}
		else
		{
			num = iMacroDefine.GainMaterialFromGameMax;
			num2 = num + iMacroDefine.GainMaterialFromTaskMax;
		}
		for (int i = num; i < num2 && i >= 0 && i < m_arrGainMaterialInGame.Length; i++)
		{
			if (m_arrGainMaterialInGame[i].nItemID == -1)
			{
				m_arrGainMaterialInGame[i].nItemID = nID;
				m_arrGainMaterialInGame[i].nItemCount = nCount;
				break;
			}
			if (m_arrGainMaterialInGame[i].nItemID == nID)
			{
				m_arrGainMaterialInGame[i].nItemCount += nCount;
				break;
			}
		}
	}

	public CMaterialInfo GetGainMaterial(int nIndex)
	{
		if (nIndex < 0 || nIndex >= m_arrGainMaterialInGame.Length)
		{
			return null;
		}
		return m_arrGainMaterialInGame[nIndex];
	}

	public int GetGainMaterialCount()
	{
		if (m_arrGainMaterialInGame == null)
		{
			return -1;
		}
		return m_arrGainMaterialInGame.Length;
	}

	public bool HasAnyMaterial()
	{
		CMaterialInfo[] arrGainMaterialInGame = m_arrGainMaterialInGame;
		foreach (CMaterialInfo cMaterialInfo in arrGainMaterialInGame)
		{
			if (cMaterialInfo.nItemID > 0)
			{
				return true;
			}
		}
		return false;
	}
}
