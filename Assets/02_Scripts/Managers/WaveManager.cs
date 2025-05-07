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
        public Vector3 spawnPosition;  // ���� ������ ��ġ
        public string enemyType;       // ���� Ÿ�� (Melee, Ranged, Elite ��)
    }

    // �ϵ��ڵ��� ���̺� ������
    public List<SpawnData> waveSpawnTable = new List<SpawnData>()
    {
        new SpawnData() { spawnPosition = new Vector3(0, 0, 0), enemyType = "Ranged" },
        new SpawnData() { spawnPosition = new Vector3(5, 0, 5), enemyType = "Ranged" },
        new SpawnData() { spawnPosition = new Vector3(-5, 0, -5), enemyType = "Elite" },
    };

    private int currentWave = 0;  // ���� ���̺�

    private void Start()
    {
        SpawnWave(currentWave);  // ù ��° ���̺� ����
    }

    // ���̺꿡 �´� ���� ����
    public void SpawnWave(int waveIndex)
    {
        // ���� ���̺꿡 �´� ���� ������ ��������
        List<SpawnData> spawnDataList = waveSpawnTable;

        foreach (var spawnData in spawnDataList)
        {
            // ���� ����
            Enemy_base enemy = EnemyManager.instance.SpawnEnemy(spawnData.enemyType, spawnData.spawnPosition, Quaternion.identity);
            if (enemy != null)
            {
                enemy.gameObject.SetActive(true);  // �� Ȱ��ȭ
            }
        }
    }

    // ���� ���̺�� �Ѿ��
    public void NextWave()
    {
        currentWave++;
        SpawnWave(currentWave);  // ���� ���̺� ����
    }
}
