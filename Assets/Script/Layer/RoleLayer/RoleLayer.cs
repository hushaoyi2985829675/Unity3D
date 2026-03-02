using System;
using System.Collections;
using System.Collections.Generic;
using RoleNs;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine;
using UnityEngine.UI;

public class RoleLayer : PanelBase
{
    [SerializeField]
    private TableView tableView;
    [SerializeField]
    private Transform rightNode;
    private int selId;
    private int selTag;
    private RoleScene roleScene;
    private RoleNode roleNode;
    private RoleDescNode roleDescNode;
    private TabItem oldTabItem;
    public override void onEnter(params object[] data)
    {
        tableView.AddRefreshEvent(CreateTabItem);
    }

    public override void onShow(params object[] data)
    {
        roleNode = null;
        selId = 1;
        selTag = 1;
        SceneLoader.Instance.LoadAsyncScene("RoleScene",false, (sceneBase) =>
        {
           roleScene = sceneBase as RoleScene;
           tableView.SetNum(3);
           RefreshUI();
        });
    }

    private void CreateTabItem(int index, GameObject obj)
    {
        TabItem tabItem = obj.GetComponent<TabItem>();
        tabItem.RefreshSel(selTag == index + 1);
        if (selTag == index + 1)
        {
            oldTabItem = tabItem;
        }
        tabItem.Init(index + 1, (tagId) =>
        {
            if (tagId == selTag)
            {
                return;
            }
          
            if (oldTabItem)
            {
                oldTabItem.RefreshSel(false);
            }
            tabItem.RefreshSel(true);
            oldTabItem = tabItem;
            selTag = tagId;
            RefreshUI();  
        });
    }

    private void OnTabClick(int tagId)
    {
        
    }

    private void RefreshUI()
    {
        CloseUILayer(roleNode?.gameObject);
        CloseUILayer(roleDescNode?.gameObject);
        //角色
        if (selTag == 1)
        {
            if (roleNode == null)
            {
                GameObject obj = Ui.Instance.GetLayerPrefab("RoleLayer/RoleNode");
                roleNode = AddUINode<RoleNode>(obj, rightNode);
            }
            roleNode.SetActive(true);
            roleNode.Init(OnRoleCardClick);
        }

        if (selTag == 2)
        {
            if (roleDescNode == null)
            {
                GameObject obj = Ui.Instance.GetLayerPrefab("RoleLayer/roleDescNode");
                roleDescNode = AddUINode<RoleDescNode>(obj, rightNode);
            }
            roleDescNode.SetActive(true);
            roleDescNode.Init(selId);
        }
    }

    private void OnRoleCardClick(int id)
    {
        if (selId == id)
        {
            return;
        }
        selId = id;
        roleScene.RefreshUI(id);
    }

    public override void onExit()
    {
        SceneLoader.Instance.RemoveScene("RoleScene");
    }
}
