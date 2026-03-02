using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
    private Action<string> animationEvent;
    private Dictionary<string, Action<string>> customEventDict = new Dictionary<string, Action<string>>();

    private void Awake()
    {
    
    }

    private void OnAnimationEvent(string eventName)
    {
        animationEvent?.Invoke(eventName);
    }

    public void PostAnimationEvent(string eventName)
    {
        if (customEventDict.ContainsKey(eventName))
        {
            customEventDict[eventName].Invoke(eventName);
        }
    }
    
    public void RegisterAnimationEvent(string eventName,Action<string> action)
    {
        if (!customEventDict.ContainsKey(eventName))
        {
            customEventDict.Add(eventName, action); 
        }
        else
        {
            customEventDict[eventName] += action;
        }
    }

    public void AddAnimationEvent(Action<string> action)
    {
        animationEvent += action;
    }

    private void Update()
    {
       
    }
}
