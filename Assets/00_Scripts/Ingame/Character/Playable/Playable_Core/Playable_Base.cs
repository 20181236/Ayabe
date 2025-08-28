using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public abstract class PlayableBase : CharacterBase
{
    public override ObjectType ObjectType => ObjectType.Playable;
    public enum PlayableState { Idle, Chasing, Attack, Dead, Create }

    [Header("Playable Settings")]
    public PlayableID playableID;
    public PlayableType playableType;

    [Header("Game Object References")]
    public WeaponType equippedWeapon;
    public GameObject bullet;
    public GameObject missile;
    public Transform excapeSpotTransform;

    // CharacterBase의 공통 변수(_rigidbody, animator 등)를 사용하므로 이 변수들은 제거합니다.
    // public Rigidbody rigidbodyPlayable;
    // public Animator playableAnimator;
    // public Transform playableBulletFirePoint;

    [HideInInspector] public PlayableState currentState;
    protected EnemyBase currentTarget;

    [Header("Skill System")]
    public SkillData exSkillData;
    protected SkillBase exSkill;
    protected Vector3 exSkillTargetPosition;
    public List<SkillId> ownedSkills;
    public SkillSlot exSkillSlot;

    protected override void Awake()
    {
        // 부모 클래스의 Awake를 먼저 호출합니다.
        base.Awake();

        // 중복되는 GetComponent는 CharacterBase에서 이미 처리되었으므로 삭제합니다.
        // rigidbodyPlayable = GetComponent<Rigidbody>();
        // navMeshAgent = GetComponent<NavMeshAgent>();
        // playableAnimator = GetComponentInChildren<Animator>();
        // buffManager = GetComponent<BuffManager>();

        if (PlayableManager.instance != null)
            PlayableManager.instance.RegisterPlayable(this);

        currentState = PlayableState.Create;
        isCreate = true;
        Initialize();
        readyBasicAttack = true;
        readySkill = false;
        readyExSkill = false;
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

        if (currentState == PlayableState.Chasing)
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

        if (currentState == PlayableState.Attack)
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
        if (currentState == PlayableState.Chasing)
        {
            if (navMeshAgent != null && navMeshAgent.enabled)
            {
                MoveToTarget(currentTarget.transform.position);
            }
        }
        else if (currentState == PlayableState.Dead)
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

        // 중복되는 코드를 제거합니다.
        // currentHealth = MaxHealth;
        // isCreate = false;
        currentState = PlayableState.Idle;

        isIdle = true;
        // readyBasicAttack = false; // base.Initialize에서 처리
        isUsingSkill = false;

        if (navMeshAgent != null)
            navMeshAgent.speed = moveSpeed;
    }

    public virtual void SetData(PlayableData data)
    {
        playableType = data.playableType;
        baseMaxHealth = data.maxHealth;
        baseAttackPower = data.attackPower;
        baseAttackRange = data.attackRange;
        baseAttackInterval = data.AttackInterval;
        baseHealPower = data.HealPower;
        moveSpeed = data.moveSpeed;
        skillInterval = data.skillInterval;
        exSkillData = data.exSkillData;
        exSkillInterval = data.exSkillInterval;
        if (navMeshAgent != null) navMeshAgent.speed = moveSpeed;
        if (exSkillSlot != null) exSkillSlot.Setup(exSkillData, this);
        Initialize();
    }

    public void SetExSkill(SkillBase skill)
    {
        exSkill = skill;
    }

    // 이 메서드들은 override 키워드를 명시적으로 추가합니다.
    protected override void CoolTime()
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

    protected virtual void UpdateTargetAndDistance()
    {
        if (isDead)
            return;
        currentTarget = GetNearestEnemyToPosition(transform.position);
        if (currentTarget == null)
            return;
        if (currentTarget.ObjectType == this.ObjectType)
        {
            currentTarget = null;
            return;
        }
        distance = Vector3.Distance(transform.position, currentTarget.transform.position);
    }

    protected virtual void CheckingAttackRenge()
    {
        if (currentTarget == null)
        {
            currentState = PlayableState.Idle;
            return;
        }
        currentState = (distance <= AttackRange) ? PlayableState.Attack : PlayableState.Chasing;
    }

    void MoveToTarget(Vector3 targetPosition)
    {
        if (navMeshAgent != null)
        {
            navMeshAgent.SetDestination(targetPosition);
        }
    }
    protected override void AttackThinking()
    {
        if (isAttacking)
            return;
        if (readyBasicAttack && !isUsingSkill && !isUsingExSkill)
        {
            BasicAttack();
        }
        if (readySkill && !isUsingSkill && !isUsingExSkill)
        {
            Skill();
        }
        if (exSkillTimer >= exSkillInterval && !isUsingSkill && !isUsingExSkill)
        {
            //ExSkill();
        }
    }

    protected override void BasicAttack()
    {
        if (!isAttack || currentTarget == null || currentTarget.isDead)
            return;
        isAttacking = true;
        isBasicAttack = true;
        animator.SetBool("isAttack", true);
        ShootBulletAtTarget();
        basicAttackTimer = 0;
        readyBasicAttack = false;
        isBasicAttack = false;
        isAttacking = false;
        animator.SetBool("isAttack", false);
        currentState = PlayableState.Idle;
    }

    protected override void ShootBulletAtTarget()
    {
        if (currentTarget == null || currentTarget.isDead)
            return;
        Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        Bullet bullet = BulletPoolManager.instance.GetBullet(BulletPoolManager.PoolType.PlayableBullet);
        if (bullet != null)
        {
            bullet.transform.position = bulletFirePoint.position;
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
    protected override void ExSkill()
    {
        if (exSkill == null) return;
        SkillContext context = new SkillContext
        {
            Caster = gameObject,
            TargetPosition = exSkillTargetPosition
        };
        exSkill.Execute(context);
        exSkillTimer = 0;
        readyExSkill = false;
    }

    public EnemyBase GetNearestEnemyToPosition(Vector3 position)
    {
        EnemyBase nearestEnemy = null;
        float minDist = Mathf.Infinity;
        foreach (var enemy in EnemyManager.instance.enemies)
        {
            if (enemy == null || enemy.isDead) continue;
            float dist = Vector3.Distance(position, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearestEnemy = enemy;
            }
        }
        return nearestEnemy;
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
            ApplyDamage(damage, false); // 폭발 피해가 아니라면 false로 설정

            if (projectile is Bullet bullet)
                BulletPoolManager.instance.ReturnBullet(bullet);
            else
                Destroy(projectile.gameObject);
        }
    }

    public override void ApplyDamage(float damage, bool isExplosion, Vector3? explosionPos = null)
    {
        currentHealth -= damage;
        if (currentHealth <= 0 && !isDead) Die();
        DamageManager.instance.ShowDamage(headTransform.position, Mathf.FloorToInt(damage));
        if (healthBarInstance != null) healthBarInstance.SetHealth(currentHealth);
        StartCoroutine(OnDamage(isExplosion));
    }
    protected override void Die()
    {
        base.Die();
        SkillPanel.instance.ClearSkillsForCaster(this);
    }
    protected override void OnDestroyed()
    {
        if (PlayableManager.instance != null) PlayableManager.instance.UnregisterPlayable(this);
        base.OnDestroyed();
    }
    IEnumerator OnDamage(bool isExplosion)
    {
        foreach (MeshRenderer mesh in meshs) mesh.material.color = Color.red;
        if (isExplosion)
        {
            if (navMeshAgent != null && navMeshAgent.enabled) navMeshAgent.enabled = false;
        }
        yield return new WaitForSeconds(0.1f);
        if (isExplosion && navMeshAgent != null && !isDead) navMeshAgent.enabled = true;
        if (currentHealth > 0)
        {
            foreach (MeshRenderer mesh in meshs) mesh.material.color = Color.white;
        }
        else
        {
            foreach (MeshRenderer mesh in meshs) mesh.material.color = Color.gray;
        }
    }
}