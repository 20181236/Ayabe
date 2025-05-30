using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Luna_Playable : PlayableBase
{
    protected override void Skill()
    {
        if (currentTarget == null)
            return;

        isAttacking = true;
        isSkill = true;
        Vector3 directionToTarget = (currentTarget.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(new Vector3(directionToTarget.x, 0, directionToTarget.z));
        GameObject instantMissile = Instantiate(
            missile,
            transform.position + Vector3.up * 8f + Vector3.forward * 3f,
        Quaternion.LookRotation(directionToTarget)
        );
        Missile missileScript = instantMissile.GetComponent<Missile>();
        missileScript.target = currentTarget.transform;
        Rigidbody missileRigidbody = instantMissile.GetComponent<Rigidbody>();
        if (missileRigidbody != null)
        {
            missileRigidbody.velocity = directionToTarget * 20f;
        }
        skillTimer = 0;
        readySkill = false;
        isSkill = false;
        isAttacking = false;
        currentState = PlayableState.Idle;
    }
}
