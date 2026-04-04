using UnityEngine;
using System.Collections;

public class iSplashEffect : MonoBehaviour
{
    public Vector2 m_v2RandomTime = new Vector2(8f, 12f);
    private const float m_fTimeStep = 0.5f;
    protected bool m_bActive;
    protected float m_fNextTime;
    protected float m_fLastTime;
    protected ParticleSystem[] m_arrParticleSystem;
    protected TAudioController m_AudioController;

    private void Awake()
    {
        m_arrParticleSystem = GetComponentsInChildren<ParticleSystem>();
        if (m_arrParticleSystem != null)
        {
            foreach (ParticleSystem ps in m_arrParticleSystem)
            {
                var emission = ps.emission;
                emission.enabled = false;
            }
        }
        m_fNextTime = GetRandomTime();
        m_AudioController = GetComponent<TAudioController>();
        if (m_AudioController == null)
        {
            m_AudioController = gameObject.AddComponent<TAudioController>();
        }
    }

    private void Update()
    {
        if (m_bActive)
            return;
        m_fNextTime -= Time.deltaTime;
        if (m_fNextTime <= 0f)
        {
            Play();
        }
    }

    float GetRandomTime()
    {
        float newTime;
        int safety = 0;
        do
        {
            float raw = Random.Range(m_v2RandomTime.x, m_v2RandomTime.y);
            newTime = Mathf.Round(raw / m_fTimeStep) * m_fTimeStep;
            safety++;
        }
        while (Mathf.Approximately(newTime, m_fLastTime) && safety < 10);
        m_fLastTime = newTime;
        return newTime;
    }

    protected void Play()
    {
        if (m_arrParticleSystem != null)
        {
            foreach (ParticleSystem ps in m_arrParticleSystem)
            {
                var emission = ps.emission;
                emission.enabled = true;
                ps.Play();
            }
        }
        m_bActive = true;
        StartCoroutine(PlaySplashAudioDelayed());
        StartCoroutine(AutoStop());
    }

    IEnumerator PlaySplashAudioDelayed()
    {
        yield return new WaitForSeconds(2f);

        if (m_AudioController != null)
        {
            m_AudioController.PlayAudio("Amb_Splash");
        }
    }

    IEnumerator AutoStop()
    {
        float maxDuration = 0f;
        if (m_arrParticleSystem != null)
        {
            foreach (ParticleSystem ps in m_arrParticleSystem)
            {
                float duration = ps.main.duration + ps.main.startLifetime.constantMax;
                if (duration > maxDuration)
                    maxDuration = duration;
            }
        }
        yield return new WaitForSeconds(maxDuration);
        Stop();
    }

    protected void Stop()
    {
        if (m_arrParticleSystem != null)
        {
            foreach (ParticleSystem ps in m_arrParticleSystem)
            {
                var emission = ps.emission;
                emission.enabled = false;
                ps.Stop();
            }
        }
        m_bActive = false;
        m_fNextTime = GetRandomTime();
    }
}