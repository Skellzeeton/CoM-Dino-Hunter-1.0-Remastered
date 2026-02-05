using System.Collections.Generic;
using UnityEngine;

internal class TUIInputManageriOS
{
	private static TUIInput[] m_input;

	public static void UpdateInput()
	{
		List<TUIInput> list = new List<TUIInput>();
		for (int i = 0; i < Input.touches.Length; i++)
		{
			TUIInput item = default(TUIInput);
			item.fingerId = Input.touches[i].fingerId;
			item.position = Input.touches[i].position;
			switch (Input.touches[i].phase)
			{
			case TouchPhase.Began:
				item.inputType = TUIInputType.Began;
				break;
			case TouchPhase.Moved:
				item.inputType = TUIInputType.Moved;
				break;
			case TouchPhase.Ended:
				item.inputType = TUIInputType.Ended;
				break;
			case TouchPhase.Stationary:
				item.inputType = TUIInputType.Stationary;
				break;
			case TouchPhase.Canceled:
				item.inputType = TUIInputType.Canceled;
				break;
			}
			list.Add(item);
		}
		m_input = list.ToArray();
	}

	public static TUIInput[] GetInput()
	{
		return m_input;
	}
}
