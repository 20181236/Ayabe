using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

public class PlayableMnager : MonoBehaviour
{
    public static PlayableMnager instance { get; private set; }
    public List<PlayableBase> playables = new List<PlayableBase>();
    public Dictionary<PlayableType, List<PlayableBase>> playablesType = new Dictionary<PlayableType, List<PlayableBase>>();
    public PlayableData[] playableDatas; // 에디터에 드래그해서 연결할 수 있게

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        foreach (PlayableType type in (PlayableType[])System.Enum.GetValues(typeof(PlayableType)))
        {
            playablesType[type] = new List<PlayableBase>();
        }
    }
    public void SpawnPlayable(PlayableType type, Vector3 spawnPosition)
    {
        PlayableData data = System.Array.Find(playableDatas, d => d.playableType == type);
        if (data != null)
        {
            PlayableBase playable = PlayableFactory.CreatePlayable(data, spawnPosition);
            if (playable != null)
            {
                RegisterPlayable(playable);
            }
        }
        else
        {
            Debug.LogError("PlayableData not found for type: " + type);
        }
    }
    public void RegisterPlayable(PlayableBase playable)
    {
        if (!playables.Contains(playable))
        {
            playables.Add(playable);
            playablesType[playable.playableType].Add(playable);
        }
    }
    public void UnregisterPlayable(PlayableBase playable)
    {
        if (playables.Contains(playable))
        {
            playables.Remove(playable);
            playablesType[playable.playableType].Remove(playable);
        }
    }
    public List<PlayableBase> GetPlayables()
    {
        return playables;
    }
    public List<PlayableBase> GetPlayablesOfType(PlayableType type)
    {
        return playablesType[type];
    }
    public bool HasPlayable()
    {
        return playables.Count > 0;
    }
    public bool HasPlayableOfType(PlayableType type)
    {
        return playablesType[type].Count > 0;
    }
}