using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PlayableMnager : MonoBehaviour
{
    public static PlayableMnager instance { get; private set; }
    public GameObject playablePrefab; // 어떤 애를 생성할건지 필요할듯
    public List<SoonDoBu_Playable> playables = new List<SoonDoBu_Playable>();
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

    // 플레이어블 등록
    public void RegisterPlayable(SoonDoBu_Playable playable)
    {
        if (!playables.Contains(playable))
        {
            playables.Add(playable);
        }
    }

    // 플레이어블 제거
    public void UnregisterPlayable(SoonDoBu_Playable playable)
    {
        if (playables.Contains(playable))
        {
            playables.Remove(playable);
        }
    }

    // 모든 플레이어블 가져오기
    public List<SoonDoBu_Playable> GetPlayables()
    {
        return playables;
    }

    // 플레이어블이 하나라도 있는지 체크
    public bool HasPlayable()
    {
        return playables.Count > 0;
    }
}