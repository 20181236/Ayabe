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
//            Debug.LogWarning($"Skill {skillId}가 등록되어 있지 않습니다.");
//            return false;
//        }

//        if (skill is SkillBase skillBase)
//        {
//            if (!ManaManager.instance.UseMana(skillBase.skillData.manaCost))
//            {
//                Debug.Log("스킬 사용 실패: 마나 부족");
//                return false;
//            }
//        }
//        else
//        {
//            Debug.LogWarning("SkillBase 상속 안한 스킬은 마나 사용 처리 불가");
//        }

//        skill.SkillExecute(caster, target);
//        return true;
//    }
//}

