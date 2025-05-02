using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossExMissile : MonoBehaviour
{
    public Animator animator;
    public GameObject subMissilePrefab;
    public Vector3 targetPosition;
    private bool hasSplit = false;  // Split이 이미 실행되었는지 체크하는 변수

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Init(Vector3 target)
    {
        targetPosition = target;
    }

    public void Split()
    {
        if (hasSplit) return;  // 이미 Split이 실행되었으면 종료

        hasSplit = true;  // Split을 한 번 실행했으므로 플래그 설정

        Debug.Log("Split");

        // 서브 미사일 생성
        for (int i = 0; i < 5; i++)
        {
            Vector2 offset = Random.insideUnitCircle * 2f;
            Vector3 subTarget = new Vector3(targetPosition.x + offset.x, 10f, targetPosition.z + offset.y);
            GameObject sub = Instantiate(subMissilePrefab, transform.position, Quaternion.identity);
            sub.GetComponent<BossExSubMissile>().Init(subTarget);
        }

        // 미사일 객체 제거
        Destroy(gameObject);
    }
}
