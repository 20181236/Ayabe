using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff : MonoBehaviour
{
    //지금당장은 필요없는 클래스가 맞음 근데 나중에 CSV나 테이블 들어오면 필요할지도
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
