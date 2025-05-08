using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Enemy_ChaseState : IEnemyState
{
    private Enemy_base enemy;

    public void Enter(Enemy_base enemy)
    {
        this.enemy = enemy;
        enemy.currentTarget = enemy.GetNearestEnemyToPosition(enemy.transform.position);
    }

    public void Update()
    {
        enemy.Targeting();  // 拌加 鸥百 芭府 眉农
    }

    public void FixedUpdate()
    {
        if (enemy.currentTarget != null)
        {
            enemy.MoveToTarget(enemy.currentTarget.transform.position);
        }
    }

    public void Exit()
    {
        if (enemy.navMeshAgent != null)
            enemy.navMeshAgent.isStopped = true;
    }
}
