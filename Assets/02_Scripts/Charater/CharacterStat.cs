//폐기예정

//using UnityEngine;

//public class CharacterStats : MonoBehaviour
//{
//    // 기본값
//    public float baseMaxHealth;
//    public float baseAttackPower;
//    public float baseAttackInterval;
//    public float baseAttackRange;
//    public float baseHealPower;

//    // 버프 보정값
//    private float buffedMaxHealth;
//    private float buffedAttackPower;
//    private float buffedAttackInterval;
//    private float buffedAttackRange;
//    private float buffedHealPower;

//    // 최종 적용값 (읽기 전용)
//    public float MaxHealth => baseMaxHealth + buffedMaxHealth;
//    public float AttackPower => baseAttackPower + buffedAttackPower;
//    public float AttackInterval => baseAttackInterval + buffedAttackInterval;
//    public float AttackRange => baseAttackRange + buffedAttackRange;
//    public float HealPower => baseHealPower + buffedHealPower;

//    public void InitializeStats(PlayableData data)
//    {
//        baseMaxHealth = data.maxHealth;
//        baseAttackPower = data.attackPower;
//        baseAttackInterval = data.AttackInterval;
//        baseAttackRange = data.attackRange;
//        baseHealPower = 0f; // 힐 수치가 없으면 0 기본값
//    }

//    // 버프 적용
//    public void AddBuffStat(BuffStatType stat, float value)
//    {
//        switch (stat)
//        {
//            case BuffStatType.MaxHealth:
//                buffedMaxHealth += value;
//                break;
//            case BuffStatType.AttackPower:
//                buffedAttackPower += value;
//                break;
//            case BuffStatType.AttackInterval:
//                buffedAttackInterval += value;
//                break;
//            case BuffStatType.AttackRange:
//                buffedAttackRange += value;
//                break;
//            case BuffStatType.HealPower:
//                buffedHealPower += value;
//                break;
//        }
//    }

//    // 버프 제거
//    public void RemoveBuffStat(BuffStatType stat, float value)
//    {
//        AddBuffStat(stat, -value);
//    }
//}
