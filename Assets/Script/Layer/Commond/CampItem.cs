using System;
using System.Collections;
using System.Collections.Generic;
using RoleCampNs;
using UnityEngine;
using UnityEngine.UI;

public class CampItem : PanelBase
{
    [SerializeField]
    private Image image;
    [SerializeField]
    private Toggle toggle;
    private Action<int> clickAction;
    public override void onEnter(params object[] data)
    {
        
    }

    public override void onShow(params object[] data)
    {
        
    }

    public void AddEvent(Action<int> action)
    {
        clickAction = action;
    }

    public void Init(int campId,ToggleGroup toggleGroup)
    {
        RoleCampInfo roleCampInfo = ConfigManager.Instance.GetRoleCampInfoById(campId);
        Sprite sprite = Ui.Instance.GetSprite(roleCampInfo.campImg);
        image.sprite = sprite;
        toggle.group = toggleGroup;
        toggle.onValueChanged.AddListener(isOn =>
        {
            if (!isOn || clickAction == null)
            {
                return;
            }
            clickAction(campId);
        });
    }

    public override void onExit()
    {
        
    }
}
