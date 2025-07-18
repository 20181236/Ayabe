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
            Debug.LogError("SkillExecutor: caster.");
            return;
        }

        Debug.Log($": {data.skillId}");

        SkillBase skill = SkillFactory.CreateSkill(data);

        SkillContext context = new SkillContext
        {
            Caster = caster
        };

        SkillEffectController.instance.PlaySkillEffect();

        switch (data.castType)
        {
            case CastType.Instant:
                skill.Execute(context);
                break;

            case CastType.TargetPoint:
                Targeting.instance.StartPositionTargeting(data, pos =>
                {
                    context.TargetPosition = pos;
                    skill.Execute(context);
                });
                break;

            case CastType.TargetUnit:
                Targeting.instance.StartUnitTargeting(unit =>
                {
                    context.Target = unit;
                    ClearAllHighlights(); 
                    skill.Execute(context);
                },
                unit => FilteringTeamSkill(unit, data.skillType));
                break;
        }

        if (data.castType == CastType.TargetUnit)
        {
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