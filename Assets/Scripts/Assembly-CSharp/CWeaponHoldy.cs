using System;
using System.Collections.Generic;
using UnityEngine;

public class CWeaponHoldy : CWeaponBase
{
    private const int EFF_FATAL_BOSS = 1904;
    protected float m_fRadius;
    protected float m_fAngle;
    protected float m_fEffectTime;
    protected float m_fEffectTimeCount;
    protected GameObject m_FireEffect;
    protected ParticleSystem[] m_arrParticleSystem;
    
    protected override void OnEquip(CCharPlayer player)
    {
        if (player == null || m_pWeaponLvlInfo == null)
            return;
        RefreshBulletUI();
        if (m_FireEffect != null)
        {
            UnityEngine.Object.Destroy(m_FireEffect);
            m_FireEffect = null;
        }
        GameObject prefab = PrefabManager.Get(m_pWeaponLvlInfo.nFire);
        if (prefab == null)
            return;
        m_FireEffect = UnityEngine.Object.Instantiate(prefab);
        if (m_FireEffect == null)
            return;
        Transform shootTf = player.GetShootMouseTf();
        if (shootTf != null)
        {
            m_FireEffect.transform.SetParent(shootTf);
            m_FireEffect.transform.localPosition = Vector3.zero;
            m_FireEffect.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
        }
        m_arrParticleSystem = m_FireEffect.GetComponentsInChildren<ParticleSystem>(true);
        SetParticleEmission(false);
    }

    protected override void OnDestroy()
    {
        if (m_FireEffect != null)
        {
            UnityEngine.Object.Destroy(m_FireEffect);
            m_FireEffect = null;
        }
        m_arrParticleSystem = null;
    }
    
    protected override void OnFire(CCharPlayer player)
    {
        if (player == null || m_pWeaponLvlInfo == null)
            return;
        if (!player.IsCanAttack())
            return;
        m_fFireLightTime = 1.5f;
        player.PlayAnimMix(kAnimEnum.Attack, WrapMode.Loop, 1f);
        player.PlayAudio(m_pWeaponLvlInfo.sAudioFire);
        SetParticleEmission(true);
        m_fRadius = 0f;
        m_fAngle = 0f;
        m_fEffectTime = 0.5f;
        m_pWeaponLvlInfo.GetAtkModeValue(0, ref m_fRadius);
        m_pWeaponLvlInfo.GetAtkModeValue(1, ref m_fAngle);
        m_pWeaponLvlInfo.GetAtkModeValue(2, ref m_fEffectTime);
        m_fEffectTimeCount = m_fEffectTime;
    }

    protected override void OnStop(CCharPlayer player)
    {
        if (player != null)
        {
            player.StopAction(kAnimEnum.Attack);
            if (m_pWeaponLvlInfo != null)
            {
                player.StopAudio(m_pWeaponLvlInfo.sAudioFire);
                if (m_pWeaponLvlInfo.nElementType == 3)
                    player.PlayAudio("Weapon_ice_end");
                else
                    player.PlayAudio("Weapon_flame_end");
            }
        }
        SetParticleEmission(false);
    }

