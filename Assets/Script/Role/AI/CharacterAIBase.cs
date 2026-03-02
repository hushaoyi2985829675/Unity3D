using System;
using System.Collections;
using System.Collections.Generic;
using RoleNs;
using UnityEngine;
using UnityEngine.AI;

public class CharacterAIBase : MonoBehaviour
{
    //角色相关
    [SerializeField]
    private int roleId;
    private RoleCamp roleCamp;
    protected RoleInfo  roleInfo;
    //脚本
    private Animator animator;
    private RoleBehaviorLogic roleBehaviorLogic;
    //状态
    public NavMeshAgent navMeshAgent;
    private bool isFoundMonster;
    //属性
    private Vector3 velocity;
    [Header("目标")]
    [SerializeField]
    private GameObject target;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        InitRole(roleId);
    }
    
    void Start()
    {
        
    }
    
    public void InitRole(int id)
    {
        roleId = id;
        roleInfo = ConfigManager.Instance.GetRoleInfoById(id);
        SetNavSpeed(roleInfo.moveSpeed);
    }

    private void Update()
    {
        target = FightTool.FindNearestTargetByLayer(transform, 10f, "Monster");
        velocity = transform.InverseTransformDirection(navMeshAgent.velocity);
    }

    public RoleInfo GetRoleInfo()
    {
        return roleInfo;
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

    #region 导航操作

    //移动
    public void Move(Vector3 pos)
    {
        navMeshAgent.SetDestination(pos);
    }
    
    //是否到达
    public bool CheckIfReachedDestination()
    {
        return !navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance;
    }

    public void StopMove()
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.ResetPath();
        navMeshAgent.isStopped = false;
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

    #endregion
    void OnDrawGizmos()
    {
        // if (!Application.isPlaying)
        // {
        //     return;
        // }
        Gizmos.color = target == null ? Color.green : Color.red;
        // 可选：绘制中心点（人物位置）
        Gizmos.DrawWireSphere(transform.position, 10f);
    }
}
