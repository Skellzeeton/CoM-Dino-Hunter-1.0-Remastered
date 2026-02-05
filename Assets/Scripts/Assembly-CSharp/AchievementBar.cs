using UnityEngine;

public class AchievementBar : MonoBehaviour
{
	public TUISlider slider;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void Show(int m_value)
	{
		base.gameObject.SetActiveRecursively(true);
		if (slider != null)
		{
			slider.sliderValue = (float)m_value / 100f;
		}
	}

	public void Hide()
	{
		base.gameObject.SetActiveRecursively(false);
	}
}
