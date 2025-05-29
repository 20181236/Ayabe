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
        // 목표 방향 계산
        Vector3 directionToTarget = (currentTarget.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(new Vector3(directionToTarget.x, 0, directionToTarget.z));

        // 미사일 인스턴스 생성
        GameObject instantMissile = Instantiate(
            missile,
            transform.position + Vector3.up * 5f,  // 미사일의 생성 위치
            Quaternion.LookRotation(directionToTarget)
        );

        Missile missileScript = instantMissile.GetComponent<Missile>();
        missileScript.target = currentTarget.transform;

        // 미사일에 속도 적용 (Rigidbody가 필요)
        Rigidbody missileRigidbody = instantMissile.GetComponent<Rigidbody>();
        if (missileRigidbody != null)
        {
            missileRigidbody.velocity = directionToTarget * 20f;  // missileSpeed는 미사일의 속도
        }
        skillTimer = 0;
        readySkill = false;
        isSkill = false;
        isAttacking=false;
    }
}