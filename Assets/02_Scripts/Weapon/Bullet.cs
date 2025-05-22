using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 5f;
    private float timer;
    public bool isActive;
    public PoolManager.PoolType poolType;  // 추가: 자기 자신 타입 저장

    private void OnEnable()
    {
        timer = 0f; // 총알이 활성화되면 타이머 초기화
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            PoolManager.instance.ReturnBullet(this, poolType);
        }
    }

    private void OnDisable()
    {
        // 필요시 이곳에 효과 리셋, 상태 초기화 가능
    }
}


