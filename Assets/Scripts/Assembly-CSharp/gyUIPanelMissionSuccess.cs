using UnityEngine;

public class gyUIPanelMissionSuccess : MonoBehaviour
{
    public GameObject mLightBase;
    public GameObject mLightAnim;
    public GameObject mTitleText;
    public GameObject mTitleIcon;
    public GameObject mStatisticsBackground;
    public GameObject mStatisticsContext1;
    public GameObject mStatisticsContext2;
    public GameObject mStatisticsContext3;
    public gyUIHopNumber mContext1;
    public gyUIHopNumber mContext2;
    public gyUIHopNumber mContext3;
    protected bool m_bShow;
    protected int m_nStep;
    protected float m_fStepCount;

    private void Awake()
    {
        m_bShow = false;
        gameObject.SetActiveRecursively(false);
    }

    private void Update()
    {
        if (!m_bShow)
            return;
        float deltaTime = Time.deltaTime;
        switch (m_nStep)
        {
            case 0:
                m_fStepCount -= deltaTime;
                if (m_fStepCount <= 0f)
                {
                    mLightAnim.SetActiveRecursively(true);
                    mTitleText.SetActiveRecursively(true);
                    TweenPosition tween = TweenPosition.Begin(mTitleText, 0.5f, Vector3.zero);
                    tween.from = new Vector3(3f, 260f, 0f);
                    tween.to = new Vector3(3f, 97f, 0f);
                    tween.method = UITweener.Method.BounceIn;
                    m_nStep = 1;
                    m_fStepCount = 0.2f;
                }
                break;
            case 1:
                m_fStepCount -= deltaTime;
                if (m_fStepCount <= 0f)
                {
                    mTitleIcon.SetActiveRecursively(true);

                    TweenPosition tween = TweenPosition.Begin(mTitleIcon, 0.5f, Vector3.zero);
                    tween.from = new Vector3(-120f, 260f, 0f);
                    tween.to = new Vector3(-120f, 83f, 0f);
                    tween.method = UITweener.Method.EaseIn;

                    m_nStep = 2;
                    m_fStepCount = 0.2f;
                }
                break;
            case 2:
                m_fStepCount -= deltaTime;
                if (m_fStepCount <= 0f)
                {
                    mStatisticsBackground.SetActiveRecursively(true);
                    m_nStep = 3;
                    m_fStepCount = 0.2f;
                }
                break;
            case 3:
                m_fStepCount -= deltaTime;
                if (m_fStepCount <= 0f)
                {
                    mStatisticsContext1.SetActiveRecursively(true);
                    m_nStep = 4;
                    m_fStepCount = 0.5f;
                }
                break;
            case 4:
                m_fStepCount -= deltaTime;
                if (m_fStepCount <= 0f)
                {
                    mStatisticsContext2.SetActiveRecursively(true);
                    m_nStep = 5;
                    m_fStepCount = 0.5f;
                }
                break;
            case 5:
                m_fStepCount -= deltaTime;
                if (m_fStepCount <= 0f)
                {
                    mStatisticsContext3.SetActiveRecursively(true);
                    m_nStep = 6;
                    m_fStepCount = 0.5f;
                }
                break;
        }
    }

    public void Show(bool bShow)
    {
        m_bShow = bShow;
        gameObject.SetActiveRecursively(bShow);
        mLightBase.SetActiveRecursively(false);
        mLightAnim.SetActiveRecursively(false);
        mTitleText.SetActiveRecursively(false);
        mTitleIcon.SetActiveRecursively(false);
        mStatisticsBackground.SetActiveRecursively(false);
        mStatisticsContext1.SetActiveRecursively(false);
        mStatisticsContext2.SetActiveRecursively(false);
        mStatisticsContext3.SetActiveRecursively(false);
        transform.localPosition = bShow
            ? new Vector3(0f, 0f, transform.localPosition.z)
            : new Vector3(10000f, 10000f, transform.localPosition.z);
        if (!bShow)
            return;
        mLightBase.SetActiveRecursively(true);
        TweenScale tween = TweenScale.Begin(mLightBase, 0.5f, Vector3.one);
        tween.from = Vector3.zero;
        tween.to = Vector3.one;
        tween.method = UITweener.Method.EaseIn;
        m_nStep = 0;
        m_fStepCount = 0.5f;
    }

    public void SetGainExp(int nValue)
    {
        if (mContext1 != null)
            mContext1.Go(0f, nValue, Mathf.Clamp01(nValue / 300f) * 5f);
    }

    public void SetGainGold(int nValue)
    {
        if (mContext2 != null)
            mContext2.Go(0f, nValue, Mathf.Clamp01(nValue / 300f) * 5f);
    }

    public void SetGainGoldEarned(int nValue)
    {
        if (mContext3 != null)
            mContext3.Go(0f, nValue, Mathf.Clamp01(nValue / 300f) * 5f);
    }

    public bool IsContextHop()
    {
        if (mContext1 == null || !mContext1.gameObject.activeSelf || !mContext1.isHop)
            return false;
        if (mContext2 == null || !mContext2.gameObject.activeSelf || !mContext2.isHop)
            return false;
        if (mContext3 == null || !mContext3.gameObject.activeSelf || !mContext3.isHop)
            return false;
        return true;
    }

    public void StopContextHop()
    {
        if (mContext1 != null) mContext1.Stop();
        if (mContext2 != null) mContext2.Stop();
        if (mContext3 != null) mContext3.Stop();
    }
}
