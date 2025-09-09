using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : CharacterBase
{
    public override ObjectType ObjectType => ObjectType.Enemy;
    public enum EnemyState { Idle, Chasing, Attack, Dead, Create }

    [Header("Enemy Settings")]
    public EnemyID enemyID;
    public EnemyType enemyType;

    // 이 변수들은 CharacterBase에서 이미 선언되었으므로 제거합니다.
    // public Rigidbody rigidbodyEnemy;
    public Transform enemyBulletFirePoint;

    [HideInInspector] public EnemyState currentState;
    protected PlayableBase currentTarget;

    protected override void Awake()
    {
        base.Awake(); // CharacterBase의 Awake를 호출하여 컴포넌트 할당
        if (EnemyManager.instance != null) EnemyManager.instance.RegisterEnemy(this);
        Initialize();
    }

    protected override void Start()
    {
        InitHealthBar();
    }

    protected override void Update()
    {
        if (isDead)
            return;

        CoolTime();
        UpdateTargetAndDistance();
        CheckingAttackRenge();

        if (currentState == EnemyState.Chasing)
        {
            isIdle = false;
            isChase = true;
            isAttack = false;
            if (navMeshAgent != null && navMeshAgent.enabled)
            {
                navMeshAgent.isStopped = false;
                MoveToTarget(currentTarget.transform.position);
            }
        }
        if (currentState == EnemyState.Attack)
        {
            isIdle = false;
            isChase = false;
            isAttack = true;
            if (navMeshAgent != null && navMeshAgent.enabled)
            {
                navMeshAgent.isStopped = true;
            }
            AttackThinking();
        }
    }

    protected override void FixedUpdate()
    {
        if (currentState == EnemyState.Chasing)
        {
            if (navMeshAgent != null && navMeshAgent.enabled)
            {
                MoveToTarget(currentTarget.transform.position);
            }
        }
        else if (currentState == EnemyState.Dead)
        {
            if (_rigidbody != null)
            {
                _rigidbody.velocity = Vector3.zero;
            }
        }
    }

    protected override void Initialize()
    {
        base.Initialize(); // 부모의 Initialize()를 먼저 호출합니다.

        currentState = EnemyState.Create;
        readyBasicAttack = false;
        readySkill = false;
        readyExSkill = false;
        isUsingSkill = false;
        currentState = EnemyState.Idle;
        isIdle = true;

        currentHealth = MaxHealth;
    }

    public virtual void SetData(EnemyData data)
    {
        enemyType = data.enemyType;
        baseMaxHealth = data.maxHealth;
        baseAttackPower = data.attackPower;
        baseAttackRange = data.attackRange;
        baseAttackSpeed = data.AttackInterval;
        baseHealPower = data.HealPower;
        moveSpeed = data.moveSpeed;
        if (navMeshAgent != null) navMeshAgent.speed = moveSpeed;
        Initialize();
    }

    protected virtual void UpdateTargetAndDistance()
    {
        if (isDead)
            return;

        currentTarget = GetNearestPlayableToPosition(transform.position);
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
        }
    }

    protected override void CoolTime()
    {
        basicAttackTimer += Time.deltaTime;
        if (basicAttackTimer >= AttackSpeed)
        {
            readyBasicAttack = true;
        }
        skillTimer += Time.deltaTime;
        if (skillTimer >= skillCoolTime)
        {
            readySkill = true;
        }
        exSkillTimer += Time.deltaTime;
        if (exSkillTimer >= exSkillCoolTime)
        {
            readyExSkill = true;
        }
    }

    protected override void AttackThinking()
    {
        if (readyBasicAttack && !isUsingSkill)
        {
            BasicAttack();
        }
        else if (readySkill && !isUsingSkill)
        {
            Skill();
        }
        else if (exSkillTimer >= exSkillCoolTime)
        {
            ExSkill();
        }
    }

    protected override void BasicAttack()
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

    protected override void ShootBulletAtTarget()
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
            bullet.SetDamageFromStat(this.AttackPower);
            bullet.ShooterType = this.ObjectType;
            Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
            if (bulletRigidbody != null)
            {
                bulletRigidbody.velocity = direction * bullet.speed;
            }
        }
    }

    protected override void Skill() { }
    protected override void ExSkill() { }

    public PlayableBase GetNearestPlayableToPosition(Vector3 position)
    {
        PlayableBase nearest = null;
        float minDist = Mathf.Infinity;
        foreach (var playable in PlayableManager.instance.playables)
        {
            if (playable == null || playable.isDead) continue;
            float dist = Vector3.Distance(position, playable.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = playable;
            }
        }
        return nearest;
    }

    public Vector3 HitByExplosion(Vector3 explosionPos)
    {
        var reactVec = (transform.position - explosionPos).normalized;
        return reactVec;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<ProjectileBase>(out var projectile))
        {
            // 총알을 쏜 캐릭터와 맞는 캐릭터의 타입이 같으면 무시
            if (projectile.ShooterType == this.ObjectType)
                return;

            // 피해량 추출 및 ApplyDamage 호출
            float damage = projectile.damage;
            ApplyDamage(damage, false, null); // 폭발 피해가 아니라면 false, explosionPos는 null로 설정

            if (projectile is Bullet bullet)
                BulletPoolManager.instance.ReturnBullet(bullet);
            else
                Destroy(projectile.gameObject);
        }
    }
    public override void ApplyDamage(float damage, bool isExplosion, Vector3? explosionPos = null)
    {
        if (isDead) return;
        currentHealth -= damage;
        DamageManager.instance.ShowDamage2(headTransform.position, Mathf.FloorToInt(damage));
        if (healthBarInstance != null) healthBarInstance.SetHealth(currentHealth);
        if (currentHealth <= 0) Die();
        else StartCoroutine(OnDamage(isExplosion, explosionPos));
    }
    IEnumerator OnDamage(bool isExplosion, Vector3? explosionPos)
    {
        foreach (MeshRenderer mesh in meshs) mesh.material.color = Color.red;
        if (isExplosion && explosionPos.HasValue)
        {
            if (navMeshAgent != null) navMeshAgent.enabled = false;
            Vector3 finalVec = HitByExplosion(explosionPos.Value) + Vector3.up * 3f;
            _rigidbody.freezeRotation = false;
            _rigidbody.AddForce(finalVec * 5f, ForceMode.Impulse);
            _rigidbody.AddTorque(finalVec * 15f, ForceMode.Impulse);
        }
        yield return new WaitForSeconds(0.1f);
        foreach (MeshRenderer mesh in meshs) mesh.material.color = Color.white;
        if (isExplosion && navMeshAgent != null && !isDead) navMeshAgent.enabled = true;
    }
    protected override void Die()
    {
        base.Die();
        StageManager.instance.NotifyEnemyKilled();
    }
    protected override void OnDestroyed()
    {
        if (EnemyManager.instance != null) EnemyManager.instance.UnregisterEnemy(this);
        base.OnDestroyed();
    }
}