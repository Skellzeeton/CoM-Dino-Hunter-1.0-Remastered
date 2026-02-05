using TNetSdk;
using UnityEngine;

public class nmsg_playermove : nmsg_struct
{
	public Vector3 v3Src;

	public Vector3 v3Dst;

	public override SFSObject Pack()
	{
		SFSObject sFSObject = new SFSObject();
		sFSObject.PutFloatArray("src", new float[3] { v3Src.x, v3Src.y, v3Src.z });
		sFSObject.PutFloatArray("dst", new float[3] { v3Dst.x, v3Dst.y, v3Dst.z });
		return sFSObject;
	}

	public override void UnPack(SFSObject data)
	{
		float[] floatArray = data.GetFloatArray("src");
		v3Src = new Vector3(floatArray[0], floatArray[1], floatArray[2]);
		floatArray = data.GetFloatArray("dst");
		v3Dst = new Vector3(floatArray[0], floatArray[1], floatArray[2]);
	}
}
