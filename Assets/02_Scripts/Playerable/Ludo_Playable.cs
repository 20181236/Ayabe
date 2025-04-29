using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ludo_Playable : SoonDoBu_Playable
{
    public GameObject grenadePrefab; 

    protected override void SetStats()
    {
        maxHealth = (float)PlayableHelath.Ludo;
        attackRange = (float)PlayableAttackRenge.Ludo;
        basicSkillCooldown = (float)PlayalbeBaiscSkillCoolTime.Ludo;
    }

    protected override IEnumerator BasicSkill()
    {
        yield return new WaitForSeconds(0.5f); 

        if (currentTarget == null)
            yield break;

        Vector3 spawnPosition = transform.position + Vector3.up * 1.5f;

        GameObject grenadeObject = Instantiate(grenadePrefab, transform.position + Vector3.up * 2f, Quaternion.identity);
        Rigidbody grenadeRigidbody = grenadeObject.GetComponent<Rigidbody>();

        Vector3 toTarget = (currentTarget.transform.position - transform.position).normalized;

        Vector3 force = toTarget * 5f + Vector3.up * 7f;

        grenadeRigidbody.AddForce(force, ForceMode.Impulse);

        Granade grenadeScript = grenadeObject.GetComponent<Granade>();

        if (grenadeScript != null)
        {
            grenadeScript.targetPosition = currentTarget.transform.position;
        }
    }
}