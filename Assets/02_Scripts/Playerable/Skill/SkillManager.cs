using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance { get; private set; }

    public List<SkillData> skillDatas;  // �÷��̾ ���� ��ų ������ ����Ʈ
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
        // ��ų �����͸��� ���丮�� SkillBase �����ؼ� ��ųʸ��� ����
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
