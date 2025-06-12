using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LunaSkill : SkillBase
{
    private float skillRadius;

    public LunaSkill(SkillData data) : base(data)
    {
        skillRadius = data.skillRadius;
    }

    public override void Execute(SkillContext context)
    {
        if (context.Caster == null)
            return;

        var casterPlayable = context.Caster.GetComponent<PlayableBase>();
        if (casterPlayable == null)
            return;

        List<PlayableBase> targetsInRange = new List<PlayableBase>();

        foreach (var playable in PlayableManager.instance.GetPlayables())
        {
            if (playable == null || playable.isDead)
                continue;

            if (playable.playableType != casterPlayable.playableType)
                continue;

            float dist = Vector3.Distance(context.Caster.transform.position, playable.transform.position);

            if (dist <= skillRadius)
            {
                targetsInRange.Add(playable);
            }
        }

        foreach (var target in targetsInRange)
        {
            target.Heal(skillData.healValue);
            Debug.Log($"{target.name}¿¡°Ô {skillData.healValue} Èú");
        }
    }
}