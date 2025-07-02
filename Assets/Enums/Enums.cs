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

public enum BuffStatType
{
    MaxHealth,       // 최대 체력
    AttackPower,     // 공격력
    AttackInterval,     // 공격 속도 (속도는 증가, 쿨타임 감소)
    AttackRange,      // 공격 사거리
    CriticalRate,    // 치명타 확률
    CriticalDamage, //치명타 데미지 증가
    HealPower,       // 치유량
}
public enum BuffCategory
{
    Buff,
    Debuff,
}
public enum BuffApplyType
{
    Burst,
    Tick,
    Both
}
public enum SkillType
{
    Attack,
    Heal,
    Buff,
    Debuff
}
public enum SkillTargetType
{
    Self,
    TargetUnit,
    TeamAll,
    Area
}

public enum CastType
{
    Instant,     // 클릭 즉시 발동
    TargetUnit,  // 유닛 지정 필요
    TargetPoint  // 위치 지정 필요
}
public enum SkillId
{
    SoonDoBuSkill,
    LunaSkill,
    LudoSkill
}
public enum AreaType
{
    Damage,
    Heal
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

public enum EnemySpawnState
{
    None,       // 아직 적이 한 마리도 안 나옴
    Spawned,    // 적이 한 번이라도 소환됨
    Cleared     // 적이 모두 제거됨
}

public enum WeaponType
{
    Bullet,
    Mssile,
    Grenada
}
