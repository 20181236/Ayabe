using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    private InterfaceSkill currentSkill;

    // �븮��(Delegate) ����: ��ų ���� �Ϸ� �� ȣ���� �̺�Ʈ
    public delegate void SkillExecutedDelegate(GameObject caster, GameObject target, SkillData skillData);
    public event SkillExecutedDelegate OnSkillExecuted;

    // ��ų ���� �޼���
    public void SetSkill(SkillBase skill, SkillData data)
    {
        currentSkill = skill;
        skill.skillData = data;
    }

    // ��ų ���� �޼���
    public void ExecuteSkill(GameObject caster, GameObject target)
    {
        if (currentSkill == null)
        {
            Debug.LogWarning("���� ��ų�� �����Ǿ� ���� �ʽ��ϴ�!");
            return;
        }

        currentSkill.SkillExecute(caster, target);

        // ��ų ���� �Ϸ� �� �븮�� ȣ��
        OnSkillExecuted?.Invoke(caster, target, (currentSkill as SkillBase).skillData);
    }
}
