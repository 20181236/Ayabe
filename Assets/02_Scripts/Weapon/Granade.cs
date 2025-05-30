using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using UnityEngine;

public class Granade : ProjectileBase
{
    public float explosionDelay = 0.3f;
    public float explosionRadius = 5f;
    public float throwingSpeed = 30f;

    private bool hasExploded = false;

    public GameObject explosionEffect;
    public Vector3 targetPosition;

    protected override void SetProjectileInfo()
    {
        base.SetProjectileInfo();
        damage = 100;
        speed = 50f;
        rotateSpeed = 10f;
        isExplosion = true;
    }

    void Start()
    {
        StartCoroutine(MoveToTarget());
    }

    IEnumerator MoveToTarget()
    {
        Vector3 startPoint = transform.position;  // 현재 위치
        Vector3 endPoint = targetPosition;        // 목표 위치

        // 포물선 궤적을 위한 중간점
        Vector3 centerPoint = (startPoint + endPoint) / 2 + Vector3.up * 20f;

        float progress = 0f;
        float duration = explosionDelay;

        while (progress < 1f)
        {
            progress += Time.deltaTime / duration;

            float curvedProgress = Mathf.Pow(progress, 0.2f);

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
            yield break;

        hasExploded = true;

        if (explosionEffect != null)
        {
            explosionEffect.SetActive(true);
        }

        // 폭발 위치를 현재 위치로 설정
        Vector3 explosionPosition = transform.position;

        // 범위 내 적에게 데미지 적용
        foreach (var enemy in EnemyManager.instance.enemies)
        {
            if (enemy == null)
                continue;

            float distance = Vector3.Distance(explosionPosition, enemy.transform.position);

            if (distance <= explosionRadius)
            {
                // 폭발 위치 넘겨서 데미지 적용
                enemy.ApplyDamage(damage, isExplosion, explosionPosition);
            }
        }
        StartCoroutine(DisableAfterDelay());
    }

    IEnumerator DisableAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);

        if (explosionEffect != null)
            explosionEffect.SetActive(false);

        gameObject.SetActive(false);
    }
}

