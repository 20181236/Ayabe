using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    public int currentWave = 1;
    public int enemiesPerWave = 5;
    private int remainingEnemiesInWave;

    public float waveInterval = 3f; // 웨이브 간 대기 시간
    private bool isWaveInProgress = false;

    void Start()
    {
        StartNextWave();
    }

    public void StartNextWave()
    {
        if (isWaveInProgress)
            return;

        currentWave++;
        remainingEnemiesInWave = enemiesPerWave * currentWave;
        isWaveInProgress = true;

        // 웨이브 시작 전 대기 시간
        InvokeRepeating("ActivateEnemies", 0f, 1f); // 1초마다 적을 활성화
    }

    void ActivateEnemies()
    {
        if (remainingEnemiesInWave > 0)
        {
            Enemy_base enemy = EnemyManager.instance.SpawnEnemy("Enemy", GetRandomSpawnPosition(), Quaternion.identity);
            remainingEnemiesInWave--;
        }
        else
        {
            CancelInvoke("ActivateEnemies");
            StartCoroutine(WaitBeforeNextWave());
        }
    }

    IEnumerator WaitBeforeNextWave()
    {
        yield return new WaitForSeconds(waveInterval); // 웨이브 간 대기 시간
        isWaveInProgress = false;
        StartNextWave(); // 다음 웨이브 시작
    }

    Vector3 GetRandomSpawnPosition()
    {
        return new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f)); // 랜덤 생성 위치
    }

    public void OnEnemyDie()
    {
        if (EnemyManager.instance.activeEnemies.Count == 0) // 모든 적이 처치되었으면
        {
            StartNextWave(); // 웨이브 완료 후 다음 웨이브 시작
        }
    }
}
