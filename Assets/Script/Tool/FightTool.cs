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
        if (transform == null || radius <= 0f || string.IsNullOrEmpty(layerName))
        {
            return null;
        }

        int layerIndex = LayerMask.NameToLayer(layerName);
        if (layerIndex < 0)
        {
            return null;
        }

        Vector3 center = transform.position;
        Collider[] colliders = Physics.OverlapSphere(center, radius, 1 << layerIndex, QueryTriggerInteraction.Ignore);
        if (colliders.Length == 0)
        {
            return null;
        }

        GameObject target = colliders[0].gameObject;
        float minDistance = (center - target.transform.position).sqrMagnitude;
        if (colliders.Length > 1)
        {
            for (int i = 1; i < colliders.Length; i++)
            {
                float distance = (center - colliders[i].transform.position).sqrMagnitude;
                if (distance < minDistance)
                {
                    minDistance = distance;
                    target =  colliders[i].gameObject;
                }
            }
        }
        return target;
    }

    /// <summary>
    /// 判断指定目标是否在检测范围内（并校验层）
    /// </summary>
    /// <param name="transform">检测中心</param>
    /// <param name="radius">检测半径</param>
    /// <param name="target">需要检测的目标</param>
    /// <returns>目标在范围内且层匹配返回 true，否则 false</returns>
    public static bool IsTargetInRange(Transform transform, float radius, GameObject target)
    {
        if (transform == null || target == null || radius <= 0f)
        {
            return false;
        }
        float sqrDistance = (transform.position - target.transform.position).sqrMagnitude;
        return sqrDistance <= radius * radius;
    }

    /// <summary>
    /// 射线检测指定层是否命中
    /// </summary>
    /// <param name="transformPos">射线起点</param>
    /// <param name="direction">射线方向</param>
    /// <param name="distance">射线长度</param>
    /// <param name="layerName">需要检测的层名</param>
    public static RaycastHit CheckRaycastHitByLayer(Vector3 transformPos, Vector3 direction, float distance, string layerName)
    {
        RaycastHit hit = new RaycastHit();
        if (distance <= 0f || direction == Vector3.zero || string.IsNullOrEmpty(layerName))
        {
            return hit;
        }

        Vector3 normalizedDirection = direction.normalized;
        Vector3 maxEndPos = transformPos + normalizedDirection * distance;
        int layerIndex = LayerMask.NameToLayer(layerName);
        if (layerIndex < 0)
        {
            Debug.LogWarning($"Layer not found: {layerName}");
            Debug.DrawLine(transformPos, maxEndPos, Color.red, 0.2f, false);
            return hit;
        }

        bool hasHit = Physics.Raycast(
            transformPos,
            normalizedDirection,
            out hit,
            distance,
            1 << layerIndex,
            QueryTriggerInteraction.Ignore
        );
        Debug.Log("hasHit: " + hasHit);
        Vector3 endPos = new Vector3(hit.point.x, hit.point.y - distance, hit.point.z);
        Debug.DrawLine(transformPos, endPos, hasHit ? Color.green : Color.red, 0.2f, false);

        return hit;
    }
}
