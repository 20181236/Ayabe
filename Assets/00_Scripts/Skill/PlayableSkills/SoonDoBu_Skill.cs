using UnityEngine;

public class SoonDoBuSkill : SkillBase
{
    public override void Initialize(SkillData data)
    {
        base.Initialize(data);
    }

    public override void Execute(SkillContext context)
    {
        if (context.Target == null)
        {
            Debug.LogWarning("타겟이 없습니다. 스킬 사용 불가");
            return;
        }

        if (context.Caster != null)
        {
            // 공통 이펙트 & 애니메이션 호출
            SpawnCastEffect(context.Caster);
            HandleAnimation(context.Caster);
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
                buffId: BuffID.SoonDuBu_BuffData,         // 실제 버프 ID로 교체
                group: BuffGroup.Heal,              // 적절한 그룹 지정
                category: BuffCategory.Buff,        // 적절한 카테고리 지정
                stat: BuffStatType.HealPower,
                value: tickValue,
                duration: duration,
                applyType: BuffApplyType.Tick,
                tickInterval: interval,
                buffIcon: null                      // 아이콘 있으면 넣기
            );

            CharacterBase casterCharacter = context.Caster.GetComponent<CharacterBase>();
            playable.ApplyBuff(healOverTimeBuff, casterCharacter);

            Debug.Log($"{context.Target.name}에게 즉시 {healPower} 회복 + 5초간 회복력의 80%씩 회복 부여");
        }
        else
        {
            Debug.LogWarning("타겟이 죽었거나 PlayableBase가 없습니다");
        }
    }
}
