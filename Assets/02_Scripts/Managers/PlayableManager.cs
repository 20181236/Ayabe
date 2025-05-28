using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

public class PlayableManager : MonoBehaviour
{
    public static PlayableManager instance { get; private set; }
    public List<PlayableBase> playables = new List<PlayableBase>();
    public Dictionary<PlayableID, List<PlayableBase>> playablesID = new Dictionary<PlayableID, List<PlayableBase>>();
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

        foreach (PlayableID id in (PlayableID[])System.Enum.GetValues(typeof(PlayableID)))
        {
            playablesID[id] = new List<PlayableBase>();
        }
    }
    public void SpawnPlayable(PlayableID id, Vector3 spawnPosition)
    {
        PlayableData data = System.Array.Find(playableDatas, d => d.playableID == id);
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
            Debug.LogError("PlayableData not found for type: " + id);
        }
    }
    public void RegisterPlayable(PlayableBase playable)
    {
        if (!playables.Contains(playable))
        {
            playables.Add(playable);
            playablesID[playable.playableID].Add(playable);
        }
    }
    public void UnregisterPlayable(PlayableBase playable)
    {
        if (playables.Contains(playable))
        {
            playables.Remove(playable);
            playablesID[playable.playableID].Remove(playable);
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
    public bool HasPlayable(PlayableID id)
    {
        return playablesID[id].Count > 0;
    }
}