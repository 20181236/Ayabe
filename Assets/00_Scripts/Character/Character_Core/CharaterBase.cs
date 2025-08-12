using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class CharacterBase : MonoBehaviour
{
    [Header("Health Stats")]
    public float baseMaxHealth;
    public float buffedMaxHealth;
    public float currentHealth;
    public float MaxHealth => baseMaxHealth + buffedMaxHealth;
    public float CurrentHealth => currentHealth;

    [Header("Attack Stats")]
    public float baseAttackPower;
    public float buffedAttackPower;
    public float AttackPower => baseAttackPower + buffedAttackPower;

    public float baseAttackRange;
    public float buffedAttackRange;
    public float AttackRange => baseAttackRange + buffedAttackRange;

    public float baseAttackInterval;
    public float buffedAttackInterval;
    public float AttackInterval => baseAttackInterval + buffedAttackInterval;

    public float baseHealPower;
    public float buffedHealPower;
    public float HealPower => baseHealPower + buffedHealPower;

    [Header("Attack Timers")]
    public float basicAttackTimer;
    public float basicAttackCount;
    public float skillInterval;
    public float skillTimer;
    public float exSkillInterval;
    public float exSkillTimer;

    [Header("Movement Settings")]
    public float moveSpeed;
    public float distance;

    [Header("State Flags")]
    public bool isCreate;
    public bool isIdle;
    public bool isChase;
    public bool isAttack;
    public bool isAttacking;
    public bool isBasicAttack;  // 오타 수정 (isBisicAttack -> isBasicAttack)
    public bool isSkill;
    public bool isUsingSkill;
    public bool isExSkill;
    public bool isUsingExSkill;
    public bool isDead;
    public bool checkInAttackRange; // 오타 수정 (checkInAttackRenge -> checkInAttackRange)
    public bool readyBasicAttack;
    public bool readySkill;
    public bool readyExSkill;

    public Transform headTransform;

    [Header("Health Bar")]
    [SerializeField] protected HealthBarController healthBarPrefab;
    protected HealthBarController healthBarInstance;

    [Header("Buff System")]
    public List<Buff> activeBuffs = new List<Buff>();

    public abstract ObjectType ObjectType { get; }
    public abstract void ApplyDamage(float damage, bool isExplosion, Vector3? explosionPos = null);

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, MaxHealth);
        healthBarInstance.SetHealth(currentHealth);
    }
    public void ApplyBuff(BuffData data)
    {
        Buff buff = BuffFactory.CreateBuffFromData(data);
        activeBuffs.Add(buff);
        Debug.Log($"버프 추가됨: {buff.targetStat} / 값: {buff.value} / 타입: {buff.applyType}");

        if (buff.applyType == BuffApplyType.Burst)
        {
            OnBuffTick(buff);
            activeBuffs.Remove(buff);
        }
        else
        {
            StartCoroutine(BuffRoutine(buff));
        }

        RecalculateBuffedStats();
    }

    public void RemoveBuff(Buff buffToRemove)
    {
        if (activeBuffs.Remove(buffToRemove))
        {
            RecalculateBuffedStats();
        }
    }

    public void RecalculateBuffedStats()
    {
        buffedMaxHealth = 0f;
        buffedAttackPower = 0f;
        buffedAttackRange = 0f;
        buffedAttackInterval = 0f;
        buffedHealPower = 0f;

        foreach (var buff in activeBuffs)
        {
            float buffValue = buff.value;

            switch (buff.applyType)
            {
                case BuffApplyType.Burst:
                case BuffApplyType.Tick:
                    switch (buff.targetStat)
                    {
                        case BuffStatType.MaxHealth:
                            buffedMaxHealth += baseMaxHealth * buffValue;
                            break;
                        case BuffStatType.AttackPower:
                            buffedAttackPower += baseAttackPower * buffValue;
                            break;
                        case BuffStatType.AttackRange:
                            buffedAttackRange += baseAttackRange * buffValue;
                            break;
                        case BuffStatType.AttackInterval:
                            buffedAttackInterval += baseAttackInterval * buffValue;
                            break;
                        case BuffStatType.HealPower:
                            buffedHealPower += baseHealPower * buffValue;
                            break;
                    }
                    break;
                case BuffApplyType.Continuous:  // 여기 추가
                    switch (buff.targetStat)
                    {
                        case BuffStatType.MaxHealth:
                            buffedMaxHealth += baseMaxHealth * buffValue;
                            break;
                        case BuffStatType.AttackPower:
                            buffedAttackPower += baseAttackPower * buffValue;
                            break;
                        case BuffStatType.AttackRange:
                            buffedAttackRange += baseAttackRange * buffValue;
                            break;
                        case BuffStatType.AttackInterval:
                            buffedAttackInterval += baseAttackInterval * buffValue;
                            break;
                        case BuffStatType.HealPower:
                            buffedHealPower += baseHealPower * buffValue;
                            break;
                    }
                    break;
            }
        }

        currentHealth = Mathf.Min(currentHealth, MaxHealth);
    }

    private IEnumerator BuffRoutine(Buff buff)
    {
        float elapsed = 0f;
        Debug.Log($"BuffRoutine 시작: {buff.targetStat}, applyType: {buff.applyType}, duration: {buff.duration}, tickInterval: {buff.tickInterval}");

        if (buff.applyType == BuffApplyType.Tick)
        {
            // tickInterval 0 이하일 경우 방지
            float interval = Mathf.Max(buff.tickInterval, 0.1f);

            while (elapsed < buff.duration)
            {
                Debug.Log($"BuffRoutine 진행중: elapsed={elapsed}");
                yield return new WaitForSeconds(interval);
                OnBuffTick(buff);
                Debug.Log($"OnBuffTick 호출됨: {buff.targetStat}");
                elapsed += interval;
            }
        }
        else if (buff.applyType == BuffApplyType.Continuous)
        {
            // Continuous는 딱히 틱으로 나누어 효과 주는 게 아니라,
            // 버프 적용 기간 동안 버프 유지 -> 끝나면 제거
            yield return new WaitForSeconds(buff.duration);
        }
        else// Burst
        {
            OnBuffTick(buff);
            Debug.Log($"OnBuffTick 호출됨 (Burst 타입): {buff.targetStat}");
            yield return new WaitForSeconds(buff.duration);
        }

        Debug.Log($"BuffRoutine 종료: {buff.targetStat}");
        activeBuffs.Remove(buff);
        RecalculateBuffedStats();
    }



    protected virtual void OnBuffTick(Buff buff)
    {
        switch (buff.targetStat)
        {
            case BuffStatType.HealPower:
                Heal(baseHealPower * buff.value);
                break;
        }
    }
    public Vector3 GetCasterPosition()
    {
        return transform.position;
    }
}
