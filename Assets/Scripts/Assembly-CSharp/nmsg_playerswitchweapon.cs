using TNetSdk;

public class nmsg_playerswitchweapon : nmsg_struct
{
	public int nWeaponID;

	public int nWeaponLevel;

	public override SFSObject Pack()
	{
		SFSObject sFSObject = new SFSObject();
		sFSObject.PutInt("weaponid", nWeaponID);
		sFSObject.PutInt("weaponlevel", nWeaponLevel);
		return sFSObject;
	}

	public override void UnPack(SFSObject data)
	{
		nWeaponID = data.GetInt("weaponid");
		nWeaponLevel = data.GetInt("weaponlevel");
	}
}
