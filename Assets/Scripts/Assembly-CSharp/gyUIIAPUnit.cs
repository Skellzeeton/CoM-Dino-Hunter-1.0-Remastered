using UnityEngine;
using gyIAPSystem;

public class gyUIIAPUnit : MonoBehaviour
{
	public UILabel mGainValue;

	public UISprite mGainIcon;

	public GameObject mButton;

	public UILabel mButtonLabel;

	public int nIndex;

	public int nIAPID;

	public string sPrice;

	protected CIAPInfo m_IAPInfo;

	private void Awake()
	{
		m_IAPInfo = iIAPManager.GetInstance().GetIAPInfo(nIAPID);
		if (m_IAPInfo != null)
		{
			if (mButtonLabel != null)
			{
				mButtonLabel.text = sPrice;
			}
			if (mGainValue != null)
			{
				mGainValue.text = m_IAPInfo.nValue.ToString();
			}
		}
	}
}
