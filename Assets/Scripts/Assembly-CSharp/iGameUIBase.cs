using System.Collections.Generic;
using UnityEngine;
using gyAchievementSystem;
using gyTaskSystem;

public class iGameUIBase : MonoBehaviour
{
	protected Transform mPreLoadNode;

	protected gyUIManager m_UIManager;

	protected gyUIAimCross m_UIAimCross;

	protected int m_nCurWeaponType;

	protected iGameSceneBase m_GameScene;

	protected iGameData m_GameData;

	protected iGameState m_GameState;

	protected CPoolManage m_NGUIDamage;

	protected CPoolManage m_NGUILifeBar;

	protected CPoolManage m_NGUIScreenTip;

	protected CPoolManage m_NGUIGoldTip;

	protected CPoolManage m_NGUIMaterialTip;

	protected CPoolManage m_NGUITextTip;

	protected Dictionary<int, gyLifeBarHUD> m_dictLifeBar;

	protected iGameTaskUIPlane m_GameTaskUIPlane;

	protected CUIProtraitInfo m_MainPlayerPortraitInfo;

	protected Dictionary<int, CUIProtraitInfo> m_dictTeamMate;

	protected CSlipAssistant m_SlipAssist;

	protected Transform[] m_TeamMateNode;

	private void Awake()
	{
		GameObject gameObject = GameObject.Find("UI Root (2D)");
		if (gameObject == null)
		{
			Debug.LogError("cant find UI Root (2D)");
			return;
		}
		m_UIManager = gameObject.GetComponent<gyUIManager>();
		if (m_UIManager == null)
		{
			Debug.LogError("cant find gyUIManager");
			return;
		}
		if (m_UIManager.mButtonShoot != null)
		{
			m_UIManager.mButtonShoot.gameObject.SetActiveRecursively(false);
		}
		ShowSkipUI(false);
		ShowTutorials(false);
		if (mPreLoadNode == null)
		{
			GameObject gameObject2 = GameObject.Find("GamePreLoad");
			if (gameObject2 == null)
			{
				gameObject2 = new GameObject("GamePreLoad");
			}
			if (gameObject2 != null)
			{
				mPreLoadNode = gameObject2.transform;
			}
		}
		m_dictLifeBar = new Dictionary<int, gyLifeBarHUD>();
		m_GameTaskUIPlane = GetControl<iGameTaskUIPlane>("_AnchorTop/NGUITaskUIPlane");
		m_MainPlayerPortraitInfo = new CUIProtraitInfo();
		m_MainPlayerPortraitInfo.Initialize(m_UIManager.mHeadPortrait);
		float num = (float)Screen.height / 360f;
		float num2 = (float)Screen.width / 520f;
		float num3 = ((!(num < num2)) ? num2 : num);
		foreach (Transform item in m_UIManager.mParent)
		{
			if (item.name.IndexOf("Anchor") != -1)
			{
				item.localScale *= num3;
			}
		}
        Debug.Log("Screen size = " + Screen.width + " * " + Screen.height);
        Debug.Log("Screen scale multiplier = " + num3);
        m_SlipAssist = new CSlipAssistant();
	}

	private void Update()
	{
		if (CAchievementManager.GetInstance().GetTipCount() > 0 && m_UIManager != null && m_UIManager.mAchievementTip != null && !m_UIManager.mAchievementTip.isActive)
		{
			CAchievementTip cAchievementTip = CAchievementManager.GetInstance().PopTip();
			if (cAchievementTip != null)
			{
				m_UIManager.mAchievementTip.ShowTip(cAchievementTip.sName, cAchievementTip.nStep);
			}
		}
	}

	public void Initialize()
	{
		m_GameScene = iGameApp.GetInstance().m_GameScene;
		m_GameData = iGameApp.GetInstance().m_GameData;
		m_GameState = iGameApp.GetInstance().m_GameState;
		m_GameTaskUIPlane.Initialize();
		InitTaskUI();
		m_NGUIDamage = new CPoolManage();
		m_NGUIDamage.Initialize("Artist/GameUI/NGUIDamage", mPreLoadNode, m_UIManager.mParent, 1);
		m_NGUILifeBar = new CPoolManage();
		m_NGUILifeBar.Initialize("Artist/GameUI/NGUILifeBar", mPreLoadNode, m_UIManager.mParent, 1);
		m_NGUIScreenTip = new CPoolManage();
		m_NGUIScreenTip.Initialize("Artist/GameUI/NGUIScreenTip", mPreLoadNode, m_UIManager.mParent, 1);
		m_NGUIGoldTip = new CPoolManage();
		m_NGUIGoldTip.Initialize("Artist/GameUI/NGUIGoldTip", mPreLoadNode, m_UIManager.mParent, 1);
		m_NGUIMaterialTip = new CPoolManage();
		m_NGUIMaterialTip.Initialize("Artist/GameUI/NGUIMaterialTip", mPreLoadNode, m_UIManager.mParent, 1);
		m_NGUITextTip = new CPoolManage();
		m_NGUITextTip.Initialize("Artist/GameUI/NGUITextTip", mPreLoadNode, m_UIManager.mParent, 1);
		Reset();
	}

	public void Destroy()
	{
		foreach (gyLifeBarHUD value in m_dictLifeBar.Values)
		{
			if (value.gameObject != null)
			{
				Object.Destroy(value.gameObject);
			}
		}
		m_dictLifeBar.Clear();
		ClearTeamMateProtrait();
	}

	public void InitTaskUI()
	{
		if (m_GameScene.m_TaskManager == null)
		{
			return;
		}
		List<CTaskBase> taskList = m_GameScene.m_TaskManager.GetTaskList();
		if (taskList == null)
		{
			return;
		}
		m_GameTaskUIPlane.Clear();
		foreach (CTaskBase item in taskList)
		{
			CTaskInfo taskInfo = item.GetTaskInfo();
			if (taskInfo != null)
			{
				iGameTaskUIBase iGameTaskUIBase2 = m_GameTaskUIPlane.Add(taskInfo.nType);
				if (!(iGameTaskUIBase2 == null))
				{
					iGameTaskUIBase2.Initialize(item);
				}
			}
		}
	}

	public void Reset()
	{
		ShowSuccess(false);
		ShowFailed(false);
	}

	public T GetControl<T>(string path) where T : MonoBehaviour
	{
		Transform transform = m_UIManager.mParent.Find(path);
		if (transform == null)
		{
			return (T)null;
		}
		return transform.GetComponent<T>();
	}

	public GameObject GetControl(string path)
	{
		Transform transform = m_UIManager.mParent.Find(path);
		if (transform == null)
		{
			return null;
		}
		return transform.gameObject;
	}

	public GameObject AddControl(int nPrefab, Transform parent)
	{
		GameObject gameObject = PrefabManager.Get(nPrefab);
		if (gameObject == null)
		{
			return null;
		}
		Vector3 localScale = gameObject.transform.localScale;
		GameObject gameObject2 = (GameObject)Object.Instantiate(gameObject);
		if (gameObject2 == null)
		{
			return null;
		}
		if (parent != null)
		{
			gameObject2.transform.parent = parent;
		}
		else
		{
			gameObject2.transform.parent = m_UIManager.mParent;
		}
		gameObject2.transform.localScale = localScale;
		return gameObject2;
	}

	public void AddDmgUI(float fValue, Vector2 v2Pos, Color color, gyUILabelDmg.kMode mode)
	{
		GameObject gameObject = m_NGUIDamage.Get();
		if (!(gameObject == null))
		{
			gyUILabelDmg component = gameObject.GetComponent<gyUILabelDmg>();
			if (!(component == null))
			{
				gameObject.transform.parent = m_UIManager.mParent;
				component.SetLabel(fValue.ToString("0.##"));
				component.SetColor(color);
				component.Go(v2Pos, mode);
			}
		}
	}