    private void SetParticleEmission(bool enabled)
    {
        if (m_arrParticleSystem == null)
            return;
        foreach (ParticleSystem ps in m_arrParticleSystem)
        {
            if (ps == null)
                continue;
            var emission = ps.emission;
            emission.enabled = enabled;
            if (enabled)
                ps.Play(true);
            else
                ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    protected override void OnUpdate(CCharPlayer player, float deltaTime)
    {
        if (player == null || m_pWeaponLvlInfo == null)
            return;
        if (m_fFireIntervalCount < m_fFireInterval)
            m_fFireIntervalCount += deltaTime;
        if (!m_bFire)
            return;
        if (!player.IsCanAttack())
        {
            Stop(player);
            return;
        }
        m_fEffectTimeCount += deltaTime;
        if (m_fEffectTimeCount < m_fEffectTime)
            return;
        m_fEffectTimeCount = 0f;
        if (IsBulletEmpty)
        {
            player.PlayAudio("Weapon_nobullet_flamethrower");
            Stop(player);
            return;
        }
        ConsumeBullet();
        ShowFireLight(true);
        iGameUIBase gameUI = m_GameScene.GetGameUI();
        if (gameUI != null)
            gameUI.ExpandAimCross();
        Vector3 fireOrigin = player.GetUpBodyPos() + new Vector3(0f, 0.7f, 0f);
        Dictionary<int, CCharMob> mobData = m_GameScene.GetMobData();
        foreach (CCharMob mob in mobData.Values)
        {
            if (mob == null || mob.isDead)
                continue;
            Vector3 toMob = mob.Pos - player.Pos;
            if (toMob.sqrMagnitude > m_fRadius * m_fRadius)
                continue;
            if (m_fAngle > 0f)
            {
                toMob.y = 0f;
                float dot = Vector3.Dot(player.Dir2D, toMob.normalized);
                if (dot < Mathf.Cos(m_fAngle * Mathf.Deg2Rad / 2f))
                    continue;
            }
            Vector3 hitDir = mob.Pos - player.Pos;
            Vector3 hitPos = mob.GetBloodPos(fireOrigin, hitDir);
            m_GameScene.AddHitEffect(hitPos, hitDir, m_pWeaponLvlInfo.nHit);
            if (!isNetPlayerShoot)
                OnHitMob(player, mob, hitPos, hitDir, string.Empty);
            mob.PlayAudio(kAudioEnum.HitBody);
            switch (m_pWeaponLvlInfo.nElementType)
            {
                case 1:
                    mob.PlayAudio("Fx_Impact_fire");
                    break;
                case 2:
                    mob.PlayAudio("Fx_Impact_electric");
                    break;
                case 3:
                    mob.PlayAudio("Fx_Impact_freeze");
                    break;
            }
        }
    }

    protected override void OnHitMob(CCharPlayer player, CCharMob mob, Vector3 hitpos, Vector3 hitdir, string sBodyPart = "")
    {
        mob.SetLifeBarParam(1f);
        float damage = player.CalcWeaponDamage(m_pWeaponLvlInfo);
        float critChance = player.CalcCritical(m_pWeaponLvlInfo);
        float critBonus = player.CalcCriticalDmg(m_pWeaponLvlInfo);
        bool isCritical = false;
        if (critChance > UnityEngine.Random.Range(1f, 100f))
        {
            damage *= 1f + critBonus / 100f;
            isCritical = true;
        }
        damage *= 1f - mob.CalcProtect() / 100f;
        if (damage < 1f)
            damage = 1f;
        mob.OnHit(-damage, m_pWeaponLvlInfo, string.Empty);
        m_GameScene.AddDamageText(damage, hitpos, isCritical);
        m_GameScene.AddHitEffect(hitpos, Vector3.forward, 1115);
        iGameLogic.HitInfo hitinfo = new iGameLogic.HitInfo
        {
            v3HitDir = hitdir,
            v3HitPos = hitpos
        };
        m_GameLogic = m_GameScene.GetGameLogic();
        if (m_GameLogic != null)
        {
            m_GameLogic.CaculateFunc(player, mob,
                m_pWeaponLvlInfo.arrFunc,
                m_pWeaponLvlInfo.arrValueX,
                m_pWeaponLvlInfo.arrValueY,
                ref hitinfo);
            m_GameLogic.ltDamageInfo.Add(damage);
            CGameNetSender.GetInstance().BattleDamageMob(mob.UID, m_GameLogic.ltDamageInfo);
        }
        if (!mob.isDead)
            return;
        if (mob.IsBoss())
        {
            m_GameScene.AddEffect(mob.GetBone(1).position, Vector3.forward, 4.25f, EFF_FATAL_BOSS);
        }
        CMobInfoLevel mobInfo = mob.GetMobInfo();
        if (mobInfo != null)
        {
            int exp = mobInfo.nExp;
            float bonus = player.Property.GetValue(kProEnum.Char_IncreaseExp);
            if (bonus > 0f)
                exp = (int)(exp * (1f + bonus / 100f));
            player.AddExp(exp);
            m_GameScene.AddExpText(exp, hitinfo.v3HitPos);
        }
    }
}