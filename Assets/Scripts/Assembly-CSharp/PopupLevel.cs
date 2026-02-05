using UnityEngine;

public class PopupLevel : MonoBehaviour
{
	public GameObject go_popup;

	public PopupLevel_Frame01 popuplevel_frame01;

	public PopupLevel_Frame02 popuplevel_frame02;

	public PopupLevel_Frame03 popuplevel_frame03;

	public TUILabel label_title;

	public TUIMeshSprite img_title_bg;

	public TUIButtonClick btn_start;

	public PopupTips popup_tips;

	private TUILevelInfo level_info;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void SetBtnStartEnable(bool m_enable)
	{
		if (m_enable)
		{
			btn_start.Disable(false);
		}
		else
		{
			btn_start.Disable(true);
		}
	}

	public void Show(TUILevelInfo m_level_info)
	{
		if (popuplevel_frame01 == null || popuplevel_frame02 == null || popuplevel_frame03 == null)
		{
			Debug.Log("error!");
			return;
		}
		level_info = m_level_info;
		if (level_info == null)
		{
			Debug.Log("error! no info");
		}
		else
		{
			popuplevel_frame01.SetInfo(level_info.introduce01);
			popuplevel_frame02.SetInfo(level_info.introduce02);
			popuplevel_frame03.SetGoodsInfo(level_info.goods_drop_list);
			popuplevel_frame03.SetRecommend(level_info.recommend_role_info, level_info.recommend_weapon_info);
			label_title.Text = m_level_info.title;
		}
		base.transform.localPosition = new Vector3(0f, 0f, base.transform.localPosition.z);
		go_popup.GetComponent<Animation>().Play();
		if (img_title_bg != null)
		{
			string mapTexture = TUIMappingInfo.Instance().GetMapTexture(level_info.id);
			img_title_bg.texture = mapTexture;
		}
		if (!popuplevel_frame03.GetOpenStart())
		{
			btn_start.Disable(true);
		}
		else
		{
			btn_start.Disable(false);
		}
	}

	public void Hide()
	{
		base.transform.localPosition = new Vector3(0f, -1000f, base.transform.localPosition.z);
	}

	public void ShowTips(TUIControl m_control)
	{
		if (popup_tips == null || m_control == null)
		{
			Debug.Log("error!");
			return;
		}
		GoodsNeedItemImg component = m_control.GetComponent<GoodsNeedItemImg>();
		if (component != null)
		{
			string goodsName = component.GetGoodsName();
			popup_tips.SetInfo(goodsName, m_control.transform.position, PopupTips.TipsPivot.TopRight);
		}
	}

	public void HideTips()
	{
		if (popup_tips == null)
		{
			Debug.Log("error!");
		}
		else
		{
			popup_tips.Hide();
		}
	}
}
