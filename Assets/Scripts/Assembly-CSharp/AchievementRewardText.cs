using UnityEngine;

public class AchievementRewardText : MonoBehaviour
{
	public TUILabel label_value01;

	public TUIMeshSprite img_unit01;

	public TUILabel label_value02;

	public TUIMeshSprite img_unit02;

	private string gold_texture = "title_jingbi";

	private string crystal_texture = "title_shuijing";

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void Show(int m_value01, UnitType m_type01)
	{
		base.gameObject.SetActiveRecursively(true);
		if (label_value01 != null)
		{
			label_value01.gameObject.SetActiveRecursively(true);
			label_value01.Text = m_value01 + "x";
		}
		if (img_unit01 != null)
		{
			img_unit01.gameObject.SetActiveRecursively(true);
			switch (m_type01)
			{
			case UnitType.Gold:
				img_unit01.texture = gold_texture;
				break;
			case UnitType.Crystal:
				img_unit01.texture = crystal_texture;
				break;
			}
		}
		if (label_value02 != null)
		{
			label_value02.gameObject.SetActiveRecursively(false);
		}
		if (img_unit02 != null)
		{
			img_unit02.gameObject.SetActiveRecursively(false);
		}
	}

	public void Show(int m_value01, UnitType m_type01, int m_value02, UnitType m_type02)
	{
		base.gameObject.SetActiveRecursively(true);
		if (label_value01 != null)
		{
			label_value01.gameObject.SetActiveRecursively(true);
			label_value01.Text = m_value01 + "x";
		}
		if (img_unit01 != null)
		{
			img_unit01.gameObject.SetActiveRecursively(true);
			switch (m_type01)
			{
			case UnitType.Gold:
				img_unit01.texture = gold_texture;
				break;
			case UnitType.Crystal:
				img_unit01.texture = crystal_texture;
				break;
			}
		}
		if (label_value02 != null)
		{
			label_value02.gameObject.SetActiveRecursively(true);
			label_value02.Text = m_value02 + "x";
		}
		if (img_unit02 != null)
		{
			img_unit02.gameObject.SetActiveRecursively(true);
			switch (m_type02)
			{
			case UnitType.Gold:
				img_unit02.texture = gold_texture;
				break;
			case UnitType.Crystal:
				img_unit02.texture = crystal_texture;
				break;
			}
		}
	}

	public void Hide()
	{
		base.gameObject.SetActiveRecursively(false);
	}
}
