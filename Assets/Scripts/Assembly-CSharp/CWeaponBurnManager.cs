using System;
using System.Collections.Generic;
using UnityEngine;

public class CWeaponBurnManager : MonoBehaviour
{
    public const int TickCount = 10;
    public const float TickInterval = 1f;
    public const float DisplayInterval = 1f;

    private const float NormalSlowPerStack = 0.02f;
    private const float NormalMaxSlow = 0.35f;
    private const float BossSlowPerStack = 0.0075f;
    private const float BossMaxSlow = 0.175f;

    private class BurnState
    {
        public float tickDamage;
        public int ticksRemaining;
        public float timer;
        public Action<CCharMob, float> applyDamage;
    }

    private class DisplayState
    {
        public float accumulatedDamage;
        public float timer;
        public Action<CCharMob, float> showText;
    }

    private static CWeaponBurnManager s_Instance;

    public static CWeaponBurnManager Instance
    {
        get
        {
            if (s_Instance == null)
            {
                GameObject go = new GameObject("[CWeaponBurnManager]");
                UnityEngine.Object.DontDestroyOnLoad(go);
                s_Instance = go.AddComponent<CWeaponBurnManager>();
            }
            return s_Instance;
        }
    }

    private readonly Dictionary<CCharMob, List<BurnState>> m_Burns = new Dictionary<CCharMob, List<BurnState>>();
    private readonly Dictionary<CCharMob, DisplayState> m_Displays = new Dictionary<CCharMob, DisplayState>();
    private readonly List<CCharMob> m_PendingMobRemoval = new List<CCharMob>();
    private readonly List<BurnState> m_PendingBurnRemoval = new List<BurnState>();

    private readonly Dictionary<CCharMob, float> m_OriginalMoveSpeeds = new Dictionary<CCharMob, float>();

    private void Awake()
    {
        if (s_Instance != null && s_Instance != this)
        {
            UnityEngine.Object.Destroy(gameObject);
            return;
        }
        s_Instance = this;
    }

    private void OnDestroy()
    {
        if (s_Instance == this)
            s_Instance = null;
        m_Burns.Clear();
        m_Displays.Clear();
        m_PendingMobRemoval.Clear();
        m_PendingBurnRemoval.Clear();
        m_OriginalMoveSpeeds.Clear();
    }

    private bool IsGameActive()
    {
        iGameApp app = iGameApp.GetInstance();
        if (app == null)
            return true;
        iGameSceneBase scene = app.m_GameScene;
        if (scene == null)
            return true;
        if (scene.isPause)
            return false;
        switch (scene.GameStatus)
        {
            case iGameSceneBase.kGameStatus.Pause:
            case iGameSceneBase.kGameStatus.CutScene:
            case iGameSceneBase.kGameStatus.GameTutorial:
            case iGameSceneBase.kGameStatus.GameOver_Process:
            case iGameSceneBase.kGameStatus.GameOver_ShowTime:
            case iGameSceneBase.kGameStatus.GameOver_Revive:
            case iGameSceneBase.kGameStatus.GameOver:
            case iGameSceneBase.kGameStatus.GameOver_Material:
            case iGameSceneBase.kGameStatus.GameOver_LeaveScene:
                return false;
            default:
                return true;
        }
    }

    public void ApplyBurn(CCharMob mob, float baseDamage, float totalPercent,
            Action<CCharMob, float> applyDamage,
            Action<CCharMob, float> showText)
    {
        if (mob == null || mob.isDead || applyDamage == null)
            return;
        if (totalPercent <= 0f || baseDamage <= 0f)
            return;
        float tickDamage = baseDamage * (totalPercent / 100f) / TickCount;
        if (tickDamage <= 0f)
            return;
        List<BurnState> list;
        if (!m_Burns.TryGetValue(mob, out list))
        {
            list = new List<BurnState>();
            m_Burns[mob] = list;
        }
        list.Add(new BurnState
        {
            tickDamage = tickDamage,
            ticksRemaining = TickCount,
            timer = TickInterval,
            applyDamage = applyDamage
        });
        RefreshBurnSlow(mob);
        if (showText != null)
        {
            DisplayState display;
            if (!m_Displays.TryGetValue(mob, out display))
            {
                display = new DisplayState
                {
                    accumulatedDamage = 0f,
                    timer = DisplayInterval,
                    showText = showText
                };
                m_Displays[mob] = display;
            }
            else
            {
                display.showText = showText;
            }
        }
    }

