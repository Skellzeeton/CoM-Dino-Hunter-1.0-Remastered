using TNetSdk;

public class nmsg_playerskill : nmsg_struct
{
	public int m_nSkillID;

	public int m_nTargetUID;

	public override SFSObject Pack()
	{
		SFSObject sFSObject = new SFSObject();
		sFSObject.PutInt("skillid", m_nSkillID);
		sFSObject.PutInt("targetuid", m_nTargetUID);
		return sFSObject;
	}

	public override void UnPack(SFSObject data)
	{
		m_nSkillID = data.GetInt("skillid");
		m_nTargetUID = data.GetInt("targetuid");
	}
}
