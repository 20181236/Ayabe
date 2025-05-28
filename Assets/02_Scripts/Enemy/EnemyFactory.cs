using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyFactory
{
    public static Enemybase CreateEnemy(EnemyData data, Vector3 position)
    {
        if (data == null || data.prefab == null)
            return null;

        GameObject enemyObj = GameObject.Instantiate(data.prefab, position, Quaternion.identity);
        Enemybase enemy = enemyObj.GetComponent<Enemybase>();

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