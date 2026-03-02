using System.Collections;
using System.Collections.Generic;
using RoleNs;
using UnityEngine;
using UnityEngine.UI;

public class RoleDescNode : PanelBase
{
    [SerializeField]
    private Text text;
    public override void onEnter(params object[] data)
    {
        
    }

    public override void onShow(params object[] data)
    {
        
    }

    public void Init(int id)
    {
        RoleInfo roleInfo = ConfigManager.Instance.GetRoleInfoById(id);
        text.text = roleInfo.desc;
    }

    public override void onExit()
    {
        
    }
}
