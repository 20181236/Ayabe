public enum AnimatorType
{
    isWalk,
    isAttack
}

public enum ObjectType
{
    Playable,
    Enemy,
    Neutral
}

public enum GameLayerMask
{
    Default = 0,
    TransparentFX = 1,
    IgnoreRaycast = 2,
    Water = 4,
    UI = 5,
    Playable = 6,
    Enemy = 7,
    Projectile = 8
}

/// <summary>
/// Playable
/// </summary>
public enum PlayableID
{
    SoonDoBu,
    Luna,
    Ludo
}

public enum PlayableType
{
    Front,
    Middle,
    Back
}

public enum PlayableState
{
    Create,
    Idle,
    Chasing,
    Attack,
    Dead
}
public enum PlayableAttackState
{
    BasicAttack,
    SkillAttack,
    ExSkillAttack,
    Reload,
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

/// <summary>
/// Enemy
/// </summary>
public enum EnemyID
{
    Thanker,
    Midller,
    Sinper,
    Stage1Boss,
}

public enum EnemyType
{
    Normal,
    Elite,
    Boss,

}

public enum EnemyState
{
    Create,
    Idle,
    Chasing,
    Attack,
    Dead
}

public enum EnemyAttackState
{
    BasicAttack,
    SkillAttack,
    ExSkillAttack,
    Reload,
}

public enum EnemyHealth
{
    Middler = 1000,
    Thanker = 3000,
    Boss = 5000,
}

public enum EnemyAttackRange
{
    Thanker = 50,
    Middler = 50,
    Sinper = 50,
    Boss = 100,
}

public enum WeaponType
{
    Bullet,
    Mssile,
    Grenada
}