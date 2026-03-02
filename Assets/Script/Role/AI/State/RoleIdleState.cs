using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleIdleState : RoleStateBase
{
    public RoleIdleState(RoleBehaviorLogic roleBehaviorLogic) : base(roleBehaviorLogic)
    {
    }

    public override void OnEnter()
    {
        roleBehaviorLogic.IdleEnter();
    }

    public override void OnUpdate()
    {
        roleBehaviorLogic.IdleUpdate();
    }

    public override void OnExit()
    {
        roleBehaviorLogic.IdleExit();
    }
}
