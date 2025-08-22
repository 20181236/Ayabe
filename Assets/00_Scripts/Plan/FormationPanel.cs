using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FormationPanel : MonoBehaviour
{
    public Image[] slotIcons;

    [Header("모든 캐릭터 데이터")]
    public List<PlayableData> allPlayableDataList;

    public void OnClickSlot()
    {
        CharacterSelect popup = PlanUIManager.instance.ShowPopup<CharacterSelect>(PopupList.SetPlayablePopup);

        // 모든 캐릭터 데이터 전달
        popup.Init(allPlayableDataList);

        popup.onSelectionConfirmed = (selectedList) =>
        {
            ApplySelection(selectedList);
        };
    }

    public void ApplySelection(List<PlayableData> selected)
    {
        for (int i = 0; i < slotIcons.Length; i++)
        {
            if (i < selected.Count)
            {
                slotIcons[i].sprite = selected[i].playableIcon;
                Debug.Log($"Slot {i}에 {selected[i].playableID} 캐릭터 적용");
            }
            else
            {
                slotIcons[i].sprite = null;
                Debug.Log($"Slot {i} 비어있음");
            }
        }
    }

}


