using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossExSubMissile : ProjectileBase
{


    private void OnTriggerEnter(Collider other)
    {
        // 이 부분은 동일하게 작동합니다. (단, 부딪히는 대상이 Rigidbody를 가져야 함)
        CharacterBase hitCharacter = other.GetComponent<CharacterBase>();
        if (hitCharacter != null)
        {
            hitCharacter.ApplyDamage(damage, isExplosion);
            Destroy(gameObject);
            return;
        }

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