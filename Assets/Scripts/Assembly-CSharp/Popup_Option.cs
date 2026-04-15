using UnityEngine;

public class Popup_Option : MonoBehaviour
{
    public TUIButtonPush btn_music;
    public TUIButtonPush btn_sfx;
    public TUIButtonPush btn_amb;
    public Transform go_popup;

    private bool music_open_now = true;
    private bool sfx_open_now = true;
    private bool ambience_open_now = true;

    public void SetOption(bool m_music_open, bool m_sfx_open, bool m_amb_open)
    {
        music_open_now = m_music_open;
        sfx_open_now = m_sfx_open;
        ambience_open_now = m_amb_open;

        btn_music.m_bPressed = !music_open_now;
        btn_music.Show();

        btn_sfx.m_bPressed = !sfx_open_now;
        btn_sfx.Show();

        if (btn_amb != null)
        {
            btn_amb.m_bPressed = !ambience_open_now;
            btn_amb.Show();
        }
    }

    public void Show()
    {
        transform.localPosition = new Vector3(0f, 0f, transform.localPosition.z);
        if (go_popup != null && go_popup.GetComponent<Animation>() != null)
        {
            go_popup.GetComponent<Animation>().Play();
        }
    }

    public void Hide()
    {
        transform.localPosition = new Vector3(0f, -1000f, transform.localPosition.z);
    }

    public void SetMusicNow()
    {
        music_open_now = !music_open_now;
        btn_music.m_bPressed = !music_open_now;
        btn_music.Show();
    }

    public void SetSFXNow()
    {
        sfx_open_now = !sfx_open_now;
        btn_sfx.m_bPressed = !sfx_open_now;
        btn_sfx.Show();
    }

    public void SetAmbNow()
    {
        ambience_open_now = !ambience_open_now;
        if (btn_amb != null)
        {
            btn_amb.m_bPressed = !ambience_open_now;
            btn_amb.Show();
        }
    }

    public bool GetMusicNow()
    {
        return music_open_now;
    }

    public bool GetSFXNow()
    {
        return sfx_open_now;
    }

    public bool GetAmbNow()
    {
        return ambience_open_now;
    }

    public void RestoreOption()
    {
        btn_music.m_bPressed = !music_open_now;
        btn_music.Show();

        btn_sfx.m_bPressed = !sfx_open_now;
        btn_sfx.Show();

        if (btn_amb != null)
        {
            btn_amb.m_bPressed = !ambience_open_now;
            btn_amb.Show();
        }
    }
}