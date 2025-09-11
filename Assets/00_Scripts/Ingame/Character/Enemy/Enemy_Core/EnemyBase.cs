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
    public PlayableBase CurrentTarget => currentTarget;  // 읽기 전용

    //protected float distance;
    public float Distance => distance;  // 읽기 전용

    private InterfaceBehaviorTreeNode rootNode;


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

        rootNode?.Evaluate();
    }

    


    protected override void FixedUpdate()
    {

    }

    //팩토리에서 이걸 참고해서 인스턴스하고 그다음 Initialize()가 이뤄지기떄문에 위에
    public virtual void SetData(EnemyData data)
    {
        enemyType = data.enemyType;
        baseMaxHealth = data.maxHealth;
        baseAttackPower = data.attackPower;
        baseAttackRange = data.attackRange;
        baseAttackSpeed = data.AttackInterval;
        baseHealPower = data.HealPower;
        baseMoveSpeed = data.moveSpeed;
        if (navMeshAgent != null) navMeshAgent.speed = baseMoveSpeed;
        Initialize();
    }

    protected override void Initialize()
    {
        base.Initialize(); // 부모의 Initialize()를 먼저 호출합니다.
        BuildBehaviorTree();
        currentHealth = MaxHealth;
        currentTarget = null;
        distance = 0f;
    }

    //  EnemyBehaviorTree의 역할을 이 함수가 대신합니다.
    private void BuildBehaviorTree()
    {
        // --- 1. 말단 노드들 생성 및 초기화 ---
        var hasTargetNode = new BehaviorTreeConditionNode();
        hasTargetNode.Initialize(() => EnemyConditions.HasTarget(this));

        var isTargetInRangeNode = new BehaviorTreeConditionNode();
        isTargetInRangeNode.Initialize(() => EnemyConditions.IsTargetInAttackRange(this));

        var basicAttackNode = new BehaviorTreeActionNode();
        basicAttackNode.Initialize(() => EnemyActions.BasicAttack(this));

        var chaseTargetNode = new BehaviorTreeActionNode();
        chaseTargetNode.Initialize(() => EnemyActions.ChaseTarget(this));

        var standbyNode = new BehaviorTreeActionNode();
        standbyNode.Initialize(() => EnemyActions.Standby(this));

        var idleNode = new BehaviorTreeActionNode();
        idleNode.Initialize(() => EnemyActions.Idle(this));

        // --- 2. 중간 계층 노드 생성 및 초기화 (모두 2단계 패턴 사용) ---

        // [레벨 3] 공격 또는 대기 선택
        var attackOrStandbySelector = new BehaviorTreeSelectorNode(); // 1. 생성
        attackOrStandbySelector.Initialize(new List<InterfaceBehaviorTreeNode> // 2. 초기화
    {
        basicAttackNode,
        standbyNode
    });

        // [레벨 2] 사거리 안 또는 밖의 행동 결정

        // 사거리 안일 때의 행동 (시퀀스)
        var inRangeSequence = new BehaviorTreeSequenceNode(); // 1. 생성
        inRangeSequence.Initialize(new List<InterfaceBehaviorTreeNode> // 2. 초기화
    {
        isTargetInRangeNode,
        attackOrStandbySelector
    });

        var inRangeOrChaseSelector = new BehaviorTreeSelectorNode(); // 1. 생성
        inRangeOrChaseSelector.Initialize(new List<InterfaceBehaviorTreeNode> // 2. 초기화
    {
        inRangeSequence,
        chaseTargetNode
    });

        // [레벨 1] 타겟 유무에 따른 행동 결정
        var targetActionSequence = new BehaviorTreeSequenceNode(); // 1. 생성
        targetActionSequence.Initialize(new List<InterfaceBehaviorTreeNode> // 2. 초기화
    {
        hasTargetNode,
        inRangeOrChaseSelector
    });

        // [레벨 0] 최종 루트 노드
        var rootSelector = new BehaviorTreeSelectorNode(); // 1. 생성
        rootSelector.Initialize(new List<InterfaceBehaviorTreeNode> // 2. 초기화
    {
        targetActionSequence,
        idleNode
    });

        rootNode = rootSelector;
    }

    protected virtual void UpdateTargetAndDistance()
    {
        currentTarget = GetNearestPlayableToPosition(transform.position);
        if (currentTarget != null)
            distance = Vector3.Distance(transform.position, currentTarget.transform.position);
    }

    public void MoveToTarget(Vector3 targetPosition)
    {
        if (navMeshAgent != null && navMeshAgent.enabled)
        {
            navMeshAgent.isStopped = false; // 이동 시작 시 항상 isStopped를 false로 설정
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

    protected override void BasicAttack()
    {
        if (currentTarget == null || currentTarget.isDead) return;

        //여기에 쿨타임 초기화 로직을 추가합니다.
        readyBasicAttack = false; // 공격 준비 상태를 false로 변경
        basicAttackTimer = 0;     // 타이머를 0으로 리셋

        isAttacking = true;
        animator.SetBool("isAttack", true);

        ShootBulletAtTarget();
    }

    public void ExecuteBasicAttack()
    {
        BasicAttack();
    }
    public void OnAttackAnimationEnd()
    {
        isAttacking = false;
        animator.SetBool("isAttack", false);
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