using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillPanel : MonoBehaviour
{
    public List<SkillSlot> skillSlots;  // 인스펙터에서 슬롯 4개 이상 연결할 것

    private PlayableBase currentPlayable;

    /// <summary>
    /// 현재 조작할 캐릭터의 스킬들을 슬롯에 바인딩
    /// </summary>
    public void SetCurrentPlayable(PlayableBase playable)
    {
        currentPlayable = playable;

        int slotIndex = 0;

        if (playable.exSkillData != null && slotIndex < skillSlots.Count)
        {
            skillSlots[slotIndex].Setup(playable.exSkillData, playable);
            skillSlots[slotIndex].gameObject.SetActive(true);
            slotIndex++;
        }
    }
}