    private void RefreshBurnSlow(CCharMob mob)
    {
        if (mob == null)
            return;
        List<BurnState> burns;
        if (!m_Burns.TryGetValue(mob, out burns) || burns.Count <= 0 || mob.isDead)
        {
            RestoreMobSpeed(mob);
            return;
        }
        float originalSpeed;
        if (!m_OriginalMoveSpeeds.TryGetValue(mob, out originalSpeed))
        {
            originalSpeed = mob.Property.GetValue(kProEnum.MoveSpeed);
            m_OriginalMoveSpeeds[mob] = originalSpeed;
        }
        bool isBoss = mob.IsBoss();
        float slowPerStack = isBoss ? BossSlowPerStack : NormalSlowPerStack;
        float maxSlow = isBoss ? BossMaxSlow : NormalMaxSlow;
        float slow = Mathf.Min(maxSlow, burns.Count * slowPerStack);
        float speedMultiplier = 1f - slow;
        mob.Property.SetValueBase(kProEnum.MoveSpeed, originalSpeed * speedMultiplier);
    }

    private void RestoreMobSpeed(CCharMob mob)
    {
        if (ReferenceEquals(mob, null))
            return;
        float originalSpeed;
        if (m_OriginalMoveSpeeds.TryGetValue(mob, out originalSpeed))
        {
            if (mob != null && mob.Property != null)
            {
                mob.Property.SetValueBase(kProEnum.MoveSpeed, originalSpeed);
            }
            m_OriginalMoveSpeeds.Remove(mob);
        }
    }

    private void Update()
    {
        if (!IsGameActive())
            return;
        float dt = Time.deltaTime;
        if (m_Burns.Count > 0)
        {
            m_PendingMobRemoval.Clear();
            foreach (KeyValuePair<CCharMob, List<BurnState>> kvp in m_Burns)
            {
                CCharMob mob = kvp.Key;
                List<BurnState> burns = kvp.Value;
                if (mob == null || mob.isDead)
                {
                    m_PendingMobRemoval.Add(mob);
                    continue;
                }
                m_PendingBurnRemoval.Clear();
                for (int i = 0; i < burns.Count; i++)
                {
                    BurnState state = burns[i];
                    state.timer -= dt;
                    while (state.timer <= 0f && state.ticksRemaining > 0)
                    {
                        state.timer += TickInterval;
                        state.ticksRemaining--;
                        DisplayState display;
                        if (m_Displays.TryGetValue(mob, out display))
                            display.accumulatedDamage += state.tickDamage;
                        state.applyDamage(mob, state.tickDamage);
                        if (mob == null || mob.isDead)
                            break;
                    }
                    if (state.ticksRemaining <= 0 || mob == null || mob.isDead)
                        m_PendingBurnRemoval.Add(state);
                }
                for (int i = 0; i < m_PendingBurnRemoval.Count; i++)
                    burns.Remove(m_PendingBurnRemoval[i]);
                if (burns.Count == 0 || mob == null || mob.isDead)
                {
                    m_PendingMobRemoval.Add(mob);
                }
                else
                {
                    RefreshBurnSlow(mob);
                }
            }
            for (int i = 0; i < m_PendingMobRemoval.Count; i++)
            {
                CCharMob mob = m_PendingMobRemoval[i];
                m_Burns.Remove(mob);
                RestoreMobSpeed(mob);
            }
        }
        if (m_Displays.Count > 0)
        {
            m_PendingMobRemoval.Clear();
            foreach (KeyValuePair<CCharMob, DisplayState> kvp in m_Displays)
            {
                CCharMob mob = kvp.Key;
                DisplayState display = kvp.Value;
                if (mob == null || mob.isDead)
                {
                    m_PendingMobRemoval.Add(mob);
                    continue;
                }
                display.timer -= dt;
                if (display.timer <= 0f)
                {
                    display.timer = DisplayInterval;

                    if (display.accumulatedDamage > 0f)
                    {
                        display.showText(mob, display.accumulatedDamage);
                        display.accumulatedDamage = 0f;
                    }
                }
                if (!m_Burns.ContainsKey(mob) && display.accumulatedDamage <= 0f)
                    m_PendingMobRemoval.Add(mob);
            }
            for (int i = 0; i < m_PendingMobRemoval.Count; i++)
            {
                CCharMob mob = m_PendingMobRemoval[i];
                m_Displays.Remove(mob);
                if (!m_Burns.ContainsKey(mob))
                    RestoreMobSpeed(mob);
            }
        }
    }
}