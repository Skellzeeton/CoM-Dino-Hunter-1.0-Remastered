using UnityEngine;

public class CWeaponSpawn : CWeaponBase
{
	protected override void OnEquip(CCharPlayer player)
	{
		RefreshBulletUI();
	}

	protected override void OnFire(CCharPlayer player)
	{
		if (!player.IsCanAttack())
		{
			return;
		}
		if (base.IsBulletEmpty)
		{
			CUISound.GetInstance().Play("Weapon_nobullet_rpg");
			Stop(player);
			return;
		}
		ConsumeBullet();
		ShowFireLight(true);
		iGameUIBase gameUI = m_GameScene.GetGameUI();
		if (gameUI != null)
		{
			gameUI.ExpandAimCross();
		}
		player.PlayAnimMix(kAnimEnum.Attack, WrapMode.ClampForever, 1f);
		int nValue = 0;
		float fValue = 0f;
		m_pWeaponLvlInfo.GetAtkModeValue(0, ref nValue);
		m_pWeaponLvlInfo.GetAtkModeValue(1, ref fValue);
		Vector3 shootMouse = player.GetShootMouse();
		Vector3 vector = player.m_v3CurNetAimDir;
		if (!base.isNetPlayerShoot)
		{
			vector = Camera.main.ScreenPointToRay(m_GameState.GetScreenCenterV3()).direction;
		}
		vector *= fValue;
		iSpawnBullet iSpawnBullet2 = m_GameScene.AddSpawn(player.UID, nValue, shootMouse, vector, m_pWeaponLvlInfo.arrFunc, m_pWeaponLvlInfo.arrValueX, m_pWeaponLvlInfo.arrValueY);
		if (iSpawnBullet2 != null)
		{
			iSpawnBullet2.m_pWeaponLvlInfo = m_pWeaponLvlInfo;
		}
		player.PlayAudio(m_pWeaponLvlInfo.sAudioFire);
		m_GameScene.AddFireEffect(player.GetShootMouseTf(), vector, m_pWeaponLvlInfo.nFire, 2f);
		m_GameScene.AddFireEffect(player.GetWeaponBackTf(), -vector, 1009, 2f);
	}

	protected override void OnUpdate(CCharPlayer player, float deltaTime)
	{
		if (m_fFireIntervalCount < m_fFireInterval)
		{
			m_fFireIntervalCount += deltaTime;
		}
		if (m_bFire && !(m_fFireIntervalCount < m_fFireInterval))
		{
			m_fFireIntervalCount = 0f;
			OnFire(player);
		}
	}
}
