using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Missile : ProjectileBase
{
    public Transform target;
    private Rigidbody rigidbodyMissile;
    protected override void SetProjectileInfo()
    {
        base.SetProjectileInfo();
        damage = 10;
        speed = 30f;
        rotateSpeed = 20f;
        isExplosion = false;
    }

    void Awake()
    {
        rigidbodyMissile = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (target == null)
            return;

        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // ȸ�� ����
        rigidbodyMissile.MoveRotation(Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.fixedDeltaTime));
        // �ӵ� ����
        rigidbodyMissile.velocity = transform.forward * speed;
    }
}

