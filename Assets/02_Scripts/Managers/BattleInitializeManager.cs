using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleInitializer : MonoBehaviour
{

    public GameObject PlayableBulletPrefab;
    public GameObject EnemyBulletPrefab;

    // Start is called before the first frame update
    void Start()
    {
        PoolManager.instance.RegisterBulletPrefab(PoolManager.PoolType.EnemyBullet, EnemyBulletPrefab.GetComponent<Bullet>());
        PoolManager.instance.CreatePooling(PoolManager.PoolType.EnemyBullet, 50);
        PoolManager.instance.RegisterBulletPrefab(PoolManager.PoolType.PlayableBullet, PlayableBulletPrefab.GetComponent<Bullet>());
        PoolManager.instance.CreatePooling(PoolManager.PoolType.PlayableBullet, 50);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
