using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance { get; private set; }

    public WaveData[] waves;
    private int currentWaveIndex = 0;

    public int TotalEnemyCount { get; private set; } = 0;
    public int RemainingEnemyCount { get; private set; } = 0;
    
   [SerializeField] private EnemyNokori enemyNokori; // 여기로 직접 연결

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        CountTotalEnemies();
        enemyNokori.UpdateEnemyCount(RemainingEnemyCount, TotalEnemyCount);
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
    private void CountTotalEnemies()
    {
        foreach (var wave in waves)
        {
            TotalEnemyCount += wave.enemiesInWave.Length;
        }
        RemainingEnemyCount = TotalEnemyCount;
    }

    public void NotifyEnemyKilled()
    {
        RemainingEnemyCount--;
        enemyNokori.UpdateEnemyCount(RemainingEnemyCount, TotalEnemyCount);
    }
}

