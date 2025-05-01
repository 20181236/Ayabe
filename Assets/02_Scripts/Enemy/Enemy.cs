using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;

public class Enemy : Enemy_base
{
    protected override void SetStats()
    {
        maxHealth = (float)EnemyHealth.Thanker;
        attackRange = (float)EnemyAttackRange.Thanker;
        attackInterval = 1.5f;
    }

    protected override void Attack()
    {
        base.Attack();  
    }
}
