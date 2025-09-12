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

    public Transform enemyBulletFirePoint;

    [HideInInspector] public EnemyState currentState;
    protected PlayableBase currentTarget;
    public PlayableBase CurrentTarget => currentTarget;

    public float Distance => distance;

    private InterfaceBehaviorTreeNode rootNode;

    protected override void Awake()
    {
        base.Awake();
        if (EnemyManager.instance != null) EnemyManager.instance.RegisterEnemy(this);
        Initialize();
        //if (navMeshAgent != null)
        //{
        //    navMeshAgent.updatePosition = true; // 직접 위치를 업데이트하도록 변경
        //}
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

    public virtual void SetData(EnemyData data)
    {
        baseMaxHealth = data.maxHealth;
        baseAttackPower = data.attackPower;
        baseAttackRange = data.attackRange;
        baseAttackSpeed = data.attackSpeed;
        baseHealPower = data.HealPower;
        baseMoveSpeed = data.moveSpeed;
        if (navMeshAgent != null) 
            navMeshAgent.speed = baseMoveSpeed;
        Initialize();
    }

    protected override void Initialize()
    {
        base.Initialize();
        BuildBehaviorTree();
        currentHealth = MaxHealth;
        currentTarget = null;
        distance = 0f;
    }

    private void BuildBehaviorTree()
    {
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

        var attackOrStandbySelector = new BehaviorTreeSelectorNode();
        attackOrStandbySelector.Initialize(new List<InterfaceBehaviorTreeNode>
        {
            basicAttackNode,
            standbyNode
        });

        var inRangeSequence = new BehaviorTreeSequenceNode();
        inRangeSequence.Initialize(new List<InterfaceBehaviorTreeNode>
        {
            isTargetInRangeNode,
            attackOrStandbySelector
        });

        var inRangeOrChaseSelector = new BehaviorTreeSelectorNode();
        inRangeOrChaseSelector.Initialize(new List<InterfaceBehaviorTreeNode>
        {
            inRangeSequence,
            chaseTargetNode
        });

        var targetActionSequence = new BehaviorTreeSequenceNode();
        targetActionSequence.Initialize(new List<InterfaceBehaviorTreeNode>
        {
            hasTargetNode,
            inRangeOrChaseSelector
        });

        var rootSelector = new BehaviorTreeSelectorNode();
        rootSelector.Initialize(new List<InterfaceBehaviorTreeNode>
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
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(targetPosition);
        }
    }

    // OnAnimatorMove는 애니메이션 루트 모션을 사용하지 않으므로 비워둡니다.
    //protected virtual void OnAnimatorMove() { }

    protected override void CoolTime()
    {
        basicAttackTimer += Time.deltaTime;
        if (basicAttackTimer >= AttackSpeed)
        {
            readyBasicAttack = true;
            isAttacking = false; // 공격 쿨타임이 다 차면 공격 가능 상태로 변경
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
        if (currentTarget == null || currentTarget.isDead) 
            return;


        readyBasicAttack = false;
        basicAttackTimer = 0f;
        isAttacking = true; // 공격 실행 상태로 변경

        ShootBulletAtTarget();
        Debug.Log("총알발싸까지옴");
    }

    public void ExecuteBasicAttack()
    {
        BasicAttack();
    }

    // OnAttackAnimationEnd는 더 이상 필요하지 않으므로 비워두거나 삭제할 수 있습니다.
    //public void OnAttackAnimationEnd() { }

    protected override void ShootBulletAtTarget()
    {
        // 1. 함수가 시작되었는지 확인
        Debug.Log("--- ShootBulletAtTarget 함수 시작 ---");

        if (currentTarget == null || currentTarget.isDead)
        {
            Debug.LogWarning("타겟이 없거나 죽어서 총알 발사를 취소합니다.");
            return;
        }

        Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        // 2. BulletPoolManager에서 총알을 가져오기 직전인지 확인
        Debug.Log("BulletPoolManager에서 총알을 가져옵니다...");
        Bullet bullet = BulletPoolManager.instance.GetBullet(BulletPoolManager.PoolType.EnemyBullet);

        // 3. GetBullet의 결과를 확인하는 것이 가장 중요합니다!
        if (bullet != null)
        {
            // 이 로그가 뜬다면, 총알 객체는 성공적으로 받아온 것입니다.
            Debug.Log("<color=green>성공: 총알을 풀에서 가져왔습니다!</color> 이제 총알 위치를 설정합니다.");

            if (enemyBulletFirePoint == null)
            {
                Debug.LogError("치명적 오류: 'enemyBulletFirePoint'가 Inspector에 할당되지 않았습니다!", this.gameObject);
                return;
            }

            bullet.transform.position = enemyBulletFirePoint.position;
            bullet.transform.rotation = Quaternion.LookRotation(direction);
            bullet.SetDamageFromStat(this.AttackPower);
            bullet.ShooterType = this.ObjectType;

            Debug.Log("<color=cyan>총알 설정 완료!</color>");
        }
        else
        {
            // 이 에러 로그가 뜬다면, 풀에서 총알을 가져오는 데 실패한 것입니다.
            Debug.LogError("<color=red>실패: BulletPoolManager.GetBullet()이 null을 반환했습니다.</color> 풀이 비었거나 설정이 잘못되었을 수 있습니다.");
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
            if (playable == null || playable.isDead) 
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

    public Vector3 HitByExplosion(Vector3 explosionPos)
    {
        var reactVec = (transform.position - explosionPos).normalized;
        return reactVec;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<ProjectileBase>(out var projectile))
        {
            if (projectile.ShooterType == this.ObjectType)
                return;

            float damage = projectile.damage;
            ApplyDamage(damage, false, null);

            if (projectile is Bullet bullet)
                BulletPoolManager.instance.ReturnBullet(bullet);
            else
                Destroy(projectile.gameObject);
        }
    }

    public override void ApplyDamage(float damage, bool isExplosion, Vector3? explosionPos = null)
    {
        if (isDead) 
            return;

        currentHealth -= damage;

        DamageManager.instance.ShowDamage2(headTransform.position, Mathf.FloorToInt(damage));

        if (healthBarInstance != null) 
            healthBarInstance.SetHealth(currentHealth);

        if (currentHealth <= 0) 
            Die();
        else 
            StartCoroutine(OnDamage(isExplosion, explosionPos));
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

        foreach 
            (MeshRenderer mesh in meshs) mesh.material.color = Color.white;
        if (isExplosion && navMeshAgent != null && !isDead)
            navMeshAgent.enabled = true;
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
    }
}