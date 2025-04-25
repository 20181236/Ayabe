using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


//이건 내가할단계가 아닌거같다
public class PlayableBase : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth = 0f;
    public float moveSpeed;
    public float attackRange = 15.0f;//이건 프리팹 저장해서 하자

    public bool isChase;
    public bool isAttack;
    public bool isDead;

    public GameObject bullet;

    public Transform excapeSpotTransform;
    public Rigidbody rigidbodySelf;
    public BoxCollider boxCollider;
    public MeshRenderer[] meshs;
    public NavMeshAgent navMeshAgent;
    public Animator animator;

    protected virtual void Awake()
    {
        rigidbodySelf = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        boxCollider = GetComponent<BoxCollider>();
        animator = GetComponentInChildren<Animator>();
        meshs = GetComponentsInChildren<MeshRenderer>();

        currentHealth = maxHealth;
    }

    protected virtual void Start()
    {
        Invoke("ChaseStart", 2f);
    }

    protected virtual void Update()
    {
        Targeting();

        if (navMeshAgent.enabled)
        {
            if (excapeSpotTransform != null)
                navMeshAgent.SetDestination(excapeSpotTransform.position);
            else
                navMeshAgent.SetDestination(transform.position + Vector3.right * 15f);

            navMeshAgent.isStopped = !isChase;
        }
    }

    protected virtual void FixedUpdate()
    {
        Moving();
        FreezeVelocity();
    }

    protected void FreezeVelocity()
    {
        if (isChase)
        {
            rigidbodySelf.velocity = Vector3.zero;
            rigidbodySelf.angularVelocity = Vector3.zero;
        }
    }

    public virtual void Moving() { }

    protected void ChaseStart()
    {
        isChase = true;
        animator.SetBool(eAnimatorType.isWalk.ToString(), true);
    }

    protected virtual void Targeting() { }

    protected virtual IEnumerator Attack()
    {
        yield break;
    }

    public virtual IEnumerator OnDamage(Vector3 reactVec, bool isGrenade)
    {
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.red;
        }

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
        }
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.tag == "EnemyBullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            currentHealth -= bullet.damage;

            Vector3 reactVec = transform.position - other.transform.position;
            Destroy(other.gameObject);

            StartCoroutine(OnDamage(reactVec, false));
        }
    }
}