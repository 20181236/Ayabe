using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyActions
{
    /// <summary>
    /// 기본 공격을 수행합니다.
    /// </summary>
    public static BehaviorTreeState BasicAttack(EnemyBase enemy)
    {
        Debug.Log("BT Decision: BasicAttack");
        // 이미 다른 공격 중이거나, 공격 쿨타임이 차지 않았으면 행동 실패
        if (enemy.isAttacking || !enemy.readyBasicAttack)
        {
            return BehaviorTreeState.Failed;
        }

        // EnemyBase에 있는 실제 공격 메서드를 호출합니다.
        enemy.ExecuteBasicAttack();

        // 공격은 즉시 끝나는 행동으로 간주하고 성공을 반환합니다.
        return BehaviorTreeState.Success;
    }

    /// <summary>
    /// 타겟을 향해 이동(추격)합니다.
    /// </summary>
    public static BehaviorTreeState ChaseTarget(EnemyBase enemy)
    {
        Debug.Log("BT Decision: ChaseTarget");
        // 추격할 타겟이 없으면 실패
        if (!EnemyConditions.HasTarget(enemy))
        {
            // 혹시 이동 중이었다면 멈추게 합니다.
            if (enemy.navMeshAgent.enabled && enemy.navMeshAgent.hasPath)
                enemy.navMeshAgent.ResetPath();
            enemy.animator.SetBool("isChase", false);
            return BehaviorTreeState.Failed;
        }

        // EnemyBase의 이동 메서드를 호출합니다.
        enemy.MoveToTarget(enemy.CurrentTarget.transform.position);
        enemy.animator.SetBool("isChase", true);

        // 이동은 한 프레임에 끝나지 않으므로 '실행 중' 상태를 반환합니다.
        return BehaviorTreeState.Running;
    }

    public static BehaviorTreeState Standby(EnemyBase enemy)
    {
        // 이동 중이었다면 멈춥니다. isStopped를 true로 하면 현재 경로를 기억한 채로 멈춥니다.
        if (enemy.navMeshAgent.enabled && !enemy.navMeshAgent.isStopped)
        {
            enemy.navMeshAgent.isStopped = true;
        }

        // 추격 애니메이션은 끕니다. (대기 또는 전투 대기 애니메이션으로 전환)
        enemy.animator.SetBool("isChase", false);

        // 대기는 항상 성공하는 행동입니다.
        return BehaviorTreeState.Success;
    }

    /// <summary>
    /// 아무것도 하지 않고 대기합니다.
    /// </summary>
    public static BehaviorTreeState Idle(EnemyBase enemy)
    {
        Debug.Log("BT Decision: Idle");
        // 이동 중이었다면 멈춥니다.
        if (enemy.navMeshAgent.enabled && enemy.navMeshAgent.hasPath)
            enemy.navMeshAgent.ResetPath();

        enemy.animator.SetBool("isChase", false);

        // 대기는 즉시 성공하는 행동입니다.
        return BehaviorTreeState.Success;
    }
}