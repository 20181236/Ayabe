using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Enemy_DeadState : IEnemyState
{
    private Enemy_base enemy;

    public void Enter(Enemy_base enemy)
    {
        this.enemy = enemy;
        enemy.Die();
    }

    public void Update() { }
    public void FixedUpdate()
    {
        enemy.rigidbodyEnemy.velocity = Vector3.zero;
    }

    public void Exit() { }
}
