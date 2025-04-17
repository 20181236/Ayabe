using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform target;       // 따라갈 대상 (빈 오브젝트)
    public Vector3 offset = new Vector3(0, 5, -10); // 카메라 위치 오프셋
    public float smoothSpeed = 0.125f; // 부드럽게 따라가기 속도

    void Update()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        transform.position = smoothedPosition;
        transform.LookAt(target); // 대상 바라보기 (필요에 따라 제거 가능)
    }
}
