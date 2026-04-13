using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TAudioController : MonoBehaviour
{
	public delegate void OnAudioEventPlay(ref string eventName);

	public bool useAuidoEvent = true;

	private OnAudioEventPlay onAudioEventPlay;
	private readonly Dictionary<string, GameObject> m_loadedAudio = new Dictionary<string, GameObject>();
	private readonly HashSet<string> m_alwaysPreloaded = new HashSet<string>();

	private Transform m_audioRoot;
	private Coroutine m_unloadRoutine;

	private void Awake()
	{
		Transform t = transform.Find("Audio");
		if (t == null)
		{
			GameObject go = new GameObject("Audio");
			go.transform.parent = transform;
			go.transform.localPosition = Vector3.zero;
			m_audioRoot = go.transform;
		}
		else
		{
			m_audioRoot = t;
		}
	}

	public void PlayAudio(string objName)
	{
		if (string.IsNullOrEmpty(objName) || !useAuidoEvent)
		{
			return;
		}

		if (onAudioEventPlay != null)
		{
			onAudioEventPlay(ref objName);
		}

		string shortName = GetShortName(objName);
		GameObject audioObj = GetOrCreateAudio(objName, shortName);
		if (audioObj == null)
		{
			return;
		}

		ITAudioEvent evt = audioObj.GetComponent<ITAudioEvent>();
		if (evt != null)
		{
			evt.Trigger();
		}
	}

	public void StopAudio(string audioName)
	{
		if (string.IsNullOrEmpty(audioName))
		{
			return;
		}

		string shortName = GetShortName(audioName);
		GameObject obj;
		if (m_loadedAudio.TryGetValue(shortName, out obj))
		{
			ITAudioEvent evt = obj.GetComponent<ITAudioEvent>();
			if (evt != null)
			{
				evt.Stop();
			}
		}
	}

	public void SetAudioEventPlayDelegate(OnAudioEventPlay onAudioEventDelegate)
	{
		onAudioEventPlay = onAudioEventDelegate;
	}

	public void PreloadAlwaysSounds()
	{
		GameObject[] prefabs = Resources.LoadAll<GameObject>("SoundEvent");
		if (prefabs == null)
		{
			return;
		}

		for (int i = 0; i < prefabs.Length; i++)
		{
			GameObject prefab = prefabs[i];
			if (prefab == null)
			{
				continue;
			}

			if (!PrefabHasAlwaysPreload(prefab))
			{
				continue;
			}

			PreloadPrefab(prefab);
		}
	}

	public void ScheduleUnloadAfterSceneChange(float delaySeconds)
	{
		if (m_unloadRoutine != null)
		{
			StopCoroutine(m_unloadRoutine);
		}
		m_unloadRoutine = StartCoroutine(CoUnloadAfterSceneChange(delaySeconds));
	}

	private IEnumerator CoUnloadAfterSceneChange(float delaySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);

		UnloadTransientAudio();

		yield return Resources.UnloadUnusedAssets();
		m_unloadRoutine = null;
	}

	private GameObject GetOrCreateAudio(string fullName, string shortName)
	{
		GameObject obj;
		if (m_loadedAudio.TryGetValue(shortName, out obj))
		{
			return obj;
		}

		Transform existing = m_audioRoot.Find(shortName);
		if (existing != null)
		{
			obj = existing.gameObject;
			m_loadedAudio.Add(shortName, obj);
			RegisterAlwaysPreloadIfNeeded(obj, shortName);
			WarmClips(obj);
			return obj;
		}

		GameObject prefab = Resources.Load("SoundEvent/" + fullName) as GameObject;
		if (prefab == null)
		{
			Debug.LogWarning(fullName + " is null");
			return null;
		}

		obj = Instantiate(prefab) as GameObject;
		if (obj == null)
		{
			return null;
		}

		obj.name = shortName;
		obj.transform.parent = m_audioRoot;
		obj.transform.localPosition = Vector3.zero;
		obj.transform.localRotation = Quaternion.identity;

		m_loadedAudio.Add(shortName, obj);
		RegisterAlwaysPreloadIfNeeded(obj, shortName);
		WarmClips(obj);

		return obj;
	}

	private void PreloadPrefab(GameObject prefab)
	{
		string shortName = prefab.name;
		if (string.IsNullOrEmpty(shortName))
		{
			return;
		}

		if (m_loadedAudio.ContainsKey(shortName))
		{
			m_alwaysPreloaded.Add(shortName);
			return;
		}

		GameObject obj = Instantiate(prefab) as GameObject;
		if (obj == null)
		{
			return;
		}

		obj.name = shortName;
		obj.transform.parent = m_audioRoot;
		obj.transform.localPosition = Vector3.zero;
		obj.transform.localRotation = Quaternion.identity;

		m_loadedAudio.Add(shortName, obj);
		m_alwaysPreloaded.Add(shortName);
		WarmClips(obj);
	}

	private void WarmClips(GameObject obj)
	{
		if (obj == null)
		{
			return;
		}

		TAudioEffectRandom[] randomEffects = obj.GetComponentsInChildren<TAudioEffectRandom>(true);
		if (randomEffects == null)
		{
			return;
		}

		for (int i = 0; i < randomEffects.Length; i++)
		{
			TAudioEffectRandom effect = randomEffects[i];
			if (effect != null && effect.alwaysPreload)
			{
				AudioClip[] clips = effect.audioClips;
				if (clips == null)
				{
					continue;
				}

				for (int j = 0; j < clips.Length; j++)
				{
					AudioClip clip = clips[j];
					if (clip != null)
					{
						clip.LoadAudioData();
					}
				}
			}
		}
	}

	private void RegisterAlwaysPreloadIfNeeded(GameObject obj, string shortName)
	{
		if (obj == null)
		{
			return;
		}

		TAudioEffectRandom[] effects = obj.GetComponentsInChildren<TAudioEffectRandom>(true);
		if (effects == null)
		{
			return;
		}

		for (int i = 0; i < effects.Length; i++)
		{
			TAudioEffectRandom effect = effects[i];
			if (effect != null && effect.alwaysPreload)
			{
				m_alwaysPreloaded.Add(shortName);
				return;
			}
		}
	}

	private bool PrefabHasAlwaysPreload(GameObject prefab)
	{
		TAudioEffectRandom[] effects = prefab.GetComponentsInChildren<TAudioEffectRandom>(true);
		if (effects == null)
		{
			return false;
		}

		for (int i = 0; i < effects.Length; i++)
		{
			TAudioEffectRandom effect = effects[i];
			if (effect != null && effect.alwaysPreload)
			{
				return true;
			}
		}
		return false;
	}

	private bool IsAudioStillPlaying(GameObject obj)
	{
		if (obj == null)
		{
			return false;
		}

		ITAudioEvent[] events = obj.GetComponentsInChildren<ITAudioEvent>(true);
		if (events == null)
		{
			return false;
		}

		for (int i = 0; i < events.Length; i++)
		{
			ITAudioEvent evt = events[i];
			if (evt != null && (evt.isPlaying || evt.isLoop))
			{
				return true;
			}
		}
		return false;
	}

	private void UnloadTransientAudio()
	{
		List<string> removeKeys = new List<string>();

		foreach (KeyValuePair<string, GameObject> kvp in m_loadedAudio)
		{
			if (m_alwaysPreloaded.Contains(kvp.Key))
			{
				continue;
			}

			if (IsAudioStillPlaying(kvp.Value))
			{
				continue;
			}

			if (kvp.Value != null)
			{
				Object.Destroy(kvp.Value);
			}
			removeKeys.Add(kvp.Key);
		}

		for (int i = 0; i < removeKeys.Count; i++)
		{
			m_loadedAudio.Remove(removeKeys[i]);
		}
	}

	private string GetShortName(string path)
	{
		int i = path.LastIndexOf('/');
		if (i >= 0)
		{
			path = path.Substring(i + 1);
		}
		return path;
	}

	private void OnDestroy()
	{
		UnloadAll();
	}

	public void UnloadAll()
	{
		if (m_unloadRoutine != null)
		{
			StopCoroutine(m_unloadRoutine);
			m_unloadRoutine = null;
		}

		foreach (KeyValuePair<string, GameObject> kvp in m_loadedAudio)
		{
			if (kvp.Value != null)
			{
				Object.Destroy(kvp.Value);
			}
		}

		m_loadedAudio.Clear();
		m_alwaysPreloaded.Clear();
	}
}