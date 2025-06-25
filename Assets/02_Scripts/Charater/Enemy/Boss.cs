using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class Boss : EnemyBase
{
    public float skillDuration = 2f; // 미사일 생성 지속 시간
    public float missileSpawnDelay = 1f;   // 미사일 생성 간 딜레이
    float skillDurationTimer = 0f;
    public GameObject skillMissile;
    public GameObject exSkillMissile;
    public Transform enemyMissileFirePoint;
    public Transform enemyExMissileFirePoint;

    protected override void Skill()
    {
        if (!readySkill|| isUsingSkill)
            return;
        isUsingSkill = true;
        readySkill = false;
        skillDurationTimer = 0f;
        StartCoroutine(MissilesPattern());
    }
    private IEnumerator MissilesPattern()
    {

        List<PlayableBase> playables = PlayableManager.instance.GetPlayables();
        int count = playables.Count;

        while (skillDurationTimer < skillDuration)
        {
            foreach (var target in playables)
            {
                if (target == null) continue;

                Vector3 direction = (target.transform.position - enemyMissileFirePoint.position).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                GameObject instantMissile = Instantiate(skillMissile, enemyMissileFirePoint.position, targetRotation);
                BossSkillMissile missileScript = instantMissile.GetComponent<BossSkillMissile>();
                missileScript.target = target.transform;
            }

            yield return new WaitForSeconds(missileSpawnDelay);
            skillDurationTimer += missileSpawnDelay;
        }
        isUsingSkill = false;
        skillTimer = 0f;
        readySkill = false;     
    }
    protected override void ExSkill()
    {
        if (!readySkill || isUsingExSkill)
            return;
        isUsingExSkill = true;
        GameObject exMissileObject = Instantiate(exSkillMissile, enemyExMissileFirePoint.position, Quaternion.identity);
        exSkillTimer = 0f;
        skillDurationTimer = 0f;
        isUsingExSkill = false;
        readyExSkill = false;
    }
}
