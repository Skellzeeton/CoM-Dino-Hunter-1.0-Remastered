using UnityEngine;

public class PopupSkillUpdate : MonoBehaviour
{
	public TUILabel label_title;

	public TUILabel label_introduce;

	public LevelStars level_stars;

	public PopupSkillUpdateBuy btn_buy;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void ShowSkillUpdate()
	{
		base.gameObject.transform.localPosition = new Vector3(0f, 0f, base.gameObject.transform.localPosition.z);
		base.gameObject.GetComponent<Animation>().Play();
	}

	public void HideSkillUpdate()
	{
		base.gameObject.transform.localPosition = new Vector3(0f, 1000f, base.gameObject.transform.localPosition.z);
	}

	public void SetInfo(ScrollList_SkillItem m_item)
	{
		if (m_item == null)
		{
			Debug.Log("error!");
			return;
		}
		int skillLevel = m_item.GetSkillLevel();
		if (skillLevel >= 5)
		{
			Debug.Log("!!!you reach max level!!!");
			return;
		}
		label_introduce.Text = m_item.GetSkillIntroduce();
		label_title.Text = m_item.GetSkillName();
		float x = label_title.CalculateBounds(label_title.Text).size.x;
		x *= label_title.transform.localScale.x;
		Vector3 position = new Vector3(label_title.transform.localPosition.x + x + 8f, label_title.transform.localPosition.y, label_title.transform.localPosition.z);
		level_stars.SetStars(m_item.GetSkillLevel(), position);
		if (m_item.GetSkillUpdatePrice() == null)
		{
			Debug.Log("error!");
			return;
		}
		Debug.Log("price:" + m_item.GetSkillUpdatePrice().price + m_item.GetSkillUpdatePrice().unit_type.ToString());
		btn_buy.SetBtnText(m_item.GetSkillUpdatePrice().price, m_item.GetSkillUpdatePrice().unit_type);
	}
}
