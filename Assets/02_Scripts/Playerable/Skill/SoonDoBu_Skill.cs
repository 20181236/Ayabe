using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoonDoBuSkill : SkillBase
{
    public float healValue;

    private void Awake()
    {
        if (skillData != null)
        {
            healValue = skillData.healValue;
        }
    }
    public override void SkillExecute(GameObject caster, GameObject target)
    {
        if (target == null)
            return;

        var playable = target.GetComponent<PlayableBase>();
        if (playable != null && !playable.isDead)
        {
            playable.Heal(healValue);
            Debug.Log($"{target.name}에게 {healValue}만큼 힐을 적용했습니다.");
        }
    }

}