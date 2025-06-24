using UnityEngine;

public class PoisonGrenade : ProjectileBase
{
    public GameObject plosionEffectPrefab;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float flightTime = 1.0f; // 전체 비행 시간
    private float timer = 0f;
    private float height = 15f; // 포물선 높이

    public void SetTarget(Vector3 target)
    {
        startPosition = transform.position;
        targetPosition = target;
    }
    protected override void SetProjectileInfo()
    {
        base.SetProjectileInfo();
        damage = 100;
        speed = 50f;
        rotateSpeed = 10f;
        isExplosion = true;
    }

    private void Update()
    {
        if (timer < flightTime)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / flightTime);

            // 수평 보간
            Vector3 horizontalPos = Vector3.Lerp(startPosition, targetPosition, t);

            // 높이 포물선 계산 (Parabola)
            float arc = 4 * height * t * (1 - t); // 포물선 y 보정

            Vector3 finalPos = horizontalPos + Vector3.up * arc;
            transform.position = finalPos;

            // 회전 (선택사항)
            transform.LookAt(finalPos + Vector3.forward);
        }
        else
        {
            OnArrive();
        }
    }

    private void OnArrive()
    {
        if (plosionEffectPrefab != null)
        {
            Instantiate(plosionEffectPrefab, transform.position +Vector3.up*2, Quaternion.identity);
        }
        Debug.Log("Poison Grenade 도착!");
        Destroy(gameObject);
    }
}
