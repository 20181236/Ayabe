using UnityEngine;

//입력 이벤트 중개 및 스킬 실행 요청
public class InputSkill : MonoBehaviour
{
    public static InputSkill instance { get; private set; }

    public SkillPanel skillPanel;
    public PlayableBase skillCaster;

    [SerializeField] private SkillToolTip skillToolTip;

    private bool isSelectingSkill = false;  // 스킬 선택 모드 상태
    private SkillData selectedSkill = null; // 현재 선택된 스킬

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
    public void SetSkillCaster(PlayableBase newCaster)
    {
        skillCaster = newCaster;
        Debug.Log($"[InputSkill] 현재 캐스터는: {skillCaster.playableID}");
    }

    public void OnSkillButtonDown(SkillId skillId, Vector2 pos)
    {
        Debug.Log($"[{skillId}] 스킬 버튼 누름 at {pos}");

        SkillData skillData = skillPanel.GetSkillDataId(skillId);

        if (skillData == null) 
            return;

        if (!isSelectingSkill)
        {
            // 선택 모드가 아니면 선택 모드 진입
            EnterSkillSelectMode(skillData);
        }
        else
        {
            if (selectedSkill == skillData)
            {
                // 같은 스킬을 눌렀으면 실행 (추후 구현)
                ExecuteSkill(skillData, pos);
            }
            else
            {
                // 다른 스킬 선택으로 변경
                EnterSkillSelectMode(skillData);
            }
        }
    }
    private void EnterSkillSelectMode(SkillData skillData)
    {
        isSelectingSkill = true;
        selectedSkill = skillData;

        Debug.Log($"스킬 선택 모드 진입: {skillData.skillId}");

        SkillEffectController.instance.StartSkillEffect();

        skillToolTip.Show(skillData);
    }
    public void ExitSkillSelectMode()
    {
        isSelectingSkill = false;
        selectedSkill = null;

        Debug.Log("스킬 선택 모드 종료 = 실행됨");

        SkillEffectController.instance.EndSkillEffect();

        skillToolTip.Hide();
    }
    private void ExecuteSkill(SkillData skillData, Vector2 pos)
    {
        Debug.Log($"스킬 실행 요청: {skillData.skillId} at {pos}");

        // 스킬 실행 요청 전달
        SkillExecutor.instance.OnSkillSelected(skillData.caster, skillData);

        ExitSkillSelectMode();


    }

    public void OnSkillButtonUp(SkillId skillId, Vector2 pos)
    {
        SkillData skillData = skillPanel.GetSkillDataId(skillId);

        if (skillData == null)
        {
            Debug.LogWarning("SkillData가 없습니다: " + skillId);
            return;
        }

        if (skillData.caster == null)
        {
            Debug.LogError($"[InputSkill] SkillData {skillId}의 caster가 설정되지 않았습니다!");
            return;
        }

        Debug.Log($"[{skillId}] 스킬 버튼 뗌 at {pos}, caster: {skillData.caster.name}");

        SkillExecutor.instance.OnSkillSelected(skillData.caster, skillData);
    }

    public void OnSkillButtonDrag(SkillId skillId, Vector2 pos)
    {
        Debug.Log($"[{skillId}] 드래그 중 at {pos}");
    }
}
