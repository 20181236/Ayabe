using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LudoPlayable : PlayableBase
{
    public GameObject grenadePrefab;

    protected override void Skill()
    {
        if (currentTarget == null || currentTarget.isDead)
            return;

        if (currentTarget.ObjectType == this.ObjectType)
            return;
        readySkill = false;
        skillTimer = 0;

        isAttacking = true;
        isSkill = true;

        isAttacking = true;
        isSkill = true;

        GameObject grenadeObject = Instantiate(
            grenadePrefab,
            transform.position + Vector3.up * 8f + Vector3.forward * 3f,
            Quaternion.identity);

        Rigidbody grenadeRigidbody = grenadeObject.GetComponent<Rigidbody>();
        Vector3 toTarget = (currentTarget.transform.position - transform.position).normalized;
        Vector3 force = toTarget * 8f + Vector3.up * 7f;
        grenadeRigidbody.AddForce(force, ForceMode.Impulse);

        Granade grenadeScript = grenadeObject.GetComponent<Granade>();
        if (grenadeScript != null)
        {
            grenadeScript.targetPosition = currentTarget.transform.position;
        }

        skillTimer = 0;
        readySkill = false;
        isSkill = false;
        isAttacking = false;
        currentState = PlayableState.Idle;
    }

}