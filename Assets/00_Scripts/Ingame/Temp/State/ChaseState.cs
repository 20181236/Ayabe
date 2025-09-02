using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : PlayableStateInterface
{
    private PlayableBase owner;

    public void Enter(PlayableBase playable)
    {
        this.owner = playable;

        owner.animator.SetBool("isStandby", false); // 추격 중에는 전투 대기 상태가 아님

        if (owner.navMeshAgent != null && owner.navMeshAgent.enabled)
        {
            owner.navMeshAgent.isStopped = false;
        }
    }

    public void Update()
    {
        if (owner.currentTarget == null || owner.currentTarget.isDead)
        {
            owner.TransitionToState(PlayableBase.PlayableState.Idle);
            return;
        }

        owner.UpdateTargetAndDistance();

        // 공격 범위에 들어왔을 때,
        if (owner.distance <= owner.AttackRange)
        {
            // 금 다른 공격을 하고 있는 중이 아닐 때만 AttackState로 전환합니다.
            if (!owner.isAttacking)
            {
                owner.TransitionToState(PlayableBase.PlayableState.Attack);
            }
        }
        // 공격 범위 밖에 있다면 계속 추적합니다.
        else
        {
            owner.navMeshAgent.SetDestination(owner.currentTarget.transform.position);
            float speedPercent = owner.navMeshAgent.velocity.magnitude / owner.navMeshAgent.speed;
            owner.animator.SetFloat("moveSpeed", Mathf.Clamp(speedPercent, 0.01f, 1f));
        }
    }

    public void Exit()
    {
        if (owner.navMeshAgent != null && owner.navMeshAgent.enabled)
        {
            owner.navMeshAgent.isStopped = true;
        }
        owner.animator.SetFloat("moveSpeed", 0f);
    }
}
