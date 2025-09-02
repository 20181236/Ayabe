using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandbyState : PlayableStateInterface
{
    private PlayableBase owner;

    public void Enter(PlayableBase playable)
    {
        this.owner = playable;
        owner.animator.SetFloat("moveSpeed", 0f);
        owner.animator.SetBool("isStandby", true);
        if (owner.navMeshAgent != null && owner.navMeshAgent.enabled)
        {
            owner.navMeshAgent.isStopped = true;
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

        if (owner.distance > owner.AttackRange && !owner.isAttacking)
        {
            owner.TransitionToState(PlayableBase.PlayableState.Chasing);
            return;
        }

        Vector3 direction = (owner.currentTarget.transform.position - owner.transform.position).normalized;
        owner.transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        // 공격 준비가 되면,
        if (owner.readyBasicAttack && !owner.isAttacking)
        {
            // Attack 상태로 전환하는 대신, 직접 공격을 실행합니다.
            // owner.TransitionToState(PlayableBase.PlayableState.Attack); // <<-- 이 줄을 아래처럼 변경
            owner.ExecuteAttackAction();
        }
    }

    public void Exit()
    {
        // Standby 상태를 떠날 때 isStandby 플래그를 꺼줍니다.
        owner.animator.SetBool("isStandby", false);
    }
}