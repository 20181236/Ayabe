using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance { get; private set; }
    public GameObject enemyPrefab;
    public List<Enemy_base> enemies = new List<Enemy_base>();
    public Transform[] EnemySpawnPoints;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RegisterEnemy(Enemy_base enemy)
    {
        if (!enemies.Contains(enemy))
            enemies.Add(enemy);
    }

    public void UnregisterEnemy(Enemy_base enemy)
    {
        if (enemies.Contains(enemy))
            enemies.Remove(enemy);
    }

    public List<Enemy_base> GetEnemies()
    {
        return enemies;
    }
    public bool HasEnemy()
    {
        return enemies.Count > 0;
    }
}