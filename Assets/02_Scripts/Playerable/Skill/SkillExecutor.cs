using UnityEngine;

public class SkillExecutor : MonoBehaviour
{
    public GameObject caster;

    public void OnSkillSelected(SkillData data)
    {
        SkillBase skill = SkillFactory.CreateSkill(data);

        SkillContext context = new SkillContext
        {
            Caster = caster
        };

        switch (data.castType)
        {
            case CastType.Instant:
                skill.Execute(context);
                break;

            case CastType.TargetPoint:
                Targeting.instance.RequestPosition(pos =>
                {
                    context.TargetPosition = pos;
                    skill.Execute(context);
                });
                break;

            case CastType.TargetUnit:
                Targeting.instance.RequestUnit(unit =>
                {
                    context.Target = unit;  // SkillContext의 Target 프로퍼티에 저장
                    skill.Execute(context);
                }, unit => FilteringTeamSkill(unit, data.skillType));
                break;
        }
    }
    private bool FilteringTeamSkill(GameObject unit, SkillType skillType)
    {
        var character = unit.GetComponent<CharacterBase>();
        if (character == null)
            return false;

        switch (skillType)
        {
            case SkillType.TargetAttack:
                return character.ObjectType == ObjectType.Enemy;

            case SkillType.TargetHeal:
            case SkillType.Buff:
                return character.ObjectType == ObjectType.Playable;

            default:
                return false;
        }
    }
}
