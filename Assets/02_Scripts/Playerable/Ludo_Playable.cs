using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ludo_Playable : PlayableBase
{
    public GameObject grenadePrefab;

    protected override void Skill()
    {
        if (currentTarget == null)
            return;
        isAttacking = true;
        isSkill = true;
        Vector3 spawnPosition = transform.position + Vector3.up * 3f;
        GameObject grenadeObject = Instantiate(
            grenadePrefab,
            transform.position + Vector3.up * 8f + Vector3.forward * 3f,
            Quaternion.identity);
        Rigidbody grenadeRigidbody = grenadeObject.GetComponent<Rigidbody>();
        Vector3 toTarget = (currentTarget.transform.position - transform.position).normalized;//방향주고
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