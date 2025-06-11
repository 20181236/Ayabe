using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "Skill/SkillData")]
public class SkillData : ScriptableObject
{
    public SkillId skillId;
    public SkillType skillType;
    public int manaCost;
    //public float cooldown;
    //public Sprite icon;
}
