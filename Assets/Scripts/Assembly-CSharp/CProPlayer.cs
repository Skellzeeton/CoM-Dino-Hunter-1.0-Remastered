using System.Collections.Generic;

public class CProPlayer : CProBase
{
	protected Dictionary<int, CSkillPro> m_dictSkillPro;

	protected List<int> m_ltSkillPassive;

	protected int[] m_arrFunc;

	protected int[] m_arrValueX;

	protected int[] m_arrValueY;

	public CProPlayer()
	{
		m_dictSkillPro = new Dictionary<int, CSkillPro>();
		m_ltSkillPassive = new List<int>();
		m_arrFunc = new int[3];
		m_arrValueX = new int[3];
		m_arrValueY = new int[3];
		RegisterPro(kProEnum.MoveSpeedAcc);
		RegisterPro(kProEnum.EquipStoneUp);
		RegisterPro(kProEnum.All_Dmg);
		RegisterPro(kProEnum.All_Dmg_Rate);
		RegisterPro(kProEnum.All_Speed);
		RegisterPro(kProEnum.All_BeatBack);
		RegisterPro(kProEnum.All_Critical);
		RegisterPro(kProEnum.All_CriticalDmg);
		RegisterPro(kProEnum.All_Protect);
		RegisterPro(kProEnum.All_Capacity);
		RegisterPro(kProEnum.Melee_Dmg);
		RegisterPro(kProEnum.Melee_Dmg_Rate);
		RegisterPro(kProEnum.Melee_Speed);
		RegisterPro(kProEnum.Melee_BeatBack);
		RegisterPro(kProEnum.Melee_Critical);
		RegisterPro(kProEnum.Melee_CriticalDmg);
		RegisterPro(kProEnum.Melee_Protect);
		RegisterPro(kProEnum.Range_Dmg);
		RegisterPro(kProEnum.Range_Dmg_Rate);
		RegisterPro(kProEnum.Range_Speed);
		RegisterPro(kProEnum.Range_BeatBack);
		RegisterPro(kProEnum.Range_Critical);
		RegisterPro(kProEnum.Range_CriticalDmg);
		RegisterPro(kProEnum.Range_Protect);
		RegisterPro(kProEnum.Crossbow_Dmg);
		RegisterPro(kProEnum.Crossbow_Dmg_Rate);
		RegisterPro(kProEnum.Crossbow_Speed);
		RegisterPro(kProEnum.Crossbow_BeatBack);
		RegisterPro(kProEnum.Crossbow_Critical);
		RegisterPro(kProEnum.Crossbow_CriticalDmg);
		RegisterPro(kProEnum.Crossbow_Protect);
		RegisterPro(kProEnum.AutoRifle_Dmg);
		RegisterPro(kProEnum.AutoRifle_Dmg_Rate);
		RegisterPro(kProEnum.AutoRifle_Speed);
		RegisterPro(kProEnum.AutoRifle_BeatBack);
		RegisterPro(kProEnum.AutoRifle_Critical);
		RegisterPro(kProEnum.AutoRifle_CriticalDmg);
		RegisterPro(kProEnum.AutoRifle_Protect);
		RegisterPro(kProEnum.ShotGun_Dmg);
		RegisterPro(kProEnum.ShotGun_Dmg_Rate);
		RegisterPro(kProEnum.ShotGun_Speed);
		RegisterPro(kProEnum.ShotGun_BeatBack);
		RegisterPro(kProEnum.ShotGun_Critical);
		RegisterPro(kProEnum.ShotGun_CriticalDmg);
		RegisterPro(kProEnum.ShotGun_Protect);
		RegisterPro(kProEnum.HoldGun_Dmg);
		RegisterPro(kProEnum.HoldGun_Dmg_Rate);
		RegisterPro(kProEnum.HoldGun_Speed);
		RegisterPro(kProEnum.HoldGun_BeatBack);
		RegisterPro(kProEnum.HoldGun_Critical);
		RegisterPro(kProEnum.HoldGun_CriticalDmg);
		RegisterPro(kProEnum.HoldGun_Protect);
		RegisterPro(kProEnum.Rocket_Dmg);
		RegisterPro(kProEnum.Rocket_Dmg_Rate);
		RegisterPro(kProEnum.Rocket_Speed);
		RegisterPro(kProEnum.Rocket_BeatBack);
		RegisterPro(kProEnum.Rocket_Critical);
		RegisterPro(kProEnum.Rocket_CriticalDmg);
		RegisterPro(kProEnum.Rocket_Protect);
		RegisterPro(kProEnum.Rocket_AOE_Range);
		RegisterPro(kProEnum.Skill_CD_Faster);
		RegisterPro(kProEnum.Skill_CD_Faster_Rate);
		RegisterPro(kProEnum.Char_MoveSpeedUp);
		RegisterPro(kProEnum.Char_RecoverLife);
		RegisterPro(kProEnum.Char_MSEquip_Off);
		RegisterPro(kProEnum.Char_IncreaseGold);
		RegisterPro(kProEnum.Char_IncreaseExp);
	}

