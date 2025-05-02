using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemy_base
{
    public int maxTargetCount = 3;
    public float exSkillCoolTime = 10f;
    public float exSkillTimer = 0f;

    private bool skillActivated = false;
    //public bool readyExSkillActive = false;

    public GameObject missile;
    public GameObject exMissile;

    protected override void Update()
    {
        base.Update();

        if (isDead)
            return;

        // ExSkill 쿨타임 관리
        ExSkillCooldown();
    }

    protected override void SetStats()
    {
        maxHealth = (float)EnemyHealth.Boss;
        attackRange = (float)EnemyAttackRange.Boss;
        attackInterval = 2f;
    }

    protected override void HandleState()
    {
        base.HandleState();

        switch (currentState)
        {
            case EnemyState.Skill:
                break;
            case EnemyState.ExSkill:
                if (readyExSkillActive)
                {
                    ExSkill();  // ExSkill 실행
                }
                break;
        }
    }

    protected override void Attack()
    {
        base.Attack();

        // 공격 횟수가 5회 이상일 때 스킬 발동
        if (attackCount > 5)
        {
            currentState = EnemyState.Skill;
            Skill();
            attackCount = 0;
        }

        // ExSkill이 준비되었으면 ExSkill 상태로 전환
        if (readyExSkillActive && currentState != EnemyState.ExSkill)
        {
            currentState = EnemyState.ExSkill;  // ExSkill 상태로 변경
            readyExSkillActive = false;  // ExSkill 준비 상태를 false로 설정
        }
    }

    protected override void Skill()
    {
        List<SoonDoBu_Playable> targets = new List<SoonDoBu_Playable>(PlayableMnager.instance.playables);

        // 유효한 타겟만 필터링
        List<SoonDoBu_Playable> validTargets = new List<SoonDoBu_Playable>();

        foreach (SoonDoBu_Playable target in targets)
        {
            if (target != null && !target.isDead)
            {
                validTargets.Add(target);
            }
        }

        // 가까운 순으로 정렬
        validTargets.Sort((a, b) =>
            Vector3.Distance(transform.position, a.transform.position)
            .CompareTo(Vector3.Distance(transform.position, b.transform.position))
        );

        // 최대 타겟 수만큼 미사일 발사
        for (int i = 0; i < Mathf.Min(maxTargetCount, validTargets.Count); i++)
        {
            SoonDoBu_Playable target = validTargets[i];
            Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
            Vector3 lookDirection = new Vector3(directionToTarget.x, 0, directionToTarget.z);
            transform.rotation = Quaternion.LookRotation(lookDirection);

            GameObject instantMissile = Instantiate(
                missile,
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

        currentState = EnemyState.Attack;  // 공격 상태로 돌아가도록 설정
    }

    protected override void ExSkill()
    {
        // 유효한 타겟 랜덤 선택
        var targets = PlayableMnager.instance.playables.FindAll(t => t != null && !t.isDead);
        if (targets.Count == 0)
        {
            return;
        }

        var selectedTarget = targets[UnityEngine.Random.Range(0, targets.Count)];

        if (selectedTarget == null)
        {
            return;
        }

        // 방향 설정
        Vector3 direction = (selectedTarget.transform.position - transform.position).normalized;
        Vector3 spawnPos = transform.position + Vector3.up * 10f + direction * 5f;

        if (exMissile == null)
        {
            return;
        }

        // Instantiate BossExMissile
        GameObject missileObj = Instantiate(exMissile, spawnPos, Quaternion.LookRotation(direction));
        var exMissileScript = missileObj.GetComponent<BossExMissile>();
        if (exMissileScript == null)
        {
            return;
        }
        exMissileScript.Init(selectedTarget.transform.position);

        // ExSkill을 사용한 후 상태 변경
        currentState = EnemyState.Attack;  // 상태를 다시 Attack으로 변경
    }

    private void ExSkillCooldown()
    {
        if (!readyExSkillActive)
        {
            exSkillTimer += Time.deltaTime;

            if (exSkillTimer >= exSkillCoolTime)
            {
                exSkillTimer = 0f;
                readyExSkillActive = true;  // ExSkill 준비 완료
            }
        }
    }
}
