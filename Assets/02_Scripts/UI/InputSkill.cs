using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

        //  skillPanel 자동 할당
        if (skillPanel == null)
        {
            skillPanel = FindObjectOfType<SkillPanel>();
            if (skillPanel == null)
                Debug.LogError("SkillPanel을 씬에서 찾을 수 없습니다!");
        }
    }

    public void OnSkillButtonDown(SkillId skillId, Vector2 pos)
    {
        Debug.Log($"[{skillId}] 스킬 버튼 누름 at {pos}");
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
            SkillExecutor.instance.OnSkillSelected(skillData);
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
