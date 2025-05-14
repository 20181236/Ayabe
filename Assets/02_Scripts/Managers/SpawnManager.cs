using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instance { get; private set; }

    public WaveData[] waves;
    private int currentWaveIndex = 0;
    public bool autoStartNextWave = true;
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
    public void StartWave()
    {
        if (currentWaveIndex >= waves.Length)
        {
            return;
        }

        StartCoroutine(SpawnWaveCoroutine(waves[currentWaveIndex]));
        currentWaveIndex++;
    }

    private IEnumerator SpawnWaveCoroutine(WaveData wave)
    {
        foreach (var info in wave.enemiesInWave)
        {
            EnemyManager.instance.SpawnEnemy(info.enemyType, info.spawnPosition);
            yield return new WaitForSeconds(info.delayAfter);
        }

        if (autoStartNextWave)
        {
            // 적이 전부 죽었는지 판단 후 자동 진행 가능
            StartCoroutine(WaitAndCheckNextWave());
        }
    }
    private IEnumerator WaitAndCheckNextWave()
    {
        yield return new WaitUntil(() => !EnemyManager.instance.HasEnemy());
        StartWave();
    }
}
