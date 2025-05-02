using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Playable_base : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    public float attackRange;
    public float attackInterval;
    public float attackTimer;
    public float attackCount;
    public float moveSpeed;

    public bool isCreate;
    public bool isChase;
    public bool isAttack;
    public bool readyAttack;
    public bool isDead;

    public Rigidbody rigidbodyPlayable;
    public BoxCollider boxCollider;
    public MeshRenderer[] meshs;
    public NavMeshAgent navMeshAgent;
    public Animator animator;

    public GameObject playableBullet;

    [HideInInspector] public PlayableState currentState;

    protected SoonDoBu_Playable currentTarget;

    protected virtual void Awake()
    {
        SetStats();
        rigidbodyPlayable = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        currentHealth = maxHealth;
        currentState = PlayableState.Create;
    }
    protected virtual void Start()
    {
        if (PlayableMnager.instance != null)
            PlayableMnager.instance.RegisterPlayable_imsi(this);
    }

    protected virtual void Update()
    {
        if (isDead)
            return;
        HandleState();
        AttackCoolTime();
    }

    protected virtual void FixedUpdate()
    {
        // 예시: 추적 상태에서 타겟을 향해 계속 이동
        if (currentState == PlayableState.Chasing)
        {
            MoveToTarget(currentTarget.transform.position);
        }
        else if (currentState == PlayableState.Dead)
        {
            // 죽었을 때 더 이상 이동하지 않음
            rigidbodyPlayable.velocity = Vector3.zero;
        }
    }

    protected virtual void SetStats() { }

    protected virtual void HandleState()
    {
        switch (currentState)
        {
            case PlayableState.Create:
                Initialize();
                break;

            case PlayableState.Idle:
                Debug.Log("idle");
                Targeting();
                break;

            case PlayableState.Chasing:
                Debug.Log("Chasing");
                currentTarget = GetNearestEnemyToPosition(transform.position);

                if (currentTarget != null)
                {
                    MoveToTarget(currentTarget.transform.position);
                }
                break;

            case PlayableState.Attack:
                Attack();
                break;

            case PlayableState.Dead:
                Die();
                break;

            case PlayableState.Skill:
            case PlayableState.ExSkill:
                // 스킬 처리
                break;
        }
    }
    protected virtual void Initialize()
    {
        isCreate = false;
        isChase = true;
        currentState = PlayableState.Idle;
    }

    public virtual void Targeting()
    {
        if (isDead)
            return;

        currentTarget = GetNearestEnemyToPosition(transform.position);

        if (currentTarget == null)
            return;

        float distance = Vector3.Distance(transform.position, currentTarget.transform.position);

        if (distance > attackRange)
        {
            currentState = PlayableState.Chasing;
        }
        else if (distance <= attackRange && attackTimer >= attackInterval)
        {
            currentState = PlayableState.Attack;
        }
    }

    void MoveToTarget(Vector3 targetPosition)
    {
        if (navMeshAgent != null)
        {
            navMeshAgent.SetDestination(targetPosition);

            float distance = Vector3.Distance(transform.position, targetPosition);

            if (distance <= attackRange && attackTimer >= attackInterval)
            {
                navMeshAgent.isStopped = true;
                currentState = PlayableState.Attack;
            }
            else
            {
                navMeshAgent.isStopped = false;
            }
        }
    }

    protected virtual void AttackCoolTime()
    {
        attackTimer += Time.deltaTime;
        if (attackTimer >= attackInterval)
            readyAttack = true;
    }

    protected virtual void Attack()
    {
        isChase = false;
        isAttack = true;
        animator.SetBool("isAttack", true);
        attackCount++;

        if (currentTarget == null || currentTarget.isDead)
        {
            isAttack = false;
            isChase = true;
            animator.SetBool("isAttack", false);

            currentTarget = null;
            currentState = PlayableState.Idle;
            attackTimer = 0f;
            return;
        }
        ShootBulletAtTarget();

        isAttack = false;
        isChase = true;
        attackTimer = 0f;
        animator.SetBool("isAttack", false);
        currentState = PlayableState.Idle;
    }

    protected void ShootBulletAtTarget()
    {
        if (currentTarget == null || currentTarget.isDead)
            return;

        Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        GameObject bullet = Instantiate(playableBullet,
            transform.position + Vector3.up * 3f,
            Quaternion.LookRotation(direction));
        bullet.GetComponent<Rigidbody>().velocity = direction * 20;
    }


    public SoonDoBu_Playable GetNearestEnemyToPosition(Vector3 position)
    {
        SoonDoBu_Playable nearest = null;
        float minDist = Mathf.Infinity;

        foreach (var playable in PlayableMnager.instance.playables)
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
        animator.SetTrigger("doDie");
        OnDestroy();
    }

    private void OnDestroy()
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
            Destroy(other.gameObject);
            ApplyDamage(bullet.damage, reactVec, false);
        }
    }


    public void ApplyDamage(float damage, Vector3 reactVec, bool isGrenade)
    {
        currentHealth -= damage;
        StartCoroutine(OnDamage(reactVec, isGrenade));
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
            animator.SetTrigger("doDie");

            reactVec = reactVec.normalized + Vector3.up * (isGrenade ? 3f : 1f);
            rigidbodyPlayable.freezeRotation = false;
            rigidbodyPlayable.AddForce(reactVec * 5, ForceMode.Impulse);

            if (isGrenade)
                rigidbodyPlayable.AddTorque(reactVec * 15, ForceMode.Impulse);

            Destroy(gameObject, 1.8f);
        }
    }
}
