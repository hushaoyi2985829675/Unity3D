using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class EventInfo
{
    public GameEventType eventType;
    public int id;
    public object[] data;

    public EventInfo(GameEventType type, int id, object[] data = null)
    {
        this.eventType = type;
        this.id = id;
        this.data = data;
    }
}

public abstract class PanelBase : MonoBehaviour
{
    [Header("是否是全屏页面")]
    [SerializeField]
    public bool isFullScreen;
    private Dictionary<int, PanelBase> UINodeDict = new Dictionary<int, PanelBase>();
    private List<int> scheduleList = new List<int>();
    private List<int> delayList = new List<int>();
    private List<EventInfo> eventList = new List<EventInfo>();
    public abstract void onEnter(params object[] data);

    public abstract void onShow(params object[] data);

    protected T AddUINode<T>(GameObject layerRef, Transform parent, params object[] data)
        where T : PanelBase
    {
        PanelBase layer = UIManager.Instance.AddUINode(layerRef, parent, data);
        int gId = layer.gameObject.GetInstanceID();
        UINodeDict[gId] = layer;
        return layer as T;
    }

    protected PanelBase AddUINode(GameObject layerRef, Transform parent, params object[] data)
    {
        return AddUINode<PanelBase>(layerRef, parent, data);
    }

    protected void CloseUINode(Transform trans)
    {
        if (trans == null) return;

        // 若子物体是 PanelBase，先执行其清理（事件、定时器、子节点等）再销毁
        PanelBase childPanel = trans.GetComponent<PanelBase>();
        if (childPanel != null)
        {
            childPanel.onExit();
            childPanel.Hide();
        }

        int gId = trans.gameObject.GetInstanceID();
        UINodeDict.Remove(gId);
        Destroy(trans.gameObject);
    }

    protected void CloseNodeAllUINode(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            CloseUINode(child);
        }
    }

    public abstract void onExit();

    protected void AddEvent(GameEventType type, object[] data = null)
    {
        int id = EventManager.Instance.AddEvent(type, data);
        EventInfo eventInfo = new EventInfo(type, id, data);
        eventList.Add(eventInfo);
    }

    private void RemoveEvent()
    {
        foreach (EventInfo eventInfo in eventList)
        {
            EventManager.Instance.RemoveEvent(eventInfo.eventType, eventInfo.id, eventInfo.data);
        }
    }

    protected void AddScheduleCallback(Action callback, float delay)
    {
        int id = TimerManage.Instance.AddScheduleCallback(callback, delay);
        scheduleList.Add(id);
    }

    protected void AddDelayCallback(Action callback, float delay)
    {
        int id = TimerManage.Instance.AddDelayCallback(callback, delay);
        delayList.Add(id);
    }

    public void Hide()
    {
        foreach (PanelBase uiNode in UINodeDict.Values)
        {
            UIManager.Instance.CloseUINode(uiNode.gameObject);
            Destroy(uiNode.gameObject);
        }

        UINodeDict.Clear();
        RemoveEvent();
        RemoveAllScheduleAndDelay();
    }

    //清除所有定时器延时函数
    protected void RemoveAllScheduleAndDelay()
    {
        //定时器
        foreach (int id in scheduleList)
        {
            TimerManage.Instance.RemoveScheduleCallback(id);
        }

        scheduleList.Clear();
        foreach (int id in delayList)
        {
            TimerManage.Instance.RemoveDelayCallback(id);
        }

        delayList.Clear();
    }

    public bool GetIsFullScreen()
    {
        return  isFullScreen;
    }
}
