using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class CStartPointManager
{
	protected int m_nID;

	protected Color m_Color;

	protected Dictionary<int, CStartPoint> m_dictStartPoint;

	public int ID
	{
		get
		{
			return m_nID;
		}
		set
		{
			m_nID = value;
		}
	}

	public Color GizmosColor
	{
		get
		{
			return m_Color;
		}
		set
		{
			m_Color = value;
		}
	}

	public CStartPointManager()
	{
		m_dictStartPoint = new Dictionary<int, CStartPoint>();
	}

	public CStartPoint GetRandom()
	{
		int num = Random.Range(0, m_dictStartPoint.Count);
		foreach (CStartPoint value in m_dictStartPoint.Values)
		{
			num--;
			if (num < 0)
			{
				return value;
			}
		}
		return null;
	}

	public bool IsInside2D(Vector3 v3Pos)
	{
		foreach (CStartPoint value in m_dictStartPoint.Values)
		{
			if (value.IsInside2D(v3Pos))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsInside3D(Vector3 v3Pos)
	{
		foreach (CStartPoint value in m_dictStartPoint.Values)
		{
			if (value.IsInside3D(v3Pos))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsInside2D(int nID, Vector3 v3Pos)
	{
		if (!m_dictStartPoint.ContainsKey(nID))
		{
			return false;
		}
		return m_dictStartPoint[nID].IsInside2D(v3Pos);
	}

	public bool IsInside3D(int nID, Vector3 v3Pos)
	{
		if (!m_dictStartPoint.ContainsKey(nID))
		{
			return false;
		}
		return m_dictStartPoint[nID].IsInside3D(v3Pos);
	}

	public CStartPoint Get(int nID)
	{
		if (!m_dictStartPoint.ContainsKey(nID))
		{
			return null;
		}
		return m_dictStartPoint[nID];
	}

	public void Set(int nID, CStartPoint point)
	{
		if (!m_dictStartPoint.ContainsKey(nID))
		{
			m_dictStartPoint.Add(nID, point);
		}
		else
		{
			m_dictStartPoint[nID] = point;
		}
	}

	public void Del(int nID)
	{
		if (m_dictStartPoint.ContainsKey(nID))
		{
			m_dictStartPoint.Remove(nID);
		}
	}

	public Dictionary<int, CStartPoint> GetData()
	{
		return m_dictStartPoint;
	}

	public bool Load(string sPath)
	{
		string empty = string.Empty;
		TextAsset textAsset = (TextAsset)Resources.Load(sPath, typeof(TextAsset));
		if (textAsset == null)
		{
			return false;
		}
		empty = textAsset.ToString();
		if (empty.Length < 1)
		{
			return false;
		}
		ParseXml(empty);
		return true;
	}

	public void ParseXml(string context)
	{
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(context);
		string empty = string.Empty;
		m_dictStartPoint.Clear();
		XmlNode documentElement = xmlDocument.DocumentElement;
		empty = documentElement.Attributes["id"].Value;
		if (empty.Length < 1)
		{
			return;
		}
		m_nID = int.Parse(empty);
		empty = documentElement.Attributes["color"].Value;
		if (empty.Length < 1)
		{
			return;
		}
		string[] array = empty.Split(',');
		if (array.Length < 4)
		{
			return;
		}
        m_Color.r = ServerX.ParseFloat(array[0]);
        m_Color.g = ServerX.ParseFloat(array[1]);
        m_Color.b = ServerX.ParseFloat(array[2]);
        m_Color.a = ServerX.ParseFloat(array[3]);
        foreach (XmlNode childNode in documentElement.ChildNodes)
		{
			if (childNode.Name != "Point")
			{
				continue;
			}
			empty = childNode.Attributes["id"].Value;
			if (empty.Length < 1)
			{
				continue;
			}
			int nID = int.Parse(empty);
			CStartPoint cStartPoint = new CStartPoint();
            empty = childNode.Attributes["pos"].Value;
            if (empty.Length > 0)
            {
                array = empty.Split(',');
                if (array.Length >= 2)
                {
                    cStartPoint.v3Pos = new Vector3(ServerX.ParseFloat(array[0]), ServerX.ParseFloat(array[1]), ServerX.ParseFloat(array[2]));
                }
            }
            empty = childNode.Attributes["size"].Value;
            if (empty.Length > 0)
            {
                array = empty.Split(',');
                if (array.Length >= 2)
                {
                    cStartPoint.v3Size = new Vector3(ServerX.ParseFloat(array[0]), ServerX.ParseFloat(array[1]), ServerX.ParseFloat(array[2]));
                }
            }
            Set(nID, cStartPoint);
		}
	}
}
