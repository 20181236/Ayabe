using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SenseiCamera : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, 10, -10); // 카메라 위치 오프셋
    public float smoothTime = 0.3f;
    public float minZoom = 80f;
    public float maxZoom = 20f;
    public float zoomLimiter = 30f;//???

    private Vector3 velocity;
    private Camera camera;

    void Start()
    {
        camera = Camera.main;
    }

    void LateUpdate()
    {
        var playables = PlayableManager.instance?.GetPlayables();
        if (playables == null || playables.Count == 0)
            return;

        List<Transform> targets = new List<Transform>();
        foreach (var p in playables)
        {
            if (p != null)
                targets.Add(p.transform);
        }

        Move(targets);
        Zoom(targets);
    }

    void Move(List<Transform> targets)
    {

    }

    void Zoom(List<Transform> targets)
    {

    }
}
