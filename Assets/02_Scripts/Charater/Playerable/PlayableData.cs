using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "PlayableData", menuName = "PlayableData")]
public class PlayableData : ScriptableObject
{
    public PlayableID playableID;
    public PlayableType playableType;
    public GameObject prefab;
    public float maxHealth;
    public float attackPower;
    public float attackRange;
    public float basicAttackInterval;
    public float skillInterval;
    public float exSkillInterval;
    public float moveSpeed;

    public SkillData exSkillData;  // 고유 Ex스킬 데이터 연결
}
