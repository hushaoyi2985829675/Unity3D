using System;
using MonsterNs;
using UnityEngine;

public class MonsterAnition : MonoBehaviour
{
    private MonsterAIBase monsterAI;
    private Animator animator;
    private Vector3 velocity;
    private MonsterInfo monsterInfo;

    private void Awake()
    {
        monsterAI = GetComponent<MonsterAIBase>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (monsterAI == null || animator == null)
        {
            return;
        }

        monsterInfo = monsterAI.GetMonsterInfo();
        if (monsterInfo == null || monsterInfo.moveSpeed == 0)
        {
            return;
        }

        velocity = monsterAI.GetVelocity();
        animator.SetBool("Ground", monsterAI.isGround);
        animator.SetFloat("VelocityZ", velocity.z / monsterInfo.moveSpeed);
        animator.SetFloat("VelocityX", velocity.x / monsterInfo.moveSpeed);
        animator.SetFloat("VelocityZAbs", Math.Abs(velocity.z) / monsterInfo.moveSpeed);
        animator.SetFloat("AttackSpeed", monsterInfo.attackSpeed);
        animator.SetInteger("AttackStage", monsterAI.GetAttackStage());
    }
}
