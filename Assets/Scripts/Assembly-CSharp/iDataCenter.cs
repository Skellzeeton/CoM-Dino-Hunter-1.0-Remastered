using GUPS.AntiCheat.Protected;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using UnityEngine;
using gyAchievementSystem;

public class iDataCenter
{
	protected string m_sVersion = "1.0.0";

	private const string SAVE_FILE = "gamedata.xml";
	private const string BACKUP_FILE = "gamedata.xml.bak";
	private const string TEMP_FILE = "gamedata.xml.tmp";

	protected ProtectedInt32 m_nGold;
	protected ProtectedInt32 m_nCrystal;
	protected ProtectedInt32 m_nStashLevel;
	protected int m_nCrystalTotalGain;
	protected int m_nCrystalTotalConsume;
	protected Dictionary<int, ProtectedInt32> m_dictMaterials;
	protected Dictionary<int, int> m_dictWeapon;
	protected Dictionary<int, int> m_dictEquipStone;
	protected Dictionary<int, int> m_dictPassiveSkill;
	protected Dictionary<int, CCharSaveInfo> m_dictCharSaveInfo;
	protected List<CLevelSaveInfo> m_ltLevelSaveInfo;
	protected bool m_bMusic;
	protected bool m_bSound;
	protected bool m_bAmbience;
	protected bool m_bAutoAim;
	protected bool m_bStartCutsceneReplay;
	protected int m_nUnLockSignType;
	protected int m_nUnLockSignID;
	protected int m_nSceneProccess;
	protected bool m_bTutorial;
	protected bool m_bTutorialVillage;
	protected bool m_bEvaluate;
	protected int m_nEnterAppCount;
	protected Dictionary<int, int> m_dictWeaponSign;
	protected Dictionary<int, int> m_dictEquipStoneSign;
	protected Dictionary<int, int> m_dictSkillSign;
	protected Dictionary<int, int> m_dictCharacterSign;
	protected int m_nCurCharID;
	protected int[] m_arrSelectWeapon;
	protected Dictionary<int, int[]> m_dictSelectPassiveSkill;
	protected int m_nCurEquipStone;
	protected ProtectedInt32 m_nLatestLevel;
	protected bool m_bUnLockLevel;
	protected List<int> m_ltLevelList;
	protected bool m_bFirstTimePlay;

	public bool isFirstTimePlay
	{
		get { return m_bFirstTimePlay; }
	}

	public bool isTutorial
	{
		get { return m_bTutorial; }
		set { m_bTutorial = value; }
	}

	public bool isTutorialVillage
	{
		get { return m_bTutorialVillage; }
		set { m_bTutorialVillage = value; }
	}

	public bool MusicSwitch
	{
		get { return m_bMusic; }
		set { m_bMusic = value; }
	}

	public bool SoundSwitch
	{
		get { return m_bSound; }
		set { m_bSound = value; }
	}
	
	public bool AmbienceSwitch
	{
		get { return m_bAmbience; }
		set { m_bAmbience = value; }
	}

	public bool AutoAimSwitch
	{
		get { return m_bAutoAim; }
		set { m_bAutoAim = value; }
	}
	
	public bool StartCutsceneReplay
	{
		get { return m_bStartCutsceneReplay; }
		set { m_bStartCutsceneReplay = value; }
	}

	public ProtectedInt32 Gold
	{
		get { return m_nGold; }
	}

	public ProtectedInt32 Crystal
	{
		get { return m_nCrystal; }
	}

	public int CurCharID
	{
		get { return m_nCurCharID; }
		set { m_nCurCharID = value; }
	}

	public int CurEquipStone
	{
		get { return m_nCurEquipStone; }
		set { m_nCurEquipStone = value; }
	}

	public ProtectedInt32 LatestLevel
	{
		get { return m_nLatestLevel; }
		set { m_nLatestLevel = value; }
	}

	public int SceneProccess
	{
		get { return m_nSceneProccess; }
	}

	public ProtectedInt32 StashLevel
	{
		get { return m_nStashLevel; }
		set { m_nStashLevel = value; }
	}

	public ProtectedInt32 StashCount
	{
		get
		{
			ProtectedInt32 num = 0;
			foreach (ProtectedInt32 value in m_dictMaterials.Values)
			{
				num += value;
			}
			return num;
		}
	}

	public ProtectedInt32 StashCountMax
	{
		get
		{
			iGameData gameData = iGameApp.GetInstance().m_GameData;
			if (gameData == null)
			{
				return 0;
			}
			return gameData.GetStashCapacityCount(m_nStashLevel);
		}
	}

	public ProtectedInt32 HighestCharLevel
	{
		get
		{
			ProtectedInt32 num = 0;
			foreach (CCharSaveInfo value in m_dictCharSaveInfo.Values)
			{
				if (num == 0 || num < value.nLevel)
				{
					num = value.nLevel;
				}
			}
			return num;
		}
	}

	public bool isEvaluate
	{
		get { return m_bEvaluate; }
		set { m_bEvaluate = value; }
	}

	public int EnterAppCount
	{
		get { return m_nEnterAppCount; }
		set { m_nEnterAppCount = value; }
	}

	public int UnLockSignType
	{
		get { return m_nUnLockSignType; }
		set { m_nUnLockSignType = value; }
	}

	public int UnLockSignID
	{
		get { return m_nUnLockSignID; }
		set { m_nUnLockSignID = value; }
	}

