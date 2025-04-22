using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance { get; private set; }
    public GameObject enemyPrefab;//생성도 여기서 관리해야할거같음
    //public List<GameObject> enemies = new List<GameObject>();//게임오브젝트에서 불러와야할게 너무 많다네
    private List<Enemy> enemies = new List<Enemy>();

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

    // 적 등록
    public void RegisterEnemy(Enemy enemy)
    {
        if (!enemies.Contains(enemy))
        {
            enemies.Add(enemy);
        }
    }

    // 적 제거
    public void UnregisterEnemy(Enemy enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
        }
    }
    // 모든 적 가져오기
    public List<Enemy> GetEnemies()
    {
        return enemies;
    }

    // 적이 하나라도 있는지 체크
    public bool HasEnemy()
    {
        return enemies.Count > 0;
    }


}