	public void AddExpText(Vector2 v2Pos, int nExp)
	{
		GameObject gameObject = m_NGUIDamage.Get();
		if (!(gameObject == null))
		{
			gyUILabelDmg component = gameObject.GetComponent<gyUILabelDmg>();
			if (!(component == null))
			{
				gameObject.transform.parent = m_UIManager.mParent;
				component.SetLabel("EXP:" + nExp);
				component.SetColor(new Color(0f, 0.15f, 0.9f));
				component.Go(v2Pos, gyUILabelDmg.kMode.Mode5);
			}
		}
	}

	public void AddGoldUI(float fValue, Vector3 v3Pos)
	{
		GameObject gameObject = m_NGUIGoldTip.Get();
		if (!(gameObject == null))
		{
			gyUILabelDmg component = gameObject.GetComponent<gyUILabelDmg>();
			if (!(component == null))
			{
				gameObject.transform.parent = m_UIManager.mParent;
				component.SetLabel(fValue.ToString());
				component.Go(v3Pos);
			}
		}
	}

	public void AddMaterialUI(Vector3 v3Pos, string sIcon, int nCount)
	{
		GameObject gameObject = m_NGUIMaterialTip.Get();
		if (gameObject == null)
		{
			return;
		}
		gyUIMaterials component = gameObject.GetComponent<gyUIMaterials>();
		if (!(component == null))
		{
			gameObject.transform.parent = m_UIManager.mParent;
			gameObject.transform.localPosition = Vector3.zero;
			component.SetIcon(sIcon);
			component.SetValue(nCount);
			Vector3 vector = Vector3.zero;
			if (m_UIManager != null && m_UIManager.mHeadPortrait != null)
			{
				vector = m_UIManager.mHeadPortrait.transform.localPosition;
			}
			component.Go(v3Pos, vector);
		}
	}

	public void SetWeaponIcon(string sName)
	{
		if (!(m_UIManager == null) && !(m_UIManager.mWeapon == null))
		{
			iGameUIWeapon component = m_UIManager.mWeapon.GetComponent<iGameUIWeapon>();
			if (!(component == null))
			{
				component.SetIcon(sName);
			}
		}
	}

	public void SetSkillIcon(string sName)
	{
		if (!(m_UIManager == null) && !(m_UIManager.mSkill == null))
		{
			m_UIManager.mSkill.SetIcon(sName);
		}
	}

	public gyLifeBarHUD CreateLifeBar(CCharBase target)
	{
		if (target == null)
		{
			return null;
		}
		GameObject gameObject = m_NGUILifeBar.Get();
		if (gameObject == null)
		{
			return null;
		}
		gyLifeBarHUD component = gameObject.GetComponent<gyLifeBarHUD>();
		if (component == null)
		{
			return null;
		}
		component.Initialize(target);
		m_dictLifeBar.Add(target.UID, component);
		return component;
	}

	public void RemoveLifeBar(int nUID)
	{
		if (m_dictLifeBar.ContainsKey(nUID))
		{
			gyUIPoolObject component = m_dictLifeBar[nUID].GetComponent<gyUIPoolObject>();
			if (component != null)
			{
				component.TakeBack(0f);
			}
			else
			{
				Object.Destroy(m_dictLifeBar[nUID].gameObject);
			}
			m_dictLifeBar.Remove(nUID);
		}
	}

	public void ShootLifeBar(int nUID)
	{
		foreach (KeyValuePair<int, gyLifeBarHUD> item in m_dictLifeBar)
		{
			if (nUID != item.Key)
			{
				item.Value.SetTime(0f, 0.5f);
			}
		}
	}

	public gyUIScreenTip CreateScreenTip(GameObject actor, GameObject target)
	{
		if (actor == null || target == null)
		{
			return null;
		}
		GameObject gameObject = m_NGUIScreenTip.Get();
		if (gameObject == null)
		{
			return null;
		}
		gyUIScreenTip component = gameObject.GetComponent<gyUIScreenTip>();
		if (component == null)
		{
			return null;
		}
		component.Initialize(actor, target);
		return component;
	}

	public void RemoveScreenTip(gyUIScreenTip screentip)
	{
		if (!(screentip.gameObject == null))
		{
			screentip.Clear();
			gyUIPoolObject component = screentip.gameObject.GetComponent<gyUIPoolObject>();
			if (component != null)
			{
				component.TakeBack(0f);
			}
			else
			{
				Object.Destroy(screentip.gameObject);
			}
		}
	}

	public iGameTaskUIPlane GetTaskPlane()
	{
		return m_GameTaskUIPlane;
	}

	public void ShowSuccess(bool bShow)
	{
		if (!(m_UIManager == null) && !(m_UIManager.mPanelMissionComplete == null))
		{
			m_UIManager.mPanelMissionComplete.Show(bShow);
			if (bShow)
			{
				m_UIManager.mPanelMissionComplete.SetGainExp(m_GameState.m_nLevelRewardExp);
				m_UIManager.mPanelMissionComplete.SetGainGold(m_GameState.m_nLevelRewardGold);
				m_UIManager.mPanelMissionComplete.SetGainGoldEarned(m_GameState.GainGoldInGame);
			}
		}
	}

	public void ShowFailed(bool bShow)
	{
		if (!(m_UIManager == null) && !(m_UIManager.mPanelMissionFailed == null))
		{
			m_UIManager.mPanelMissionFailed.Show(bShow);
		}
	}

	public void ShowRevive(bool bShow)
	{
		if (!(m_UIManager == null) && !(m_UIManager.mPanelRevive == null))
		{
			m_UIManager.mPanelRevive.Show(bShow);
			if (bShow)
			{
				m_UIManager.mPanelRevive.ShowStatistcs(true);
				m_UIManager.mPanelRevive.SetLostGold(m_GameState.GainGoldInGame);
				m_UIManager.mPanelRevive.SetReviveTime(10f);
			}
		}
	}

	public void PauseReviveTime(bool bPause)
	{
		if (!(m_UIManager == null) && !(m_UIManager.mPanelRevive == null))
		{
			m_UIManager.mPanelRevive.PauseReviveTime(bPause);
		}
	}

	public void ResetReviveTime()
	{
		if (!(m_UIManager == null) && !(m_UIManager.mPanelRevive == null))
		{
			m_UIManager.mPanelRevive.ResetReviveTime();
		}
	}

	public void ShowMaterial(bool bShow)
	{
		if (m_UIManager == null || m_UIManager.mPanelMaterial == null)
		{
			return;
		}
		m_UIManager.mPanelMaterial.Show(bShow);
		if (!bShow)
		{
			return;
		}
		for (int i = 0; i < iMacroDefine.GainMaterialFromGameMax; i++)
		{
			CMaterialInfo gainMaterial = m_GameState.GetGainMaterial(i);
			if (gainMaterial == null || gainMaterial.nItemID == -1)
			{
				continue;
			}
			m_UIManager.mPanelMaterial.ShowItem(i, true);
			m_UIManager.mPanelMaterial.SetCount(i, gainMaterial.nItemCount);
			CItemInfoLevel itemInfo = m_GameData.GetItemInfo(gainMaterial.nItemID, 1);
			if (itemInfo != null)
			{
				GameObject gameObject = PrefabManager.Get("Artist/Atlas/Material/" + itemInfo.sIcon);
				if (gameObject != null)
				{
					m_UIManager.mPanelMaterial.SetIcon(i, gameObject.GetComponent<UIAtlas>());
				}
				m_UIManager.mPanelMaterial.SetIconBG(i, "kuangdj_" + itemInfo.nRare);
			}
		}
		iDataCenter dataCenter = m_GameData.GetDataCenter();
		if (dataCenter != null)
		{
			m_UIManager.mPanelMaterial.SetStashCur(dataCenter.StashCount);
			m_UIManager.mPanelMaterial.SetStashMax(dataCenter.StashCountMax);
		}
	}

	public void ShowLevelUp(bool bShow)
	{
		if (!(m_UIManager == null) && !(m_UIManager.mPanelLevelUp == null))
		{
			m_UIManager.mPanelLevelUp.Show(bShow);
			if (bShow)
			{
				m_UIManager.mPanelLevelUp.SetLevelContext(m_GameState.nLastLevel, m_GameState.nNowLevel);
				m_UIManager.mPanelLevelUp.SetHPContext(m_GameState.nLastHP, m_GameState.nNowHP);
			}
		}
	}

