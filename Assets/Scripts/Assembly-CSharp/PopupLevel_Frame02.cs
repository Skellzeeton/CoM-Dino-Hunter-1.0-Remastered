using UnityEngine;

public class PopupLevel_Frame02 : MonoBehaviour
{
	public TUILabel label_introduce;



	public void SetInfo(string m_introduce)
	{
		if (label_introduce == null)
		{
			Debug.Log("error!");
		}
		else
		{
			label_introduce.Text = m_introduce;
		}
	}
}
