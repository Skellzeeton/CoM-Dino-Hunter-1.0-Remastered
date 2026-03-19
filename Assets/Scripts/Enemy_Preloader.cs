using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class Enemy_Preloader : MonoBehaviour
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
        if (!hasPreloaded)
        {
            StartCoroutine(PreloadPrefabs());
            hasPreloaded = true;
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
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

        if (IsNormalMapScene(scene.name))
        {
            if (!hasPreloaded)
            {
                StartCoroutine(PreloadPrefabs());
                hasPreloaded = true;
            }
        }
        else
        {
        }
    }

    private bool IsNormalMapScene(string sceneName)
    {
        if (sceneName.IndexOf("Yulin") != -1) return true;
        string[] normalScenes = {"SceneForest", "SceneIce", "SceneLava", "SceneLava2", "SceneSnow"};
        for (int i = 0; i < normalScenes.Length; i++)
        {
            if (sceneName == normalScenes[i]) return true;
        }
        return false;
    }

    private IEnumerator PreloadPrefabs()
    {
        foreach (var entry in prefabsToPreload)
        {
            if (entry == null || entry.prefab == null)
                continue;

            GameObject inst = Instantiate(entry.prefab);
            DontDestroyOnLoad(inst);
            inst.transform.SetParent(null);
            inst.SetActive(true);
            Animator animator = inst.GetComponentInChildren<Animator>();
            if (animator != null && animator.runtimeAnimatorController != null)
            {
                animator.Play(animator.GetCurrentAnimatorStateInfo(0).shortNameHash);
                animator.Update(0f);
            }
            
            Animation legacy = inst.GetComponentInChildren<Animation>();
            if (legacy != null)
            {
                foreach (AnimationState state in legacy)
                {
                    legacy.Play(state.name);
                    legacy.Sample();
                    legacy.Stop();
                }
            }

            inst.SetActive(false);
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
