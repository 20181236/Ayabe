using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyFactory
{
    public static Enemy_base CreateEnemy(EnemyData data, Vector3 position)
    {
        if (data == null || data.prefab == null)
        {
            Debug.LogError("EnemyData or prefab is null");
            return null;    
        }

        GameObject enemyObj = GameObject.Instantiate(data.prefab, position, Quaternion.identity);
        Debug.Log($"[EnemyFactory] Creating enemy of type {data.enemyType} at {position}");
        Enemy_base enemy = enemyObj.GetComponent<Enemy_base>();

        if (enemy != null)
        {
            enemy.SetData(data); // Enemy¿« Ω∫≈» º≥¡§
        }
        else
        {
            Debug.LogError("Prefab does not contain Enemy_base component");
        }

        return enemy;
    }
}