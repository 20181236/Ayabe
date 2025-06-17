using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoonDoBuSkill : SkillBase
{
    public SoonDoBuSkill(SkillData data) : base(data) { }

    public override void Execute(SkillContext context)
    {
        if (context.Target == null)
        {
            Debug.LogWarning("타겟이 없습니다. 스킬 사용 불가");
            return;
        }

        var playable = context.Target.GetComponent<PlayableBase>();
        if (playable != null && !playable.isDead)
        {
            playable.Heal(skillData.healValue);
            Debug.Log($"{context.Target.name}에게 {skillData.healValue} 힐 시전 완료");
        }
        else
        {
            Debug.LogWarning("타겟이 죽었거나 PlayableBase가 없습니다");
        }
    }
}
