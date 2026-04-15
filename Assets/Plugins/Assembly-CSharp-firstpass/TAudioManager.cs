using System.Collections.Generic;
using UnityEngine;

public class TAudioManager : MonoBehaviour
{
    private class AudioInfo
    {
        public ITAudioEvent audioEvt;
        public bool loop;
        public bool sfx;
        public bool ambience;
        public float volume;
    }

    public System.Action<bool, bool, bool> onPersistSettings;

    private Dictionary<AudioSource, AudioInfo> m_playAudios = new Dictionary<AudioSource, AudioInfo>();
    private Dictionary<string, List<ITAudioRule>> m_audio_rules = new Dictionary<string, List<ITAudioRule>>();

    private bool m_isMusicOn = true;
    private bool m_isSoundOn = true;
    private bool m_isAmbienceOn = true;

    private float m_musicVolume = 1f;
    private float m_soundVolume = 1f;
    private float m_ambienceVolume = 1f;

    private AudioListener audioListener;
    private static TAudioManager s_instance;

    public static TAudioManager instance
    {
        get
        {
            if (s_instance == null && Application.isPlaying)
            {
                GameObject target = new GameObject("TAudioManager", typeof(TAudioManager));
                Object.DontDestroyOnLoad(target);
            }
            return s_instance;
        }
    }

    public static bool checkInstance
    {
        get { return s_instance != null; }
    }

    public void ApplySettings(bool musicOn, bool soundOn, bool ambienceOn)
    {
        m_isMusicOn = musicOn;
        m_isSoundOn = soundOn;
        m_isAmbienceOn = ambienceOn;
    }

    private void SaveSettings()
    {
        if (onPersistSettings != null)
        {
            onPersistSettings(m_isMusicOn, m_isSoundOn, m_isAmbienceOn);
        }
    }

    public bool isMusicOn
    {
        get { return m_isMusicOn; }
        set
        {
            if (m_isMusicOn == value)
                return;

            m_isMusicOn = value;

            foreach (KeyValuePair<AudioSource, AudioInfo> playAudio in m_playAudios)
            {
                AudioInfo info = playAudio.Value;
                if (!info.sfx && !info.ambience)
                {
                    if (m_isMusicOn)
                        playAudio.Key.Play();
                    else
                        playAudio.Key.Pause();
                }
            }

            SaveSettings();
        }
    }

    public bool isSoundOn
    {
        get { return m_isSoundOn; }
        set
        {
            if (m_isSoundOn == value)
                return;

            m_isSoundOn = value;

            foreach (KeyValuePair<AudioSource, AudioInfo> playAudio in m_playAudios)
            {
                AudioInfo info = playAudio.Value;
                if (info.sfx)
                {
                    if (m_isSoundOn && info.loop && info.audioEvt != null)
                        info.audioEvt.Trigger();
                    else
                        playAudio.Key.Stop();
                }
            }

            SaveSettings();
        }
    }

    public bool isAmbienceOn
    {
        get { return m_isAmbienceOn; }
        set
        {
            if (m_isAmbienceOn == value)
                return;

            m_isAmbienceOn = value;

            foreach (KeyValuePair<AudioSource, AudioInfo> playAudio in m_playAudios)
            {
                AudioInfo info = playAudio.Value;
                if (info.ambience)
                {
                    if (m_isAmbienceOn)
                        playAudio.Key.Play();
                    else
                        playAudio.Key.Pause();
                }
            }

            SaveSettings();
        }
    }

