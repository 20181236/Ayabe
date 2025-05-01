using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemy_base
{
    public int maxTargetCount = 3;
    public float exSkillCoolTime = 30f;
    public float exSkillTimer = 0f;

    private bool skillActivated = false;
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

        currentState = EnemyState.Attack;  // ��ų �߻� �� ���� ����
    }

    // Ư�� ��ų ���� �޼���
    void ExSkill()
    {
        // Ư�� ��ų ����
        Debug.Log("Executing extra skill");
        // ��: ������ ���� ����, �̻��� �߻� ��
    }

    // Ư�� ��ų ��Ÿ�� üũ�� ó���ϴ� �Լ�
    private void ExSkillCooldown()
    {
        exSkillTimer += Time.deltaTime;

        if (exSkillTimer >= exSkillCoolTime && !isExSkillActive)
        {
            isExSkillActive = true;
            exSkillTimer = 0f;
            currentState = EnemyState.ExSkill;

        }
    }
}