	public void AddTeamMateProtrait(int nUID)
	{
	}

	public void DelTeamMateProtrait(int nUID)
	{
	}

	public void ClearTeamMateProtrait()
	{
	}

	public void ResortTeamMateProtrait()
	{
	}

	public CUIProtraitInfo GetProtrait(int nUID)
	{
		if (nUID == -1)
		{
			return m_MainPlayerPortraitInfo;
		}
		return null;
	}

	public void SetProtraitIcon(string sName, int nUID = -1)
	{
		CUIProtraitInfo protrait = GetProtrait(nUID);
		if (protrait != null)
		{
			protrait.SetIcon(sName);
		}
	}

	public void SetProtraitName(string sName, int nUID = -1)
	{
		CUIProtraitInfo protrait = GetProtrait(nUID);
		if (protrait != null)
		{
			protrait.SetName(sName);
		}
	}

	public void SetProtraitLife(float fRate, int nUID = -1)
	{
		CUIProtraitInfo protrait = GetProtrait(nUID);
		if (protrait != null)
		{
			protrait.SetLife(fRate);
		}
	}

	public void SetProtraitExp(int nTimes, float fExpRate, int nUID = -1)
	{
		CUIProtraitInfo protrait = GetProtrait(nUID);
		if (protrait != null)
		{
			protrait.SetExp(fExpRate);
		}
	}

	public void SetProtraitLevel(int nLevel, int nUID = -1)
	{
		CUIProtraitInfo protrait = GetProtrait(nUID);
		if (protrait != null)
		{
			protrait.SetLevel(nLevel);
		}
	}

	public void PlayProtraitLevelAnim(int nUID = -1)
	{
		CUIProtraitInfo protrait = GetProtrait(nUID);
		if (protrait != null)
		{
			protrait.ShowLevelAnim();
		}
	}

	public void ShowPauseUI(bool bShow)
	{
		if (m_UIManager == null || m_UIManager.mGamePauseDialog == null)
		{
			return;
		}
		m_UIManager.mGamePauseDialog.gameObject.SetActiveRecursively(bShow);
		if (m_UIManager.mScreenMask != null)
		{
			m_UIManager.mScreenMask.ShowMask(bShow, -9f, -1);
		}
		float z = m_UIManager.mGamePauseDialog.transform.localPosition.z;
		if (bShow)
		{
			m_UIManager.mGamePauseDialog.transform.localPosition = new Vector3(0f, 0f, z);
			iDataCenter dataCenter = m_GameData.GetDataCenter();
			if (dataCenter != null)
			{
				if (m_UIManager.mGamePauseDialog.mMusicSwitch != null)
				{
					m_UIManager.mGamePauseDialog.mMusicSwitch.Switch(dataCenter.MusicSwitch);
				}
				if (m_UIManager.mGamePauseDialog.mSoundSwitch != null)
				{
					m_UIManager.mGamePauseDialog.mSoundSwitch.Switch(dataCenter.SoundSwitch);
				}
			}
			if (m_GameScene.CurGameLevelInfo != null && m_UIManager.mGamePauseDialog.mTaskDesc != null)
			{
				m_UIManager.mGamePauseDialog.mTaskDesc.text = m_GameScene.CurGameLevelInfo.sLevelDesc;
			}
		}
		else
		{
			m_UIManager.mGamePauseDialog.transform.localPosition = new Vector3(0f, 10000f, z);
		}
	}

	public void ShowIAPUI(bool bShow, int nParam = -1)
	{
		if (!(m_UIManager == null) && !(m_UIManager.mIAPDialog == null))
		{
			m_UIManager.mIAPDialog.Show(bShow);
			if (m_UIManager.mScreenMask != null)
			{
				m_UIManager.mScreenMask.ShowMask(bShow, -9f, -1);
			}
			if (bShow)
			{
				m_UIManager.mIAPDialog.SetTitleParam(nParam);
			}
		}
	}

	public void ShowMessageBox(string str, gyUIMessageBox.kMessageBoxType msgboxtype = gyUIMessageBox.kMessageBoxType.OK, gyUIEventRegister.OnClickFunc onok = null)
	{
		if (!(m_UIManager == null) && !(m_UIManager.mMessageBox == null))
		{
			m_UIManager.mMessageBox.SetMask(m_UIManager.mScreenMask);
			m_UIManager.mMessageBox.Message(str, msgboxtype, onok);
		}
	}

	public void HideMessageBox()
	{
		if (!(m_UIManager == null) && !(m_UIManager.mMessageBox == null))
		{
			m_UIManager.mMessageBox.SetMask(m_UIManager.mScreenMask);
			m_UIManager.mMessageBox.Hide();
		}
	}

	public void ShowStashFullDialog(bool bShow, int nNextStashCount = 1)
	{
		if (!(m_UIManager == null) && !(m_UIManager.mStashFullDialog == null))
		{
			m_UIManager.mStashFullDialog.Show(bShow);
			if (m_UIManager.mScreenMask != null)
			{
				m_UIManager.mScreenMask.ShowMask(bShow, -9f, -1);
			}
			if (bShow)
			{
				m_UIManager.mStashFullDialog.SetContent("Add Stash Size to " + nNextStashCount + "([11FF00]+5[-])");
			}
		}
	}

	public void ShowGameUI()
	{
		if (m_UIManager.mHeadPortrait != null)
		{
			m_UIManager.mHeadPortrait.SetActiveRecursively(true);
		}
		if (m_UIManager.mWeapon != null)
		{
			m_UIManager.mWeapon.Show(true);
		}
		if (m_UIManager.mFastWeapon != null)
		{
			m_UIManager.mFastWeapon.SetActiveRecursively(true);
		}
		if (m_UIManager.mSkill != null)
		{
			m_UIManager.mSkill.gameObject.SetActiveRecursively(true);
		}
		if (m_UIManager.mWheelMove != null)
		{
			m_UIManager.mWheelMove.gameObject.SetActiveRecursively(true);
		}
		if (m_UIManager.mWheelShoot != null)
		{
			m_UIManager.mWheelShoot.gameObject.SetActiveRecursively(true);
		}
		if (m_UIManager.mTaskPlane != null)
		{
			m_UIManager.mTaskPlane.SetActiveRecursively(true);
		}
		if (m_UIAimCross != null)
		{
			m_UIAimCross.gameObject.SetActiveRecursively(true);
		}
		if (m_UIManager.mPause != null)
		{
			m_UIManager.mPause.gameObject.SetActiveRecursively(true);
		}
		ShowShootUI(true);
	}

	public void HideGameUI()
	{
		if (m_UIManager.mHeadPortrait != null)
		{
			m_UIManager.mHeadPortrait.SetActiveRecursively(false);
		}
		if (m_UIManager.mWeapon != null)
		{
			m_UIManager.mWeapon.Show(false);
		}
		if (m_UIManager.mFastWeapon != null)
		{
			m_UIManager.mFastWeapon.SetActiveRecursively(false);
		}
		if (m_UIManager.mSkill != null)
		{
			m_UIManager.mSkill.gameObject.SetActiveRecursively(false);
		}
		if (m_UIManager.mWheelMove != null)
		{
			m_UIManager.mWheelMove.gameObject.SetActiveRecursively(false);
		}
		if (m_UIManager.mWheelShoot != null)
		{
			m_UIManager.mWheelShoot.gameObject.SetActiveRecursively(false);
		}
		if (m_UIManager.mTaskPlane != null)
		{
			m_UIManager.mTaskPlane.SetActiveRecursively(false);
		}
		if (m_UIAimCross != null)
		{
			m_UIAimCross.gameObject.SetActiveRecursively(false);
		}
		if (m_UIManager.mPause != null)
		{
			m_UIManager.mPause.gameObject.SetActiveRecursively(false);
		}
		ShowPauseUI(false);
		ShowShootUI(false);
	}

