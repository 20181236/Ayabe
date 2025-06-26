using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface InterfaceBuff
{

    float Duration { get; }

    // 이 버프가 영향을 주는 스탯 종류 (MaxHealth, AttackPower 등)
    BuffStatType StatType { get; }

    // 스탯에 얼마나 영향을 주는지 (ex: +10, -5)
    float StatValue { get; }

    void OnApply(GameObject target);    // 이펙트, 사운드 등
    void OnRemove(GameObject target);   // 이펙트 제거, 상태 초기화 등
    void OnUpdate(GameObject target, float deltaTime); // 지속 피해, 점진적 효과 등
}