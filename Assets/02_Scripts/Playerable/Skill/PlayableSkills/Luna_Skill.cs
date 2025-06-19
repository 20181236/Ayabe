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
        // context.TargetPosition: ±¤¿ª Èú ¹üÀ§ Áß½É À§Ä¡
        Vector3 center = context.TargetPosition;

        // Èú ¹üÀ§ ³» ¾Æ±º Å½»ö (layer ¼³Á¤ ÇÊ¿ä)
        Collider[] allies = Physics.OverlapSphere(center, skillData.skillRadius, LayerMask.GetMask("Playable"));

        int healedCount = 0;

        foreach (Collider allyCollider in allies)
        {
            var playable = allyCollider.GetComponent<PlayableBase>();
            if (playable != null && !playable.isDead)
            {
                playable.Heal(skillData.healValue);
                healedCount++;
            }
        }

        Debug.Log($"±¤¿ª Èú ½ÇÇà: {healedCount}¸í¿¡°Ô {skillData.healValue} Èú ¿Ï·á");
    }
}
