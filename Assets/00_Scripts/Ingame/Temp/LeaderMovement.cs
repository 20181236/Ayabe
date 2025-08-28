using UnityEngine;

public class LeaderMovement : MonoBehaviour
{
    public float acceleration = 8f;
    public float maxSpeed = 50f;
    private float currentSpeed = 0f;
    private bool isLaunched = false;

    public void Launch()
    {
        isLaunched = true;
    }

    void Update()
    {
        if (!isLaunched) return;

        // 가속도 적용
        currentSpeed += acceleration * Time.deltaTime;
        currentSpeed = Mathf.Min(currentSpeed, maxSpeed);

        // 앞으로 이동 (Z축 기준, 필요시 forward 방향 변경 가능)
        transform.position += transform.forward * currentSpeed * Time.deltaTime;
    }
}
