using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NewBehaviourScript
{
    public static void SetActive(this MonoBehaviour mono, bool isActive)
    {
        mono.gameObject.SetActive(isActive);
    }

    public static bool IsActive(this MonoBehaviour mono)
    {
        return mono.gameObject.activeSelf;
    }
}
