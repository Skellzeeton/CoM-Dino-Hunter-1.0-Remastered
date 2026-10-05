using System.Collections.Generic;
using UnityEngine;

public class GoodsNeedItemBuy : MonoBehaviour
{
	public TUILabel label_price_normal;
	public TUILabel label_price_press;
	public TUIMeshSprite img_price_unit_normal;
	public TUIMeshSprite img_price_unit_press;

	public GameObject img_lock;

	private int index;
	private Vector3 m_position = Vector3.zero;

	private int goods_id;
	private GoodsQualityType goods_quality;
	private int goods_lack_count;
	private int goods_price;
	private UnitType goods_unit_type;

	private string gold_texture = "title_jingbi";
	private string crystal_texture = "title_shuijing";

	private bool m_can_buy = true;

	private void Start()
	{
		m_position = base.gameObject.transform.localPosition;

		if (img_lock != null)
		{
			img_lock.SetActive(false);
		}
	}


	public void SetInfo(int m_price, int m_id, GoodsQualityType m_good_quality, int m_lack_count, UnitType m_gold_type)
	{
		int num = m_price * m_lack_count;
		goods_price = m_price;
		goods_id = m_id;
		goods_quality = m_good_quality;
		goods_lack_count = m_lack_count;
		goods_unit_type = m_gold_type;
		base.gameObject.transform.localPosition = m_position;
		m_can_buy = IsGoodsUnlocked(m_id);
		if (!m_can_buy)
		{
			label_price_normal.Text = string.Empty;
			label_price_press.Text = string.Empty;
			img_price_unit_normal.texture = string.Empty;
			img_price_unit_press.texture = string.Empty;
			if (img_lock != null)
			{
				img_lock.SetActive(true);
			}
			if (base.GetComponent<Animation>() != null)
			{
				base.GetComponent<Animation>().wrapMode = WrapMode.Loop;
				base.GetComponent<Animation>().Play();
			}
			return;
		}
		if (img_lock != null)
		{
			img_lock.SetActive(false);
		}
		label_price_normal.Text = num.ToString();
		label_price_press.Text = num.ToString();
		switch (m_gold_type)
		{
			case UnitType.Gold:
				img_price_unit_normal.texture = gold_texture;
				img_price_unit_press.texture = gold_texture;
				break;
			case UnitType.Crystal:
				img_price_unit_normal.texture = crystal_texture;
				img_price_unit_press.texture = crystal_texture;
				break;
		}
		if (base.GetComponent<Animation>() != null)
		{
			base.GetComponent<Animation>().wrapMode = WrapMode.Loop;
			base.GetComponent<Animation>().Play();
		}
	}

	public void HideInfo()
	{
		label_price_normal.Text = string.Empty;
		label_price_press.Text = string.Empty;
		img_price_unit_normal.texture = string.Empty;
		img_price_unit_press.texture = string.Empty;
		goods_id = 0;
		goods_lack_count = 0;
		goods_price = 0;
		goods_unit_type = UnitType.Gold;
		m_can_buy = true;
		if (img_lock != null)
		{
			img_lock.SetActive(false);
		}
		base.gameObject.transform.localPosition = m_position + new Vector3(0f, -1000f, 0f);
		if (base.GetComponent<Animation>() != null)
		{
			base.GetComponent<Animation>().wrapMode = WrapMode.Loop;
			base.GetComponent<Animation>().Stop();
		}
	}

	public bool CanBuy()
	{
		return m_can_buy;
	}

	private bool IsGoodsUnlocked(int materialID)
	{
		if (materialID <= 0)
		{
			return true;
		}
		iGameData gameData = iGameApp.GetInstance().m_GameData;
		if (gameData == null)
		{
			return true;
		}
		iDataCenter dataCenter = gameData.GetDataCenter();
		if (dataCenter == null)
		{
			return true;
		}
		iGameLevelCenter levelCenter = gameData.GetGameLevelCenter();
		if (levelCenter == null)
		{
			return true;
		}
		Dictionary<int, GameLevelInfo> levels = levelCenter.GetData();
		if (levels == null)
		{
			return true;
		}
		int firstDropLevelID = int.MaxValue;
		foreach (GameLevelInfo level in levels.Values)
		{
			if (level == null || level.ltRewardMaterial == null)
			{
				continue;
			}
			foreach (CRewardMaterial reward in level.ltRewardMaterial)
			{
				if (reward == null || reward.nID != materialID)
				{
					continue;
				}

				if (level.nID < firstDropLevelID)
				{
					firstDropLevelID = level.nID;
				}

				break;
			}
		}
		if (firstDropLevelID == int.MaxValue)
		{
			return true;
		}
		return dataCenter.IsLevelPassed(firstDropLevelID);
	}

	public int GetGoodsID()
	{
		return goods_id;
	}

	public GoodsQualityType GetGoodsQuality()
	{
		return goods_quality;
	}

	public int GetGoodsLackCount()
	{
		return goods_lack_count;
	}

	public int GetGoodsPrice()
	{
		return goods_price;
	}

	public UnitType GetGoodsUnitType()
	{
		return goods_unit_type;
	}

	public int GetIndex()
	{
		return index;
	}

	public void SetIndex(int m_index)
	{
		index = m_index;
	}
}