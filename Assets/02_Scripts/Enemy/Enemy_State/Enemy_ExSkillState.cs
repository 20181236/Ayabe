using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Enemy_ExSkillState : IEnemyState
{
    private Enemy_base enemy;

    public void Enter(Enemy_base enemy)
    {
        this.enemy = enemy;
        enemy.ExSkill();
        enemy.ChangeState(enemy.enemy_AttackState); // ExSkill 후 공격상태로 복귀
    }

    public void Update() { }
    public void FixedUpdate() { }
    public void Exit() { }
}
