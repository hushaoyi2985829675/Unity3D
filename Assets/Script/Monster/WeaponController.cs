using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private BoxCollider boxCollider;
    private AnimationEvent animationEvent;
    private UnitBehaviorLogicBase unitBehaviorLogicBase;
    private UnitAIBase unitAI;
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        animationEvent = Ui.Instance.GetComponentByParent<AnimationEvent>(transform);
        unitBehaviorLogicBase = Ui.Instance.GetComponentByParent<UnitBehaviorLogicBase>(transform);
        unitAI = Ui.Instance.GetComponentByParent<UnitAIBase>(transform);
    }
    void Start()
    {
        boxCollider.enabled = false;
        animationEvent?.AddAnimationEvent(OnAnimationEvent);
    }

    void Update()
    {
        
    }

    public void OnAnimationEvent(string eventName)
    {
        if(eventName == "AttackStart")
        {
            if(unitAI.GetMonsterTarget() != null)
            {
                //让目标做出反应
                unitAI.GetMonsterTarget().GetComponent<UnitBehaviorLogicBase>().ReactToDanger();
            }
        }
    }
    public void SetDetectAttack(bool isDetect)
    {
        boxCollider.enabled = isDetect;
    }
    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Role"))
        {
            other.gameObject.GetComponent<UnitBehaviorLogicBase>().TriggerHit();
        }
    }

}
