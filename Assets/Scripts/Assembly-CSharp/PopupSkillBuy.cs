using UnityEngine;

public class PopupSkillBuy : MonoBehaviour
{
	public TUILabel label_title;

	public TUILabel label_introduce;

	public PopupSkillUpdateBuy btn_buy;

	public GameObject go_popup;

	private string gold_texture = "title_jingbi";

	private string crystal_texture = "title_shuijing";

	private void Start()
	{
	}

	private void Update()
	{
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
		TUIPriceInfo skillBuyPrice = m_item.GetSkillBuyPrice();
		if (skillBuyPrice == null)
		{
			Debug.Log("error!");
			return;
		}
		int price = skillBuyPrice.price;
		UnitType unit_type = skillBuyPrice.unit_type;
		string skillIntroduceEx = m_item.GetSkillIntroduceEx();
		string skillName = m_item.GetSkillName();
		if (label_title != null)
		{
			label_title.Text = skillName;
		}
		if (label_introduce != null)
		{
			label_introduce.Text = skillIntroduceEx;
		}
		if (btn_buy != null)
		{
			btn_buy.SetBtnText(price, unit_type);
		}
	}

	public void Show()
	{
		base.gameObject.transform.localPosition = new Vector3(0f, 0f, base.gameObject.transform.localPosition.z);
		if (go_popup != null && go_popup.GetComponent<Animation>() != null)
		{
			go_popup.GetComponent<Animation>().Play();
		}
	}

	public void Hide()
	{
		base.gameObject.transform.localPosition = new Vector3(0f, -1000f, base.gameObject.transform.localPosition.z);
	}
}
