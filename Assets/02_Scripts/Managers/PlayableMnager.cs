using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PlayableMnager : MonoBehaviour
{
    public static PlayableMnager instance { get; private set; }
    public GameObject playablePrefab; // � �ָ� �����Ұ��� �ʿ��ҵ�
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

    // �÷��̾�� ���
    public void RegisterPlayable(SoonDoBu_Playable playable)
    {
        if (!playables.Contains(playable))
        {
            playables.Add(playable);
        }
    }

    // �÷��̾�� ����
    public void UnregisterPlayable(SoonDoBu_Playable playable)
    {
        if (playables.Contains(playable))
        {
            playables.Remove(playable);
        }
    }

    // ��� �÷��̾�� ��������
    public List<SoonDoBu_Playable> GetPlayables()
    {
        return playables;
    }

    // �÷��̾���� �ϳ��� �ִ��� üũ
    public bool HasPlayable()
    {
        return playables.Count > 0;
    }
}