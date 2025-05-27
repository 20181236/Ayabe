using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using static PoolManager;

public abstract class PlayableBase : MonoBehaviour
{
    [Header("Playable Settings")]
    public PlayableType playableType;
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
    [Header("Playable State Flags")]
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
    protected Enemybase currentTarget;

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
        if (PlayableMnager.instance != null)
            PlayableMnager.instance.RegisterPlayable(this);
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
            MoveToTarget(currentTarget.transform.position);
        }
        if (currentState == PlayableState.Attack)
        {
            isIdle = false;
            isChase = false;
            isAttack = true;
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
    }
    public virtual void SetData(PlayableData data)
    {
        playableType = data.playableType; 
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
        playableAnimator.SetBool("isAttack", true);
        navMeshAgent.isStopped = true;
        basicAttackCount++;

        ShootBulletAtTarget();

        isAttack = false;
        basicAttackCount = 0f;
        isChase = true;
        playableAnimator.SetBool("isAttack", false);
        currentState = PlayableState.Idle;

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

        Bullet bullet = PoolManager.instance.GetBullet(PoolManager.PoolType.EnemyBullet);

        if (bullet != null)
        {
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
    }
    public Enemybase GetNearestEnemyToPosition(Vector3 position)
    {
        Enemybase nearestEnemy = null;
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
    //-------------데미지 고쳐야됨
    protected virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0 && !isDead)
        {
            currentState = PlayableState.Dead;
            Die();
        }
    }
    public void ApplyDamage(float damage, Vector3 reactVec, bool isGrenade)
    {
        currentHealth -= damage;
        StartCoroutine(OnDamage(reactVec, isGrenade));
    }
   //------------------------------------
    protected virtual void Die()
    {
        isDead = true;
        playableAnimator.SetTrigger("doDie");
        OnDestroy();
    }
    public void OnDestroy()
    {
        if (PlayableMnager.instance != null)
            PlayableMnager.instance.UnregisterPlayable(this);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyBullet"))
        {
            Bullet bullet = other.GetComponent<Bullet>();
            Vector3 reactVec = transform.position - other.transform.position;
            //Destroy(other.gameObject);
            ApplyDamage(10, reactVec, false);
        }
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
            playableAnimator.SetTrigger("doDie");

            reactVec = reactVec.normalized + Vector3.up * (isGrenade ? 3f : 1f);
            rigidbodyPlayable.freezeRotation = false;
            rigidbodyPlayable.AddForce(reactVec * 5, ForceMode.Impulse);

            Destroy(gameObject, 1.8f);
        }
    }
}