	public void ShowTutorials(bool bShow)
	{
		if (!(m_UIManager == null) && !(m_UIManager.mTutorialsPanel == null))
		{
			m_UIManager.mTutorialsPanel.Show(bShow);
		}
	}

	public void ShowSkipUI(bool bShow)
	{
		if (!(m_UIManager.mSkip == null))
		{
			m_UIManager.mSkip.gameObject.SetActiveRecursively(bShow);
		}
	}

	public void FadeIn(float fTime)
	{
		if (!(m_UIManager.mScreenMask == null))
		{
			m_UIManager.mScreenMask.FadeIn(fTime);
		}
	}

	public void FadeOut(float fTime)
	{
		if (!(m_UIManager.mScreenMask == null))
		{
			m_UIManager.mScreenMask.FadeOut(fTime);
		}
	}

	public void MovieUIIn(float fTime)
	{
		if (!(m_UIManager.mMovieMask == null))
		{
			m_UIManager.mMovieMask.MoveIn(false);
		}
	}

	public void MovieUIOut(float fTime)
	{
		if (!(m_UIManager.mMovieMask == null))
		{
			m_UIManager.mMovieMask.MoveOut(true);
		}
	}

	public void InitAimCross(int nWeaponType, float fPrecise)
	{
		if (m_nCurWeaponType != nWeaponType && m_UIAimCross != null)
		{
			Object.Destroy(m_UIAimCross.gameObject);
			m_UIAimCross = null;
		}
		m_nCurWeaponType = nWeaponType;
		if (m_UIAimCross == null)
		{
			GameObject original = null;
			switch (nWeaponType)
			{
			case 2:
				original = PrefabManager.Get("Artist/GameUI/NGUIAimPoint_AutoRifle");
				break;
			case 0:
				original = PrefabManager.Get("Artist/GameUI/NGUIAimPoint_HandGun");
				break;
			case 4:
				original = PrefabManager.Get("Artist/GameUI/NGUIAimPoint_HoldGun");
				break;
			case 1:
				original = PrefabManager.Get("Artist/GameUI/NGUIAimPoint_Melee");
				break;
			case 5:
				original = PrefabManager.Get("Artist/GameUI/NGUIAimPoint_Rocket");
				break;
			case 3:
				original = PrefabManager.Get("Artist/GameUI/NGUIAimPoint_ShootGun");
				break;
			}
			GameObject gameObject = Object.Instantiate(original) as GameObject;
			if (gameObject != null)
			{
				if (m_UIManager != null && m_UIManager.mAchorCenter != null)
				{
					gameObject.transform.parent = m_UIManager.mAchorCenter;
					gameObject.transform.localScale = Vector3.one;
					gameObject.transform.localPosition = Vector3.zero;
				}
				m_UIAimCross = gameObject.GetComponent<gyUIAimCross>();
			}
		}
		if (!(m_UIAimCross == null))
		{
			m_UIAimCross.Initialize(fPrecise);
		}
	}

	public void ExpandAimCross()
	{
		if (!(m_UIAimCross == null))
		{
			m_UIAimCross.Expand();
		}
	}

	public void ShowAchievementTip(string str, int star)
	{
		if (!(m_UIManager.mAchievementTip == null))
		{
			m_UIManager.mAchievementTip.ShowTip(str, star);
		}
	}

	public void ShowTip(string str)
	{
		GameObject gameObject = m_NGUITextTip.Get();
		if (!(gameObject == null))
		{
			gyUILabelDmg component = gameObject.GetComponent<gyUILabelDmg>();
			if (!(component == null))
			{
				component.SetLabel(str);
				component.Go(new Vector2(-Screen.width / 2, 0f));
			}
		}
	}

	public void SetSkillCD(float fTime)
	{
		if (m_UIManager != null && m_UIManager.mSkill != null)
		{
			m_UIManager.mSkill.SetCD(fTime);
		}
	}

	public void FinishSkillCD()
	{
		if (m_UIManager != null && m_UIManager.mSkill != null)
		{
			m_UIManager.mSkill.FinishCD();
		}
	}

	public void ShowScreenBlood(bool bShow, float fAlpha = 1f, float fTime = 0.5f)
	{
		if (!(m_UIManager == null) && !(m_UIManager.mScreenBloodMask == null))
		{
			m_UIManager.mScreenBloodMask.ShowMask(bShow, -10f, 10, fAlpha);
			if (bShow)
			{
				m_UIManager.mScreenBloodMask.FadeIn(fTime);
			}
		}
	}

	public void SetBulletNum(int nNum, int nMax, bool bAnim = false)
	{
		if (!(m_UIManager == null) && !(m_UIManager.mWeapon == null))
		{
			m_UIManager.mWeapon.SetBullet(nNum, nMax);
			if (bAnim)
			{
				m_UIManager.mWeapon.PlayFullBulletAnim();
			}
		}
	}

	public void ShowShootUI(bool bShow)
	{
		if (!(m_UIManager == null) && m_UIManager.mWheelShoot != null)
		{
			m_UIManager.mWheelShoot.gameObject.SetActiveRecursively(bShow);
		}
	}

	public void RegisterEvent()
	{
		gyUIEventRegister gyUIEventRegister2 = null;
		gyUIEventRegister2 = GetEventRegister(m_UIManager.mScreenTouch);
		gyUIEventRegister2.RegisterOnDrag(Event_SlipScreenFast);
		gyUIEventRegister2.RegisterOnClick(Event_Continue);
		gyUIEventRegister2 = GetEventRegister(m_UIManager.mWheelMove.gameObject);
		gyUIEventRegister2.RegisterOnPress(Event_Move);
		gyUIEventRegister2.RegisterOnDrag(Event_Move);
		gyUIEventRegister2 = GetEventRegister(m_UIManager.mWheelShoot.gameObject);
		gyUIEventRegister2.RegisterOnPress(Event_Shoot);
		gyUIEventRegister2.RegisterOnDrag(Event_SlipScreen);
		gyUIEventRegister2 = GetEventRegister(m_UIManager.mButtonShoot);
		gyUIEventRegister2.RegisterOnPress(Event_Shoot);
		gyUIEventRegister2 = GetEventRegister(m_UIManager.mTutorialsPanel.mMask);
		gyUIEventRegister2.RegisterOnClick(Event_ClickTutorial);
		gyUIEventRegister2 = GetEventRegister(m_UIManager.mPause);
		gyUIEventRegister2.RegisterOnClick(Event_Pause);
		gyUIEventRegister2 = GetEventRegister(m_UIManager.mSkip);
		gyUIEventRegister2.RegisterOnClick(Event_Skip);
		if (m_UIManager.mWeapon != null)
		{
			gyUIEventRegister2 = GetEventRegister(m_UIManager.mWeapon.gameObject);
			gyUIEventRegister2.RegisterOnClick(Event_SwitchWeapon);
			if (m_UIManager.mWeapon.mPurchase != null)
			{
				gyUIEventRegister2 = GetEventRegister(m_UIManager.mWeapon.mPurchase);
				gyUIEventRegister2.RegisterOnClick(Event_PurchaseBullet);
			}
		}
		gyUIEventRegister2 = GetEventRegister(m_UIManager.mFastWeapon);
		gyUIEventRegister2.RegisterOnClick(Event_SwitchWeaponPrev);
		gyUIEventRegister2 = GetEventRegister(m_UIManager.mSkill.gameObject);
		gyUIEventRegister2.RegisterOnClick(Event_UseSkill);
		if (m_UIManager.mPanelRevive != null)
		{
			gyUIEventRegister2 = GetEventRegister(m_UIManager.mPanelRevive.mReviveButton);
			gyUIEventRegister2.RegisterOnClick(Event_Revive);
		}
		if (m_UIManager.mPanelMaterial != null)
		{
			gyUIEventRegister2 = GetEventRegister(m_UIManager.mPanelMaterial.mTakeAllButton);
			gyUIEventRegister2.RegisterOnClick(Event_ClickTakeAllMaterial);
			gyUIEventRegister2 = GetEventRegister(m_UIManager.mPanelMaterial.mBackButton);
			gyUIEventRegister2.RegisterOnClick(Event_ClickCloseMaterialPanel);
			m_UIManager.mPanelMaterial.RegisterOnClickCell(Event_ClickMaterial);
			m_UIManager.mPanelMaterial.RegisterOnPressCell(Event_PressMaterial);
		}
		if (m_UIManager.mGamePauseDialog != null)
		{
			gyUIEventRegister2 = GetEventRegister(m_UIManager.mGamePauseDialog.mMusicSwitch.gameObject);
			gyUIEventRegister2.RegisterOnClick(Event_Pause_MusicSwitch);
			gyUIEventRegister2 = GetEventRegister(m_UIManager.mGamePauseDialog.mSoundSwitch.gameObject);
			gyUIEventRegister2.RegisterOnClick(Event_Pause_SoundSwitch);
			gyUIEventRegister2 = GetEventRegister(m_UIManager.mGamePauseDialog.mbtnClose);
			gyUIEventRegister2.RegisterOnClick(Event_Pause_Close);
			gyUIEventRegister2 = GetEventRegister(m_UIManager.mGamePauseDialog.mbtnContinue);
			gyUIEventRegister2.RegisterOnClick(Event_Pause_GoToMap);
			gyUIEventRegister2 = GetEventRegister(m_UIManager.mGamePauseDialog.mbtnVillage);
			gyUIEventRegister2.RegisterOnClick(Event_Pause_GoToVillage);
		}
		if (m_UIManager.mIAPDialog != null)
		{
			gyUIEventRegister2 = GetEventRegister(m_UIManager.mIAPDialog.mClose);
			gyUIEventRegister2.RegisterOnClick(Event_CloseIAPDialog);
			gyUIEventRegister2 = GetEventRegister(m_UIManager.mIAPDialog.arrUIIAPUnit[0].mButton);
			gyUIEventRegister2.RegisterOnClick(Event_PurchaseIAP1);
			gyUIEventRegister2 = GetEventRegister(m_UIManager.mIAPDialog.arrUIIAPUnit[1].mButton);
			gyUIEventRegister2.RegisterOnClick(Event_PurchaseIAP2);
			gyUIEventRegister2 = GetEventRegister(m_UIManager.mIAPDialog.arrUIIAPUnit[2].mButton);
			gyUIEventRegister2.RegisterOnClick(Event_PurchaseIAP3);
		}
		if (m_UIManager.mStashFullDialog != null)
		{
			gyUIEventRegister2 = GetEventRegister(m_UIManager.mStashFullDialog.mbtnClose);
			gyUIEventRegister2.RegisterOnClick(Event_StashFullDialogClose);
			gyUIEventRegister2 = GetEventRegister(m_UIManager.mStashFullDialog.mbtnIncrease);
			gyUIEventRegister2.RegisterOnClick(Event_StashFullDialogPurchase);
		}
	}

