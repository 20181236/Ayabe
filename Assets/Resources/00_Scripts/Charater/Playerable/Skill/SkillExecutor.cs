using UnityEngine;

public class SkillExecutor : MonoBehaviour
{
    public static SkillExecutor instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void OnSkillSelected(GameObject caster, SkillData data)
    {
        if (caster == null)
        {
            Debug.LogError("SkillExecutor: caster가 할당되지 않았습니다.");
            return;
        }

        Debug.Log($"스킬 선택됨: {data.skillId}");

        SkillBase skill = SkillFactory.CreateSkill(data);

        SkillContext context = new SkillContext
        {
            Caster = caster
        };

        SkillEffectController.instance.PlaySkillEffect(); // 어둡게 처리

        switch (data.castType)
        {
            case CastType.Instant:
                skill.Execute(context);
                break;

            case CastType.TargetPoint:
                Targeting.instance.RequestPosition(data, pos =>
                {
                    context.TargetPosition = pos;
                    skill.Execute(context);
                });
                break;

            case CastType.TargetUnit:
                Targeting.instance.RequestUnit(unit =>
                {
                    context.Target = unit;
                    ClearAllHighlights(); // 선택 후 하이라이트 제거
                    skill.Execute(context);
                },
                unit => FilteringTeamSkill(unit, data.skillType));
                break;
        }

        if (data.castType == CastType.TargetUnit)
        {
            // 대상 유닛 미리 하이라이트 표시
            HighlightTargets(data.skillType);
        }
    }

    private bool FilteringTeamSkill(GameObject unit, SkillType skillType)
    {
        var character = unit.GetComponent<CharacterBase>();
        if (character == null)
            return false;

        switch (skillType)
        {
            case SkillType.Attack:
                return character.ObjectType == ObjectType.Enemy;
            case SkillType.Heal:
            case SkillType.Buff:
                return character.ObjectType == ObjectType.Playable;
            default:
                return false;
        }
    }

    private void HighlightTargets(SkillType skillType)
    {
        Debug.Log($"HighlightTargets called with skillType: {skillType}");

        CharacterBase[] allCharacters = FindObjectsOfType<CharacterBase>();
        foreach (var character in allCharacters)
        {
            GameObject gameoObject = character.gameObject;
            var highlight = gameoObject.GetComponent<HighlightEffect>();
            if (highlight == null)
            {
                Debug.Log($"No HighlightEffect found on {gameoObject.name}");
                continue;
            }

            bool shouldHighlight = FilteringTeamSkill(gameoObject, skillType);
            Debug.Log($"{gameoObject.name} shouldHighlight: {shouldHighlight}");
            highlight.SetHighlight(shouldHighlight);
        }
    }

    private void ClearAllHighlights()
    {
        HighlightEffect[] allHighlights = FindObjectsOfType<HighlightEffect>();
        foreach (var highlight in allHighlights)
        {
            highlight.SetHighlight(false);
        }
    }
}
