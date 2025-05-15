using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Missile : MonoBehaviour
{
    public Transform target;
    public int damage;
    public float speed = 50f;
    public float rotateSpeed = 10f;

    private Rigidbody rigidbodyMissile;

    void Awake()
    {
        rigidbodyMissile = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (target == null)
            return;

        // ��ǥ ��ġ�� ���� ���
        Vector3 direction = (target.position - transform.position).normalized;

        // ȸ�� (Quaternion.RotateTowards ���)
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

        // �̵� (transform.Translate ���)
        transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);
    }

    void OnCollisionEnter(Collision collision)
    {
        // �÷��̾�� �浹���� ��
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject); // �̻��� �ı�
        }
    }
}
