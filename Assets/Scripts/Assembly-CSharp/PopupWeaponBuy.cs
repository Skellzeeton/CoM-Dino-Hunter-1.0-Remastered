using UnityEngine;

public class PopupWeaponBuy : MonoBehaviour
{
	public enum PopupWeaponBuyState
	{
		State_None = 0,
		State_Craft = 1,
		State_Update = 2,
		State_Disable = 3
	}

	public TUILabel label_normal;

	public TUILabel label_press;

	public TUIButtonClick btn_click;

	private PopupWeaponBuyState btn_state;

	private void Start()
	{
		SetStateCraft();
	}

	private void Update()
	{
	}

	public PopupWeaponBuyState GetState()
	{
		return btn_state;
	}

	public void SetStateCraft()
	{
		if (btn_state != PopupWeaponBuyState.State_Craft)
		{
			label_normal.Text = "CRAFT";
			label_press.Text = "CRAFT";
			if (btn_click != null)
			{
				btn_click.gameObject.SetActiveRecursively(true);
				btn_click.Show();
			}
			btn_state = PopupWeaponBuyState.State_Craft;
		}
	}

	public void SetStateUpdate()
	{
		if (btn_state != PopupWeaponBuyState.State_Update)
		{
			label_normal.Text = "UPGRADE";
			label_press.Text = "UPGRADE";
			if (btn_click != null)
			{
				btn_click.gameObject.SetActiveRecursively(true);
				btn_click.Show();
			}
			btn_state = PopupWeaponBuyState.State_Update;
		}
	}

	public void SetStateDisable()
	{
		if (btn_state != PopupWeaponBuyState.State_Disable)
		{
			if (btn_click != null)
			{
				btn_click.gameObject.SetActiveRecursively(false);
			}
			btn_state = PopupWeaponBuyState.State_Disable;
		}
	}
}
