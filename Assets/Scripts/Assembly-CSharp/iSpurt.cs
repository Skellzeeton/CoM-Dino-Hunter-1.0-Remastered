using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class EffectPair
{
	public int OriginalID;
	public int OverrideID;
}

public class iSpurt : _iAnimEventBase
{
	public List<EffectPair> effectPairs = new List<EffectPair>();

	public void iSpurt_PlayEffect(int nPrefabID)
	{
		int idToUse = nPrefabID;
		bool found = false;
		foreach (var pair in effectPairs)
		{
			if (pair.OriginalID == nPrefabID)
			{
				idToUse = pair.OverrideID;
				found = true;
				break;
			}
		}
		if (!found)
		{
			idToUse = nPrefabID;
		}
		PlayEffect(idToUse);
	}

	protected override void TransformRefresh(GameObject o)
	{
		base.TransformRefresh(o);
		o.transform.forward = m_Node.up;
		o.transform.position = o.transform.position + o.transform.forward * 0.2f;
	}
}