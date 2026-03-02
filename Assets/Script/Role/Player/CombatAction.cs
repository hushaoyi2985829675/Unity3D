using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatAction : MonoBehaviour
{
   private CharacterState  characterState;
   private AnimationEvent  animationEvent;
   private RoleCamp roleCamp;
   private InputEvent inputEvent;
   [Header("攻击段数")]
   [SerializeField]
   private int attackStage = 0;
   [Header("攻击间隔")]
   [SerializeField]
   private float attackTime;
   private float localAttackTime;
   [Header("眩晕时长")]
   [SerializeField]
   private float dizzyTime;
   private float localDizzyTime;
   private bool isAttacking = false;
   private bool isSKill;
   
   private void Awake()
   {
      characterState =  GetComponent<CharacterState>();
      animationEvent = GetComponent<AnimationEvent>();
      inputEvent = GetComponent<InputEvent>();
      roleCamp = characterState.GetRoleCamp();
   }

   private void Start()
   {
      animationEvent.AddAnimationEvent(OnAttackAniAction); 
      animationEvent.RegisterAnimationEvent("Hit",OnHitAction);
      inputEvent.swordShieldEvent += OnSwordShieldAction;
      inputEvent.attackEvent += OnAttackAction;
      localAttackTime = attackTime;
   }

   private void Update()
   {
      if (attackStage != 0)
      {
         if (!isAttacking)
         {
            localAttackTime -= Time.deltaTime;
            if (localAttackTime <= 0)
            {
               attackStage = 0;
            }
         }
      }

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
   void OnSwordShieldAction(InputValue inputValue)
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
   void OnAttackAction(InputValue inputValue)
   {
      if (isAttacking)
      {
         return;
      }
      // EditorApplication.isPaused = true;
      if (characterState.GetGround())
      {
         attackStage += 1;
      }
      else
      {
         attackStage = 0;
      }
      characterState.SetAttackStage(attackStage);
      isAttacking = true;
      characterState.AddPlayerState(PlayerState.Attack);
   }

   //攻击动画
   void OnAttackAniAction(string eventName)
   {
      switch (eventName)
      {
         case "AttackEnd":
            isAttacking = false;
            localAttackTime = attackTime;
            characterState.RemovePlayerState(PlayerState.Attack);
            if (attackStage == 2)
            {
               attackStage = 0;
               characterState.SetAttackStage(attackStage);
            }
            break;
         case "AttackJumpEnd":
            isAttacking = false;
            localAttackTime = attackTime;
            characterState.RemovePlayerState(PlayerState.Attack);
            break;
         case "ComboEnd":
            isSKill = false;
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
      if (isSKill || characterState.GetPlayerState() == PlayerState.Jump)
      {
         return;
      }
      if (characterState.GetPlayerState() == PlayerState.Attack)
      {
         characterState.RemovePlayerState(PlayerState.Attack);
      }
      isSKill = true;
      characterState.AddPlayerState(PlayerState.Skill);
      animationEvent.PostAnimationEvent("ComboTrigger");
   }

   void OnHitAction(string eventName)
   {
      PlayerState playerState = characterState.GetPlayerState();
      if (playerState == PlayerState.SwordShield)
      {
         animationEvent.PostAnimationEvent("HitTrigger");
      }
      else
      {
         if (playerState != PlayerState.Skill)
         {
            //受伤动画打断其他动画
            characterState.ClearState();
            characterState.AddPlayerState(PlayerState.Hit);
            animationEvent.PostAnimationEvent("HitTrigger");
         } 
      }
   }
}
