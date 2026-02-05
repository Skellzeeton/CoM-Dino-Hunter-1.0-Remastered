using UnityEngine;
using UnityEngine.SceneManagement;

public class TUIBlockManager : MonoBehaviour
{
    private static TUIBlockManager _instance;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        MoveBlockBGObjects();
    }

    private void MoveBlockBGObjects()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
        {
            if (go.name == "Block_BG")
            {
                Vector3 pos = go.transform.position;
                pos.z += 2f;
                go.transform.position = pos;
            }
        }
        TUIBlock[] tuiBlocks = FindObjectsOfType<TUIBlock>();
        foreach (TUIBlock block in tuiBlocks)
        {
            Vector3 pos = block.transform.position;
            pos.z += 2f;
            block.transform.position = pos;
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
