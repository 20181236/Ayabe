using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_base : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    public float attackRange;
    public float attackInterval;
    public float attackTimer;
    public float attackCount;
    public float moveSpeed;

    public bool isCreate;
    public bool isChase;
    public bool isAttack;
    public bool readyAttack;
    public bool isDead;

    public bool readyExSkillActive = true;

    public Rigidbody rigidbodyEnemy;
    public BoxCollider boxCollider;
    public MeshRenderer[] meshs;
    public NavMeshAgent navMeshAgent;
    public Animator animator;

    public GameObject enemyBullet;

    [HideInInspector] public EnemyState currentState;

    protected SoonDoBu_Playable currentTarget;

    // 상태 변경 중첩 방지용 플래그
    public bool isStateChanging = false;

    protected virtual void Awake()
    {

        rigidbodyEnemy = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        currentHealth = maxHealth;

        EnemyStateManager.instance.Initialize(this);
    }

    protected virtual void Start()
    {
        if (EnemyManager.instance != null)
            EnemyManager.instance.RegisterEnemy(this);
    }

    protected virtual void Update()
    {
        if (isDead)
            return;

        EnemyStateManager.instance.Update();
    }

    protected virtual void FixedUpdate()
    {
        if (currentState == EnemyState.Chasing)
        {
            MoveToTarget(currentTarget.transform.position);
        }
        else if (currentState == EnemyState.Dead)
        {
            rigidbodyEnemy.velocity = Vector3.zero;
        }
    }

    protected virtual void SetStats() { }

    public virtual void HandleState()
    {
        switch (currentState)
        {
            case EnemyState.Create:
                Initialize();
                break;

            case EnemyState.Idle:
                Targeting();
                break;

            case EnemyState.Chasing:
                currentTarget = GetNearestEnemyToPosition(transform.position);
                if (currentTarget != null)
                {
                    MoveToTarget(currentTarget.transform.position);
                }
                break;

            case EnemyState.Attack:
                Attack();
                break;

            case EnemyState.Dead:
                Die();
                break;

            case EnemyState.Skill:
                Skill();
                break;

            case EnemyState.ExSkill:
                ExSkill();
                break;
        }
    }

    protected virtual void Skill() { }

    protected virtual void ExSkill()
    {
        // ExSkill 로직 처리
        currentState = EnemyState.Attack;  // 상태를 다시 Attack으로 변경
        isStateChanging = false;  // 상태 변경 완료
    }

    // Enemy initialization
    public virtual void Initialize()
    {
        SetStats();
        isCreate = false;
        isChase = true;
    }

    // Targeting logic
    public virtual void Targeting()
    {
        if (isDead)
            return;

        currentTarget = GetNearestEnemyToPosition(transform.position);

        if (currentTarget == null)
            return;

        float distance = Vector3.Distance(transform.position, currentTarget.transform.position);

        if (distance > attackRange)
        {
            currentState = EnemyState.Chasing;
        }
        else if (distance <= attackRange && attackTimer >= attackInterval)
        {
            currentState = EnemyState.Attack;
        }
    }

    // Move towards the target position
    void MoveToTarget(Vector3 targetPosition)
    {
        if (navMeshAgent != null)
        {
            navMeshAgent.SetDestination(targetPosition);

            float distance = Vector3.Distance(transform.position, targetPosition);

            if (distance <= attackRange && attackTimer >= attackInterval)
            {
                navMeshAgent.isStopped = true;
                currentState = EnemyState.Attack;
            }
            else
            {
                navMeshAgent.isStopped = false;
            }
        }
    }

    // Cooldown management for attacks
    protected virtual void AttackCoolTime()
    {
        attackTimer += Time.deltaTime;
        if (attackTimer >= attackInterval)
        {
            readyAttack = true;
        }
    }

    // Attack logic
    protected virtual void Attack()
    {
        if (!readyAttack || currentTarget == null || currentTarget.isDead)
        {
            return;
        }

        isChase = false;
        isAttack = true;
        animator.SetBool("isAttack", true);
        attackCount++;
        ShootBulletAtTarget();

        // Reset attack state after attack
        readyAttack = false;
        attackTimer = 0f;
        isAttack = false;
        isChase = true;
        animator.SetBool("isAttack", false);
        currentState = EnemyState.Idle;

        if (attackCount > 5)
        {
            currentState = EnemyState.Skill;
            Skill();
            attackCount = 0;
        }


        if (readyExSkillActive && currentState != EnemyState.ExSkill)
        {
            currentState = EnemyState.ExSkill;  // ExSkill 상태로 변경
            readyExSkillActive = false;  // ExSkill 준비 상태를 false로 설정
            isStateChanging = true;  // 상태 변경 중임을 설정
        }
    }

    // Shoot bullet at the target
    protected void ShootBulletAtTarget()
    {
        if (currentTarget == null || currentTarget.isDead)
            return;

        Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        GameObject bullet = Instantiate(enemyBullet,
            transform.position + Vector3.up * 3f,
            Quaternion.LookRotation(direction));
        bullet.GetComponent<Rigidbody>().velocity = direction * 20;
    }

    // Get nearest target to the position
    public SoonDoBu_Playable GetNearestEnemyToPosition(Vector3 position)
        {
        SoonDoBu_Playable nearest = null;
        float minDist = Mathf.Infinity;

        foreach (var playable in PlayableMnager.instance.playables)
        {
            if (playable == null)
                continue;
            float dist = Vector3.Distance(position, playable.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = playable;
            }
        }
        return nearest;
    }

    // Handle damage application
    protected virtual void ApplyDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0 && !isDead)
        {
            currentState = EnemyState.Dead;
        }
    }

    // Die logic
    protected virtual void Die()
    {
        isDead = true;
        animator.SetTrigger("doDie");
        OnDestroy();
    }

    // Destroy enemy and unregister from the manager
    public void OnDestroy()
    {
        if (EnemyManager.instance != null)
            EnemyManager.instance.UnregisterEnemy(this);
    }
}


