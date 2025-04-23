using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;
using UnityEngine.AI;

public class SoonDoBu_Playable : MonoBehaviour
{
    Enemy currentTarget;

    public float moveSpeed;
    public float attackRange = 15.0f;//이건 프리팹 저장해서 하자

    public bool isChase;
    public bool isAttack;
    public bool isDead;

    public GameObject bullet;

    public Transform targetTransform;
    public Rigidbody rigidbody_SoonDoBu;
    public NavMeshAgent navMeshAgent;
    public Animator animator;

    private void Start()
    {
        
    }

    private void Awake()
    {
        rigidbody_SoonDoBu = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        Invoke("ChaseStart", 2f);
    }
    void ChaseStart()
    {
        isChase = true;
        animator.SetBool(eAnimatorType.isWalk.ToString(), true);
    }

    void Update()
    {
        Targeting();

        if (navMeshAgent.enabled)
        {
            if (targetTransform != null)
            {
                navMeshAgent.SetDestination(targetTransform.position);
            }
            else
            {
                // 이거 수정해야됨...
                Vector3 rightDestination = transform.position + Vector3.right * 15f;//상수 좋지않음 이거 고정된 길이임...
                navMeshAgent.SetDestination(rightDestination);
            }
            navMeshAgent.isStopped = !isChase;
        }
    }

    void FixedUpdate()
    {
        Moving();
        FreezeVelocity();
    }
    void FreezeVelocity()
    {
        if (isChase)
        {
            rigidbody_SoonDoBu.velocity = Vector3.zero;
            //angularVelocity = PhysicsRotationVelocity
            rigidbody_SoonDoBu.angularVelocity = Vector3.zero;
        }
    }

    public void Moving()
    {
    }
    void MoveToMap_End_Object()
    {
    }

    void Targeting()
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

        if (distance <= attackRange && !isAttack)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        animator.SetBool("isAttack", true);

        yield return new WaitForSeconds(0.5f);

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
            bullet,
            transform.position + Vector3.up * 1f,
            Quaternion.LookRotation(directionToTarget)
            );
        Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
        rigidBullet.velocity = directionToTarget * 20;

        yield return new WaitForSeconds(2f);

        isAttack = false;
        isChase = true;
        animator.SetBool("isAttack", false);
    }

    public Enemy GetNearestEnemyToPosition(Vector3 position)
    {
        Enemy nearestEnemy = null;
        float minDist = Mathf.Infinity;

        foreach (Enemy enemy in EnemyManager.instance.enemies)
        {
            if (enemy == null) continue;

            float dist = Vector3.Distance(position, enemy.transform.position);
            Debug.Log(enemy);
            if (dist < minDist)
            {
                minDist = dist;
                nearestEnemy = enemy;
            }
        }
        return nearestEnemy;
    }
}
