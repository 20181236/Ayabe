using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : CharacterBase, InterfaceHealth
{
    public override ObjectType ObjectType => ObjectType.Enemy;
    [Header("Enemy Settings")]
    public EnemyID enemyID;
    public EnemyType enemyType;

    [Header("Component References")]
    public Rigidbody rigidbodyEnemy;
    public BoxCollider boxCollider;
    public MeshRenderer[] meshs;
    public NavMeshAgent navMeshAgent;
    public Animator animator;
    public Transform enemyBulletFirePoint;

    [HideInInspector] public EnemyState currentState;
    protected PlayableBase currentTarget;
    protected virtual void Awake()
    {
        rigidbodyEnemy = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        if (EnemyManager.instance != null)
            EnemyManager.instance.RegisterEnemy(this);

        Initialize();

    }

    protected virtual void Start()
    {
        if (EnemyManager.instance != null)
            EnemyManager.instance.RegisterEnemy(this);

        InitHealthBar();
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
            navMeshAgent.isStopped = false;
            MoveToTarget(currentTarget.transform.position);
        }
        if (currentState == EnemyState.Attack)
        {
            isIdle = false;
            isChase = false;
            isAttack = true;
            navMeshAgent.isStopped = true;
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
        currentHealth = MaxHealth;
        currentState = EnemyState.Create;
        isCreate = true;
        readyBasicAttack = false;
        readySkill = false;
        readyExSkill = false;
        isCreate = false;
        isUsingSkill = false;
        currentState = EnemyState.Idle;
        isIdle = true;
    }

    public virtual void SetData(EnemyData data)
    {
        enemyType = data.enemyType;

        baseMaxHealth = data.maxHealth;
        baseAttackPower = data.attackPower;
        baseAttackRange = data.attackRange;
        baseAttackInterval = data.AttackInterval;
        baseHealPower = data.HealPower;
        moveSpeed = data.moveSpeed;

        if (navMeshAgent != null)
            navMeshAgent.speed = moveSpeed;

        Initialize();
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
        currentState = (distance <= AttackRange) ? EnemyState.Attack : EnemyState.Chasing;
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
        if (basicAttackTimer >= AttackInterval)
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
        if (readyBasicAttack && !isUsingSkill)
        {
            BasicAttack();
        }
        else if (readySkill && !isUsingSkill)
        {
            Skill();
        }
        else if (exSkillTimer >= exSkillInterval)
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
        isAttacking = true;
        isBasicAttack = true;
        animator.SetBool("isAttack", true);
        ShootBulletAtTarget();
        //basicAttackCount++;
        basicAttackTimer = 0;
        isBasicAttack = false;
        readyBasicAttack = false;
        isAttacking = false;
        animator.SetBool("isAttack", false);
        currentState = EnemyState.Idle;

        //if (basicAttackCount > 5)
        //{
        //    readySkill = true;
        //    basicAttackCount = 0;
        //}
        //basicAttackTimer = 0;
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

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<ProjectileBase>(out var projectile))
        {
            // 자기 자신 무시
            if (projectile.ShooterType == ObjectType)
                return;

            // CharacterBase 컴포넌트가 있는지 먼저 확인
            if (gameObject.TryGetComponent<CharacterBase>(out var character))
            {
                if (projectile.ShooterType == character.ObjectType)
                {
                    return;
                }
                projectile.OnHit(gameObject);

                if (projectile is Bullet bullet)
                    BulletPoolManager.instance.ReturnBullet(bullet);
                else
                    Destroy(projectile.gameObject);
            }
        }
    }
    public Vector3 HitByExplosion(Vector3 explosionPos)
    {
        var reactVec = (transform.position - explosionPos).normalized;
        return reactVec;
    }

    //public override void ApplyDamage(float damage, bool isExplosion, Vector3? explosionPos = null)
    //{
    //    if (isDead)
    //        return;

    //    currentHealth -= damage;

    //    DamageManager.instance.ShowDamage2(headTransform.position, Mathf.FloorToInt(damage));

    //    StartCoroutine(OnDamage(isExplosion, explosionPos));
    //}
    public override void ApplyDamage(float damage, bool isExplosion, Vector3? explosionPos = null)
    {
        if (isDead)
            return;

        currentHealth -= damage;
        DamageManager.instance.ShowDamage2(headTransform.position, Mathf.FloorToInt(damage));

        // 체력바 업데이트
        if (healthBarInstance != null)
        {
            healthBarInstance.SetHealth(currentHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(OnDamage(isExplosion, explosionPos));
        }
    }

    //IEnumerator OnDamage(bool isExplosion, Vector3? explosionPos)
    //{
    //    foreach (MeshRenderer mesh in meshs)
    //        mesh.material.color = Color.red;

    //    yield return new WaitForSeconds(0.1f);

    //    if (currentHealth > 0)
    //    {
    //        foreach (MeshRenderer mesh in meshs)
    //            mesh.material.color = Color.white;

    //        Vector3 finalVec;

    //        if (isExplosion && explosionPos.HasValue)
    //            finalVec = HitByExplosion(explosionPos.Value) + Vector3.up * 3f;
    //        else
    //            finalVec = Vector3.up * 1f;

    //        rigidbodyEnemy.freezeRotation = false;
    //        rigidbodyEnemy.AddForce(finalVec * 5f, ForceMode.Impulse);

    //        if (isExplosion)
    //            rigidbodyEnemy.AddTorque(finalVec * 15f, ForceMode.Impulse);
    //    }
    //    else
    //    {
    //        currentState = EnemyState.Dead;
    //        Die();
    //        foreach (MeshRenderer mesh in meshs)
    //            mesh.material.color = Color.gray;
    //        Destroy(gameObject, 1.8f);
    //    }
    //}
    IEnumerator OnDamage(bool isExplosion, Vector3? explosionPos)
    {
        foreach (MeshRenderer mesh in meshs)
            mesh.material.color = Color.red;

        yield return new WaitForSeconds(0.1f);

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
    protected virtual void Die()
    {
        if (isDead)
            return;

        isDead = true;
        isChase = false;
        animator.SetTrigger("doDie");

        //WaveManager.instance.NotifyEnemyKilled();
        StageManager.instance.NotifyEnemyKilled();

        foreach (MeshRenderer mesh in meshs)
            mesh.material.color = Color.gray;

        currentState = EnemyState.Dead;

        if (healthBarInstance != null)
        {
            Destroy(healthBarInstance.gameObject);
            healthBarInstance = null;
        }

        OnDestroy();
    }

    public void OnDestroy()
    {
        if (EnemyManager.instance != null)
            EnemyManager.instance.UnregisterEnemy(this);

        Destroy(gameObject, 1.8f);
    }

}
