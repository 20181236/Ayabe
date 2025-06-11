using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance { get; private set; }
    public List<EnemyBase> enemies = new List<EnemyBase>();
    public Dictionary<EnemyID, List<EnemyBase>> enemiesID = new Dictionary<EnemyID, List<EnemyBase>>();
    public Dictionary<EnemyType, List<EnemyBase>> enemiesType = new Dictionary<EnemyType, List<EnemyBase>>();
    public EnemyData[] enemyDatas; // 에디터에 드래그해서 연결할 수 있게
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        foreach (EnemyID id in (EnemyID[])System.Enum.GetValues(typeof(EnemyID)))
        {
            enemiesID[id] = new List<EnemyBase>();
        }
    }
    public void SpawnEnemy(EnemyID id, Vector3 spawnPosition)
    {
        EnemyData data = System.Array.Find(enemyDatas, d => d.enemyID == id);
        if (data != null)
        {
            EnemyBase enemy = EnemyFactory.CreateEnemy(data, spawnPosition);
            if (enemy != null)
            {
                RegisterEnemy(enemy);
            }
        }
        else
        {   
            Debug.LogError("EnemyData not found for type: " + id);
        }
    }

    public void RegisterEnemy(EnemyBase enemy)
    {
        if (!enemies.Contains(enemy))
        {
            enemies.Add(enemy);
            enemiesID[enemy.enemyID].Add(enemy);
        }
    }

    public void UnregisterEnemy(EnemyBase enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
            enemiesID[enemy.enemyID].Remove(enemy);
        }
    }
    public List<EnemyBase> GetEnemies()
    {
        return enemies;
    }
    public List<EnemyBase> GetEnemiesType(EnemyType type)
    {
        return enemiesType[type];
    }
    public bool HasEnemy()
    {
        return enemies.Count > 0;
    }
    public bool HasEnemyOfType(EnemyType type)
    {
        return enemiesType[type].Count > 0;
    }
    public bool HasBoss()
    {
        return HasEnemyOfType(EnemyType.Boss);
    }
}