using UnityEngine;

public class WeaponKindItemBtn : MonoBehaviour
{
	public TUIMeshSprite img_new_mark_normal;

	public TUIMeshSprite img_new_mark_press;

	private string texture_mark = "new";

	private string texture_new = "new2";

	private NewMarkType new_mark_type;

	private TUIButtonSelect btn_select;

	private void Awake()
	{
		btn_select = base.gameObject.GetComponent<TUIButtonSelect>();
		if (btn_select == null)
		{
			Debug.Log("no btn_select!");
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (img_new_mark_normal.gameObject.active && img_new_mark_press.gameObject.active)
		{
			Debug.Log("!!!");
		}
	}

	public void ShowNewMark(NewMarkType m_type)
	{
		switch (m_type)
		{
		case NewMarkType.Mark:
			if (img_new_mark_normal != null)
			{
				img_new_mark_normal.texture = texture_mark;
			}
			if (img_new_mark_press != null)
			{
				img_new_mark_press.texture = texture_mark;
			}
			break;
		case NewMarkType.New:
			if (img_new_mark_normal != null)
			{
				img_new_mark_normal.texture = texture_new;
			}
			if (img_new_mark_press != null)
			{
				img_new_mark_press.texture = texture_new;
			}
			break;
		default:
			if (img_new_mark_normal != null)
			{
				img_new_mark_normal.texture = string.Empty;
			}
			if (img_new_mark_press != null)
			{
				img_new_mark_press.texture = string.Empty;
			}
			break;
		}
		new_mark_type = m_type;
	}

	public NewMarkType GetNewMark()
	{
		return new_mark_type;
	}

	public void HideNewMark()
	{
		if (img_new_mark_normal != null)
		{
			img_new_mark_normal.texture = string.Empty;
			new_mark_type = NewMarkType.None;
		}
	}
}
