using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SkillOutLine : MonoBehaviour
{
    public int segments = 100;    // 원의 세그먼트 수 (클수록 원에 가까움)
    public float radius = 5f;     // 원의 반지름

    private LineRenderer line;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = segments + 1; // 마지막 점은 처음 점과 같아야 하니까 +1

        CreatePoints();
    }

    void CreatePoints()
    {
        float angle = 0f;

        for (int i = 0; i <= segments; i++)
        {
            float x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            float y = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;

            line.SetPosition(i, new Vector3(x, y, 0));

            angle += 360f / segments;
        }
    }
}
