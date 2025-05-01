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

    public Rigidbody rigidbodyEnemy;
    public BoxCollider boxCollider;
    public MeshRenderer[] meshs;
    public NavMeshAgent navMeshAgent;
    public Animator animator;

    public GameObject enemyBullet;

    [HideInInspector] public EnemyState currentState;

    protected SoonDoBu_Playable currentTarget;

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
        HandleState();
        AttackCoolTime();
    }

    protected virtual void FixedUpdate()
    {
        // 예시: 추적 상태에서 타겟을 향해 계속 이동
        if (currentState == EnemyState.Chasing)
        {
            MoveToTarget(currentTarget.transform.position);
        }
        else if (currentState == EnemyState.Dead)
        {
            // 죽었을 때 더 이상 이동하지 않음
            rigidbodyEnemy.velocity = Vector3.zero;
        }
    }

    protected virtual void SetStats() { }

    protected virtual void HandleState()
    {
        switch (currentState)
        {
            case EnemyState.Create:
                Initialize();
                break;

            case EnemyState.Idle:
                Debug.Log("idle");
                Targeting();
                break;

            case EnemyState.Chasing:
                Debug.Log("Chasing");
                MoveToTarget(currentTarget.transform.position);
                break;

            case EnemyState.Attack:
                Attack();
                break;

            case EnemyState.Dead:
                Die();
                break;

            case EnemyState.Skill:
            case EnemyState.ExSkill:
                // 스킬 처리
                break;
        }
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

        // 타겟이 null이거나 죽었으면 다시 찾기
        if (currentTarget == null || currentTarget.isDead)
        {
            currentTarget = GetNearestEnemyToPosition(transform.position);
        }

        if (currentTarget == null)
            return; // 살아있는 적이 아예 없을 때

        float distance = Vector3.Distance(transform.position, currentTarget.transform.position);

        // 공격 사거리 밖이면 추적 상태로 전환
        if (distance > attackRange)
        {
            currentState = EnemyState.Chasing;
        }
        // 공격 사거리 안이면 공격 상태로 전환
        else if (distance <= attackRange && attackTimer >= attackInterval)
        {
            currentState = EnemyState.Attack;
        }
    }

    void MoveToTarget(Vector3 targetPosition)
    {
        if (navMeshAgent != null)
        {
            navMeshAgent.SetDestination(targetPosition);  // NavMesh를 활용해 목표로 이동

            // 공격 사거리 내로 들어오면 멈추고 공격 상태로 전환
            float distance = Vector3.Distance(transform.position, targetPosition);

            if (distance <= attackRange && attackTimer >= attackInterval)
            {
                // 공격 범위에 들어오면 멈추기
                navMeshAgent.isStopped = true;  // 이동을 멈춤
                currentState = EnemyState.Attack;  // 공격 상태로 전환
            }
            else
            {
                // 공격 범위 밖이면 이동 재개
                navMeshAgent.isStopped = false;
            }
        }
    }

    protected virtual void AttackCoolTime()
    {
        attackTimer += Time.deltaTime;
        if (attackTimer >= attackInterval)
            readyAttack = true;
    }

    protected virtual void Attack()
    {
        isChase = false;
        isAttack = true;
        animator.SetBool("isAttack", true);
        attackCount++;

        if (currentTarget == null || currentTarget.isDead)
        {
            isAttack = false;
            isChase = true;
            animator.SetBool("isAttack", false);

            currentTarget = null;
            currentState = EnemyState.Idle;
            attackTimer = 0f;
            return;
        }
        ShootBulletAtTarget();

        isAttack = false;
        isChase = true;
        attackTimer = 0f;
        currentState = EnemyState.Idle;
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

    private void OnDestroy()
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
