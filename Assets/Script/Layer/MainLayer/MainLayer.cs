using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainLayer: PanelBase
{
    [SerializeField]
    private Button roleBtn;

    private GameObject rolePrefab;
    private void Awake()
    {
        rolePrefab = Ui.Instance.GetLayerPrefab("RoleLayer/RoleLayer");
    }

    void Start()
    {
        roleBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.AddLayer(rolePrefab);
        });
    }

    public override void onEnter(params object[] data)
    {
        
    }

    public override void onShow(params object[] data)
    {
        
    }

    public override void onExit()
    {
        
    }
}
