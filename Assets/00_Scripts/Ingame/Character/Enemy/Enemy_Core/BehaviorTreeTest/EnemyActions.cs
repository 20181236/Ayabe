using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyActions
{
    public static BehaviorTreeState BasicAttack(EnemyBase enemy)
    {
        // 공격 중이거나 쿨타임이 차지 않았으면 실패 반환
        if (enemy.isAttacking || !enemy.readyBasicAttack)
        {
            return BehaviorTreeState.Failed;
        }

        // 공격을 위해 NavMeshAgent를 멈춤
        if (enemy.navMeshAgent.enabled)
        {
            enemy.navMeshAgent.isStopped = true;
        }

        // 실제 공격 실행
        enemy.ExecuteBasicAttack();
        Debug.Log("총알 발사!"); // 정상적으로 호출되는지 확인용 로그

        return 
            BehaviorTreeState.Success;
    }

    public static BehaviorTreeState ChaseTarget(EnemyBase enemy)
    {
        if (!EnemyConditions.HasTarget(enemy))
        {
            // 타겟을 잃었을 때 경로 초기화
            if (enemy.navMeshAgent.enabled && enemy.navMeshAgent.hasPath)
                enemy.navMeshAgent.ResetPath();
            return BehaviorTreeState.Failed;
        }

        // 타겟을 향해 이동
        enemy.MoveToTarget(enemy.CurrentTarget.transform.position);

        // 행동이 계속 진행 중임을 의미하는 Running 상태 반환
        return BehaviorTreeState.Running;
    }

    public static BehaviorTreeState Standby(EnemyBase enemy)
    {
        // 이동을 멈춤
        if (enemy.navMeshAgent.enabled && !enemy.navMeshAgent.isStopped)
        {
            enemy.navMeshAgent.isStopped = true;
        }
        return BehaviorTreeState.Success;
    }

    public static BehaviorTreeState Idle(EnemyBase enemy)
    {
        // 경로가 있다면 초기화
        if (enemy.navMeshAgent.enabled && enemy.navMeshAgent.hasPath)
            enemy.navMeshAgent.ResetPath();
        return BehaviorTreeState.Success;
    }
}