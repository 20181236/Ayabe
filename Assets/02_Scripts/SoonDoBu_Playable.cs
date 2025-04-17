using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;
using UnityEngine.AI;

public class SoonDoBu_Playable : MonoBehaviour
{
    public int curentHealth;
    public int maxHealth;

    public float moveSpeed;

    public bool isChase;
    public bool isAttack;
    public bool isDead;

    public Transform targetTransform;
    public Rigidbody rigidbody;
    public NavMeshAgent navMeshAgent;
    public Animator animator;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        curentHealth = maxHealth;

        Invoke("ChaseStart", 2f);
    }
    void ChaseStart()
    {
        isChase = true;
        animator.SetBool(eAnimatorType.isWalk.ToString(), true);
    }

    void Update()
    {
        if (navMeshAgent.enabled)
        {
            navMeshAgent.SetDestination(targetTransform.position);
            navMeshAgent.isStopped = !isChase;
        }
    }

    void FreezeVelocity()
    {
        if (isChase)
        {
            rigidbody.velocity = Vector3.zero;
            //angularVelocity = PhysicsRotationVelocity
            rigidbody.angularVelocity = Vector3.zero;
        }
    }
    void Targeting()
    {

        //if (!isDead)
        //{
        //    float targetRadius = 0.5f;
        //    float targetRange = 25f;
           
        //    RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Enemy"));

        //    if (rayHits.Length > 0 && !isAttack)
        //    {
        //        StartCoroutine(Attack());
        //    }
        //}
    }
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
    void FixedUpdate()
    {
        Targeting();
        FreezeVelocity();
    }
}

