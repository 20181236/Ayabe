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
    Skill,
    ExSkill,
    Dead
}

public enum EnemyHealth
{
    Middler = 1000,
    Thanker=3000,
    Boss = 5000,
}

public enum EnemyAttackRange
{
    Thanker =  30,
    Middler = 30,
    Boss = 100,
}

