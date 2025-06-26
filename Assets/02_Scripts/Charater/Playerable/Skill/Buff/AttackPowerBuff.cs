using UnityEngine;

[CreateAssetMenu(fileName = "AttackPowerBuff", menuName = "Buffs/AttackPowerBuff")]
public class AttackPowerBuff : ScriptableObject, InterfaceBuff
{
    public BuffApplyType buffApplyType;
    public float duration = 5f;
    public float attackPowerBonus = 10f;

    public float Duration => duration;

    public BuffStatType StatType => BuffStatType.AttackPower;
    public float StatValue => attackPowerBonus;

    public void OnApply(GameObject target)
    {
        
    }

    public void OnRemove(GameObject target)
    {
        
    }

    public void OnUpdate(GameObject target, float deltaTime)
    {
       
    }
}
