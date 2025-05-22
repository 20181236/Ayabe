using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public enum PoolType
    {
        EnemyBullet,
        EnemyMissile
    }

    public static PoolManager instance;

    private Dictionary<PoolType, Bullet> bulletPrefabDictionary = new Dictionary<PoolType, Bullet>();
    private Dictionary<PoolType, Queue<Bullet>> bulletPool = new Dictionary<PoolType, Queue<Bullet>>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void RegisterBulletPrefab(PoolType type, Bullet prefab)
    {
        if (bulletPrefabDictionary.ContainsKey(type))//중복 등록 방지를 위해 ContainsKey로 체크
        {
            Debug.LogWarning($"PoolManager: {type} prefab is already registered.");
            return;
        }
        bulletPrefabDictionary[type] = prefab;
        bulletPool[type] = new Queue<Bullet>();
    }

    public void CreatePooling(PoolType type, int count)
    {
        if (!bulletPrefabDictionary.ContainsKey(type))
        {
            Debug.LogError($"PoolManager: No prefab registered for {type}.");
            return;
        }

        if (!bulletPool.ContainsKey(type))
            bulletPool[type] = new Queue<Bullet>();

        for (int i = 0; i < count; i++)
        {
            Bullet newBullet = Instantiate(bulletPrefabDictionary[type]);
            newBullet.gameObject.SetActive(false);
            bulletPool[type].Enqueue(newBullet);
        }
    }

    public Bullet GetBullet(PoolType type)
    {
        if (!bulletPool.ContainsKey(type))
        {
            Debug.LogError($"PoolManager: No pool exists for {type}.");
            return null;
        }

        Queue<Bullet> poolQueue = bulletPool[type];

        Bullet bullet;

        if (poolQueue.Count > 0)
        {
            bullet = poolQueue.Dequeue();
        }
        else
        {
            if (!bulletPrefabDictionary.ContainsKey(type))
            {
                Debug.LogError($"PoolManager: No prefab registered for {type}.");
                return null;
            }

            bullet = Instantiate(bulletPrefabDictionary[type]);
        }

        bullet.gameObject.SetActive(true);
        return bullet;
    }

    public void ReturnBullet(Bullet bullet, PoolType type)
    {
        bullet.gameObject.SetActive(false);

        if (!bulletPool.ContainsKey(type))
        {
            Debug.LogWarning($"PoolManager: Pool for {type} doesn't exist. Creating new pool.");
            bulletPool[type] = new Queue<Bullet>();
        }

        bulletPool[type].Enqueue(bullet);
    }
}
