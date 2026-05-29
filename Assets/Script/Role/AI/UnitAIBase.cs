using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitAIBase : MonoBehaviour
{
    //组件
    protected Animator animator;
    public NavMeshAgent navMeshAgent;
    //属性
    protected Vector3 velocity;
    [Header("目标")]
    [SerializeField]
    protected GameObject target;
    [SerializeField]
    protected string targetSearchLayer = "Monster";
    [SerializeField]
    public bool isGround;
    [SerializeField]
    protected int attackStage = 1;
    public float ReactValue = 80;
    protected RaycastHit raycastHit;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    protected virtual void Start()
    {
    }
    protected virtual void Update()
    {
        // 当前目标为空，或已超出检测范围时，重新搜索最近目标
        if (target == null || !FightTool.IsTargetInRange(transform, 10f, target))
        {
            target = FightTool.FindNearestTargetByLayer(transform, 10f, targetSearchLayer);
        }

        if (navMeshAgent != null)
        {
            velocity = transform.InverseTransformDirection(navMeshAgent.velocity);
        }
        CheckRaycastr();
    }

    protected void CheckRaycastr()
    {
        // 从角色中心点发射向下射线
        raycastHit = FightTool.CheckRaycastHitByLayer(transform.position, Vector3.down, 0.5f, "Ground");
        isGround = raycastHit.collider != null;
    }

    //获取导航速度
    public Vector3 GetVelocity()
    {
        return velocity;
    }

    //是否发现敌人
    public GameObject GetMonsterTarget()
    {
        return target;
    }

    //攻击段数
    public void SetAttackStage(int stage)
    {
        attackStage = stage;
    }

    public int GetAttackStage()
    {
        return attackStage;
    }

    #region 导航操作

    //移动
    public void Move(Vector3 pos)
    {
        navMeshAgent.SetDestination(pos);
    }

    //是否到达
    public bool CheckIfReachedDestination()
    {
        if (navMeshAgent == null || !navMeshAgent.enabled)
        {
            return true;
        }

        if (navMeshAgent.pathPending)
        {
            return false;
        }

        // 给一个小容差，避免 Root Motion 由于步幅离散在终点附近反复绕圈。
        float reachThreshold = Mathf.Max(navMeshAgent.stoppingDistance, 0.25f);
        return navMeshAgent.remainingDistance <= reachThreshold;
    }

    public void StopMove()
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.ResetPath();
        navMeshAgent.isStopped = false;
        navMeshAgent.velocity = Vector3.zero;
    }

    //设置导航速度
    public void SetNavSpeed(float speed)
    {
        navMeshAgent.speed = speed;
    }

    //设置停止距离
    public void SetStopDistance(float distance)
    {
        navMeshAgent.stoppingDistance = distance;
    }

    public void SetNavMove(Vector3 pos)
    {
        navMeshAgent.Move(pos);
    }

    public void SetApplyRootMotion(bool apply)
    {
        animator.applyRootMotion = apply;
    }

    public void PlayAnimation(string animationName)
    {
        animator.Play(animationName);
    }

    public void SetAnimatorFloat(string triggerName, float value)
    {
      animator?.SetFloat(triggerName, value);
    }

    #endregion

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = target == null ? Color.red : Color.green;
        // 可选：绘制中心点（人物位置）
        Gizmos.DrawWireSphere(transform.position, 10f);
    }
}
