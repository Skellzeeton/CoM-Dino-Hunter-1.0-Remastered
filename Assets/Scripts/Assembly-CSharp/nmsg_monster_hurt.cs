using TNetSdk;

public class nmsg_monster_hurt : nmsg_struct
{
	public int m_nMobUID;

	public kAnimEnum m_HurtAnim;

	public override SFSObject Pack()
	{
		SFSObject sFSObject = new SFSObject();
		sFSObject.PutInt("mobuid", m_nMobUID);
		sFSObject.PutInt("hurtanim", (int)m_HurtAnim);
		return sFSObject;
	}

	public override void UnPack(SFSObject data)
	{
		m_nMobUID = data.GetInt("mobuid");
		m_HurtAnim = (kAnimEnum)data.GetInt("hurtanim");
	}
}
