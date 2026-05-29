using System;
using System.Collections;
using System.Collections.Generic;
using RoleNs;
using UnityEngine;


public enum PlayerState
{
   Idle = 0,
   Walk,
   SwordShield,
   Jump,
   Attack = 7,
   Skill,
   Hit,
   Dizzy,
   Slide = 11,
   Chase = 12,
   Roll = 13,
}

public class CharacterState : MonoBehaviour
{
   //速度
   [SerializeField]
   private int roleId = 1;
   [Header("转向速度")]
   public int turnSpeed = 10;
   private RoleInfo roleInfo;
   public bool isGround;
   private bool lastGround;
   private bool isDizzy;
   private bool isHit;
   private Vector3 velocity;
   [SerializeField]
   private RoleCamp roleCamp;
   [SerializeField]
   private PlayerState playerState;
   [SerializeField]
   private List<PlayerState> playerStateList =  new List<PlayerState>();
   private Dictionary<PlayerState,int> statePriorityDict = new Dictionary<PlayerState,int>();
   private int attackStage = 0;
   private Animator animator;

   private void Awake()
   {
      animator = GetComponentInChildren<Animator>();
      statePriorityDict = new Dictionary<PlayerState,int>()
      {
         {PlayerState.Idle,0},
         {PlayerState.Walk,1 },
         {PlayerState.Chase,2},
         {PlayerState.Roll,3},
         {PlayerState.SwordShield,5 },
         {PlayerState.Jump,6 },
         {PlayerState.Attack,7 },
         {PlayerState.Hit,8 },
         {PlayerState.Dizzy,8 },
         {PlayerState.Skill,9 },
      };
      roleInfo = ConfigManager.Instance.GetRoleInfoById(roleId);
   }

   void Start()
   {
      
      playerState = PlayerState.Idle;
      playerStateList.Add(PlayerState.Idle);
   }
   
   //设置阵营
   public void SetRoleCamp(RoleCamp camp)
   {
      roleCamp = camp;
   }
   //获取阵营
   public RoleCamp GetRoleCamp()
   {
      return roleCamp;
   }
   #region 设置状态
   //速度
   public void SetVelocity(Vector3 v)
   {
      velocity = v;
   }
   public Vector3 GetVelocity()
   {
      return velocity;
   }
   //地面
   public void SetGround(bool isGround)
   {
      this.isGround = isGround;
   }
   //上一帧地面
   public void SetLastGround(bool isGround)
   {
      lastGround = isGround;
   }
   //获取地面
   public bool GetGround()
   {
      return isGround;
   }
   //获取上一帧地面
   public bool GetLastGround()
   {
      return lastGround;
   }
   //获取当前状态是否大于输入状态

   //添加玩家状态
   public void AddPlayerState(PlayerState state)
   {
      if (playerState == state || playerStateList.Contains(state))
      {
         return;
      }

      if (statePriorityDict[state] > statePriorityDict[playerState])
      {
         playerState = state;
      }
      int index = playerStateList.BinarySearch(state);
      if (index < 0)
      {
         index = ~index;
      }
      playerStateList.Insert(index, state);
   }

   public void RemovePlayerState(PlayerState state)
   {
      PlayerState newState = playerStateList.Find(x=>x == state);
      if (newState == PlayerState.Idle)
      {
         Debug.LogError("没有状态: " + state);
         return;
      }
      playerStateList.Remove(state);
      if (playerState == state)
      {
         playerState = playerStateList[^1];
      }
   }

   public PlayerState GetPlayerState()
   {
      return playerState;
   }
   //眩晕
   public void SetDizzy(bool isDizzy)
   {
       this.isDizzy =  isDizzy;
   }

   public bool GetDizzy()
   {
      return isDizzy;
   }

   //攻击段数
   public void SetAttackStage(int attackStage)
   {
      this.attackStage = attackStage;
   }
   public int GetAttackStage()
   {
      return attackStage;
   }
   #endregion
   
   public void SetIsHit(bool isHit)
   {
      this.isHit = isHit;
   }
   public bool GetIsHit()
   {
      return isHit;
   }
   public void ClearState()
   {
      for (int i = playerStateList.Count - 1; i >= 0; i--)
      {
         if (playerStateList[i] != PlayerState.Idle && playerStateList[i] != PlayerState.Walk)
         {
            playerStateList.RemoveAt(i);
         }
      }
      playerState = playerStateList[^1];
   }
   
   //获取属性
   public RoleInfo GetRoleInfo()
   {
      return roleInfo;
   }

   public void SetAnimatorTrigger(string triggerName)
   {
      if(string.IsNullOrEmpty(triggerName))
      {
         return;
      }
      animator?.SetTrigger(triggerName);
   }
   public void SetAnimatorFloat(string triggerName, float value)
   {
      animator?.SetFloat(triggerName, value);
   }
   public void SetAnimatorBool(string triggerName, bool value)
   {
      animator?.SetBool(triggerName, value);
   }
   public void PlayAnimation(string animationName, float transition = 0.25f)
   {
      // if(animator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
      // {
      //    return;
      // }
      animator.CrossFade(animationName, transition);
      animator?.Play(animationName);
   }
   private void LateUpdate()
   {
      // if (animator == null || roleInfo == null) return;
      // float moveSpeed = roleInfo.moveSpeed;
      // if (moveSpeed <= 0f) moveSpeed = 1f;
      // animator.SetBool("Ground", isGround);
      // animator.SetBool("LastGround", lastGround);
      // if (isGround && !lastGround && playerState == PlayerState.Jump)
      //    animator.SetTrigger("JumpEndTrigger");
      // animator.SetFloat("VelocityZ", velocity.z / moveSpeed);
      // animator.SetFloat("VelocityX", velocity.x / moveSpeed);
      // animator.SetFloat("VelocityZAbs", Mathf.Abs(velocity.z) / moveSpeed);
      // animator.SetFloat("VelocityXAbs", Mathf.Abs(velocity.x) / moveSpeed);
      // animator.SetInteger("AttackStage", attackStage);
      // animator.SetInteger("State", (int)playerState);
   }
}
