using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SkillFactory
{
    public static SkillBase CreateSkill(SkillData data)
    {
        switch (data.skillId)
        {
            case SkillId.SoonDoBuSkill:
                return new SoonDoBuSkill(data);
            case SkillId.LunaSkill:
                return new LunaSkill(data);
            //case SkillId.LudoSkill:
            //    return new LudoSkill(data);
            default:
                Debug.LogWarning("Unknown skillId: " + data.skillId);
                return null;
        }
    }
}
