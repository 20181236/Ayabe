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
<<<<<<< HEAD
    private void Start()
    {
        InitializePools();
    }

    private void InitializePools()
    {
        foreach (var setting in enemyPoolSettings)
        {
            Queue<Enemy_base> pool = new Queue<Enemy_base>();
            for (int i = 0; i < setting.poolSize; i++)
            {
                GameObject enemyObject = Instantiate(setting.enemyPrefab);
                enemyObject.SetActive(false);
                pool.Enqueue(enemyObject.GetComponent<Enemy_base>());
            }
            enemyPools.Add(setting.enemyType, pool);
        }
    }

    public Enemy_base SpawnEnemy(string type, Vector3 position, Quaternion rotation)
    {
        if (!enemyPools.ContainsKey(type))
        {
            Debug.LogError($"Enemy type '{type}' not found in pool.");
            return null;
        }

        Queue<Enemy_base> pool = enemyPools[type];

        if (pool.Count == 0)
        {
            Debug.LogWarning($"Pool for '{type}' is empty! Instantiating new one.");
            EnemyPoolSetting setting = enemyPoolSettings.Find(x => x.enemyType == type);
            if (setting == null)
            {
                Debug.LogError($"No pool setting found for type '{type}'");
                return null;
            }

            GameObject enemyObject = Instantiate(setting.enemyPrefab);
            enemyObject.SetActive(false);
            pool.Enqueue(enemyObject.GetComponent<Enemy_base>());
        }

        Enemy_base enemy = pool.Dequeue();
        enemy.transform.position = position;
        enemy.transform.rotation = rotation;
        enemy.gameObject.SetActive(true);

        RegisterEnemy(enemy);

        return enemy;
    }

    public void DespawnEnemy(string type, Enemy_base enemy)
    {
        if (!enemyPools.ContainsKey(type))
        {
            Debug.LogWarning($"Enemy pool for type '{type}' does not exist. Destroying object.");
            Destroy(enemy.gameObject);
            return;
        }

        UnregisterEnemy(enemy);
        enemy.gameObject.SetActive(false);
        enemyPools[type].Enqueue(enemy);
    }
=======
>>>>>>> parent of c1d2e0f (EnemyPolling_init)

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