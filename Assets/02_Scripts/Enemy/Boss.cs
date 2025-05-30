using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : EnemyBase
{
    public int maxTargetCount = 3;
    public bool readyExSkillActive = false;

    public GameObject skMissile;
    public GameObject exMissile;
    public GameObject exSubMissile;

    protected override void Skill()
    {
        if (!readySkill)
            return;
        List<PlayableBase> targets = new List<PlayableBase>(PlayableManager.instance.playables);
        List<PlayableBase> validTargets = new List<PlayableBase>();
        foreach (PlayableBase target in targets)
        {
            if (target != null && !target.isDead)
            {
                validTargets.Add(target);
            }
        }
        validTargets.Sort((a, b) =>
            Vector3.Distance(transform.position, a.transform.position)
            .CompareTo(Vector3.Distance(transform.position, b.transform.position))
        );
        for (int i = 0; i < Mathf.Min(maxTargetCount, validTargets.Count); i++)
        {
            PlayableBase target = validTargets[i];
            Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
            Vector3 lookDirection = new Vector3(directionToTarget.x, 0, directionToTarget.z);
            transform.rotation = Quaternion.LookRotation(lookDirection);

            GameObject instantMissile = Instantiate(
                skMissile,
                transform.position + Vector3.up * 1.0f + lookDirection * 1.5f, // 캐릭터보다 앞에서 발사
                Quaternion.LookRotation(lookDirection)
            );
            Missile missileScript = instantMissile.GetComponent<Missile>();
            missileScript.target = target.transform;
            Rigidbody missileRigidbody = instantMissile.GetComponent<Rigidbody>();
            if (missileRigidbody != null)
            {
                missileRigidbody.velocity = lookDirection * 50f;  // 미사일 초기 속도
            }
        }
        skillTimer = 0;
        readySkill = false;
    }
    protected override void ExSkill()
    {
        //    if (!readyExSkill)
        //        return;
        //    var targets = PlayableMnager.instance.playables.FindAll(t => t != null && !t.isDead);
        //    if (targets.Count == 0)
        //    {
        //        return;
        //    }
        //    var selectedTarget = targets[UnityEngine.Random.Range(0, targets.Count)];
        //    if (selectedTarget == null)
        //    {
        //        return;
        //    }
        //    Vector3 direction = (selectedTarget.transform.position - transform.position).normalized;
        //    Vector3 spawnPos = transform.position + Vector3.up * 10f + direction * 5f;
        //    if (exMissile == null)
        //    {
        //        return;
        //    }
        //    GameObject missileObj = Instantiate(
        //        exMissile, 
        //        spawnPos, 
        //        Quaternion.LookRotation(direction));
        //    exSkillTimer = 0;
        //    readyExSkill = false;
        //    var exMissileScript = missileObj.GetComponent<BossExMissile>();
        //    if (exMissileScript == null)
        //    {
        //        return;
        //    }
        //    exMissileScript.Init(selectedTarget.transform.position);

    }
}
