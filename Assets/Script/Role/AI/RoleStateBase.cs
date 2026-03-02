using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RoleStateBase
{
    protected RoleBehaviorLogic roleBehaviorLogic;
   protected Animator animator;
   public RoleStateBase(RoleBehaviorLogic roleBehaviorLogic)
   {
       this.roleBehaviorLogic = roleBehaviorLogic;
       animator = roleBehaviorLogic.GetComponent<Animator>();
   }

   public abstract void OnEnter();
   public abstract void OnUpdate();
   public abstract void OnExit();
}
