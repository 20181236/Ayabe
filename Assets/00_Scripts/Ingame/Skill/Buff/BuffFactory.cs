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
        buff.buffIcon = data.buffIcon; // buffIcon은 Buff 클래스에 public으로 만들어야 함
        return buff;
    }

    public static BuffData CreateRuntimeBuff(
        BuffID buffId,
        BuffGroup group,
        BuffCategory category,
        BuffStatType stat,
        float value,
        float duration,
        BuffApplyType applyType = BuffApplyType.Tick,
        float tickInterval = 1f,
        Sprite buffIcon = null
    )
    {
        var buff = ScriptableObject.CreateInstance<BuffData>();
        buff.SetData(buffId, group, category, stat, value, duration, applyType, tickInterval, buffIcon);
        return buff;
    }

}


//public static class BuffFactory
//{
//    public static Buff CreateBuffFromData(BuffData data)
//    {
//        Buff buff = new Buff();
//        buff.Initialize(data);
//        buff.Initialize(
//            data.buffId,
//            data.group,
//            data.category,
//            data.applyType,
//            data.targetStat,
//            data.value,
//            data.duration,
//            data.tickInterval,
//            data.buffIcon
//        );
//        return buff;
//    }


//    public static BuffData CreateRuntimeBuff(BuffStatType stat, float value, float duration, BuffApplyType applyType = BuffApplyType.Tick, float tickInterval = 1f)
//    {
//        var buff = ScriptableObject.CreateInstance<BuffData>();
//        buff.SetData(stat, value, duration, applyType, tickInterval);
//        return buff;
//    }
//}