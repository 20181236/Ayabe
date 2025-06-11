using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class SkillBase : MonoBehaviour, InterfaceSkill
{
    public SkillData skillData;

    // 대리자 구현
    public event Action<GameObject, SkillData> OnSkillExecuted;

    protected virtual bool UseSkill()
    {
        if (ManaManager.instance == null)
            return false;

        return ManaManager.instance.UseMana(skillData.manaCost);
    }

    public virtual void SkillExecute(GameObject caster, GameObject target)
    {
        if (!UseSkill())
            return;

        // 스킬 실제 동작 구현
        Debug.Log($"{skillData.skillId} 실행됨");

        // 대리자 호출??????????????????????????????????
        OnSkillExecuted?.Invoke(caster, skillData);
    }
}