using UnityEngine;

public class iBuilding : MonoBehaviour
{
	public enum kState
	{
		None = 0,
		Normal1 = 1,
		Normal2 = 2,
		Normal3 = 3
	}

	public GameObject mModelNormal1;
	public GameObject mModelNormal2;
	public GameObject mModelNormal3;

	public float fNormalRate1 = 100f;
	public float fNormalRate2 = 50f;
	public float fNormalRate3;

	public Transform[] arrAttackPoint;

	public kState m_State;

	protected GameObject m_EntityModel;

	public float m_fLife;
	public float m_fLifeMax;

	protected Renderer[] m_Renderer;
	protected Vector3 m_v3ScrColor;
	protected Vector3 m_v3DstColor;
	protected float m_fColorRate;

	protected TAudioController m_AudioController;

	public bool IsBroken
	{
		get { return m_State == kState.Normal3; }
	}

	private void Awake()
	{
		m_State = kState.None;
		Transform transform = base.transform.Find("Entity");
		if (transform != null)
			m_EntityModel = transform.gameObject;
		m_fColorRate = 1f;
		m_AudioController = GetComponent<TAudioController>();
		if (m_AudioController == null)
			m_AudioController = base.gameObject.AddComponent<TAudioController>();
	}

	private void Start()  { }

	private void Update()
	{
		if (m_Renderer == null || !(m_fColorRate < 1f))
			return;
		m_fColorRate += Time.deltaTime;
		Vector3 vector = Vector3.Lerp(m_v3ScrColor, m_v3DstColor, m_fColorRate);
		foreach (Renderer renderer in m_Renderer)
		{
			if (renderer.material != null)
			{
				Color color = renderer.material.color;
				color.r = vector.x;
				color.g = vector.y;
				color.b = vector.z;
				renderer.material.color = color;
			}
		}
	}

	public void Initialize(float fLife, float fLifeMax)
	{
		m_fLife    = fLife;
		m_fLifeMax = fLifeMax;
		float num = m_fLife / m_fLifeMax * 100f;
		if (num <= fNormalRate3)
		{
			m_State = kState.Normal3;
			mModelNormal1.SetActiveRecursively(false);
			mModelNormal2.SetActiveRecursively(false);
			mModelNormal3.SetActiveRecursively(true);
			m_Renderer = mModelNormal3.GetComponentsInChildren<Renderer>();
		}
		else if (num <= fNormalRate2)
		{
			m_State = kState.Normal2;
			mModelNormal1.SetActiveRecursively(false);
			mModelNormal2.SetActiveRecursively(true);
			mModelNormal3.SetActiveRecursively(false);
			m_Renderer = mModelNormal2.GetComponentsInChildren<Renderer>();
		}
		else
		{
			m_State = kState.Normal1;
			mModelNormal1.SetActiveRecursively(true);
			mModelNormal2.SetActiveRecursively(false);
			mModelNormal3.SetActiveRecursively(false);
			m_Renderer = mModelNormal1.GetComponentsInChildren<Renderer>();
		}
	}

	public void AddHP(float fDmg)
	{
		kState prevState = m_State;
		m_fLife += fDmg;
		if (m_fLife > m_fLifeMax)
			m_fLife = m_fLifeMax;
		else if (m_fLife <= 0f)
			m_fLife = 0f;
		float num = m_fLife / m_fLifeMax * 100f;
		if (num <= fNormalRate3)
		{
			m_State = kState.Normal3;
			mModelNormal1.SetActiveRecursively(false);
			mModelNormal2.SetActiveRecursively(false);
			mModelNormal3.SetActiveRecursively(true);
			m_Renderer = mModelNormal3.GetComponentsInChildren<Renderer>();
		}
		else if (num <= fNormalRate2)
		{
			m_State = kState.Normal2;
			mModelNormal1.SetActiveRecursively(false);
			mModelNormal2.SetActiveRecursively(true);
			mModelNormal3.SetActiveRecursively(false);
			m_Renderer = mModelNormal2.GetComponentsInChildren<Renderer>();
		}
		else
		{
			m_State = kState.Normal1;
			mModelNormal1.SetActiveRecursively(true);
			mModelNormal2.SetActiveRecursively(false);
			mModelNormal3.SetActiveRecursively(false);
			m_Renderer = mModelNormal1.GetComponentsInChildren<Renderer>();
		}
		if (fDmg < 0f)
		{
			if (m_fLife <= 0f)
			{
				PlayAudio("Mat_Fence_destroy");
			}
			else if (m_State != prevState)
			{
				PlayAudio("Mat_Fence_crash");
			}
			m_v3ScrColor = new Vector3(1f, 0f, 0f);
			m_v3DstColor = new Vector3(1f, 1f, 1f);
			m_fColorRate = 0f;
			if (m_Renderer != null)
			{
				foreach (Renderer renderer in m_Renderer)
				{
					if (renderer.material != null)
						renderer.material.color = new Color(1f, 0f, 0f);
				}
			}
		}
	}

	public Vector3 GetRandomPoint()
	{
		if (arrAttackPoint == null)
			return base.transform.position;
		return arrAttackPoint[Random.Range(0, arrAttackPoint.Length)].position;
	}

	public void PlayAudio(string sName)
	{
		if (m_AudioController != null)
			m_AudioController.PlayAudio(sName);
	}

	public void StopAudio(string sName)
	{
		if (m_AudioController != null)
			m_AudioController.StopAudio(sName);
	}
}