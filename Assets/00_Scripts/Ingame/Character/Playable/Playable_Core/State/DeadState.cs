using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DeadState : PlayableStateInterface
{
    public void Enter(PlayableBase owner)
    {
        Debug.Log($"{owner.name} enters Dead state.");
    }

    // 죽으면 아무것도 안 함
    public void Update() { }

    // 죽으면 다른 상태로 가지 않음
    public void Exit() { }
}