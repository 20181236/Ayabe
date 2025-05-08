using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Enemy_AttackState : IEnemyState
{
    private Enemy_base enemy;

    public void Enter(Enemy_base enemy)
    {
        this.enemy = enemy;
        enemy.animator.SetBool("isAttack", true);
    }

    public void Update()
    {
        if (!enemy.readyAttack || enemy.currentTarget == null || enemy.currentTarget.isDead)
        {
            enemy.ChangeState(enemy.enemy_IdleState);
            return;
        }

        enemy.ShootBulletAtTarget();
        enemy.readyAttack = false;
        enemy.attackTimer = 0f;
        enemy.attackCount++;

        enemy.animator.SetBool("isAttack", false);

        if (enemy.attackCount > 5)
        {
            enemy.attackCount = 0;
            enemy.ChangeState(enemy.enemy_SkillState);
        }
        else if (enemy.readyExSkillActive)
        {
            enemy.readyExSkillActive = false;
            enemy.ChangeState(enemy.enemy_ExSkillState);
        }
        else
        {
            enemy.ChangeState(enemy.enemy_IdleState);
        }
    }

    public void FixedUpdate() { }

    public void Exit() { }
}
