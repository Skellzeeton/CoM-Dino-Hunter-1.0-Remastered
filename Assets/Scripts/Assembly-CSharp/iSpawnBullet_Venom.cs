using UnityEngine;

public class iSpawnBullet_Venom : iSpawnBullet_Parabola
{
	public int nGroundVenom;

	protected override void OnHitGround(Vector3 v3Hit)
	{
		GameObject gameObject = m_GameScene.AddSceneGameObject(nGroundVenom, v3Hit, Vector3.forward, -1f);
		if (!(gameObject == null))
		{
			iSceneDamage component = gameObject.GetComponent<iSceneDamage>();
			if (component != null)
			{
				component.SetActive(true);
			}
		}
	}
}
