using UnityEngine;

//입력 이벤트 중개 및 스킬 실행 요청
public class InputSkill : MonoBehaviour
{
    public static InputSkill instance { get; private set; }

    public SkillPanel skillPanel;
    public PlayableBase skillCaster;

    [SerializeField] private ToolTip skillToolTip;

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
        Debug.Log($"[InputSkill] 현재 캐스터 변경됨: {skillCaster.name} (ID: {skillCaster.playableID})");
    }

    public void OnSkillButtonDown(SkillId skillId, Vector2 pos)
    {
        Debug.Log($"[{skillId}] 스킬 버튼 누름 at {pos}");
        Debug.Log($"[InputSkill] OnSkillButtonDown 호출 - 스킬ID: {skillId}, 포지션: {pos}");

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

        ScreenAndTimeEffectController.instance.StartEffect();

        skillToolTip.Show(skillData);
    }
    public void ExitSkillSelectMode()
    {
        isSelectingSkill = false;
        selectedSkill = null;

        Debug.Log("스킬 선택 모드 종료 = 실행됨");

        ScreenAndTimeEffectController.instance.EndEffect();

        skillToolTip.Hide();
    }

    private PlayableBase FindCasterByOwnerId(PlayableID ownerId)
    {
        var allCasters = FindObjectsOfType<PlayableBase>();
        foreach (var caster in allCasters)
        {
            if (caster.playableID == ownerId)
                return caster;
        }
        return null;
    }

    private void ExecuteSkill(SkillData skillData, Vector2 pos)
    {
        if (skillCaster == null)
        {
            Debug.LogError("[InputSkill] skillCaster가 설정되지 않았습니다!");
            return;
        }

        Debug.Log($"[InputSkill] SkillExecutor에 스킬 실행 요청 - 캐스터: {skillCaster.name}, 스킬: {skillData.skillId}");
        SkillExecutor.instance.OnSkillSelected(skillCaster.gameObject, skillData);

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

        PlayableBase caster = FindCasterByOwnerId(skillData.ownerId);
        if (caster == null)
        {
            Debug.LogError("[InputSkill] 캐스터를 찾을 수 없습니다!");
            return;
        }

        SkillExecutor.instance.OnSkillSelected(caster.gameObject, skillData);
    }


    public void OnSkillButtonDrag(SkillId skillId, Vector2 pos)
    {
        Debug.Log($"[{skillId}] 드래그 중 at {pos}");
    }
}
