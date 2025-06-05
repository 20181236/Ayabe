using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Missile : ProjectileBase
{
    private Rigidbody rigidbodyMissile;
    protected override void SetProjectileInfo()
    {
        base.SetProjectileInfo();
        damage = 10;
        speed = 30f;
        rotateSpeed = 20f;
        isExplosion = false;
    }

    protected override void Awake()
    {
        SetTargetMask();
        rigidbodyMissile = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (target == null)
            return;

        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // 회전 보간
        rigidbodyMissile.MoveRotation(Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.fixedDeltaTime));
        // 속도 설정
        rigidbodyMissile.velocity = transform.forward * speed;
    }
}

