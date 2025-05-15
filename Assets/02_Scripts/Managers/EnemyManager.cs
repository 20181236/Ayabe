using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance { get; private set; }
    public List<Enemy_base> enemies = new List<Enemy_base>();
    public Dictionary<EnemyType, List<Enemy_base>> enemiesType = new Dictionary<EnemyType, List<Enemy_base>>();
    public EnemyData[] enemyDatas; // 에디터에 드래그해서 연결할 수 있게


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        //DontDestroyOnLoad(gameObject);
        foreach (EnemyType type in (EnemyType[])System.Enum.GetValues(typeof(EnemyType)))
        {
            enemiesType[type] = new List<Enemy_base>();
        }
    }
    public void SpawnEnemy(EnemyType type, Vector3 spawnPosition)
    {
        EnemyData data = System.Array.Find(enemyDatas, d => d.enemyType == type);
        if (data != null)
        {
            Enemy_base enemy = EnemyFactory.CreateEnemy(data, spawnPosition);
            if (enemy != null)
            {
                RegisterEnemy(enemy);
            }
        }
        else
        {   
            Debug.LogError("EnemyData not found for type: " + type);
        }
    }

    public void RegisterEnemy(Enemy_base enemy)
    {
        if (!enemies.Contains(enemy))
        {
            enemies.Add(enemy);
            enemiesType[enemy.enemyType].Add(enemy);
        }
    }

    public void UnregisterEnemy(Enemy_base enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
            enemiesType[enemy.enemyType].Remove(enemy);
        }
    }
    public List<Enemy_base> GetEnemies()
    {
        return enemies;
    }
    public List<Enemy_base> GetEnemiesType(EnemyType type)
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