using UnityEngine;

public class SoonDoBuSkill : SkillBase
{
    [SerializeField] private BuffData soonDuBuBuffData;
    //[SerializeField] private Sprite soonDuBuBuffIcon;
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
            SpawnCastEffect(context.Caster);
            HandleAnimation(context.Caster, "doExSkill");
        }

        var playable = context.Target.GetComponent<PlayableBase>();

        if (playable != null && !playable.isDead)
        {
            // 1. 즉시 회복: 치유력의 100%를 즉시 적용
            // 버프 시스템을 통하지 않고 직접 Heal() 함수 호출
            playable.Heal(playable.HealPower);

            // 2. 지속 힐 버프: 치유력의 80%로 5초간 지속
            CharacterBase casterCharacter = context.Caster.GetComponent<CharacterBase>();

            if (skillData != null && skillData.buffData != null)
            {   
                // 즉시 힐은 이미 적용했으므로, BuffData의 ApplyType을 'Tick'으로 설정하여 지속 힐만 적용되도록 함
                playable.ApplyBuff(skillData.buffData, casterCharacter);
            }
        }
    }

    //if (playable != null && !playable.isDead)
    //{
    //    // 1) 즉시 회복
    //    float healPower = playable.HealPower;
    //    playable.Heal(healPower);

    //    // 2) 5초 동안 회복력의 80%씩 1초 간격으로 회복
    //    float tickValue = 0.8f;     // 회복력의 80%
    //    float duration = 5f;
    //    float interval = 1f;

    //    BuffData healOverTimeBuff = BuffFactory.CreateRuntimeBuff(
    //        buffId: BuffID.SoonDuBu_BuffData,         // 실제 버프 ID로 교체
    //        group: BuffGroup.Heal,              // 적절한 그룹 지정
    //        category: BuffCategory.Buff,        // 적절한 카테고리 지정
    //        stat: BuffStatType.HealPower,
    //        value: tickValue,
    //        duration: duration,
    //        applyType: BuffApplyType.Tick,
    //        tickInterval: interval,
    //        buffIcon: soonDuBuBuffIcon          // 아이콘 있으면 넣기
    //    );

    //    CharacterBase casterCharacter = context.Caster.GetComponent<CharacterBase>();
    //    playable.ApplyBuff(healOverTimeBuff, casterCharacter);

    //    Debug.Log($"{context.Target.name}에게 즉시 {healPower} 회복 + 5초간 회복력의 80%씩 회복 부여");
    //}
    //else
    //{
    //    Debug.LogWarning("타겟이 죽었거나 PlayableBase가 없습니다");
    //}
}