using System;
using System.Collections;
using System.Collections.Generic;
using RoleCampNs;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class CampNode : PanelBase
{
    [SerializeField]
    private Transform parent;
    [SerializeField]
    private ToggleGroup  toggleGroup;
    private Action<int> onClick;
    private GameObject campItemRef;
    public override void onEnter(params object[] data)
    {
        
    }

    public override void onShow(params object[] data)
    {
        
    }

    private void Awake()
    {
        campItemRef = Ui.Instance.GetLayerPrefab("Common/CampNode/CampItem");
    }

    public void Init(Action<int> callback)
    {
        onClick = callback;
        RefreshUI();
    }

    private void RefreshUI()
    {
        Ui.Instance.RemoveAllChildren(parent);
        Dictionary<int, RoleCampInfo> campDict = ConfigManager.Instance.GetRoleCampConfig();
        foreach (var keyValuePair in campDict)
        {
            RoleCampInfo campInfo = keyValuePair.Value;
            CampItem campItem = AddUINode<CampItem>(campItemRef,parent,new object[] {(object)onClick});
            campItem.Init(campInfo.campId,toggleGroup);
            campItem.AddEvent(onClick);
        }
    }
    public override void onExit()
    {
        
    }
}
