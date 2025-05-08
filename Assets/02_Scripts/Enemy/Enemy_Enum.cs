public enum EnemyType
{
    Melee,
    Ranged,
    Elite,
    Boss
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
    Thanker=3000,
    Boss = 5000,
}

public enum EnemyAttackRange
{
    Thanker =  50,
    Middler = 50,
    Sinper = 50,
    Boss = 100,
}

