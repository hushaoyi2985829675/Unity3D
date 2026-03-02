using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviorBase
{
    public abstract bool Play();
    public List<BehaviorBase> behaviorList;
}

public class Tree : MonoBehaviour
{
   public void CheckBehavior(List<BehaviorBase> behaviors,int idx)
   {
       if (behaviors.Count <= idx)
       {
           return;
       }
       if (behaviors[idx].Play())
       {
           CheckBehavior(behaviors[idx].behaviorList,0);
           return;
       }
       CheckBehavior(behaviors, idx + 1);
   }
}
