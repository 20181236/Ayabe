using System.Collections.Generic;
using UnityEngine;

public class BulletPoolManager : MonoBehaviour
{
    public static BulletPoolManager instance { get; private set; }

    public enum PoolType
    {
        PlayableBullet,
        EnemyBullet
    }

    private Dictionary<PoolType, Bullet> bulletPrefabDictionary = new Dictionary<PoolType, Bullet>();//여기서 프리팹 정보
    private Dictionary<PoolType, List<Bullet>> bulletPoolDictionary = new Dictionary<PoolType, List<Bullet>>();//여기서 리스트 정보

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void RegisterBulletPrefab(PoolType type, Bullet prefab)
    {
        if (!bulletPrefabDictionary.ContainsKey(type))
        {
            bulletPrefabDictionary.Add(type, prefab);
            bulletPoolDictionary.Add(type, new List<Bullet>());
        }
    }

    public void CreatePooling(PoolType type, int count)
    {
        if (!bulletPrefabDictionary.ContainsKey(type))
            return;
        for (int i = 0; i < count; i++)
        {
            Bullet bullet = Instantiate(bulletPrefabDictionary[type]);
            bullet.gameObject.SetActive(false);
            bulletPoolDictionary[type].Add(bullet);
        }
    }

    public Bullet GetBullet(PoolType type)
    {
        if (!bulletPoolDictionary.ContainsKey(type))
        return null;
        foreach (Bullet bullet in bulletPoolDictionary[type])
        {
            if (!bullet.gameObject.activeInHierarchy)
            {
                bullet.gameObject.SetActive(true);
                return bullet;
            }
        }
        return null;
    }

    public void ReturnBullet(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
    }
}


