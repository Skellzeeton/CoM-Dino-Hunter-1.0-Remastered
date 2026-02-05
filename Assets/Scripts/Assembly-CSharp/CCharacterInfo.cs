using System.Collections.Generic;

public class CCharacterInfo
{
	public int nID;

	public Dictionary<int, CCharacterInfoLevel> dictCharacterInfoLevel;

	public int nMaxLevel;

	public int nUnLockLevel;

	public bool isCrystalUnLock;

	public int nUnLockPrice;

	public bool isCrystalPurchase;

	public int nPurchasePrice;

	public List<int> ltCharacterPassiveSkill;

	public CCharacterInfo()
	{
		dictCharacterInfoLevel = new Dictionary<int, CCharacterInfoLevel>();
		ltCharacterPassiveSkill = new List<int>();
	}

	public void Add(int nLevel, CCharacterInfoLevel characterinfolevel)
	{
		if (!dictCharacterInfoLevel.ContainsKey(nLevel))
		{
			dictCharacterInfoLevel.Add(nLevel, characterinfolevel);
			if (nLevel - nMaxLevel == 1)
			{
				nMaxLevel = nLevel;
			}
		}
	}

	public CCharacterInfoLevel Get(int nLevel)
	{
		if (!dictCharacterInfoLevel.ContainsKey(nLevel))
		{
			return null;
		}
		return dictCharacterInfoLevel[nLevel];
	}

	public bool IsMaxLevel(int nLevel)
	{
		return nLevel >= nMaxLevel;
	}
}
