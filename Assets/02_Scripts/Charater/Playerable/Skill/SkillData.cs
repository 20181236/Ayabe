using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "Skill/SkillData")]
public class SkillData : ScriptableObject
{
    public SkillId skillId;
    public SkillType skillType;
    public CastType castType;
    public AreaType areaType;   // 장판이 딜인지 힐인지 구분
    public GameObject weaponPrefab;
    public Sprite icon;
    public int manaCost;
    public float range;
    public float skillRadius;
    public float skillAngle;
    public float healValue;
    public float effectAmount;               // 딜 혹은 힐량 (healValue 대신 통합 가능)
    public float effectInterval = 1f;       //주기
    public float areaDuration;        // 장판 지속 시간 (초)
}