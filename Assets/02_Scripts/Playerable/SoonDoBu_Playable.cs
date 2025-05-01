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
    [Header("Player Stats")]
    public float maxHealth;
    public float currentHealth;
    public float attackRange;

    [Header("Skills and Cooldowns")]
    public float basicSkillCooldown;
    public float basicSkillTimer;
    public bool readyBasicSkill;

    [Header("State Flags")]
    public PlayableState currentState;
    public bool isChase;
    public bool isAttack;
    public bool isDead;

    //이것도 뺄 수 있을 거 같음
    [Header("Game Objects")]
    public GameObject bullet;
    public GameObject missile;
    public Transform excapeSpotTransform;

    [Header("Components")]
    public Rigidbody rigidbody_Playable;
    public BoxCollider boxCollider;
    public MeshRenderer[] meshs;
    public NavMeshAgent navMeshAgent;
    public Animator animator;

    // -------------------- Targeting --------------------
    protected Enemy_base currentTarget;

    void Start()
    {
        PlayableMnager.instance.RegisterPlayable(this);
        StartCoroutine(SkillCooldownRoutine());
    }

    void Awake()
    {
        SetStats();

        rigidbody_Playable = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        boxCollider = GetComponent<BoxCollider>();
        animator = GetComponentInChildren<Animator>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        currentHealth = maxHealth;

        Invoke("ChaseStart", 2f);
    }

    void Update()
    {
        if (isDead)
            return;
        HandleState();
    }

    void FixedUpdate()
    {
        FreezeVelocity();
    }

    // -------------------- State Management --------------------
    protected virtual void SetStats()
    {
        maxHealth = (float)PlayableHelath.SoonDobu;
        currentHealth = 0f;
        attackRange = (float)PlayableAttackRenge.SoonDobu;

        basicSkillCooldown = (float)PlayalbeBaiscSkillCoolTime.SoonDobu;
        basicSkillTimer = 0f;
        readyBasicSkill = false;
    }

    void ChaseStart()
    {
        isChase = true;
        animator.SetBool(eAnimatorType.isWalk.ToString(), true);
    }

    void HandleState()
    {
        if (isDead)
            return;

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
    void HandleMovement()
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
    void FreezeVelocity()
    {
        if (isChase)
        {
            rigidbody_Playable.velocity = Vector3.zero;
            rigidbody_Playable.angularVelocity = Vector3.zero;
        }
    }

    // -------------------- Targeting & Attacking --------------------
    void Targeting()
    {
        if (isDead)
            return;

        if (currentTarget == null || currentTarget.isDead)
        {
            currentTarget = GetNearestEnemyToPosition(transform.position);
        }

        //if (currentTarget == null)
        //    return; ai가 중복체크래

        float distance = Vector3.Distance(transform.position, currentTarget.transform.position);

        if (distance <= attackRange && !isAttack)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
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
    IEnumerator SkillCooldownRoutine()
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

    protected virtual IEnumerator BasicSkill()
    {
        // 기본 스킬 구현
        yield return new WaitForSeconds(0.5f);  // 딜레이 수정

        if (currentTarget == null)
            yield break;

        // 목표 방향 계산
        Vector3 directionToTarget = (currentTarget.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(new Vector3(directionToTarget.x, 0, directionToTarget.z));

        // 미사일 인스턴스 생성
        GameObject instantMissile = Instantiate(
            missile,
            transform.position + Vector3.up * 5f,  // 미사일의 생성 위치
            Quaternion.LookRotation(directionToTarget)
        );

        Missile missileScript = instantMissile.GetComponent<Missile>();
        missileScript.target = currentTarget.transform;

        // 미사일에 속도 적용 (Rigidbody가 필요)
        Rigidbody missileRigidbody = instantMissile.GetComponent<Rigidbody>();
        if (missileRigidbody != null)
        {
            missileRigidbody.velocity = directionToTarget * 20f;  // missileSpeed는 미사일의 속도
        }

        yield return new WaitForSeconds(0.2f);
    }

    // -------------------- Enemy Detection --------------------
    public Enemy_base GetNearestEnemyToPosition(Vector3 position)
    {
        Enemy_base nearestEnemy = null;
        float minDist = Mathf.Infinity;

        foreach (Enemy_base enemy in EnemyManager.instance.enemies)
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