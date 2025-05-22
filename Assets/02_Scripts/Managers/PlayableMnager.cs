using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

public class PlayableMnager : MonoBehaviour
{
    public static PlayableMnager instance { get; private set; }
    public GameObject playablePrefab; // 어떤 애를 생성할건지 필요할듯
    public List<SoonDoBuPlayable> playables = new List<SoonDoBuPlayable>();
    public List<PlayableBase> playables_imsi = new List<PlayableBase>();
    public Transform[] PlayableSpawnPoints;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public void RegisterPlayable(SoonDoBuPlayable playable)
    {
        if (!playables.Contains(playable))
        {
            playables.Add(playable);
        }
    }

    public void RegisterPlayable_imsi(PlayableBase playables)
    {
        if (!playables_imsi.Contains(playables))
        {
            playables_imsi.Add(playables);
        }
    }

    public void UnregisterPlayable(SoonDoBuPlayable playable)
    {
        if (playables.Contains(playable))
        {
            playables.Remove(playable);
        }
    }

    public void UnregisterPlayable(PlayableBase playable)
    {
        if (playables_imsi.Contains(playable))
        {
            playables_imsi.Remove(playable);
        }
    }

    public List<SoonDoBuPlayable> GetPlayables()
    {
        return playables;
    }

    public bool HasPlayable()
    {
        return playables.Count > 0;
    }
}