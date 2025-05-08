using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Enemy_SkillState : IEnemyState
{
    private Enemy_base enemy;

    public void Enter(Enemy_base enemy)
    {
        this.enemy = enemy;
        enemy.Skill();
        enemy.ChangeState(enemy.enemy_IdleState);
    }

    public void Update() { }
    public void FixedUpdate() { }
    public void Exit() { }
}
