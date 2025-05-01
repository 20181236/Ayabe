using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance { get; private set; }
    public GameObject enemyPrefab;//어떤 애를 생성할건지 필요할듯
    //public List<GameObject> enemies = new List<GameObject>();//게임오브젝트에서 불러와야할게 너무 많다네
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

    // 적이 하나라도 있는지 체크
    public bool HasEnemy()
    {
        return enemies.Count > 0;
    }
}