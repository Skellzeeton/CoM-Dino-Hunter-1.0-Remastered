using System.Collections.Generic;
using TNetSdk;
using UnityEngine;

public class nmsg_startgame : nmsg_struct
{
	public class CPlayerPos
	{
		public int nUID;

		public Vector3 v3Pos;
	}

	public int nGameLevel;

	public List<CPlayerPos> ltPlayerPos = new List<CPlayerPos>();

	public override SFSObject Pack()
	{
		SFSObject sFSObject = new SFSObject();
		sFSObject.PutInt("gamelevel", nGameLevel);
		sFSObject.PutInt("playercount", ltPlayerPos.Count);
		for (int i = 0; i < ltPlayerPos.Count; i++)
		{
			CPlayerPos cPlayerPos = ltPlayerPos[i];
			sFSObject.PutInt("player" + i, cPlayerPos.nUID);
			sFSObject.PutFloatArray("playerpos" + i, new float[3]
			{
				cPlayerPos.v3Pos.x,
				cPlayerPos.v3Pos.y,
				cPlayerPos.v3Pos.z
			});
		}
		return sFSObject;
	}

	public override void UnPack(SFSObject data)
	{
		nGameLevel = data.GetInt("gamelevel");
		int @int = data.GetInt("playercount");
		for (int i = 0; i < @int; i++)
		{
			CPlayerPos cPlayerPos = new CPlayerPos();
			cPlayerPos.nUID = data.GetInt("player" + i);
			float[] floatArray = data.GetFloatArray("playerpos" + i);
			cPlayerPos.v3Pos = new Vector3(floatArray[0], floatArray[1], floatArray[2]);
			ltPlayerPos.Add(cPlayerPos);
		}
	}
}
