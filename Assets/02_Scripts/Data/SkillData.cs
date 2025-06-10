using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum SkillType
{
    None,
    AreaAttack,
    TargetAttack,
    Heal,
    Buff
}
[CreateAssetMenu(fileName = "SkillData", menuName = "Skill/SkillData")]
public class SkillData : ScriptableObject
{
    public string skillName;
    public int manaCost;
    //public float cooldown;
    //public Sprite icon;
    public SkillType skillType; // �������Ͽ� �ĺ��� (��: ��������, ���ϰ��� ��)

}
