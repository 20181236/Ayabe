using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RangeWarning : MonoBehaviour
{
    public float radius = 25f;       // ������ (�̻��� ��� �ݰ�)
    public int segments = 60;        // ���� �󸶳� �����ϰ� �׸���
    public Color warningColor = Color.red;

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = segments + 1;
        lineRenderer.useWorldSpace = false;
        lineRenderer.loop = true;
        lineRenderer.startColor = warningColor;
        lineRenderer.endColor = warningColor;
        lineRenderer.widthMultiplier = 0.1f;

        CreateCircle();
    }

    void CreateCircle()
    {
        float angle = 0f;
        for (int i = 0; i <= segments; i++)
        {
            float x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            float z = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            lineRenderer.SetPosition(i, new Vector3(x, 0, z));
            angle += 360f / segments;
        }
    }
}