using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class BuffManager : MonoBehaviour
{
    public event Action<BuffData> OnBuffAdded;
    public event Action<BuffData> OnBuffRemoved;

    private Dictionary<BuffData, Buff> buffDataToBuff = new Dictionary<BuffData, Buff>();

    public void ApplyBuff(BuffData data, CharacterBase owner)
    {
        if (buffDataToBuff.ContainsKey(data))
        {
            // 기존 버프가 있으면 갱신
            Buff existingBuff = buffDataToBuff[data];
            existingBuff.duration = data.duration;
            existingBuff.value = data.value;
        }
        else
        {
            // 새 버프 추가
            Buff buff = BuffFactory.CreateBuffFromData(data);
            buff.SetOwner(owner);
            buffDataToBuff[data] = buff;

            owner.ApplyBuff(data);

            OnBuffAdded?.Invoke(data);

            StartCoroutine(RemoveBuffAfterDuration(data, data.duration));
        }
    }


    // 버프 소유자 반환
    public CharacterBase GetOwnerOfBuff(BuffData data)
    {
        if (buffDataToBuff.TryGetValue(data, out Buff buff))
            return buff.owner;
        return null;
    }

    //private IEnumerator RunBuffCoroutine(Buff buff)
    //{
    //    // 즉시 효과
    //    if (buff.applyType == BuffApplyType.Burst || buff.applyType == BuffApplyType.Both)
    //    {
    //        ApplyBuffEffect(buff);
    //    }

    //    // 지속 틱 효과
    //    if (buff.applyType == BuffApplyType.Tick || buff.applyType == BuffApplyType.Both)
    //    {
    //        float elapsed = 0f;
    //        while (elapsed < buff.duration)
    //        {
    //            ApplyBuffEffect(buff);
    //            yield return new WaitForSeconds(buff.tickInterval);
    //            elapsed += buff.tickInterval;
    //        }
    //    }
    //}

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
        buffDataToBuff.Remove(data);
        OnBuffRemoved?.Invoke(data);
    }

    //private IEnumerator RemoveBuffAfterDuration(BuffData data, float duration)
    //{
    //    yield return new WaitForSeconds(duration);
    //    OnBuffRemoved?.Invoke(data);
    //}
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
