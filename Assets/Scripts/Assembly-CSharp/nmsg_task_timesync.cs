using TNetSdk;

public class nmsg_task_timesync : nmsg_struct
{
	public float m_fGameTime;

	public override SFSObject Pack()
	{
		SFSObject sFSObject = new SFSObject();
		sFSObject.PutFloat("gametime", m_fGameTime);
		return sFSObject;
	}

	public override void UnPack(SFSObject data)
	{
		m_fGameTime = data.GetFloat("gametime");
	}
}
