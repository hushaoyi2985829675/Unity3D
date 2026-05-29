using MonsterNs;
using UnityEngine;

public class MonsterAIBase : UnitAIBase
{
    [SerializeField]
    private int monsterId;
    protected MonsterInfo monsterInfo;

    protected override void Awake()
    {
        targetSearchLayer = "Role";
        base.Awake();
        InitMonster(monsterId);
    }

    protected override void Start()
    {
        base.Start();
        navMeshAgent.updatePosition = false;
    }

    public virtual void InitMonster(int id)
    {
        monsterId = id;
        monsterInfo = ConfigManager.Instance.GetMonsterInfoById(id);
        SetNavSpeed(monsterInfo.moveSpeed);
    }

    public MonsterInfo GetMonsterInfo()
    {
        return monsterInfo;
    }
}
