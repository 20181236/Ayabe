using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Luna_Playable : SoonDoBu_Playable
{
    protected override void SetStats()
    {
        maxHealth = (float)PlayableHelath.Luna;
        attackRange = (float)PlayableAttackRenge.Luna;
        basicSkillCooldown = (float)PlayalbeBaiscSkillCoolTime.Luna;
    }
    protected override IEnumerator BasicSkill()
    {
        yield return new WaitForSeconds(0.5f);  // µô·¹ÀÌ ¼öÁ¤
        if (currentTarget == null)
            yield break;
        Vector3 directionToTarget = (currentTarget.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(new Vector3(directionToTarget.x, 0, directionToTarget.z)); 
        GameObject instantMissile = Instantiate(
            missile,
            transform.position + Vector3.up * 3f,
            Quaternion.LookRotation(directionToTarget)
        );
        Missile missileScript = instantMissile.GetComponent<Missile>();
        missileScript.target = currentTarget.transform;
        Rigidbody missileRigidbody = instantMissile.GetComponent<Rigidbody>();
        if (missileRigidbody != null)
        {
            missileRigidbody.velocity = directionToTarget * 20f;
        }
        yield return new WaitForSeconds(0.2f);
    }
}
