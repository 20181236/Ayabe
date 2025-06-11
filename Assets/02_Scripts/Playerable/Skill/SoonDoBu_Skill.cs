using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoonDoBuSkill : SkillBase
{
        public override void SkillExecute(GameObject caster, GameObject target)
        {
            if (target == null) return;

            var playable = target.GetComponent<PlayableBase>();
            if (playable != null && !playable.isDead)
            {
                playable.Heal(skillData.healValue);
                Debug.Log($"{target.name}에게 {skillData.healValue}만큼 힐을 적용했습니다.");
            }
        }
    }