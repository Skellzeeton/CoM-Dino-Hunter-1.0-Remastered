using System.Collections.Generic;
using UnityEngine;

public class TUILevelGoodsNeedInfo
{
	public List<TUIGoodsNeedInfo> level_goods_need01;

	public List<TUIGoodsNeedInfo> level_goods_need02;

	public List<TUIGoodsNeedInfo> level_goods_need03;

	public List<TUIGoodsNeedInfo> level_goods_need04;

	public List<TUIGoodsNeedInfo> level_goods_need05;

	public TUILevelGoodsNeedInfo(List<TUIGoodsNeedInfo> need01, List<TUIGoodsNeedInfo> need02, List<TUIGoodsNeedInfo> need03, List<TUIGoodsNeedInfo> need04, List<TUIGoodsNeedInfo> need05)
	{
		level_goods_need01 = need01;
		level_goods_need02 = need02;
		level_goods_need03 = need03;
		level_goods_need04 = need04;
		level_goods_need05 = need05;
	}

	public List<TUIGoodsNeedInfo> GetGoodsNeedInfo(int index)
	{
		switch (index)
		{
		case 1:
			return level_goods_need01;
		case 2:
			return level_goods_need02;
		case 3:
			return level_goods_need03;
		case 4:
			return level_goods_need04;
		case 5:
			return level_goods_need05;
		default:
			Debug.Log("can't find goods_need_info!");
			return null;
		}
	}
}
