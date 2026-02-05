using TNetSdk;

public class nmsg_task_compelete : nmsg_struct
{
	public bool m_isSuccess;

	public override SFSObject Pack()
	{
		SFSObject sFSObject = new SFSObject();
		sFSObject.PutBool("issuccess", m_isSuccess);
		return sFSObject;
	}

	public override void UnPack(SFSObject data)
	{
		m_isSuccess = data.GetBool("issuccess");
	}
}
