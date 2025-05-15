using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 2f;

    private Coroutine returnCoroutine;

    private void OnEnable()
    {
        returnCoroutine = StartCoroutine(ReturnAfterSeconds());
    }
        
    private void OnDisable()
    {
        if (returnCoroutine != null)
            StopCoroutine(returnCoroutine);
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private IEnumerator ReturnAfterSeconds()
    {
        yield return new WaitForSeconds(lifeTime);
        Bullet_ObjectPooling.instance.ReturnBullet(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 충돌시 바로 반환
        Bullet_ObjectPooling.instance.ReturnBullet(this);
    }
}

