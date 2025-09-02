using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : PlayableStateInterface
{
    public void Enter(PlayableBase owner)
    {
        // 1. 상태에 진입하자마자 공격을 실행합니다.
        owner.ExecuteAttackAction();

        // 2. 공격 실행 후, 다음 프레임에 곧바로 Standby 상태로 돌아가 대기합니다.
        owner.TransitionToState(PlayableBase.PlayableState.Standby);
    }

    public void Update()
    {
        // Enter에서 모든 것을 처리하므로 Update는 비워둡니다.
    }

    public void Exit() { }
}