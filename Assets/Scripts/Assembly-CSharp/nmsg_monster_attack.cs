using TNetSdk;

public class nmsg_monster_attack : nmsg_struct
{
	public int m_nMobUID;

	public int m_nTargetUID;

	public int m_nComboSkillID;

	public override SFSObject Pack()
	{
		SFSObject sFSObject = new SFSObject();
		sFSObject.PutInt("mobuid", m_nMobUID);
		sFSObject.PutInt("targetuid", m_nTargetUID);
		sFSObject.PutInt("comboskillid", m_nComboSkillID);
		return sFSObject;
	}

	public override void UnPack(SFSObject data)
	{
		m_nMobUID = data.GetInt("mobuid");
		m_nTargetUID = data.GetInt("targetuid");
		m_nComboSkillID = data.GetInt("comboskillid");
	}
}
