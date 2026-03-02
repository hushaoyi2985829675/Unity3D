using System;
using System.Collections;
using System.Collections.Generic;
using RoleNs;
using Unity.VisualScripting;
using UnityEngine;

public class RoleNode : PanelBase
{
    [SerializeField]
    private TableView tableView;
    [SerializeField]
    private CampNode campNode;
    
    private List<RoleInfo> roleInfoList = new List<RoleInfo>();
    private Dictionary<int,List<RoleInfo>> roleInfoDict =  new Dictionary<int, List<RoleInfo>>();
    private RoleCardNode oldRoleCardNode;
    private int selId;
    private Action<int> callback;
    public override void onEnter(params object[] data)
    {
        tableView.AddRefreshEvent(CreateTable);
        Dictionary<int,RoleInfo> roleDict = ConfigManager.Instance.GetRoleConfig();
        foreach (var keyValuePair in roleDict )
        {
            RoleInfo roleInfo = keyValuePair.Value;
            if (!roleInfoDict.ContainsKey(roleInfo.campId))
            {
                roleInfoDict[roleInfo.campId] =  new List<RoleInfo>();    
            }
            roleInfoDict[roleInfo.campId].Add(roleInfo);
        }
        foreach (var keyValuePair in roleInfoDict)
        {
            List<RoleInfo> roleInfoList = keyValuePair.Value;
            roleInfoList.Sort((a, b) => a.quality > b.quality ? -1 : 1);
        }

        selId = roleInfoDict[1][0].roleId;
    }

    public override void onShow(params object[] data)
    {
        campNode.Init(CampAction);
    }

    public void Init(Action<int> callback)
    {
        this.callback = callback;
        Debug.Log(selId);
        callback(selId);
    }

    private void CampAction(int campId)
    {
        roleInfoList = roleInfoDict[campId];
        tableView.SetNum(roleInfoList.Count);
    }
    private void CreateTable(int index,GameObject obj)
    {
        RoleCardNode RoleCardNode = obj.GetComponent<RoleCardNode>();
        RoleCardNode.RefreshUI(roleInfoList[index].roleId, (id) =>
        {
            if (selId == id)
            {
                return;
            }

            selId = id;
            if (oldRoleCardNode != null)
            {
                oldRoleCardNode.RefreshSel(false);
            }
            oldRoleCardNode = RoleCardNode;
            if (callback != null)
            {
                callback(selId);
            }
        });
        RoleCardNode.RefreshSel(selId == roleInfoList[index].roleId);
        if (selId == roleInfoList[index].roleId)
        {
            oldRoleCardNode = RoleCardNode;
        }
    }
    public override void onExit()
    {
        
    }
}
