using System;
using System.Collections;
using System.Collections.Generic;
using RoleNs;
using UnityEditor;
using UnityEngine;

public class RoleBehaviorLogic : MonoBehaviour
{
    private CharacterAIBase characterAI;
    private Animator animator;

    //状态机
    private Dictionary<PlayerState, RoleStateBase> roleStateList;

    [SerializeField]
    private PlayerState curState = PlayerState.Jump;

    //状态
    private PlayerState playerState;

    //属性
    protected float idleTime = 2;
    protected float localIdleTime;
    protected RoleInfo roleInfo;
    private GameObject target;
    
    //追击更新频率
    private float chaseUpdateInterval = 0.3f;
    private float localChaseUpdateInterval = 0;
    
    private void Awake()
    {
        roleStateList = new Dictionary<PlayerState, RoleStateBase>();
        characterAI = GetComponent<CharacterAIBase>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        roleInfo = characterAI.GetRoleInfo();
        idleTime = roleInfo.idleTime;
        roleStateList.Add(PlayerState.Idle, new RoleIdleState(this));
        roleStateList.Add(PlayerState.Walk, new RoleWalkState(this));
        roleStateList.Add(PlayerState.Chase, new RoleChaseState(this));
    }

    private void Update()
    {
        if (characterAI.GetMonsterTarget() != null)
        {
            playerState = PlayerState.Chase;
        }
        ChangeState(playerState);
        roleStateList[curState].OnUpdate();
    }

    #region IdleSate

    //待机状态
    public virtual void IdleEnter()
    {
    }

    public virtual void IdleUpdate()
    {
        localIdleTime += Time.deltaTime;
        if (localIdleTime >= idleTime)
        {
            playerState = PlayerState.Walk;
        }
    }

    public virtual void IdleExit()
    {
        localIdleTime = 0;
    }

    #endregion

    #region WalkSate

    public virtual void OnWalkEnter()
    {
        //随机出一个坐标
        Vector3 pos = Ui.Instance.GetRandomPointInCircle(transform.position, 10);
        characterAI.SetNavSpeed(roleInfo.moveSpeed);
        // characterAI.SetStopDistance(0);
        characterAI.Move(new Vector3(transform.TransformPoint(pos).x, pos.y, transform.TransformPoint(pos).z));
    }

    public virtual void OnWalkUpdate()
    {
        if (characterAI.CheckIfReachedDestination())
        {
            playerState = PlayerState.Idle;
        }
    }

    public virtual void OnWalkExit()
    {
        characterAI.StopMove();
    }

    #endregion

    #region ChaseState

    //待机状态
    public virtual void ChaseEnter()
    {
        target = characterAI.GetMonsterTarget();
        if (target == null)
        {
            playerState = PlayerState.Idle;
        }
        else
        {
            characterAI.SetNavSpeed(roleInfo.runSpeed);
            // characterAI.SetStopDistance(roleInfo.attackDic);
            characterAI.Move(target.transform.position);
            localChaseUpdateInterval = 0;
        }
    }

    public virtual void ChaseUpdate()
    {
        target = characterAI.GetMonsterTarget();
        if (target != null)
        {
            //不要每帧更新
            if (localChaseUpdateInterval >= chaseUpdateInterval)
            {
                characterAI.Move(target.transform.position);
            }
            if (characterAI.CheckIfReachedDestination())
            { 
                //攻击
                playerState = PlayerState.Attack;
                localChaseUpdateInterval = 0;
            }
        }
        else
        {
            playerState = PlayerState.Idle;
        }
    }

    public virtual void ChaseExit()
    {
    }

    #endregion

    //切换状态
    public virtual void ChangeState(PlayerState state)
    {
        if (state == curState)
        {
            return;
        }

        if (roleStateList.ContainsKey(curState))
        {
            RoleStateBase roleStateBase = roleStateList[curState];
            roleStateBase.OnExit();
        }

        curState = state;
        roleStateList[curState].OnEnter();
        animator.SetInteger("State", (int) curState);
    }

    // void OnDrawGizmos()
    // {
    //     // if (!Application.isPlaying)
    //     // {
    //     //     return;
    //     // }
    //     Gizmos.color = Color.blue;
    //     // 可选：绘制中心点（人物位置）
    //     Gizmos.DrawWireSphere(transform.position, 8f);
    // }
}