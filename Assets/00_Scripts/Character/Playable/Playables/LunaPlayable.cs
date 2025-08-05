using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LunaPlayable : PlayableBase
{
    [SerializeField] private BuffData attackBuffData;

    protected override void Skill()
    {
        Debug.Log("Luna스킬사용");
        if (attackBuffData == null)
        {
            Debug.Log("Attack Buff Data is not assigned.");
            return;
        }

        isUsingSkill = true;
        skillTimer = 0f;
        readySkill = false;

        StartCoroutine(SkillRoutine());
    }

    private IEnumerator SkillRoutine()
    {
        //// 애니메이션 재생 등 필요 시 처리
        //playableAnimator.SetTrigger("doSkill");

        yield return new WaitForSeconds(0.5f); // 애니메이션 캐스팅 타임 등

        // 아군 전체에게 버프 적용
        foreach (var playables in PlayableManager.instance.playables)
        {
            if (playables != null && !playables.isDead)
            {
                playables.ApplyBuff(attackBuffData);
                Debug.Log($"{playables.name} 버프 적용 후 공격력: {playables.AttackPower}");
            }
        }

        isUsingSkill = false;
    }
}
