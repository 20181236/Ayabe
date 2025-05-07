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

    public bool isStateChanging = false;

    protected virtual void Awake()
    {
        SetStats();
        rigidbodyEnemy = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        currentHealth = maxHealth;
        currentState = EnemyState.Create;
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

        if (currentState == EnemyState.Chasing)
        {
            Targeting();
        }

        HandleState();

        AttackCoolTime();
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

    protected virtual void HandleState()
    {
        if (isStateChanging) return;  // 상태 변경 중에는 다른 상태로 변경하지 않음

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

    protected virtual void Skill()
    {
    }

    protected virtual void ExSkill()
    {
        // ExSkill 로직
        // ...

        // ExSkill이 끝난 후 상태 변경
        currentState = EnemyState.Attack;  // 상태를 다시 Attack으로 변경
        isStateChanging = false;  // 상태 변경 완료
    }

    protected virtual void Initialize()
    {
        isCreate = false;
        isChase = true;
        currentState = EnemyState.Idle;
    }

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

    protected virtual void AttackCoolTime()
    {
        attackTimer += Time.deltaTime;
        if (attackTimer >= attackInterval)
        {
            readyAttack = true;
        }
    }

    protected virtual void Attack()
    {
        if (!readyAttack || currentTarget == null || currentTarget.isDead)
        {
            return;
        }

        isChase = false;
        isAttack = true;
        animator.SetBool("isAttack", true);
        navMeshAgent.isStopped = true;
        attackCount++;
    
        ShootBulletAtTarget();

        // 공격 후 상태 리셋
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
            currentState = EnemyState.ExSkill;
            readyExSkillActive = false;
            isStateChanging = true;
        }
    }

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

    protected virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0 && !isDead)
        {
            currentState = EnemyState.Dead;
        }
    }

    protected virtual void Die()
    {
        isDead = true;
        animator.SetTrigger("doDie");
        OnDestroy();
    }

    public void OnDestroy()
    {
        if (EnemyManager.instance != null)
            EnemyManager.instance.UnregisterEnemy(this);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            Bullet bullet = other.GetComponent<Bullet>();
            Vector3 reactVec = transform.position - other.transform.position;
            Destroy(other.gameObject);
            ApplyDamage(bullet.damage, reactVec, false);
        }
    }

    public void HitByGrenade(Vector3 explosionPos)
    {
        Vector3 reactVec = transform.position - explosionPos;
        ApplyDamage(100f, reactVec, true);
    }

    public void ApplyDamage(float damage, Vector3 reactVec, bool isGrenade)
    {
        currentHealth -= damage;
        StartCoroutine(OnDamage(reactVec, isGrenade));
    }

    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade)
    {
        foreach (MeshRenderer mesh in meshs)
            mesh.material.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        if (currentHealth > 0)
        {
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.white;
        }
        else
        {
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.gray;

            isDead = true;
            isChase = false;
            animator.SetTrigger("doDie");

            reactVec = reactVec.normalized + Vector3.up * (isGrenade ? 3f : 1f);
            rigidbodyEnemy.freezeRotation = false;
            rigidbodyEnemy.AddForce(reactVec * 5, ForceMode.Impulse);

            if (isGrenade)
                rigidbodyEnemy.AddTorque(reactVec * 15, ForceMode.Impulse);

            Destroy(gameObject, 1.8f);
        }
    }
}

