using TNetSdk;

public class nmsg_game_playerinfo : nmsg_struct
{
	public int nCharID;

	public int nCharLevel;

	public int nWeaponID;

	public override SFSObject Pack()
	{
		SFSObject sFSObject = new SFSObject();
		sFSObject.PutInt("charid", nCharID);
		sFSObject.PutInt("charlevel", nCharLevel);
		sFSObject.PutInt("charweapon", nWeaponID);
		return sFSObject;
	}

	public override void UnPack(SFSObject data)
	{
		nCharID = data.GetInt("charid");
		nCharLevel = data.GetInt("charlevel");
		nWeaponID = data.GetInt("charweapon");
	}
}
