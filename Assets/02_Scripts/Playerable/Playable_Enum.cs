using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayableState
{
    Create,
    Idle,
    Chasing,
    Attack,
    Skill,
    ExSkill,
    Dead
}
public enum PlayableType
{
    forward,
    middle,
    back
}

public enum PlayableHelath
{
    SoonDobu = 5000,
    Luna = 5000,
    Ludo = 5000,
}
public enum PlayableAttackRenge
{
    SoonDobu = 50,
    Luna = 50,
    Ludo = 50,
}

public enum PlayalbeBaiscSkillCoolTime
{
    SoonDobu = 10,
    Luna = 5,
    Ludo = 5,
}