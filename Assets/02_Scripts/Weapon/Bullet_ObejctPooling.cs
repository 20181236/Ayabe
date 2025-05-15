using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_ObjectPooling : MonoBehaviour
{
    public static Bullet_ObjectPooling instance;

    [SerializeField]
    private GameObject bulletPrefab;

    Queue<Bullet> bulletQueue = new Queue<Bullet>();

    private void Awake()
    {
        instance = this;
        Initialize(50);
    }

    private void Initialize(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var bullet = CreateNewBullet();
            bulletQueue.Enqueue(bullet);
        }
    }

    private Bullet CreateNewBullet()
    {
        var bulletObject = Instantiate(bulletPrefab).GetComponent<Bullet>();
        bulletObject.gameObject.SetActive(false);
        bulletObject.transform.SetParent(transform);
        return bulletObject;
    }

    public Bullet GetBullet()
    {
        if (bulletQueue.Count > 0)
        {
            var bullet = bulletQueue.Dequeue();
            bullet.gameObject.SetActive(true);
            bullet.transform.SetParent(null);
            return bullet;
        }
        else
        {
            var bullet = CreateNewBullet();
            bullet.gameObject.SetActive(true);
            bullet.transform.SetParent(null);
            return bullet;
        }
    }

    public void ReturnBullet(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
        bullet.transform.SetParent(transform);
        bulletQueue.Enqueue(bullet);
    }
}

