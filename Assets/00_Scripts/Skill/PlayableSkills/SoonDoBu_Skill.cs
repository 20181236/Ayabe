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
            // 1) 즉시 회복
            float healPower = playable.HealPower;
            playable.Heal(healPower);

            // 2) 5초 동안 회복력의 80%씩 1초 간격으로 회복
            float tickValue = 0.8f;     // 회복력의 80%
            float duration = 5f;
            float interval = 1f;

            BuffData healOverTimeBuff = BuffFactory.CreateRuntimeBuff(
                BuffStatType.HealPower,
                tickValue,
                duration,
                BuffApplyType.Tick,
                interval
            );

            playable.ApplyBuff(healOverTimeBuff);

            Debug.Log($"{context.Target.name}에게 즉시 {healPower} 회복 + 5초간 회복력의 80%씩 회복 부여");
        }
        else
        {
            Debug.LogWarning("타겟이 죽었거나 PlayableBase가 없습니다");
        }
    }
}
