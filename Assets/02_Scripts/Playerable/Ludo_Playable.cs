using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ludo_Playable : PlayableBase
{
    public GameObject grenadePrefab;

    protected override void Skill()
    {
        //yield return null;

        //yield return new WaitForSeconds(0.5f);

        //if (currentTarget == null)
            //yield break;
        Vector3 spawnPosition = transform.position + Vector3.up * 3f;
        GameObject grenadeObject = Instantiate(
            grenadePrefab,
            transform.position + Vector3.up * 2f,
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
    }
}