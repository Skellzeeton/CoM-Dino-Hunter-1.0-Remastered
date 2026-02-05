using UnityEngine;

public class gyUITutorialsPanel : MonoBehaviour
{
	public GameObject mMask;

	public GameObject[] arrTutorials;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void Show(bool bShow)
	{
		base.gameObject.SetActiveRecursively(bShow);
		if (arrTutorials != null)
		{
			for (int i = 0; i < arrTutorials.Length; i++)
			{
				arrTutorials[i].SetActiveRecursively(bShow);
			}
		}
	}
}
