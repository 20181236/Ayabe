using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;
using UnityEngine.AI;

public class SoonDoBu_Playable : MonoBehaviour
{
    Enemy currentTarget;

    public float moveSpeed;
    public float attackRange = 5.0f;//이건 프리팹 저장해서 하자

    public bool isChase;
    public bool isAttack;
    public bool isDead;

    public Transform targetTransform;
    public Rigidbody rigidbody_SoonDoBu;
    public NavMeshAgent navMeshAgent;
    public Animator animator;

    private void Start()
    {
        StartCoroutine(RetargetingRoutine());
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
            navMeshAgent.SetDestination(targetTransform.position);
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
    IEnumerator RetargetingRoutine()
    {
        while (!isDead)
        {
            currentTarget = GameManager.instance.GetNearestEnemyToPosition(transform.position);

            if (currentTarget != null)
            {
                targetTransform = currentTarget.transform;
            }

            yield return new WaitForSeconds(0.5f); // 0.5초마다 타겟 다시 찾기
        }
    }
    void Targeting()
    {
        if (currentTarget == null)
            return;

        float distance = Vector3.Distance(transform.position, currentTarget.transform.position);

        if (distance <= attackRange && !isAttack)
        {
            //StartCoroutine(Attack());
        }
    }

    //void Targeting()
    //{
    //    //if (isDead) return;

    //    // 타겟이 없거나 죽었으면 새로 찾기 상태는 아직 구현안됨
    //    if (currentTarget == null) //|| currentTarget.Dead)
    //    {
    //        currentTarget = GameManager.instance.GetNearestEnemyToPosition(transform.position);
    //    }

    //    if (currentTarget != null)
    //    {
    //        float distance = Vector3.Distance(transform.position, currentTarget.transform.position);

    //        if (distance <= attackRange && !isAttack)
    //        {
    //            //StartCoroutine(Attack());
    //        }
    //    }
    //}

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        animator.SetBool(eAnimatorType.isAttack.ToString(), true);
        yield return new WaitForSeconds(0.5f);
        //공격에 대한 로직필요

        isChase = true;
        isAttack = false;
        animator.SetBool(eAnimatorType.isAttack.ToString(), false);

        yield return new WaitForSeconds(2f);
    }

}
