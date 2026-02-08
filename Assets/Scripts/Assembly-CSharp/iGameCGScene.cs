using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI;

public class iGameCGScene : MonoBehaviour
{
    private void Start()
    {
        SceneManager.LoadScene("Scene_Main");
        Application.targetFrameRate = 60;
    }
}
