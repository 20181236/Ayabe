using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections.Generic;
using UnityEngine;

public class WaveSpawnManager : MonoBehaviour
{
    [System.Serializable]
    public class SpawnData
    {
        public Vector3 spawnPosition;  // 적이 스폰될 위치
        public string enemyType;       // 적의 타입 (Melee, Ranged, Elite 등)
    }

    // 하드코딩된 웨이브 데이터
    public List<SpawnData> waveSpawnTable = new List<SpawnData>()
    {
        new SpawnData() { spawnPosition = new Vector3(0, 0, 0), enemyType = "Ranged" },
        new SpawnData() { spawnPosition = new Vector3(5, 0, 5), enemyType = "Ranged" },
        new SpawnData() { spawnPosition = new Vector3(-5, 0, -5), enemyType = "Elite" },
    };

    private int currentWave = 0;  // 현재 웨이브

    private void Start()
    {
        SpawnWave(currentWave);  // 첫 번째 웨이브 스폰
    }

    // 웨이브에 맞는 적을 스폰
    public void SpawnWave(int waveIndex)
    {
        // 현재 웨이브에 맞는 스폰 데이터 가져오기
        List<SpawnData> spawnDataList = waveSpawnTable;

        foreach (var spawnData in spawnDataList)
        {
            // 적을 생성
            Enemy_base enemy = EnemyManager.instance.SpawnEnemy(spawnData.enemyType, spawnData.spawnPosition, Quaternion.identity);
            if (enemy != null)
            {
                enemy.gameObject.SetActive(true);  // 적 활성화
            }
        }
    }

    // 다음 웨이브로 넘어가기
    public void NextWave()
    {
        currentWave++;
        SpawnWave(currentWave);  // 다음 웨이브 스폰
    }
}
