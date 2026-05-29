using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
   private const int MaxAttackStage = 2;

   private CharacterState  characterState;
   private AnimationEvent  animationEvent;
   private RoleCamp roleCamp;
   [Header("攻击间隔")]
   [SerializeField]
   private float attackTime;
   private float localAttackTime;
   [Header("眩晕时长")]
   [SerializeField]
   private float dizzyTime;
   private float localDizzyTime;
   private bool isAttacking = false;
   private bool isSkill;
   
   private void Awake()
   {
      characterState =  GetComponent<CharacterState>();
      animationEvent = GetComponent<AnimationEvent>();
      roleCamp = characterState.GetRoleCamp();
   }

   private void Start()
   {
      animationEvent.AddAnimationEvent(OnAttackAniAction); 
      localAttackTime = attackTime;
   }

   private void Update()
   {
      TryResetAttackByTimeout();

      if (characterState.GetPlayerState() == PlayerState.Dizzy)
      {
         localDizzyTime += Time.deltaTime;
         if (localDizzyTime >= dizzyTime)
         {
            //眩晕结束
            characterState.SetDizzy(false);
            characterState.RemovePlayerState(PlayerState.Dizzy);
            localDizzyTime = 0;
         }
      }
   }

   //k键逻辑
   void OnSwordShieldInput(InputValue inputValue)
   {
      float value = inputValue.Get<float>();
      if (roleCamp ==  RoleCamp.SwordShield)
      {
         if (Mathf.Approximately(value, 1))
         {
            characterState.AddPlayerState(PlayerState.SwordShield);
         }
         else
         {
            characterState.RemovePlayerState(PlayerState.SwordShield);
         }
      }
   }
   //攻击逻辑
   void OnAttackInput(InputValue inputValue)
   {
      if (isAttacking) return;

      int nextStage = characterState.GetGround()
         ? characterState.GetAttackStage() + 1
         : 0;
      characterState.SetAttackStage(nextStage);
      isAttacking = true;
      characterState.AddPlayerState(PlayerState.Attack);
   }

   /// <summary> 超时未连击则重置攻击段数。 </summary>
   void TryResetAttackByTimeout()
   {
      if (characterState.GetAttackStage() == 0 || isAttacking) return;
      localAttackTime -= Time.deltaTime;
      if (localAttackTime <= 0)
         ResetAttackStage();
   }

   /// <summary> 结束当前攻击表现并重启连击计时。 </summary>
   void EndCurrentAttack()
   {
      isAttacking = false;
      localAttackTime = attackTime;
      characterState.RemovePlayerState(PlayerState.Attack);
   }

   /// <summary> 重置攻击段数为 0（落地未连击 / 连击结束 / 超时）。 </summary>
   void ResetAttackStage()
   {
      characterState.SetAttackStage(0);
   }

   //攻击动画
   void OnAttackAniAction(string eventName)
   {
      switch (eventName)
      {
         case "AttackEnd":
            EndCurrentAttack();
            if (characterState.GetAttackStage() >= MaxAttackStage)
               ResetAttackStage();
            break;
         case "AttackJumpEnd":
            EndCurrentAttack();
            break;
         case "ComboEnd":
            isSkill = false;
            characterState.RemovePlayerState(PlayerState.Skill);
            break;
         case "HitEnd":
            characterState.RemovePlayerState(PlayerState.Hit);
            if (characterState.GetDizzy())
            {
               characterState.AddPlayerState(PlayerState.Dizzy);
            }
            break;
      }
   }
   //技能按键
   void OnComboInput(InputValue inputValue)
   {
      PlayerState state = characterState.GetPlayerState();
      if (isSkill || state == PlayerState.Jump) return;
      if (state == PlayerState.Attack)
         characterState.RemovePlayerState(PlayerState.Attack);
      isSkill = true;
      characterState.AddPlayerState(PlayerState.Skill);
      animationEvent.PostAnimationEvent("ComboTrigger");
      characterState.SetAnimatorTrigger("ComboTrigger");
   }

   void OnHitAction()
   {
      PlayerState state = characterState.GetPlayerState();
      if (state == PlayerState.SwordShield)
      {
         characterState.SetAnimatorTrigger("HitTrigger");
         return;
      }
      if (state == PlayerState.Skill) return;

      // 受伤动画打断其他动画
      characterState.ClearState();
      characterState.AddPlayerState(PlayerState.Hit);
      characterState.SetAnimatorTrigger("HitTrigger");
   }
   public void OnHitInput()
   {
      OnHitAction();
   }
}
