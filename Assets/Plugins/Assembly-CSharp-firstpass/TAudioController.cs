using UnityEngine;
using System.Collections.Generic;

public class TAudioController : MonoBehaviour
{
    public delegate void OnAudioEventPlay(ref string eventName);

    public bool useAuidoEvent = true;

    // NEW
    [Header("Preload Settings")]
    public bool alwaysPreload = false;

    private OnAudioEventPlay onAudioEventPlay;

    // cache loaded audio objects
    private Dictionary<string, GameObject> m_loadedAudio = new Dictionary<string, GameObject>();

    private Transform m_audioRoot;

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

        // preload everything already referenced (optional future use)
        if (alwaysPreload)
        {
            PreLoad();
        }
    }

    public void PlayAudio(string objName)
    {
        if (objName.Length < 1 || !useAuidoEvent)
            return;

        if (onAudioEventPlay != null)
            onAudioEventPlay(ref objName);

        string key = GetShortName(objName);

        GameObject audioObj = GetOrCreateAudio(objName, key);

        if (audioObj == null)
            return;

        ITAudioEvent evt = audioObj.GetComponent<ITAudioEvent>();
        if (evt != null)
            evt.Trigger();
    }

    GameObject GetOrCreateAudio(string fullName, string shortName)
    {
        GameObject obj;
        if (m_loadedAudio.TryGetValue(shortName, out obj))
            return obj;
        Transform existing = m_audioRoot.Find(shortName);
        if (existing != null)
        {
            obj = existing.gameObject;
            m_loadedAudio.Add(shortName, obj);
            return obj;
        }
        GameObject prefab = Resources.Load("SoundEvent/" + fullName) as GameObject;
        if (prefab == null)
        {
            Debug.LogWarning(fullName + " is null");
            return null;
        }
        obj = Instantiate(prefab);
        obj.name = shortName;
        obj.transform.parent = m_audioRoot;
        obj.transform.localPosition = Vector3.zero;
        m_loadedAudio.Add(shortName, obj);
        return obj;
    }

    public void StopAudio(string audioName)
    {
        if (audioName.Length < 1)
            return;
        string key = GetShortName(audioName);
        GameObject obj;
        if (m_loadedAudio.TryGetValue(key, out obj))
        {
            ITAudioEvent evt = obj.GetComponent<ITAudioEvent>();
            if (evt != null)
                evt.Stop();
        }
    }

    private string GetShortName(string path)
    {
        int i = path.LastIndexOf('/');
        if (i >= 0) path = path.Substring(i + 1);
        return path;
    }

    [ContextMenu("PreLoad")]
    private void PreLoad()
    {
        //Debug.Log("TAudioController PreLoad called on: " + name);
    }

    private void OnDestroy()
    {
        if (gameObject.scene.name == null || gameObject.scene.name == "")
            return;
        UnloadAll();
    }

    public void UnloadAll()
    {
        foreach (var kvp in m_loadedAudio)
        {
            if (kvp.Value != null)
                Destroy(kvp.Value);
        }

        m_loadedAudio.Clear();
    }
}