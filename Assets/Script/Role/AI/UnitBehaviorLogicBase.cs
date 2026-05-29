using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitState
{
    public Action OnEnter;
    public Action OnUpdate;
    public Action OnExit;

    public UnitState(Action onEnter, Action onUpdate, Action onExit)
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

public abstract class UnitBehaviorLogicBase : MonoBehaviour
{
    protected UnitAIBase unitAI;
    protected Animator animator;
    protected AnimationEvent animationEvent;
    protected WeaponController weaponController;
    protected Dictionary<PlayerState, UnitState> unitStateList;

    [SerializeField]
    private PlayerState curState = PlayerState.Idle;

    public PlayerState playerState;

    protected float idleTime = 2;
    protected float localIdleTime;
    protected GameObject target;
    protected bool isAttackDetect = false;
    [SerializeField]
    private float minHitInterval = 0.5f;
    protected float localHitInterval = 0;
    private float chaseUpdateInterval = 0.3f;
    private float localChaseUpdateInterval = 0;

    [SerializeField]
    private float turnSpeed = 10f;
    private bool isAttack;

    protected virtual void Awake()
    {
        unitStateList = new Dictionary<PlayerState, UnitState>();
        unitAI = GetComponent<UnitAIBase>();
        animator = GetComponent<Animator>();
        animationEvent = GetComponent<AnimationEvent>();
        weaponController = Ui.Instance.GetComponentByChild<WeaponController>(transform);
    }

    protected virtual void Start()
    {
        idleTime = GetIdleTime();
        unitStateList = new Dictionary<PlayerState, UnitState>();
        unitStateList.Add(PlayerState.Idle, new UnitState(IdleEnter, IdleUpdate, IdleExit));
        unitStateList.Add(PlayerState.Walk, new UnitState(OnWalkEnter, OnWalkUpdate, OnWalkExit));
        unitStateList.Add(PlayerState.Chase, new UnitState(ChaseEnter, ChaseUpdate, ChaseExit));
        unitStateList.Add(PlayerState.Attack, new UnitState(AttackEnter, AttackUpdate, AttackExit));
        unitStateList.Add(PlayerState.Hit, new UnitState(HitEnter, HitUpdate, HitExit));
        unitStateList.Add(PlayerState.Dizzy, new UnitState(DizzyEnter, DizzyUpdate, DizzyExit));
        unitStateList[curState].Enter();
        if (animationEvent != null)
        {
            animationEvent.AddAnimationEvent(OnAnimationAction);
        }
        OnLogicStart();
    }

    protected virtual void Update()
    {
        if(localHitInterval > 0)
        {
            localHitInterval -= Time.deltaTime;
        }
        ChangeState(playerState);
        unitStateList[curState].OnUpdate();
    }

    protected virtual void OnAnimationAction(string eventName)
    {
        switch (eventName)
        {
            case "AttackStart":
                
                break;
            case "AttackEnd":
                isAttack = false;
                break;
            case "AttackDetectStatr":
                isAttackDetect = true;
                weaponController?.SetDetectAttack(true);
                break;
            case "AttackDetectEnd":
                isAttackDetect = false;
                weaponController?.SetDetectAttack(false);
                break;
            case "HitEnd":
                OnHitAnimationEnd();
                break;
        }
    }

    protected virtual void OnLogicStart() { }
    protected abstract float GetIdleTime();
    protected abstract float GetMoveSpeed();
    protected abstract float GetRunSpeed();
    protected abstract float GetAttackDistance();

    protected void SetPlayerState(PlayerState state)
    {
        playerState = state;
    }

    #region IdleState

    public virtual void IdleEnter()
    {
        unitAI.PlayAnimation("Idle");
    }

    public virtual void IdleUpdate()
    {
        //追击,攻击
        target = unitAI.GetMonsterTarget();
        if (target != null)
        {
             if (FightTool.IsTargetInRange(transform, GetAttackDistance(), target))
            {
                 FightTool.LookAtTarget(transform, target.transform, turnSpeed, () =>
                {
                   playerState = PlayerState.Attack;
                });
            }
            else
            {
                playerState = PlayerState.Chase;
            }
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

    #region WalkState

    public virtual void OnWalkEnter()
    {

        Vector3 pos = Ui.Instance.GetRandomPointInCircle(transform.position, 10);
        unitAI.SetNavSpeed(GetMoveSpeed());
        // Root Motion 下需要一个最小停止半径，避免在终点附近来回纠偏转圈。
        unitAI.SetStopDistance(0.25f);
        unitAI.Move(new Vector3(transform.TransformPoint(pos).x, pos.y, transform.TransformPoint(pos).z));
    }

    public virtual void OnWalkUpdate()
    {
        unitAI.SetAnimatorFloat("Velocity", unitAI.GetVelocity().magnitude / GetMoveSpeed());
        unitAI.PlayAnimation("Move");
        if (unitAI.GetMonsterTarget() != null)
        {
            playerState = PlayerState.Chase;
            return;
        }
        if (unitAI.CheckIfReachedDestination())
        {
            playerState = PlayerState.Idle;
        }
    }

    public virtual void OnWalkExit()
    {
        unitAI.StopMove();
    }

    #endregion

    #region ChaseState

    public virtual void ChaseEnter()
    {
        target = unitAI.GetMonsterTarget();
        if (target == null)
        {
            playerState = PlayerState.Idle;
        }
        else
        {
            
            unitAI.SetNavSpeed(GetRunSpeed());
            unitAI.SetStopDistance(GetAttackDistance());
            unitAI.Move(target.transform.position);
            localChaseUpdateInterval = 0;
        }
    }

    public virtual void ChaseUpdate()
    {
        target = unitAI.GetMonsterTarget();
        if (target != null)
        {
            unitAI.SetAnimatorFloat("Velocity", unitAI.GetVelocity().magnitude / GetMoveSpeed());
            unitAI.PlayAnimation("Chase");
            localChaseUpdateInterval = localChaseUpdateInterval + Time.deltaTime;
            if (localChaseUpdateInterval >= chaseUpdateInterval)
            {
                unitAI.Move(target.transform.position);
                localChaseUpdateInterval = 0;
            }
            if (FightTool.IsTargetInRange(transform, GetAttackDistance(), target))
            {
                playerState = PlayerState.Idle;
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

    public virtual void AttackEnter()
    {
        unitAI.StopMove();
        unitAI.SetAttackStage(1);
        isAttack = true;
    }

    public virtual void AttackUpdate()
    {
        if(!isAttack)
        {
            int stage = unitAI.GetAttackStage();
            if(stage < 2)
            {
                stage++;
                unitAI.SetAttackStage(stage);
                unitAI.PlayAnimation("Attack");
                isAttack = true;
            }
            else
            {
                 playerState = PlayerState.Idle;
            }
        }
    }

    public virtual void AttackExit()
    {
        isAttack = false;
    }

    #endregion
    
    #region HitState

    // TODO: 受击状态进入逻辑（如停移动、播放受击参数等）
    public virtual void HitEnter()
    {  
        //播放受伤还是眩晕
    }

    // TODO: 受击状态每帧逻辑（如计时、转回 Idle/Chase）
    public virtual void HitUpdate()
    {
    }

    // TODO: 受击状态退出逻辑（如清理标记）
    public virtual void HitExit()
    {
        localHitInterval = minHitInterval;
    }
    // 对外触发入口：其他组件命中时调用。
    public virtual void TriggerHit()
    {
       
      
        
        //TODO: 扣血
        //TODO: 刷新UI
        // 
    }

    private void OnHitAnimationEnd()
    {
        //是否眩晕
        playerState = PlayerState.Idle;
    }
    #endregion

    #region DizzyState

    public virtual void DizzyEnter()
    {
    }

    public virtual void DizzyUpdate()
    {
    }

    public virtual void DizzyExit()
    {
    }

    #endregion

    public virtual void ChangeState(PlayerState state)
    {
        if (state == curState)
        {
            return;
        }

        if (unitStateList.ContainsKey(curState))
        {
            UnitState unitState = unitStateList[curState];
            unitState.OnExit();
        }

        curState = state;
        unitStateList[curState].OnEnter();
        // animator.SetInteger("State", (int)curState);
    }

    //面对攻击时做出反应
    public virtual void ReactToDanger()
    {
        
    }
}
