using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StageManager : MonoBehaviour
{
    public static StageManager instance { get; private set; }

    public int totalWaves;
    public bool isStageClear = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        if (SpawnManager.instance != null)
        {
            totalWaves = SpawnManager.instance.waves.Length;
            SpawnManager.instance.StartWave(); // 첫 웨이브 자동 시작
        }
        else
        {
            Debug.LogError("SpawnManager가 존재하지 않습니다.");
        }
    }

    private void Update()
    {

    }

}
