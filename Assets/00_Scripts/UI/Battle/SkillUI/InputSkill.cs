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
        if (skillPanel == null)
        {
            Debug.LogError("skillPanel is NULL!");
            return;
        }

        Debug.Log($"[{skillId}] 스킬 버튼 뗌 at {pos}");

        SkillData skillData = skillPanel.GetSkillDataById(skillId);
        if (skillData != null)
        {
            if (skillCaster != null)
            {
                SkillExecutor.instance.OnSkillSelected(skillCaster.gameObject, skillData);
            }
            else
            {
                Debug.LogError("스킬 캐스터가 설정되어 있지 않습니다!");
            }
        }
        else
        {
            Debug.LogWarning("SkillData가 없습니다: " + skillId);
        }
    }


    public void OnSkillButtonDrag(SkillId skillId, Vector2 pos)
    {
        Debug.Log($"[{skillId}] 드래그 중 at {pos}");
    }
}
