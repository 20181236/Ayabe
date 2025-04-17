using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private List<Enemy> enemies = new List<Enemy>();//���ʹ� ����Ʈ ���� OK

    public float detectionRange = 10f; // Ž�� �Ÿ� OK
    public string enemyTag = "Enemy"; // ������ �±� ????????????? �±׷ΰ���?

    public LayerMask detectionLayer; // ������ ���̾� ���̾�� OK

    private static EnemyManager instance = null;
    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static EnemyManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    public void RegisterEnemy(Enemy enemy)//�����Լ� OK
    {
        if (!enemies.Contains(enemy))
            enemies.Add(enemy);
    }
    public void UnregisterEnemy(Enemy enemy)//�׿����� �����Լ� OK
    {
        if (enemies.Contains(enemy))
            enemies.Remove(enemy);
    }

    //���� ���� �ڵ尡�ƴ� gpt���� �̻��Ҽ�����
    //distance�� �Ⱦ���
    //�ʿ���°� ����, ���̾ ������ �ȳ����Ŀ�����
    public Enemy GetClosestEnemy(Vector3 fromCharaterPosition)//���ؾȵ�
    {
        Enemy closest = null;//ó�� �ʱ�ȭ OK
        float minDist = detectionRange * detectionRange;//sqrMagnitude�� ���� �Ŵϱ� �Ÿ� ������ ���� OK

        foreach (var enemy in enemies)//���⼭���� Ž���ϸ鼭 �����ٴ� �� OK
        {
            if (enemy == null || )//������ �ѱ�OK
                continue;

            if (!enemy.CompareTag(enemyTag))//Enemy�±� �ƴϸ� ���� OK
                continue;

            if (((1 << enemy.gameObject.layer) & detectionLayer) == 0)//���ش� �ȵ����� ���̾� Ž�� '����'???? ��Ʈ�ΰ��� ���ľߵ� ���̾ 4�������� �̰�ó����
                continue;

            float dist = (enemy.transform.position - fromCharaterPosition).sqrMagnitude;//���� ������ �˰���, �̰ͺ��� distance�� ����������� �� �Ÿ��� �����ϴ� 

            if (dist < minDist)
            {
                minDist = dist;
                closest = enemy;
            }
        }

        return closest;
    }
}
