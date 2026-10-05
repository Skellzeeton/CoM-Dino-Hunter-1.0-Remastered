using UnityEngine;

public class LevelMapSign : MonoBehaviour
{
	public GameObject go_sign;



	public void PlaySignAnimation()
	{
		if (go_sign != null && go_sign.GetComponent<Animation>() != null)
		{
			go_sign.GetComponent<Animation>().wrapMode = WrapMode.Loop;
			go_sign.GetComponent<Animation>().Play();
		}
	}
}
