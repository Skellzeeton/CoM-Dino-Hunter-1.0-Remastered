using System.Collections.Generic;
using UnityEngine;

public class iSceneDamage : MonoBehaviour
{
    public enum kMode
    {
        Once = 0,
        Continue = 1
    }

    protected class CHurtTargetInfo
    {
        public CCharBase target;
        public float time;
    }

    public kMode  m_Mode;
    
    public float  m_fInvertal;
    
    public float  m_fDamage;

    public int    m_nBuffID;
    
    public float  m_fBuffTime;

    protected iGameSceneBase m_GameScene;
    
    protected iGameUIBase    m_GameUI;
    
    protected bool           m_bActive;

    protected List<CHurtTargetInfo> m_ltHurtTarget;
    
    protected List<CHurtTargetInfo> m_ltHurtTargetDestroy;

    private void Awake()
    {
        m_ltHurtTarget        = new List<CHurtTargetInfo>();
        m_ltHurtTargetDestroy = new List<CHurtTargetInfo>();
    }

    private void Start()  { }

    private void Update()
    {
        if (!m_bActive) return;

        if (m_GameScene == null)
            m_GameScene = iGameApp.GetInstance().m_GameScene;
        if (m_GameUI == null && m_GameScene != null)
            m_GameUI = m_GameScene.GetGameUI();
        if (m_GameScene == null || m_GameScene.isPause) return;

        foreach (CHurtTargetInfo item in m_ltHurtTarget)
        {
            item.time -= Time.deltaTime;
            if (item.time <= 0f)
                m_ltHurtTargetDestroy.Add(item);
        }
        foreach (CHurtTargetInfo item2 in m_ltHurtTargetDestroy)
            m_ltHurtTarget.Remove(item2);
        m_ltHurtTargetDestroy.Clear();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (m_bActive && m_Mode != kMode.Continue && m_GameScene != null && !m_GameScene.isPause)
        {
            CCharBase component = collider.transform.root.GetComponent<CCharBase>();
            if (component != null && !component.isDead)
                MakeDamage(component);
        }
    }

    private void OnTriggerStay(Collider collider)
    {
        if (m_bActive && m_Mode != kMode.Once && m_GameScene != null && !m_GameScene.isPause)
        {
            CCharBase component = collider.transform.root.GetComponent<CCharBase>();
            if (component != null && !component.isDead)
                MakeDamage(component);
        }
    }

    protected void ResetTime(CCharBase target)
    {
        CHurtTargetInfo info = new CHurtTargetInfo();
        info.time   = m_fInvertal;
        info.target = target;
        m_ltHurtTarget.Add(info);
    }

    protected bool IsCanDamage(CCharBase target)
    {
        foreach (CHurtTargetInfo item in m_ltHurtTarget)
            if (item.target == target) return false;
        return true;
    }

    protected void MakeDamage(CCharBase target)
    {
        if (!IsCanDamage(target)) return;
        ResetTime(target);
        if (m_GameScene == null || !m_GameScene.IsMyself(target) || target.isDead) return;
        float maxHP   = GetTargetMaxHP(target);
        float damage  = maxHP > 0f ? maxHP * (m_fDamage / 100f) : m_fDamage;
        target.OnHit(0f - damage, null, string.Empty);
        m_GameScene.AddDamageText(damage, target.GetBone(1).position, Color.red);
        if (m_nBuffID > 0)
            target.AddBuff(m_nBuffID, m_fBuffTime);
    }

    private float GetTargetMaxHP(CCharBase target)
    {
        CCharMob mob = target as CCharMob;
        if (mob != null) return mob.HPMAX;

        CCharUser user = target as CCharUser;
        if (user != null) return user.MaxHP;

        return 0f;
    }

    public void SetActive(bool bActive)
    {
        m_bActive = bActive;
    }
}