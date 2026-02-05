using TNetSdk;

public class nmsg_battle_damage_player : nmsg_struct
{
	public float m_fDamage;

	public override SFSObject Pack()
	{
		SFSObject sFSObject = new SFSObject();
		sFSObject.PutFloat("damage", m_fDamage);
		return sFSObject;
	}

	public override void UnPack(SFSObject data)
	{
		m_fDamage = data.GetFloat("damage");
	}
}
