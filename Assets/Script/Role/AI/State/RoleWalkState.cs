using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleWalkState : RoleStateBase
{
    public RoleWalkState(RoleBehaviorLogic roleBehaviorLogic) : base(roleBehaviorLogic)
    {
    }

    public override void OnEnter()
    {
        roleBehaviorLogic.OnWalkEnter();
    }

    public override void OnUpdate()
    {
        roleBehaviorLogic.OnWalkUpdate();
    }

    public override void OnExit()
    {
        roleBehaviorLogic.OnWalkExit();
    }
}
