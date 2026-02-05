using System;
using System.Collections.Generic;
using UnityEngine;

public class iGameLogic
{
	public class HitInfo
	{
		public Vector3 v3HitDir = Vector3.zero;

		public Vector3 v3HitPos = Vector3.zero;

		public CWeaponInfoLevel weaponinfolevel;

		public bool isPlayerSkill;

		public int nFromSkill = -1;

		public bool isHurt;
	}

	public List<float> ltDamageInfo;

	protected iGameSceneBase m_GameScene;

	protected iGameUIBase m_GameUI;

	protected iGameState m_GameState;

	protected iGameData m_GameData;

	protected List<int> m_ltFunc;

	protected List<int> m_ltValueX;

	protected List<int> m_ltValueY;

	public void Initialize()
	{
		m_GameScene = iGameApp.GetInstance().m_GameScene;
		m_GameUI = m_GameScene.GetGameUI();
		m_GameState = iGameApp.GetInstance().m_GameState;
		m_GameData = iGameApp.GetInstance().m_GameData;
		ltDamageInfo = new List<float>();
		m_ltFunc = new List<int>();
		m_ltValueX = new List<int>();
		m_ltValueY = new List<int>();
	}

	public void CaculateFunc(CCharBase actor, CCharBase target, int[] arrFunc, int[] arrValueX, int[] arrValueY, ref HitInfo hitinfo)
	{
		ltDamageInfo.Clear();
		for (int i = 0; i < arrFunc.Length; i++)
		{
			int num = arrFunc[i];
			int num2 = arrValueX[i];
			int num3 = arrValueY[i];
			if (num > 0)
			{
			}
			switch (num)
			{
			case 2:
				if (target != null && (target.Property == null || !(target.Property.GetValue(kProEnum.Invincible) > 0f)))
				{
					float num20 = num2;
					float num21 = actor.CalcCritical(hitinfo.weaponinfolevel);
					float num22 = actor.CalcCriticalDmg(hitinfo.weaponinfolevel);
					bool bCritical4 = false;
					if (num21 > UnityEngine.Random.Range(1f, 100f))
					{
						num20 *= 1f + num22 / 100f;
						bCritical4 = true;
					}
					float num23 = target.CalcProtect();
					num20 *= 1f - num23 / 100f;
					target.OnHit(0f - num20, hitinfo.weaponinfolevel, string.Empty);
					hitinfo.isHurt = true;
					if (m_GameScene.IsMyself(actor))
					{
						m_GameScene.AddDamageText(num20, hitinfo.v3HitPos, bCritical4);
					}
					else if (m_GameScene.IsMyself(target))
					{
						m_GameScene.AddDamageText(num20, hitinfo.v3HitPos, Color.red, bCritical4);
					}
					ltDamageInfo.Add(num20);
				}
				break;
			case 5:
				if (target != null && (target.Property == null || !(target.Property.GetValue(kProEnum.Invincible) > 0f)))
				{
					float num15 = actor.Property.GetValue(kProEnum.Damage) * (float)MyUtils.Low32(num2) / 100f + (float)MyUtils.High32(num2);
					float num16 = actor.CalcCritical(hitinfo.weaponinfolevel);
					float num17 = actor.CalcCriticalDmg(hitinfo.weaponinfolevel);
					bool bCritical3 = false;
					if (num16 > UnityEngine.Random.Range(1f, 100f))
					{
						num15 *= 1f + num17 / 100f;
						bCritical3 = true;
					}
					float num18 = target.CalcProtect();
					num15 *= 1f - num18 / 100f;
					target.OnHit(0f - num15, hitinfo.weaponinfolevel, string.Empty);
					hitinfo.isHurt = true;
					if (m_GameScene.IsMyself(actor))
					{
						m_GameScene.AddDamageText(num15, hitinfo.v3HitPos, bCritical3);
					}
					else if (m_GameScene.IsMyself(target))
					{
						m_GameScene.AddDamageText(num15, hitinfo.v3HitPos, Color.red, bCritical3);
					}
					ltDamageInfo.Add(num15);
				}
				break;
			case 7:
			{
				if (target.Property != null && target.Property.GetValue(kProEnum.Invincible) > 0f)
				{
					break;
				}
				float num5 = num2;
				float num6 = actor.CalcCritical(hitinfo.weaponinfolevel);
				float num7 = actor.CalcCriticalDmg(hitinfo.weaponinfolevel);
				foreach (CCharMob item in m_GameScene.GetMobEnumerator())
				{
					if (!(item == actor) && !item.isDead && !(Vector3.Distance(item.Pos, hitinfo.v3HitPos) > (float)num3))
					{
						float num8 = num5;
						bool bCritical = false;
						if (num6 > UnityEngine.Random.Range(1f, 100f))
						{
							num8 *= 1f + num7 / 100f;
							bCritical = true;
						}
						float num9 = item.CalcProtect();
						num8 *= 1f - num9 / 100f;
						target.OnHit(0f - num8, hitinfo.weaponinfolevel, string.Empty);
						hitinfo.isHurt = true;
						m_GameScene.AddDamageText(num8, item.GetBone(1).position, bCritical);
					}
				}
				break;
			}
			case 8:
			{
				if (target.Property != null && target.Property.GetValue(kProEnum.Invincible) > 0f)
				{
					break;
				}
				float num10 = actor.Property.GetValue(kProEnum.Damage) * (float)MyUtils.Low32(num2) / 100f + (float)MyUtils.High32(num2);
				float num11 = actor.CalcCritical(hitinfo.weaponinfolevel);
				float num12 = actor.CalcCriticalDmg(hitinfo.weaponinfolevel);
				foreach (CCharMob item2 in m_GameScene.GetMobEnumerator())
				{
					if (!(item2 == actor) && !item2.isDead && !(Vector3.Distance(item2.Pos, hitinfo.v3HitPos) > (float)num3))
					{
						float num13 = num10;
						bool bCritical2 = false;
						if (num11 > UnityEngine.Random.Range(1f, 100f))
						{
							num13 *= 1f + num12 / 100f;
							bCritical2 = true;
						}
						float num14 = item2.CalcProtect();
						num13 *= 1f - num14 / 100f;
						target.OnHit(0f - num13, hitinfo.weaponinfolevel, string.Empty);
						hitinfo.isHurt = true;
						m_GameScene.AddDamageText(num13, item2.GetBone(1).position, bCritical2);
					}
				}
				break;
			}
			case 3:
				if (target == null)
				{
					return;
				}
				target.AddBuff(num2, num3, hitinfo.nFromSkill);
				break;
			case 4:
			{
				if (!(target != null) || (target.Property != null && target.Property.GetValue(kProEnum.Invincible) > 0f))
				{
					break;
				}
				float value = target.Property.GetValue(kProEnum.ResistBeatBack);
				if (value <= 0f)
				{
					target.BeatBack(hitinfo.v3HitDir, num2);
					break;
				}
				float num4 = num3;
				if (actor.IsPlayer() && hitinfo.weaponinfolevel != null)
				{
					num4 = (float)num3 + ((CCharPlayer)actor).CalcWeaponBeatBack(hitinfo.weaponinfolevel);
				}
				if (value < num4)
				{
					target.BeatBack(hitinfo.v3HitDir, (float)num2 * ((num4 - value) / num4));
				}
				break;
			}
			case 9:
				if (target != null && (m_GameScene.IsMyself(actor) || m_GameScene.IsMyself(target)))
				{
					float num19 = target.Property.GetValue(kProEnum.HPMax) * (float)num2 / 100f + (float)num3;
					target.AddHP(num19);
					m_GameScene.AddHealText(num19, target.GetBone(0).position);
				}
				break;
			case 101:
			{
				CCharUser cCharUser2 = target as CCharUser;
				if (cCharUser2 != null)
				{
					float value2 = cCharUser2.Property.GetValue(kProEnum.Char_IncreaseExp);
					if (value2 > 0f)
					{
						num2 = (int)((float)num2 * (1f + value2 / 100f));
					}
					cCharUser2.AddExp(num2);
					m_GameScene.AddExpText(num2, cCharUser2.GetBone(0).position);
				}
				break;
			}
			case 10:
				target.SetStealth(true, num2);
				break;
			case 11:
				if (target.Property == null || !(target.Property.GetValue(kProEnum.Invincible) > 0f))
				{
					target.SetStun(true, num2);
				}
				break;
			case 100:
			{
				CCharUser cCharUser3 = target as CCharUser;
				if (cCharUser3 != null)
				{
					float value3 = cCharUser3.Property.GetValue(kProEnum.Char_IncreaseGold);
					if (value3 > 0f)
					{
						num2 = (int)((float)num2 * (1f + value3 / 100f));
					}
					m_GameState.AddGold(num2);
					m_GameScene.AddGoldText(num2, cCharUser3.GetBone(1).position);
				}
				break;
			}
			case 102:
			{
				CCharUser cCharUser = target as CCharUser;
				if (cCharUser != null)
				{
					CItemInfoLevel itemInfo = m_GameData.GetItemInfo(num2, 1);
					if (itemInfo != null)
					{
						m_GameScene.AddMaterial(cCharUser.GetBone(1).position, itemInfo.sIcon, num3);
					}
				}
				m_GameState.AddMaterial(num2, num3);
				break;
			}
			}
		}
	}

