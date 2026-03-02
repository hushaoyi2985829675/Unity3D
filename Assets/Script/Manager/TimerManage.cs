using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class TimeInfo
{
    Action callback;
    private float callTime;
    private float curTime;
    public int id;
    private static Stack<int> idStack = new Stack<int>();
    private static int idInx;

    public TimeInfo(Action callback, float callTime)
    {
        this.callback = callback;
        this.callTime = callTime;
        curTime = 0;
        if (idStack.Count == 0)
        {
            idInx++;
            id = idInx;
        }
        else
        {
            id = idStack.Pop();
        }
    }

    public static void RecycleId(int id)
    {
        idStack.Push(id);
    }

    public bool AddTime()
    {
        curTime += Time.deltaTime;
        if (curTime >= callTime)
        {
            callback();
            return true;
        }

        return false;
    }

    public void ScheduleCallback()
    {
        curTime += Time.deltaTime;
        if (curTime >= callTime)
        {
            callback();
            curTime = 0;
        }
    }
}

public class TimerManage : Singleton<TimerManage>
{
    private Dictionary<int, TimeInfo> delayDict = new Dictionary<int, TimeInfo>();
    private Dictionary<int, TimeInfo> scheduleDict = new Dictionary<int, TimeInfo>();
    List<int> keysToRemove = new List<int>();
    private Dictionary<int, TimeInfo> delayCacheDict = new Dictionary<int, TimeInfo>();

    public int AddDelayCallback(Action callback, float delay)
    {
        TimeInfo timeInfo = new TimeInfo(callback, delay);
        delayCacheDict.Add(timeInfo.id, timeInfo);
        return timeInfo.id;
    }

    public void Update()
    {
        foreach (KeyValuePair<int, TimeInfo> keyValuePair in delayCacheDict)
        {
            delayDict.Add(keyValuePair.Key, keyValuePair.Value);
        }

        delayCacheDict.Clear();
    }

    public void LateUpdate()
    {
        foreach (KeyValuePair<int, TimeInfo> keyValuePair in delayDict)
        {
            TimeInfo timeInfo = keyValuePair.Value;
            if (timeInfo.AddTime())
            {
                keysToRemove.Add(timeInfo.id);
            }
        }

        foreach (int key in keysToRemove)
        {
            delayDict.Remove(key);
        }

        keysToRemove.Clear();
        foreach (KeyValuePair<int, TimeInfo> keyValuePair in scheduleDict)
        {
            TimeInfo timeInfo = keyValuePair.Value;
            timeInfo.ScheduleCallback();
        }
    }

    public int AddScheduleCallback(Action callback, float delay, bool isCall = true)
    {
        if (isCall)
        {
            callback();
        }

        TimeInfo timeInfo = new TimeInfo(callback, delay);
        scheduleDict.Add(timeInfo.id, timeInfo);
        return timeInfo.id;
    }

    public void RemoveScheduleCallback(int id)
    {
        TimeInfo.RecycleId(id);
        scheduleDict.Remove(id);
    }

    public void RemoveDelayCallback(int id)
    {
        TimeInfo.RecycleId(id);
        delayDict.Remove(id);
    }
}