using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// LunaPlayable.cs
using System.Collections;
using UnityEngine;

public class LunaPlayable : PlayableBase
{
    [SerializeField] private BuffData attackBuffData;

    protected override void Skill()
    {
        if (attackBuffData == null)
        {
            Debug.LogWarning("Attack Buff Data is not assigned.");
            return;
        }

        if (isUsingSkill)
            return;

        Debug.Log($"{name} Skill() 호출됨");

        isUsingSkill = true;
        skillTimer = 0f;
        readySkill = false;

        StartCoroutine(SkillRoutine());
    }

    private IEnumerator SkillRoutine()
    {
        // 필요하면 애니메이션 트리거
        // animator.SetTrigger("doSkill");

        Debug.Log($"{name} 스킬 시전 대기 시작");
        yield return new WaitForSeconds(0.5f);

        // 아군 전체에게 버프 적용
        foreach (var playable in PlayableManager.instance.playables)
        {
            if (playable != null && !playable.isDead)
            {
                Debug.Log($"버프 적용 대상: {playable.name}");
                playable.ApplyBuff(attackBuffData, this);
            }
        }

        isUsingSkill = false;
        Debug.Log($"{name} 스킬 종료");
    }

    // 상태 패턴용: IdleState 또는 AttackState에서 주기적으로 호출
    public void TryExecuteSkill()
    {
        if (readySkill && !isUsingSkill)
        {
            ExecuteAttackAction(); // 내부에서 Skill() 호출
        }
    }
}



//public class LunaPlayable : PlayableBase
//{
//    [SerializeField] private BuffData attackBuffData;

//    protected override void Skill()
//    {
//        Debug.Log("Luna스킬사용");
//        if (attackBuffData == null)
//        {
//            Debug.Log("Attack Buff Data is not assigned.");
//            return;
//        }

//        isUsingSkill = true;
//        skillTimer = 0f;
//        readySkill = false;

//        StartCoroutine(SkillRoutine());
//    }

//    private IEnumerator SkillRoutine()
//    {
//        //// 애니메이션 재생 등 필요 시 처리
//        //playableAnimator.SetTrigger("doSkill");

//        yield return new WaitForSeconds(0.5f); // 애니메이션 캐스팅 타임 등

//        // 아군 전체에게 버프 적용
//        foreach (var playables in PlayableManager.instance.playables)
//        {
//            if (playables != null && !playables.isDead)
//            {
//                playables.ApplyBuff(attackBuffData);
//                Debug.Log($"{playables.name} 버프 적용 후 공격력: {playables.AttackPower}");
//            }
//        }

//        isUsingSkill = false;
//    }
//}
