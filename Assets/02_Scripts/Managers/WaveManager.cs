using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    public int currentWave = 1;
    public int enemiesPerWave = 5;
    private int remainingEnemiesInWave;

    public float waveInterval = 3f; // ���̺� �� ��� �ð�
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

        // ���̺� ���� �� ��� �ð�
        InvokeRepeating("ActivateEnemies", 0f, 1f); // 1�ʸ��� ���� Ȱ��ȭ
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
        yield return new WaitForSeconds(waveInterval); // ���̺� �� ��� �ð�
        isWaveInProgress = false;
        StartNextWave(); // ���� ���̺� ����
    }

    Vector3 GetRandomSpawnPosition()
    {
        return new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f)); // ���� ���� ��ġ
    }

    public void OnEnemyDie()
    {
        if (EnemyManager.instance.activeEnemies.Count == 0) // ��� ���� óġ�Ǿ�����
        {
            StartNextWave(); // ���̺� �Ϸ� �� ���� ���̺� ����
        }
    }
}
