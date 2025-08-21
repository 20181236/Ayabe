using System.Collections;
using UnityEngine;

public class PoisonGrenade : ProjectileBase
{
    [SerializeField] private GameObject plosionEffectPrefab;
    [SerializeField] private SkillData skillData;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float attackPower;

    public float skillRatio = 1.5f; // 스킬 계수 (예: 150%)
    private float maximumHeight = 15f; // 포물선 최고 높이
    private float flightDuration = 1.0f; // 실제 시간 기준 총 비행 시간 (초)

    public void SetTarget(Vector3 target)
    {
        startPosition = transform.position;
        targetPosition = target;
    }

    public void SetAttackPower(float power)
    {
        attackPower = power;
    }

    protected override void SetProjectileInfo()
    {
        base.SetProjectileInfo();
        damage = 100;         // 기본 데미지
        speed = 50f;          // 필요 시 사용
        rotateSpeed = 10f;    // 필요 시 사용
        isExplosion = true;
    }

    private void Start()
    {
        StartCoroutine(ParabolaMoveCoroutine());
    }

    private IEnumerator ParabolaMoveCoroutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < flightDuration)
        {
            float progressRatio = elapsedTime / flightDuration;

            // 선형 보간 위치 (수평 이동)
            Vector3 horizontalPosition = Vector3.Lerp(startPosition, targetPosition, progressRatio);

            // 포물선 높이 계산
            float verticalOffset = 4f * maximumHeight * progressRatio * (1f - progressRatio);

            Vector3 currentPosition = horizontalPosition + Vector3.up * verticalOffset;
            transform.position = currentPosition;

            // 시각적 회전 (선택 사항)
            transform.LookAt(currentPosition + Vector3.forward);

            yield return null;
            elapsedTime += Time.unscaledDeltaTime; // 타임스케일 무시하고 실제 시간 기준으로 진행
        }

        OnArrive();
    }

    private void OnArrive()
    {
        if (plosionEffectPrefab != null)
        {
            GameObject explosion = Instantiate(plosionEffectPrefab, transform.position + Vector3.up * 2, Quaternion.identity);
            Debug.Log("영역전개");

            Area area = explosion.GetComponent<Area>();
            if (area != null)
            {
                area.SetArea(skillData);
                area.SetAttackPower(attackPower);
            }
        }

        Debug.Log("Poison Grenade 도착!");
        Destroy(gameObject);
    }
}
