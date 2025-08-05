using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SkillFactory
{
    public static SkillBase CreateSkill(SkillData data)
    {
        SkillBase skill = null;

        switch (data.skillId)
        {
            case SkillId.SoonDoBuSkill:
                skill = new SoonDoBuSkill();
                break;
            case SkillId.LunaSkill:
                skill = new LunaSkill();
                break;
            case SkillId.LudoSkill:
                skill = new LudoSkill();
                break;
            default:
                Debug.LogWarning($"[SkillFactory] Unknown skillId: {data.skillId}");
                return null;
        }

        skill.Initialize(data); // 여기서 초기화
        return skill;
    }
}
