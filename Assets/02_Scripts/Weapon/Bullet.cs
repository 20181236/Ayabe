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
        timer = 0f; // �Ѿ��� Ȱ��ȭ�Ǹ� Ÿ�̸� �ʱ�ȭ
    }

    private void Update()
    {

    }

}


