using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_ExSubMissile : MonoBehaviour
{
    public GameObject redCirclePrefab;
    public GameObject explosionEffect;
    public float damageRadius = 2f;
    public int damageAmount = 50;

    private Vector3 targetPosition;
    private GameObject warning;

    public void Init(Vector3 target)
    {
        targetPosition = target;
        transform.position = target + Vector3.up * 10f; // 낙하시작
        warning = Instantiate(redCirclePrefab, new Vector3(target.x, 0.01f, target.z), Quaternion.identity);
        GetComponent<Animator>().SetTrigger("Fall");
    }

    public void Explode()
    {
        //Collider[] hitTargets = Physics.OverlapSphere(targetPosition, damageRadius, LayerMask.GetMask("Playable"));
        //foreach (Collider col in hitTargets)
        //{
        //    var player = col.GetComponent<PlayableBase>();
        //    if (player != null)
        //        player.TakeDamage(damageAmount);  // 피해 적용
        //}

        //Instantiate(explosionEffect, targetPosition, Quaternion.identity);
        //Destroy(warning);
        //Destroy(gameObject);
    }
}
