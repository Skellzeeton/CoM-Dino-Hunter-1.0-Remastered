using UnityEngine;

public class Top_Bar : MonoBehaviour
{
	public GameObject label_level;

	public GameObject label_exp;

	public GameObject label_gold;

	public GameObject label_crystal;

	public GameObject img_exp;

	public TUIMeshSprite img_role;

	public TUIButtonClick btn_back;

	public TUIButtonClick btn_gold;

	public TUIButtonClick btn_crystal;

	private Vector3 img_exp_normal_position = Vector3.zero;

	private int level;

	private int exp;

	private int level_exp;

	private int gold;

	private int crystal;

	private void Awake()
	{
		img_exp_normal_position = img_exp.transform.localPosition;
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void SetAllValue(int m_level, int m_exp, int m_level_exp, int m_gold, int m_crystal, int m_role_id)
	{
		level = m_level;
		exp = m_exp;
		level_exp = m_level_exp;
		gold = m_gold;
		crystal = m_crystal;
		label_level.GetComponent<TUILabel>().Text = "LV. " + level;
		label_exp.GetComponent<TUILabel>().Text = exp.ToString();
		label_gold.GetComponent<TUILabel>().Text = gold.ToString();
		label_crystal.GetComponent<TUILabel>().Text = crystal.ToString();
		if (level_exp == 0)
		{
			img_exp.transform.localPosition = new Vector3(img_exp_normal_position.x + 90f, img_exp_normal_position.y, img_exp_normal_position.z);
		}
		else
		{
			img_exp.transform.localPosition = new Vector3(img_exp_normal_position.x + (float)(90 * exp / level_exp), img_exp_normal_position.y, img_exp_normal_position.z);
		}
		if (img_role != null)
		{
			string roleTexture = TUIMappingInfo.Instance().GetRoleTexture(m_role_id);
			img_role.texture = roleTexture;
		}
	}

	public void SetLevelValue(int m_level)
	{
		level = m_level;
		label_level.GetComponent<TUILabel>().Text = m_level.ToString();
	}

	public void SetExpValueBar(int m_exp, int m_level_exp)
	{
		exp = m_exp;
		level_exp = m_level_exp;
		label_exp.GetComponent<TUILabel>().Text = m_exp.ToString();
		if (m_level_exp == 0)
		{
			img_exp.transform.localPosition = new Vector3(img_exp_normal_position.x + 90f, img_exp_normal_position.y, img_exp_normal_position.z);
		}
		else
		{
			img_exp.transform.localPosition = new Vector3(img_exp_normal_position.x + (float)(90 * m_exp / m_level_exp), img_exp_normal_position.y, img_exp_normal_position.z);
		}
	}

	public void SetGoldValue(int m_gold)
	{
		gold = m_gold;
		label_gold.GetComponent<TUILabel>().Text = m_gold.ToString();
	}

	public void SetCrystalValue(int m_crystal)
	{
		crystal = m_crystal;
		label_crystal.GetComponent<TUILabel>().Text = m_crystal.ToString();
	}

	public void SetRole(int m_role_id)
	{
		if (img_role != null)
		{
			string roleTexture = TUIMappingInfo.Instance().GetRoleTexture(m_role_id);
			img_role.texture = roleTexture;
		}
	}

	public int GetLevelValue()
	{
		return level;
	}

	public int GetGoldValue()
	{
		return gold;
	}

	public int GetCrystalValue()
	{
		return crystal;
	}

	public void SetBtnBackShow(bool m_show)
	{
		if (btn_back == null)
		{
			Debug.Log("error!");
		}
		else if (m_show)
		{
			btn_back.gameObject.SetActiveRecursively(true);
			btn_back.Reset();
		}
		else
		{
			btn_back.gameObject.SetActiveRecursively(false);
		}
	}

	public void SetBtnGoldShow(bool m_show)
	{
		if (btn_gold == null)
		{
			Debug.Log("error!");
		}
		else if (m_show)
		{
			btn_gold.gameObject.SetActiveRecursively(true);
			btn_gold.Reset();
		}
		else
		{
			btn_gold.gameObject.SetActiveRecursively(false);
		}
	}

	public void SetBtnCrystalShow(bool m_show)
	{
		if (btn_crystal == null)
		{
			Debug.Log("error!");
		}
		else if (m_show)
		{
			btn_crystal.gameObject.SetActiveRecursively(true);
			btn_crystal.Reset();
		}
		else
		{
			btn_crystal.gameObject.SetActiveRecursively(false);
		}
	}
}
