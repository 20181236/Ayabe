using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Granade : MonoBehaviour
{
    //�ڵ� ���

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

    // ��ǥ ��ġ�� �̵�
    IEnumerator MoveToTarget()
    {
        Vector3 startPoint = transform.position;//����ġ
        Vector3 endPoint = targetPosition;//��������ġ

        // ������ ��������ؼ� �߰��� �ִ� �������� �ִ°�
        Vector3 centerPoint = (startPoint + endPoint) / 2 + Vector3.up * 30f;

        //�̵������?
        float progress = 0f;
        float duration = explosionDelay;    

        //��ǥ�� �����Ҷ� ����
        while (progress < 1f)
        {
            progress += Time.deltaTime / duration;

            Vector3 m1 = Vector3.Slerp(startPoint, centerPoint, progress);
            Vector3 m2 = Vector3.Slerp(centerPoint, endPoint, progress);
            transform.position = Vector3.Slerp(m1, m2, progress);

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
