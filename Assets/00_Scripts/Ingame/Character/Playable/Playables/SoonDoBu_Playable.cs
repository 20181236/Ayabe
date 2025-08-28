using System;
using System.Collections;
using System.Collections.Generic;
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
        //animator.SetBool("isSkill", true); // 스킬 애니메이션을 위한 bool 파라미터 필요

        Vector3 spawnPos = transform.position + Vector3.up * 8f + Vector3.forward * 3f;

        GameObject instantMissile = Instantiate(missile, spawnPos, Quaternion.identity);

        Missile missileScript = instantMissile.GetComponent<Missile>();
        missileScript.target = currentTarget.transform;

        skillTimer = 0;
        readySkill = false;

        isSkill = false;
        isAttacking = false;

        // PlayableState.Idle -> CharacterBase.CharacterState.Idle 로 변경
        currentState = PlayableState.Idle;
    }
}