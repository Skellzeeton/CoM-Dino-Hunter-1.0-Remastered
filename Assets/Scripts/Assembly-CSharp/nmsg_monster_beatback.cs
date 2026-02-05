using TNetSdk;
using UnityEngine;

public class nmsg_monster_beatback : nmsg_struct
{
	public int m_nMobUID;

	public Vector3 m_v3Dst;

	public override SFSObject Pack()
	{
		SFSObject sFSObject = new SFSObject();
		sFSObject.PutInt("mobuid", m_nMobUID);
		sFSObject.PutFloatArray("dst", new float[3] { m_v3Dst.x, m_v3Dst.y, m_v3Dst.z });
		return sFSObject;
	}

	public override void UnPack(SFSObject data)
	{
		m_nMobUID = data.GetInt("mobuid");
		float[] floatArray = data.GetFloatArray("dst");
		m_v3Dst = new Vector3(floatArray[0], floatArray[1], floatArray[2]);
	}
}
