using UnityEngine;
using gyAchievementSystem;
using gyEvent;

[RequireComponent(typeof(CharacterController))]
public class CCharUser : CCharPlayer
{
	protected CharacterController m_Controller;

	protected float m_fCurSpeedMax;

	protected float m_fCurSpeedSideMax;

	protected float m_fCurSpeed;

	protected float m_fCurSpeedSide;

	protected kCharMoveState m_CharMoveState;

	protected Vector2 m_v2MoveDir;

	protected Vector2 m_v2Move;

	protected bool m_bUpdatePos;

	protected bool m_bUpdateDir;

	protected float m_fUpdatePosTime;

	protected float m_fUpdateDirTime;

	protected float m_fSkillCD;

	protected float m_fSkillCDcount;

	protected int m_nCurWeaponIndex;

	public override Vector3 ShootDir
	{
		get
		{
			iCameraTrail iCameraTrail2 = m_GameScene.GetCamera();
			if (iCameraTrail2 == null)
			{
				return Vector3.forward;
			}
			return iCameraTrail2.transform.forward;
		}
	}

	public float CurSkillCD
	{
		get
		{
			return m_fSkillCD;
		}
	}

	public int CurWeaponIndex
	{
		get
		{
			return m_nCurWeaponIndex;
		}
		set
		{
			m_nCurWeaponIndex = value;
		}
	}
	
	public float MaxHP
	{
		get { return m_fHPMax; }
	}

	public new void Awake()
	{
		base.Awake();
		m_nType = kCharType.User;
		m_curMoveDir = kCharMoveDir.None;
		m_fCurSpeedMax = 0f;
		m_fCurSpeedSideMax = 0f;
		m_fCurSpeed = 0f;
		m_fCurSpeedSide = 0f;
		m_CharMoveState = kCharMoveState.None;
		m_v2MoveDir = Vector2.zero;
		m_v2Move = Vector2.zero;
		m_Controller = m_Model.GetComponent<CharacterController>();
		if (m_Controller == null)
		{
			m_Controller = m_Model.AddComponent<CharacterController>();
		}
		m_Controller.slopeLimit = 30f;
		m_Controller.stepOffset = 0.4f;
		m_Controller.center = new Vector3(0f, 1f, 0f);
	}

