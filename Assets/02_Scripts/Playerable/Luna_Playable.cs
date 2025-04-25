using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Luna_Playable : SoonDoBu_Playable
{

    protected override IEnumerator BasicSkill()
    {
        // Custom implementation of BasicSkill
        yield return new WaitForSeconds(0.5f);  // Example of modifying the delay

        if (currentTarget == null)
            yield break;

        // Custom logic for the advanced skill
        Vector3 directionToTarget = (currentTarget.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(new Vector3(directionToTarget.x, 0, 90)); // Adjusted rotation

        // Example of changing the missile behavior
        GameObject instantMissile = Instantiate(
            missile,
            transform.position + Vector3.up * 10f,  // Custom position
            Quaternion.LookRotation(directionToTarget)
        );

        Missile missileScript = instantMissile.GetComponent<Missile>();
        missileScript.target = currentTarget.transform;

        // More advanced logic or effects can go here

        yield return new WaitForSeconds(0.2f);
    }
}