	public override void Initialize(int nID, int nLevel)
	{
		CCharacterInfoLevel characterInfo = m_GameData.GetCharacterInfo(nID, nLevel);
		if (characterInfo != null)
		{
			SetValueBase(kProEnum.HPMax, characterInfo.fLifeBase);
			SetValueBase(kProEnum.MoveSpeed, 6f);
			SetValueBase(kProEnum.MoveSpeedAcc, 6f);
			SetValueBase(kProEnum.Critical, 5f);
			SetValueBase(kProEnum.CriticalDmg, 100f);
		}
	}

	public override CSkillPro GetSkillPro(int nSkillID)
	{
		if (!m_dictSkillPro.ContainsKey(nSkillID))
		{
			return null;
		}
		return m_dictSkillPro[nSkillID];
	}

	public override void UpdateSkill(CCharBase charbase)
	{
		if (!charbase.IsPlayer() && !charbase.IsUser())
		{
			return;
		}
		CCharPlayer cCharPlayer = charbase as CCharPlayer;
		if (cCharPlayer == null)
		{
			return;
		}
		foreach (CProValue value in m_dictPro.Values)
		{
			value.m_fValueAffectFromSkill = 0f;
			value.UpdateValue();
		}
		m_dictSkillPro.Clear();
		m_ltSkillPassive.Clear();
		if (!cCharPlayer.GetSkillPassiveList(ref m_ltSkillPassive))
		{
			return;
		}
		if (m_ltSkillPassive != null)
		{
			foreach (int item in m_ltSkillPassive)
			{
				CSkillInfoLevel skillInfo = m_GameData.GetSkillInfo(item, 1);
				if (skillInfo == null || skillInfo.nType != 1)
				{
					continue;
				}
				for (int i = 0; i < 3; i++)
				{
					int num = skillInfo.arrFunc[i];
					int num2 = skillInfo.arrValueX[i];
					int num3 = skillInfo.arrValueY[i];
					switch (num)
					{
					case 1:
						ProFuncSkill((kProEnum)MyUtils.Low32(num2), MyUtils.Low32(num3), MyUtils.High32(num2), MyUtils.High32(num3));
						break;
					case 12:
						CaculateProSkill(num2, num3);
						break;
					}
				}
			}
		}
		for (int j = 0; j < 3; j++)
		{
			int nID = 0;
			int nLevel = 0;
			if (!cCharPlayer.GetCarryPassiveSkill(j, ref nID, ref nLevel))
			{
				continue;
			}
			CSkillInfoLevel skillInfo2 = m_GameData.GetSkillInfo(nID, nLevel);
			if (skillInfo2 == null || skillInfo2.nType != 1)
			{
				continue;
			}
			for (int k = 0; k < 3; k++)
			{
				int num4 = skillInfo2.arrFunc[k];
				int num5 = skillInfo2.arrValueX[k];
				int num6 = skillInfo2.arrValueY[k];
				switch (num4)
				{
				case 1:
					ProFuncSkill((kProEnum)MyUtils.Low32(num5), MyUtils.Low32(num6), MyUtils.High32(num5), MyUtils.High32(num6));
					break;
				case 12:
					CaculateProSkill(num5, num6);
					break;
				}
			}
		}
	}

	public override void UpdateBuff(CCharBase charbase)
	{
		foreach (CProValue value in m_dictPro.Values)
		{
			value.m_fValueAffectFromBuff = 0f;
			value.UpdateValue();
		}
		for (int i = 0; i < 10; i++)
		{
			iBuffData buffBySlot = charbase.GetBuffBySlot(i);
			if (buffBySlot == null || buffBySlot.m_nID == 0)
			{
				continue;
			}
			CBuffInfo buffInfo = m_GameData.GetBuffInfo(buffBySlot.m_nID);
			if (buffInfo == null)
			{
				continue;
			}
			for (int j = 0; j < 3; j++)
			{
				m_arrFunc[j] = buffInfo.arrFunc[j];
				m_arrValueX[j] = buffInfo.arrValueX[j];
				m_arrValueY[j] = buffInfo.arrValueY[j];
			}
			if (buffBySlot.m_nFromSkill > 0 && GetSkillPro(buffBySlot.m_nFromSkill) != null)
			{
				CaculateBuffFuncBySkillPro(buffBySlot.m_nFromSkill, buffInfo, ref m_arrFunc, ref m_arrValueX, ref m_arrValueY);
			}
			for (int k = 0; k < 3; k++)
			{
				int num = m_arrFunc[k];
				if (num == 1)
				{
					ProFuncBuff((kProEnum)MyUtils.Low32(m_arrValueX[k]), MyUtils.Low32(m_arrValueY[k]), MyUtils.High32(m_arrValueX[k]), MyUtils.High32(m_arrValueY[k]));
				}
			}
		}
	}

