using TNetSdk;
using UnityEngine;

public class nmsg_playerbeatback : nmsg_struct
{
	public int m_nPlayerUID;

	public Vector3 m_v3Dst;

	public override SFSObject Pack()
	{
		SFSObject sFSObject = new SFSObject();
		sFSObject.PutInt("playeruid", m_nPlayerUID);
		sFSObject.PutFloatArray("dst", new float[3] { m_v3Dst.x, m_v3Dst.y, m_v3Dst.z });
		return sFSObject;
	}

	public override void UnPack(SFSObject data)
	{
		m_nPlayerUID = data.GetInt("playeruid");
		float[] floatArray = data.GetFloatArray("dst");
		m_v3Dst = new Vector3(floatArray[0], floatArray[1], floatArray[2]);
	}
}
