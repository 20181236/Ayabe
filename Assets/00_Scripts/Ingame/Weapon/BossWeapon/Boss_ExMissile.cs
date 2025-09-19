using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossExMissile : MonoBehaviour
{
    public float maxY = 100f;
    public float speed = 15f;
    public GameObject bossExSubMissilePrefab;
    public int subMissileCount = 5;
    public float scatterRadius = 30f;

    private bool hasSplit = false;

    private void FixedUpdate()
    {
        // 위로 이동
        transform.position += Vector3.up * speed * Time.fixedDeltaTime;

        // 일정 Y좌표 이상이면 서브 미사일 생성 후 삭제
        if (!hasSplit && transform.position.y >= maxY)
        {
            SplitIntoSubMissiles();
            hasSplit = true;
            Destroy(gameObject);
        }
    }

    private void SplitIntoSubMissiles()
    {
        var allPlayables = PlayableManager.instance.GetPlayables();
        if (allPlayables == null || allPlayables.Count == 0)
            return;

        // subMissileCount 만큼 자탄을 생성합니다.
        for (int i = 0; i < subMissileCount; i++)
        {
            // % (나머지) 연산을 이용해 모든 플레이어를 순서대로 타겟팅합니다.
            // 예: 0, 1, 2, 0, 1, 2...
            PlayableBase currentTarget = allPlayables[i % allPlayables.Count];

            // 확산 범위 없이 정확히 해당 타겟을 노립니다.
            Vector3 targetPosition = currentTarget.transform.position;

            // 원한다면 여기에 작은 랜덤 오프셋을 추가할 수도 있습니다.
            Vector2 randomCircle = Random.insideUnitCircle * scatterRadius; 
            Vector3 offset = new Vector3(randomCircle.x, 0f, randomCircle.y);
            targetPosition += offset;

            Vector3 spawnPos = transform.position;
            Debug.Log($"타겟: {currentTarget.name}, 목표 좌표: {targetPosition}, 생성 좌표: {spawnPos}");

            GameObject miniMissile = Instantiate(bossExSubMissilePrefab, spawnPos, Quaternion.identity);

            BossExSubMissile subScript = miniMissile.GetComponent<BossExSubMissile>();
            if (subScript != null)
            {
                //subScript.SetTarget(currentTarget.transform);
            }
        }
    }
}
        //private void SplitIntoSubMissiles()
        //{
        //    var allPlayables = PlayableManager.instance.GetPlayables();
        //    if (allPlayables == null || allPlayables.Count == 0)
        //        return;

        //    PlayableBase selectTarget = allPlayables[Random.Range(0, allPlayables.Count)];
        //    Vector3 basePos = selectTarget.transform.position;

        //    for (int i = 0; i < subMissileCount; i++)
        //    {
        //        // 확산 범위 랜덤
        //        Vector2 randomCircle = Random.insideUnitCircle * scatterRadius;
        //        Vector3 offset = new Vector3(randomCircle.x, 0f, randomCircle.y);
        //        Vector3 spawnPos = transform.position;

        //        GameObject miniMissile = Instantiate(bossExSubMissilePrefab, spawnPos, Quaternion.identity);

        //        BossExSubMissile subScript = miniMissile.GetComponent<BossExSubMissile>();
        //        if (subScript != null)
        //        {
        //            // 타겟 위치를 바탕으로 초기 회전 및 직선 이동 설정
        //            subScript.SetDirection(basePos + offset);
        //        }
        //    }
        //}
    //}


//public class BossExMissile : ProjectileBase
//{
//    public Animator exMissileAnimator;
//    public float delayBeforeFall = 1.5f;

//    public GameObject bossExSubMissilePrefab;
//    public Rigidbody rigidbodyExSubMissile;
//    public int subMissileCount = 5;
//    public float scatterRadius = 10f;
//    public float fallSpeed = 25f;

//    public bool hasSplit = false;

//    protected override void Awake()
//    {
//        SetTargetMask();
//        SetProjectileInfo();
//        rigidbodyExSubMissile = GetComponent<Rigidbody>();
//        exMissileAnimator = GetComponent<Animator>();
//        if (exMissileAnimator != null)
//            exMissileAnimator.SetTrigger("Rise");
//        else
//            Debug.LogWarning("Animator가 없습니다.");
//    }
//    protected override void SetProjectileInfo()
//    {
//        base.SetProjectileInfo();
//        WeaponType weaponType = WeaponType.Mssile;
//        damage = 50f;
//        speed = 50f;
//        rotateSpeed = 720f;
//        isExplosion = false;
//    }
//    // 애니메이션 이벤트에서 호출됨
//    public void OnRiseAnimationEnd()
//    {
//        if (hasSplit) return;
//        hasSplit = true;

//        SelectRandomTargetAndSpawnMissiles();
//        Destroy(gameObject);
//    }

//    void SelectRandomTargetAndSpawnMissiles()
//    {
//        var allPlayables = PlayableManager.instance.GetPlayables();

//        if (allPlayables == null || allPlayables.Count == 0)
//            return;

//        PlayableBase selectTarget = allPlayables[Random.Range(0, allPlayables.Count)];
//        Vector3 basePos = selectTarget.transform.position;

//        for (int i = 0; i < subMissileCount; i++)
//        {
//            Vector2 randomCircle = Random.insideUnitCircle * scatterRadius;
//            Vector3 offset = new Vector3(randomCircle.x, 30f, randomCircle.y);
//            Vector3 spawnPos = basePos + offset;

//            GameObject miniMissile = Instantiate(bossExSubMissilePrefab, spawnPos, Quaternion.identity);
//        }
//    }
//}
