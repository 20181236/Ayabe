using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossSkillMissile : ProjectileBase
{
    private Rigidbody rigidbodyMissile;
    private bool initialized = false;
    private Vector3 moveDirection;

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
        rigidbodyMissile.useGravity = false; // 중력은 끄기
    }

    public void Initialize(Transform targetTransform)
    {
        if (targetTransform == null) return;
        moveDirection = (targetTransform.position - transform.position).normalized;
        rigidbodyMissile.velocity = moveDirection * speed;
        transform.rotation = Quaternion.LookRotation(moveDirection);
        initialized = true;
    }

    private void FixedUpdate()
    {
        if (!initialized) return;

        // 이동 방향에 맞게 회전 유지
        if (rigidbodyMissile.velocity.sqrMagnitude > 0.01f)
            rigidbodyMissile.rotation = Quaternion.LookRotation(rigidbodyMissile.velocity);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 캐릭터에 맞으면 데미지
        CharacterBase hitCharacter = other.GetComponent<CharacterBase>();
        if (hitCharacter != null)
        {
            hitCharacter.ApplyDamage(damage, isExplosion);
            Debug.Log($"[BossExSubMissile] {hitCharacter.name}에게 데미지를 주었습니다! Damage={damage}");
            Destroy(gameObject);
            return;
        }

        // 바닥과 충돌 시 제거
        if (other.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
