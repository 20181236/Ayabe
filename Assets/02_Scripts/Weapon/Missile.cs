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

        // 목표 위치로 방향 계산
        Vector3 direction = (target.position - transform.position).normalized;

        // 회전 (Quaternion.RotateTowards 사용)
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

        // 이동 (transform.Translate 사용)
        transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);
    }

    void OnCollisionEnter(Collision collision)
    {
        // 플레이어와 충돌했을 때
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject); // 미사일 파괴
        }
    }
}
