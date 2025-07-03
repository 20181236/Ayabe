using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BuffFactory
{
    public static Buff CreateBuffFromData(BuffData data)
    {
        return new Buff(
            //id: data.buffId,
            category: data.category,
            applyType: data.applyType,
            stat: data.targetStat,
            value: data.value,
            duration: data.duration,
            tickInterval: data.tickInterval
        );
    }
    public static BuffData CreateRuntimeBuff(BuffStatType stat, float value, float duration, BuffApplyType applyType = BuffApplyType.Tick, float tickInterval = 1f)
    {
        var buff = ScriptableObject.CreateInstance<BuffData>();
        buff.SetData(stat, value, duration, applyType, tickInterval);
        return buff;
    }
}