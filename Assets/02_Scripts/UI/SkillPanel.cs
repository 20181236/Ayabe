using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillPanel : MonoBehaviour
{
    public List<SkillSlot> skillSlots;  // �ν����Ϳ��� ���� 4�� �̻� ������ ��

    private PlayableBase currentPlayable;

    /// <summary>
    /// ���� ������ ĳ������ ��ų���� ���Կ� ���ε�
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
