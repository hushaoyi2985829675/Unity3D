using System;
using System.Collections;
using System.Collections.Generic;
using RoleNs;
using UnityEngine;

public class AnimationAI : MonoBehaviour
{
    CharacterAIBase characterAI;
    Animator animator;
    private Vector3 velocity;
    private RoleInfo roleInfo;
    private void Awake()
    {
        characterAI = GetComponent<CharacterAIBase>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        velocity = characterAI.GetVelocity();
        roleInfo = characterAI.GetRoleInfo();
        animator.SetBool("Ground", characterAI.isGround);
        animator.SetFloat("VelocityZ", velocity.z / roleInfo.moveSpeed);
        animator.SetFloat("VelocityX", velocity.x / roleInfo.moveSpeed);
        animator.SetFloat("VelocityZAbs", Math.Abs(velocity.z) / roleInfo.moveSpeed);
        animator.SetFloat("AttackSpeed", roleInfo.attackSpeed);
        animator.SetInteger("AttackStage", characterAI.GetAttackStage());
    }
}
