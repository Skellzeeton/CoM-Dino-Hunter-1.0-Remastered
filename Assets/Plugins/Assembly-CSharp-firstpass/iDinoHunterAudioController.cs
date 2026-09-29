using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(TAudioController))]
public class iDinoHunterAudioController : MonoBehaviour
{
	public bool m_bBoss;

	[System.Serializable]
	public class SoundOverridePair
	{
		public string originalName;
		public string overrideName;
	}

	[Header("Sound Overrides")]
	[Tooltip("Map an original sound name to a replacement. Only matching sounds will be changed.")]
	public List<SoundOverridePair> soundOverrides = new List<SoundOverridePair>();

	protected TAudioController m_AudioController;

	private void Awake()
	{
		m_AudioController = GetComponent<TAudioController>();
	}

	public void PlayAudioByMobType(string sName)
	{
		if (m_AudioController == null || base.transform.root == null)
			return;
		string finalName = GetOverriddenName(sName);
		if (m_bBoss)
			finalName += "_Boss";
		m_AudioController.PlayAudio(finalName);
	}

	private string GetOverriddenName(string originalName)
	{
		foreach (var pair in soundOverrides)
		{
			if (pair.originalName == originalName)
				return pair.overrideName;
		}
		return originalName;
	}
}