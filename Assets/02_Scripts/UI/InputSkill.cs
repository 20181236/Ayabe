using UnityEngine;
using System;
using UnityEngine.UI;

//Skill분류? 및 실행?
public class InputSkill : MonoBehaviour
{
    public static InputSkill instance { get; private set; }
    public SkillPanel skillPanel;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        // skillPanel 자동 할당
        if (skillPanel == null)
        {
            skillPanel = FindObjectOfType<SkillPanel>();
            if (skillPanel == null)
                Debug.LogError("SkillPanel을 씬에서 찾을 수 없습니다!");
        }
    }

    private void Start()
    {
        // 씬 내 모든 SkillButtonHandler 찾아서 이벤트 등록
        SkillButtonHandler[] skillButtons = FindObjectsOfType<SkillButtonHandler>();
        InitializeSkillButtons(skillButtons);
    }

    // 스킬 버튼 델리게이트에 이벤트 핸들러 등록
    public void InitializeSkillButtons(SkillButtonHandler[] skillButtons)
    {
        foreach (var button in skillButtons)
        {
            button.OnSkillDown += OnSkillButtonDown;
            button.OnSkillUp += OnSkillButtonUp;
            button.OnSkillDrag += OnSkillButtonDrag;
        }
    }

    // 버튼 누름 이벤트 처리
    public void OnSkillButtonDown(SkillId skillId, Vector2 pos)
    {
        Debug.Log($"[{skillId}] 스킬 버튼 누름 at {pos}");
        // 필요하면 이 시점에 스킬 쿨타임 체크 등 추가 가능
    }

    // 버튼 뗌 이벤트 처리
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
            SkillExecutor.instance.OnSkillSelected(skillData);
        }
        else
        {
            Debug.LogWarning("SkillData가 없습니다: " + skillId);
        }
    }

    // 버튼 드래그 이벤트 처리
    public void OnSkillButtonDrag(SkillId skillId, Vector2 pos)
    {
        Debug.Log($"[{skillId}] 드래그 중 at {pos}");
        // 필요하면 드래그 관련 처리 추가 가능
    }
}
