using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "EnemyData", menuName = "EnemyData")]
public class EnemyData : ScriptableObject
{
    public EnemyID enemyID;
    public EnemyType enemyType;
    public GameObject prefab;
    public float maxHealth;
    public float attackRange;
    public float basicAttackInterval;
    public float skillInterval;
    public float exSkillInterval;
    public float moveSpeed;
}
