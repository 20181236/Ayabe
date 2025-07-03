using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossSkillMissile : ProjectileBase
{
    private Rigidbody rigidbodyMissile;
    protected override void SetProjectileInfo()
    {
        base.SetProjectileInfo();
        WeaponType weaponType = WeaponType.Mssile;
        damage = 25f;
        speed = 50f;
        rotateSpeed = 720f;
        isExplosion = false;
    }

    protected override void Awake()
    {
        SetTargetMask();
        rigidbodyMissile = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (target == null)
            return;
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.fixedDeltaTime);
            rigidbodyMissile.MoveRotation(newRotation);
            rigidbodyMissile.velocity = direction * speed;
        }
        else
        {
            rigidbodyMissile.velocity = transform.forward * speed;
        }
    }
}
