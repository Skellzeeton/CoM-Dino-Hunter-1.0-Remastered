using EventCenter;
using UnityEngine;

public class Scene_Main : MonoBehaviour
{
    public TUIFade m_fade;

    private float m_fade_in_time;

    private float m_fade_out_time;

    private bool do_fade_in;

    private bool is_fade_out;

    private bool do_fade_out;

    private string next_scene = "Scene_MainMenu";

    private int next_scene_id;

    private bool sfx_open_now = true;

    private bool music_open_now = true;

    public TUILabel label_text;

    public PopupIAP popup_warning;

    private bool connect_success;

    private ServerConnectFailType server_connect_fail;

    private void Awake()
    {
        TUIDataServer.Instance().Initialize();
        global::EventCenter.EventCenter.Instance.Register<TUIEvent.BackEvent_SceneMain>(TUIEvent_SetUIInfo);
    }

    private void Start()
    {
        global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneMain("TUIEvent_EnterInfo"));
        if (music_open_now)
        {
            CUISound.GetInstance().Play("BGM_theme");
        }
    }

    private void Update()
    {
        if (m_fade == null)
        {
            Debug.Log("error!no found m_fade!");
        }
        m_fade_in_time += Time.deltaTime;
        if (m_fade_in_time >= m_fade.fadeInTime && !do_fade_in)
        {
            do_fade_in = true;
        }
        if (!is_fade_out)
        {
            return;
        }
        m_fade_out_time += Time.deltaTime;
        if (!(m_fade_out_time >= m_fade.fadeOutTime) || do_fade_out)
        {
            return;
        }
        do_fade_out = true;
        m_fade.SetFadeOutEnd();
        if (next_scene_id != 0)
        {
            TUIMappingInfo.SwitchSceneInt switchSceneInt = TUIMappingInfo.Instance().GetSwitchSceneInt();
            if (switchSceneInt != null)
            {
                switchSceneInt(next_scene_id);
            }
        }
        else
        {
            TUIMappingInfo.SwitchSceneStr switchSceneStr = TUIMappingInfo.Instance().GetSwitchSceneStr();
            if (switchSceneStr != null)
            {
                switchSceneStr(next_scene);
            }
        }
    }

    private void OnDestroy()
    {
        global::EventCenter.EventCenter.Instance.Unregister<TUIEvent.BackEvent_SceneMain>(TUIEvent_SetUIInfo);
    }

    public void TUIEvent_SetUIInfo(object sender, TUIEvent.BackEvent_SceneMain m_event)
    {
        if (m_event.GetEventName() == "TUIEvent_OptionInfo")
        {
            if (m_event.GetEventInfo() == null || m_event.GetEventInfo().GetOptionInfo() == null)
            {
                Debug.Log("error!");
                return;
            }
            sfx_open_now = m_event.GetEventInfo().GetOptionInfo().sfx_open;
            music_open_now = m_event.GetEventInfo().GetOptionInfo().music_open;
        }
        else if (m_event.GetEventName() == "TUIEvent_EnterInfo")
        {
            connect_success = true;
            if (label_text != null)
            {
                label_text.Text = "touch to play";
            }
        }
        else if (m_event.GetEventName() == "TUIEvent_EnterLevel")
        {
            if (m_event.GetControlSuccess())
            {
                int wparam = m_event.GetWparam();
                if (wparam != 0)
                {
                    next_scene_id = wparam;
                }
                else
                {
                    next_scene = "Scene_MainMenu";
                }
                if (!is_fade_out)
                {
                    is_fade_out = true;
                    m_fade.FadeOut();
                }
            }
            else
            {
                m_fade_in_time = 0f;
                do_fade_in = false;
                m_fade.FadeIn();
            }
        }
        else
        {
            connect_success = true;
            if (label_text != null)
            {
                label_text.Text = "touch to play";
            }
            else
            {
                Debug.Log("error!");
            }
        }
    }

    public void TUIEvent_Enter(TUIControl control, int event_type, float wparam, float lparam, object data)
    {
        if (event_type == 3)
        {
            if (sfx_open_now)
            {
                CUISound.GetInstance().Play("UI_Entergame");
            }
            global::EventCenter.EventCenter.Instance.Publish(this, new TUIEvent.SendEvent_SceneMain("TUIEvent_EnterLevel"));
        }
    }

    public void TUIEvent_CloseWarnning(TUIControl control, int event_type, float wparam, float lparam, object data)
    {
        if (event_type == 3)
        {
            if (sfx_open_now)
            {
                CUISound.GetInstance().Play("UI_Button");
            }
            if (popup_warning != null)
            {
                popup_warning.Hide();
            }
            else
            {
                Debug.Log("error!");
            }
        }
    }
}