	public iDataCenter()
	{
		m_bMusic = true;
		m_bSound = true;
		m_bAmbience = true;
		m_bAutoAim = true;
		m_bStartCutsceneReplay = false;
		m_nGold = new ProtectedInt32();
		m_nCrystal = new ProtectedInt32();
		m_nStashLevel = new ProtectedInt32();
		m_nStashLevel = 1;
		m_dictMaterials = new Dictionary<int, ProtectedInt32>();
		m_dictWeapon = new Dictionary<int, int>();
		m_dictEquipStone = new Dictionary<int, int>();
		m_dictPassiveSkill = new Dictionary<int, int>();
		m_dictCharSaveInfo = new Dictionary<int, CCharSaveInfo>();
		m_dictWeaponSign = new Dictionary<int, int>();
		m_dictEquipStoneSign = new Dictionary<int, int>();
		m_dictSkillSign = new Dictionary<int, int>();
		m_dictCharacterSign = new Dictionary<int, int>();
		m_nCurCharID = 1;
		m_arrSelectWeapon = new int[3] { 2, 1, -1 };
		m_dictSelectPassiveSkill = new Dictionary<int, int[]>();
		m_nCurEquipStone = 0;
		m_ltLevelList = new List<int>();
		for (int i = 1001; i <= 1024; i++)
		{
			m_ltLevelList.Add(i);
		}
		m_nLatestLevel = 1001;
		m_bUnLockLevel = false;
		m_ltLevelSaveInfo = new List<CLevelSaveInfo>();
		m_bFirstTimePlay = false;
		m_bTutorial = false;
		m_bTutorialVillage = false;
		m_bEvaluate = false;
		m_nEnterAppCount = 0;
	}

	private string GetSavePath(string fileName)
	{
		return System.IO.Path.Combine(Application.persistentDataPath, fileName);
	}

	private bool TryReadEncryptedFile(string path, ref string decryptedContent)
	{
		decryptedContent = string.Empty;

		if (!File.Exists(path))
		{
			return false;
		}

		try
		{
			string encrypted = File.ReadAllText(path);
			if (string.IsNullOrEmpty(encrypted))
			{
				return false;
			}

			decryptedContent = XXTEAUtils.Decrypt(encrypted, iGameApp.GetInstance().GetKey());
			return !string.IsNullOrEmpty(decryptedContent);
		}
		catch
		{
			return false;
		}
	}

