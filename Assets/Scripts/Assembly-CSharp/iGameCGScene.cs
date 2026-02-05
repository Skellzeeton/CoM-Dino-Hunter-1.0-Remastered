using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI;

public class iGameCGScene : MonoBehaviour
{
    [Header("Video Setup")]
    public VideoPlayer videoPlayer;
    public RawImage videoImage;
    public RenderTexture renderTexture;
    public VideoClip introClip;

    private bool videoSkipped = false;

    private void Start()
    {
        if (renderTexture == null)
        {
            renderTexture = new RenderTexture(1920, 1080, 0);
            renderTexture.name = "IntroVideoRenderTexture";
        }
        if (videoImage == null)
        {
            GameObject canvasGO = new GameObject("IntroVideoCanvas");
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();

            GameObject rawImageGO = new GameObject("VideoRawImage");
            rawImageGO.transform.SetParent(canvasGO.transform, false);
            videoImage = rawImageGO.AddComponent<RawImage>();

            RectTransform rt = videoImage.rectTransform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            videoImage.texture = renderTexture;
        }
        if (videoPlayer == null || introClip == null)
        {
            Debug.LogWarning("Intro video setup incomplete, loading main scene directly.");
            SceneManager.LoadScene("Scene_Main");
            return;
        }

        StartCoroutine(PlayIntroVideo());
    }

    private void Update()
    {
        if (videoSkipped) return;

#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            SkipVideo("Mouse Click");
        }
#elif UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
        {
            SkipVideo("Touch Input");
        }
#endif
    }

    private System.Collections.IEnumerator PlayIntroVideo()
    {
        videoSkipped = false;

        videoPlayer.targetTexture = renderTexture;
        videoImage.texture = renderTexture;

        videoPlayer.Stop();
        videoPlayer.clip = introClip;
        videoPlayer.Prepare();

        while (!videoPlayer.isPrepared)
            yield return null;

        videoPlayer.Play();

        // Wait until finished or skipped
        while (videoPlayer.isPlaying && !videoSkipped)
            yield return null;

        LoadMainScene();
    }

    private void SkipVideo(string reason)
    {
        if (videoSkipped) return;

        Debug.Log("Video skipped: " + reason);
        videoSkipped = true;

        if (videoPlayer != null && videoPlayer.isPlaying)
            videoPlayer.Stop();

        LoadMainScene();
    }

    private void LoadMainScene()
    {
        SceneManager.LoadScene("Scene_Main");
    }
}
