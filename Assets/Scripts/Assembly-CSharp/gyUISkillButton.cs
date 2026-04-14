using UnityEngine;

public class gyUISkillButton : MonoBehaviour
{
	public UIFilledSprite mMask;
	public UISprite mIcon;
	public UISprite mIconAnim;
	protected bool m_bCD;
	protected float m_fTime;
	protected float m_fTimeCount;
	private bool m_bFinishTweenPlayed;

	private void Awake()
	{
		if (mMask != null)
		{
			mMask.fillAmount = 0f;
		}
		m_bFinishTweenPlayed = false;
	}

	private void Update()
	{
		if (!m_bCD)
		{
			return;
		}
		iGameApp app = iGameApp.GetInstance();
		if (app != null && app.m_GameScene != null)
		{
			if (app.m_GameScene.GameStatus == iGameSceneBase.kGameStatus.Pause)
			{
				return;
			}
		}
		CCharUser user = GetCurrentUser();
		if (user != null && user.CurSkillCD > 0f)
		{
			m_fTime = user.CurSkillCD;
			m_fTimeCount = user.CurSkillCDCount;
		}
		else
		{
			m_fTimeCount += Time.deltaTime;
		}
		if (m_fTime <= 0f)
		{
			FinishCD();
			return;
		}
		if (m_fTimeCount >= m_fTime)
		{
			FinishCD();
		}
		else if (mMask != null)
		{
			mMask.fillAmount = 1f - (m_fTimeCount / m_fTime);
		}
	}

	private CCharUser GetCurrentUser()
	{
		iGameApp app = iGameApp.GetInstance();
		if (app == null || app.m_GameScene == null)
		{
			return null;
		}
		return app.m_GameScene.GetUser();
	}

	public void SetIcon(string str)
	{
		if (mMask != null && mIcon != null && mIconAnim != null)
		{
			mMask.spriteName = str;
			mIcon.spriteName = str;
			mIconAnim.spriteName = str;
		}
	}

	public void SetCD(float fTime)
	{
		m_bCD = true;
		m_fTime = fTime;
		m_fTimeCount = 0f;
		m_bFinishTweenPlayed = false;
		if (mMask != null)
		{
			mMask.fillAmount = 1f;
		}
	}

	public void FinishCD()
	{
		m_bCD = false;
		m_fTimeCount = m_fTime;

		if (mMask != null)
		{
			mMask.fillAmount = 0f;
		}

		if (m_bFinishTweenPlayed)
		{
			return;
		}
		m_bFinishTweenPlayed = true;
		if (mIconAnim != null)
		{
			TweenAlpha tweenAlpha = TweenAlpha.Begin(mIconAnim.gameObject, 0.5f, 0f);
			tweenAlpha.from = 1f;
			tweenAlpha.to = 0f;
			TweenScale tweenScale = TweenScale.Begin(mIconAnim.gameObject, 0.5f, Vector3.zero);
			tweenScale.from = mIcon.transform.localScale;
			tweenScale.to = tweenScale.from * 2f;
		}
	}
}