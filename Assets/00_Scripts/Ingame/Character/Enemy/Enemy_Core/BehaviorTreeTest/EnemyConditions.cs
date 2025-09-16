
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyConditions
{
    public static bool HasTarget(EnemyBase enemy)
    {
        return enemy.CurrentTarget != null && !enemy.CurrentTarget.isDead;
    }


    public static bool IsTargetInAttackRange(EnemyBase enemy)
    {
        // 타겟이 없으면 사거리 안에 있을 수 없으므로 false를 반환합니다.
        if (!HasTarget(enemy))
        {
            return false;
        }
        return enemy.Distance <= enemy.AttackRange;
    }
}