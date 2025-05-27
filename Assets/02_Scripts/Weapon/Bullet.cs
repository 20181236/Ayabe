using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 1f;
    private float timer;

    private void OnEnable()
    {
        timer = 0f; // 총알이 활성화되면 타이머 초기화
    }

    private void Update()
    {

    }

}