	private bool TryLoadXmlDocument(string path, out XmlDocument doc)
	{
		doc = null;

		string xmlText = string.Empty;
		if (!TryReadEncryptedFile(path, ref xmlText))
		{
			return false;
		}

		try
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(xmlText);
			doc = xmlDocument;
			return true;
		}
		catch
		{
			doc = null;
			return false;
		}
	}

	private bool HasChild(XmlNode root, string childName)
	{
		if (root == null)
		{
			return false;
		}

		foreach (XmlNode node in root.ChildNodes)
		{
			if (node != null && node.Name == childName)
			{
				return true;
			}
		}
		return false;
	}

	private int CountExpectedSections(XmlNode root)
	{
		if (root == null)
		{
			return 0;
		}

		int count = 0;
		string[] expected =
		{
			"passedlevel",
			"character",
			"weapon",
			"skill",
			"equipstone",
			"materials",
			"unlocksign",
			"achievementdata"
		};

		for (int i = 0; i < expected.Length; i++)
		{
			if (HasChild(root, expected[i]))
			{
				count++;
			}
		}

		return count;
	}

	private bool IsValidGameDataDocument(XmlDocument doc)
	{
		if (doc == null || doc.DocumentElement == null)
		{
			return false;
		}

		XmlNode root = doc.DocumentElement;
		if (root.Name != "gamedata")
		{
			return false;
		}
		int coreCount = 0;
		string[] coreSections =
		{
			"character",
			"weapon",
			"skill",
			"equipstone",
			"materials",
			"unlocksign"
		};

		for (int i = 0; i < coreSections.Length; i++)
		{
			if (HasChild(root, coreSections[i]))
			{
				coreCount++;
			}
		}

		return coreCount >= 4;
	}

	private bool IsSeverelyDifferent(XmlDocument currentDoc, XmlDocument backupDoc)
	{
		if (!IsValidGameDataDocument(currentDoc) || !IsValidGameDataDocument(backupDoc))
		{
			return false;
		}

		XmlNode currentRoot = currentDoc.DocumentElement;
		XmlNode backupRoot = backupDoc.DocumentElement;

		if (currentRoot == null || backupRoot == null)
		{
			return false;
		}

		if (currentRoot.Name != backupRoot.Name)
		{
			return true;
		}
		int currentSections = CountExpectedSections(currentRoot);
		int backupSections = CountExpectedSections(backupRoot);
		int diff = Math.Abs(currentSections - backupSections);
		if (diff >= 4)
		{
			return true;
		}
		if (currentSections < 4 && backupSections >= 4)
		{
			return true;
		}
		return false;
	}

	private void RestoreBackupToCurrent()
	{
		string currentPath = GetSavePath(SAVE_FILE);
		string backupPath = GetSavePath(BACKUP_FILE);

		if (!File.Exists(backupPath))
		{
			return;
		}

		try
		{
			string backupEncrypted = File.ReadAllText(backupPath);
			if (string.IsNullOrEmpty(backupEncrypted))
			{
				return;
			}

			File.WriteAllText(currentPath, backupEncrypted);
		}
		catch
		{
		}
	}

	private void SaveEncryptedAtomic(string xmlText)
	{
		string currentPath = GetSavePath(SAVE_FILE);
		string backupPath = GetSavePath(BACKUP_FILE);
		string tempPath = GetSavePath(TEMP_FILE);

		string encrypted = XXTEAUtils.Encrypt(xmlText, iGameApp.GetInstance().GetKey());

		try
		{
			File.WriteAllText(tempPath, encrypted);
		}
		catch
		{
			return;
		}

		XmlDocument currentDoc;
		bool currentValid = TryLoadXmlDocument(currentPath, out currentDoc) && IsValidGameDataDocument(currentDoc);

		try
		{
			if (currentValid && File.Exists(currentPath))
			{
				File.Replace(tempPath, currentPath, backupPath, true);
			}
			else
			{
				if (File.Exists(currentPath))
				{
					File.Delete(currentPath);
				}
				File.Move(tempPath, currentPath);
			}
		}
		catch
		{
			try
			{
				if (File.Exists(currentPath))
				{
					File.Delete(currentPath);
				}
				File.Move(tempPath, currentPath);
			}
			catch
			{
			}
		}
	}

	public bool Load()
	{
		XmlDocument currentDoc;
		XmlDocument backupDoc;
		bool currentOk = TryLoadXmlDocument(GetSavePath(SAVE_FILE), out currentDoc) && IsValidGameDataDocument(currentDoc);
		bool backupOk = TryLoadXmlDocument(GetSavePath(BACKUP_FILE), out backupDoc) && IsValidGameDataDocument(backupDoc);

		XmlDocument chosenDoc = null;

		if (currentOk && backupOk)
		{
			if (IsSeverelyDifferent(currentDoc, backupDoc))
			{
				chosenDoc = backupDoc;
				RestoreBackupToCurrent();
			}
			else
			{
				chosenDoc = currentDoc;
			}
		}
		else if (currentOk)
		{
			chosenDoc = currentDoc;
		}
		else if (backupOk)
		{
			chosenDoc = backupDoc;
			RestoreBackupToCurrent();
		}
		else
		{
			m_bFirstTimePlay = true;
			SetCharacter(1, 1, 0);
			SetWeaponLevel(1, 1);
			SetWeaponLevel(2, 1);
			Save();
			return false;
		}

		string value = string.Empty;
		string text = string.Empty;
		XmlNode documentElement = chosenDoc.DocumentElement;
		if (MyUtils.GetAttribute(documentElement, "version", ref value))
		{
			text = value;
		}

		if (text == "1.0.0")
		{
			Load_1_0(documentElement);
		}
		else
		{
			Load_1_0(documentElement);
		}

		return true;
	}

	public void Save()
	{
		XmlDocument xmlDocument = new XmlDocument();
		XmlNode newChild = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", "no");
		xmlDocument.AppendChild(newChild);

		string empty = string.Empty;
		XmlElement xmlElement = xmlDocument.CreateElement("gamedata");
		xmlDocument.AppendChild(xmlElement);

		xmlElement.SetAttribute("version", m_sVersion);
		xmlElement.SetAttribute("gold", m_nGold.ToString());
		xmlElement.SetAttribute("crystal", m_nCrystal.ToString());
		xmlElement.SetAttribute("stashlevel", m_nStashLevel.ToString());
		xmlElement.SetAttribute("latestlevel", m_nLatestLevel.ToString());
		xmlElement.SetAttribute("isunlocklevel", m_bUnLockLevel.ToString());
		xmlElement.SetAttribute("proccess", m_nSceneProccess.ToString());
		xmlElement.SetAttribute("crystaltotalgain", m_nCrystalTotalGain.ToString());
		xmlElement.SetAttribute("crystaltotalconsume", m_nCrystalTotalConsume.ToString());
		xmlElement.SetAttribute("isMusic", m_bMusic.ToString());
		xmlElement.SetAttribute("isSound", m_bSound.ToString());
		xmlElement.SetAttribute("isAmbience", m_bAmbience.ToString());
		xmlElement.SetAttribute("isStartCutsceneReplay", m_bStartCutsceneReplay.ToString());
		xmlElement.SetAttribute("isTutorial", m_bTutorial.ToString());
		xmlElement.SetAttribute("isTutorialVillage", m_bTutorialVillage.ToString());
		xmlElement.SetAttribute("isEvaluate", m_bEvaluate.ToString());
		xmlElement.SetAttribute("enterappcount", m_nEnterAppCount.ToString());

		XmlElement xmlElement2 = xmlDocument.CreateElement("passedlevel");
		xmlElement.AppendChild(xmlElement2);
		foreach (CLevelSaveInfo item in m_ltLevelSaveInfo)
		{
			XmlElement xmlElement3 = xmlDocument.CreateElement("node");
			xmlElement2.AppendChild(xmlElement3);
			xmlElement3.SetAttribute("id", item.nID.ToString());
			xmlElement3.SetAttribute("isignorecg", item.isIgnoreCG.ToString());
		}

		XmlElement xmlElement4 = xmlDocument.CreateElement("character");
		xmlElement.AppendChild(xmlElement4);
		xmlElement4.SetAttribute("select", m_nCurCharID.ToString());
		foreach (CCharSaveInfo value2 in m_dictCharSaveInfo.Values)
		{
			XmlElement xmlElement5 = xmlDocument.CreateElement("node");
			xmlElement4.AppendChild(xmlElement5);
			xmlElement5.SetAttribute("id", value2.nID.ToString());
			xmlElement5.SetAttribute("level", value2.nLevel.ToString());
			xmlElement5.SetAttribute("exp", value2.nExp.ToString());
		}

		XmlElement xmlElement6 = xmlDocument.CreateElement("weapon");
		xmlElement.AppendChild(xmlElement6);
		empty = string.Empty;
		int[] arrSelectWeapon = m_arrSelectWeapon;
		for (int i = 0; i < arrSelectWeapon.Length; i++)
		{
			int num = arrSelectWeapon[i];
			empty = ((empty.Length >= 1) ? (empty + "," + num) : num.ToString());
		}
		xmlElement6.SetAttribute("select", empty);
		foreach (KeyValuePair<int, int> item2 in m_dictWeapon)
		{
			XmlElement xmlElement7 = xmlDocument.CreateElement("node");
			xmlElement6.AppendChild(xmlElement7);
			xmlElement7.SetAttribute("id", item2.Key.ToString());
			xmlElement7.SetAttribute("level", item2.Value.ToString());
		}

		XmlElement xmlElement8 = xmlDocument.CreateElement("skill");
		xmlElement.AppendChild(xmlElement8);
		foreach (KeyValuePair<int, int[]> item3 in m_dictSelectPassiveSkill)
		{
			XmlElement xmlElement9 = xmlDocument.CreateElement("selectnode");
			xmlElement8.AppendChild(xmlElement9);
			xmlElement9.SetAttribute("charid", item3.Key.ToString());
			empty = string.Empty;
			int[] value = item3.Value;
			for (int j = 0; j < value.Length; j++)
			{
				int num2 = value[j];
				empty = ((empty.Length >= 1) ? (empty + "," + num2) : num2.ToString());
			}
			xmlElement9.SetAttribute("select", empty);
		}
		foreach (KeyValuePair<int, int> item4 in m_dictPassiveSkill)
		{
			XmlElement xmlElement10 = xmlDocument.CreateElement("node");
			xmlElement8.AppendChild(xmlElement10);
			xmlElement10.SetAttribute("id", item4.Key.ToString());
			xmlElement10.SetAttribute("level", item4.Value.ToString());
		}

		XmlElement xmlElement11 = xmlDocument.CreateElement("equipstone");
		xmlElement.AppendChild(xmlElement11);
		xmlElement11.SetAttribute("select", m_nCurEquipStone.ToString());
		foreach (KeyValuePair<int, int> item5 in m_dictEquipStone)
		{
			XmlElement xmlElement12 = xmlDocument.CreateElement("node");
			xmlElement11.AppendChild(xmlElement12);
			xmlElement12.SetAttribute("id", item5.Key.ToString());
			xmlElement12.SetAttribute("level", item5.Value.ToString());
		}

		XmlElement xmlElement13 = xmlDocument.CreateElement("materials");
		xmlElement.AppendChild(xmlElement13);
		foreach (KeyValuePair<int, ProtectedInt32> dictMaterial in m_dictMaterials)
		{
			if (dictMaterial.Value != 0)
			{
				XmlElement xmlElement14 = xmlDocument.CreateElement("node");
				xmlElement13.AppendChild(xmlElement14);
				xmlElement14.SetAttribute("id", dictMaterial.Key.ToString());
				xmlElement14.SetAttribute("count", dictMaterial.Value.ToString());
			}
		}

		XmlElement xmlElement15 = xmlDocument.CreateElement("unlocksign");
		xmlElement.AppendChild(xmlElement15);
		xmlElement15.SetAttribute("unlocksigntype", m_nUnLockSignType.ToString());
		xmlElement15.SetAttribute("unlocksignid", m_nUnLockSignID.ToString());
		foreach (KeyValuePair<int, int> item6 in m_dictWeaponSign)
		{
			XmlElement xmlElement16 = xmlDocument.CreateElement("weaponsign");
			xmlElement15.AppendChild(xmlElement16);
			xmlElement16.SetAttribute("id", item6.Key.ToString());
			xmlElement16.SetAttribute("sign", item6.Value.ToString());
		}
		foreach (KeyValuePair<int, int> item7 in m_dictEquipStoneSign)
		{
			XmlElement xmlElement17 = xmlDocument.CreateElement("equipstonesign");
			xmlElement15.AppendChild(xmlElement17);
			xmlElement17.SetAttribute("id", item7.Key.ToString());
			xmlElement17.SetAttribute("sign", item7.Value.ToString());
		}
		foreach (KeyValuePair<int, int> item8 in m_dictSkillSign)
		{
			XmlElement xmlElement18 = xmlDocument.CreateElement("skillsign");
			xmlElement15.AppendChild(xmlElement18);
			xmlElement18.SetAttribute("id", item8.Key.ToString());
			xmlElement18.SetAttribute("sign", item8.Value.ToString());
		}
		foreach (KeyValuePair<int, int> item9 in m_dictCharacterSign)
		{
			XmlElement xmlElement19 = xmlDocument.CreateElement("charactersign");
			xmlElement15.AppendChild(xmlElement19);
			xmlElement19.SetAttribute("id", item9.Key.ToString());
			xmlElement19.SetAttribute("sign", item9.Value.ToString());
		}
		CAchievementCenter achievementCenter = CAchievementManager.GetInstance().GetAchievementCenter();
		if (achievementCenter != null)
		{
			achievementCenter.SaveData(xmlDocument, xmlElement);
		}

		StringWriter stringWriter = new StringWriter();
		xmlDocument.Save(stringWriter);
		SaveEncryptedAtomic(stringWriter.ToString());
	}

	protected void Load_1_0(XmlNode root)
	{
		string value = string.Empty;

		if (MyUtils.GetAttribute(root, "gold", ref value))
		{
			m_nGold = int.Parse(value);
		}
		if (MyUtils.GetAttribute(root, "crystal", ref value))
		{
			m_nCrystal = int.Parse(value);
		}
		if (MyUtils.GetAttribute(root, "stashlevel", ref value))
		{
			m_nStashLevel = int.Parse(value);
		}
		if (MyUtils.GetAttribute(root, "latestlevel", ref value))
		{
			m_nLatestLevel = int.Parse(value);
		}
		if (MyUtils.GetAttribute(root, "isunlocklevel", ref value))
		{
			m_bUnLockLevel = bool.Parse(value);
		}
		if (MyUtils.GetAttribute(root, "proccess", ref value))
		{
			m_nSceneProccess = int.Parse(value);
		}
		if (MyUtils.GetAttribute(root, "crystaltotalgain", ref value))
		{
			m_nCrystalTotalGain = int.Parse(value);
		}
		if (MyUtils.GetAttribute(root, "crystaltotalconsume", ref value))
		{
			m_nCrystalTotalConsume = int.Parse(value);
		}
		if (MyUtils.GetAttribute(root, "isMusic", ref value))
		{
			m_bMusic = bool.Parse(value);
		}
		if (MyUtils.GetAttribute(root, "isSound", ref value))
		{
			m_bSound = bool.Parse(value);
		}
		if (MyUtils.GetAttribute(root, "isAmbience", ref value))
		{
			m_bAmbience = bool.Parse(value);
		}
		if (MyUtils.GetAttribute(root, "isStartCutsceneReplay", ref value))
		{
			m_bStartCutsceneReplay = bool.Parse(value);
		}
		if (MyUtils.GetAttribute(root, "isTutorial", ref value))
		{
			m_bTutorial = bool.Parse(value);
		}
		if (MyUtils.GetAttribute(root, "isTutorialVillage", ref value))
		{
			m_bTutorialVillage = bool.Parse(value);
		}
		if (MyUtils.GetAttribute(root, "isEvaluate", ref value))
		{
			m_bEvaluate = bool.Parse(value);
		}
		if (MyUtils.GetAttribute(root, "enterappcount", ref value))
		{
			m_nEnterAppCount = int.Parse(value);
		}

		foreach (XmlNode item in root)
		{
			if (item.Name == "passedlevel")
			{
				foreach (XmlNode item2 in item)
				{
					CLevelSaveInfo cLevelSaveInfo = new CLevelSaveInfo();
					if (MyUtils.GetAttribute(item2, "id", ref value))
					{
						cLevelSaveInfo.nID = int.Parse(value);
					}
					if (MyUtils.GetAttribute(item2, "isignorecg", ref value))
					{
						cLevelSaveInfo.isIgnoreCG = bool.Parse(value);
					}
					m_ltLevelSaveInfo.Add(cLevelSaveInfo);
				}
			}
			else if (item.Name == "achievementdata")
			{
				CAchievementCenter achievementCenter = CAchievementManager.GetInstance().GetAchievementCenter();
				if (achievementCenter != null)
				{
					achievementCenter.LoadData(item);
				}
			}
			else if (item.Name == "character")
			{
				if (MyUtils.GetAttribute(item, "select", ref value))
				{
					m_nCurCharID = int.Parse(value);
				}
				foreach (XmlNode item3 in item)
				{
					if (!(item3.Name != "node") && MyUtils.GetAttribute(item3, "id", ref value))
					{
						int nCharID = int.Parse(value);
						int nLevel = 1;
						int nExp = 0;
						if (MyUtils.GetAttribute(item3, "level", ref value))
						{
							nLevel = int.Parse(value);
						}
						if (MyUtils.GetAttribute(item3, "exp", ref value))
						{
							nExp = int.Parse(value);
						}
						SetCharacter(nCharID, nLevel, nExp);
					}
				}
			}
			else if (item.Name == "weapon")
			{
				if (MyUtils.GetAttribute(item, "select", ref value))
				{
					string[] array = value.Split(',');
					for (int i = 0; i < array.Length && i < m_arrSelectWeapon.Length; i++)
					{
						m_arrSelectWeapon[i] = int.Parse(array[i]);
					}
				}
				foreach (XmlNode item4 in item)
				{
					if (!(item4.Name != "node") && MyUtils.GetAttribute(item4, "id", ref value))
					{
						int nWeaponID = int.Parse(value);
						int nWeaponLevel = 0;
						if (MyUtils.GetAttribute(item4, "level", ref value))
						{
							nWeaponLevel = int.Parse(value);
						}
						SetWeaponLevel(nWeaponID, nWeaponLevel);
					}
				}
			}
			else if (item.Name == "skill")
			{
				foreach (XmlNode item5 in item)
				{
					if (item5.Name == "selectnode")
					{
						if (!MyUtils.GetAttribute(item5, "charid", ref value))
						{
							continue;
						}
						int nCharID2 = int.Parse(value);
						if (MyUtils.GetAttribute(item5, "select", ref value))
						{
							string[] array = value.Split(',');
							for (int j = 0; j < array.Length; j++)
							{
								SetSelectPassiveSkill(nCharID2, j, int.Parse(array[j]));
							}
						}
					}
					else if (item5.Name == "node" && MyUtils.GetAttribute(item5, "id", ref value))
					{
						int nSkillID = int.Parse(value);
						int nLevel2 = 0;
						if (MyUtils.GetAttribute(item5, "level", ref value))
						{
							nLevel2 = int.Parse(value);
						}
						SetPassiveSkill(nSkillID, nLevel2);
					}
				}
			}
			else if (item.Name == "equipstone")
			{
				if (MyUtils.GetAttribute(item, "select", ref value))
				{
					m_nCurEquipStone = int.Parse(value);
				}
				foreach (XmlNode item6 in item)
				{
					if (!(item6.Name != "node") && MyUtils.GetAttribute(item6, "id", ref value))
					{
						int nItemID = int.Parse(value);
						int nLevel3 = 0;
						if (MyUtils.GetAttribute(item6, "level", ref value))
						{
							nLevel3 = int.Parse(value);
						}
						SetEquipStone(nItemID, nLevel3);
					}
				}
			}
			else if (item.Name == "materials")
			{
				foreach (XmlNode item7 in item)
				{
					if (!(item7.Name != "node") && MyUtils.GetAttribute(item7, "id", ref value))
					{
						int nItemID2 = int.Parse(value);
						int nCount = 0;
						if (MyUtils.GetAttribute(item7, "count", ref value))
						{
							nCount = int.Parse(value);
						}
						SetMaterialNum(nItemID2, nCount);
					}
				}
			}
			else
			{
				if (!(item.Name == "unlocksign"))
				{
					continue;
				}
				if (MyUtils.GetAttribute(item, "unlocksigntype", ref value))
				{
					m_nUnLockSignType = int.Parse(value);
				}
				if (MyUtils.GetAttribute(item, "unlocksignid", ref value))
				{
					m_nUnLockSignID = int.Parse(value);
				}
				foreach (XmlNode item8 in item)
				{
					if (item8.Name == "weaponsign")
					{
						int num = 0;
						int value2 = 0;
						if (MyUtils.GetAttribute(item8, "id", ref value))
						{
							num = int.Parse(value);
						}
						if (MyUtils.GetAttribute(item8, "sign", ref value))
						{
							value2 = int.Parse(value);
						}
						if (num > 0 && !m_dictWeaponSign.ContainsKey(num))
						{
							m_dictWeaponSign.Add(num, value2);
						}
					}
					else if (item8.Name == "equipstonesign")
					{
						int num2 = 0;
						int value3 = 0;
						if (MyUtils.GetAttribute(item8, "id", ref value))
						{
							num2 = int.Parse(value);
						}
						if (MyUtils.GetAttribute(item8, "sign", ref value))
						{
							value3 = int.Parse(value);
						}
						if (num2 > 0 && !m_dictEquipStoneSign.ContainsKey(num2))
						{
							m_dictEquipStoneSign.Add(num2, value3);
						}
					}
					else if (item8.Name == "skillsign")
					{
						int num3 = 0;
						int value4 = 0;
						if (MyUtils.GetAttribute(item8, "id", ref value))
						{
							num3 = int.Parse(value);
						}
						if (MyUtils.GetAttribute(item8, "sign", ref value))
						{
							value4 = int.Parse(value);
						}
						if (num3 > 0 && !m_dictWeaponSign.ContainsKey(num3))
						{
							m_dictSkillSign.Add(num3, value4);
						}
					}
					else if (item8.Name == "charactersign")
					{
						int num4 = 0;
						int value5 = 0;
						if (MyUtils.GetAttribute(item8, "id", ref value))
						{
							num4 = int.Parse(value);
						}
						if (MyUtils.GetAttribute(item8, "sign", ref value))
						{
							value5 = int.Parse(value);
						}
						if (num4 > 0 && !m_dictCharacterSign.ContainsKey(num4))
						{
							m_dictCharacterSign.Add(num4, value5);
						}
					}
				}
			}
		}
	}
	
	public List<int> GetLevelList()
	{
		return m_ltLevelList;
	}

	public List<CLevelSaveInfo> GetLevelSaveInfoData()
	{
		return m_ltLevelSaveInfo;
	}

	public Dictionary<int, ProtectedInt32> GetMaterialData()
	{
		return m_dictMaterials;
	}

	public Dictionary<int, int> GetWeaponData()
	{
		return m_dictWeapon;
	}

	public CCharSaveInfo GetCharacter(int nCharID)
	{
		if (!m_dictCharSaveInfo.ContainsKey(nCharID))
		{
			return null;
		}
		return m_dictCharSaveInfo[nCharID];
	}

	public bool GetPassiveSkill(int nSkillID, ref int nSkillLevel)
	{
		if (!m_dictPassiveSkill.ContainsKey(nSkillID))
		{
			return false;
		}
		nSkillLevel = m_dictPassiveSkill[nSkillID];
		return true;
	}

	public bool GetEquipStone(int nItemID, ref int nItemLevel)
	{
		if (!m_dictEquipStone.ContainsKey(nItemID))
		{
			return false;
		}
		nItemLevel = m_dictEquipStone[nItemID];
		return true;
	}

	public int GetWeaponLevel(int nWeaponID)
	{
		if (!m_dictWeapon.ContainsKey(nWeaponID))
		{
			return -1;
		}
		return m_dictWeapon[nWeaponID];
	}

	public int GetSelectWeapon(int nIndex)
	{
		if (nIndex < 0 || nIndex >= m_arrSelectWeapon.Length)
		{
			return -1;
		}
		return m_arrSelectWeapon[nIndex];
	}

	public bool HasSelectWeapon(int nWeaponID)
	{
		for (int i = 0; i < m_arrSelectWeapon.Length; i++)
		{
			if (m_arrSelectWeapon[i] != -1 && m_arrSelectWeapon[i] == nWeaponID)
			{
				return true;
			}
		}
		return false;
	}

	public int GetSelectPassiveSkill(int nCharID, int nIndex)
	{
		if (!m_dictSelectPassiveSkill.ContainsKey(nCharID))
		{
			return -1;
		}
		int[] array = m_dictSelectPassiveSkill[nCharID];
		if (nIndex < 0 || nIndex >= array.Length)
		{
			return -1;
		}
		return array[nIndex];
	}

	public bool HasSelectPassiveSkill(int nCharID, int nSkillID)
	{
		if (!m_dictSelectPassiveSkill.ContainsKey(nCharID))
		{
			return false;
		}
		int[] array = m_dictSelectPassiveSkill[nCharID];
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != -1 && array[i] == nSkillID)
			{
				return true;
			}
		}
		return false;
	}

	public Dictionary<int, int> GetWeaponSignData()
	{
		return m_dictWeaponSign;
	}

	public Dictionary<int, int> GetSkillSignData()
	{
		return m_dictSkillSign;
	}

	public Dictionary<int, int> GetEquipStoneSignData()
	{
		return m_dictEquipStoneSign;
	}

	public Dictionary<int, int> GetCharacterSignData()
	{
		return m_dictCharacterSign;
	}

	public int GetMaterialNum(int nItemID)
	{
		if (!m_dictMaterials.ContainsKey(nItemID))
		{
			return -1;
		}
		return m_dictMaterials[nItemID];
	}

	public void AddMaterialNum(int nItemID, int nCount)
	{
		if (nItemID != -1)
		{
			if (!m_dictMaterials.ContainsKey(nItemID))
			{
				m_dictMaterials.Add(nItemID, nCount);
				return;
			}
			Dictionary<int, ProtectedInt32> dictMaterials;
			Dictionary<int, ProtectedInt32> dictionary = (dictMaterials = m_dictMaterials);
			int key;
			int key2 = (key = nItemID);
			key = dictMaterials[key];
			dictionary[key2] = key + nCount;
		}
	}

	public void SetMaterialNum(int nItemID, int nCount)
	{
		if (nItemID != -1)
		{
			if (!m_dictMaterials.ContainsKey(nItemID))
			{
				m_dictMaterials.Add(nItemID, nCount);
			}
			else
			{
				m_dictMaterials[nItemID] = nCount;
			}
		}
	}

	public int CheckStashVolume(int nCount)
	{
		int stashCountMax = StashCountMax;
		int stashCount = StashCount;
		if (stashCount + nCount > stashCountMax)
		{
			return stashCountMax - stashCount;
		}
		return nCount;
	}

	public void SetWeaponLevel(int nWeaponID, ProtectedInt32 nWeaponLevel)
	{
		if (!m_dictWeapon.ContainsKey(nWeaponID))
		{
			m_dictWeapon.Add(nWeaponID, nWeaponLevel);
		}
		else
		{
			m_dictWeapon[nWeaponID] = nWeaponLevel;
		}
	}

	public void SetCharacter(int nCharID, ProtectedInt32 nLevel, ProtectedInt32 nExp)
	{
		if (!m_dictCharSaveInfo.ContainsKey(nCharID))
		{
			m_dictCharSaveInfo.Add(nCharID, new CCharSaveInfo(nCharID));
		}
		m_dictCharSaveInfo[nCharID].nLevel = nLevel;
		m_dictCharSaveInfo[nCharID].nExp = nExp;
	}

	public void UnlockCharacter(int nCharID)
	{
		if (!m_dictCharSaveInfo.ContainsKey(nCharID))
		{
			m_dictCharSaveInfo.Add(nCharID, new CCharSaveInfo(nCharID));
			m_dictCharSaveInfo[nCharID].nLevel = -1;
			m_dictCharSaveInfo[nCharID].nExp = 0;
		}
	}

	public void SetPassiveSkill(int nSkillID, ProtectedInt32 nLevel)
	{
		if (!m_dictPassiveSkill.ContainsKey(nSkillID))
		{
			m_dictPassiveSkill.Add(nSkillID, nLevel);
		}
		m_dictPassiveSkill[nSkillID] = nLevel;
	}

	public void UnlockPassiveSkill(int nSkillID)
	{
		if (!m_dictPassiveSkill.ContainsKey(nSkillID))
		{
			m_dictPassiveSkill.Add(nSkillID, -1);
		}
	}

	public void SetEquipStone(int nItemID, int nLevel)
	{
		if (!m_dictEquipStone.ContainsKey(nItemID))
		{
			m_dictEquipStone.Add(nItemID, nLevel);
		}
		else
		{
			m_dictEquipStone[nItemID] = nLevel;
		}
	}

	public void UnlockEquipStone(int nItemID)
	{
		if (!m_dictEquipStone.ContainsKey(nItemID))
		{
			m_dictEquipStone.Add(nItemID, -1);
		}
	}

	public void AddGold(ProtectedInt32 nGold)
	{
        ProtectedInt32 num = m_nGold;
		num += nGold;
		if (num < 0)
		{
			num = 0;
		}
		m_nGold = num;
	}

	public void AddCrystal(ProtectedInt32 nCrystal)
	{
        ProtectedInt32 num = m_nCrystal;
        ProtectedInt32 num2 = num + nCrystal;
		if (num2 < 0)
		{
			num2 = 0;
		}
		m_nCrystal = num2;
		if (nCrystal > 0)
		{
			m_nCrystalTotalGain += nCrystal;
		}
		if (nCrystal < 0)
		{
			if (num2 == 0)
			{
				m_nCrystalTotalConsume += num;
			}
			else
			{
				m_nCrystalTotalConsume += nCrystal;
			}
		}
	}

	public void SetSelectWeapon(int nIndex, int nWeaponID)
	{
		if (nIndex >= 0 && nIndex < m_arrSelectWeapon.Length)
		{
			m_arrSelectWeapon[nIndex] = nWeaponID;
		}
	}

	public void SetSelectPassiveSkill(int nCharID, int nIndex, int nPassiveSkillID)
	{
		if (!m_dictSelectPassiveSkill.ContainsKey(nCharID))
		{
			m_dictSelectPassiveSkill.Add(nCharID, new int[3] { -1, -1, -1 });
		}
		int[] array = m_dictSelectPassiveSkill[nCharID];
		if (nIndex >= 0 && nIndex < array.Length)
		{
			array[nIndex] = nPassiveSkillID;
		}
	}

	public void UnlockNewLevelPrepare()
	{
		m_bUnLockLevel = true;
	}

	public void UnlockNewLevelConfirm(int nNewLevel)
	{
		m_bUnLockLevel = false;
		m_nLatestLevel = nNewLevel;
	}

	public bool GetNewLevel(ref int nNewLevel)
	{
		if (!m_bUnLockLevel)
		{
			return false;
		}
		for (int i = 0; i < m_ltLevelList.Count - 1; i++)
		{
			if (m_nLatestLevel == m_ltLevelList[i])
			{
				nNewLevel = m_ltLevelList[i + 1];
				return true;
			}
		}
		return false;
	}

	public bool GetWeaponSign(int nWeaponID, ref int nSignState)
	{
		if (!m_dictWeaponSign.ContainsKey(nWeaponID))
		{
			return false;
		}
		nSignState = m_dictWeaponSign[nWeaponID];
		return true;
	}

	public void SetWeaponSign(int nWeaponID, int nSignState)
	{
		if (!m_dictWeaponSign.ContainsKey(nWeaponID))
		{
			m_dictWeaponSign.Add(nWeaponID, nSignState);
		}
		else
		{
			m_dictWeaponSign[nWeaponID] = nSignState;
		}
	}

	public bool GetEquipStoneSign(int nID, ref int nSignState)
	{
		if (!m_dictEquipStoneSign.ContainsKey(nID))
		{
			return false;
		}
		nSignState = m_dictEquipStoneSign[nID];
		return true;
	}

	public void SetEquipStoneSign(int nID, int nSignState)
	{
		if (!m_dictEquipStoneSign.ContainsKey(nID))
		{
			m_dictEquipStoneSign.Add(nID, nSignState);
		}
		else
		{
			m_dictEquipStoneSign[nID] = nSignState;
		}
	}

	public bool GetSkillSign(int nID, ref int nSignState)
	{
		if (!m_dictSkillSign.ContainsKey(nID))
		{
			return false;
		}
		nSignState = m_dictSkillSign[nID];
		return true;
	}

	public void SetSkillSign(int nID, int nSignState)
	{
		if (!m_dictSkillSign.ContainsKey(nID))
		{
			m_dictSkillSign.Add(nID, nSignState);
		}
		else
		{
			m_dictSkillSign[nID] = nSignState;
		}
	}

	public bool GetCharacterSign(int nID, ref int nSignState)
	{
		if (!m_dictCharacterSign.ContainsKey(nID))
		{
			return false;
		}
		nSignState = m_dictCharacterSign[nID];
		return true;
	}

	public void SetCharacterSign(int nID, int nSignState)
	{
		if (!m_dictCharacterSign.ContainsKey(nID))
		{
			m_dictCharacterSign.Add(nID, nSignState);
		}
		else
		{
			m_dictCharacterSign[nID] = nSignState;
		}
	}

	public void AddSceneProccess(int nAdd)
	{
		m_nSceneProccess += nAdd;
	}

	public bool IsLevelPassed(int nLevel)
	{
		foreach (CLevelSaveInfo item in m_ltLevelSaveInfo)
		{
			if (item.nID == nLevel)
			{
				return true;
			}
		}
		return false;
	}

	public void SetPassedLevel(int nLevel)
	{
		foreach (CLevelSaveInfo item in m_ltLevelSaveInfo)
		{
			if (item.nID == nLevel)
			{
				return;
			}
		}
		CLevelSaveInfo cLevelSaveInfo = new CLevelSaveInfo();
		cLevelSaveInfo.nID = nLevel;
		cLevelSaveInfo.isIgnoreCG = true;
		m_ltLevelSaveInfo.Add(cLevelSaveInfo);
	}

	public void SetLevelIgnoreCG(int nLevel, bool bIgnore)
	{
		foreach (CLevelSaveInfo item in m_ltLevelSaveInfo)
		{
			if (item.nID != nLevel)
			{
				continue;
			}
			item.isIgnoreCG = bIgnore;
			break;
		}
	}

	public bool IsLevelIgnoreCG(int nLevel)
	{
		foreach (CLevelSaveInfo item in m_ltLevelSaveInfo)
		{
			if (item.nID == nLevel)
			{
				return item.isIgnoreCG;
			}
		}
		return false;
	}
}
