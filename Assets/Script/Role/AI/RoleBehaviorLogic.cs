using System;
using System.Collections;
using System.Collections.Generic;
using RoleNs;
using UnityEngine;

public class RoleState
{
    public Action OnEnter;
    public Action OnUpdate;
    public Action OnExit;

    public RoleState(Action onEnter, Action onUpdate, Action onExit)
    {
        OnEnter = onEnter;
        OnUpdate = onUpdate;
        OnExit = onExit;
    }

    public void Enter()
    {
        OnEnter?.Invoke();
    }

    public void Update()
    {
        OnUpdate?.Invoke();
    }

    public void Exit()
    {
        OnExit?.Invoke();
    }
}

public class RoleBehaviorLogic : MonoBehaviour
{
    private CharacterAIBase characterAI;
    private Animator animator;
    private AnimationEvent animationEvent;
    //状态机
    private Dictionary<PlayerState, RoleState> roleStateList;

    [SerializeField]
    private PlayerState curState = PlayerState.Idle;

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

    //攻击间隔
    [Header("攻击间隔")]
    [SerializeField]
    private float attackInterval = 0.5f;
    private float localAttackInterval = 0;
    private bool isAttack;

    private void Awake()
    {
        roleStateList = new Dictionary<PlayerState, RoleState>();
        characterAI = GetComponent<CharacterAIBase>();
        animator = GetComponent<Animator>();
        animationEvent = GetComponent<AnimationEvent>();
    }

    void Start()
    {
        roleInfo = characterAI.GetRoleInfo();
        idleTime = roleInfo.idleTime;
        roleStateList = new Dictionary<PlayerState, RoleState>();
        roleStateList.Add(PlayerState.Idle,new RoleState(IdleEnter, IdleUpdate, IdleExit));
        roleStateList.Add(PlayerState.Walk,new RoleState(OnWalkEnter, OnWalkUpdate, OnWalkExit));
        roleStateList.Add(PlayerState.Chase,new RoleState(ChaseEnter, ChaseUpdate, ChaseExit));
        roleStateList.Add(PlayerState.Attack,new RoleState(AttackEnter, AttackUpdate, AttackExit));
        roleStateList[curState].Enter();
        animationEvent.AddAnimationEvent(OnAnimationAction);
    }

    private void Update()
    {
        if (localAttackInterval > 0f)
        {
            localAttackInterval -= Time.deltaTime;
            if (localAttackInterval <= 0f)
            {
                localAttackInterval = 0f;
                characterAI.SetAttackStage(1);
            }
        }
        ChangeState(playerState);
        roleStateList[curState].OnUpdate();
    }

    void OnAnimationAction(string eventName)
    {
        switch (eventName)
        {
            case "AttackEnd":
                isAttack = false;
                localAttackInterval = attackInterval;
                break;
        }
    }

    #region IdleSate

    //待机状态
    public virtual void IdleEnter()
    {
    }

    public virtual void IdleUpdate()
    {
        if (characterAI.GetMonsterTarget() != null)
        {
            playerState = PlayerState.Chase;
            return;
        }
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
        characterAI.SetStopDistance(0);
        characterAI.Move(new Vector3(transform.TransformPoint(pos).x, pos.y, transform.TransformPoint(pos).z));
    }

    public virtual void OnWalkUpdate()
    {
        if (characterAI.GetMonsterTarget() != null)
        {
            playerState = PlayerState.Chase;
            return;
        }
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

    //追击状态

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
            characterAI.SetStopDistance(roleInfo.attackDic);
            characterAI.Move(target.transform.position);
            localChaseUpdateInterval = 0;
        }
    }

    public virtual void ChaseUpdate()
    {
        target = characterAI.GetMonsterTarget();
        if (target != null)
        {
            localChaseUpdateInterval = localChaseUpdateInterval + Time.deltaTime;
            //不要每帧更新
            if (localChaseUpdateInterval >= chaseUpdateInterval)
            {
                characterAI.Move(target.transform.position);
                localChaseUpdateInterval = 0;
            }
            if (FightTool.IsTargetInRange(transform, roleInfo.attackDic, target))
            {
                //攻击
                playerState = PlayerState.Attack;
            }
        }
        else
        {
            playerState = PlayerState.Idle;
        }
    }

    public virtual void ChaseExit()
    {
        localChaseUpdateInterval = 0;
    }

    #endregion

    #region AttackState

    //攻击状态

    public virtual void AttackEnter()
    {

    }

    public virtual void AttackUpdate()
    { 
        if(!isAttack)
        {
            if (FightTool.IsTargetInRange(transform, roleInfo.attackDic, target))
            {
                isAttack = true;
                if (localAttackInterval > 0f && characterAI.GetAttackStage() < 2)
                {  
                    characterAI.SetAttackStage(characterAI.GetAttackStage() + 1);
                }
                else
                {
                    characterAI.SetAttackStage(1);
                }
                localAttackInterval = 0f;
            }
            else
            {
                playerState = PlayerState.Chase;
            }
        }
    }

    public virtual void AttackExit()
    {
        localAttackInterval = 0;
        isAttack = false;
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
            RoleState roleState = roleStateList[curState];
            roleState.OnExit();
        }

        curState = state;
        roleStateList[curState].OnEnter();
        animator.SetInteger("State", (int)curState);
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