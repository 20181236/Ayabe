using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class SkillBase : MonoBehaviour, InterfaceSkill
{
    public SkillData skillData;

    // �븮�� ����
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

        // ��ų ���� ���� ����
        Debug.Log($"{skillData.skillId} �����");

        // �븮�� ȣ��??????????????????????????????????
        OnSkillExecuted?.Invoke(caster, skillData);
    }
}