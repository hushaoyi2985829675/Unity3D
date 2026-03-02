using System;
using System.Collections;
using System.Collections.Generic;
using RoleNs;
using UnityEngine;

public class Animations : MonoBehaviour
{
    private CharacterState characterState;
    private RoleInfo roleInfo;
    private Animator animator;
    private bool isGround;
    private bool lastGround;
    private Vector3 velocity;
    private PlayerState playerState;
    private AnimationEvent animationEvent;
    private void Awake()
    {
        characterState = GetComponent<CharacterState>();
        animator = GetComponentInChildren<Animator>();
        animationEvent = GetComponent<AnimationEvent>();
    }

    private void Start()
    {
        roleInfo = characterState.GetRoleInfo();
        animationEvent.RegisterAnimationEvent("JumpTrigger",OnTrigger);
        animationEvent.RegisterAnimationEvent("ComboTrigger",OnTrigger);
        animationEvent.RegisterAnimationEvent("HitTrigger",OnTrigger);
    }

    private void Update()
    {
        playerState = characterState.GetPlayerState();
        velocity = characterState.GetVelocity();
        isGround = characterState.GetGround();
        lastGround = characterState.GetLastGround();
        animator.SetBool("Ground", isGround);
        animator.SetBool("LastGround", lastGround);
        if (isGround && !lastGround && playerState == PlayerState.Jump)
        {
            animator.SetTrigger("JumpEndTrigger");
        }
        animator.SetFloat("VelocityZ", velocity.z / roleInfo.moveSpeed);
        animator.SetFloat("VelocityX", velocity.x / roleInfo.moveSpeed);
        animator.SetFloat("VelocityZAbs", Math.Abs(velocity.z) / roleInfo.moveSpeed);
        animator.SetFloat("VelocityXAbs", Math.Abs(velocity.x) / roleInfo.moveSpeed);
        int attackStage = characterState.GetAttackStage();
        animator.SetInteger("AttackStage",attackStage);
        animator.SetInteger("State",(int)playerState);
    }

    private void OnTrigger(string eventName)
    {
        animator.SetTrigger(eventName);
    }
}