	public new void Start()
	{
		base.Start();
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	public new void Update()
	{
		float deltaTime = Time.deltaTime;
		base.Update();
		if (base.isDead)
		{
			return;
		}
		if (m_GameScene.GameStatus == iGameSceneBase.kGameStatus.Pause)
		{
			return;
		}
		UpdateAnimation();
		if (m_curWeapon != null)
		{
			m_curWeapon.Update(this, deltaTime);
		}
		if (m_fSkillCDcount < m_fSkillCD)
		{
			m_fSkillCDcount += deltaTime;
			if (m_fSkillCDcount > m_fSkillCD)
			{
				m_fSkillCDcount = m_fSkillCD;
			}
		}
		if (m_bUpdatePos)
		{
			m_fUpdatePosTime -= deltaTime;
			if (m_fUpdatePosTime <= 0f)
			{
				m_fUpdatePosTime = 0.5f;
				m_bUpdatePos = false;
				CGameNetSender.GetInstance().PlayerMove(base.Pos, base.Pos);
			}
		}
		if (!m_bUpdateDir)
		{
			return;
		}
		m_fUpdateDirTime -= deltaTime;
		if (m_fUpdateDirTime <= 0f)
		{
			m_fUpdateDirTime = 0.5f;
			m_bUpdateDir = false;
			RaycastHit hitInfo;
			if (Physics.Raycast(new Ray(GetShootMouse(), ShootDir), out hitInfo, 1000f, -1610612736))
			{
				CGameNetSender.GetInstance().PlayerAim(hitInfo.point);
			}
		}
	}

	public new void FixedUpdate()
	{
		base.FixedUpdate();
		float deltaTime = Time.deltaTime;
		if (m_CharMoveState == kCharMoveState.None)
		{
			return;
		}
		if (m_CharMoveState == kCharMoveState.Acc)
		{
			if (m_fCurSpeed < m_fCurSpeedMax)
			{
				m_fCurSpeed += m_Property.GetValue(kProEnum.MoveSpeedAcc) * deltaTime;
				if (m_fCurSpeed >= m_fCurSpeedMax)
				{
					m_fCurSpeed = m_fCurSpeedMax;
				}
			}
			if (m_fCurSpeedSide < m_fCurSpeedSideMax)
			{
				m_fCurSpeedSide += m_Property.GetValue(kProEnum.MoveSpeedAcc) * deltaTime;
				if (m_fCurSpeedSide >= m_fCurSpeedSideMax)
				{
					m_fCurSpeedSide = m_fCurSpeedSideMax;
				}
			}
			if (m_fCurSpeed == m_fCurSpeedMax && m_fCurSpeedSide == m_fCurSpeedSideMax)
			{
				m_CharMoveState = kCharMoveState.Max;
			}
		}
		float num = 0f;
		if (m_Property.GetValue(kProEnum.Char_MSEquip_Off) == 0f && m_curWeapon != null && m_curWeaponLvlInfo != null)
		{
			num = m_curWeaponLvlInfo.fMSDownRateEquip;
			if (m_curWeapon.IsFire())
			{
				num += m_curWeaponLvlInfo.fMSDownRateShoot;
			}
		}
		float value = m_Property.GetValue(kProEnum.Char_MoveSpeedUp);
		m_v2Move.x = (m_fCurSpeedSide + value) * (float)((m_v2MoveDir.x > 0f) ? 1 : (-1)) * (1f - num);
		m_v2Move.y = (m_fCurSpeed + value) * (float)((m_v2MoveDir.y > 0f) ? 1 : (-1)) * (1f - num);
		m_v2Move *= deltaTime;
		Vector3 zero = Vector3.zero;
		zero += m_ModelTransform.forward * m_v2Move.y;
		zero += m_ModelTransform.right * m_v2Move.x;
		Vector3 position = m_ModelTransform.position;
		zero.y = -20f * deltaTime;
		CollisionFlags collisionFlags = m_Controller.Move(zero);
		m_bUpdatePos = true;
	}

	public new void LateUpdate()
	{
		base.LateUpdate();
		m_ModelTransform.rotation = Quaternion.Euler(0f - m_fPitch, m_fYaw, m_fRoll);
	}

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		iItem component = hit.transform.root.GetComponent<iItem>();
		if (!(component == null) && component.ToughItem(this))
		{
			component.Destroy();
		}
	}

	public void MoveByCompass(float fRateX, float fRateY)
	{
		if (m_GameScene.GameStatus == iGameSceneBase.kGameStatus.Pause)
		{
			MoveStop();
			return;
		}

		Vector2 input = new Vector2(fRateX, fRateY);
		float magnitude = Mathf.Clamp01(input.magnitude);

		if (magnitude > 0.0001f)
		{
			Vector2 dir = input.normalized;
			m_v2MoveDir = dir;

			float baseSpeed = m_Property.GetValue(kProEnum.MoveSpeed);
			float speedScale = magnitude;

			m_fCurSpeedMax = baseSpeed * Mathf.Abs(dir.y) * speedScale;
			m_fCurSpeedSideMax = baseSpeed * Mathf.Abs(dir.x) * speedScale;

			UpdateMoveAnim(m_v2MoveDir);

			m_CharMoveState = kCharMoveState.Max;
			m_fCurSpeed = m_fCurSpeedMax;
			m_fCurSpeedSide = m_fCurSpeedSideMax;
		}
		else
		{
			MoveStop();
		}
	}

	public void MoveStop()
	{
		if (m_CharMoveState != 0)
		{
			m_bUpdatePos = false;
			CGameNetSender.GetInstance().PlayerMoveStop(base.Pos);
		}
		m_CharMoveState = kCharMoveState.None;
		m_fCurSpeedMax = 0f;
		m_fCurSpeedSideMax = 0f;
		m_fCurSpeed = 0f;
		m_fCurSpeedSide = 0f;
		StopMoveAnim();
	}

	protected void UpdateAnimation()
	{
		float step = m_AnimData.GetStep(kAnimEnum.MoveForward);
		if (step > 0f)
		{
			m_AnimManager.SetAnimSpeed(kAnimEnum.MoveForward, m_AnimManager.GetAnimLen(kAnimEnum.MoveForward) / (step / m_v2Move.magnitude));
		}
	}

