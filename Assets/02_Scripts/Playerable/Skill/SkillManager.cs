using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    private InterfaceSkill currentSkill;

    // 대리자(Delegate) 선언: 스킬 실행 완료 시 호출할 이벤트
    public delegate void SkillExecutedDelegate(GameObject caster, GameObject target, SkillData skillData);
    public event SkillExecutedDelegate OnSkillExecuted;

    // 스킬 세팅 메서드
    public void SetSkill(SkillBase skill, SkillData data)
    {
        currentSkill = skill;
        skill.skillData = data;
    }

    // 스킬 실행 메서드
    public void ExecuteSkill(GameObject caster, GameObject target)
    {
        if (currentSkill == null)
        {
            Debug.LogWarning("현재 스킬이 설정되어 있지 않습니다!");
            return;
        }

        currentSkill.SkillExecute(caster, target);

        // 스킬 실행 완료 후 대리자 호출
        OnSkillExecuted?.Invoke(caster, target, (currentSkill as SkillBase).skillData);
    }
}
