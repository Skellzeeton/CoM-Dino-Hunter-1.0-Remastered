using UnityEngine;

public class Lable_Blink : MonoBehaviour
{
	private float blink_time;

	private void Start()
	{
		blink_time = 0f;
	}

	private void Update()
	{
	}

	private void LateUpdate()
	{
		blink_time += Time.deltaTime;
		Color color = base.transform.GetComponent<TUILabel>().color;
		Color colorBK = base.transform.GetComponent<TUILabel>().colorBK;
		base.transform.GetComponent<TUILabel>().colorBK = new Color(colorBK.r, colorBK.g, colorBK.b, (128f + 127f * Mathf.Sin(blink_time * 5f)) / 255f);
		base.transform.GetComponent<TUILabel>().color = new Color(color.r, color.g, color.b, (128f + 127f * Mathf.Sin(blink_time * 5f)) / 255f);
	}
}
