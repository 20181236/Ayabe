using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SenseiCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 15, -10);
    public float followSpeed = 2f;  // 너무 빠르면 흔들림 유발

    void LateUpdate()
    {
        if (target == null) return;

        // 목표 위치 계산
        Vector3 targetPos = target.position + offset;

        // z축이나 y축을 원하지 않게 바꾸는 걸 방지 (필요 시)
        targetPos.y = offset.y;  // 고정 y높이 유지
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * followSpeed);

        // 고정된 각도 유지
        transform.rotation = Quaternion.Euler(60f, -90f, 0f);
    }
}
    