using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class PlayableBase : CharacterBase
{
    public override ObjectType ObjectType => ObjectType.Playable;

    [Header("Playable Settings")]
    public PlayableID playableID;
    public PlayableType playableType;

    [Header("Base Stats")]
    public float baseMaxHealth;
    public float baseAttackPower;
    public float baseAttackRange;
    public float baseAttackInterval;
    public float baseHealPower;

    [Header("Buffed Stats")]
    public float buffedMaxHealth;
    public float buffedAttackPower;
    public float buffedAttackRange;
    public float buffedAttackInterval;
    public float buffedHealPower;

    [Header("Runtime Stats")]
    public float currentHealth;
    public float MaxHealth => baseMaxHealth + buffedMaxHealth;
    public float AttackPower => baseAttackPower + buffedAttackPower;
    public float AttackRange => baseAttackRange + buffedAttackRange;
    public float AttackInterval => baseAttackInterval + buffedAttackInterval;
    public float HealPower => baseHealPower + buffedHealPower;

    [Header("Attack Settings")]
    public float basicAttackTimer;
    public float basicAttackCount;
    public float skillInterval;
    public float skillTimer;
    public float exSkillInterval;
    public float exSkillTimer;

    [Header("Movement Settings")]
    public float moveSpeed;
    public float distance;

    [Header("Playable State Flags")]
    public bool isCreate;
    public bool isIdle;
    public bool isChase;
    public bool isAttack;
    public bool isAttacking;
    public bool isBisicAttack;
    public bool isSkill;
    public bool isUsingSkill;
    public bool isExSkill;
    public bool isUsingExSkill;
    public bool isDead;
    public bool checkInAttackRenge;
    public bool readyBasicAttack;
    public bool readySkill;
    public bool readyExSkill;

    [Header("Component References")]
    public Rigidbody rigidbodyPlayable;
    public BoxCollider boxCollider;
    public MeshRenderer[] meshs;
    public NavMeshAgent navMeshAgent;
    public Animator playableAnimator;
    public Transform playableBulletFirePoint;

    [Header("Game Object References")]
    public GameObject bullet;
    public GameObject missile;
    public Transform excapeSpotTransform;

    [HideInInspector] public PlayableState currentState;
    protected EnemyBase currentTarget;

    public SkillData exSkillData;
    protected SkillBase exSkill;
    protected Vector3 exSkillTargetPosition;

    public List<SkillId> ownedSkills;
    public SkillSlot exSkillSlot;

    [Header("Buff System")]
    public List<Buff> activeBuffs = new List<Buff>();

    public Transform headTransform; // 머리 위치

    protected virtual void Awake()
    {
        rigidbodyPlayable = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        playableAnimator = GetComponentInChildren<Animator>();

        currentState = PlayableState.Create;
        isCreate = true;
        Initialize();
        readyBasicAttack = true;
        readySkill = false;
        readyExSkill = false;
    }

    protected virtual void Start()
    {
        if (PlayableManager.instance != null)
            PlayableManager.instance.RegisterPlayable(this);
        // SkillPanel panel = FindObjectOfType<SkillPanel>();
        // panel.AssignCasterToSkills(this.gameObject, ownedSkills);
    }

    protected virtual void Update()
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
            navMeshAgent.isStopped = false;
            MoveToTarget(currentTarget.transform.position);
        }

        if (currentState == PlayableState.Attack)
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
        if (currentState == PlayableState.Chasing)
        {
            MoveToTarget(currentTarget.transform.position);
        }
        else if (currentState == PlayableState.Dead)
        {
            rigidbodyPlayable.velocity = Vector3.zero;
        }
    }

    protected virtual void Initialize()
    {
        currentHealth = MaxHealth;
        isCreate = false;
        currentState = PlayableState.Idle;

        isIdle = true;
        readyBasicAttack = false;
        isUsingSkill = false;
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

        if (exSkillSlot != null)
        {
            exSkillSlot.Setup(exSkillData, this);
        }

        Initialize();
    }

    public void SetExSkill(SkillBase skill)
    {
        exSkill = skill;
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
        currentState = (distance <= AttackRange) ? PlayableState.Attack : PlayableState.Chasing;
    }

    void MoveToTarget(Vector3 targetPosition)
    {
        if (navMeshAgent != null)
        {
            navMeshAgent.SetDestination(targetPosition);
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

    protected virtual void BasicAttack()
    {
        if (!isAttack || currentTarget == null || currentTarget.isDead)
            return;

        isAttacking = true;
        isBisicAttack = true;

        playableAnimator.SetBool("isAttack", true);
        ShootBulletAtTarget();

        basicAttackTimer = 0;
        readyBasicAttack = false;
        isBisicAttack = false;
        isAttacking = false;

        playableAnimator.SetBool("isAttack", false);
        currentState = PlayableState.Idle;
    }

    protected void ShootBulletAtTarget()
    {
        if (currentTarget == null || currentTarget.isDead)
            return;

        Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        Bullet bullet = BulletPoolManager.instance.GetBullet(BulletPoolManager.PoolType.PlayableBullet);

        if (bullet != null)
        {
            bullet.transform.position = playableBulletFirePoint.position;
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

    protected virtual void Skill()
    {
    }

    protected virtual void ExSkill()
    {
        if (exSkill == null)
            return;

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
            if (enemy == null || enemy.isDead)
                continue;

            if (enemy.ObjectType == this.ObjectType)
                continue;

            float dist = Vector3.Distance(position, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearestEnemy = enemy;
            }
        }
        return nearestEnemy;
    }

    public override void ApplyDamage(float damage, bool isExplosion, Vector3? explosionPos = null)
    {
        currentHealth -= damage;

        if (currentHealth <= 0 && !isDead)
        {
            currentState = PlayableState.Dead;
            Die();
        }
        DamageManager.instance.ShowDamage(headTransform.position, Mathf.FloorToInt(damage));

        StartCoroutine(OnDamage(isExplosion));
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, MaxHealth);
    }

    protected virtual void Die()
    {
        isDead = true;
        isChase = false;
        playableAnimator.SetTrigger("doDie");
        SkillPanel.instance.ClearSkillsForCaster(this);
        Destroy(gameObject, 1.8f);
        OnDestroy();
    }

    public void OnDestroy()
    {
        if (PlayableManager.instance != null)
            PlayableManager.instance.UnregisterPlayable(this);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<ProjectileBase>(out var projectile))
        {
            if (projectile.ShooterType == ObjectType)
                return;

            if (gameObject.TryGetComponent<CharacterBase>(out var character))
            {
                if (projectile.ShooterType == character.ObjectType)
                    return;

                projectile.OnHit(gameObject);

                if (projectile is Bullet bullet)
                    BulletPoolManager.instance.ReturnBullet(bullet);
                else
                    Destroy(projectile.gameObject);
            }
        }
    }

    IEnumerator OnDamage(bool isExplosion)
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
        }
    }

    public void ApplyBuff(BuffData data)
    {
        Buff buff = BuffFactory.CreateBuffFromData(data);
        activeBuffs.Add(buff);
        Debug.Log($"버프 추가됨: {buff.targetStat} / 값: {buff.value} / 타입: {buff.applyType}");

        if (buff.applyType == BuffApplyType.Burst)
        {
            OnBuffTick(buff);
            activeBuffs.Remove(buff);
        }
        else
        {
            StartCoroutine(BuffRoutine(buff));
        }

        RecalculateBuffedStats();
    }

    public void RemoveBuff(Buff buffToRemove)
    {
        if (activeBuffs.Remove(buffToRemove))
        {
            RecalculateBuffedStats();
        }
    }

    private void RecalculateBuffedStats()
    {
        buffedMaxHealth = 0f;
        buffedAttackPower = 0f;
        buffedAttackRange = 0f;
        buffedAttackInterval = 0f;
        buffedHealPower = 0f;

        foreach (var buff in activeBuffs)
        {
            float buffValue = buff.value;

            switch (buff.applyType)
            {
                case BuffApplyType.Burst:
                case BuffApplyType.Tick:
                    switch (buff.targetStat)
                    {
                        case BuffStatType.MaxHealth:
                            buffedMaxHealth += baseMaxHealth * buffValue;
                            break;
                        case BuffStatType.AttackPower:
                            buffedAttackPower += baseAttackPower * buffValue;
                            break;
                        case BuffStatType.AttackRange:
                            buffedAttackRange += baseAttackRange * buffValue;
                            break;
                        case BuffStatType.AttackInterval:
                            buffedAttackInterval += baseAttackInterval * buffValue;
                            break;
                        case BuffStatType.HealPower:
                            buffedHealPower += baseHealPower * buffValue;
                            break;
                    }
                    break;
            }
        }

        currentHealth = Mathf.Min(currentHealth, MaxHealth);
    }

    private IEnumerator BuffRoutine(Buff buff)
    {
        float elapsed = 0f;
        Debug.Log($"BuffRoutine 시작: {buff.targetStat}, applyType: {buff.applyType}");

        if (buff.applyType == BuffApplyType.Tick)
        {
            while (elapsed < buff.duration)
            {
                yield return new WaitForSeconds(buff.tickInterval);
                OnBuffTick(buff);
                elapsed += buff.tickInterval;
            }
        }
        else
        {
            OnBuffTick(buff);
            yield return new WaitForSeconds(buff.duration);
        }

        Debug.Log($"BuffRoutine 종료: {buff.targetStat}");
        activeBuffs.Remove(buff);
        RecalculateBuffedStats();
    }

    protected virtual void OnBuffTick(Buff buff)
    {
        switch (buff.targetStat)
        {
            case BuffStatType.HealPower:
                Heal(baseHealPower * buff.value);
                break;
        }
    }
}