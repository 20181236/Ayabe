using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff : MonoBehaviour
{
    public BuffStatType buffType;
    public float value;
    public float duration;
    public float timeRemaining;

    public Buff(BuffStatType type, float value, float duration)
    {
        this.buffType = type;
        this.value = value;
        this.duration = duration;
        this.timeRemaining = duration;
    }
}
