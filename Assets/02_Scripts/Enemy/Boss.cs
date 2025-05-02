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

        // ExSkill ��Ÿ�� ����
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
                    ExSkill();  // ExSkill ����
                }
                break;
        }
    }

    protected override void Attack()
    {
        base.Attack();

        // ���� Ƚ���� 5ȸ �̻��� �� ��ų �ߵ�
        if (attackCount > 5)
        {
            currentState = EnemyState.Skill;
            Skill();
            attackCount = 0;
        }

        // ExSkill�� �غ�Ǿ����� ExSkill ���·� ��ȯ
        if (readyExSkillActive && currentState != EnemyState.ExSkill)
        {
            currentState = EnemyState.ExSkill;  // ExSkill ���·� ����
            readyExSkillActive = false;  // ExSkill �غ� ���¸� false�� ����
        }
    }

    protected override void Skill()
    {
        List<SoonDoBu_Playable> targets = new List<SoonDoBu_Playable>(PlayableMnager.instance.playables);

        // ��ȿ�� Ÿ�ٸ� ���͸�
        List<SoonDoBu_Playable> validTargets = new List<SoonDoBu_Playable>();

        foreach (SoonDoBu_Playable target in targets)
        {
            if (target != null && !target.isDead)
            {
                validTargets.Add(target);
            }
        }

        // ����� ������ ����
        validTargets.Sort((a, b) =>
            Vector3.Distance(transform.position, a.transform.position)
            .CompareTo(Vector3.Distance(transform.position, b.transform.position))
        );

        // �ִ� Ÿ�� ����ŭ �̻��� �߻�
        for (int i = 0; i < Mathf.Min(maxTargetCount, validTargets.Count); i++)
        {
            SoonDoBu_Playable target = validTargets[i];
            Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
            Vector3 lookDirection = new Vector3(directionToTarget.x, 0, directionToTarget.z);
            transform.rotation = Quaternion.LookRotation(lookDirection);

            GameObject instantMissile = Instantiate(
                missile,
                transform.position + Vector3.up * 1.0f + lookDirection * 1.5f, // ĳ���ͺ��� �տ��� �߻�
                Quaternion.LookRotation(lookDirection)
            );

            Missile missileScript = instantMissile.GetComponent<Missile>();
            missileScript.target = target.transform;

            Rigidbody missileRigidbody = instantMissile.GetComponent<Rigidbody>();
            if (missileRigidbody != null)
            {
                missileRigidbody.velocity = lookDirection * 50f;  // �̻��� �ʱ� �ӵ�
            }
        }

        currentState = EnemyState.Attack;  // ���� ���·� ���ư����� ����
    }

    protected override void ExSkill()
    {
        // ��ȿ�� Ÿ�� ���� ����
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

        // ���� ����
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

        // ExSkill�� ����� �� ���� ����
        currentState = EnemyState.Attack;  // ���¸� �ٽ� Attack���� ����
    }

    private void ExSkillCooldown()
    {
        if (!readyExSkillActive)
        {
            exSkillTimer += Time.deltaTime;

            if (exSkillTimer >= exSkillCoolTime)
            {
                exSkillTimer = 0f;
                readyExSkillActive = true;  // ExSkill �غ� �Ϸ�
            }
        }
    }
}
