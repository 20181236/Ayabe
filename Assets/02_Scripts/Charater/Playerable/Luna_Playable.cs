using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Luna_Playable : PlayableBase
{
    public InterfaceBuff attackPowerBuff;
    protected override void Skill()
    {
        if (attackPowerBuff == null)
        {
            Debug.LogWarning("공격력 버프가 설정되지 않았습니다.");
            return;
        }

        Debug.Log("루나 스킬 발동! 아군 전원에게 공격력 버프 적용");

        foreach (var playables in PlayableManager.instance.playables)
        {
            if (playables != null && !playables.isDead)
            {
                playables.AddBuffStat()
            }
        }

        // 스킬 쿨타임 초기화 등
        skillTimer = 0;
        readySkill = false;
    }
}
