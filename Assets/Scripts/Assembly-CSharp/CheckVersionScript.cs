using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using UnityEngine;

public class CheckVersionScript : MonoBehaviour
{
	public class ServerInfo
	{
		public string m_strServerAddress = string.Empty;

		public string m_strServerReserveIP = string.Empty;

		public List<int> m_iServerPort = new List<int>();

		public int m_iGroupBeginIndex;

		public ServerInfo(string address, string ip, List<int> port, int groupBeginIndex)
		{
			m_strServerAddress = address;
			m_strServerReserveIP = ip;
			m_iServerPort = port;
			m_iGroupBeginIndex = groupBeginIndex;
		}
	}

	private string url = "http://account.trinitigame.com/game/CoMI/CoMI-LAN.txt?time=";

	public float m_fStartTime = -1f;

	private float m_LoadTimeLimit = 10f;

	public float m_fVersion = -1f;

	public List<ServerInfo> m_serverInfo = new List<ServerInfo>();

	private OnCheckVersionCallBack m_ResponseDelegate;

	public void SetCallBack(OnCheckVersionCallBack callback)
	{
		m_ResponseDelegate = callback;
	}

	public IEnumerator Start()
	{
		m_fStartTime = Time.time;
		string randomSeedUrl = url + UnityEngine.Random.Range(0, 100);
		WWW www = new WWW(randomSeedUrl);
		yield return www;
		if (www.error != null)
		{
			Debug.LogError("CheckVersionScript.Start() - " + www.error);
			if (m_ResponseDelegate != null)
			{
				m_ResponseDelegate(true, string.Empty, null);
			}
		}
		else if (string.IsNullOrEmpty(www.text))
		{
			if (m_ResponseDelegate != null)
			{
				m_ResponseDelegate(true, string.Empty, null);
			}
		}
		else
		{
			LoadData(www.text);
			if (m_ResponseDelegate != null)
			{
				m_ResponseDelegate(false, m_fVersion.ToString(), m_serverInfo);
			}
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void Update()
	{
		if (Time.time - m_fStartTime > m_LoadTimeLimit)
		{
			if (m_ResponseDelegate != null)
			{
				m_ResponseDelegate(true, string.Empty, null);
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void LoadData(string input)
	{
		Debug.Log("CheckVersion.ServerData[" + input + "]");
		string empty = string.Empty;
		try
		{
			empty = Decrypt(input);
			Debug.LogWarning("CheckVersion.Decrypt[" + empty + "]");
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(empty);
			XmlElement documentElement = xmlDocument.DocumentElement;
			m_fVersion = float.Parse(((XmlElement)documentElement.GetElementsByTagName("AppVersion").Item(0)).GetAttribute("Value"));
			XmlElement xmlElement = (XmlElement)documentElement.GetElementsByTagName("Server").Item(0);
			foreach (XmlElement item3 in xmlElement.GetElementsByTagName("ServerInfo"))
			{
				string attribute = item3.GetAttribute("Address");
				string attribute2 = item3.GetAttribute("ReserveIP");
				List<int> list = new List<int>();
				string[] array = item3.GetAttribute("Port").Split('|');
				string[] array2 = array;
				foreach (string s in array2)
				{
					int item = int.Parse(s);
					list.Add(item);
				}
				int groupBeginIndex = int.Parse(item3.GetAttribute("GroupIndexBegin"));
				m_serverInfo.Add(new ServerInfo(attribute, attribute2, list, groupBeginIndex));
			}
			XmlElement xmlElement3 = (XmlElement)documentElement.GetElementsByTagName("TestServer").Item(0);
			foreach (XmlElement item4 in xmlElement3.GetElementsByTagName("ServerInfo"))
			{
				string attribute3 = item4.GetAttribute("Address");
				string attribute4 = item4.GetAttribute("ReserveIP");
				List<int> list2 = new List<int>();
				string[] array3 = item4.GetAttribute("Port").Split('|');
				string[] array4 = array3;
				foreach (string s2 in array4)
				{
					int item2 = int.Parse(s2);
					list2.Add(item2);
				}
				int groupBeginIndex2 = int.Parse(item4.GetAttribute("GroupIndexBegin"));
				m_serverInfo.Add(new ServerInfo(attribute3, attribute4, list2, groupBeginIndex2));
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("CheckVersion Error " + ex);
		}
	}

	private string Decrypt(string input_data)
	{
		string empty = string.Empty;
		byte[] data = Convert.FromBase64String(input_data);
		string s = "1234";
		byte[] bytes = XXTEAUtils.Decrypt(data, Encoding.ASCII.GetBytes(s));
		return Encoding.UTF8.GetString(bytes);
	}

	public static void Encrypt()
	{
		string text = Utils.SavePath();
		string path = text + "/CoMI.xml";
		string path2 = text + "/CoMI.txt";
		string text2 = string.Empty;
		if (File.Exists(path))
		{
			StreamReader streamReader = null;
			try
			{
				streamReader = new StreamReader(path);
				text2 = streamReader.ReadToEnd();
			}
			catch
			{
				Debug.Log("ERROR - Encrypt()!!!");
			}
			finally
			{
				if (streamReader != null)
				{
					streamReader.Close();
				}
			}
		}
		if (!string.IsNullOrEmpty(text2))
		{
			string empty = string.Empty;
			string s = "12345";
			byte[] inArray = XXTEAUtils.Encrypt(Encoding.UTF8.GetBytes(text2), Encoding.UTF8.GetBytes(s));
			string value = Convert.ToBase64String(inArray);
			StreamWriter streamWriter = new StreamWriter(path2, false);
			streamWriter.Write(value);
			streamWriter.Flush();
			streamWriter.Close();
		}
	}
}
