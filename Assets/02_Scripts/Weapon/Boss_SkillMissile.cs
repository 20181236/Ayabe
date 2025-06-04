using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossSkillMissile : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 5f;

    private void Start()
    {

    }

    private void Update()
    {
        // �� ������ ������ �̵�
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    // �ʿ��ϸ� �浹 ó�� �Լ� �߰� ����
    private void OnTriggerEnter(Collider other)
    {
    }

    // ���� ȿ�� �Լ� ����
    private void Explode()
    {

        Destroy(gameObject);
    }
}
