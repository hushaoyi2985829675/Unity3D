using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightTool 
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="radius"> 半径 </param>
    /// <param name="layerName"> Layer名 </param>
    public static GameObject FindNearestTargetByLayer(Transform transform,float radius,string layerName) 
    {
        int layerIndex = LayerMask.NameToLayer(layerName);
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, 1 << layerIndex);
        if (colliders.Length == 0)
        {
            return null;
        }
        GameObject target = colliders[0].gameObject;
        float minDistance = (transform.position - target.transform.position).sqrMagnitude;
        if (colliders.Length > 1)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                float dic = (transform.position - colliders[i].transform.position).sqrMagnitude;
                if (dic < minDistance)
                {
                    minDistance = dic;
                    target =  colliders[i].gameObject;
                }
            }
        }
        return target;
    }
}
