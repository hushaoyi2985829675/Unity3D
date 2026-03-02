using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EffectType
{
    MonsterDeath,
    CritRate,
}

public class EffectManager : Singleton<EffectManager>
{
    // Dictionary<EffectType, EffectBase> effects;
    // Transform effectNode;
    // private string path;
    //
    // private void Awake()
    // {
    //     effects = new Dictionary<EffectType, EffectBase>();
    //     path = "Effect/";
    //     if (effectNode == null)
    //     {
    //         GameObject go = new GameObject("EffectNode");
    //         effectNode = go.transform;
    //         DontDestroyOnLoad(go);
    //     }
    // }
    //
    // void Start()
    // {
    // }
    //
    // public void PlayEff(EffectType effectType, Vector2 target)
    // {
    //     EffectBase eff;
    //     if (effects.ContainsKey(effectType))
    //     {
    //         eff = effects[effectType];
    //     }
    //     else
    //     {
    //         eff = CreateEff(effectType);
    //     }
    //
    //     eff.Play(target);
    // }
    //
    // public void DelectCache(EffectType effectType)
    // {
    //     effects.Remove(effectType);
    // }
    //
    // private EffectBase CreateEff(EffectType effectType)
    // {
    //     GameObject eff;
    //     GameObject effRef = null;
    //     switch (effectType)
    //     {
    //         case EffectType.CritRate:
    //             effRef = Resources.Load<GameObject>(path + "Player/CritRate/CritRateEff");
    //             break;
    //     }
    //
    //     if (effRef == null)
    //     {
    //         Debug.Log("特效找不到");
    //         return null;
    //     }
    //
    //     eff = Instantiate(effRef, effectNode);
    //     EffectBase effectBase = eff.GetComponent<EffectBase>();
    //     effects.Add(effectType, effectBase);
    //     return effectBase;
    // }
}