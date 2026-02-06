using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class SkyScene_Preloader : MonoBehaviour
{
    [System.Serializable]
    public class PrefabEntry
    {
        public GameObject prefab;
    }

    public List<PrefabEntry> prefabsToPreload = new List<PrefabEntry>();

    private List<GameObject> tempInstances = new List<GameObject>();
    private string currentScene = "";
    private bool hasPreloaded = false;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == currentScene)
            return;

        currentScene = scene.name;

        if (IsSkyMapScene(scene.name))
        {
            if (!hasPreloaded)
            {
                StartCoroutine(PreloadPrefabs());
                hasPreloaded = true;
            }
        }
        else
        {
            UnloadPrefabs();
            hasPreloaded = false;
        }
    }

    private bool IsSkyMapScene(string sceneName)
    {
        return sceneName == "SceneGorge" ||
               sceneName == "SceneGorge 1" ||
               sceneName == "SceneGorge 2";
    }

    private IEnumerator PreloadPrefabs()
    {
        foreach (var entry in prefabsToPreload)
        {
            if (entry == null || entry.prefab == null)
                continue;

            GameObject inst = Instantiate(entry.prefab);
            inst.transform.SetParent(transform, false);
            inst.SetActive(true);
            tempInstances.Add(inst);
        }

        yield break;
    }

    private void UnloadPrefabs()
    {
        foreach (var obj in tempInstances)
        {
            if (obj != null)
                Destroy(obj);
        }
        tempInstances.Clear();
    }
}
