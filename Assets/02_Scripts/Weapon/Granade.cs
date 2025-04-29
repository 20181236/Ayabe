using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Granade : MonoBehaviour
{
    public float explosionDelay = 2f;
    public float explosionRadius = 5f;
    public float explosionDamage = 100f;
    public float throwingSpeed = 10f;

    private bool hasExploded = false;

    public GameObject explosionEffect;
    public Vector3 targetPosition;

    void Start()
    {
        StartCoroutine(MoveToTarget());
    }

    // ��ǥ ��ġ�� �̵��ϴ� �ڷ�ƾ
    IEnumerator MoveToTarget()
    {
        // ��ǥ ��ġ���� �Ÿ� ���
        float distance = Vector3.Distance(transform.position, targetPosition);

        // �Ÿ� ��� �ӵ� ���� (�ָ� �������� ������ �̵�)
        float speed = Mathf.Lerp(5f, throwingSpeed, distance / 10f);

        while (Vector3.Distance(transform.position, targetPosition) > 0.8f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }

        StartCoroutine(Esplosion());
    }

    IEnumerator Esplosion()
    {
        if (hasExploded)
            yield return null;

        hasExploded = true;

        if (explosionEffect != null)
        {
            explosionEffect.SetActive(true);
        }

        foreach (var enemy in EnemyManager.instance.enemies)
        {
            if (enemy == null)
                continue;

            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (distance <= explosionRadius)
            {
                enemy.HitByGrenade(transform.position);
            }
        }
        StartCoroutine(DisableAfterDelay());
    }

    IEnumerator DisableAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
        explosionEffect.SetActive(false);
        gameObject.SetActive(false);
    }
}
