using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BossExSubMissile : ProjectileBase
{
    private Rigidbody exSubMissileRigidbody;

    protected override void Awake()
    {
        base.Awake();
        exSubMissileRigidbody = GetComponent<Rigidbody>();
        if (exSubMissileRigidbody != null)
        {
            exSubMissileRigidbody.useGravity = true;
            exSubMissileRigidbody.velocity = Vector3.down * 10f; // 낙하 속도
        }
    }

    protected override void SetProjectileInfo()
    {
        base.SetProjectileInfo();
        damage = 25f;
        speed = 0f;   // 낙하만 하므로 자체 속도 없음
        isExplosion = false; 
    }
}