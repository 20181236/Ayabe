using UnityEngine;
using UnityEngine.UI;

public class FormationSlot : MonoBehaviour
{
    public Image slotIcon;
    public int slotIndex;
    private PlayableData assignedData;

    public void SetData(PlayableData data)
    {
        assignedData = data;
        if (slotIcon != null)
            slotIcon.sprite = data != null ? data.playableIcon : null;
    }

    public PlayableData GetData()
    {
        return assignedData;
    }
}
