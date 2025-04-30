using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;

public class Enemy : MonoBehaviour
{
    public EnemyType enemyType;

    public float maxHealth;
    public float currentHealth;
    public float attackRange;

    public int attackCount;
    public float attackInterval = 2f;//2초에 한번씩쏘고
    public float attackTimer = 0f;//인터벌비교변수이고

    public float skillCooldown = 15f;//스킬 쿨타임이고
    public float skillTimer = 0f;//스킬 쿨타임을 비교할 변수

    public bool isChase;
    public bool isAttack;
    public bool isDead;

    public Rigidbody rigidbodyEnemy;
    public BoxCollider boxCollider;
    public MeshRenderer[] meshs;
    public NavMeshAgent navMeshAgent;
    public Animator animator;
    public GameObject enemyBullet;

    SoonDoBu_Playable currentTarget;

    private void Start()
    {
        EnemyManager.instance.RegisterEnemy(this);
    }
    void Awake()
    {
        SetStats();
        rigidbodyEnemy = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        currentHealth = maxHealth;

        //Invoke("ChaseStart", 2f);

    }

    void Update()
    {
        Targeting();
        attackTimer += Time.deltaTime;
    }

    protected virtual void SetStats()
    {
        maxHealth = (float)EnemyHelath.Thanker;
        currentHealth = 0f;
        attackRange = 10.0f;
    }

    public void Targeting()
    {
        if (isDead)
            return;

        // 타겟이 null이거나 죽었으면 다시 찾기
        if (currentTarget == null || currentTarget.isDead)
        {
            currentTarget = GetNearestEnemyToPosition(transform.position);
        }

        if (currentTarget == null)
            return; // 살아있는 적이 아예 없을 때

        float distance = Vector3.Distance(transform.position, currentTarget.transform.position);

        // 공격 범위 내에 있고, 충분한 시간이 지나면 공격 시작
        if (distance <= attackRange && !isAttack && attackTimer >= attackInterval)
        {
            StartCoroutine(Attack());
            attackTimer = 0f; // 공격 후 타이머 초기화
        }
    }

    public IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        animator.SetBool("isAttack", true);

        if (currentTarget == null || currentTarget.isDead)
        {
            isAttack = false;
            isChase = true;
            animator.SetBool("isAttack", false);
            yield break;
        }

        Vector3 directionToTarget = (currentTarget.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(new Vector3(directionToTarget.x, 0, directionToTarget.z));

        GameObject instantBullet = Instantiate(
            enemyBullet,
            transform.position + Vector3.up * 3f,
            Quaternion.LookRotation(directionToTarget)
        );
        Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
        rigidBullet.velocity = directionToTarget * 20;

        yield return new WaitForSeconds(0.2f); 

        isAttack = false;
        isChase = true;
        attackCount++;
    }

    public SoonDoBu_Playable GetNearestEnemyToPosition(Vector3 position)
    {
        SoonDoBu_Playable nearestPlayable = null;
        float minDist = Mathf.Infinity;

        foreach (SoonDoBu_Playable playable in PlayableMnager.instance.playables)
        {
            if (playable == null) continue;

            float dist = Vector3.Distance(position, playable.transform.position);
            Debug.Log(playable);
            if (dist < minDist)
            {
                minDist = dist;
                nearestPlayable = playable;
            }
        }
        return nearestPlayable;
    }

    private void OnDestroy()
    {
        if (EnemyManager.instance != null)
            EnemyManager.instance.UnregisterEnemy(this);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enemy Trigger: " + other.name);
        if (other.tag == "Bullet")
        {
            Debug.Log("Bullet hit!");
            Bullet bullet = other.GetComponent<Bullet>();
            currentHealth -= bullet.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            Destroy(other.gameObject);

            Debug.Log("Range : " + currentHealth);

            StartCoroutine(OnDamage(reactVec, false));
        }
    }
    public void HitByGrenade(Vector3 explositonPos)
    {
        currentHealth -= 100;
        Vector3 reactVec = transform.position - explositonPos;
        StartCoroutine(OnDamage(reactVec, true));
    }


    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade)
    {
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.red;
        }

        yield return new WaitForSeconds(0.1f);

        if (currentHealth > 0)
        {
            foreach (MeshRenderer mesh in meshs)
            {
                mesh.material.color = Color.white;
            }
        }
        else
        {
            foreach (MeshRenderer mesh in meshs)
            {
                mesh.material.color = Color.gray;
            }
            isDead = true;
            isChase = false;
            //rigidbodyEnemy.enabled = false;
            animator.SetTrigger("doDie");

            if (isGrenade)
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up * 3;

                rigidbodyEnemy.freezeRotation = false;
                rigidbodyEnemy.AddForce(reactVec * 5, ForceMode.Impulse);
                rigidbodyEnemy.AddTorque(reactVec * 15, ForceMode.Impulse);
            }
            else
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up;
                rigidbodyEnemy.AddForce(reactVec * 5, ForceMode.Impulse);
            }

            Destroy(gameObject, 1.8f);
        }
    }
}
