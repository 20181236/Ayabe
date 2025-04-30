using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemy
{
    public enum BossStateType
    {
        Create,
        Idle,
        Attack,
        Skill,
        ExSkill
    }

    BossStateType currentState;
    public Transform missilePort;

    private bool isExSkillActive = false;

    public GameObject missile;
    //public GameObject bossMissile;

    SoonDoBu_Playable currentTarget;
    private void Awake()
    {
        SetStats();
        rigidbodyEnemy = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        currentHealth = maxHealth;

        StartCoroutine(StateMachine());
    }

    void Update()
    {
        if (isDead)
        {
            StopAllCoroutines();
            return;
        }
        skillTimer += Time.deltaTime;
        attackTimer += Time.deltaTime;

        Targeting();
    }
    protected override void SetStats()
    {
        maxHealth = (float)EnemyHelath.Boss;
        attackRange = (float)EnemyAttackRenge.Boss;
        attackInterval = 1.5f;
        attackCount = 0;

        isAttack = false;
    }
    IEnumerator StateMachine()
    {
        while (!isDead)
        {
            Debug.Log("Current State: " + currentState);  // 상태를 출력하여 확인

            switch (currentState)
            {
                case BossStateType.Create:
                    // Create 상태에서 일정 시간이 지나거나 조건이 만족되면 Idle로 전환
                    currentState = BossStateType.Idle;  // Create 후 Idle로 전환
                    Debug.Log("Transition to Idle from Create");
                    break;

                case BossStateType.Idle:
                    Debug.Log("In Idle State");
                    if (isExSkillActive)
                    {
                        currentState = BossStateType.ExSkill;
                        Debug.Log("Transition to ExSkill");
                    }
                    else if (!isAttack && attackTimer >= attackInterval)
                    {
                        if (attackCount < 5)
                        {
                            currentState = BossStateType.Attack;
                            Debug.Log("Transition to Attack");
                        }
                        else
                        {
                            currentState = BossStateType.Skill;
                            Debug.Log("Transition to Skill");
                        }
                    }
                    break;

                case BossStateType.Attack:
                    Debug.Log("In Attack State");
                    if (!isAttack) // 공격 중이 아니면 공격 시작
                    {
                        StartCoroutine(Attack());
                        Debug.Log("Attack");
                        currentState = BossStateType.Idle;  // 공격이 끝난 후 상태를 Idle로 전환
                        Debug.Log("Transition to Idle");
                    }
                    break;

                case BossStateType.Skill:
                    Debug.Log("In Skill State");
                    if (skillTimer >= skillCooldown && !isExSkillActive)
                    {
                        StartCoroutine(Skill());
                        attackCount = 0;
                        currentState = BossStateType.Idle;
                        Debug.Log("Transition to Idle");
                    }
                    break;

                case BossStateType.ExSkill:
                    Debug.Log("In ExSkill State");
                    StartCoroutine(ExSkill());
                    skillTimer = 0f;
                    currentState = BossStateType.Idle;
                    Debug.Log("Transition to Idle");
                    break;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }


    IEnumerator Skill()
    {
        yield return new WaitForSeconds(0.5f);

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

            yield return new WaitForSeconds(0.2f);
        }
    }

    IEnumerator ExSkill()
    {
        yield return null;
    }
}