	public void RegisterEvent_Windows()
	{
		gyUIEventRegister gyUIEventRegister2 = null;
		gyUIEventRegister2 = GetEventRegister(m_UIManager.mScreenTouch);
		gyUIEventRegister2.RegisterOnPress(Event_Shoot);
		gyUIEventRegister2.RegisterOnClick(Event_Continue);
		gyUIEventRegister2 = GetEventRegister(m_UIManager.mTutorialsPanel.mMask);
		gyUIEventRegister2.RegisterOnClick(Event_ClickTutorial);
		gyUIEventRegister2 = GetEventRegister(m_UIManager.mPause);
		gyUIEventRegister2.RegisterOnClick(Event_Pause);
		gyUIEventRegister2 = GetEventRegister(m_UIManager.mSkip);
		gyUIEventRegister2.RegisterOnClick(Event_Skip);
		if (m_UIManager.mWeapon != null)
		{
			gyUIEventRegister2 = GetEventRegister(m_UIManager.mWeapon.gameObject);
			gyUIEventRegister2.RegisterOnClick(Event_SwitchWeapon);
			if (m_UIManager.mWeapon.mPurchase != null)
			{
				gyUIEventRegister2 = GetEventRegister(m_UIManager.mWeapon.mPurchase);
				gyUIEventRegister2.RegisterOnClick(Event_PurchaseBullet);
			}
		}
		gyUIEventRegister2 = GetEventRegister(m_UIManager.mFastWeapon);
		gyUIEventRegister2.RegisterOnClick(Event_SwitchWeaponPrev);
		gyUIEventRegister2 = GetEventRegister(m_UIManager.mSkill.gameObject);
		gyUIEventRegister2.RegisterOnClick(Event_UseSkill);
		if (m_UIManager.mPanelRevive != null)
		{
			gyUIEventRegister2 = GetEventRegister(m_UIManager.mPanelRevive.mReviveButton);
			gyUIEventRegister2.RegisterOnClick(Event_Revive);
		}
		if (m_UIManager.mPanelMaterial != null)
		{
			gyUIEventRegister2 = GetEventRegister(m_UIManager.mPanelMaterial.mTakeAllButton);
			gyUIEventRegister2.RegisterOnClick(Event_ClickTakeAllMaterial);
			gyUIEventRegister2 = GetEventRegister(m_UIManager.mPanelMaterial.mBackButton);
			gyUIEventRegister2.RegisterOnClick(Event_ClickCloseMaterialPanel);
			m_UIManager.mPanelMaterial.RegisterOnClickCell(Event_ClickMaterial);
			m_UIManager.mPanelMaterial.RegisterOnPressCell(Event_PressMaterial);
		}
		if (m_UIManager.mGamePauseDialog != null)
		{
			gyUIEventRegister2 = GetEventRegister(m_UIManager.mGamePauseDialog.mMusicSwitch.gameObject);
			gyUIEventRegister2.RegisterOnClick(Event_Pause_MusicSwitch);
			gyUIEventRegister2 = GetEventRegister(m_UIManager.mGamePauseDialog.mSoundSwitch.gameObject);
			gyUIEventRegister2.RegisterOnClick(Event_Pause_SoundSwitch);
			gyUIEventRegister2 = GetEventRegister(m_UIManager.mGamePauseDialog.mbtnClose);
			gyUIEventRegister2.RegisterOnClick(Event_Pause_Close);
			gyUIEventRegister2 = GetEventRegister(m_UIManager.mGamePauseDialog.mbtnContinue);
			gyUIEventRegister2.RegisterOnClick(Event_Pause_GoToMap);
			gyUIEventRegister2 = GetEventRegister(m_UIManager.mGamePauseDialog.mbtnVillage);
			gyUIEventRegister2.RegisterOnClick(Event_Pause_GoToVillage);
		}
		if (m_UIManager.mIAPDialog != null)
		{
			gyUIEventRegister2 = GetEventRegister(m_UIManager.mIAPDialog.mClose);
			gyUIEventRegister2.RegisterOnClick(Event_CloseIAPDialog);
			gyUIEventRegister2 = GetEventRegister(m_UIManager.mIAPDialog.arrUIIAPUnit[0].mButton);
			gyUIEventRegister2.RegisterOnClick(Event_PurchaseIAP1);
			gyUIEventRegister2 = GetEventRegister(m_UIManager.mIAPDialog.arrUIIAPUnit[1].mButton);
			gyUIEventRegister2.RegisterOnClick(Event_PurchaseIAP2);
			gyUIEventRegister2 = GetEventRegister(m_UIManager.mIAPDialog.arrUIIAPUnit[2].mButton);
			gyUIEventRegister2.RegisterOnClick(Event_PurchaseIAP3);
		}
		if (m_UIManager.mStashFullDialog != null)
		{
			gyUIEventRegister2 = GetEventRegister(m_UIManager.mStashFullDialog.mbtnClose);
			gyUIEventRegister2.RegisterOnClick(Event_StashFullDialogClose);
			gyUIEventRegister2 = GetEventRegister(m_UIManager.mStashFullDialog.mbtnIncrease);
			gyUIEventRegister2.RegisterOnClick(Event_StashFullDialogPurchase);
		}
	}

