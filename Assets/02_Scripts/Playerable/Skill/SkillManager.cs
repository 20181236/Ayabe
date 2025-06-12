using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance { get; private set; }

    public List<SkillData> skillDatas;  // 플레이어가 가진 스킬 데이터 리스트
    private Dictionary<SkillId, SkillBase> skillInstances = new Dictionary<SkillId, SkillBase>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        // 스킬 데이터마다 팩토리로 SkillBase 생성해서 딕셔너리에 저장
        foreach (var skillData in skillDatas)
        {
            var skill = SkillFactory.CreateSkill(skillData);
            if (skill != null)
                skillInstances[skillData.skillId] = skill;
        }
    }

    public void UseSkill(SkillId skillId, SkillContext context)
    {
        if (skillInstances.TryGetValue(skillId, out SkillBase skill))
        {
            skill.Execute(context);
        }
    }
}
