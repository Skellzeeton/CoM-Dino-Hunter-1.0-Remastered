using TNetSdk;
using UnityEngine;

public class nmsg_playeraim : nmsg_struct
{
	public Vector3 v3AimPoint;

	public override SFSObject Pack()
	{
		SFSObject sFSObject = new SFSObject();
		sFSObject.PutFloatArray("aimpoint", new float[3] { v3AimPoint.x, v3AimPoint.y, v3AimPoint.z });
		return sFSObject;
	}

	public override void UnPack(SFSObject data)
	{
		float[] floatArray = data.GetFloatArray("aimpoint");
		v3AimPoint = new Vector3(floatArray[0], floatArray[1], floatArray[2]);
	}
}
