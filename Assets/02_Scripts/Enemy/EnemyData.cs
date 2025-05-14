using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "EnemyData", menuName = "Resources/EnemyData")]//, order = 1)]
public class EnemyData : ScriptableObject
{
    public EnemyType enemyType;
    public GameObject prefab;
    public float maxHealth;
    public float attackRange;
    public float basicAttackInterval;
    public float skillInterval;
    public float exSkillInterval;
    public float moveSpeed;
}
