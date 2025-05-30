using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : CharacterBase
{
    public override ObjectType ObjectType => ObjectType.Enemy;
    [Header("Enemy Settings")]
    public EnemyID enemyID;
    public EnemyType enemyType;
    [Header("Health Stats")]
    public float maxHealth;
    public float currentHealth;

    [Header("Attack Settings")]
    public float attackRange;
    public float basicAttackInterval;
    public float basicAttackTimer;
    public float basicAttackCount;
    public float skillInterval;
    public float skillTimer;
    public float exSkillInterval;
    public float exSkillTimer;

    [Header("Movement Settings")]
    public float moveSpeed;
    public float distance;

    [Header("Enemy State Flags")]
    public bool isCreate;
    public bool isIdle;
    public bool isChase;
    public bool isAttack;
    public bool isSkill;
    public bool isExSkill;
    public bool isDead;
    public bool checkInAttackRenge;
    public bool readyBasicAttack;
    public bool readySkill;
    public bool readyExSkill;

    [Header("Component References")]
    public Rigidbody rigidbodyEnemy;
    public BoxCollider boxCollider;
    public MeshRenderer[] meshs;
    public NavMeshAgent navMeshAgent;
    public Animator animator;
    public Transform enemyBulletFirePoint;

    [HideInInspector] public EnemyState currentState;
    protected PlayableBase currentTarget;

    public int test = 0;

    protected virtual void Awake()
    {
        rigidbodyEnemy = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        currentState = EnemyState.Create;
        isCreate = true;
        Initialize();
        readyBasicAttack = true;
        readySkill = false;
        readyExSkill = false;
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

        CoolTime();

        UpdateTargetAndDistance();//여기서 현재 타겟(리타겟포함), 타겟과 거리 계속 업데이트됨

        CheckingAttackRenge();

        if (currentState == EnemyState.Chasing)
        {
            isIdle = false;
            isChase = true;
            isAttack = false;
            MoveToTarget(currentTarget.transform.position);
        }
        if (currentState == EnemyState.Attack)
        {
            isIdle = false;
            isChase = false;
            isAttack = true;
            AttackThnking();
        }
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

    protected virtual void Initialize()
    {
        currentHealth = maxHealth;
        isCreate = false;
        currentState = EnemyState.Idle;
        isIdle = true;
        readyBasicAttack = false;
    }

    public virtual void SetData(EnemyData data)
    {
        enemyType = data.enemyType;
        maxHealth = data.maxHealth;
        attackRange = data.attackRange;
        basicAttackInterval = data.basicAttackInterval;
        skillInterval = data.skillInterval;
        exSkillInterval = data.exSkillInterval;
        moveSpeed = data.moveSpeed;
    }
    protected virtual void UpdateTargetAndDistance()
    {
        if (isDead)
            return;

        currentTarget = GetNearestEnemyToPosition(transform.position);

        if (currentTarget == null)
            return;

        distance = Vector3.Distance(transform.position, currentTarget.transform.position);
    }

    protected virtual void CheckingAttackRenge()
    {
        currentState = (distance <= attackRange) ? EnemyState.Attack : EnemyState.Chasing;
    }

    void MoveToTarget(Vector3 targetPosition)
    {
        if (navMeshAgent != null)
        {
            navMeshAgent.SetDestination(targetPosition);

            //float distance = Vector3.Distance(transform.position, targetPosition);
        }
    }

    protected virtual void CoolTime()
    {
        basicAttackTimer += Time.deltaTime;
        if (basicAttackTimer >= basicAttackInterval)
        {
            readyBasicAttack = true;
        }
        skillTimer += Time.deltaTime;
        if (skillTimer >= skillInterval)
        {
            readySkill = true;
        }
        exSkillTimer += Time.deltaTime;
        if (exSkillTimer >= exSkillInterval)
        {
            readyExSkill = true;
        }
    }
    protected virtual void AttackThnking()
    {
        if (readyBasicAttack)
        {
            BasicAttack();
        }
        if (readySkill)
        {
            Skill();
        }
        if (exSkillTimer >= exSkillInterval)
        {
            ExSkill();
        }
    }
    protected virtual void BasicAttack()
    {
        if (!isAttack || currentTarget == null || currentTarget.isDead)
        {
            return;
        }
        isChase = false;
        isAttack = true;
        animator.SetBool("isAttack", true);
        navMeshAgent.isStopped = true;
        basicAttackCount++;

        ShootBulletAtTarget();

        isAttack = false;
        basicAttackCount = 0f;
        isChase = true;
        animator.SetBool("isAttack", false);
        currentState = EnemyState.Idle;

        if (basicAttackCount > 5)
        {
            readySkill = true;
            basicAttackCount = 0;
        }
        basicAttackTimer = 0;
        readyBasicAttack = false;
    }
    protected void ShootBulletAtTarget()
    {
        if (currentTarget == null || currentTarget.isDead)
            return;

        Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        Bullet bullet = BulletPoolManager.instance.GetBullet(BulletPoolManager.PoolType.EnemyBullet);

        if (bullet != null)
        {
            bullet.transform.position = enemyBulletFirePoint.position;
            bullet.transform.rotation = Quaternion.LookRotation(direction);

            Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
            if (bulletRigidbody != null)
            {
                bulletRigidbody.velocity = direction * bullet.speed;
            }
        }
    }
    protected virtual void Skill()
    {
    }

    protected virtual void ExSkill()
    {
    }

    public PlayableBase GetNearestEnemyToPosition(Vector3 position)
    {
        PlayableBase nearest = null;
        float minDist = Mathf.Infinity;

        foreach (var playable in PlayableManager.instance.playables)
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

    protected virtual void Die()
    {
        isDead = true;
        isChase = false;
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
            if (other.TryGetComponent<ProjectileBase>(out var projectile))
            {
                projectile.OnHit(gameObject);

                if (projectile is Bullet bullet)
                {
                    BulletPoolManager.instance.ReturnBullet(bullet);
                }
                else
                {
                    Destroy(projectile.gameObject);
                }
            }
        }
    public Vector3 HitByExplosion(Vector3 explosionPos)
    {
        var reactVec = (transform.position - explosionPos).normalized;
        return reactVec;
    }

    public override void ApplyDamage(float damage, bool isExplosion, Vector3? explosionPos = null)
    {
        currentHealth -= damage;
        StartCoroutine(OnDamage(isExplosion, explosionPos));
    }

    IEnumerator OnDamage(bool isExplosion, Vector3? explosionPos)
    {
        foreach (MeshRenderer mesh in meshs)
            mesh.material.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        if (currentHealth > 0)
        {
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.white;

            Vector3 finalVec;

            if (isExplosion && explosionPos.HasValue)
                finalVec = HitByExplosion(explosionPos.Value) + Vector3.up * 3f;
            else
                finalVec = Vector3.up * 1f;

            rigidbodyEnemy.freezeRotation = false;
            rigidbodyEnemy.AddForce(finalVec * 5f, ForceMode.Impulse);

            if (isExplosion)
                rigidbodyEnemy.AddTorque(finalVec * 15f, ForceMode.Impulse);
        }
        else
        {
            currentState = EnemyState.Dead;
            Die();
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.gray;
            Destroy(gameObject, 1.8f);
        }
    }
}
