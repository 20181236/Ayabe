using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;
using static BulletPoolManager;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 1f;
    private float timer;
    private bool isActive = false;

    private void OnEnable()
    {
        timer = 0f;
        isActive = true;
    }

    private void Update()
    {
        if (!isActive)
            return;

        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            ReturnToPool();
        }
    }

    public void ReturnToPool()
    {
        isActive = false;
        BulletPoolManager.instance.ReturnBullet(this);
    }
}


