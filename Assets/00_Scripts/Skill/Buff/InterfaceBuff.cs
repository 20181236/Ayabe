using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface InterfaceBuff
{
    BuffCategory category { get; }
    BuffApplyType applyType { get; }
    BuffStatType targetStat { get; }
    float value { get; }
    float duration { get; }
    float tickInterval { get; }
    //string BuffId { get; }
}