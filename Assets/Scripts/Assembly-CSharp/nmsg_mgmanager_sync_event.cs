using TNetSdk;

public class nmsg_mgmanager_sync_event : nmsg_struct
{
	public int m_nEventIndex;

	public override SFSObject Pack()
	{
		SFSObject sFSObject = new SFSObject();
		sFSObject.PutInt("eventindex", m_nEventIndex);
		return sFSObject;
	}

	public override void UnPack(SFSObject data)
	{
		m_nEventIndex = data.GetInt("eventindex");
	}
}
