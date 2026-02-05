using GUPS.AntiCheat.Protected;
using UnityEngine;

public class Sell_Stash : MonoBehaviour
{
	public TUILabel label_count;

	public TUILabel label_price;

	public TUIMeshSprite img_price;

	private string gold_texture = "title_jingbi";

	private string crystal_texture = "title_shuijing";

	private ProtectedInt32 count;

	private TUIPriceInfo price_info;

	private ProtectedInt32 count_now;

	private bool enable;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void SetParam(ProtectedInt32 m_count, TUIPriceInfo m_price_info)
	{
		if (m_price_info == null)
		{
			Debug.Log("error!");
			return;
		}
		count = m_count;
		price_info = m_price_info;
		count_now = m_count;
		SetCountText(count);
		SetPriceText(count * price_info.price);
		if (price_info.unit_type == UnitType.Gold)
		{
			img_price.texture = gold_texture;
		}
		else if (price_info.unit_type == UnitType.Crystal)
		{
			img_price.texture = crystal_texture;
		}
	}

	public void SetParamNull()
	{
		label_count.Text = string.Empty;
		label_price.Text = string.Empty;
	}

	public void SetCountPlus(ProtectedInt32 m_count)
	{
        ProtectedInt32 num = m_count + count_now;
		if (num <= count)
		{
			count_now = num;
		}
		else
		{
			count_now = count;
			CUISound.GetInstance().Stop("UI_Count");
		}
		SetCountText(count_now);
		SetPriceText(count_now * price_info.price);
	}

	public void SetCountSubstract(int m_count)
	{
        ProtectedInt32 num = count_now - m_count;
		if (num >= 1)
		{
			count_now = num;
		}
		else
		{
			count_now = 1;
			CUISound.GetInstance().Stop("UI_Count");
		}
		SetCountText(count_now);
		SetPriceText(count_now * price_info.price);
	}

	public void SetCountText(ProtectedInt32 m_count)
	{
		label_count.Text = m_count.ToString();
	}

	public void SetPriceText(ProtectedInt32 m_price)
	{
		label_price.Text = m_price.ToString();
	}

	public ProtectedInt32 GetCountNow()
	{
		return count_now;
	}

	public ProtectedInt32 GetTotalPrice()
	{
		return count_now * price_info.price;
	}

	public UnitType GetUnitType()
	{
		return price_info.unit_type;
	}

	public void Enable(bool m_bool)
	{
		enable = m_bool;
	}

	public bool Enalbe()
	{
		return enable;
	}
}
