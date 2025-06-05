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
            exSubMissileRigidbody.velocity = Vector3.down * 10f; // ���� �ӵ�
        }
    }

    protected override void SetProjectileInfo()
    {
        base.SetProjectileInfo();
        damage = 25f;
        speed = 0f;   // ���ϸ� �ϹǷ� ��ü �ӵ� ����
        isExplosion = false; 
    }
}