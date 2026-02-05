using TNetSdk;

public class nmsg_task_getitem : nmsg_struct
{
	public int m_nItemID;

	public override SFSObject Pack()
	{
		SFSObject sFSObject = new SFSObject();
		sFSObject.PutInt("itemid", m_nItemID);
		return sFSObject;
	}

	public override void UnPack(SFSObject data)
	{
		m_nItemID = data.GetInt("itemid");
	}
}
