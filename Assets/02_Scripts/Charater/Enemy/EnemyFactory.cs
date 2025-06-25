using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyFactory
{
    public static EnemyBase CreateEnemy(EnemyData data, Vector3 position)
    {
        if (data == null || data.prefab == null)
            return null;

        GameObject enemyObject = GameObject.Instantiate(data.prefab, position, Quaternion.identity);
        EnemyBase enemy = enemyObject.GetComponent<EnemyBase>();

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