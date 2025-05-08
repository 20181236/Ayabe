using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

public class PlayableMnager : MonoBehaviour
{
    public static PlayableMnager instance { get; private set; }
    public GameObject playablePrefab; // 어떤 애를 생성할건지 필요할듯
    public List<SoonDoBu_Playable> playables = new List<SoonDoBu_Playable>();
    public List<Playable_base> playables_imsi = new List<Playable_base>();
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
    
    public void RegisterPlayable(SoonDoBu_Playable playable)
    {
        if (!playables.Contains(playable))
        {
            playables.Add(playable);
        }
    }

    public void RegisterPlayable_imsi(Playable_base playables)
    {
        if (!playables_imsi.Contains(playables))
        {
            playables_imsi.Add(playables);
        }
    }

    public void UnregisterPlayable(SoonDoBu_Playable playable)
    {
        if (playables.Contains(playable))
        {
            playables.Remove(playable);
        }
    }

    public void UnregisterPlayable(Playable_base playable)
    {
        if (playables_imsi.Contains(playable))
        {
            playables_imsi.Remove(playable);
        }
    }

    public List<SoonDoBu_Playable> GetPlayables()
    {
        return playables;
    }

    public bool HasPlayable()
    {
        return playables.Count > 0;
    }
}