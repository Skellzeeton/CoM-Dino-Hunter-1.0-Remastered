using TNetSdk;

public class nmsg_monster_dead : nmsg_struct
{
	public int m_nMobUID;

	public kDeadMode m_DeadMode;

	public override SFSObject Pack()
	{
		SFSObject sFSObject = new SFSObject();
		sFSObject.PutInt("mobuid", m_nMobUID);
		sFSObject.PutInt("deadmode", (int)m_DeadMode);
		return sFSObject;
	}

	public override void UnPack(SFSObject data)
	{
		m_nMobUID = data.GetInt("mobuid");
		m_DeadMode = (kDeadMode)data.GetInt("deadmode");
	}
}
