using System.Xml;
using UnityEngine;

public class MyUtils
{
	protected static int m_nUIDCount;

	protected static PlatformEnum m_SimulatePlatform;

	public static PlatformEnum SimulatePlatform
	{
		get
		{
			return m_SimulatePlatform;
		}
		set
		{
			m_SimulatePlatform = value;
		}
	}

	public static bool isWindows
	{
		get
		{
			return m_SimulatePlatform == PlatformEnum.Windows;
		}
	}

	public static bool isIOS
	{
		get
		{
			return m_SimulatePlatform == PlatformEnum.IOS;
		}
	}

	public static bool isAndroid
	{
		get
		{
			return m_SimulatePlatform == PlatformEnum.Android;
		}
	}

	public static bool isPad
	{
		get
		{
			if (Screen.width == 2048 || Screen.width == 1024)
			{
				return true;
			}
			return false;
		}
	}

	public static int GetUID()
	{
		if (++m_nUIDCount > 99999999)
		{
			m_nUIDCount = 1;
		}
		return m_nUIDCount;
	}

	public static void LimitEulerAngle(ref float value, float min, float max)
	{
		if (min == 0f && max == 0f)
		{
			return;
		}
		if (min < 0f)
		{
			min += 360f;
			float num = (min + max) / 2f;
			if (value < min && value > num)
			{
				value = min;
			}
			if (value > max && value < num)
			{
				value = max;
			}
		}
		else
		{
			float num2 = (360f + min + max) / 2f;
			if (value < min || value > num2)
			{
				value = min;
			}
			if (value > max && value < num2)
			{
				value = max;
			}
		}
	}

	public static bool LimitAngle(ref float value, float min, float max)
	{
		if (min == 0f && max == 0f)
		{
			if (value > 360f || value < -360f)
			{
				value %= 360f;
			}
			return true;
		}
		if (value < min)
		{
			value = min;
		}
		else if (value > max)
		{
			value = max;
		}
		return false;
	}

	public static float AngleAroundAxis(Vector3 dirA, Vector3 dirB, Vector3 axis)
	{
		dirA -= Vector3.Project(dirA, axis);
		dirB -= Vector3.Project(dirB, axis);
		float num = Vector3.Angle(dirA, dirB);
		return num * (float)((!(Vector3.Dot(axis, Vector3.Cross(dirA, dirB)) < 0f)) ? 1 : (-1));
	}

	public static float Lerp(float src, float dst, float rate)
	{
		if (rate >= 1f)
		{
			return dst;
		}
		if (rate <= 0f)
		{
			return src;
		}
		return src + (dst - src) * rate;
	}

	public static bool Compare(float cpvalue, int operate, float value, float maxvalue = 0f)
	{
		if (maxvalue == 0f)
		{
			switch (operate)
			{
			case 3:
				operate = 1;
				break;
			case 4:
				operate = 2;
				break;
			}
		}
		switch (operate)
		{
		case 1:
			if (value >= cpvalue)
			{
				return true;
			}
			break;
		case 3:
			if (value / maxvalue >= cpvalue / 100f)
			{
				return true;
			}
			break;
		case 2:
			if (value < cpvalue)
			{
				return true;
			}
			break;
		case 4:
			if (value / maxvalue < cpvalue / 100f)
			{
				return true;
			}
			break;
		}
		return false;
	}

	public static int High32(int nValue)
	{
		return nValue >> 16;
	}

	public static int Low32(int nValue)
	{
		return nValue & 0xFFFF;
	}

	public static int Make32(int nHigh, int nLow)
	{
		return ((nHigh & 0xFFFF) << 16) + (nLow & 0xFFFF);
	}

	public static float GetDistance(Vector3 p1, Vector3 p2)
	{
		return (p1 - p2).magnitude;
	}

	public static float GetDistanceSqr(Vector3 p1, Vector3 p2)
	{
		return (p1 - p2).sqrMagnitude;
	}

	public static Vector3 GetDir(Vector3 p1, Vector3 p2)
	{
		return (p2 - p1).normalized;
	}

	public static float K4S5(float value)
	{
		return Mathf.Floor(value + 0.5f);
	}

	public static float ReckonAnimSpeed(float fTime, float fAnimLen)
	{
		return fAnimLen / fTime;
	}

	public static GameObject LoadResources(string sPath)
	{
		Object @object = Resources.Load(sPath);
		if (@object == null)
		{
			return null;
		}
		return (GameObject)Object.Instantiate(@object);
	}

	public static bool GetAttribute(XmlNode node, string name, ref string value)
	{
		if (node == null || node.Attributes[name] == null)
		{
			return false;
		}
		value = node.Attributes[name].Value.Trim();
		if (value.Length < 1)
		{
			return false;
		}
		return true;
	}

	public static string TimeToString(float fTime, bool bIgnoreHour = false)
	{
		if (fTime == 0f)
		{
			if (bIgnoreHour)
			{
				return "--:--";
			}
			return "--:--:--";
		}
		int num = Mathf.FloorToInt(fTime);
		int num2 = num / 60;
		num %= 60;
		int num3 = num2 / 60;
		num2 %= 60;
		string text = string.Empty;
		if (!bIgnoreHour)
		{
			text = ((num3 >= 10) ? (text + num3) : (text + "0" + num3));
			text += ":";
		}
		text = ((num2 >= 10) ? (text + num2) : (text + "0" + num2));
		text += ":";
		if (num < 10)
		{
			return text + "0" + num;
		}
		return text + num;
	}

	public static string PriceToString(int nPrice)
	{
		if (nPrice == 0)
		{
			return nPrice.ToString();
		}
		if (nPrice % 1000000 == 0)
		{
			return nPrice / 1000000 + "M";
		}
		if (nPrice % 1000 == 0)
		{
			return nPrice / 1000 + "K";
		}
		return nPrice.ToString();
	}

	public static Vector3 GetControlPos(Transform transform)
	{
		Vector3 zero = Vector3.zero;
		while (transform.parent != null && transform.name != "TUIControls")
		{
			zero += transform.localPosition;
			transform = transform.parent;
		}
		return zero;
	}

	public static T GetControl<T>(Transform root, string path) where T : MonoBehaviour
	{
		Transform transform = root.Find(path);
		if (transform == null)
		{
			return (T)null;
		}
		return transform.GetComponent<T>();
	}

	public static float CalcShootLightTimeRatio(float min, float max)
	{
		return 1f / (max - min);
	}

	public static bool IsRetina()
	{
		if (Screen.width == 2048 || Screen.width == 960)
		{
			return true;
		}
		return false;
	}

	public static string CombinateStr(string[] arrValue)
	{
		string text = string.Empty;
		if (arrValue == null)
		{
			return text;
		}
		for (int i = 0; i < arrValue.Length; i++)
		{
			text = ((i != 0) ? (text + "," + arrValue[i]) : arrValue[i]);
		}
		return text;
	}

	public static int Formula_Gold2Crystal(int nGold)
	{
		int num = 0;
		num = Mathf.CeilToInt(0.01904f * Mathf.Pow(nGold * 10, 0.8f) - 3f);
		if (num < 1)
		{
			num = 1;
		}
		return num;
	}
}
