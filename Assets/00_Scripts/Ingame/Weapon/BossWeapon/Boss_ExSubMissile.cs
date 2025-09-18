using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BossExSubMissile : ProjectileBase
{
    private Rigidbody _rigidbody;
    //public float speed = 25f;

    private Vector3 _targetPosition;

    protected override void Awake()
    {
        base.Awake();
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = false; // 직선 이동이므로 중력 비활성
    }

    // 생성 시 타겟 방향으로 회전
    public void SetDirection(Vector3 targetPosition)
    {
        _targetPosition = targetPosition;
        Vector3 direction = (_targetPosition - transform.position).normalized;
        _rigidbody.velocity = direction * speed;
    }
    private void FixedUpdate()
    {
        if (_rigidbody.velocity.sqrMagnitude > 0.001f)
        {
            // 이동 방향으로 회전
            _rigidbody.rotation = Quaternion.LookRotation(_rigidbody.velocity);
        }
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

//public class BossExSubMissile : ProjectileBase
//{
//    private Rigidbody exSubMissileRigidbody;

//    protected override void Awake()
//    {
//        base.Awake();
//        exSubMissileRigidbody = GetComponent<Rigidbody>();
//        if (exSubMissileRigidbody != null)
//        {
//            exSubMissileRigidbody.useGravity = true;
//            exSubMissileRigidbody.velocity = Vector3.down * 10f; // 낙하 속도
//        }
//    }

//    protected override void SetProjectileInfo()
//    {
//        base.SetProjectileInfo();
//        damage = 25f;
//        speed = 0f;   // 낙하만 하므로 자체 속도 없음
//        isExplosion = false; 
//    }
//    private void OnCollisionEnter(Collision collision)
//    {
//        // 바닥 태그를 가진 오브젝트와 충돌 시
//        if (collision.gameObject.CompareTag("Ground"))
//        {
//            Destroy(gameObject); // 또는 풀링 시스템을 쓰고 있다면 ReturnToPool 호출
//            Debug.Log("[BossExSubMissile] 바닥에 닿아 미사일 제거됨.");
//        }
//    }
//}