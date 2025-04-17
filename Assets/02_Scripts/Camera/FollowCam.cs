using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform target;       // ���� ��� (�� ������Ʈ)
    public Vector3 offset = new Vector3(0, 5, -10); // ī�޶� ��ġ ������
    public float smoothSpeed = 0.125f; // �ε巴�� ���󰡱� �ӵ�

    void Update()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        transform.position = smoothedPosition;
        transform.LookAt(target); // ��� �ٶ󺸱� (�ʿ信 ���� ���� ����)
    }
}