	public override void UpdateEquip(CCharBase charbase)
	{
		if (!charbase.IsPlayer() && !charbase.IsUser())
		{
			return;
		}
		CCharPlayer cCharPlayer = charbase as CCharPlayer;
		if (cCharPlayer == null)
		{
			return;
		}
		foreach (CProValue value in m_dictPro.Values)
		{
			value.m_fValueAffectFromEquip = 0f;
			value.UpdateValue();
		}
		CItemInfoLevel itemInfo = m_GameData.GetItemInfo(cCharPlayer.CurEquipStone, cCharPlayer.CurEquipStoneLevel);
		if (itemInfo == null || itemInfo.nType != 1)
		{
			return;
		}
		int num = (int)GetValue(kProEnum.EquipStoneUp);
		if (num == -1)
		{
			num = 0;
		}
		for (int i = 0; i < 3; i++)
		{
			int num2 = itemInfo.arrFunc[i];
			int num3 = itemInfo.arrValueX[i];
			int num4 = itemInfo.arrValueY[i];
			switch (num2)
			{
			case 1:
				ProFuncEquip((kProEnum)MyUtils.Low32(num3), MyUtils.Low32(num4) + num, MyUtils.High32(num3), MyUtils.High32(num4));
				break;
			case 12:
				CaculateProSkill(num3, num4);
				break;
			}
		}
	}

	protected void CaculateProSkill(int valuex, int valuey)
	{
		int num = MyUtils.Low32(valuex);
		CSkillPro cSkillPro = GetSkillPro(num);
		if (cSkillPro == null)
		{
			cSkillPro = new CSkillPro();
			cSkillPro.nID = num;
			m_dictSkillPro.Add(num, cSkillPro);
		}
		if (cSkillPro != null)
		{
			switch ((kSkillProEnum)MyUtils.High32(valuex))
			{
			case kSkillProEnum.RemainTime:
				cSkillPro.fRemainTime += valuey;
				break;
			case kSkillProEnum.BuffUp:
				cSkillPro.fBuffUp += valuey;
				break;
			case kSkillProEnum.DamageUp:
				cSkillPro.fDamageUp += valuey;
				break;
			case kSkillProEnum.BeatBack:
				cSkillPro.fBeatBack += valuey;
				break;
			case kSkillProEnum.CDDown:
				cSkillPro.fCDDown += valuey;
				break;
			case kSkillProEnum.BuffUpID:
				cSkillPro.ltBuffUpID.Add(valuey);
				break;
			case kSkillProEnum.HealUp:
				cSkillPro.fHealUp += valuey;
				break;
			}
		}
	}

	public override void CaculateSkillFuncBySkillPro(CSkillInfoLevel skillinfolevel, ref List<int> ltFunc, ref List<int> ltValueX, ref List<int> ltValueY)
	{
		if (skillinfolevel == null)
		{
			return;
		}
		CSkillPro skillPro = GetSkillPro(skillinfolevel.nID);
		if (skillPro == null)
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < 3; i++)
		{
			num = skillinfolevel.arrFunc[i];
			num2 = skillinfolevel.arrValueX[i];
			num3 = skillinfolevel.arrValueY[i];
			switch (num)
			{
			case 5:
				num2 += (int)skillPro.fDamageUp;
				break;
			case 4:
				num3 += (int)skillPro.fBeatBack;
				break;
			case 3:
				num3 += (int)skillPro.fRemainTime;
				break;
			case 9:
				num3 += (int)skillPro.fHealUp;
				break;
			}
			ltFunc.Add(num);
			ltValueX.Add(num2);
			ltValueY.Add(num3);
		}
		foreach (int item in skillPro.ltBuffUpID)
		{
			ltFunc.Add(3);
			ltValueX.Add(item);
			ltValueY.Add(num3);
		}
	}

	public void CaculateBuffFuncBySkillPro(int nSkillID, CBuffInfo buffinfo, ref int[] arrFunc, ref int[] arrValuex, ref int[] arrValuey)
	{
		if (buffinfo == null)
		{
			return;
		}
		CSkillPro skillPro = GetSkillPro(nSkillID);
		if (skillPro == null)
		{
			return;
		}
		for (int i = 0; i < 3; i++)
		{
			arrFunc[i] = buffinfo.arrFunc[i];
			arrValuex[i] = buffinfo.arrValueX[i];
			arrValuey[i] = buffinfo.arrValueY[i];
			int num = buffinfo.arrFunc[i];
			if (num == 1)
			{
				int nHigh = MyUtils.High32(arrValuey[i]);
				int num2 = MyUtils.Low32(arrValuey[i]);
				num2 += (int)skillPro.fBuffUp;
				arrValuey[i] = MyUtils.Make32(nHigh, num2);
			}
		}
	}
}
