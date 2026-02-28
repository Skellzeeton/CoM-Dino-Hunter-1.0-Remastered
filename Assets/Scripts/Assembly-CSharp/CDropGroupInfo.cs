using System.Collections.Generic;
using UnityEngine;

public class CDropGroupInfo
{
    public int nID;

    public List<CDropItem> ltItem;

    public CDropGroupInfo()
    {
        ltItem = new List<CDropItem>();
    }

    public void Clear()
    {
        if (ltItem != null)
        {
            ltItem.Clear();
        }
    }

    public void Add(CDropItem item)
    {
        if (ltItem != null)
        {
            ltItem.Add(item);
        }
    }

    public int GetCount()
    {
        if (ltItem == null)
        {
            return 0;
        }
        return ltItem.Count;
    }

    public int GetDropItem()
    {
        string text = string.Empty;
        string text2 = string.Empty;
        float[] array = new float[ltItem.Count];
        for (int i = 0; i < ltItem.Count; i++)
        {
            if (i == 0)
            {
                array[i] = ltItem[i].fRate;
                text = array[i].ToString();
                text2 = ltItem[i].nItemID.ToString();
            }
            else
            {
                array[i] = array[i - 1] + ltItem[i].fRate;
                text = text + "," + array[i];
                text2 = text2 + "," + ltItem[i].nItemID;
            }
        }
        Debug.Log(text);
        Debug.Log(text2);
        float num = Random.Range(0f, array[ltItem.Count - 1]);
        for (int j = 0; j < ltItem.Count; j++)
        {
            if (num <= array[j])
            {
                return ltItem[j].nItemID;
            }
        }
        return -1;
    }
}