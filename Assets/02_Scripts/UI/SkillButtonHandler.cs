using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillButtonHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public Image iconImage;  // 스킬 아이콘 표시용
    private SkillData skillData;

    public SkillData SkillData => skillData;  // 읽기 전용 프로퍼티 추가

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
        if (skillData == null) return;
        InputSkill.instance.OnSkillButtonDown(skillData.skillId, eventData.position);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (skillData == null) return;
        InputSkill.instance.OnSkillButtonUp(skillData.skillId, eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (skillData == null) return;
        InputSkill.instance.OnSkillButtonDrag(skillData.skillId, eventData.position);
    }
}
