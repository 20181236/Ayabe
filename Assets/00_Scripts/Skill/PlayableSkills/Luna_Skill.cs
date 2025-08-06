using System.Collections;
using UnityEngine;

public class LunaSkill : SkillBase
{
    private float skillRadius;

    public override void Initialize(SkillData data)
    {
        base.Initialize(data);
        skillRadius = data.skillRadius;
    }

    public override void Execute(SkillContext context)
    {
        if (context.Caster == null)
        {
            Debug.LogError("[LunaSkill] 실행 실패: Caster가 null입니다.");
            return;
        }

        Debug.Log($"[LunaSkill] 캐스터 이름: {context.Caster.name}");
        Debug.Log($"[LunaSkill] 캐스터 위치: {context.Caster.transform.position}");
        Debug.Log($"[LunaSkill] 타겟 위치: {context.TargetPosition}");

        // 공통 이펙트와 애니메이션 호출
        SpawnCastEffect(context.Caster);
        HandleAnimation(context.Caster);

        // 힐 범위 내 아군 힐
        Vector3 center = context.TargetPosition;
        Collider[] allies = Physics.OverlapSphere(center, skillRadius, LayerMask.GetMask("Playable"));

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

        Debug.Log($"광역 힐 실행: {healedCount}명에게 {skillData.healValue} 힐 완료");
    }
}