	public override void InitChar(int nCharID, int nLevel, int nExp = 0)
	{
		iDataCenter dataCenter = m_GameData.GetDataCenter();
		if (dataCenter != null)
		{
			for (int i = 0; i < m_arrCarryPassiveSkill.Length; i++)
			{
				int selectPassiveSkill = dataCenter.GetSelectPassiveSkill(nCharID, i);
				if (selectPassiveSkill > 0)
				{
					int nSkillLevel = 0;
					if (dataCenter.GetPassiveSkill(selectPassiveSkill, ref nSkillLevel))
					{
						m_arrCarryPassiveSkill[i] = selectPassiveSkill;
						m_arrCarryPassiveSkillLevel[i] = nSkillLevel;
					}
				}
			}
			if (dataCenter.GetEquipStone(dataCenter.CurEquipStone, ref m_nEquipStoneLevel))
			{
				m_nEquipStone = dataCenter.CurEquipStone;
			}
		}
		base.InitChar(nCharID, nLevel, nExp);
		m_fSkillCD = m_curCharacterInfoLevel.fSkillCD;
		m_fSkillCDcount = m_fSkillCD;
		iGameUIBase gameUI = m_GameScene.GetGameUI();
		if (gameUI != null && m_fHPMax != 0f)
		{
			gameUI.SetProtraitName(m_curCharacterInfoLevel.sName);
			gameUI.SetProtraitIcon(m_curCharacterInfoLevel.sIcon);
			gameUI.SetProtraitLife(m_fHP / m_fHPMax);
			CSkillInfoLevel skillInfo = m_GameData.GetSkillInfo(m_nSkill, 1);
			if (skillInfo != null)
			{
				gameUI.SetSkillIcon(skillInfo.sIcon);
			}
		}
	}

	public void SwitchWeapon(int nIndex)
	{
		CWeaponBase weapon = m_GameState.GetWeapon(nIndex);
		if (weapon == null)
		{
			return;
		}
		CWeaponInfoLevel weaponInfo = m_GameData.GetWeaponInfo(weapon.ID, weapon.Level);
		if (weaponInfo == null)
		{
			return;
		}
		if (m_GameScene != null &&
		    m_GameScene.CurGameLevelInfo != null &&
		    m_GameScene.CurGameLevelInfo.m_bLimitMelee &&
		    weaponInfo.nType != 1)
		{
			return;
		}
		m_nCurWeaponIndex = nIndex;
		SetFire(false);
		UnEquipWeapon();
		m_GameState.SwitchWeapon(nIndex);
		EquipWeapon(weapon.ID, weapon.Level, m_GameState.GetCurrWeapon());
		iGameUIBase gameUI = m_GameScene.GetGameUI();
		if (gameUI != null)
		{
			if (weapon.CurWeaponLvlInfo != null)
			{
				gameUI.InitAimCross(weapon.CurWeaponLvlInfo.nType, weapon.CurWeaponLvlInfo.fPrecise);
			}
			gameUI.SetWeaponIcon(weapon.CurWeaponLvlInfo.sIcon);
		}
		iCameraTrail iCameraTrail2 = m_GameScene.GetCamera();
		if (iCameraTrail2 != null)
		{
			iCameraTrail2.SetViewMelee(weaponInfo.nType == 1);
		}
		iGameUIBase gameUI2 = m_GameScene.GetGameUI();
		if (gameUI2 != null)
		{
			gameUI2.ShowShootUI(true);
		}
		CGameNetSender.GetInstance().PlayerSwitchWeapon(weapon.ID, weapon.Level);
	}

	public void LookAt(Vector3 v3Point)
	{
		iCameraTrail iCameraTrail2 = m_GameScene.GetCamera();
		if (!(iCameraTrail2 == null))
		{
			Vector3 forward = iCameraTrail2.transform.forward;
			forward.y = 0f;
			if (!(Vector3.Dot(base.Dir2D, forward) <= 0.1f))
			{
				Vector3 dir = (v3Point - m_ManualSpine.position).normalized;
				UpdateUpBody(dir);
				iGameApp.GetInstance().SetGizmosLine("lookat", m_ManualSpine.position, v3Point, Color.green);
				m_bUpdateDir = true;
			}
		}
	}

	public void SetFire(bool bFire)
	{
		if (m_curWeapon == null)
		{
			return;
		}
		if (bFire)
		{
			m_curWeapon.Fire(this);
			CGameNetSender.GetInstance().PlayerShoot(true);
		}
		else
		{
			m_curWeapon.Stop(this);
			CGameNetSender.GetInstance().PlayerShoot(false);
		}
		if (m_curWeaponLvlInfo.nAttackMode == 2)
		{
			iCameraTrail iCameraTrail2 = m_GameScene.GetCamera();
			if (iCameraTrail2 != null)
			{
				iCameraTrail2.ShootMode(bFire);
			}
		}
	}