	public void Skill(int nSkillID, CCharBase attacker, CCharBase defender, ref HitInfo hitinfo)
	{
		CSkillInfoLevel skillInfo = m_GameData.GetSkillInfo(nSkillID, 1);
		if (skillInfo != null)
		{
			Skill(skillInfo, attacker, defender, ref hitinfo);
		}
	}

	public void Skill(CSkillInfoLevel skillinfolevel, CCharBase attacker, CCharBase defender, ref HitInfo hitinfo)
	{
		if (skillinfolevel != null && !(attacker == null) && !(defender == null))
		{
			hitinfo.nFromSkill = skillinfolevel.nID;
			Vector3 normalized = (defender.Pos - attacker.Pos).normalized;
			CCharPlayer cCharPlayer = attacker as CCharPlayer;
			if (cCharPlayer != null && cCharPlayer.Property != null && cCharPlayer.Property.GetSkillPro(skillinfolevel.nID) != null)
			{
				m_ltFunc.Clear();
				m_ltValueX.Clear();
				m_ltValueY.Clear();
				cCharPlayer.Property.CaculateSkillFuncBySkillPro(skillinfolevel, ref m_ltFunc, ref m_ltValueX, ref m_ltValueY);
				CaculateFunc(attacker, defender, m_ltFunc.ToArray(), m_ltValueX.ToArray(), m_ltValueY.ToArray(), ref hitinfo);
			}
			else
			{
				CaculateFunc(attacker, defender, skillinfolevel.arrFunc, skillinfolevel.arrValueX, skillinfolevel.arrValueY, ref hitinfo);
			}
		}
	}

