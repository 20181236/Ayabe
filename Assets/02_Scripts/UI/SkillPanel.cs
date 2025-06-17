using UnityEngine;

public class SkillPanel : MonoBehaviour
{
    public SkillButtonHandler[] skillButtons;

    private void Start()
    {
        AutoAssignSkills();
    }

    private void AutoAssignSkills()
    {
        var skillDatas = SkillManager.instance.skillDatas;

        for (int i = 0; i < skillButtons.Length; i++)
        {
            if (i < skillDatas.Count)
            {
                skillButtons[i].SetSkill(skillDatas[i]);
                Debug.Log($"SkillPanel: 슬롯 {i} 에 {skillDatas[i].skillId} 할당");
            }
            else
            {
                Debug.LogWarning($"SkillPanel: 슬롯 {i} 는 비어 있음 (skillDatas 부족)");
            }
        }
    }

    public SkillData GetSkillDataById(SkillId skillId)
    {
        foreach (var handler in skillButtons)
        {
            if (handler.SkillData != null && handler.SkillData.skillId == skillId)
                return handler.SkillData;
        }
        return null;
    }
}
