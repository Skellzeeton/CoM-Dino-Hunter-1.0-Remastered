using TNetSdk;
using UnityEngine;

public class nmsg_game_additem : nmsg_struct
{
	public int nItemUID;

	public int nItemID;

	public Vector3 v3Pos;

	public override SFSObject Pack()
	{
		SFSObject sFSObject = new SFSObject();
		sFSObject.PutInt("itemuid", nItemUID);
		sFSObject.PutInt("itemid", nItemID);
		sFSObject.PutFloatArray("pos", new float[3] { v3Pos.x, v3Pos.y, v3Pos.z });
		return sFSObject;
	}

	public override void UnPack(SFSObject data)
	{
		nItemUID = data.GetInt("itemuid");
		nItemID = data.GetInt("itemid");
		float[] floatArray = data.GetFloatArray("pos");
		v3Pos = new Vector3(floatArray[0], floatArray[1], floatArray[2]);
	}
}
