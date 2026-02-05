using System.Globalization;
using UnityEngine;

public class ServerX : MonoBehaviour
{
	public static float ParseFloat(string str)
	{
		return float.Parse(str, CultureInfo.InvariantCulture);
	}
}