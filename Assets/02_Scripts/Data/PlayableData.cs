using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "PlayableData", menuName = "PlayableData")]
public class PlayableData : ScriptableObject
{
    public PlayableType playableType;
    public GameObject prefab;
    public float maxHealth;
    public float attackRange;
    public float basicAttackInterval;
    public float skillInterval;
    public float exSkillInterval;
    public float moveSpeed;
}
