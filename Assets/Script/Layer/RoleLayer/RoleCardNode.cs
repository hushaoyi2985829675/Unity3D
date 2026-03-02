using System;
using System.Collections;
using System.Collections.Generic;
using RoleCampNs;
using RoleNs;
using UnityEngine;
using UnityEngine.UI;

public class RoleCardNode : PanelBase
{
    [SerializeField]
    private Button roleBtn;
    [SerializeField]
    private Image roleImg;
    [SerializeField]
    private Text roleName;
    [SerializeField]
    private Image maskImg;
    [SerializeField]
    private Image selImg;
    [SerializeField]
    private Image campImg;
    
    private int roleId;
    private Dictionary<int,RoleInfo> roleInfoDict;
    private RoleInfo roleInfo;
    private Action<int> callback;
    public override void onEnter(params object[] data)
    {
        roleBtn.onClick.AddListener(OnClick);
    }

    public override void onShow(params object[] data)
    {
       
    }

    public void RefreshUI(int roleId,Action<int> callback)
    {
        this.callback = callback;
        roleInfoDict = ConfigManager.Instance.GetRoleConfig();
        this.roleId = roleId;
        if (roleInfoDict.ContainsKey(roleId))
        {
            roleInfo = roleInfoDict[roleId];
            string path = roleInfo.path;
            Sprite sprite = Ui.Instance.GetSprite(path);
            roleImg.sprite = sprite;
            roleName.text = roleInfo.name;
            roleName.color = Ui.Instance.GetQualityColor(roleInfo.quality);
            //刷新职业
            RoleCampInfo roleCampInfo = ConfigManager.Instance.GetRoleCampInfoById(roleInfo.campId);
            Sprite campSpr = Ui.Instance.GetSprite(roleCampInfo.campImg);
            campImg.sprite = campSpr;
        }
    }

    public void RefreshSel(bool isActive)
    {
        selImg.SetActive(isActive);
    }

    private void OnClick()
    {
        RefreshSel(true);
        callback(roleId);
    }

    public override void onExit()
    {
        
    }
}