	public bool IsFire()
	{
		if (m_curWeapon == null)
		{
			return false;
		}
		return m_curWeapon.IsFire();
	}

	protected void UpdateMoveAnim(Vector2 v2Compass)
	{
		kCharMoveDir kCharMoveDir2 = kCharMoveDir.None;
		float num = Vector2.Dot(Vector2.up, v2Compass.normalized);
		kCharMoveDir2 = ((num > 0.78f) ? kCharMoveDir.Forward : ((num < -0.78f) ? kCharMoveDir.Back : ((v2Compass.x > 0f) ? ((num > 0.2f) ? kCharMoveDir.ForwardRight : ((!(num < -0.2f)) ? kCharMoveDir.Right : kCharMoveDir.BackRight)) : ((num > 0.2f) ? kCharMoveDir.ForwardLeft : ((!(num < -0.2f)) ? kCharMoveDir.Left : kCharMoveDir.BackLeft)))));
		SetMoveAnim(kCharMoveDir2);
	}

	public bool IsCanMove()
	{
		return !m_bBeatBack && !base.isDead && !m_bBumping && !m_bStun;
	}

	public override bool IsCanAttack()
	{
		return base.IsCanAttack();
	}

	public bool IsCanAim()
	{
		return !base.isDead && !m_bBumping && !m_bStun;
	}

	public override void BeatBack(Vector3 v3Dir, float fDis)
	{
		base.BeatBack(v3Dir, fDis);
		MoveStop();
		SetFire(false);
		m_GameScene.ShakeCamera(0.2f, 0.1f);
	}

