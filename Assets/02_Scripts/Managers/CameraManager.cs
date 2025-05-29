using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    public float smoothSpeed = 5f;
    public float offsetX = 0f;
    public float offsetY = 10f;
    public float offsetZ = -10f;

    public float minFOV = 40f;
    public float maxFOV = 80f;
    public float fovPerZGap = 1.5f; // zGap�� Ŀ������ �󸶳� FOV �ø���

    private Camera cam;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    void Start()
    {
        cam = Camera.main;
    }

    void LateUpdate()
    {
        List<PlayableBase> playables = PlayableManager.instance.GetPlayables();

        if (playables == null || playables.Count == 0)
            return;

        float minZ = float.MaxValue;
        float maxZ = float.MinValue;

        foreach (var playable in playables)
        {
            float z = playable.transform.position.z;
            if (z < minZ) minZ = z;
            if (z > maxZ) maxZ = z;
        }

        float centerZ = (minZ + maxZ) / 2f;
        float zGap = maxZ - minZ;

        // ī�޶� ��ġ
        Vector3 targetPosition = new Vector3(offsetX, offsetY, centerZ + offsetZ);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);

        // FOV �ڵ� ����
        float targetFOV = Mathf.Clamp(60f + zGap * fovPerZGap, minFOV, maxFOV);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * smoothSpeed);
    }


}
