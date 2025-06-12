using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISkillSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image skillIcon;

    private SkillData skillData;
    public Action<SkillData> OnSkillSlotClicked;

    public void SetupSlot(SkillData data)
    {
        skillData = data;
        if (skillIcon != null)
            skillIcon.sprite = skillData.icon;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnSkillSlotClicked?.Invoke(skillData);
    }
}