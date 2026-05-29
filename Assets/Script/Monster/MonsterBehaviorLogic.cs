using MonsterNs;
using UnityEngine;

public class MonsterBehaviorLogic : UnitBehaviorLogicBase
{
    private MonsterAIBase monsterAI;
    private MonsterInfo monsterInfo;

    protected override void Awake()
    {
        base.Awake();
        monsterAI = GetComponent<MonsterAIBase>();
    }

    protected override void Start()
    {
        monsterInfo = monsterAI.GetMonsterInfo();
        base.Start();
    }

    protected override float GetIdleTime()
    {
        return monsterInfo.idleTime;
    }

    protected override float GetMoveSpeed()
    {
        return monsterInfo.moveSpeed;
    }

    protected override float GetRunSpeed()
    {
        return monsterInfo.runSpeed;
    }

    protected override float GetAttackDistance()
    {
        return monsterInfo.attackDic;
    }
   
    public override void OnWalkEnter()
    {
        base.OnWalkEnter();
        unitAI.SetApplyRootMotion(true);
    }
   public override void OnWalkExit()
    {
        base.OnWalkExit();
        unitAI.SetApplyRootMotion(false);
    }
    
    public override void ChaseEnter()
    {
        base.ChaseEnter();
        unitAI.SetApplyRootMotion(true);
    }
    public override void ChaseExit()
    {
        base.ChaseExit();
        unitAI.SetApplyRootMotion(false);
    }
    private void OnAnimatorMove()
    {
       transform.position += animator.deltaPosition;
       monsterAI.navMeshAgent.nextPosition = transform.position;
    }
   

}
