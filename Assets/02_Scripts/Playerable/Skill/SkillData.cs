using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "Skill/SkillData")]
public class SkillData : ScriptableObject
{
    public SkillId skillId;
    public SkillType skillType;
    public CastType castType;
    public Sprite icon;
    public int manaCost;
    public float renge;
    public float skillRadius;
    public float healValue;
}