using UnityEngine;

public class gyUIGamePauseDialog : MonoBehaviour
{
	public gyUISwitch mMusicSwitch;

	public gyUISwitch mSoundSwitch;

	public UILabel mTaskDesc;

	public GameObject mbtnClose;

	public GameObject mbtnVillage;

	public GameObject mbtnContinue;

	protected bool m_bShow;

	public void Show(bool bShow)
	{
		m_bShow = bShow;
		base.gameObject.SetActiveRecursively(bShow);
		if (bShow)
		{
			base.transform.localPosition = new Vector3(0f, 0f, base.transform.localPosition.z);
		}
		else
		{
			base.transform.localPosition = new Vector3(10000f, 10000f, base.transform.localPosition.z);
		}
	}
}
