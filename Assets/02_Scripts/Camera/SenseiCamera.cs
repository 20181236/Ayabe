using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SenseiCamera : MonoBehaviour
{
    public float baseDistance = 50f;
    public float distanceMultiplier = 0.5f;
    public Vector3 cameraOffset = new Vector3(20, 5, -10);

    private List<Transform> cameraTargets = new List<Transform>();

    public void RegisterCameraTarget(Transform target)
    {
        cameraTargets.Add(target);
    }

    public void UnregisterCameraTarget(Transform target)
    {
        cameraTargets.Remove(target);
    }

    void LateUpdate()
    {
        if (cameraTargets.Count == 0)
            return;

        Transform frontTarget = cameraTargets.OrderByDescending(t => t.position.z).First();
        Transform backTarget = cameraTargets.OrderBy(t => t.position.z).First();

        float zDistance = Mathf.Abs(frontTarget.position.z - backTarget.position.z);
        float dynamicDistance = baseDistance + zDistance * distanceMultiplier;

        Vector3 targetPosition = frontTarget.position + cameraOffset.normalized * dynamicDistance;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 5f);
        transform.LookAt(frontTarget);
    }
}
    