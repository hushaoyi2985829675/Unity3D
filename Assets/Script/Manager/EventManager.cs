using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum GameEventType
{
    ResEvent,
}

class EventBase
{
    private int id;
}

public class EventManager : Singleton<EventManager>
{
    private int id;

    private Stack<int> idStack;
    //资源更新事件
    private Dictionary<int, Dictionary<int, Action>> resAction;

    //血量经验等级事件
    private Action playerStatusAction;

    //弹窗事件
    private Action<string> showFlutterAction;

    //移动事件
    private Action<int> PlayerMovekEventAction;

    //攻击事件
    private Action PlayerAttackEventAction;

    //跳跃事件
    private Action PlayerJumpEventAction;

    //技能事件
    private Action<int> playerSkillAction;

    //技能更新事件
    private Action<int> playerSkillUpdateAction;

    //楼梯模式事件
    private Action<bool> stairsAction;
    private void Awake()
    {
        id = 1;
        idStack = new Stack<int>();
        resAction = new Dictionary<int, Dictionary<int, Action>>();
    }

    //添加事件
    public int AddEvent(GameEventType eventType, object[] data)
    {
        int eventId = GetEventId();
        switch (eventType)
        {
            case GameEventType.ResEvent:
                int id = (int) data[0];
                if (!resAction.ContainsKey(id))
                {
                    resAction[id] = new Dictionary<int, Action>();
                }

                if (resAction[id].ContainsKey(eventId))
                {
                    resAction[id][eventId] += (Action) data[1];
                }
                else
                {
                    resAction[id].Add(eventId, (Action) data[1]);
                }

                break;
        }

        return eventId;
    }

    private int GetEventId()
    {
        if (idStack.Count > 0)
        {
            return idStack.Pop();
        }

        id++;
        return id - 1;
    }

    //移除资源更新事件
    public void RemoveEvent(GameEventType eventType, int eventId, object[] data = null)
    {
        switch (eventType)
        {
            case GameEventType.ResEvent:
                int id = (int) data[0];
                if (resAction.ContainsKey(id))
                {
                    if (resAction[id].ContainsKey(eventId))
                    {
                        resAction[id].Remove(eventId);
                    }
                }

                break;
        }
    }
    //调用事件
    public void PostEvent(GameEventType eventType, object[] data = null)
    {
        switch (eventType)
        {
            case GameEventType.ResEvent:
                int id = (int) data[0];
                if (resAction.ContainsKey(id))
                {
                    foreach (var resPair in resAction[id])
                    {
                        resPair.Value.Invoke();
                    }
                }
                break;
        }
    }
    
}