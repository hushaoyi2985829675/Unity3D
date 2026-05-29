using System;
using RoleNs;
using UnityEngine;

public class RoleAnimation : MonoBehaviour
{
    private CharacterAIBase characterAI;
    private Animator animator;
    private Vector3 velocity;
    private RoleInfo roleInfo;

    private void Awake()
    {
        characterAI = GetComponent<CharacterAIBase>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (characterAI == null || animator == null)
        {
            return;
        }

        roleInfo = characterAI.GetRoleInfo();
        if (roleInfo == null || roleInfo.moveSpeed == 0)
        {
            return;
        }

        velocity = characterAI.GetVelocity();
        animator.SetBool("Ground", characterAI.isGround);
        animator.SetFloat("VelocityZ", velocity.z / roleInfo.moveSpeed);
        animator.SetFloat("VelocityX", velocity.x / roleInfo.moveSpeed);
        animator.SetFloat("VelocityZAbs", Math.Abs(velocity.z) / roleInfo.moveSpeed);
        animator.SetFloat("AttackSpeed", roleInfo.attackSpeed);
        animator.SetInteger("AttackStage", characterAI.GetAttackStage());
        animator.SetInteger("RollDirection", characterAI.GetRollDirection());
    }
}
