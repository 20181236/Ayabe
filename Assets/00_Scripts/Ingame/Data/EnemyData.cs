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
    public float attackPower;
    public float attackRange;
    public float attackSpeed;
    public float HealPower;
    public float skillCoolTime;
    public float exSkillCoolTime;
    public float moveSpeed;

}
