using TNetSdk;

public class nmsg_playertakeitem : nmsg_struct
{
	public int nItemUID;

	public override SFSObject Pack()
	{
		SFSObject sFSObject = new SFSObject();
		sFSObject.PutInt("itemuid", nItemUID);
		return sFSObject;
	}

	public override void UnPack(SFSObject data)
	{
		nItemUID = data.GetInt("itemuid");
	}
}
