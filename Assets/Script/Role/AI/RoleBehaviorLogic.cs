using System;
using System.Runtime.InteropServices;
using RoleNs;
using UnityEngine;

public class RoleBehaviorLogic : UnitBehaviorLogicBase
{
    private CharacterAIBase characterAI;
    private RoleInfo roleInfo;

    private float rollSpped = 3;
    private bool isRolling = false;
    
    protected override void Awake()
    {
        base.Awake();
        characterAI = GetComponent<CharacterAIBase>();
    }

    protected override void Start()
    {
        animationEvent.AddAnimationEvent(OnRoleAnimationAction);
        roleInfo = characterAI.GetRoleInfo();
        base.Start();
        unitStateList.Add(PlayerState.Roll, new UnitState(OnRollEnter, OnRollUpdate, OnRollExit));
    }
    
    protected override void Update()
    {
        base.Update();
    }

    private void OnRoleAnimationAction(string eventName)
    {
        switch (eventName)
        {
            case "RollMoveStart":
               isRolling = true;
                break;
            case "RollMoveEnd":
                isRolling = false;
                break;
            case "RollEnd":
                SetPlayerState(PlayerState.Idle);
                break;
        }
    }
    private void OnRollEnter()
    {
        
    }

    private void OnRollUpdate()
    {
        if(isRolling)
        {
            int dir = characterAI.GetRollDirection();
            Vector3 moveDir = Vector3.zero;
            switch(dir)
            {
                case 1:
                    moveDir = new Vector3(0, 0, -1);
                    break;
                case 2:
                    moveDir = new Vector3(1, 0, 0);
                    break;
                case 3:
                    moveDir = new Vector3(-1, 0, 0);
                    break;
            }
            Vector3 move = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0)) * moveDir;
            characterAI.SetNavMove(move * (rollSpped * Time.deltaTime));
        }
    }


    //滚动
    private void OnRollExit()
    {
        isRolling = false;
    }
    protected override float GetIdleTime()
    {
        return roleInfo.idleTime;
    }

    protected override float GetMoveSpeed()
    {
        return roleInfo.moveSpeed;
    }

    protected override float GetRunSpeed()
    {
        return roleInfo.runSpeed;
    }

    protected override float GetAttackDistance()
    {
        return roleInfo.attackDic;
    }
    public override void TriggerHit()
    {
        base.TriggerHit();
        if (playerState == PlayerState.SwordShield)
        {
            //扣血
            return;
        }
        if (playerState == PlayerState.Skill) 
        {
            //扣血
            return;
        }

        if(playerState == PlayerState.Roll)
        {
            return;
        }
            //判断是否进入受伤动画
        if(!isAttackDetect || localHitInterval <= 0)
        {
            playerState = PlayerState.Hit;
        }
    }

    
      //面对攻击时做出反应
    public override void ReactToDanger()
    {
        if(isAttackDetect)
        {
            return;
        }
        //随机是否进行反应
        if(UnityEngine.Random.Range(0, 100) > characterAI.ReactValue)
        {
            return;
        }
        //随机一个方向进行翻滚
        int rollDirection = UnityEngine.Random.Range(1, 4);
        characterAI.SetRollDirection(rollDirection);
        SetPlayerState(PlayerState.Roll);
    }
}