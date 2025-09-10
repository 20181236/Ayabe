using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class SoonDoBuPlayable : PlayableBase
{
    [Header("Skill Buff")]
    [SerializeField] private BuffData attackRangeBuffData; // 공격 사거리 버프 데이터로 변경

    protected override void Skill()
    {
        // 공격 사거리 버프 데이터가 할당되었는지 확인
        if (attackRangeBuffData == null)
        {
            Debug.LogWarning("SoonDoBuPlayable's Attack Range Buff Data is not assigned.");
            return;
        }

        // 스킬 사용 중이면 중복 실행 방지
        if (isUsingSkill)
            return;

        isUsingSkill = true;
        skillTimer = 0f;
        readySkill = false;

        StartCoroutine(SkillRoutine());
    }

    private IEnumerator SkillRoutine()
    {
        // 스킬 시전 애니메이션
        // animator.SetTrigger("doSkill");

        // 0.5초 대기
        yield return new WaitForSeconds(0.5f); 

        Debug.Log($"[{this.name}]이(가) 아군 전체에게 공격 사거리 버프 스킬을 시전합니다!");

        // 모든 아군을 대상으로 버프 적용
        foreach (var playable in PlayableManager.instance.playables)
        {
            if (playable != null && !playable.isDead)
            {
                // 공격 사거리 버프 적용
                playable.ApplyBuff(attackRangeBuffData, this);
            }
        }
        
        isUsingSkill = false; 
    }
}