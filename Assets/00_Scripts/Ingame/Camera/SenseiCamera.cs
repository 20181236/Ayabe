using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SenseiCamera : MonoBehaviour
{ 
    [Header("Target Manager")]
    private PlayableManager playableManager;

    [Header("Camera Settings")]
    [Tooltip("아군 평균 위치에서 카메라가 얼마나 떨어져 있을지에 대한 고정값")]
    public Vector3 cameraOffset = new Vector3(0f, 30f, -20f);

    void Start()
    {
        playableManager = PlayableManager.instance;
    }

    // 캐릭터 움직임이 끝난 후 카메라를 이동시키기 위해 LateUpdate 사용
    void LateUpdate()
    {
        CameraMove();
    }

    public void CameraMove()
    {
        if (!playableManager.HasPlayable())
        {
            return;
        }

        // 1. 모든 아군의 위치를 더해서 평균을 계산
        Vector3 averagePosition = Vector3.zero;
        var allies = playableManager.GetPlayables();
        foreach (var ally in allies)
        {
            averagePosition += ally.transform.position;
        }
        averagePosition /= allies.Count;

        // 2. 새로운 Vector3를 만들어서 카메라 위치 설정 (이 부분을 수정)
        // X 좌표는 계산된 값을 사용하고, Y와 Z 좌표는 카메라의 현재 값을 그대로 사용
        transform.position = new Vector3(
            averagePosition.x + cameraOffset.x,
            transform.position.y,
            transform.position.z
        );

        //if (!playableManager.HasPlayable())
        //{
        //    return;
        //}

        //// 1. 모든 아군의 위치를 더해서 평균을 계산
        //Vector3 averagePosition = Vector3.zero;
        //var allies = playableManager.GetPlayables();
        //foreach (var ally in allies)
        //{
        //    averagePosition += ally.transform.position;
        //}
        //averagePosition /= allies.Count;

        //// 2. 계산된 평균 위치에 고정 오프셋을 더해 카메라 위치를 즉시 설정
        //transform.position = averagePosition + cameraOffset;
    }
}
//public Vector3 baseOffset = new Vector3(0f, 15f, -15f);
//public float distanceMultiplier = 0.7f;
//public float smoothSpeed = 5f;
//public float minZoom = 1.0f;
//public float maxZoom = 3.0f;

//public Camera cam;

//public float baseFOV = 60f;    // 기본 FOV 값
//public float maxFOV = 90f;
//public float fovSmoothSpeed = 2f;

//private void Start()
//{
//    if (cam == null)
//        cam = Camera.main;
//}

//private void LateUpdate()
//{
//    var playables = PlayableManager.instance?.GetPlayables();
//    var enemies = EnemyManager.instance?.GetEnemies();

//    if ((playables == null || playables.Count == 0) &&
//        (enemies == null || enemies.Count == 0))
//        return;

//    List<Vector3> allPositions = new List<Vector3>();
//    if (playables != null) allPositions.AddRange(GetXZPositions(playables));
//    if (enemies != null) allPositions.AddRange(GetXZPositions(enemies));

//    Vector3 center = GetCenter(allPositions);
//    float spread = GetSpread(allPositions, center);

//    float zoomFactor = Mathf.Clamp(1f + spread * distanceMultiplier, minZoom, maxZoom);
//    Vector3 dynamicOffset = baseOffset * zoomFactor;

//    Vector3 targetPosition = center + dynamicOffset;
//    transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);

//    transform.rotation = Quaternion.Lerp(transform.rotation,
//        Quaternion.LookRotation(center - transform.position),
//        Time.deltaTime * smoothSpeed);

//    // FOV 조절: spread에 따라 baseFOV ~ maxFOV 사이에서 부드럽게 변경
//    float targetFOV = Mathf.Lerp(baseFOV, maxFOV, Mathf.InverseLerp(minZoom, maxZoom, zoomFactor));
//    cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * fovSmoothSpeed);
//}

//private List<Vector3> GetXZPositions<T>(List<T> objects) where T : MonoBehaviour
//{
//    List<Vector3> positions = new List<Vector3>();
//    foreach (var obj in objects)
//    {
//        if (obj != null)
//        {
//            Vector3 pos = obj.transform.position;
//            positions.Add(new Vector3(pos.x, 0, pos.z)); // Y 무시
//        }
//    }
//    return positions;
//}

//private Vector3 GetCenter(List<Vector3> positions)
//{
//    if (positions.Count == 0)
//        return Vector3.zero;

//    Vector3 sum = Vector3.zero;

//    foreach (var pos in positions)
//        sum += pos;

//    return sum / positions.Count;
//}

//private float GetSpread(List<Vector3> positions, Vector3 center)
//{
//    if (positions.Count == 0) return 0f;
//    float totalDist = 0f;
//    foreach (var pos in positions)
//    {
//        totalDist += Vector3.Distance(center, pos);
//    }
//    return totalDist / positions.Count;
//}