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
        // 매 프레임 앞으로 이동
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    // 필요하면 충돌 처리 함수 추가 가능
    private void OnTriggerEnter(Collider other)
    {
    }

    // 폭발 효과 함수 예시
    private void Explode()
    {

        Destroy(gameObject);
    }
}