	protected void Event_Pause()
	{
		Debug.Log("click pause");
		CUISound.GetInstance().Play("UI_Button");
		m_GameScene.SetGamePause(true);
	}

	protected void Event_Skip()
	{
		Debug.Log("click skip");
		CCameraRoam.GetInstance().Stop();
	}

	protected void Event_Pause_MusicSwitch()
	{
		iDataCenter dataCenter = m_GameData.GetDataCenter();
		if (dataCenter != null)
		{
			dataCenter.MusicSwitch = !dataCenter.MusicSwitch;
			TAudioManager.instance.isMusicOn = dataCenter.MusicSwitch;
			if (m_UIManager != null && m_UIManager.mGamePauseDialog != null && m_UIManager.mGamePauseDialog.mMusicSwitch != null)
			{
				m_UIManager.mGamePauseDialog.mMusicSwitch.Switch(dataCenter.MusicSwitch);
			}
		}
	}

	protected void Event_Pause_SoundSwitch()
	{
		iDataCenter dataCenter = m_GameData.GetDataCenter();
		if (dataCenter != null)
		{
			dataCenter.SoundSwitch = !dataCenter.SoundSwitch;
			TAudioManager.instance.isSoundOn = dataCenter.SoundSwitch;
			if (m_UIManager != null && m_UIManager.mGamePauseDialog != null && m_UIManager.mGamePauseDialog.mSoundSwitch != null)
			{
				m_UIManager.mGamePauseDialog.mSoundSwitch.Switch(dataCenter.SoundSwitch);
			}
		}
	}

	protected void Event_Pause_AutoAimSwitch()
	{
	}

	protected void CallBack_GoToVillage()
	{
		CUISound.GetInstance().Play("UI_Button");
		iDataCenter dataCenter = m_GameData.GetDataCenter();
		if (dataCenter != null)
		{
			dataCenter.Save();
		}
		ShowPauseUI(false);
		FadeOut(0.5f);
		m_GameScene.LeaveGame(0.5f);
		if (m_GameScene.CurGameLevelInfo != null)
		{
			//iGameApp.GetInstance().Flurry_QuitStage(m_GameScene.CurGameLevelInfo.nID);
		}
	}

	protected void Event_Pause_GoToVillage()
	{
		CUISound.GetInstance().Play("UI_Button");
		ShowMessageBox("Are you sure?", gyUIMessageBox.kMessageBoxType.OKCANCEL, CallBack_GoToVillage);
	}

	protected void CallBack_GoToMap()
	{
		CUISound.GetInstance().Play("UI_Button");
		iDataCenter dataCenter = m_GameData.GetDataCenter();
		if (dataCenter != null)
		{
			dataCenter.Save();
		}
		ShowPauseUI(false);
		FadeOut(0.5f);
		m_GameScene.LeaveGame(0.5f, kGameSceneEnum.Map);
		if (m_GameScene.CurGameLevelInfo != null)
		{
			//iGameApp.GetInstance().Flurry_QuitStage(m_GameScene.CurGameLevelInfo.nID);
		}
	}

	protected void Event_Pause_GoToMap()
	{
		CUISound.GetInstance().Play("UI_Button");
		ShowMessageBox("Are you sure?", gyUIMessageBox.kMessageBoxType.OKCANCEL, CallBack_GoToMap);
	}

	protected void Event_Pause_Close()
	{
		CUISound.GetInstance().Play("UI_Cancle");
		iDataCenter dataCenter = m_GameData.GetDataCenter();
		if (dataCenter != null)
		{
			dataCenter.Save();
		}
		m_GameScene.SetGamePause(false);
	}

	protected void Event_Move(bool bPressed)
	{
		CCharUser user = m_GameScene.GetUser();
		if (user == null || !user.IsCanMove())
		{
			return;
		}
		if (bPressed)
		{
			Vector2 wheelOffSet = m_UIManager.mWheelMove.WheelOffSet;
			if (!(wheelOffSet == Vector2.zero))
			{
				float fRateX = Mathf.Clamp01(Mathf.Abs(wheelOffSet.x / 0.6f)) * (float)((wheelOffSet.x > 0f) ? 1 : (-1));
				float fRateY = Mathf.Clamp01(Mathf.Abs(wheelOffSet.y / 0.6f)) * (float)((wheelOffSet.y > 0f) ? 1 : (-1));
				user.MoveByCompass(fRateX, fRateY);
			}
		}
		else
		{
			user.MoveStop();
		}
	}

	protected void Event_Move(Vector2 delta)
	{
		CCharUser user = m_GameScene.GetUser();
		if (!(user == null) && user.IsCanMove())
		{
			Vector2 wheelOffSet = m_UIManager.mWheelMove.WheelOffSet;
			if (!(wheelOffSet == Vector2.zero))
			{
				float fRateX = Mathf.Clamp01(Mathf.Abs(wheelOffSet.x / 0.6f)) * (float)((wheelOffSet.x > 0f) ? 1 : (-1));
				float fRateY = Mathf.Clamp01(Mathf.Abs(wheelOffSet.y / 0.6f)) * (float)((wheelOffSet.y > 0f) ? 1 : (-1));
				user.MoveByCompass(fRateX, fRateY);
			}
		}
	}

	protected void Event_Shoot(bool bPressed)
	{
		if ((m_GameScene.GameStatus != iGameSceneBase.kGameStatus.Gameing && m_GameScene.GameStatus != iGameSceneBase.kGameStatus.GameOver_ShowTime) || (MyUtils.SimulatePlatform == PlatformEnum.Windows && ((bPressed && !Input.GetMouseButtonDown(0)) || (!bPressed && !Input.GetMouseButtonUp(0)))))
		{
			return;
		}
		CCharUser user = m_GameScene.GetUser();
		if (!(user == null))
		{
			user.SetFire(bPressed);
			m_SlipAssist.Tap();
			if (bPressed)
			{
				m_GameScene.AssistAim_Start();
			}
			else
			{
				m_GameScene.AssistAim_Stop();
			}
		}
	}

	protected void Event_SwitchWeapon()
	{
		if (m_GameScene.GameStatus != iGameSceneBase.kGameStatus.Gameing && m_GameScene.GameStatus != iGameSceneBase.kGameStatus.GameOver_ShowTime)
		{
			return;
		}
		CUISound.GetInstance().Play("UI_Weapon_change");
		CCharUser user = m_GameScene.GetUser();
		if (user == null)
		{
			return;
		}
		int curWeaponIndex = user.CurWeaponIndex;
		int num = curWeaponIndex + 1;
		while (num != curWeaponIndex && m_GameState.GetWeapon(num) == null)
		{
			num++;
			if (num >= 3)
			{
				num = 0;
			}
		}
		curWeaponIndex = num;
		user.SwitchWeapon(curWeaponIndex);
	}
	
	protected void Event_SwitchWeaponPrev()
	{
		if (m_GameScene.GameStatus != iGameSceneBase.kGameStatus.Gameing && m_GameScene.GameStatus != iGameSceneBase.kGameStatus.GameOver_ShowTime)
		{
			return;
		}
		CUISound.GetInstance().Play("UI_Weapon_change");
		CCharUser user = m_GameScene.GetUser();
		if (user == null)
		{
			return;
		}
		int curWeaponIndex = user.CurWeaponIndex;
		int num = curWeaponIndex - 1;
		while (num != curWeaponIndex && m_GameState.GetWeapon(num) == null)
		{
			num--;
			if (num < 0)
			{
				num = 2;
			}
		}
		curWeaponIndex = num;
		user.SwitchWeapon(curWeaponIndex);
	}

	protected void Event_UseSkill()
	{
		if (m_GameScene.GameStatus == iGameSceneBase.kGameStatus.Gameing || m_GameScene.GameStatus == iGameSceneBase.kGameStatus.GameOver_ShowTime)
		{
			CCharUser user = m_GameScene.GetUser();
			if (!(user == null) && user.IsCanAttack() && !user.IsSkillCD())
			{
				user.UseSkill(user.SkillID);
			}
		}
	}

