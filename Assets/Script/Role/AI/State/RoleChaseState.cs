using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleChaseState : RoleStateBase
{
    public RoleChaseState(RoleBehaviorLogic roleBehaviorLogic) : base(roleBehaviorLogic)
    {
    }

    public override void OnEnter()
    {
        roleBehaviorLogic.ChaseEnter(); 
    }

    public override void OnUpdate()
    {
        roleBehaviorLogic.ChaseUpdate();
    }

    public override void OnExit()
    {
        roleBehaviorLogic.ChaseExit();
    }
}
