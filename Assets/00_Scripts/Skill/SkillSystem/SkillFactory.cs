using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SkillFactory
{
    public static SkillBase CreateSkill(SkillData data)
    {
        SkillBase skillInstance = null;

        switch (data.skillId)
        {
            case SkillId.SoonDoBuSkill:
                skillInstance = InstantiateSkillComponent<SoonDoBuSkill>();
                break;
            case SkillId.LunaSkill:
                skillInstance = InstantiateSkillComponent<LunaSkill>();
                break;
            case SkillId.LudoSkill:
                skillInstance = InstantiateSkillComponent<LudoSkill>();
                break;
            default:
                Debug.LogWarning("Unknown skillId: " + data.skillId);
                break;
        }

        if (skillInstance != null)
        {
            skillInstance.Initialize(data);  // 생성자 대신 초기화 메서드 호출
        }

        return skillInstance;
    }

    private static T InstantiateSkillComponent<T>() where T : SkillBase
    {
        GameObject skillGameObject = new GameObject(typeof(T).Name);
        return skillGameObject.AddComponent<T>();
    }
}