	protected void Event_SlipScreenFast(Vector2 delta)
	{
		if (m_GameScene.GameStatus != iGameSceneBase.kGameStatus.Gameing && m_GameScene.GameStatus != iGameSceneBase.kGameStatus.GameOver_ShowTime)
		{
			return;
		}
		CCharUser user = m_GameScene.GetUser();
		iCameraTrail iCameraTrail2 = m_GameScene.GetCamera();
		if (!(delta != Vector2.zero) || !m_SlipAssist.Slip(delta))
		{
			return;
		}
		if (delta.x != 0f)
		{
			iCameraTrail2.Yaw(m_SlipAssist.m_fCurFrameYaw);
			if (user.IsCanAim())
			{
				user.SetYaw(iCameraTrail2.GetYaw());
			}
		}
		if (delta.y != 0f)
		{
			iCameraTrail2.Pitch(m_SlipAssist.m_fCurFramePitch);
		}
		if (user.IsCanAim() && (delta.x != 0f || delta.y != 0f))
		{
			user.LookAt(iCameraTrail2.ScreenPointToRay(m_GameState.ScreenCenter, 0f).GetPoint(1000f));
		}
	}

	protected void Event_SlipScreen(Vector2 delta)
	{
		if (m_GameScene.GameStatus != iGameSceneBase.kGameStatus.Gameing && m_GameScene.GameStatus != iGameSceneBase.kGameStatus.GameOver_ShowTime)
		{
			return;
		}
		CCharUser user = m_GameScene.GetUser();
		iCameraTrail iCameraTrail2 = m_GameScene.GetCamera();
		if (!(delta != Vector2.zero) || !m_SlipAssist.Slip(delta))
		{
			return;
		}
		if (delta.x != 0f)
		{
			iCameraTrail2.Yaw(m_SlipAssist.m_fCurFrameYaw * 0.5f);
			if (user.IsCanAim())
			{
				user.SetYaw(iCameraTrail2.GetYaw());
			}
		}
		if (delta.y != 0f)
		{
			iCameraTrail2.Pitch(m_SlipAssist.m_fCurFramePitch);
		}
		if (user.IsCanAim() && (delta.x != 0f || delta.y != 0f))
		{
			user.LookAt(iCameraTrail2.ScreenPointToRay(m_GameState.ScreenCenter, 0f).GetPoint(1000f));
		}
		if (Mathf.Abs(delta.x) > 1f && Mathf.Abs(delta.y) > 1f && m_GameScene.IsAssistAim())
		{
			m_GameScene.AssistAim_Stop();
		}
	}

	protected void Event_SlipScreen()
	{
		if (m_GameScene.GameStatus != iGameSceneBase.kGameStatus.Gameing && m_GameScene.GameStatus != iGameSceneBase.kGameStatus.GameOver_ShowTime)
		{
			return;
		}
		CCharUser user = m_GameScene.GetUser();
		iCameraTrail iCameraTrail2 = m_GameScene.GetCamera();
		Vector2 wheelOffSet = m_UIManager.mWheelShoot.WheelOffSet;
		if (!(Mathf.Abs(wheelOffSet.x) < 0.2f))
		{
			iCameraTrail2.Yaw(wheelOffSet.x * 1f);
			if (user.IsCanAim())
			{
				user.SetYaw(iCameraTrail2.GetYaw());
			}
			if (user.IsCanAim())
			{
				user.LookAt(iCameraTrail2.ScreenPointToRay(m_GameState.ScreenCenter, 0f).GetPoint(1000f));
			}
		}
	}

	protected void Event_Revive()
	{
		CUISound.GetInstance().Play("UI_Button");
		CCharUser user = m_GameScene.GetUser();
		if (user == null || !user.isDead || m_GameScene.GameStatus != iGameSceneBase.kGameStatus.GameOver_Revive)
		{
			return;
		}
		iDataCenter dataCenter = m_GameData.GetDataCenter();
		if (dataCenter == null)
		{
			return;
		}
		if (dataCenter.Crystal < 10)
		{
			m_GameScene.StartIAPPurchase(10 - dataCenter.Crystal);
			return;
		}
		dataCenter.AddCrystal(-10);
		dataCenter.Save();
		CUISound.GetInstance().Play("UI_Crystal");
		m_GameScene.ReviveGame();
        if (MyUtils.isWindows)
        {
            Screen.lockCursor = false;
        }
        /*CFlurryManager.GetInstance().ConsumeCrystal(CFlurryManager.kConsumeType.Revive);
		if (m_GameScene.CurGameLevelInfo != null)
		{
			iGameApp.GetInstance().Flurry_CharRevive(m_GameScene.CurGameLevelInfo.nID);
		}*/
    }

	protected void Event_Continue()
	{
		if (m_GameScene.GameStatus != iGameSceneBase.kGameStatus.GameOver)
		{
			return;
		}
		CUISound.GetInstance().Play("UI_Button");
		if (m_GameScene.isMissionSuccess)
		{
			if (m_UIManager == null || m_UIManager.mPanelMissionComplete == null)
			{
				return;
			}
			if (m_GameScene.GameStatusStep == 1)
			{
				if (m_UIManager.mPanelMissionComplete.IsContextHop())
				{
					m_UIManager.mPanelMissionComplete.StopContextHop();
					m_GameScene.GameStatusStep = 2;
				}
				return;
			}
			if (m_GameScene.GameStatusStep == 2)
			{
				CCharUser user = m_GameScene.GetUser();
				if (user == null || m_GameScene.CurGameLevelInfo == null)
				{
					return;
				}
				if (m_GameState.nLastLevel < user.Level)
				{
					m_GameState.nNowLevel = user.Level;
					CCharacterInfoLevel characterInfo = m_GameData.GetCharacterInfo(user.ID, m_GameState.nLastLevel);
					CCharacterInfoLevel characterInfo2 = m_GameData.GetCharacterInfo(user.ID, user.Level);
					if (characterInfo != null && characterInfo2 != null)
					{
						m_GameState.nLastHP = (int)characterInfo.fLifeBase;
						m_GameState.nNowHP = (int)characterInfo2.fLifeBase;
					}
					m_GameScene.GameStatusStep = 3;
					m_GameScene.GameStatusStepTime = 2f;
					ShowSuccess(false);
					ShowLevelUp(true);
				}
				else if (m_GameState.HasAnyMaterial())
				{
					ShowSuccess(false);
					m_GameScene.GameOver_TakeMaterial();
				}
				else
				{
					m_GameScene.LeaveGame(0f);
				}
			}
			if (m_GameScene.GameStatusStep == 3)
			{
			}
			if (m_GameScene.GameStatusStep == 4)
			{
				if (m_GameState.HasAnyMaterial())
				{
					ShowLevelUp(false);
					m_GameScene.GameOver_TakeMaterial();
				}
				else
				{
					m_GameScene.LeaveGame(0f);
				}
			}
		}
		else
		{
			m_GameScene.LeaveGame(0f);
		}
	}

	protected void Event_PressMaterial(int nIndex, bool bPress)
	{
		if (m_UIManager == null || m_UIManager.mPanelMaterial == null)
		{
			return;
		}
		CMaterialInfo gainMaterial = m_GameState.GetGainMaterial(nIndex);
		if (gainMaterial == null)
		{
			return;
		}
		if (bPress)
		{
			CItemInfoLevel itemInfo = m_GameData.GetItemInfo(gainMaterial.nItemID, 1);
			if (itemInfo != null)
			{
				m_UIManager.mPanelMaterial.ShowMaterialName(nIndex, itemInfo.sName);
			}
		}
		else
		{
			m_UIManager.mPanelMaterial.HideMaterialName();
		}
	}

