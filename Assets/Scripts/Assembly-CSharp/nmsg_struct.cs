using TNetSdk;

public abstract class nmsg_struct
{
	public kGameNetEnum msghead;

	public abstract SFSObject Pack();

	public abstract void UnPack(SFSObject data);
}
