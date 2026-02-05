using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class FrameCounter : MonoBehaviour
{
    public float updateInterval = 0.5f;
    private float m_accum;
    private int m_frames;
    private float m_timeLeft;
    private Text uiText;

    private void Awake()
    {
        m_timeLeft = updateInterval;
        uiText = GetComponent<Text>();
        if (uiText == null)
        {
            Debug.LogError("FrameCounter requires a Text component.");
        }
    }

    private void Update()
    {
        if (uiText == null) return;

        m_timeLeft -= Time.deltaTime;
        m_accum += Time.timeScale / Time.deltaTime;
        m_frames++;

        if (m_timeLeft <= 0f)
        {
            float fps = m_accum / m_frames;
            float msPerFrame = 1000f / fps;
            uiText.text = string.Format("timePerFrame: {0:0.00}ms\nframePerSecond: {1:0.00}", msPerFrame, fps);

            m_timeLeft = updateInterval;
            m_accum = 0f;
            m_frames = 0;
        }
    }
}