	protected void Event_ClickMaterial(int nIndex)
	{
		if (m_UIManager == null || m_UIManager.mPanelMaterial == null)
		{
			return;
		}
		iDataCenter dataCenter = m_GameData.GetDataCenter();
		if (dataCenter == null)
		{
			return;
		}
		iGameData gameData = iGameApp.GetInstance().m_GameData;
		if (gameData == null)
		{
			return;
		}
		CMaterialInfo gainMaterial = m_GameState.GetGainMaterial(nIndex);
		if (gainMaterial == null || gainMaterial.nItemID == -1 || gainMaterial.nItemCount <= 0)
		{
			return;
		}
		int num = dataCenter.CheckStashVolume(1);
		if (num < 1)
		{
			Debug.Log("stash full, please purchase more stash pos!");
			int stashCapacityCount = gameData.GetStashCapacityCount(dataCenter.StashLevel + 1);
			if (stashCapacityCount > 0)
			{
				ShowStashFullDialog(true, stashCapacityCount);
			}
			return;
		}
		CUISound.GetInstance().Play("UI_Material_get");
		dataCenter.AddMaterialNum(gainMaterial.nItemID, num);
		gainMaterial.nItemCount -= num;
		m_UIManager.mPanelMaterial.SetIconAnimate(nIndex, num);
		if (gainMaterial.nItemCount <= 0)
		{
			gainMaterial.nItemID = -1;
			m_UIManager.mPanelMaterial.ShowItem(nIndex, false);
		}
		else
		{
			m_UIManager.mPanelMaterial.SetCount(nIndex, gainMaterial.nItemCount);
		}
		bool flag = true;
		int gainMaterialCount = m_GameState.GetGainMaterialCount();
		for (int i = 0; i < gainMaterialCount; i++)
		{
			CMaterialInfo gainMaterial2 = m_GameState.GetGainMaterial(i);
			if (gainMaterial2 != null && gainMaterial2.nItemID != -1 && gainMaterial2.nItemCount > 0)
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			dataCenter.Save();
			m_UIManager.mPanelMaterial.HideMainFrame();
			FadeOut(2f);
			m_GameScene.LeaveGame(2f);
		}
	}

	protected void Event_ClickTakeAllMaterial()
	{
		CUISound.GetInstance().Play("UI_Takeall");
		if (m_UIManager == null || m_UIManager.mPanelMaterial == null)
		{
			return;
		}
		iDataCenter dataCenter = m_GameData.GetDataCenter();
		if (dataCenter == null)
		{
			return;
		}
		iGameData gameData = iGameApp.GetInstance().m_GameData;
		if (gameData == null)
		{
			return;
		}
		int gainMaterialCount = m_GameState.GetGainMaterialCount();
		if (gainMaterialCount < 1)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < gainMaterialCount; i++)
		{
			CMaterialInfo gainMaterial = m_GameState.GetGainMaterial(i);
			if (gainMaterial == null || gainMaterial.nItemID == -1 || gainMaterial.nItemCount <= 0)
			{
				continue;
			}
			num = dataCenter.CheckStashVolume(gainMaterial.nItemCount);
			if (num < 1)
			{
				int stashCapacityCount = gameData.GetStashCapacityCount(dataCenter.StashLevel + 1);
				if (stashCapacityCount > 0)
				{
					ShowStashFullDialog(true, stashCapacityCount);
				}
				return;
			}
			dataCenter.AddMaterialNum(gainMaterial.nItemID, num);
			gainMaterial.nItemCount -= num;
			m_UIManager.mPanelMaterial.SetIconAnimate(i, num);
			if (gainMaterial.nItemCount <= 0)
			{
				gainMaterial.nItemID = -1;
				m_UIManager.mPanelMaterial.ShowItem(i, false);
			}
			else
			{
				m_UIManager.mPanelMaterial.SetCount(i, gainMaterial.nItemCount);
			}
		}
		dataCenter.Save();
		m_UIManager.mPanelMaterial.HideMainFrame();
		FadeOut(2f);
		m_GameScene.LeaveGame(2f);
	}

	protected void Event_ClickBackToHome()
	{
		CUISound.GetInstance().Play("UI_Button");
		FadeOut(0.5f);
		m_GameScene.LeaveGame(0.5f);
	}

	protected void Event_ClickCloseMaterialPanel()
	{
		CUISound.GetInstance().Play("UI_Cancle");
		m_UIManager.mPanelMaterial.HideMainFrame();
		FadeOut(0.5f);
		m_GameScene.LeaveGame(0.5f);
	}

	protected void Event_PurchaseBullet()
	{
		CUISound.GetInstance().Play("UI_Button");
		iDataCenter dataCenter = m_GameData.GetDataCenter();
		if (dataCenter == null)
		{
			return;
		}
		if (dataCenter.Crystal < 10)
		{
			m_GameScene.StartIAPPurchase(10 - dataCenter.Crystal);
			return;
		}
		dataCenter.AddCrystal(-10);
		dataCenter.Save();
		CUISound.GetInstance().Play("UI_Crystal");
		m_GameScene.FullWeaponBullet();
		/*CFlurryManager.GetInstance().ConsumeCrystal(CFlurryManager.kConsumeType.Bullet);
		if (m_GameScene.CurGameLevelInfo != null)
		{
			iGameApp.GetInstance().Flurry_PurchaseBullet(m_GameScene.CurGameLevelInfo.nID);
		}*/
	}

	protected void Event_CloseIAPDialog()
	{
		CUISound.GetInstance().Play("UI_Cancle");
		m_GameScene.FinishIAPPurchase();
	}

	protected void PurchaseIAP(int nIndex)
	{
		if (!(m_UIManager == null) && !(m_UIManager.mIAPDialog == null))
		{
			gyUIIAPUnit iAPUnit = m_UIManager.mIAPDialog.GetIAPUnit(nIndex);
			if (!(iAPUnit == null))
			{
				m_GameScene.PurchaseIAP(iAPUnit.nIAPID);
				ShowIAPUI(false);
				ShowMessageBox("Waiting......", gyUIMessageBox.kMessageBoxType.None);
			}
		}
	}

	protected void Event_PurchaseIAP1()
	{
		PurchaseIAP(0);
	}

	protected void Event_PurchaseIAP2()
	{
		PurchaseIAP(1);
	}

	protected void Event_PurchaseIAP3()
	{
		PurchaseIAP(2);
	}

	protected void Event_StashFullDialogClose()
	{
		ShowStashFullDialog(false);
	}

	protected void Event_StashFullDialogPurchase()
	{
		iGameData gameData = iGameApp.GetInstance().m_GameData;
		if (gameData == null)
		{
			return;
		}
		iDataCenter dataCenter = gameData.GetDataCenter();
		if (dataCenter == null)
		{
			return;
		}
		int stashLevel = dataCenter.StashLevel;
		CStashCapacity stashCapacity = gameData.GetStashCapacity(stashLevel);
		CStashCapacity stashCapacity2 = gameData.GetStashCapacity(stashLevel + 1);
		if (stashCapacity == null || stashCapacity2 == null || !stashCapacity.isCrystalPurchase)
		{
			return;
		}
		if (dataCenter.Crystal >= stashCapacity.nPrice)
		{
			dataCenter.StashLevel++;
			dataCenter.AddCrystal(-stashCapacity.nPrice);
			dataCenter.Save();
			if (m_UIManager != null && m_UIManager.mPanelMaterial != null)
			{
				m_UIManager.mPanelMaterial.SetStashCur(dataCenter.StashCount);
				m_UIManager.mPanelMaterial.SetStashMax(dataCenter.StashCountMax, true);
			}
			//CFlurryManager.GetInstance().ConsumeCrystal(CFlurryManager.kConsumeType.StashSize);
		}
		else
		{
			m_GameScene.StartIAPPurchase(stashCapacity.nPrice - dataCenter.Crystal);
		}
		ShowStashFullDialog(false);
	}

	protected void Event_ClickTutorial()
	{
		m_GameScene.FinishTutorial();
	}

	protected gyUIEventRegister GetEventRegister(GameObject o)
	{
		gyUIEventRegister gyUIEventRegister2 = o.GetComponent<gyUIEventRegister>();
		if (gyUIEventRegister2 == null)
		{
			gyUIEventRegister2 = o.AddComponent<gyUIEventRegister>();
		}
		return gyUIEventRegister2;
	}
}
