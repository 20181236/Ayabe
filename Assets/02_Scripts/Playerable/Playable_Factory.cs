using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayableFactory
{
    public static PlayableBase CreatePlayable(PlayableData data, Vector3 position)
    {
        if (data == null || data.prefab == null)
        {
            Debug.LogError("PlayableData or prefab is null");
            return null;
        }

        GameObject playableObj = GameObject.Instantiate(data.prefab, position, Quaternion.identity);
        Debug.Log($"[PlayableFactory] Creating playable of type {data.playableType} at {position}");

        PlayableBase playable = playableObj.GetComponent<PlayableBase>();

        if (playable != null)
        {
            playable.SetData(data); // ���� �� �ʱ� ����
        }
        else
        {
            Debug.LogError("Prefab does not contain PlayableBase component");
        }

        return playable;
    }
}

