using System;
using System.Collections;
using System.Collections.Generic;
using RoleNs;
using UnityEngine;

public class CharacterAIBase :  UnitAIBase
{
    //角色相关
    [SerializeField]
    private int roleId;
    private RoleCamp roleCamp;
    protected RoleInfo  roleInfo;
    private RoleBehaviorLogic roleBehaviorLogic;
    //状态
    private bool isFoundMonster;

    private UnitBehaviorLogicBase unitBehaviorLogic;
    private int rollDirection = 0;
    protected override void Awake()
    {
        targetSearchLayer = "Monster";
        base.Awake();
        InitRole(roleId);
    }
    
    void Start()
    {
    }
    
    public virtual void InitRole(int id)
    {
        roleId = id;
        roleInfo = ConfigManager.Instance.GetRoleInfoById(id);
        SetNavSpeed(roleInfo.moveSpeed);
    }

    public RoleInfo GetRoleInfo()
    {
        return roleInfo;
    }

    public int GetRollDirection()
    {
        return rollDirection;
    }

    public void SetRollDirection(int direction)
    {
        rollDirection = direction;
    }
}
