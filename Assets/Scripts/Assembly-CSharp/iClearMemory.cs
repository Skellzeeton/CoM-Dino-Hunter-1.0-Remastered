using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Scripting;

public class iClearMemory : MonoBehaviour
{
	private const float ReEnableDelaySeconds = 30f;

	private Coroutine reEnableRoutine;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public void ClearMemory()
	{
		DisableAutomaticGC();
		ClearImmidately();
		ScheduleReEnableAutomaticGC();
	}

	protected IEnumerator Clear()
	{
		DisableAutomaticGC();
		GC.Collect();
		yield return Resources.UnloadUnusedAssets();
		ScheduleReEnableAutomaticGC();
	}

	protected void ClearImmidately()
	{
		GC.Collect();
		Resources.UnloadUnusedAssets();
	}

	private void DisableAutomaticGC()
	{
		if (Application.isEditor)
			return;
		GarbageCollector.GCMode = GarbageCollector.Mode.Disabled;
	}

	private void ScheduleReEnableAutomaticGC()
	{
		if (Application.isEditor)
			return;
		if (reEnableRoutine != null)
			StopCoroutine(reEnableRoutine);
		reEnableRoutine = StartCoroutine(ReEnableAutomaticGCAfterDelay());
	}

	private IEnumerator ReEnableAutomaticGCAfterDelay()
	{
		yield return new WaitForSecondsRealtime(ReEnableDelaySeconds);
		GarbageCollector.GCMode = GarbageCollector.Mode.Enabled;
		reEnableRoutine = null;
	}
}