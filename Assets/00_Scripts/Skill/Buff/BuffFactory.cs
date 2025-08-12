using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BuffFactory
{
    public static Buff CreateBuffFromData(BuffData data)
    {
        Buff buff = new Buff();
        buff.Initialize(
            data.buffId,
            data.group,
            data.category,
            data.applyType,
            data.targetStat,
            data.value,
            data.duration,
            data.tickInterval
        );
        return buff;
    }


    public static BuffData CreateRuntimeBuff(BuffStatType stat, float value, float duration, BuffApplyType applyType = BuffApplyType.Tick, float tickInterval = 1f)
    {
        var buff = ScriptableObject.CreateInstance<BuffData>();
        buff.SetData(stat, value, duration, applyType, tickInterval);
        return buff;
    }
}