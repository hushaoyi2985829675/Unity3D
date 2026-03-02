using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static bool applicationIsQuitting = false;
    public static T Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                return null;
            }
            if (_instance == null)
            {
                _instance = (T) FindObjectOfType(typeof(T));
                if (_instance == null)
                {
                    GameObject go = new GameObject(typeof(T).Name);
                    go.AddComponent<T>();
                    _instance = go.GetComponent<T>();
                    DontDestroyOnLoad(go);
                }
            }

            return _instance;
        }
    }

    protected virtual void OnDestroy()
    {
        applicationIsQuitting = true;
    }

    // public static implicit operator T(Singleton<T> unused)
    // {
    //     return Instance;
    // }
}