    private void Awake()
    {
        if (s_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        s_instance = this;
        DontDestroyOnLoad(gameObject);

        audioListener = FindAnyObjectByType<AudioListener>();
        if (!audioListener)
        {
            GameObject listenerObj = new GameObject("AudioListener", typeof(AudioListener));
            DontDestroyOnLoad(listenerObj);
            audioListener = listenerObj.GetComponent<AudioListener>();
        }
    }
	private void OnDestroy()
	{
		s_instance = null;
	}

	private void Update()
	{
		List<AudioSource> list = new List<AudioSource>();
		foreach (KeyValuePair<AudioSource, AudioInfo> playAudio in m_playAudios)
		{
			AudioSource key = playAudio.Key;
			AudioInfo value = playAudio.Value;
			if ((bool)value.audioEvt && !value.audioEvt.isPlaying)
			{
				list.Add(key);
			}
		}
		foreach (AudioSource item in list)
		{
			m_playAudios.Remove(item);
		}
	}

	public void Pause(AudioSource audio)
	{
		audio.Pause();
	}

	public void PlaySound(AudioSource audio, AudioClip clip, bool loop, bool cutoff)
	{
		if (!TryPlay(audio.gameObject, clip.length / audio.pitch))
		{
			return;
		}
		AudioInfo value;
		if (m_playAudios.TryGetValue(audio, out value))
		{
			value.volume = audio.volume;
		}
		else
		{
			value = new AudioInfo();
			value.audioEvt = audio.GetComponent<ITAudioEvent>();
			value.loop = loop;
			value.sfx = true;
			value.volume = audio.volume;
			m_playAudios.Add(audio, value);
		}
		audio.volume *= m_soundVolume;
		if (loop)
		{
			audio.loop = true;
			audio.clip = clip;
			if (m_isSoundOn)
			{
				audio.Play();
			}
			return;
		}
		audio.loop = false;
		if (cutoff)
		{
			audio.clip = clip;
		}
		if (m_isSoundOn)
		{
			if (cutoff)
			{
				audio.Play();
			}
			else
			{
				audio.PlayOneShot(clip);
			}
		}
	}

	public void PlaySound(AudioSource audio, AudioClip clip, bool loop)
	{
		PlaySound(audio, clip, loop, false);
	}

	public void StopSound(AudioSource audio)
	{
		audio.Stop();
		TryStop(audio.gameObject);
		if (m_playAudios.ContainsKey(audio))
		{
			m_playAudios.Remove(audio);
		}
	}

	public void PlayMusic(AudioSource audio, AudioClip clip, bool loop)
	{
		PlayMusic(audio, clip, loop, false);
	}

	public void PlayMusic(AudioSource audio, AudioClip clip, bool loop, bool cutoff)
	{
		if (!TryPlay(audio.gameObject, clip.length / audio.pitch))
		{
			return;
		}
		AudioInfo value;
		if (m_playAudios.TryGetValue(audio, out value))
		{
			value.volume = audio.volume;
		}
		else
		{
			value = new AudioInfo();
			value.audioEvt = audio.GetComponent<ITAudioEvent>();
			value.loop = loop;
			value.sfx = false;
			value.volume = audio.volume;
			m_playAudios.Add(audio, value);
		}
		audio.volume *= m_musicVolume;
		if (loop)
		{
			audio.loop = true;
			audio.clip = clip;
			if (m_isMusicOn)
			{
				audio.Play();
			}
			return;
		}
		audio.loop = false;
		if (cutoff)
		{
			audio.clip = clip;
		}
		if (m_isMusicOn)
		{
			if (cutoff)
			{
				audio.Play();
			}
			else
			{
				audio.PlayOneShot(clip);
			}
		}
	}

	public void StopMusic(AudioSource audio)
	{
		audio.Stop();
		TryStop(audio.gameObject);
		if (m_playAudios.ContainsKey(audio))
		{
			m_playAudios.Remove(audio);
		}
	}

	public void StopAll()
	{
		List<AudioInfo> list = new List<AudioInfo>();
		foreach (KeyValuePair<AudioSource, AudioInfo> playAudio in m_playAudios)
		{
			list.Add(playAudio.Value);
		}
		foreach (AudioInfo item in list)
		{
			item.audioEvt.Stop();
		}
		m_playAudios.Clear();
	}

	private bool TryPlay(GameObject go, float length)
	{
		string text = go.name;
		for (int num = text.IndexOf("(Clone)"); num >= 0; num = text.IndexOf("(Clone)"))
		{
			text = text.Substring(0, num);
		}
		List<ITAudioRule> value = null;
		if (m_audio_rules.TryGetValue(text, out value))
		{
			float over_time = Time.realtimeSinceStartup + length;
			foreach (ITAudioRule item in value)
			{
				if (!item.Try(text, go, over_time))
				{
					return false;
				}
			}
		}
		return true;
	}

	private void TryStop(GameObject go)
	{
		string text = go.name;
		for (int num = text.IndexOf("(Clone)"); num >= 0; num = text.IndexOf("(Clone)"))
		{
			text = text.Substring(0, num);
		}
		List<ITAudioRule> value = null;
		if (!m_audio_rules.TryGetValue(text, out value))
		{
			return;
		}
		foreach (ITAudioRule item in value)
		{
			item.Stop(go);
		}
	}

	public void RegistRule(string name, ITAudioRule rule)
	{
		List<ITAudioRule> value = null;
		if (m_audio_rules.TryGetValue(name, out value))
		{
			value.Add(rule);
			return;
		}
		value = new List<ITAudioRule>();
		value.Add(rule);
		m_audio_rules.Add(name, value);
	}

	public void ClearRegistRule()
	{
		m_audio_rules.Clear();
	}
}