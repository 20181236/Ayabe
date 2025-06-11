//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class SkillExecutor
//{
//    private Dictionary<SkillId, InterfaceSkill> skillStrategies = new Dictionary<SkillId, InterfaceSkill>();

//    public void RegisterSkill(SkillId skillId, InterfaceSkill skill)
//    {
//        skillStrategies[skillId] = skill;
//    }

//    public bool ExecuteSkill(SkillId skillId, GameObject caster, GameObject target)
//    {
//        if (!skillStrategies.TryGetValue(skillId, out var skill))
//        {
//            Debug.LogWarning($"Skill {skillId}�� ��ϵǾ� ���� �ʽ��ϴ�.");
//            return false;
//        }

//        if (skill is SkillBase skillBase)
//        {
//            if (!ManaManager.instance.UseMana(skillBase.skillData.manaCost))
//            {
//                Debug.Log("��ų ��� ����: ���� ����");
//                return false;
//            }
//        }
//        else
//        {
//            Debug.LogWarning("SkillBase ��� ���� ��ų�� ���� ��� ó�� �Ұ�");
//        }

//        skill.SkillExecute(caster, target);
//        return true;
//    }
//}

