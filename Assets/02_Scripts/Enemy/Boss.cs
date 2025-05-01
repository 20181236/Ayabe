using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemy_base
{
    public float exSkillCoolTime = 30f;
    public float exSkillTimer = 0f;

    private bool isExSkillActive = false;

    public GameObject missile;
    public Transform missilePort;

    protected override void Update()
    {
        base.Update();

        if (isDead)
            return;

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
                Skill();
                break;
            case EnemyState.ExSkill:
                break;
        }
    }

    protected override void Attack()
    {
        base.Attack();

        if (attackCount > 5)
        {
            currentState = EnemyState.Skill;
            Skill();
            attackCount = 0;
        }
    }

    public void Skill()
    {
        List<SoonDoBu_Playable> targets = new List<SoonDoBu_Playable>(PlayableMnager.instance.playables);

        foreach (SoonDoBu_Playable target in targets)
        {
            if (target == null || target.isDead)
                continue;

            Vector3 directionToTarget = (target.transform.position - transform.position).normalized;

            Vector3 lookDirection = new Vector3(directionToTarget.x, 0, directionToTarget.z);
            transform.rotation = Quaternion.LookRotation(lookDirection);

            GameObject instantMissile = Instantiate(
                missile,
                transform.position + Vector3.up * 1.0f,
                Quaternion.LookRotation(lookDirection)
            );

            Missile missileScript = instantMissile.GetComponent<Missile>();
            missileScript.target = target.transform;

            Rigidbody missileRigidbody = instantMissile.GetComponent<Rigidbody>();
            if (missileRigidbody != null)
            {
                missileRigidbody.velocity = lookDirection.normalized * 10f;
            }
        }
    }

    // 특수 스킬 실행 메서드
    void ExSkill()
    {
        // 특수 스킬 로직
        Debug.Log("Executing extra skill");
        // 예: 보스의 범위 공격, 미사일 발사 등
    }

    // 특수 스킬 쿨타임 체크를 처리하는 함수
    private void ExSkillCooldown()
    {
        exSkillTimer += Time.deltaTime;

        if (exSkillTimer >= exSkillCoolTime && !isExSkillActive)
        {
            isExSkillActive = true;
            exSkillTimer = 0f;
            currentState = EnemyState.ExSkill;

            // 특수 스킬을 일정 시간 후에 실행
            //InvokeAction(EXSkill(), 1.0f);  // 예: 1초 후 특수 스킬 실행
        }
    }

    // Action을 통해 특수 스킬을 실행하는 메서드
    public void InvokeAction(Action action, float delay)
    {
        StartCoroutine(InvokeWithDelay(action, delay));
    }

    private IEnumerator InvokeWithDelay(Action action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
}
