using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabView : MonoBehaviour
{
    public GameObject Item;
    Action<int, GameObject> RefreshItemEvent;
    Dictionary<int,GameObject> ItemList = new Dictionary<int, GameObject>();

    public void SetNum(int num, int selTag)
    {
        for (int i = 0; i < num; i++)
        {
            if (!ItemList.ContainsKey(i))
            {
                var item = Instantiate(Item, transform);
                item.SetActive(true);
                Toggle toggle = item.GetComponent<Toggle>();
                toggle.isOn = selTag == i;
                toggle.group = GetComponent<ToggleGroup>();
                RefreshItemEvent.Invoke(i, item);
                ItemList[i] = item;
            }   
        }
    }
    public void AddRefreshEvent(Action<int, GameObject> action)
    {
        if (RefreshItemEvent != null)
        {
            return;
        }
        RefreshItemEvent = action;
    }
}
