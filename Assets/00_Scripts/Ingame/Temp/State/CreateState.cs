using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CreateState : PlayableStateInterface
{
    public void Enter(PlayableBase owner)
    {
        // 이 상태에 진입하자마자 필요한 초기화 로직을 여기에 넣을 수 있습니다.
        Debug.Log($"{owner.name}가 생성되었습니다. (Create State)");

        owner.TransitionToState(PlayableBase.PlayableState.Idle);
    }

    public void Update()
    {
        // Create 상태는 한 프레임만에 끝나므로 Update에서 할 일은 없습니다.
    }

    public void Exit()
    {
        // Idle 상태로 넘어가기 직전에 호출됩니다.
    }
}