using TNetSdk;

public class nmsg_playershoot : nmsg_struct
{
	public bool bShoot;

	public override SFSObject Pack()
	{
		SFSObject sFSObject = new SFSObject();
		sFSObject.PutBool("isShoot", bShoot);
		return sFSObject;
	}

	public override void UnPack(SFSObject data)
	{
		bShoot = data.GetBool("isShoot");
	}
}
