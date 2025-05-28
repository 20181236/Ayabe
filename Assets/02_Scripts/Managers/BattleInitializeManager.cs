using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleInitializer : MonoBehaviour
{
    public GameObject PlayableBulletPrefab;
    public GameObject EnemyBulletPrefab;

    public PlayableSpawnData[] spawnDatas;
    private int currentIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        BulletPoolManager.instance.RegisterBulletPrefab(BulletPoolManager.PoolType.EnemyBullet, EnemyBulletPrefab.GetComponent<Bullet>());
        BulletPoolManager.instance.CreatePooling(BulletPoolManager.PoolType.EnemyBullet, 30);
        BulletPoolManager.instance.RegisterBulletPrefab(BulletPoolManager.PoolType.PlayableBullet, PlayableBulletPrefab.GetComponent<Bullet>());
        BulletPoolManager.instance.CreatePooling(BulletPoolManager.PoolType.PlayableBullet, 30);
        for (int i = 0; i < spawnDatas.Length; i++)
        {
            StartPlaySpawn();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartPlaySpawn()
    {
        if (currentIndex < spawnDatas.Length)
        {
            SpawnManager.instance.PlayableSpawn(spawnDatas[currentIndex]);
            currentIndex++;
        }
    }
}
