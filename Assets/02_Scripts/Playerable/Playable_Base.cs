using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public abstract class Playable_base : MonoBehaviour
{
    [Header("Player Stats")]
    public float maxHealth;
    public float currentHealth;
    public float attackRange;
    public float attackInterval;
    public float attackTimer;
    public float attackCount;
    public float moveSpeed;

    [Header("Skills and Cooldowns")]
    public float basicSkillCooldown;
    public float basicSkillTimer;
    public bool readyBasicSkill;

    [Header("State Flags")]
    public PlayableState currentState;
    public bool isChase;
    public bool isAttack;
    public bool isDead;

    [Header("Game Objects")]
    public GameObject bullet;
    public GameObject missile;

    [Header("Components")]
    public Rigidbody rigidbody_Playable;
    public BoxCollider boxCollider;
    public MeshRenderer[] meshs;
    public NavMeshAgent navMeshAgent;
    public Animator animator;

    protected Enemy_base currentTarget;

    // 추상 메서드
    protected abstract void SetStats();
    protected abstract void HandleState();
    protected abstract IEnumerator Attack();
    protected abstract IEnumerator BasicSkill();

    void Start()
    {
        //PlayableMnager.instance.RegisterPlayable(this); 
        StartCoroutine(SkillCooldownRoutine());
    }

    void Awake()
    {
        SetStats();
        InitializeComponents();
    }

    void Update()
    {
        if (isDead)
            return;

        HandleState();
    }

    void FixedUpdate()
    {
        FreezeVelocity();
    }

    // -------------------- Movement --------------------
    void FreezeVelocity()
    {
        if (isChase)
        {
            rigidbody_Playable.velocity = Vector3.zero;
            rigidbody_Playable.angularVelocity = Vector3.zero;
        }
    }

    // -------------------- Targeting & Attacking --------------------
    public void Targeting()
    {
        if (isDead)
            return;

        if (currentTarget == null || currentTarget.isDead)
        {
            currentTarget = GetNearestEnemyToPosition(transform.position);
        }

        float distance = Vector3.Distance(transform.position, currentTarget.transform.position);

        if (distance <= attackRange && !isAttack)
        {
            StartCoroutine(Attack());
        }
    }

    public Enemy_base GetNearestEnemyToPosition(Vector3 position)
    {
        Enemy_base nearestEnemy = null;
        float minDist = Mathf.Infinity;

        foreach (Enemy_base enemy in EnemyManager.instance.enemies)
        {
            if (enemy == null) continue;

            float dist = Vector3.Distance(position, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearestEnemy = enemy;
            }
        }
        return nearestEnemy;
    }

    // -------------------- Skill & Cooldown --------------------
    IEnumerator SkillCooldownRoutine()
    {
        while (!isDead)
        {
            if (!readyBasicSkill)
            {
                yield return new WaitForSeconds(basicSkillCooldown);
                readyBasicSkill = true;
            }
            else
            {
                yield return null;
            }
        }
    }

    // -------------------- Damage Handling --------------------
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDead = true;
            animator.SetBool("isDead", true);
        }
    }

    private void InitializeComponents()
    {
        rigidbody_Playable = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        boxCollider = GetComponent<BoxCollider>();
        animator = GetComponentInChildren<Animator>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        currentHealth = maxHealth;
    }
}

