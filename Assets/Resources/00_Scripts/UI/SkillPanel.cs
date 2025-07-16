using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//스킬 초기화 및 캐스터 지정
public class SkillPanel : MonoBehaviour
{
    public SkillButtonHandler[] skillButtons;
    public List<SkillData> skillDatas;

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
                var handler = skillButtons[i];

                // 대리자 할당
                handler.OnSkillDown = InputSkill.instance.OnSkillButtonDown;
                handler.OnSkillUp = InputSkill.instance.OnSkillButtonUp;
                handler.OnSkillDrag = InputSkill.instance.OnSkillButtonDrag;

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

    public void AssignCasterToSkills(GameObject caster, List<SkillId> skills)
    {
        foreach (var data in skillDatas)
        {
            if (skills.Contains(data.skillId))
            {
                data.caster = caster;
                Debug.Log($"SkillPanel: {data.skillId}의 시전자로 {caster.name} 설정");
            }
        }
    }
}
