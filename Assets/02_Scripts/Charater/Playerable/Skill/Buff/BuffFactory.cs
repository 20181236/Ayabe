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
}