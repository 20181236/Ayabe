using UnityEngine;

public class PoisonGrenade : ProjectileBase
{
    [SerializeField] public GameObject plosionEffectPrefab;
    [SerializeField] private SkillData skillData;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float attackPower;
    public float skillRatio = 1.5f; // 스킬 계수150%
    private float flightTime = 1.0f; // 전체 비행 시간
    private float timer = 0f;
    private float height = 15f; // 포물선 높이

    private bool ignoreTimeScale = false;  // 추가

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

    public void SetAttackPower(float power)
    {
        attackPower = power;
    }
    public void SetIgnoreTimeScale(bool ignore)
    {
        ignoreTimeScale = ignore;
    }

    protected override void Update()
    {
        // base.Update();

        float delta = ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
        timer += delta;
        Debug.Log($"PoisonGrenade Update - ignoreTimeScale: {ignoreTimeScale}, delta: {delta}, timer: {timer}");
        Debug.Log($"Time.deltaTime: {Time.deltaTime}, Time.unscaledDeltaTime: {Time.unscaledDeltaTime}, timeScale: {Time.timeScale}");


        if (timer < flightTime)
        {
            float t = Mathf.Clamp01(timer / flightTime);

            Vector3 horizontalPosision = Vector3.Lerp(startPosition, targetPosition, t);
            float arc = 4 * height * t * (1 - t);

            Vector3 finalPosision = horizontalPosision + Vector3.up * arc;
            transform.position = finalPosision;

            transform.LookAt(finalPosision + Vector3.forward);
        }
        else
        {
            OnArrive();
        }
    }

    //private void Update()
    //{
    //    if (timer < flightTime)
    //    {
    //        timer += Time.deltaTime;
    //        float t = Mathf.Clamp01(timer / flightTime);

    //        수평 보간
    //        Vector3 horizontalPosision = Vector3.Lerp(startPosition, targetPosition, t);

    //        높이 포물선 계산(Parabola)
    //        float arc = 4 * height * t * (1 - t); // 포물선 y 보정

    //        Vector3 finalPosision = horizontalPosision + Vector3.up * arc;
    //        transform.position = finalPosision;

    //        회전(선택사항)
    //        transform.LookAt(finalPosision + Vector3.forward);
    //    }
    //    else
    //    {
    //        OnArrive();
    //    }
    //}

    private void OnArrive()
    {
        if (plosionEffectPrefab != null)
        {
            GameObject plosion = Instantiate(plosionEffectPrefab, transform.position + Vector3.up * 2, Quaternion.identity);
            Debug.Log("영역전개");

            // Area 컴포넌트가 있으면 skillData를 전달한다
            Area area = plosion.GetComponent<Area>();
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
