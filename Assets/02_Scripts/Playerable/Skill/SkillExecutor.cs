using UnityEngine;

public class SkillExecutor : MonoBehaviour
{
    public static SkillExecutor instance { get; private set; }
    public GameObject caster;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void OnSkillSelected(SkillData data)
    {
        Debug.Log($"스킬 선택됨: {data.skillId}");

        SkillBase skill = SkillFactory.CreateSkill(data);

        SkillContext context = new SkillContext
        {
            Caster = caster
        };

        switch (data.castType)
        {
            case CastType.Instant:
                Debug.Log("즉시 시전 스킬");
                skill.Execute(context);
                Debug.Log("스킬 실행 완료");
                break;

            case CastType.TargetPoint:
                Debug.Log("위치 지정 스킬 - 위치 요청");
                Targeting.instance.RequestPosition(pos =>
                {
                    Debug.Log($"위치 지정 완료: {pos}");
                    context.TargetPosition = pos;
                    skill.Execute(context);
                    Debug.Log("스킬 실행 완료");
                });
                break;

            case CastType.TargetUnit:
                Debug.Log("유닛 지정 스킬 - 유닛 요청");
                Targeting.instance.RequestUnit(unit =>
                {
                    Debug.Log($"유닛 지정 완료: {unit.name}");
                    context.Target = unit;
                    skill.Execute(context);
                    Debug.Log("스킬 실행 완료");
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
