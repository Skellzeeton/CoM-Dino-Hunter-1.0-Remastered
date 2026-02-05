using System.Collections.Generic;

public class CSkillInfoLevel
{
	public int nID;

	public int nLevel;

	public int nType;

	public int nRangeType;

	public List<float> ltRangeValue;

	public int nTargetLimit;

	public string sName;

	public string sDesc;

	public string sIcon;

	public int nTargetMax;

	public int[] arrFunc;

	public int[] arrValueX;

	public int[] arrValueY;

	public int nAnim;

	public int nSkillMode;

	public List<float> ltSkillModeValue;

	public string sUseAudio = string.Empty;

	public bool isCrystalPurchase;

	public int nPurchasePrice;

	public string sLevelUpDesc = string.Empty;

	public CSkillInfoLevel()
	{
		nAnim = 2;
		ltRangeValue = new List<float>();
		ltSkillModeValue = new List<float>();
		arrFunc = new int[3];
		arrValueX = new int[3];
		arrValueY = new int[3];
	}

	public bool GetSkillRangeValue(int nIndex, ref float fValue)
	{
		if (nIndex < 0 || nIndex >= ltRangeValue.Count)
		{
			return false;
		}
		fValue = ltRangeValue[nIndex];
		return true;
	}

	public bool GetSkillRangeValue(int nIndex, ref int nValue)
	{
		if (nIndex < 0 || nIndex >= ltRangeValue.Count)
		{
			return false;
		}
		nValue = (int)ltRangeValue[nIndex];
		return true;
	}

	public bool GetSkillModeValue(int nIndex, ref float fValue)
	{
		if (nIndex < 0 || nIndex >= ltSkillModeValue.Count)
		{
			return false;
		}
		fValue = ltSkillModeValue[nIndex];
		return true;
	}

	public bool GetSkillModeValue(int nIndex, ref int nValue)
	{
		if (nIndex < 0 || nIndex >= ltSkillModeValue.Count)
		{
			return false;
		}
		nValue = (int)ltSkillModeValue[nIndex];
		return true;
	}
}
