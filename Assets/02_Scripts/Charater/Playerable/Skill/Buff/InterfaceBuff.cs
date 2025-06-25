using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuff
{

    float Duration { get; }

    // 버프가 적용될 때 실행
    void OnApply(GameObject target);

    // 버프가 끝나거나 제거될 때 실행
    void OnRemove(GameObject target);

    // 매 프레임 또는 일정 주기로 실행
    void OnUpdate(GameObject target, float deltaTime);
}