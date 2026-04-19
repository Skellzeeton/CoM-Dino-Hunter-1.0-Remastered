using gyTaskSystem;
using UnityEngine;

public class CControlWindows : CControlBase
{
	private Vector2 m_moveInput = Vector2.zero;
	private float m_sensitivityPercent = 1.0f;
	private bool m_mouseLocked = true;

	private const float kBaseYawSpeed = 245f;
	private const float kBasePitchSpeed = 135f;
	private const float kSensitivityStep = 0.125f;
	private const float kSensitivityMin = 0.25f;
	private const float kSensitivityMax = 2.0f;
	private const string kSensitivityPrefKey = "MouseSensitivityPercent";

	private void ToggleMouseLock()
	{
		m_mouseLocked = !m_mouseLocked;
		ApplyCursorLock(m_mouseLocked);
	}

	private void ApplyCursorLock(bool locked)
	{
		Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
		Cursor.visible = !locked;
	}

	private void SaveSensitivity()
	{
		m_sensitivityPercent = Mathf.Clamp(m_sensitivityPercent, kSensitivityMin, kSensitivityMax);
		PlayerPrefs.SetFloat(kSensitivityPrefKey, m_sensitivityPercent);
		PlayerPrefs.Save();
	}

	public CControlWindows()
	{
		m_GameScene = iGameApp.GetInstance().m_GameScene;
		m_GameUI = m_GameScene.GetGameUI();
		m_GameUI.RegisterEvent_Windows();
		m_DataCenter = iGameApp.GetInstance().m_GameData.GetDataCenter();
	}

	public override void Initialize()
	{
		base.Initialize();
		m_sensitivityPercent = PlayerPrefs.GetFloat(
			kSensitivityPrefKey,
			PlayerPrefs.GetFloat(kSensitivityPrefKey, 1.0f)
		);
		m_sensitivityPercent = Mathf.Clamp(m_sensitivityPercent, kSensitivityMin, kSensitivityMax);
		SaveSensitivity();
		ApplyCursorLock(true);
	}

	public override void Update(float deltaTime)
	{
		if (m_GameScene == null || m_User == null)
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.O))
		{
			m_sensitivityPercent = Mathf.Clamp(m_sensitivityPercent + kSensitivityStep, kSensitivityMin, kSensitivityMax);
			SaveSensitivity();
		}
		else if (Input.GetKeyDown(KeyCode.I))
		{
			m_sensitivityPercent = Mathf.Clamp(m_sensitivityPercent - kSensitivityStep, kSensitivityMin, kSensitivityMax);
			SaveSensitivity();
		}
		if (Input.GetKeyDown(KeyCode.F1))
		{
			ToggleMouseLock();
		}
		if (m_DataCenter.isTutorial)
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				if (m_GameScene.GameStatus == iGameSceneBase.kGameStatus.Gameing)
				{
					m_GameScene.SetGamePause(true);
					ApplyCursorLock(false);
				}
				else if (m_GameScene.GameStatus == iGameSceneBase.kGameStatus.Pause)
				{
					m_GameScene.SetGamePause(false);
					ApplyCursorLock(true);
				}
			}
		}
		if (m_GameScene.GameStatus == iGameSceneBase.kGameStatus.CutScene && Input.GetKeyDown(KeyCode.Space))
		{
			CCameraRoam.GetInstance().Stop();
		}
		if (!(m_GameScene.GameStatus == iGameSceneBase.kGameStatus.Gameing ||
			  m_GameScene.GameStatus == iGameSceneBase.kGameStatus.GameOver_ShowTime))
		{
			return;
		}
		m_moveInput.Set(0f, 0f);
		if (m_User.IsCanMove())
		{
			if (Input.GetKey(KeyCode.W)) m_moveInput.y += 1f;
			if (Input.GetKey(KeyCode.S)) m_moveInput.y -= 1f;
			if (Input.GetKey(KeyCode.A)) m_moveInput.x -= 1f;
			if (Input.GetKey(KeyCode.D)) m_moveInput.x += 1f;
		}
		if (m_moveInput == Vector2.zero)
		{
			m_User.MoveStop();
		}
		else
		{
			m_User.MoveByCompass(m_moveInput.x, m_moveInput.y);
			Ray ray = m_Camera.ScreenPointToRay(m_GameState.ScreenCenter, 0f);
			m_User.LookAt(ray.GetPoint(1000f));
		}
		if (m_mouseLocked && Cursor.lockState == CursorLockMode.Locked)
		{
			float axisX = Mathf.Clamp(Input.GetAxis("Mouse X"), -1f, 1f);
			float axisY = Mathf.Clamp(Input.GetAxis("Mouse Y"), -1f, 1f);
			float yawSpeed = kBaseYawSpeed * m_sensitivityPercent;
			float pitchSpeed = kBasePitchSpeed * m_sensitivityPercent;
			if (Mathf.Abs(axisX) > 0.001f)
			{
				m_Camera.Yaw(axisX * yawSpeed * deltaTime);
				if (m_User.IsCanAim())
				{
					m_User.SetYaw(m_Camera.GetYaw());
				}
			}
			if (Mathf.Abs(axisY) > 0.001f)
			{
				m_Camera.Pitch(axisY * pitchSpeed * deltaTime);
			}
			if (m_User.IsCanAim() && (Mathf.Abs(axisX) > 0.001f || Mathf.Abs(axisY) > 0.001f))
			{
				Ray ray2 = m_Camera.ScreenPointToRay(m_GameState.ScreenCenter, 0f);
				m_User.LookAt(ray2.GetPoint(1000f));
			}
		}
		if ((Input.GetKeyDown(KeyCode.Mouse2) || Input.GetKeyDown(KeyCode.LeftControl))
			&& m_User.IsCanAttack() && !m_User.IsSkillCD())
		{
			m_User.UseSkill(m_User.SkillID);
		}
		if (!CanSwitchWeapon())
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.Q))
		{
			CUISound.GetInstance().Play("UI_Weapon_change");
			int curWeaponIndex = m_User.CurWeaponIndex;
			int num = curWeaponIndex - 1;
			while (num != curWeaponIndex && m_GameState.GetWeapon(num) == null)
			{
				num--;
				if (num < 0)
				{
					num = 2;
				}
			}
			m_User.SwitchWeapon(num);
		}
		if (Input.GetKeyDown(KeyCode.E))
		{
			CUISound.GetInstance().Play("UI_Weapon_change");
			int curWeaponIndex2 = m_User.CurWeaponIndex;
			int num2 = curWeaponIndex2 + 1;
			while (num2 != curWeaponIndex2 && m_GameState.GetWeapon(num2) == null)
			{
				num2++;
				if (num2 >= 3)
				{
					num2 = 0;
				}
			}
			m_User.SwitchWeapon(num2);
		}
#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.Alpha9))
		{
			Debug.Log("press 9 key");
			m_GameScene.GameOver(true);
		}
#endif
	}

	private bool CanSwitchWeapon()
	{
		return !(m_User == null || m_User.isDead);
	}

	public override void LateUpdate(float deltaTime)
	{
	}
}