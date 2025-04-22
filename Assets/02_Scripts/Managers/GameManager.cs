using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //���� ����� ���� ����? ����? ���� ������ �ʿ�������
    public Transform charaterPosition;

    public static GameManager instance { get; private set; }
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

    public Enemy GetNearestEnemyToPosition(Vector3 fromPosition)
    {
        var enemies = EnemyManager.instance.GetEnemies();
        Debug.Log(enemies);
        Debug.Log(enemies.Count);
        if (enemies == null || enemies.Count == 0)
            return null;

        Enemy nearest = null;//���� ����� ���� ������ ���� ����(�ʱⰪ�� null)
        float minDistance = float.MaxValue;

        foreach (var enemy in enemies)
        {
            if (enemy == null) continue;

            float dist = Vector3.Distance(fromPosition, enemy.transform.position);
            
            if (dist < minDistance)//����� ������ ������� ���� �Ÿ��� üũ�ϰ� 1��2�� ���� �� ������� ���尡���༮�̴�����
            {
                minDistance = dist;
                nearest = enemy;
                Debug.Log(enemy.name + dist);
            }
        }
        Debug.Log(nearest);
        return nearest;
    }
}
