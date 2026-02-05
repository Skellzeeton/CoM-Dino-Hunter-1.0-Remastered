using UnityEngine;
using gyTaskSystem;

public class iGameTaskUICollectList : iGameTaskUIBase
{
	protected iGameTaskUIPlane m_GameTaskUIPlane;

	protected iGameTaskUICollect[] m_arrTaskUICollect;

	protected CTaskCollection m_TaskCollect;

	private void Awake()
	{
		base.Height = 0f;
	}

	public override void UpdateTask(float deltaTime)
	{
		if (m_TaskCollect == null)
		{
			return;
		}
		if (m_TaskTime != null)
		{
			m_TaskTime.SetTime(m_TaskCollect.TaskTime);
		}
		if (!m_TaskCollect.isUpdateData)
		{
			return;
		}
		for (int i = 0; i < m_TaskCollect.m_ltCollections.Count; i++)
		{
			if (!(m_arrTaskUICollect[i] == null))
			{
				CTaskCollection.CCollect cCollect = m_TaskCollect.m_ltCollections[i];
				if (cCollect != null)
				{
					m_arrTaskUICollect[i].SetCurNum(cCollect.nCurCount);
				}
			}
		}
		m_TaskCollect.isUpdateData = false;
	}

	public override void InitTask(CTaskBase taskbase)
	{
		m_TaskCollect = taskbase as CTaskCollection;
		if (m_TaskCollect != null && m_TaskCollect.m_ltCollections != null)
		{
			m_arrTaskUICollect = new iGameTaskUICollect[m_TaskCollect.m_ltCollections.Count];
			for (int i = 0; i < m_TaskCollect.m_ltCollections.Count; i++)
			{
				CTaskCollection.CCollect cCollect = m_TaskCollect.m_ltCollections[i];
				if (cCollect == null)
				{
					continue;
				}
				GameObject gameObject = m_GameUI.AddControl(2003, base.transform);
				if (!(gameObject == null))
				{
					gameObject.transform.localPosition = new Vector3(0f, 0f - base.Height, 0f);
					gameObject.transform.localRotation = Quaternion.identity;
					iGameTaskUICollect component = gameObject.GetComponent<iGameTaskUICollect>();
					if (!(component == null))
					{
						component.SetCurNum(cCollect.nCurCount);
						component.SetMaxNum(cCollect.nMaxCount);
						Add(i, component);
						base.Height += component.Height;
					}
				}
			}
		}
		AddTimeLimitUI(m_TaskCollect.TaskTime);
	}

	public override void Destroy()
	{
		for (int i = 0; i < m_arrTaskUICollect.Length; i++)
		{
			if (!(m_arrTaskUICollect[i] == null))
			{
				Object.Destroy(m_arrTaskUICollect[i]);
				m_arrTaskUICollect[i] = null;
			}
		}
		m_arrTaskUICollect = null;
	}

	protected void Add(int nIndex, iGameTaskUICollect collect)
	{
		if (nIndex >= 0 && nIndex < m_arrTaskUICollect.Length)
		{
			m_arrTaskUICollect[nIndex] = collect;
		}
	}
}
