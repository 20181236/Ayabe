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

    public void ExecuteSkill(SkillData data, Vector3 targetPosition, GameObject caster)
    {
        if (caster == null)
        {
            Debug.LogError("SkillExecutor: caster가 할당되지 않았습니다.");
            return;
        }

        SkillBase skill = SkillFactory.CreateSkill(data);

        SkillContext context = new SkillContext
        {
            Caster = caster,
            TargetPosition = targetPosition
        };

        SkillEffectController.instance.PlaySkillEffect();

        skill.Execute(context);
    }

    public void OnSkillSelected(GameObject caster, SkillData data)
    {
        if (caster == null)
        {
            Debug.LogError("SkillExecutor: caster가 할당되지 않았습니다.");
            return;
        }

        Debug.Log($"스킬 선택됨: {data.skillId}");

        if (data.castType == CastType.Instant)
        {
            SkillBase skill = SkillFactory.CreateSkill(data);
            SkillContext context = new SkillContext
            {
                Caster = caster
            };
            SkillEffectController.instance.PlaySkillEffect();
            skill.Execute(context);
        }
        else
        {
            Debug.Log("타겟팅이 필요한 스킬입니다. InputSkill에서 위치/유닛 선택 요청하세요.");
        }
    }

    public bool FilteringTeamSkill(GameObject unit, SkillType skillType)
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

    public void HighlightTargets(SkillType skillType)
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

    public void ClearAllHighlights()
    {
        HighlightEffect[] allHighlights = FindObjectsOfType<HighlightEffect>();
        foreach (var highlight in allHighlights)
        {
            highlight.SetHighlight(false);
        }
    }
}
