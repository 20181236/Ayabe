using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff
{
    //public string buffId;
    public BuffCategory category;
    public BuffApplyType applyType;
    public BuffStatType targetStat;
    public float value;
    public float duration;
    public float tickInterval;

    private float elapsedTime = 0f;
    private float tickTimer = 0f;

    public Buff(BuffCategory category, BuffApplyType applyType, BuffStatType stat, float value, float duration, float tickInterval = 0f)
    {
        //buffId = id;
        this.category = category;
        this.applyType = applyType;
        targetStat = stat;
        this.value = value;
        this.duration = duration;
        this.tickInterval = tickInterval;
    }

    public bool TickUpdate(float deltaTime, System.Action<Buff> onTick)
    {
        elapsedTime += deltaTime;

        if (applyType == BuffApplyType.Tick)
        {
            tickTimer += deltaTime;
            if (tickTimer >= tickInterval)
            {
                tickTimer = 0f;
                onTick?.Invoke(this);
            }
        }

        return elapsedTime >= duration;
    }
}
