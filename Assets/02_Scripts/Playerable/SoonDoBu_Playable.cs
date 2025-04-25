using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class SoonDoBu_Playable : MonoBehaviour
{
    // -------------------- Player Stats --------------------
    [Header("Player Stats")]
    public float maxHealth = 100f;
    public float currentHealth = 0f;
    public float attackRange = (float)ePlayableAttackRenge.SoonDobu;

    // -------------------- Skill and Cooldown --------------------
    [Header("Skills and Cooldowns")]
    public float basicSkillCooldown = 10f;
    public float basicSkillTimer = 0f;
    public bool readyBasicSkill = false;

    // -------------------- State Flags --------------------
    [Header("State Flags")]
    public ePlayableState currentState;
    public bool isChase;
    public bool isAttack;
    public bool isDead;

    // -------------------- Game Objects --------------------
    [Header("Game Objects")]
    public GameObject bullet;
    public GameObject missile;
    public Transform excapeSpotTransform;

    // -------------------- Components --------------------
    [Header("Components")]
    public Rigidbody rigidbody_SoonDoBu;
    public BoxCollider boxCollider;
    public MeshRenderer[] meshs;
    public NavMeshAgent navMeshAgent;
    public Animator animator;

    // -------------------- Targeting --------------------
    private Enemy currentTarget;

    private void Start()
    {
        PlayableMnager.instance.RegisterPlayable(this);
        StartCoroutine(SkillCooldownRoutine());
    }

    private void Awake()
    {
        rigidbody_SoonDoBu = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        boxCollider = GetComponent<BoxCollider>();
        animator = GetComponentInChildren<Animator>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        currentHealth = maxHealth;

        Invoke("ChaseStart", 2f);
    }

    void Update()
    {
        HandleState();
    }

    void FixedUpdate()
    {
        FreezeVelocity();
    }

    // -------------------- State Management --------------------
    private void ChaseStart()
    {
        isChase = true;
        animator.SetBool(eAnimatorType.isWalk.ToString(), true);
    }

    private void HandleState()
    {
        if (isDead) return;  // 죽으면 상태 갱신 안함

        Targeting();

        if (isChase)
            HandleMovement();

        if (isAttack)
        {
            // 공격 애니메이션이 끝나면 자동으로 Attack 상태 해제
            if (!animator.GetBool("isAttack"))
            {
                isAttack = false;
                isChase = true;  // 공격 끝나면 추적 상태로 전환
            }
        }

        if (isDead)
        {
            animator.SetBool("isDead", true);  // 죽음 애니메이션
        }
    }

    // -------------------- Movement --------------------
    private void HandleMovement()
    {
        if (navMeshAgent.enabled)
        {
            if (isChase || !isAttack)
            {
                Vector3 rightDestination = transform.position + Vector3.forward * 100f; //  상수는 나중에 조정
                navMeshAgent.SetDestination(rightDestination);
            }
            navMeshAgent.isStopped = !isChase || isAttack;
        }
    }
    private void FreezeVelocity()
    {
        if (isChase)
        {
            rigidbody_SoonDoBu.velocity = Vector3.zero;
            rigidbody_SoonDoBu.angularVelocity = Vector3.zero;
        }
    }

    // -------------------- Targeting & Attacking --------------------
    private void Targeting()
    {
        if (isDead)
            return;

        if (currentTarget == null || currentTarget.isDead)
        {
            currentTarget = GetNearestEnemyToPosition(transform.position);
        }

        if (currentTarget == null)
            return;

        float distance = Vector3.Distance(transform.position, currentTarget.transform.position);

        if (distance <= attackRange && !isAttack)
        {
            StartCoroutine(Attack());
        }
    }

    private IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        navMeshAgent.isStopped = true;
        animator.SetBool("isAttack", true);

        yield return new WaitForSeconds(0.3f);

        if (currentTarget == null || currentTarget.isDead)
        {
            isAttack = false;
            isChase = true;
            navMeshAgent.isStopped = false;
            animator.SetBool("isAttack", false);
            yield break;
        }

        Vector3 directionToTarget = (currentTarget.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(new Vector3(directionToTarget.x, 0, directionToTarget.z));

        // 기본 공격
        GameObject instantBullet = Instantiate(
            bullet,
            transform.position + Vector3.up * 3f,
            Quaternion.LookRotation(directionToTarget)
        );
        instantBullet.GetComponent<Rigidbody>().velocity = directionToTarget * 20f;

        if (readyBasicSkill)
        {
            StartCoroutine(BasicSkill());
            readyBasicSkill = false;
        }

        yield return new WaitForSeconds(0.2f);

        isAttack = false;
        isChase = true;
        navMeshAgent.isStopped = false;
        animator.SetBool("isAttack", false);
    }

    // -------------------- BasicSkill & Cooldown --------------------
    private IEnumerator SkillCooldownRoutine()
    {
        while (!isDead)
        {
            if (!readyBasicSkill)
            {
                yield return new WaitForSeconds(basicSkillCooldown);
                readyBasicSkill = true;
            }
            else
            {
                yield return null;
            }
        }
    }

    private IEnumerator BasicSkill()
    {
        yield return new WaitForSeconds(1f);

        if (currentTarget == null)
            yield break;

        Vector3 directionToTarget = (currentTarget.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(new Vector3(directionToTarget.x, 0, -90));

        GameObject instantMissile = Instantiate(
            missile,
            transform.position + Vector3.up * 7f,
            Quaternion.LookRotation(directionToTarget)
        );

        Missile missileScript = instantMissile.GetComponent<Missile>();
        missileScript.target = currentTarget.transform;

        yield return new WaitForSeconds(0.2f);
    }

    // -------------------- Enemy Detection --------------------
    public Enemy GetNearestEnemyToPosition(Vector3 position)
    {
        Enemy nearestEnemy = null;
        float minDist = Mathf.Infinity;

        foreach (Enemy enemy in EnemyManager.instance.enemies)
        {
            if (enemy == null) continue;

            float dist = Vector3.Distance(position, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearestEnemy = enemy;
            }
        }
        return nearestEnemy;
    }

    // -------------------- Damage Handling --------------------
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "EnemyBullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            currentHealth -= bullet.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            Destroy(other.gameObject);

            StartCoroutine(OnDamage(reactVec, false));
        }
    }

    private IEnumerator OnDamage(Vector3 reactVec, bool isGrenade)
    {
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.red;
        }

        yield return new WaitForSeconds(0.1f);

        if (currentHealth > 0)
        {
            foreach (MeshRenderer mesh in meshs)
            {
                mesh.material.color = Color.white;
            }
        }
        else
        {
            foreach (MeshRenderer mesh in meshs)
            {
                mesh.material.color = Color.gray;
            }
            isDead = true;
            isChase = false;
        }
    }
}