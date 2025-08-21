using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour
{
    private SkillData skillData;

    public AreaType effectType;  // 딜인지 힐인지
    private float casterAttackPower;
    public float effectAmount; // 데미지 or 힐량
    public float effectInterval; //몇 초마다 효과 발동?
    public float areaDuration;        // 장판 지속 시간 (초)

    private float tickTimer = 0f;
    private float durationTimer = 0f;

    private List<GameObject> targets = new List<GameObject>();

    private void Update()
    {
        durationTimer += Time.deltaTime;
        if (durationTimer >= areaDuration)
        {
            Destroy(gameObject);
            return;
        }

        tickTimer += Time.deltaTime;
        if (tickTimer >= effectInterval)
        {
            ApplyEffect();
            tickTimer = 0f;
        }
    }

    public void SetArea(SkillData data)
    {
        skillData = data;
        effectType = skillData.areaType;
        effectAmount = skillData.effectAmount;
        effectInterval = skillData.effectInterval;
        areaDuration = skillData.areaDuration;
    }
    public void SetAttackPower(float power)
    {
        casterAttackPower = power;
    }

    private void ApplyEffect()
    {
        // 캐릭터 공격력 기반으로 딜/힐 계산
        float scaledEffectAmount = casterAttackPower * skillData.damageMultiplier;

        foreach (GameObject target in targets)
        {
            if (target == null)
                continue;

            // 힐 대상
            var player = target.GetComponent<PlayableBase>();
            if (effectType == AreaType.Heal && player != null)
            {
                player.Heal(scaledEffectAmount);
                Debug.Log($"[Area Heal] {player.name} 이(가) {scaledEffectAmount} 만큼 회복했습니다.");
                continue;
            }

            // 딜 대상
            var enemy = target.GetComponent<EnemyBase>();
            if (effectType == AreaType.Damage && enemy != null)
            {
                enemy.ApplyDamage(scaledEffectAmount, false);
                Debug.Log($"[Area Damage] {enemy.name} 이(가) {scaledEffectAmount} 만큼 피해를 입었습니다.");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsTargetValid(other.gameObject) && !targets.Contains(other.gameObject))
        {
            targets.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        targets.Remove(other.gameObject);
    }

    private bool IsTargetValid(GameObject obj)
    {
        return obj.GetComponent<PlayableBase>() != null || obj.GetComponent<EnemyBase>() != null;
    }
}
