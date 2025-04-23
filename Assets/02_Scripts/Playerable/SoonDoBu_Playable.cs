using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;
using UnityEngine.AI;

public class SoonDoBu_Playable : MonoBehaviour
{
    Enemy currentTarget;

    public float moveSpeed;
    public float attackRange = 5.0f;//�̰� ������ �����ؼ� ����

    public bool isChase;
    public bool isAttack;
    public bool isDead;

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
                // ������ �������� ��� �̵� (Fallback)
                Vector3 rightDestination = transform.position + Vector3.right * 5f;//��� �������� �̰� ������ ������...
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

        // Ÿ���� ������ ���� ����� ���� ã�´�
        if (currentTarget == null)
        {
            currentTarget = GetNearestEnemyToPosition(transform.position);
        }
        // Ÿ���� ������ �Ÿ� üũ
        if (currentTarget != null)
        {
            float distance = Vector3.Distance(transform.position, currentTarget.transform.position);

            if (distance <= attackRange && !isAttack)
            {
                StartCoroutine(Attack());
            }
        }
    }

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        animator.SetBool(eAnimatorType.isAttack.ToString(), true);
        yield return new WaitForSeconds(0.5f);
        //���ݿ� ���� �����ʿ�

        isChase = true;
        isAttack = false;
        animator.SetBool(eAnimatorType.isAttack.ToString(), false);

        yield return new WaitForSeconds(2f);
    }

    public Enemy GetNearestEnemyToPosition(Vector3 position)
    {
        Enemy nearest = null;
        float minDist = Mathf.Infinity;

        foreach (Enemy enemy in EnemyManager.instance.enemies)
        {
            if (enemy == null) continue;

            float dist = Vector3.Distance(position, enemy.transform.position);
            Debug.Log(enemy);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = enemy;
            }
        }
        return nearest;
    }
}
