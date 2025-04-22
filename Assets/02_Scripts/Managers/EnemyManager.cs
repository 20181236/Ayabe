using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance { get; private set; }
    public GameObject enemyPrefab;//������ ���⼭ �����ؾ��ҰŰ���
    //public List<GameObject> enemies = new List<GameObject>();//���ӿ�����Ʈ���� �ҷ��;��Ұ� �ʹ� ���ٳ�
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

    // �� ���
    public void RegisterEnemy(Enemy enemy)
    {
        if (!enemies.Contains(enemy))
        {
            enemies.Add(enemy);
        }
    }

    // �� ����
    public void UnregisterEnemy(Enemy enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
        }
    }
    // ��� �� ��������
    public List<Enemy> GetEnemies()
    {
        return enemies;
    }

    // ���� �ϳ��� �ִ��� üũ
    public bool HasEnemy()
    {
        return enemies.Count > 0;
    }


}