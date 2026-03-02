using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//场景
public enum SceneType
{
    MainScene = 1,
    FightScene = 2
}

//道具分类
public enum GoodsCategory
{
    Potion = 0, //药水
    Bow = 1, //箭 
    Food = 2, //食物
}

//资源枚举
public enum ResModel
{
    Gold = 1, //金币
    Diamond = 2, //钻石
    SKillBook = 3, //技能券轴
}

// 视图方向
public enum DirectionType
{
    Horizontal,
    Vertical
}

//道具类型
public enum GoodsType
{
    Resource = 1,
    Good = 2,
    Equip = 3,
    Skill = 4,
}

//任务类型
public enum TaskType
{
    Kill = 1,
    PickUp
}


//属性
public enum AttrType
{
    Attack = 0, //攻击力
    MaxHealth = 1, //生命值
    MoveSpeed = 2, //移速 百分比
    AttackSpeed = 3, //攻速
    Armor = 4, //护甲
    CritRate = 5, //暴击率
    CritDamage = 6, //暴击伤害
    DodgeRate = 7, //闪避率 0-1
    CurHealth = 8, //当前生命值
    Exp = 9, //经验
}

//颜色
public class MyColor
{
    public static readonly Color Green = new Color(0.2f, 0.9f, 0.25f, 1f);
    public static readonly string GreenStr = "#33E640";
    public static readonly Color LightGreen = new Color(0.0196f, 0.5176f, 0.0196f);
    public static readonly string LightGreenStr = "#058405";
    public static readonly Color Red = new Color(0.95f, 0.2f, 0.25f, 1f);
    public static readonly string RedStr = "#F23340";
    public static readonly Color LightRed = new Color(0.85f, 0.15f, 0.15f, 1f);
    public static readonly string LightRedStr = "#D92626";
    
}
