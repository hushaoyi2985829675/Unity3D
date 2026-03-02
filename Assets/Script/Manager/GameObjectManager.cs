using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectManager : Singleton<GameObjectManager>
{
    [SerializeField]
    private Player player;
    //装备对象池
    Stack<GameObject> equipObjStack = new Stack<GameObject>();

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    public Player GetPlayer()
    {
        return this.player;
    }

    public Vector2 GetPlayerPos()
    {
        return player.transform.localPosition;
    }
    

    public GameObject GetEquipItemObj()
    {
        GameObject item;
        if (equipObjStack.Count > 0)
        {
            item =  equipObjStack.Pop();
        }
        else
        {
            item = Resources.Load<GameObject>("GameObjectRef/EquipItemRef");
        }
        item.transform.localPosition = new Vector3(0,0,0);
        return item;
    }

    public void RemoveObj(GameObject obj)
    {
        obj.SetActive(false);
        if (obj.CompareTag("Equip"))
        {
            equipObjStack.Push(obj);
        }
        else if (obj.CompareTag("Monster"))
        {
            
        }
    }
}
