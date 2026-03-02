using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SceneBase : MonoBehaviour
{
    public abstract void onEnter(params object[] data);
    
    
    public abstract void onExit(params object[] data);
}
