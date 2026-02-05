using UnityEngine;

public class gyUIManager : MonoBehaviour
{
	public Transform mParent;

	public Transform mAchorCenter;

	public GameObject mHeadPortrait;

	public UISprite mHeadIcon;

	public GameObject mPause;

	public GameObject mSkip;

	public iGameUIWeapon mWeapon;

	public GameObject mFastWeapon;

	public gyUISkillButton mSkill;

	public gyUIWheelButton mWheelMove;

	public gyUIWheelButton mWheelShoot;

	public GameObject mButtonShoot;

	public gyUIScreenMask mScreenMask;

	public gyUIMovieMask mMovieMask;

	public GameObject mScreenTouch;

	public iUIAchievementTip mAchievementTip;

	public GameObject mTaskPlane;

	public gyUIScreenMask mScreenBloodMask;

	public gyUIPanelMissionSuccess mPanelMissionComplete;

	public gyUIPanelMissionFailed mPanelMissionFailed;

	public gyUIPanelRevive mPanelRevive;

	public gyUIPanelMissionSuccessLevelUp mPanelLevelUp;

	public gyUIPanelMaterial mPanelMaterial;

	public gyUIGamePauseDialog mGamePauseDialog;

	public gyUIIAPIngame mIAPDialog;

	public gyUIMessageBox mMessageBox;

	public gyUIStashFullDialog mStashFullDialog;

	public gyUITutorialsPanel mTutorialsPanel;
}
