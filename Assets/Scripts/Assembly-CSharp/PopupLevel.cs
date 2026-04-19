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

	public TUILevelInfo GetLevelInfo()
	{
		return level_info;
	}

	public void SetCachedLevelInfo(TUILevelInfo info)
	{
		level_info = info;
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
			return;
		}

		popuplevel_frame01.SetInfo(level_info.introduce01);
		popuplevel_frame02.SetInfo(level_info.introduce02);
		popuplevel_frame03.SetGoodsInfo(level_info.goods_drop_list);
		label_title.Text = m_level_info.title;
		base.transform.localPosition = new Vector3(0f, 0f, base.transform.localPosition.z);
		go_popup.GetComponent<Animation>().Play();
		if (img_title_bg != null)
		{
			string mapTexture = TUIMappingInfo.Instance().GetMapTexture(level_info.id);
			img_title_bg.texture = mapTexture;
		}

		RefreshRecommend();
		if (!popuplevel_frame03.GetOpenStart())
		{
			btn_start.Disable(true);
		}
		else
		{
			btn_start.Disable(false);
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

	public void RefreshRecommend()
	{
		if (level_info == null || popuplevel_frame03 == null)
		{
			return;
		}
		iGameData gameData = iGameApp.GetInstance().m_GameData;
		if (gameData == null)
		{
			return;
		}
		iDataCenter dataCenter = gameData.GetDataCenter();
		if (dataCenter == null)
		{
			return;
		}
		GameLevelInfo gameLevelInfo = gameData.GetGameLevelInfo(level_info.id);
		if (gameLevelInfo == null)
		{
			return;
		}
		TUIRecommendRoleInfo recommend_role_info = null;
		TUIRecommendWeaponInfo recommend_weapon_info = null;
		if (gameLevelInfo.m_nRecommandType == 1)
		{
			CWeaponInfoLevel weaponInfo =
				gameData.GetWeaponInfo(gameLevelInfo.m_nRecommandID, gameLevelInfo.m_nRecommandLevel);
			if (weaponInfo != null)
			{
				bool have_equip = false;
				int num = dataCenter.GetWeaponLevel(gameLevelInfo.m_nRecommandID);
				if (num <= 0)
				{
					num = 0;
				}
				else
				{
					for (int j = 0; j < 3; j++)
					{
						if (dataCenter.GetSelectWeapon(j) == gameLevelInfo.m_nRecommandID)
						{
							have_equip = true;
							break;
						}
					}
				}
				recommend_weapon_info = new TUIRecommendWeaponInfo(
					gameLevelInfo.m_nRecommandID,
					num,
					gameLevelInfo.m_nRecommandLevel,
					have_equip,
					gameLevelInfo.m_bRecommandLimit
				);
			}
		}
		else if (gameLevelInfo.m_nRecommandType == 2)
		{
			CCharacterInfoLevel characterInfo2 =
				gameData.GetCharacterInfo(gameLevelInfo.m_nRecommandID, gameLevelInfo.m_nRecommandLevel);
			if (characterInfo2 != null)
			{
				bool have_equip2 = false;
				bool have_buy = false;
				CCharSaveInfo character2 = dataCenter.GetCharacter(gameLevelInfo.m_nRecommandID);
				if (character2 != null)
				{
					if (character2.nLevel > 0)
					{
						have_buy = true;
					}
					if (dataCenter.CurCharID == gameLevelInfo.m_nRecommandID)
					{
						have_equip2 = true;
					}
				}
				recommend_role_info = new TUIRecommendRoleInfo(
					gameLevelInfo.m_nRecommandID,
					have_buy,
					have_equip2,
					gameLevelInfo.m_bRecommandLimit
				);
			}
		}
		else if (gameLevelInfo.m_nRecommandType == 3)
		{
			CItemInfoLevel itemInfo =
				gameData.GetItemInfo(gameLevelInfo.m_nRecommandID, gameLevelInfo.m_nRecommandLevel);
			if (itemInfo != null)
			{
				bool have_equip3 = false;
				int nItemLevel = 0;
				dataCenter.GetEquipStone(gameLevelInfo.m_nRecommandID, ref nItemLevel);
				if (nItemLevel <= 0)
				{
					nItemLevel = 0;
				}
				else if (dataCenter.CurEquipStone == gameLevelInfo.m_nRecommandID)
				{
					have_equip3 = true;
				}
				recommend_weapon_info = new TUIRecommendWeaponInfo(
					gameLevelInfo.m_nRecommandID,
					nItemLevel,
					gameLevelInfo.m_nRecommandLevel,
					have_equip3,
					gameLevelInfo.m_bRecommandLimit
				);
			}
		}
		popuplevel_frame03.SetRecommend(recommend_role_info, recommend_weapon_info);
	}
}
