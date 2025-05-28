using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance { get; private set; }

    public WaveData[] waves;
    private int currentWaveIndex = 0;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void StartFirstWave()
    {
        currentWaveIndex = 0;
        StartWave();
    }

    public void StartWave()
    {
        if (currentWaveIndex < waves.Length)
        {
            SpawnManager.instance.SpawnWave(waves[currentWaveIndex]);
            currentWaveIndex++;
        }
    }

    public bool IsAllWaveSpawned()
    {
        return currentWaveIndex >= waves.Length;
    }
}

