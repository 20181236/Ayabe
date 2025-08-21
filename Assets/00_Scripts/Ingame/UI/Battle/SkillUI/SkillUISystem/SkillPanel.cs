using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//스킬 초기화 및 캐스터 지정
public class SkillPanel : MonoBehaviour
{
    public static SkillPanel instance { get; private set; }

    public SkillButtonHandler[] skillButtons;
    public List<SkillData> skillDatas;

    private Dictionary<SkillId, PlayableBase> skillCasterMap = new Dictionary<SkillId, PlayableBase>();

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
        var managerSkills = SkillManager.instance.skillDatas;
        Debug.Log($"[SkillPanel] SkillManager에서 가져온 스킬 수: {managerSkills.Count}");

        for (int i = 0; i < skillButtons.Length; i++)
        {
            if (i < managerSkills.Count)
            {
                Debug.Log($"[SkillPanel] 슬롯 {i} 에 {managerSkills[i].skillId} 할당 시도");
                skillButtons[i].SetSkill(managerSkills[i]);

                skillButtons[i].OnSkillDown = InputSkill.instance.OnSkillButtonDown;
                skillButtons[i].OnSkillUp = InputSkill.instance.OnSkillButtonUp;
                skillButtons[i].OnSkillDrag = InputSkill.instance.OnSkillButtonDrag;
            }
            else
            {
                Debug.LogWarning($"[SkillPanel] 슬롯 {i} 는 비어 있음");
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

        //foreach (var data in allSkillDatas)
        //{
        //    //if (skills.Contains(data.skillId))
        //    //{
        //    //    data.caster = caster;
        //    //    Debug.Log($"SkillPanel: {data.skillId}의 시전자로 {caster.name} 설정");
        //    //}
        //}
    }

    public void ClearSkillsForCaster(PlayableBase caster)
    {
        foreach (var button in skillButtons)
        {
            if (button.SkillData != null && caster.ownedSkills.Contains(button.SkillData.skillId))
            {
                // 스킬 데이터 해제
                button.SetSkill(null);

                // 버튼 비활성화 또는 클릭 차단
                button.gameObject.SetActive(false);
            }
        }
    }

}
