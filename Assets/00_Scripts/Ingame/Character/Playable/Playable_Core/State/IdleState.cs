using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : PlayableStateInterface
{
    private PlayableBase owner;

    public void Enter(PlayableBase playable)
    {
        this.owner = playable;

        owner.animator.SetFloat("moveSpeed", 0f);
        owner.animator.SetBool("isStandby", false); // Idle 상태는 전투 대기 상태가 아님

        if (owner.navMeshAgent != null && owner.navMeshAgent.enabled)
        {
            owner.navMeshAgent.isStopped = true;
        }
    }

    public void Update()
    {
        owner.UpdateTargetAndDistance();

        if (owner.currentTarget != null && !owner.isAttacking)
        {
            // 적이 사거리 안 -> Standby 상태로
            if (owner.distance <= owner.AttackRange)
            {
                owner.TransitionToState(PlayableBase.PlayableState.Standby);
            }
            // 적이 사거리 밖 -> Chasing 상태로
            else
            {
                owner.TransitionToState(PlayableBase.PlayableState.Chasing);
            }
        }
        var playable = (PlayableBase)owner;
        playable.TryExecuteSkill(); // Idle 상태에서 주기적으로 스킬 시도
    }

    public void Exit() { }
}