	public void Item(int nItemID, int nItemLevel, CCharBase actor, CCharBase target)
	{
		CItemInfoLevel itemInfo = m_GameData.GetItemInfo(nItemID, nItemLevel);
		if (itemInfo != null)
		{
			Item(itemInfo, actor, target);
		}
	}

	public void Item(CItemInfoLevel iteminfolevel, CCharBase actor, CCharBase target)
	{
		if (iteminfolevel != null && !(actor == null))
		{
			HitInfo hitinfo = new HitInfo();
			CaculateFunc(actor, target, iteminfolevel.arrFunc, iteminfolevel.arrValueX, iteminfolevel.arrValueY, ref hitinfo);
		}
	}

	public bool IsSkillCanUse(CCharBase actor, CCharBase target, int nSkillID)
	{
		CSkillInfoLevel skillInfo = m_GameData.GetSkillInfo(nSkillID, 1);
		if (skillInfo == null)
		{
			return false;
		}
		return IsSkillCanUse(actor, target, skillInfo);
	}

	public bool IsSkillCanUse(CCharBase actor, CCharBase target, CSkillInfoLevel skillinfolevel)
	{
		if (skillinfolevel == null || target == null)
		{
			return false;
		}
		switch (skillinfolevel.nRangeType)
		{
		case 0:
		{
			float fValue4 = 0f;
			float fValue5 = 0f;
			if (skillinfolevel.GetSkillRangeValue(0, ref fValue4) && skillinfolevel.GetSkillRangeValue(1, ref fValue5))
			{
				float num2 = Vector3.Distance(actor.Pos, target.Pos);
				if (num2 < fValue4 || num2 > fValue5)
				{
					return false;
				}
			}
			break;
		}
		case 1:
		{
			float fValue = 0f;
			float fValue2 = 0f;
			float fValue3 = 0f;
			skillinfolevel.GetSkillRangeValue(0, ref fValue);
			skillinfolevel.GetSkillRangeValue(1, ref fValue2);
			skillinfolevel.GetSkillRangeValue(2, ref fValue3);
			float num = Vector3.Distance(actor.Pos, target.Pos);
			if (num < fValue || num > fValue2)
			{
				return false;
			}
			if (fValue3 != 0f)
			{
				Vector3 vector = target.Pos - actor.Pos;
				vector.y = 0f;
				if (Vector3.Dot(actor.Dir2D, vector.normalized) < Mathf.Cos(fValue3 * ((float)Math.PI / 180f) / 2f))
				{
					return false;
				}
			}
			break;
		}
		}
		return true;
	}

	public bool IsComboCanUse(CCharBase actor, CCharBase target, int nComboID)
	{
		CSkillComboInfo skillComboInfo = m_GameData.GetSkillComboInfo(nComboID);
		if (skillComboInfo == null || skillComboInfo.ltSkill.Count < 1)
		{
			return false;
		}
		int num = skillComboInfo.ltSkill[0];
		if (num == -1)
		{
			return false;
		}
		return IsSkillCanUse(actor, target, num);
	}
}
