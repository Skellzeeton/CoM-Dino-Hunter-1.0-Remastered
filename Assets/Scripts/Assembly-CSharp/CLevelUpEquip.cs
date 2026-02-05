public class CLevelUpEquip
{
	protected iGameData m_GameData;

	protected iDataCenter m_DataCenter;

	protected CItemInfo m_pItemInfo;

	protected int m_nLevel;

	protected int m_nLevelNext;

	protected bool m_bCrystalTrade;

	protected int m_nNeedValue;

	public bool isCrystalTrade
	{
		get
		{
			return m_bCrystalTrade;
		}
	}

	public int nNeedValue
	{
		get
		{
			return m_nNeedValue;
		}
	}

	public int AfterLevel
	{
		get
		{
			return m_nLevelNext;
		}
	}

	public CLevelUpEquip()
	{
		m_GameData = iGameApp.GetInstance().m_GameData;
		m_DataCenter = m_GameData.GetDataCenter();
		m_bCrystalTrade = false;
	}

	public bool Initialize(int nItemID)
	{
		m_pItemInfo = m_GameData.GetItemInfo(nItemID);
		if (m_pItemInfo == null)
		{
			return false;
		}
		m_DataCenter.GetEquipStone(nItemID, ref m_nLevel);
		m_nLevelNext = ((m_nLevel == -1) ? 1 : (m_nLevel + 1));
		return true;
	}

	public bool IsConditionMatch()
	{
		return true;
	}

	public bool IsMaterialsMatch()
	{
		if (m_pItemInfo == null)
		{
			return false;
		}
		CItemInfoLevel cItemInfoLevel = m_pItemInfo.Get(m_nLevelNext);
		if (cItemInfoLevel == null)
		{
			return false;
		}
		for (int i = 0; i < cItemInfoLevel.ltMaterials.Count && i < cItemInfoLevel.ltMaterialsCount.Count; i++)
		{
			if (m_DataCenter.GetMaterialNum(cItemInfoLevel.ltMaterials[i]) < cItemInfoLevel.ltMaterialsCount[i])
			{
				return false;
			}
		}
		return true;
	}

	public bool IsPriceMatch()
	{
		if (m_pItemInfo == null)
		{
			return false;
		}
		CItemInfoLevel cItemInfoLevel = m_pItemInfo.Get(m_nLevelNext);
		if (cItemInfoLevel == null)
		{
			return false;
		}
		if (cItemInfoLevel.isCrystalPurchase)
		{
			if (m_DataCenter.Crystal < cItemInfoLevel.nPurchasePrice)
			{
				m_bCrystalTrade = true;
				m_nNeedValue = cItemInfoLevel.nPurchasePrice - m_DataCenter.Crystal;
				return false;
			}
		}
		else if (m_DataCenter.Gold < cItemInfoLevel.nPurchasePrice)
		{
			m_bCrystalTrade = false;
			m_nNeedValue = cItemInfoLevel.nPurchasePrice - m_DataCenter.Crystal;
			return false;
		}
		return true;
	}

	public bool LevelUp()
	{
		if (!IsMaterialsMatch() || !IsPriceMatch())
		{
			return false;
		}
		if (m_pItemInfo == null)
		{
			return false;
		}
		CItemInfoLevel cItemInfoLevel = m_pItemInfo.Get(m_nLevelNext);
		if (cItemInfoLevel == null)
		{
			return false;
		}
		for (int i = 0; i < cItemInfoLevel.ltMaterials.Count && i < cItemInfoLevel.ltMaterialsCount.Count; i++)
		{
			m_DataCenter.AddMaterialNum(cItemInfoLevel.ltMaterials[i], -cItemInfoLevel.ltMaterialsCount[i]);
		}
		if (cItemInfoLevel.isCrystalPurchase)
		{
			m_DataCenter.AddCrystal(-cItemInfoLevel.nPurchasePrice);
			m_bCrystalTrade = true;
		}
		else
		{
			m_DataCenter.AddGold(-cItemInfoLevel.nPurchasePrice);
			m_bCrystalTrade = false;
		}
		m_DataCenter.SetEquipStone(cItemInfoLevel.nID, m_nLevelNext);
		m_DataCenter.Save();
		return true;
	}
}
