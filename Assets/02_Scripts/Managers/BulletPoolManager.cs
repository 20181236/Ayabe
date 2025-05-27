using System.Collections.Generic;
using UnityEngine;

public class BulletPoolManager : MonoBehaviour
{
    public static BulletPoolManager instance { get; private set; }


    [SerializeField] private Bullet playableBulletPrefab;
    [SerializeField] private Bullet enemyBulletPrefab;

    private Dictionary<PoolType, Bullet> bulletPrefabDictionary = new Dictionary<PoolType, Bullet>();
    private Dictionary<PoolType, List<Bullet>> bulletPoolDictionary = new Dictionary<PoolType, List<Bullet>>();
    private List<Bullet> bulletPool = new List<Bullet>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void RegisterBulletPrefab()
    {

    }

    public void CreatePooling()
    {

    }

    public Bullet GetBullet()
    {

    }

    public void ReturnBullet()
    {

    }
}