	public override void SetStun(bool bStun, float fTime = 0f)
	{
		base.SetStun(bStun, fTime);
		try
		{
			isStun = bStun;
			StunTime = fTime;
			var field = this.GetType().GetField("m_bStun", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			if (field != null)
				field.SetValue(this, bStun);
		}
		catch
		{
		}

		if (bStun)
		{
			MoveStop();
			SetFire(false);
		}
	}

	public override void AddHP(float fHP)
	{
		base.AddHP(fHP);
		iGameUIBase gameUI = m_GameScene.GetGameUI();
		if (gameUI != null && m_fHPMax != 0f)
		{
			gameUI.SetProtraitLife(m_fHP / m_fHPMax);
		}
	}

	public override void OnDead(kDeadMode nDeathMode)
	{
		base.OnDead(nDeathMode);
		MoveStop();
		if (m_GameScene.IsRoomMaster() && m_GameScene.IsPlayerAllDead() && m_GameScene.m_TaskManager != null)
		{
			m_GameScene.m_TaskManager.OnPlayerDead();
		}
	}

	public override bool OnHit(float fDmg, CWeaponInfoLevel pWeaponLvlInfo = null, string sBodyPart = "")
	{
		if (m_GameScene.GameStatus != iGameSceneBase.kGameStatus.Gameing)
		{
			return false;
		}
		iGameUIBase gameUI = m_GameScene.GetGameUI();
		if (gameUI != null)
		{
			gameUI.ShowScreenBlood(true);
		}
		return base.OnHit(fDmg, pWeaponLvlInfo, sBodyPart);
	}

	public override void AddExp(int nExp)
	{
		if (m_curCharacterInfo == null || nExp == 0 || m_curCharacterInfo.IsMaxLevel(base.Level))
		{
			return;
		}
		m_nExp += nExp;
		iGameUIBase gameUI = m_GameScene.GetGameUI();
		bool flag = false;
		if (nExp <= 0)
		{
			if (m_nExp < 0)
			{
				m_nExp = 0;
			}
			if (gameUI != null)
			{
				gameUI.SetProtraitExp(0, (float)m_nExp / (float)m_curCharacterInfoLevel.nExp);
			}
		}
		else
		{
			int nLevel = m_nLevel;
			LevelUp(ref m_nExp, ref m_nLevel);
			int num = m_nLevel - nLevel;
			if (gameUI != null)
			{
				gameUI.SetProtraitExp(num, Mathf.Clamp01((float)m_nExp / (float)m_curCharacterInfoLevel.nExp));
				gameUI.SetProtraitLevel(m_nLevel);
				if (num > 0)
				{
					gameUI.PlayProtraitLevelAnim();
				}
			}
			if (num > 0)
			{
				InitChar(base.ID, base.Level, base.EXP);
				m_bUpdateProBuff = true;
				m_GameState.isCheckUnLock = true;
				flag = true;
				CGameNetSender.GetInstance().PlayerLevelUp(base.Level);
				if (base.CurCharInfoLevel.isMale)
				{
					PlayAudio("UI_Upgrade_Male");
					GameObject gameObject = m_GameScene.AddEffect(Vector3.zero, Vector3.one, 5f, 1300);
					gameObject.transform.parent = GetBone(3);
					gameObject.transform.localPosition = new Vector3(0f, 0.01f, 0f);
					gameObject.transform.localRotation = Quaternion.identity;
				}
				else
				{
					PlayAudio("UI_Upgrade_Female");
					GameObject gameObject = m_GameScene.AddEffect(Vector3.zero, Vector3.one, 5f, 1302);
					gameObject.transform.parent = GetBone(3);
					gameObject.transform.localPosition = new Vector3(0f, 0.01f, 0f);
					gameObject.transform.localRotation = Quaternion.identity;
				}
			}
		}
		iDataCenter dataCenter = m_GameData.GetDataCenter();
		if (dataCenter == null)
		{
			return;
		}
		dataCenter.SetCharacter(base.ID, base.Level, base.EXP);
		if (flag)
		{
			dataCenter.Save();
			if (base.Level == dataCenter.HighestCharLevel)
			{
				CAchievementManager.GetInstance().AddAchievement(1, new object[1] { base.Level });
			}
		}
	}

	public void AddGold(int nGold)
	{
	}

	public void LevelUp(ref int nExp, ref int nLevel)
	{
		CCharacterInfoLevel characterInfo = m_GameData.GetCharacterInfo(base.ID, nLevel);
		if (characterInfo == null)
		{
			return;
		}
		int nExp2 = characterInfo.nExp;
		if (nExp >= nExp2)
		{
			nExp -= nExp2;
			nLevel++;
			if (!m_curCharacterInfo.IsMaxLevel(nLevel))
			{
				LevelUp(ref nExp, ref nLevel);
			}
		}
	}

	public override void TakeItem(int nItemID, GameObject ItemObj)
	{
		base.TakeItem(nItemID, ItemObj);
		m_GameScene.ShowItemScreenTip(false);
		m_GameScene.ShowTriggerEndScreenTip(true);
		if (m_GameScene.m_MGManager != null)
		{
			CEventManager eventManager = m_GameScene.m_MGManager.GetEventManager();
			if (eventManager != null)
			{
				eventManager.Trigger(new EventCondition_StealEgg_Take(nItemID, m_GameScene.GetStealItem(nItemID)));
			}
		}
	}

	public override void DropItem()
	{
		base.DropItem();
		m_GameScene.ShowItemScreenTip(true);
		m_GameScene.ShowTriggerEndScreenTip(false);
	}

	public override void UseSkill(int nSkill)
	{
		base.UseSkill(nSkill);
		m_fSkillCD = m_curCharacterInfoLevel.fSkillCD;
		CSkillPro skillPro = m_Property.GetSkillPro(nSkill);
		if (skillPro != null)
		{
			m_fSkillCD -= skillPro.fCDDown;
		}
		float value = m_Property.GetValue(kProEnum.Skill_CD_Faster);
		float value2 = m_Property.GetValue(kProEnum.Skill_CD_Faster_Rate);
		m_fSkillCD = m_fSkillCD * (1f - value2 / 100f) - value;
		m_fSkillCDcount = 0f;
		iGameUIBase gameUI = m_GameScene.GetGameUI();
		if (gameUI != null)
		{
			gameUI.SetSkillCD(m_fSkillCD);
		}
	}

	public bool IsSkillCD()
	{
		if (m_fSkillCDcount < m_fSkillCD)
		{
			return true;
		}
		return false;
	}

	public void ResetSkillCD()
	{
		m_fSkillCDcount = m_fSkillCD;
		iGameUIBase gameUI = m_GameScene.GetGameUI();
		if (gameUI != null)
		{
			gameUI.FinishSkillCD();
		}
	}
}
