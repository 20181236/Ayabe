using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void SpawnWave(WaveData wave)
    {
        StartCoroutine(SpawnWaveCoroutine(wave));
    }

    private IEnumerator SpawnWaveCoroutine(WaveData wave)
    {
        foreach (var info in wave.enemiesInWave)
        {
            EnemyManager.instance.SpawnEnemy(info.enemyType, info.spawnPosition);
            yield return new WaitForSeconds(info.delayAfter);
        }
    }
}

