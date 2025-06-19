using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

//UIÃ³¸®
public class SkillButtonHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public Image iconImage;
    private SkillData skillData;

    public SkillData SkillData => skillData;

    public Action<SkillId, Vector2> OnSkillDown;
    public Action<SkillId, Vector2> OnSkillUp;
    public Action<SkillId, Vector2> OnSkillDrag;

    void Awake()
    {
        if (iconImage == null)
            iconImage = GetComponentInChildren<Image>();
    }

    public void SetSkill(SkillData data)
    {
        skillData = data;
        iconImage.sprite = data.icon;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (skillData == null)
            return;
        OnSkillDown?.Invoke(skillData.skillId, eventData.position);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (skillData == null)
            return;
        OnSkillUp?.Invoke(skillData.skillId, eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (skillData == null)
            return;
        OnSkillDrag?.Invoke(skillData.skillId, eventData.position);
    }
}