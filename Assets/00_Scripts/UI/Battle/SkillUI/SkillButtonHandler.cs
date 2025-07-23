using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//UI처리
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
        if (data == null)
        {
            Debug.LogWarning("[SkillButtonHandler] SetSkill에 null이 전달되었습니다.");
            return;
        }

        if (iconImage == null)
        {
            Debug.LogError($"[SkillButtonHandler] {data.skillId} - iconImage가 null입니다!");
        }

        if (data.icon == null)
        {
            Debug.LogError($"[SkillButtonHandler] {data.skillId} - data.icon이 null입니다!");
        }

        skillData = data;
        iconImage.sprite = data.icon;

        Debug.Log($"[SkillButtonHandler] {data.skillId} 세팅 완료, 아이콘: {data.icon.name}");
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