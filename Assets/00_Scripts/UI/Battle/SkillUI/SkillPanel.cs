using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//스킬 초기화 및 캐스터 지정
public class SkillPanel : MonoBehaviour
{
    public static SkillPanel instance { get; private set; }

    public SkillButtonHandler[] skillButtons;
    public List<SkillData> skillDatas;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        InitializeSkillButtons();
    }

    private void InitializeSkillButtons()
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

    public SkillData GetSkillDataId(SkillId skillId)
    {
        foreach (var handler in skillButtons)
        {
            if (handler.SkillData != null && handler.SkillData.skillId == skillId)
                return handler.SkillData;
        }
        return null;
    }

    public void SetCasterForSkills(GameObject caster, List<SkillId> skills)
    {
        var allSkillDatas = SkillManager.instance.skillDatas;

        foreach (var data in allSkillDatas)
        {
            if (skills.Contains(data.skillId))
            {
                data.caster = caster;
                Debug.Log($"SkillPanel: {data.skillId}의 시전자로 {caster.name} 설정");
            }
        }
    }

    public void ClearSkillsForCaster(PlayableBase caster)
    {
        foreach (var button in skillButtons)
        {
            if (button.SkillData != null && button.SkillData.caster == caster.gameObject)
            {
                // 스킬 데이터 해제
                button.SetSkill(null);

                // 버튼 비활성화 또는 클릭 차단
                button.gameObject.SetActive(false);
            }
        }
    }

}
