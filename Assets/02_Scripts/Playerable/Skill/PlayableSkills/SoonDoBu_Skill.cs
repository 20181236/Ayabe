using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoonDoBuSkill : SkillBase
{
    public SoonDoBuSkill(SkillData data) : base(data) { }

    public override void Execute(SkillContext context)
    {
        if (context.Target == null)
            return;

        var playable = context.Target.GetComponent<PlayableBase>();
        if (playable != null && !playable.isDead)
        {
            playable.Heal(skillData.healValue);
            Debug.Log($"{context.Target.name}¿¡°Ô {skillData.healValue} Èú");
        }
    }
}