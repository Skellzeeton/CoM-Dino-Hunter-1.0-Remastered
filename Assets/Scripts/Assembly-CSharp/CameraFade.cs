using UnityEngine;
using UnityEngine.UI;

public class CameraFade : MonoBehaviour
{
    protected static CameraFade fade;
    protected static GameObject cameraFade;

    public Color fadeInColor = new Color(0.5f, 0.5f, 0.5f, 0f);
    public Color fadeOutColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    protected float m_time;
    protected bool isfadein;
    protected bool isfadeout;
    protected float lasttime;

    private Image fadeImage;

    public void Init()
    {
        // Create a Canvas if none exists
        Canvas canvas = cameraFade.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10000; // mimic cameraFadeDepth

        // Add Image
        GameObject imgObj = new GameObject("FadeImage");
        imgObj.transform.SetParent(cameraFade.transform, false);
        fadeImage = imgObj.AddComponent<Image>();
        fadeImage.rectTransform.anchorMin = Vector2.zero;
        fadeImage.rectTransform.anchorMax = Vector2.one;
        fadeImage.rectTransform.offsetMin = Vector2.zero;
        fadeImage.rectTransform.offsetMax = Vector2.zero;
        fadeImage.color = fadeInColor;
    }

    public static void CameraFadeOut(float time)
    {
        Check();
        fade.FadeOut(time);
    }

    public static void CameraFadeIn(float time)
    {
        Check();
        fade.FadeIn(time);
    }

    public static void Clear()
    {
        if (cameraFade != null)
        {
            Destroy(cameraFade);
            cameraFade = null;
            fade = null;
        }
    }

    protected static void Check()
    {
        if (cameraFade == null)
        {
            cameraFade = new GameObject("CameraFade");
            fade = cameraFade.AddComponent<CameraFade>();
            fade.Init();
        }
    }

    protected void FadeOut(float time)
    {
        m_time = time;
        isfadeout = true;
        isfadein = false;
        lasttime = 0f;
    }

    protected void FadeIn(float time)
    {
        m_time = time;
        isfadein = true;
        isfadeout = false;
        lasttime = 0f;
    }

    private void Update()
    {
        if (isfadeout || isfadein)
        {
            lasttime += Time.deltaTime;
            if (fadeImage != null)
            {
                if (isfadeout)
                {
                    fadeImage.color = Color.Lerp(fadeInColor, fadeOutColor, Mathf.Clamp01(lasttime / m_time));
                }
                else if (isfadein)
                {
                    fadeImage.color = Color.Lerp(fadeOutColor, fadeInColor, Mathf.Clamp01(lasttime / m_time));
                }
            }

            if (lasttime >= m_time)
            {
                isfadein = false;
                isfadeout = false;
                lasttime = 0f;
                m_time = 0f;
            }
        }
    }
}