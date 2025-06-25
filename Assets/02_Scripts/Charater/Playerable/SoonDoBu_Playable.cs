using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class SoonDoBuPlayable : PlayableBase
{
    protected override void Skill()
    {
        if (currentTarget == null)
            return;

        isAttacking = true;
        isSkill = true;

        Vector3 spawnPos = transform.position + Vector3.up * 8f + Vector3.forward * 3f;

        GameObject instantMissile = Instantiate(missile, spawnPos, Quaternion.identity);

        Missile missileScript = instantMissile.GetComponent<Missile>();
        missileScript.target = currentTarget.transform;

        skillTimer = 0;
        readySkill = false;
        isSkill = false;
        isAttacking = false;
        currentState = PlayableState.Idle;
    }
}