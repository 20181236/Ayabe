using UnityEngine;

//입력 이벤트 중개 및 스킬 실행 요청
public class InputSkill : MonoBehaviour
{
    public static InputSkill instance { get; private set; }
    public SkillPanel skillPanel;
    public PlayableBase skillCaster;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        if (skillPanel == null)
        {
            skillPanel = FindObjectOfType<SkillPanel>();
            if (skillPanel == null)
                Debug.LogError("SkillPanel을 씬에서 찾을 수 없습니다!");
        }
    }

    private void Start()
    {
        if (skillCaster == null)
        {
            skillCaster = FindObjectOfType<PlayableBase>();
            if (skillCaster == null)
                Debug.LogError("PlayableBase (스킬 캐스터)를 찾을 수 없습니다!");
        }
        SkillButtonHandler[] skillButtons = FindObjectsOfType<SkillButtonHandler>();
        InitializeSkillButtons(skillButtons);
    }

    public void InitializeSkillButtons(SkillButtonHandler[] skillButtons)
    {
        foreach (var button in skillButtons)
        {
            button.OnSkillDown += OnSkillButtonDown;
            button.OnSkillUp += OnSkillButtonUp;
            button.OnSkillDrag += OnSkillButtonDrag;
        }
    }

    public void OnSkillButtonDown(SkillId skillId, Vector2 pos)
    {
        Debug.Log($"[{skillId}] 스킬 버튼 누름 at {pos}");
        SkillEffectController.instance?.PlaySkillEffect();
    }

    public void OnSkillButtonUp(SkillId skillId, Vector2 pos)
    {
        SkillData skillData = skillPanel.GetSkillDataById(skillId);
        if (skillData == null) return;

        if (skillCaster == null)
        {
            Debug.LogError("스킬 캐스터가 설정되어 있지 않습니다!");
            return;
        }

        if (skillData.castType == CastType.Instant)
        {
            SkillExecutor.instance.OnSkillSelected(skillCaster.gameObject, skillData);
        }
        else if (skillData.castType == CastType.TargetPoint)
        {
            Targeting.instance.RequestPosition(skillData, targetPos =>
            {
                SkillExecutor.instance.ExecuteSkill(skillData, targetPos, skillCaster.gameObject);
            });
        }
        else if (skillData.castType == CastType.TargetUnit)
        {
            Targeting.instance.RequestUnit(unit =>
            {
                SkillContext context = new SkillContext
                {
                    Caster = skillCaster.gameObject,
                    Target = unit
                };
                SkillEffectController.instance.PlaySkillEffect();
                SkillBase skill = SkillFactory.CreateSkill(skillData);
                skill.Execute(context);
                SkillExecutor.instance.ClearAllHighlights();
            }, unit => SkillExecutor.instance.FilteringTeamSkill(unit, skillData.skillType));
        }
    }


    public void OnSkillButtonDrag(SkillId skillId, Vector2 pos)
    {
        Debug.Log($"[{skillId}] 드래그 중 at {pos}");
    }
}
