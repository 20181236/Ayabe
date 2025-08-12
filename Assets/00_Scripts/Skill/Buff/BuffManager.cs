using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class BuffManager : MonoBehaviour
{
    public event Action<BuffData> OnBuffAdded;
    public event Action<BuffData> OnBuffRemoved;

    public void ApplyBuff(BuffData data, PlayableBase owner)
    {
        // Buff 생성
        Buff buff = BuffFactory.CreateBuffFromData(data);
        buff.SetOwner(owner);

        // PlayableBase에 버프 적용 요청
        owner.ApplyBuff(data);

        // UI 이벤트 호출
        OnBuffAdded?.Invoke(data);
        Debug.Log($"[BuffManager] OnBuffAdded 호출: {data.group}");

        // 버프 로직 실행
        StartCoroutine(RunBuffCoroutine(buff));

        // 일정 시간 뒤 버프 제거
        StartCoroutine(RemoveBuffAfterDuration(data, data.duration));
    }

    private IEnumerator RunBuffCoroutine(Buff buff)
    {
        // 즉시 효과
        if (buff.applyType == BuffApplyType.Burst || buff.applyType == BuffApplyType.Both)
        {
            ApplyBuffEffect(buff);
        }

        // 지속 틱 효과
        if (buff.applyType == BuffApplyType.Tick || buff.applyType == BuffApplyType.Both)
        {
            float elapsed = 0f;
            while (elapsed < buff.duration)
            {
                ApplyBuffEffect(buff);
                yield return new WaitForSeconds(buff.tickInterval);
                elapsed += buff.tickInterval;
            }
        }
    }

    private void ApplyBuffEffect(Buff buff)
    {
        if (buff.targetStat == BuffStatType.HealPower)
        {
            float healAmount = buff.owner.baseHealPower * buff.value;
            buff.owner.Heal(healAmount);
        }
        // 다른 스탯 효과도 여기서 처리 가능
    }

    private IEnumerator RemoveBuffAfterDuration(BuffData data, float duration)
    {
        yield return new WaitForSeconds(duration);
        OnBuffRemoved?.Invoke(data);
    }
}
//public void ApplyBuff(Buff buff, System.Action<Buff> onTick)
//{
//if (buff.applyType == BuffApplyType.Tick && buff.tickInterval > 0f)
//{
//    StartCoroutine(BuffTickCoroutine(buff, onTick));
//}
//else
//{
//    //즉시 적용하거나 duration이 없는 버프 처리
//    onTick?.Invoke(buff);
//}
//}

//private IEnumerator BuffTickCoroutine(Buff buff, System.Action<Buff> onTick)
//{
//    float elapsed = 0f;
//    while (elapsed < buff.duration)
//    {
//        onTick?.Invoke(buff);
//        yield return new WaitForSeconds(buff.tickInterval);
//        elapsed += buff.tickInterval;
//    }
