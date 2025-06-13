using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using Unity.VisualScripting;
using static UnityEngine.GraphicsBuffer;

public abstract class PlayableBase : CharacterBase
{
    public override ObjectType ObjectType => ObjectType.Playable;
    [Header("Playable Settings")]
    public PlayableID playableID;
    public PlayableType playableType;
    [Header("Health Stats")]
    public float maxHealth;
    public float currentHealth;
    [Header("Attack Settings")]
    public float attackPower;
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

    public SkillData exSkillData;  // 고유 스킬 데이터
    protected SkillBase exSkill;
    protected Vector3 exSkillTargetPosition;


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
    }
    protected virtual void Update()
    {
        if (isDead)
            return;

        CoolTime();

        UpdateTargetAndDistance();//여기서 현재 타겟(리타겟포함), 타겟과 거리 계속 업데이트됨

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
        currentHealth = maxHealth;
        isCreate = false;
        currentState = PlayableState.Idle;
        isIdle = true;
        readyBasicAttack = false;
        isUsingSkill = false;
    }
    public virtual void SetData(PlayableData data)
    {
        playableType = data.playableType;
        maxHealth = data.maxHealth;
        currentHealth = maxHealth;
        attackPower = data.attackPower;
        attackRange = data.attackRange;
        basicAttackInterval = data.basicAttackInterval;
        skillInterval = data.skillInterval;
        exSkillInterval = data.exSkillInterval;
        moveSpeed = data.moveSpeed;

        exSkillData = data.exSkillData; // 스킬 데이터 연결
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

        distance = Vector3.Distance(transform.position, currentTarget.transform.position);
    }
    protected virtual void CheckingAttackRenge()
    {
        currentState = (distance <= attackRange) ? PlayableState.Attack : PlayableState.Chasing;
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
        if (isAttacking) // 이미 공격중이면 리턴
            return;
        if (readyBasicAttack && !isUsingSkill && !isUsingExSkill)
        {
            BasicAttack();
            //return;
        }
        if (readySkill && !isUsingSkill && !isUsingExSkill)
        {
            Skill();
            // return;
        }
        if (exSkillTimer >= exSkillInterval && !isUsingSkill && !isUsingExSkill)
        {
            ExSkill();
            //return;
        }
    }
    protected virtual void BasicAttack()
    {
        if (!isAttack || currentTarget == null || currentTarget.isDead)
        {
            return;
        }

        isAttacking = true;
        isBisicAttack = true;
        playableAnimator.SetBool("isAttack", true);
        ShootBulletAtTarget();
        basicAttackTimer = 0;
        isBisicAttack = false;
        readyBasicAttack = false;
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
            //float buffMultiplier = 함수에서 리턴해줘야됨;
            bullet.damage = attackPower + 10;
            bullet.transform.position = playableBulletFirePoint.position;
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
        if (exSkill == null)
            return;

        SkillContext context = new SkillContext
        {
            Caster = gameObject,
            TargetPosition = exSkillTargetPosition  // 스킬 위치 지정용 변수
        };

        exSkill.Execute(context);  // 실제 스킬 실행

        exSkillTimer = 0;
        readyExSkill = false;
    }

    public EnemyBase GetNearestEnemyToPosition(Vector3 position)
    {
        EnemyBase nearestEnemy = null;
        float minDist = Mathf.Infinity;

        foreach (var playable in EnemyManager.instance.enemies)
        {
            if (playable == null)
                continue;
            float dist = Vector3.Distance(position, playable.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearestEnemy = playable;
            }
        }
        //Debug.Log(nearestEnemy);
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
        StartCoroutine(OnDamage(isExplosion));
    }

    public void Heal(float healthGain)
    {
        currentHealth = Mathf.Min(currentHealth + healthGain, maxHealth);
        Debug.Log($"[회복] {healthGain} 만큼 회복, 현재 체력: {currentHealth}");
    }
    protected virtual void Die()
    {
        isDead = true;
        isChase = false;
        playableAnimator.SetTrigger("doDie");
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
        //if (other.TryGetComponent<ProjectileBase>(out var projectile))
        //{
        //    projectile.OnHit(gameObject);
        //    var character = gameObject.GetComponent<CharacterBase>();
        //    if (character != null)
        //    {
        //        if (projectile is Bullet bullet)
        //        {
        //            BulletPoolManager.instance.ReturnBullet(bullet);
        //        }
        //        else
        //        {
        //            Destroy(projectile.gameObject);
        //        }
        //    }
        //}
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
}

