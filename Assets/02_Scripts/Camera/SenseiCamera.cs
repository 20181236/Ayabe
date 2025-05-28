using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SenseiCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 15, -10);
    public float followSpeed = 2f;  // �ʹ� ������ ��鸲 ����

    void LateUpdate()
    {
        if (target == null)
            return;
        Vector3 targetPos = target.position + offset;
        targetPos.y = offset.y;  // ���� y���� ����
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * followSpeed);

        // ������ ���� ����
        transform.rotation = Quaternion.Euler(60f, -90f, 0f);
    }
}
    