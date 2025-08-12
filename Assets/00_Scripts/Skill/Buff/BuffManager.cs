using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class BuffManager : MonoBehaviour
{
    public event Action<Buff> OnBuffAdded;
    public event Action<Buff> OnBuffRemoved;

    private List<Buff> activeBuffs = new List<Buff>();

    public void ApplyBuff(BuffData data, CharacterBase owner, CharacterBase caster)
    {
        // 같은 버프가 이미 owner에게 있는지 체크 (필요시)
        Buff existingBuff = activeBuffs.Find(b => b.owner == owner && b.buffId == data.buffId);
        if (existingBuff != null)
        {
            // 기존 버프 갱신
            existingBuff.duration = data.duration;
            existingBuff.value = data.value;
            Debug.Log($"버프 갱신: {existingBuff.buffId} on {owner.name}");
        }
        else
        {
            Buff buff = BuffFactory.CreateBuffFromData(data);
            buff.SetOwner(owner);
            buff.caster = caster;

            activeBuffs.Add(buff);

            owner.ApplyBuff(data, caster);

            OnBuffAdded?.Invoke(buff);

            StartCoroutine(RemoveBuffAfterDuration(buff));
            Debug.Log($"{caster.name}이(가) {owner.name}에게 버프 적용: {buff.targetStat} / 값: {buff.value} / 타입: {buff.applyType}");
        }
    }

    private IEnumerator RemoveBuffAfterDuration(Buff buff)
    {
        yield return new WaitForSeconds(buff.duration);
        activeBuffs.Remove(buff);
        OnBuffRemoved?.Invoke(buff);
        buff.owner.RecalculateBuffedStats();
        Debug.Log($"버프 종료: {buff.buffId} on {buff.owner.name}");
    }

    public CharacterBase GetOwnerOfBuff(Buff buff)
    {
        return buff.owner;
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
