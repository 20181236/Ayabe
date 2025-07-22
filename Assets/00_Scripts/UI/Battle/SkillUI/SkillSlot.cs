using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//드래그하여 시전하는 스킬 처리
public class SkillSlot : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{

    public SkillData skillData;
    private PlayableBase caster;
    private SkillBase skillInstance;

    public Image skillIcon;

    private bool isDragging = false;

    public void Setup(SkillData data, PlayableBase caster)
    {
        skillData = data;
        this.caster = caster;
        skillIcon.sprite = data.icon;
        skillInstance = SkillFactory.CreateSkill(data);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //if(RectTransformUtility.ScreenPointToLocalPointInRectangle(이미지.rectTransform, eventData.position,eventData.pressEventCamera, out Vector2 localPoint))

        if (!isDragging)
            return;

        Vector2 dragPosition = eventData.position;
        transform.position = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDragging)
            return;

        isDragging = false;

        Vector2 releasePosition = eventData.position;
        CastSkill(releasePosition);
    }

    private void CastSkill(Vector2 targetPosition)
    {
        if (caster == null || skillInstance == null || Camera.main == null)
            return;
        Debug.Log($"[CastSkill] 스킬 시도: {skillData.skillId}, 마나코스트: {skillData.manaCost}");
        // 마나 검사
        if (!ManaManager.instance.CanUseMana(skillData.manaCost))
        {
            Debug.Log("[CastSkill] 마나 부족으로 스킬 시전 실패");
            return;
        }
        Debug.Log("[CastSkill] 마나 소모 성공, 스킬 실행");

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(targetPosition.x, targetPosition.y, 10f));

        Collider[] hits = Physics.OverlapSphere(worldPosition, skillData.skillRadius);
        GameObject closest = null;
        float minDist = float.MaxValue;

        foreach (var hit in hits)
        {
            float dist = Vector3.Distance(worldPosition, hit.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = hit.gameObject;
            }
        }

        SkillContext context = new SkillContext
        {
            Caster = this.caster.gameObject,
            Target = closest,
            TargetPosition = worldPosition
        };

        skillInstance.Execute(context);
        ManaManager.instance.UseMana(skillData.manaCost); // 확실하게 사용
    }
}