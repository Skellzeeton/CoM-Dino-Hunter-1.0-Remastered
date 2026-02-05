using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif
#if UNITY_IOS
using Unity.Notifications.iOS;
#endif

public class iLocalNotification : MonoBehaviour
{
    public class CLocalNotifyInfo
    {
        public string alertBody;
        public float time;
    }

    protected List<CLocalNotifyInfo> m_ltLocalNotifyInfo;

    private void Awake()
    {
        m_ltLocalNotifyInfo = new List<CLocalNotifyInfo>();

#if UNITY_ANDROID
        // Create a default notification channel for Android
        var channel = new AndroidNotificationChannel()
        {
            Id = "default_channel",
            Name = "Default Channel",
            Importance = Importance.Default,
            Description = "Generic notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
#endif
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused) Register();
        else UnRegister();
    }

    private void OnApplicationQuit()
    {
        Register();
    }

    public void Register()
    {
        foreach (var item in m_ltLocalNotifyInfo)
        {
#if UNITY_IOS
            var notif = new iOSNotification
            {
                Identifier = Guid.NewGuid().ToString(),
                Title = "Notification",
                Body = item.alertBody,
                ShowInForeground = true,
                ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
                Trigger = new iOSNotificationTimeIntervalTrigger
                {
                    TimeInterval = TimeSpan.FromSeconds(item.time),
                    Repeats = false
                }
            };
            iOSNotificationCenter.ScheduleNotification(notif);
#elif UNITY_ANDROID
            var notif = new AndroidNotification
            {
                Title = "Notification",
                Text = item.alertBody,
                SmallIcon = "default", // your icon name in Plugins/Android/res/drawable
                FireTime = DateTime.Now.AddSeconds(item.time)
            };
            AndroidNotificationCenter.SendNotification(notif, "default_channel");
#endif

            Debug.Log("Scheduled notification: " + item.alertBody + " in " + item.time + " seconds.");
        }
    }

    public void UnRegister()
    {
#if UNITY_IOS
        iOSNotificationCenter.RemoveAllScheduledNotifications();
#elif UNITY_ANDROID
        AndroidNotificationCenter.CancelAllNotifications();
#endif
    }

    public void Clear()
    {
        m_ltLocalNotifyInfo.Clear();
    }

    public void Add(string alertBody, float time)
    {
        m_ltLocalNotifyInfo.Add(new CLocalNotifyInfo { alertBody = alertBody, time = time });
    }
}
