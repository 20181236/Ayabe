using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyFactory
{
    public static Enemybase CreateEnemy(EnemyData data, Vector3 position)
    {
        if (data == null || data.prefab == null)
        {
            Debug.LogError("EnemyData or prefab is null");
            return null;    
        }

        GameObject enemyObj = GameObject.Instantiate(data.prefab, position, Quaternion.identity);
        Debug.Log($"[EnemyFactory] Creating enemy of type {data.enemyType} at {position}");
        Enemybase enemy = enemyObj.GetComponent<Enemybase>();

        if (enemy != null)
        {
            enemy.SetData(data); // Enemy�� ���� ����
        }
        else
        {
            Debug.LogError("Prefab does not contain Enemy_base component");
        }

        return enemy;
    }
}