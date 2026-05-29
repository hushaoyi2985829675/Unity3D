using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MonsterNs;
using RoleCampNs;
using RoleNs;
using UnityEngine;

public class ConfigManager : Singleton<ConfigManager> 
{
   //角色表
   private Dictionary<int,RoleInfo> roleConfigDict = new Dictionary<int, RoleInfo>();
   //角色阵营表
   private Dictionary<int,RoleCampInfo> roleCampConfigDict = new Dictionary<int, RoleCampInfo>();
    // 怪物表
   private Dictionary<int, MonsterInfo> monsterConfigDict = new Dictionary<int, MonsterInfo>();


   
   private void Awake()
   {
      //角色表
      roleConfigDict = Resources.Load<RoleConfig>("Configs/Data/RoleConfig").roleInfoList.ToDictionary(roleInfo => roleInfo.roleId);
      roleCampConfigDict = Resources.Load<RoleCampConfig>("Configs/Data/RoleCampConfig").roleCampInfoList.ToDictionary(roleCampInfo => roleCampInfo.campId);
      monsterConfigDict = Resources.Load<MonsterConfig>("Configs/Data/MonsterConfig").monsterInfoList.ToDictionary(monsterInfo => monsterInfo.monsterId);
   }
   //角色表
   public Dictionary<int,RoleInfo> GetRoleConfig()
   {
      return roleConfigDict;
   }
   public RoleInfo GetRoleInfoById(int id)
   {
      return roleConfigDict[id];
   }
   //阵营表
   public Dictionary<int, RoleCampInfo> GetRoleCampConfig()
   {
      return roleCampConfigDict;
   }
   public RoleCampInfo GetRoleCampInfoById(int campId)
   {
      return roleCampConfigDict[campId];
   }

   // 怪物表
   public Dictionary<int, MonsterInfo> GetMonsterConfig()
   {
      return monsterConfigDict;
   }

   // 根据怪物ID获取怪物信息
   public MonsterInfo GetMonsterInfoById(int id)
   {
      return monsterConfigDict[id];
   }
}
