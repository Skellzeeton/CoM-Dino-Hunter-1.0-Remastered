using TNetSdk;

public class nmsg_battle_damage_mob : nmsg_struct
{
	public int m_nMobUID;

	public float m_fDamage;

	public override SFSObject Pack()
	{
		SFSObject sFSObject = new SFSObject();
		sFSObject.PutInt("mobuid", m_nMobUID);
		sFSObject.PutFloat("damage", m_fDamage);
		return sFSObject;
	}

	public override void UnPack(SFSObject data)
	{
		m_nMobUID = data.GetInt("mobuid");
		m_fDamage = data.GetFloat("damage");
	}
}
