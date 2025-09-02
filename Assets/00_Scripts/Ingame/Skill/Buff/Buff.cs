using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff
{
    public BuffID buffId;
    public BuffCategory category;
    public BuffGroup group;
    public BuffApplyType applyType;
    public BuffStatType targetStat;
    public float value;
    public float tickValue;
    public float duration;
    public float tickInterval;

    public Sprite buffIcon; // 여기에 아이콘 추가

    public CharacterBase owner;   // 버프 받는 대상
    public CharacterBase caster;  // 버프 건 주체 (캐스터)

    public void Initialize(BuffID buffId, BuffGroup group, BuffCategory category, BuffApplyType applyType, BuffStatType stat, float value, float tickValue, float duration, float tickInterval = 0f, Sprite buffIcon = null, CharacterBase caster = null)
    {
        this.buffId = buffId;
        this.category = category;
        this.group = group;
        this.applyType = applyType;
        this.targetStat = stat;
        this.value = value;
        this.tickValue = tickValue;
        this.duration = duration;
        this.tickInterval = tickInterval;
        this.buffIcon = buffIcon;  // 아이콘 저장
        this.caster = caster; // 추가
    }

    public void SetOwner(CharacterBase owner, CharacterBase caster = null)
    {
        this.owner = owner;
        this.caster = caster;
    }

    public IEnumerator BuffCoroutine()
    {
        if (applyType == BuffApplyType.Burst || applyType == BuffApplyType.Both)
        {
            if (targetStat == BuffStatType.HealPower)
            {
                float healAmount = owner.baseHealPower * value;
                owner.Heal(healAmount);
            }
            else
            {
                owner.RecalculateBuffedStats();
            }
        }

        float elapsed = 0f;

        if (applyType == BuffApplyType.Tick || applyType == BuffApplyType.Both)
        {
            while (elapsed < duration)
            {
                if (targetStat == BuffStatType.HealPower)
                {
                    float healAmount = owner.baseHealPower * value;
                    owner.Heal(healAmount);
                }
                yield return new WaitForSeconds(tickInterval);
                elapsed += tickInterval;
            }
        }
        else
        {
            yield return new WaitForSeconds(duration);
        }

        owner.RemoveBuff(this);
        owner.RecalculateBuffedStats();
    }
}

//public IEnumerator BuffCoroutine()
//{
//    float elapsed = 0f;

//    // 즉시 효과 (Burst or Both)
//    if (applyType == BuffApplyType.Burst || applyType == BuffApplyType.Both)
//    {
//        if (targetStat == BuffStatType.HealPower)
//        {
//            float healAmount = owner.baseHeal   Power * value;
//            owner.Heal(healAmount);
//        }
//        // 기타 버프도 여기에 처리 가능
//    }

//    // 지속 효과 (Tick or Both)
//    while (elapsed < duration)
//    {
//        if (applyType == BuffApplyType.Tick || applyType == BuffApplyType.Both)
//        {
//            if (targetStat == BuffStatType.HealPower)
//            {
//                float healAmount = owner.baseHealPower * value;
//                owner.Heal(healAmount);
//            }
//            // 기타 틱 버프 처리 가능
//        }

//        elapsed += tickInterval;
//        yield return new WaitForSeconds(tickInterval);
//    }

//    // 버프 종료 시 처리 (필요하면)
//}
////public string buffId;
//public BuffCategory category;
//public BuffApplyType applyType;
//public BuffStatType targetStat;
//public float value;
//public float duration;
//public float tickInterval;

//private float elapsedTime = 0f;
//private float tickTimer = 0f;

//public Buff(BuffCategory category, BuffApplyType applyType, BuffStatType stat, float value, float duration, float tickInterval = 0f)
//{
//    //buffId = id;
//    this.category = category;
//    this.applyType = applyType;
//    targetStat = stat;
//    this.value = value;
//    this.duration = duration;
//    this.tickInterval = tickInterval;
//}

//public bool TickUpdate(float deltaTime, System.Action<Buff> onTick)
//{
//    elapsedTime += deltaTime;

//    if (applyType == BuffApplyType.Tick)
//    {
//        tickTimer += deltaTime;
//        if (tickTimer >= tickInterval)
//        {
//            tickTimer = 0f;
//            onTick?.Invoke(this);
//        }
//    }

//    return elapsedTime >= duration;
//}
