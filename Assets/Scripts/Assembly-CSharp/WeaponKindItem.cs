using UnityEngine;

public class WeaponKindItem : MonoBehaviour
{
	public TUIButtonSelect btn_select01;

	public TUIButtonSelect btn_select02;

	public TUIButtonSelect btn_select03;

	public TUIButtonSelect btn_select04;

	public TUIButtonSelect btn_select05;

	public TUIButtonSelect btn_select06;

	public TUIButtonSelect btn_select07;

	private TUIButtonSelect[] btn_select_list;

	private TUIButtonSelect btn_select_now;

	private void Awake()
	{
		if (btn_select01 == null || btn_select02 == null || btn_select03 == null || btn_select04 == null || btn_select05 == null || btn_select06 == null || btn_select07 == null)
		{
			Debug.Log("error! no btn_selct");
		}
		btn_select_list = new TUIButtonSelect[7];
		btn_select_list[0] = btn_select01;
		btn_select_list[1] = btn_select02;
		btn_select_list[2] = btn_select03;
		btn_select_list[3] = btn_select04;
		btn_select_list[4] = btn_select05;
		btn_select_list[5] = btn_select06;
		btn_select_list[6] = btn_select07;
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void SetSelectBtn(WeaponType m_type)
	{
		ResetSelectBtn();
		switch (m_type)
		{
		case WeaponType.CloseWeapon:
			if (btn_select_list[0] != null)
			{
				btn_select_list[0].SetSelected(true);
				btn_select_now = btn_select_list[0];
			}
			break;
		case WeaponType.Crossbow:
			if (btn_select_list[1] != null)
			{
				btn_select_list[1].SetSelected(true);
				btn_select_now = btn_select_list[1];
			}
			break;
		case WeaponType.MachineGun:
			if (btn_select_list[2] != null)
			{
				btn_select_list[2].SetSelected(true);
				btn_select_now = btn_select_list[2];
			}
			break;
		case WeaponType.ViolenceGun:
			if (btn_select_list[3] != null)
			{
				btn_select_list[3].SetSelected(true);
				btn_select_now = btn_select_list[3];
			}
			break;
		case WeaponType.LiquidFireGun:
			if (btn_select_list[4] != null)
			{
				btn_select_list[4].SetSelected(true);
				btn_select_now = btn_select_list[4];
			}
			break;
		case WeaponType.RPG:
			if (btn_select_list[5] != null)
			{
				btn_select_list[5].SetSelected(true);
				btn_select_now = btn_select_list[5];
			}
			break;
		case WeaponType.Stoneskin:
			if (btn_select_list[6] != null)
			{
				btn_select_list[6].SetSelected(true);
				btn_select_now = btn_select_list[6];
			}
			break;
		}
	}

	public void ResetSelectBtn()
	{
		for (int i = 0; i < btn_select_list.Length; i++)
		{
			TUIButtonSelect tUIButtonSelect = btn_select_list[i];
			if (tUIButtonSelect != null)
			{
				tUIButtonSelect.Reset();
			}
		}
		btn_select_now = null;
	}

	public void SetNewMark(int m_id, NewMarkType m_type)
	{
		switch (m_id)
		{
		case 1:
			btn_select01.GetComponent<WeaponKindItemBtn>().ShowNewMark(m_type);
			break;
		case 2:
			btn_select02.GetComponent<WeaponKindItemBtn>().ShowNewMark(m_type);
			break;
		case 3:
			btn_select03.GetComponent<WeaponKindItemBtn>().ShowNewMark(m_type);
			break;
		case 4:
			btn_select04.GetComponent<WeaponKindItemBtn>().ShowNewMark(m_type);
			break;
		case 5:
			btn_select05.GetComponent<WeaponKindItemBtn>().ShowNewMark(m_type);
			break;
		case 6:
			btn_select06.GetComponent<WeaponKindItemBtn>().ShowNewMark(m_type);
			break;
		case 7:
			btn_select07.GetComponent<WeaponKindItemBtn>().ShowNewMark(m_type);
			break;
		default:
			Debug.Log("error!");
			break;
		}
	}

	public NewMarkType GetNewMark(int m_id)
	{
		switch (m_id)
		{
		case 1:
			return btn_select01.GetComponent<WeaponKindItemBtn>().GetNewMark();
		case 2:
			return btn_select02.GetComponent<WeaponKindItemBtn>().GetNewMark();
		case 3:
			return btn_select03.GetComponent<WeaponKindItemBtn>().GetNewMark();
		case 4:
			return btn_select04.GetComponent<WeaponKindItemBtn>().GetNewMark();
		case 5:
			return btn_select05.GetComponent<WeaponKindItemBtn>().GetNewMark();
		case 6:
			return btn_select06.GetComponent<WeaponKindItemBtn>().GetNewMark();
		case 7:
			return btn_select07.GetComponent<WeaponKindItemBtn>().GetNewMark();
		default:
			Debug.Log("error!");
			return NewMarkType.None;
		}
	}

	public TUIButtonSelect GetSelectBtn()
	{
		return btn_select_now;
	}
}
