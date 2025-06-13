using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillSlot : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public SkillData skillData;
    private PlayableBase caster; // 현재 캐릭터 (스킬 주인)
    private SkillBase skillInstance;

    public Image skillIcon;
    public Image cooldownOverlay;

    public float cooldownDuration = 0.3f;  // 쿨타임 총 시간 (초)
    private float cooldownTimer = 0f;

    private bool isCoolingDown = false;
    private bool isDragging = false;


    void Update()
    {
        if (isCoolingDown)
        {
            cooldownTimer -= Time.deltaTime;
            cooldownOverlay.fillAmount = cooldownTimer / cooldownDuration;

            if (cooldownTimer <= 0f)
            {
                isCoolingDown = false;
                cooldownOverlay.fillAmount = 0f;
            }
        }
    }

    public void Setup(SkillData data, PlayableBase caster)
    {
        skillData = data;
        this.caster = caster;
        skillIcon.sprite = data.icon;
        skillInstance = SkillFactory.CreateSkill(data);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isCoolingDown)
        {
            Debug.Log("Skill is cooling down!");
            return;
        }

        Debug.Log("Skill Selected: " + gameObject.name);
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging)
            return;

        Vector2 dragPosition = eventData.position;
        // TODO: 드래그 시 스킬 투사 위치를 표시하는 UI 업데이트 가능
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDragging)
            return;

        isDragging = false;

        if (isCoolingDown)
            return;

        Vector2 releasePosition = eventData.position;
        CastSkill(releasePosition);
    }

    private void CastSkill(Vector2 targetPosition)
    {
        if (skillInstance == null)
        {
            Debug.LogWarning("Skill instance is null.");
            return;
        }

        // Camera.main이 null일 수 있으니 방어코드 권장
        if (Camera.main == null)
        {
            Debug.LogError("Main Camera not found!");
            return;
        }

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

        StartCooldown();
    }

    private void StartCooldown()
    {
        isCoolingDown = true;
        cooldownTimer = cooldownDuration;
        cooldownOverlay.fillAmount = 1f;
    }
}