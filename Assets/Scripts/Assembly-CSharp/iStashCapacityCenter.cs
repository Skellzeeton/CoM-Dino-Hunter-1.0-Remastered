using GUPS.AntiCheat.Protected;
using System.Collections.Generic;

public class iStashCapacityCenter
{
	protected Dictionary<ProtectedInt32, CStashCapacity> m_dictStashCapacity;

	public iStashCapacityCenter()
	{
		m_dictStashCapacity = new Dictionary<ProtectedInt32, CStashCapacity>();
	}

	public Dictionary<ProtectedInt32, CStashCapacity> GetData()
	{
		return m_dictStashCapacity;
	}

	public CStashCapacity Get(ProtectedInt32 nLevel)
	{
		if (!m_dictStashCapacity.ContainsKey(nLevel))
		{
			return null;
		}
		return m_dictStashCapacity[nLevel];
	}

	public ProtectedInt32 GetCapacity(ProtectedInt32 nLevel)
	{
		CStashCapacity cStashCapacity = Get(nLevel);
		if (cStashCapacity == null)
		{
			return 0;
		}
		return cStashCapacity.nCapacity;
	}

	public bool Load()
	{
		for (int i = 1; i <= 500; i++)
		{
			int capacity = 100 + (i - 1) * 5;
			int num = 100 + i * 5;
			m_dictStashCapacity.Add(i, new CStashCapacity(i, true, 5, capacity, "Add capacity to " + num));
		}
		return true;
	}
}
