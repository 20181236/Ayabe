using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ePlayableState
{
    Create,
    Idle,
    TargetSearch,
    CheckRange,
    Move,
    Attack,
    Death
}
public enum ePlayableType
{
    forward,
    middle,
    back
}

public enum ePlayableHelath
{
    SoonDobu = 200,
    Luna = 150,
    Ludo = 100,
}
public enum ePlayableAttackRenge
{
    SoonDobu = 15,
    Luna = 20,
    Ludo = 25,
}
