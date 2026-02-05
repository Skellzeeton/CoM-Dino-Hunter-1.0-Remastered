using gyTaskSystem;
using System;
using UnityEngine;

public class CControlWindows : CControlBase
{
    Vector2 zero;
    float sensetivity;
    const float maxSensetivity = 5f;
    const float minSensetivity = 0.5f;

    public static Action bulletPurchase;

    public CControlWindows()
    {
        m_GameScene = iGameApp.GetInstance().m_GameScene;
        m_GameUI = m_GameScene.GetGameUI();
        m_GameUI.RegisterEvent_Windows();
        m_DataCenter = iGameApp.GetInstance().m_GameData.GetDataCenter();
    }

    public override void Initialize()
    {
        base.Initialize();

        sensetivity = PlayerPrefs.GetFloat("sensetivity", 0.5f);
    }

    public override void Update(float deltaTime)
    {
        if (m_GameScene == null || m_User == null) return;

        if (Input.GetKeyDown(KeyCode.O))
        {
            sensetivity = Mathf.Clamp(sensetivity + 0.25f, minSensetivity, maxSensetivity);
            PlayerPrefs.SetFloat("sensetivity", sensetivity);
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            sensetivity = Mathf.Clamp(sensetivity - 0.25f, minSensetivity, maxSensetivity);
            PlayerPrefs.SetFloat("sensetivity", sensetivity);
        }
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
        if (m_DataCenter.isTutorial)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (m_GameScene.GameStatus == iGameSceneBase.kGameStatus.Gameing)
                {
                    m_GameScene.SetGamePause(true);

                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                else if (m_GameScene.GameStatus == iGameSceneBase.kGameStatus.Pause)
                {
                    m_GameScene.SetGamePause(false);
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
        }

        if (m_GameScene.GameStatus == iGameSceneBase.kGameStatus.CutScene && Input.GetKeyDown(KeyCode.Space))
            CCameraRoam.GetInstance().Stop();

        if (!(m_GameScene.GameStatus == iGameSceneBase.kGameStatus.Gameing || m_GameScene.GameStatus == iGameSceneBase.kGameStatus.GameOver_ShowTime)) return;

        zero.Set(0, 0);

        if (m_User.IsCanMove())
        {
            if (Input.GetKey(KeyCode.W)) zero.y += 1f;
            if (Input.GetKey(KeyCode.S)) zero.y += -1f;
            if (Input.GetKey(KeyCode.A)) zero.x += -1f;
            if (Input.GetKey(KeyCode.D)) zero.x += 1f;
        }
        if (zero == Vector2.zero) m_User.MoveStop();
        else
        {
            m_User.MoveByCompass(zero.x, zero.y);
            Ray ray = m_Camera.ScreenPointToRay(m_GameState.ScreenCenter, 0f);
            m_User.LookAt(ray.GetPoint(1000f));
        }

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            float axis = Input.GetAxis("Mouse X");
            if (axis != 0f)
            {
                m_Camera.Yaw(axis * 270f * Time.deltaTime);
                if (m_User.IsCanAim())
                {
                    m_User.SetYaw(m_Camera.GetYaw());
                }
            }
            float axis2 = Input.GetAxis("Mouse Y");
            if (axis2 != 0f)
            {
                m_Camera.Pitch(axis2 * 270f * Time.deltaTime);
                Ray ray2 = m_Camera.ScreenPointToRay(m_GameState.ScreenCenter, 0f);
                m_User.LookAt(ray2.GetPoint(1000f));
            }
            if (Input.GetMouseButton(1))
            {
                if (Mathf.Abs(axis) > 0.1f || Mathf.Abs(axis2) > 0.1f)
                {
                    m_GameScene.AssistAim_Stop();
                }
                else if (m_User.IsFire() && !m_GameScene.IsAssistAim())
                {
                    m_GameScene.AssistAim_Start();
                }
            }
            else
            {
                if (!m_User.IsFire() && m_GameScene.IsAssistAim())
                {
                    m_GameScene.AssistAim_Stop();
                }
            }
        }

        if ((Input.GetKeyDown(KeyCode.Mouse2) || Input.GetKeyDown(KeyCode.LeftControl))
            && m_User.IsCanAttack() && !m_User.IsSkillCD())
        {
            m_User.UseSkill(m_User.SkillID);
        }

        if (!CanSwitchWeapon()) return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            CUISound.GetInstance().Play("UI_Weapon_change");

            int curWeaponIndex = m_User.CurWeaponIndex;
            int num = curWeaponIndex - 1;
            while (num != curWeaponIndex && m_GameState.GetWeapon(num) == null)
            {
                num--;
                if (num < 0)
                {
                    num = 2;
                }
            }
            m_User.SwitchWeapon(num);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {

            CUISound.GetInstance().Play("UI_Weapon_change");

            int curWeaponIndex2 = m_User.CurWeaponIndex;
            int num2 = curWeaponIndex2 + 1;
            while (num2 != curWeaponIndex2 && m_GameState.GetWeapon(num2) == null)
            {
                num2++;
                if (num2 >= 3)
                {
                    num2 = 0;
                }
            }
            m_User.SwitchWeapon(num2);
        }
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            Debug.Log("press 9 key");
            m_GameScene.GameOver(true);
        }
#endif
    }

    private bool CanSwitchWeapon()
    {
        return !(m_User == null || m_User.isDead);
    }

    public override void LateUpdate(float deltaTime)
    {
    }
}
