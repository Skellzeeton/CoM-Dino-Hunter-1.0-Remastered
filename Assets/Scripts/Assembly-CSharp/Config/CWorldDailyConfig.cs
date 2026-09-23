using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class CWorldDailyConfig
{
    public class CWorldMonsterEntry
    {
        public int nMobID;
        public float fRate;
        public int nDailyMax;
        public int nSpawnEffectID;
        public List<int> ltTaskTypeLimit;

        public CWorldMonsterEntry()
        {
            ltTaskTypeLimit = new List<int>();
            nSpawnEffectID = -1;
        }
    }

    protected static CWorldDailyConfig m_Instance;
    protected List<CWorldMonsterEntry> m_ltEntries;
    protected bool m_bLoaded;

    public static CWorldDailyConfig GetInstance()
    {
        if (m_Instance == null)
        {
            m_Instance = new CWorldDailyConfig();
            m_Instance.m_ltEntries = new List<CWorldMonsterEntry>();
        }
        return m_Instance;
    }

    public List<CWorldMonsterEntry> GetEntries()
    {
        if (!m_bLoaded) Load();
        return m_ltEntries;
    }

    public CWorldMonsterEntry FindEntry(int nMobID)
    {
        if (!m_bLoaded) Load();
        for (int i = 0; i < m_ltEntries.Count; i++)
            if (m_ltEntries[i].nMobID == nMobID) return m_ltEntries[i];
        return null;
    }

    public bool Load()
    {
        m_bLoaded = true;
        m_ltEntries.Clear();

        string content = string.Empty;
        if (MyUtils.isWindows)
        {
            if (!Utils.FileGetString("dailymob.xml", ref content))
            {
                Debug.LogWarning("[CWorldDailyConfig] dailymob.xml not found.");
                return false;
            }
        }
        else if (MyUtils.isIOS || MyUtils.isAndroid)
        {
            TextAsset textAsset = (TextAsset)Resources.Load("_config/dailymob", typeof(TextAsset));
            if (textAsset == null)
            {
                Debug.LogWarning("[CWorldDailyConfig] dailymob not found in Resources.");
                return false;
            }
            content = textAsset.ToString();
        }
        try
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(content);
            XmlNode root = doc.DocumentElement;
            if (root == null) return false;
            foreach (XmlNode section in root.ChildNodes)
            {
                if (section.Name != "worldmonster") continue;
                foreach (XmlNode node in section.ChildNodes)
                {
                    if (node.Name != "node") continue;
                    CWorldMonsterEntry entry = new CWorldMonsterEntry();
                    string val = string.Empty;
                    if (!GetAttr(node, "mobid", ref val)) continue;
                    entry.nMobID = int.Parse(val);
                    if (GetAttr(node, "rate", ref val))
                        entry.fRate = float.Parse(val, System.Globalization.CultureInfo.InvariantCulture);
                    if (GetAttr(node, "dailymax", ref val))
                        entry.nDailyMax = int.Parse(val);
                    if (GetAttr(node, "spawneffect", ref val))
                        entry.nSpawnEffectID = int.Parse(val);
                    if (GetAttr(node, "scenetypelimit", ref val))
                    {
                        foreach (string t in val.Split(','))
                        {
                            string tt = t.Trim();
                            if (tt.Length > 0) entry.ltTaskTypeLimit.Add(int.Parse(tt));
                        }
                    }
                    m_ltEntries.Add(entry);
                }
            }
            Debug.Log("[CWorldDailyConfig] Loaded " + m_ltEntries.Count + " entries.");
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError("[CWorldDailyConfig] Parse failed: " + ex.Message);
            return false;
        }
    }

    protected static bool GetAttr(XmlNode node, string name, ref string value)
    {
        if (node == null || node.Attributes[name] == null) return false;
        value = node.Attributes[name].Value.Trim();
        return value.Length > 0;
    }
}