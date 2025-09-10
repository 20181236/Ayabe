using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public abstract class PlayableBase : CharacterBase
{
    public override ObjectType ObjectType => ObjectType.Playable;
    public enum PlayableState { Create, Idle, Chasing, Attack, Standby, Dead, Victory }

    [Header("Playable Settings")]
    public PlayableID playableID;
    public PlayableType playableType;

    [Header("Game Object References")]
    public WeaponType equippedWeapon;
    public GameObject bullet;
    public GameObject missile;
    public Transform excapeSpotTransform;

    [HideInInspector] public EnemyBase currentTarget;

    [Header("Skill System")]
    public SkillData exSkillData;
    protected SkillBase exSkill;
    protected Vector3 exSkillTargetPosition;
    public List<SkillId> ownedSkills;
    public SkillSlot exSkillSlot;

    // 상태 패턴 관련 변수들 
    private Dictionary<PlayableState, PlayableStateInterface> states;
    private PlayableStateInterface _currentState;
    public PlayableStateInterface CurrentState => _currentState;

    public bool IsIdle => _currentState is IdleState;
    public bool IsChasing => _currentState is ChaseState;
    public bool IsStandby => _currentState is StandbyState;
    public bool IsAttack => _currentState is AttackState;


    protected override void Awake()
    {
        base.Awake();

        InitializeStates();

        if (PlayableManager.instance != null)
            PlayableManager.instance.RegisterPlayable(this);

        readyBasicAttack = true;
        readySkill = false;
        readyExSkill = false;
    }

    protected override void Start()
    {
        base.Start(); // InitHealthBar()
        TransitionToState(PlayableState.Create);
    }

    protected override void Update()
    {
        if (isDead) return;
        CoolTime();
        _currentState?.Update();
    }

    protected override void Initialize()
    {
        base.Initialize();
        isUsingSkill = false;
        if (navMeshAgent != null)
            navMeshAgent.speed = baseMoveSpeed;
    }

    private void InitializeStates()
    {
        states = new Dictionary<PlayableState, PlayableStateInterface>
        {
            { PlayableState.Create, new CreateState() },
            { PlayableState.Idle, new IdleState() },
            { PlayableState.Chasing, new ChaseState() },
            { PlayableState.Attack, new AttackState() },
            { PlayableState.Standby, new StandbyState() },
            { PlayableState.Dead, new DeadState() }
        };
    }
    public void TransitionToState(PlayableState nextStateKey)
    {
        if (!states.ContainsKey(nextStateKey))
        {
            Debug.LogError($"상태 Dictionary에 {nextStateKey}가 존재하지 않습니다!");
            return;
        }
        _currentState?.Exit();
        _currentState = states[nextStateKey];
        _currentState.Enter(this);
    }

    public virtual void SetData(PlayableData data)
    {
        playableType = data.playableType;
        baseMaxHealth = data.maxHealth;

        baseAttackPower = data.attackPower;
        baseAttackRange = data.attackRange;
        baseAttackSpeed = data.AttackSpeed; // 이 변수는 없습니다.


        baseHealPower = data.HealPower;

        baseMoveSpeed = data.moveSpeed;

        skillCoolTime = data.skillCoolTime;

        exSkillData = data.exSkillData;
        exSkillCoolTime = data.exSkillCoolTime;

        if (navMeshAgent != null) navMeshAgent.speed = baseMoveSpeed;
        if (exSkillSlot != null) exSkillSlot.Setup(exSkillData, this);

        Initialize();
    }

    public void SetExSkill(SkillBase skill)
    {
        exSkill = skill;
    }

    // PlayableBase.cs

    protected override void CoolTime()
    {
        basicAttackTimer += Time.deltaTime;

        if (AttackSpeed > 0)
        {
            // 공격 간격은 '1 / 공격 속도'가 됩니다.
            readyBasicAttack = basicAttackTimer >= (1f / AttackSpeed);
        }
        else
        {
            readyBasicAttack = false;
        }

        skillTimer += Time.deltaTime;
        readySkill = skillTimer >= skillCoolTime;

        exSkillTimer += Time.deltaTime;
        readyExSkill = exSkillTimer >= exSkillCoolTime;
    }

    public virtual void UpdateTargetAndDistance()
    {
        if (isDead) return;
        currentTarget = GetNearestEnemyToPosition(transform.position);
        distance = (currentTarget != null) ? Vector3.Distance(transform.position, currentTarget.transform.position) : Mathf.Infinity;
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

    protected override void Die()
    {
        if (isDead) return;
        base.Die();
        SkillPanel.instance.ClearSkillsForCaster(this);
        TransitionToState(PlayableState.Dead);
    }

    protected override void OnDestroyed()
    {
        if (PlayableManager.instance != null)
            PlayableManager.instance.UnregisterPlayable(this);
        base.OnDestroyed();
    }

    //사용법1
    public void ExecuteAttackAction()
    {
        AttackThinking();
    }

    protected override void AttackThinking()
    {
        if (isAttacking) return;
        if (readyBasicAttack && !isUsingSkill && !isUsingExSkill)
        {
            BasicAttack();
            return;
        }
        if (readySkill && !isUsingSkill && !isUsingExSkill)
        {
            Skill();
            return;
        }
        if (readyExSkill && !isUsingSkill && !isUsingExSkill)
        {
            ExSkill();
        }
        //DoAttackLogic();
    }
    //사용법2
    //private void DoAttackLogic()
    //{
    //    if (isAttacking) return;
    //    if (readyBasicAttack && !isUsingSkill && !isUsingExSkill)
    //    {
    //        BasicAttack();
    //        return;
    //    }
    //    if (readySkill && !isUsingSkill && !isUsingExSkill)
    //    {
    //        Skill();
    //        return;
    //    }
    //    if (readyExSkill && !isUsingSkill && !isUsingExSkill)
    //    {
    //        ExSkill();
    //    }
    //}

    protected override void BasicAttack()
    {
        if (currentTarget == null || currentTarget.isDead)
            return;

        basicAttackTimer = 0f;
        readyBasicAttack = false;

        StartCoroutine(DoBasicAttack());
    }

    private IEnumerator DoBasicAttack()
    {
        // 안전장치 (AttackSpeed가 0일 경우 방지)
        if (AttackSpeed <= 0)
        {
            Debug.LogError($"{name}의 AttackSpeed가 0 또는 음수입니다!");
            isAttacking = false;
            yield break;
        }

        isAttacking = true;

        float animSpeed = this.AttackSpeed;
        animator.SetFloat("attackSpeed", animSpeed);
        animator.SetTrigger("isAttack");

        yield return new WaitForEndOfFrame();

        float animLength = animator.GetCurrentAnimatorStateInfo(0).length;

        // 코루틴은 애니메이션이 끝날 때까지만 기다립니다.
        yield return new WaitForSeconds(animLength / this.AttackSpeed);

        isAttacking = false;
    }

    // 애니메이션 이벤트에서 총알 발사 Add Event잊지말것
    public void OnAttackHit()
    {
        ShootBulletAtTarget();
    }

    protected override void ShootBulletAtTarget()
    {
        if (currentTarget == null || currentTarget.isDead) return;

        Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        // 사용자의 BulletPoolManager에 따라 수정이 필요할 수 있습니다.
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

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<ProjectileBase>(out var projectile))
        {
            if (projectile.ShooterType == this.ObjectType) return;
            float damage = projectile.damage;
            ApplyDamage(damage, false);
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
        if (healthBarInstance != null)
            healthBarInstance.SetHealth(currentHealth);
        StartCoroutine(OnDamage(isExplosion));
    }

    IEnumerator OnDamage(bool isExplosion)
    {
        foreach (MeshRenderer mesh in meshs) mesh.material.color = Color.red;
        if (isExplosion && navMeshAgent != null && navMeshAgent.enabled)
            navMeshAgent.enabled = false;
        yield return new WaitForSeconds(0.1f);
        if (isExplosion && navMeshAgent != null && !isDead)
            navMeshAgent.enabled = true;
        Color restoreColor = currentHealth > 0 ? Color.white : Color.gray;
        foreach (MeshRenderer mesh in meshs) mesh.material.color = restoreColor;
    }

    // 추가: 애니메이션 이벤트에서 호출
    public void OnSpawnAnimationEnd()
    {
        // 현재 상태가 CreateState일 때만 Idle로 전환
        if (CurrentState is CreateState)
        {
            TransitionToState(PlayableState.Idle);
        }
    }

    public void EnableActions()
    {
        if (navMeshAgent != null) navMeshAgent.isStopped = false;
        isAttacking = false;
        isUsingSkill = false;
    }

    // 캐릭터 행동 정지
    public void DisableActions()
    {
        if (navMeshAgent != null) navMeshAgent.isStopped = true;
        isAttacking = true;
        isUsingSkill = true;
    }

    // State Pattern으로 대체되어 제거된 메서드들:
    // - CheckingAttackRange()
    // - HandleState()
    // - MoveToTarget()
    private void OnDrawGizmosSelected()
    {
        // 공격 사거리를 나타내는 원을 그립니다.
        Gizmos.color = Color.red; // 원의 색상을 빨간색으로 설정
        Gizmos.DrawWireSphere(transform.position, AttackRange); // 현재 위치를 중심으로 AttackRange만큼의 반지름을 가진 원을 그림
    }
}