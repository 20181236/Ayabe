public enum SkillType
{
    Self,
    Target,
    Area,
    Heal,
    Buff
}
public enum BuffType
{
    AttackPower,
    MoveSpeed,
    Defense,
    AttackSpeed,
    CriticalRate
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