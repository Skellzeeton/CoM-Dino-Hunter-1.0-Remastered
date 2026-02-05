using System.Collections.Generic;
using UnityEngine;

public class CRewardMaterial
{
	public int nID;

	public List<int> ltCount;

	public List<float> ltCountRate;

	public CRewardMaterial()
	{
		ltCount = new List<int>();
		ltCountRate = new List<float>();
	}

	public int GetDropCount()
	{
		float[] array = new float[ltCount.Count];
		for (int i = 0; i < ltCount.Count && i < ltCountRate.Count; i++)
		{
			if (i == 0)
			{
				array[i] = ltCountRate[i];
			}
			else
			{
				array[i] = array[i - 1] + ltCountRate[i];
			}
		}
		float num = Random.Range(0f, array[ltCount.Count - 1]);
		for (int j = 0; j < ltCount.Count; j++)
		{
			if (num <= array[j])
			{
				return ltCount[j];
			}
		}
		return -1;
	}
}
