using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour
{
    private SkillData skillData;

    public AreaType effectType;  // 딜인지 힐인지
    public float effectAmount; // 데미지 or 힐량
    public float effectInterval; //몇 초마다 효과 발동?
    public float areaDuration;        // 장판 지속 시간 (초)

    private float tickTimer = 0f;

    private List<GameObject> targets = new List<GameObject>();

    public void SetArea(SkillData data)
    {
        skillData = data;
        effectType = skillData.areaType;
        effectAmount = skillData.effectAmount;
        effectInterval = skillData.effectInterval;
        areaDuration = skillData.areaDuration;
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }
    private void OnTriggerExit(Collider other)
    {
        
    }
}
