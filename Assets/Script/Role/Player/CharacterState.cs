using System;
using System.Collections;
using System.Collections.Generic;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using RoleNs;
using UnityEngine;


public enum PlayerState
{
   Idle = 0,
   Walk,
   WalkBack,
   WalkLeft,
   WalkRight,
   SwordShield,
   Jump,
   Attack = 7,
   Skill,
   Hit,
   Dizzy,
   Slide = 11,
   Chase = 12,
}

public class CharacterState : MonoBehaviour
{
   //速度
   [SerializeField]
   private int roleId = 1;
   private RoleInfo roleInfo;
   public bool isGround;
   private bool lastGround;
   private bool isDizzy;
   private Vector3 velocity;
   [SerializeField]
   private RoleCamp roleCamp;
   [SerializeField]
   private PlayerState playerState;
   [SerializeField]
   private List<PlayerState> playerStateList =  new List<PlayerState>();
   private Dictionary<PlayerState,int> statePriorityDict = new Dictionary<PlayerState,int>();
   private int attackStage = 0;
   //受伤事件
   private Action hitEvents;
   private AnimationEvent animationEvent;
   private void Awake()
   {
      animationEvent = GetComponent<AnimationEvent>();
      statePriorityDict = new Dictionary<PlayerState,int>()
      {
         {PlayerState.Idle,0},
         {PlayerState.Walk,1 },
         // {PlayerState.WalkBack,2 },
         // {PlayerState.WalkLeft,3 },
         // {PlayerState.WalkRight,4},
         {PlayerState.Chase,2},
         {PlayerState.SwordShield,5 },
         {PlayerState.Jump,6 },
         {PlayerState.Attack,7 },
         {PlayerState.Hit,8 },
         {PlayerState.Dizzy,8 },
         {PlayerState.Skill,9 }
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
   public bool IsCurStatePriorityHigher(PlayerState state)
   {
      return statePriorityDict[playerState] > statePriorityDict[state];
   }

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
   
   public void OnHitInput()
   {
      Hit(true);
   }

   public void Hit(bool isDizzy)
   {
      this.isDizzy = isDizzy;
      animationEvent.PostAnimationEvent("Hit");
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
}
