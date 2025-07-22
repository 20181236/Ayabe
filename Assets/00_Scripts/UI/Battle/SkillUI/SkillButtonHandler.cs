using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillButtonHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public Image iconImage;

    [SerializeField] private Image manaFillImage; //  fill로 채워질 이미지
    [SerializeField] private Button skillButton;   // 클릭 가능 여부 제어

    private SkillData skillData;

    public SkillData SkillData => skillData;

    public Action<SkillId, Vector2> OnSkillDown;
    public Action<SkillId, Vector2> OnSkillUp;
    public Action<SkillId, Vector2> OnSkillDrag;

    void Awake()
    {
        if (iconImage == null)
            iconImage = GetComponentInChildren<Image>();
        if (skillButton == null)
            skillButton = GetComponent<Button>();
    }

    public void SetSkill(SkillData data)
    {
        skillData = data;
        iconImage.sprite = data.icon;

        //  현재 마나 기준으로 초기 fill 반영
        if (ManaManager.instance != null)
        {
            UpdateManaFill(ManaManager.instance.CurrentMana);
        }
    }

    /// <summary>
    /// 현재 마나에 따라 fill 및 버튼 on/off 결정
    /// </summary>
    public void UpdateManaFill(int currentMana)
    {
        if (skillData == null || skillData.manaCost <= 0) return;

        float ratio = Mathf.Clamp01((float)currentMana / skillData.manaCost);
        manaFillImage.fillAmount = ratio;

        // 마나 충분해야 버튼 활성화
        skillButton.interactable = ratio >= 1f;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (skillData == null || !skillButton.interactable) return;
        OnSkillDown?.Invoke(skillData.skillId, eventData.position);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (skillData == null || !skillButton.interactable) return;
        OnSkillUp?.Invoke(skillData.skillId, eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (skillData == null || !skillButton.interactable) return;
        OnSkillDrag?.Invoke(skillData.skillId, eventData.position);
    }
}
