using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SenseiCamera : MonoBehaviour
{
    public Vector3 baseOffset = new Vector3(0f, 15f, -15f);
    public float distanceMultiplier = 0.7f;
    public float smoothSpeed = 5f;
    public float minZoom = 1.0f;
    public float maxZoom = 3.0f;

    public Camera cam;

    public float baseFOV = 60f;    // 기본 FOV 값
    public float maxFOV = 90f;     
    public float fovSmoothSpeed = 2f;

    private void Start()
    {
        if (cam == null)
            cam = Camera.main;
    }

    private void LateUpdate()
    {
        var playables = PlayableManager.instance?.GetPlayables();
        var enemies = EnemyManager.instance?.GetEnemies();

        if ((playables == null || playables.Count == 0) &&
            (enemies == null || enemies.Count == 0))
            return;

        List<Vector3> allPositions = new List<Vector3>();
        if (playables != null) allPositions.AddRange(GetXZPositions(playables));
        if (enemies != null) allPositions.AddRange(GetXZPositions(enemies));

        Vector3 center = GetCenter(allPositions);
        float spread = GetSpread(allPositions, center);

        float zoomFactor = Mathf.Clamp(1f + spread * distanceMultiplier, minZoom, maxZoom);
        Vector3 dynamicOffset = baseOffset * zoomFactor;

        Vector3 targetPosition = center + dynamicOffset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);

        transform.rotation = Quaternion.Lerp(transform.rotation,
            Quaternion.LookRotation(center - transform.position),
            Time.deltaTime * smoothSpeed);

        // FOV 조절: spread에 따라 baseFOV ~ maxFOV 사이에서 부드럽게 변경
        float targetFOV = Mathf.Lerp(baseFOV, maxFOV, Mathf.InverseLerp(minZoom, maxZoom, zoomFactor));
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * fovSmoothSpeed);
    }

    private List<Vector3> GetXZPositions<T>(List<T> objects) where T : MonoBehaviour
    {
        List<Vector3> positions = new List<Vector3>();
        foreach (var obj in objects)
        {
            if (obj != null)
            {
                Vector3 pos = obj.transform.position;
                positions.Add(new Vector3(pos.x, 0, pos.z)); // Y 무시
            }
        }
        return positions;
    }

    private Vector3 GetCenter(List<Vector3> positions)
    {
        if (positions.Count == 0)
            return Vector3.zero;

        Vector3 sum = Vector3.zero;

        foreach (var pos in positions)
            sum += pos;

        return sum / positions.Count;
    }

    private float GetSpread(List<Vector3> positions, Vector3 center)
    {
        if (positions.Count == 0) return 0f;
        float totalDist = 0f;
        foreach (var pos in positions)
        {
            totalDist += Vector3.Distance(center, pos);
        }
        return totalDist / positions.Count;
    }
}
