using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace gyAchievementSystem
{
    public class CAchievementCenter
    {
        protected Dictionary<int, CAchievementInfo> m_dictAchievementInfo;
        protected Dictionary<int, CAchievementData> m_dictAchievementData;

        public CAchievementCenter()
        {
            m_dictAchievementInfo = new Dictionary<int, CAchievementInfo>();
            m_dictAchievementData = new Dictionary<int, CAchievementData>();
        }

        public Dictionary<int, CAchievementInfo> GetDataInfo()
        {
            return m_dictAchievementInfo;
        }

        public Dictionary<int, CAchievementData> GetDataData()
        {
            return m_dictAchievementData;
        }

        public CAchievementInfo GetInfo(int nID)
        {
            if (!m_dictAchievementInfo.ContainsKey(nID))
                return null;
            return m_dictAchievementInfo[nID];
        }

        public CAchievementData GetData(int nID)
        {
            if (!m_dictAchievementData.ContainsKey(nID))
                return null;
            return m_dictAchievementData[nID];
        }

        public void AddData(int nID, CAchievementData data)
        {
            if (!m_dictAchievementData.ContainsKey(nID))
            {
                m_dictAchievementData.Add(nID, data);
            }
        }

        public bool LoadInfo()
        {
            string empty = string.Empty;
            TextAsset textAsset = (TextAsset)Resources.Load("_Config/achievement", typeof(TextAsset));
            if (textAsset == null)
                return false;

            empty = textAsset.ToString();
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(empty);

            string value = string.Empty;
            XmlNode documentElement = xmlDocument.DocumentElement;
            foreach (XmlNode childNode in documentElement.ChildNodes)
            {
                if (childNode.Name != "achievement" || !GetAttribute(childNode, "id", ref value))
                    continue;

                CAchievementInfo cAchievementInfo = new CAchievementInfo();
                cAchievementInfo.nID = int.Parse(value);

                if (GetAttribute(childNode, "type", ref value))
                    cAchievementInfo.nType = int.Parse(value);

                if (GetAttribute(childNode, "param", ref value))
                {
                    cAchievementInfo.ltParam.Clear();
                    string[] array = value.Split(',');
                    for (int i = 0; i < array.Length; i++)
                        cAchievementInfo.ltParam.Add(int.Parse(array[i]));
                }

                if (GetAttribute(childNode, "key", ref value))
                    cAchievementInfo.sKey = value.Trim();

                if (GetAttribute(childNode, "name", ref value))
                    cAchievementInfo.sName = value.Trim();

                if (GetAttribute(childNode, "desc", ref value))
                    cAchievementInfo.sDesc = value.Trim();

                cAchievementInfo.ltStep.Clear();
                for (int j = 1; j <= 3; j++)
                {
                    if (GetAttribute(childNode, "step" + j, ref value))
                    {
                        string[] array = value.Split(',');
                        if (array.Length == 3)
                        {
                            CAchievementStep cAchievementStep = new CAchievementStep();
                            cAchievementStep.nStepPurpose = int.Parse(array[0]);
                            cAchievementStep.nRewardType = int.Parse(array[1]);
                            cAchievementStep.nRewardNumber = int.Parse(array[2]);
                            cAchievementInfo.ltStep.Add(cAchievementStep);
                        }
                    }
                }

                m_dictAchievementInfo.Add(cAchievementInfo.nID, cAchievementInfo);
            }
            return true;
        }

        public bool LoadData(XmlNode achievementRoot)
        {
            m_dictAchievementData.Clear();

            if (achievementRoot == null)
                return false;

            string value = string.Empty;
            foreach (XmlNode childNode in achievementRoot.ChildNodes)
            {
                if (childNode.Name != "achievement" || !GetAttribute(childNode, "id", ref value))
                    continue;

                CAchievementData cAchievementData = new CAchievementData();
                cAchievementData.nID = int.Parse(value);

                if (GetAttribute(childNode, "state", ref value))
                    cAchievementData.nState = int.Parse(value);

                if (GetAttribute(childNode, "value", ref value))
                    cAchievementData.nCurValue = int.Parse(value);

                if (GetAttribute(childNode, "isgotreward", ref value))
                {
                    string[] array = value.Split(',');
                    for (int i = 0; i < array.Length && i < 3; i++)
                        cAchievementData.SetGotReward(i, bool.Parse(array[i]));
                }

                m_dictAchievementData.Add(cAchievementData.nID, cAchievementData);
            }

            return true;
        }

        public void SaveData(XmlDocument xmlDocument, XmlElement parentElement)
        {
            if (xmlDocument == null || parentElement == null)
                return;

            XmlElement achievementRoot = xmlDocument.CreateElement("achievementdata");
            parentElement.AppendChild(achievementRoot);

            foreach (CAchievementData value in m_dictAchievementData.Values)
            {
                XmlElement xmlElement2 = xmlDocument.CreateElement("achievement");
                xmlElement2.SetAttribute("id", value.nID.ToString());
                xmlElement2.SetAttribute("state", value.nState.ToString());
                xmlElement2.SetAttribute("value", value.nCurValue.ToString());

                string text = string.Empty;
                for (int i = 0; i < 3; i++)
                {
                    text = ((i != 0) ? (text + "," + value.IsGotReward(i)) : value.IsGotReward(i).ToString());
                }

                xmlElement2.SetAttribute("isgotreward", text);
                achievementRoot.AppendChild(xmlElement2);
            }
        }

        protected bool GetAttribute(XmlNode node, string name, ref string value)
        {
            if (node == null || node.Attributes[name] == null)
                return false;

            value = node.Attributes[name].Value.Trim();
            if (value.Length < 1)
                return false;

            return true;
        }
    }
}