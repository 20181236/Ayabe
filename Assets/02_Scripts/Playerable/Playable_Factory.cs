using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayableFactory
{
    public static PlayableBase CreatePlayable(PlayableData data, Vector3 position)
    {
        if (data == null || data.prefab == null)
            return null;

        GameObject playableObject = GameObject.Instantiate(data.prefab, position, Quaternion.identity);
        PlayableBase playable = playableObject.GetComponent<PlayableBase>();

        if (playable != null)
        {
            playable.SetData(data);
        }
        else
        {
            Debug.LogError("Prefab does not contain PlayableBase component");
        }
        Transform cameraTarget = playableObject.transform.Find("CameraTarget");
        if (cameraTarget != null)
        {
            SenseiCamera camera = GameObject.FindObjectOfType<SenseiCamera>();
            if (camera != null)
            {
                camera.RegisterCameraTarget(cameraTarget);
            }
        }
        return playable;
    }
}

