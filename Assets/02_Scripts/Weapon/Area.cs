using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour
{
    private SkillData skillData;

    public AreaType effectType;  // 딜인지 힐인지
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

    private void ApplyEffect()
    {
        foreach (GameObject target in targets)
        {
            if (target == null)
                continue;

            // 힐 대상
            var player = target.GetComponent<PlayableBase>();
            if (player != null)
            {
                player.Heal(effectAmount);
                continue;
            }

            // 딜 대상
            var enemy = target.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.ApplyDamage(effectAmount, false);
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
