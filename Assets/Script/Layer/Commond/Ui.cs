using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;


public class Ui : Singleton<Ui>
{
    //路径类
    private string layerPath = "Layer/";
    private Transform heroObjNode;
    private Dictionary<string, Sprite> spriteDict = new Dictionary<string, Sprite>();

    private void Awake()
    {
        //人物节点
        heroObjNode = new GameObject("HeroObjNode").transform;
        InitSprite();
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void InitSprite()
    {
        Resources.Load<Sprite>(layerPath);
        Sprite[] sprites = Resources.LoadAll<Sprite>("UI");
        for (int i = 0; i < sprites.Length; i++)
        {
            if (spriteDict.ContainsKey(sprites[i].name))
            {
                break;
            }

            spriteDict.Add(sprites[i].name, sprites[i]);
        }
    }

    //获取页面
    public GameObject GetLayerPrefab(string layerName)
    {
        GameObject layerPrefab = Resources.Load<GameObject>(layerPath + layerName);
        if (layerPrefab == null)
        {
            Debug.LogError("Layer预制体找不到: " + layerPath + layerName);
        }

        return layerPrefab;
    }

    //获取子节点
    public GameObject GetChild(Transform parent, string name)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            if (parent.GetChild(i).name == name)
            {
                return parent.GetChild(i).gameObject;
            }

            GameObject obj = GetChild(parent.GetChild(i), name);
            if (obj != null)
            {
                return obj;
            }
        }

        return null;
    }

    //删除所有子节点
    public void RemoveAllChildren(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(parent.GetChild(i).gameObject);
        }
    }
    
    public Sprite GetSprite(string spriteName)
    {
        if (!spriteDict.ContainsKey(spriteName))
        {
            Debug.LogError("找不到图片: " + spriteName);
            return null;
        }
        Sprite sprite = spriteDict[spriteName];
        return sprite;
    }

    public Color GetQualityColor(int quality, bool isBack = true)
    {
        float normalize = 1f / 255f;

        if (isBack)
        {
            switch (quality)
            {
                case 1:
                    return new Color(235 * normalize, 235 * normalize, 235 * normalize);
                case 2:
                    return new Color(41 * normalize, 182 * normalize, 246 * normalize);
                case 3:
                    return new Color(211 * normalize, 47 * normalize, 47 * normalize);
                default:
                    return Color.white;
            }
        }
        else
        {
            switch (quality)
            {
                case 1:
                    return new Color(85 * normalize, 85 * normalize, 85 * normalize);
                case 2:
                    return new Color(13 * normalize, 71 * normalize, 161 * normalize);
                case 3:
                    return new Color(149 * normalize, 17 * normalize, 17 * normalize);
                default:
                    return Color.white;
            }
        }
    }
    //在圆里获取随机坐标
    public Vector3 GetRandomPointInCircle(Vector3 center,float radius)
    {
        for (int i = 0; i < 30; i++)
        {
            float randomAngle = Random.Range(0f, Mathf.PI * 2);
            float x = radius * math.cos(randomAngle);
            float z = radius * math.sin(randomAngle);
            NavMeshHit hit;
            if (NavMesh.SamplePosition(new Vector3(x,center.y,z) , out hit, 1.0f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }
        Debug.Log("随机移动坐标超出30次");
        return Vector3.zero;
    }
}
