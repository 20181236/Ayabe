using UnityEngine;

[CreateAssetMenu(fileName = "WaveData", menuName = "WaveData")]
public class WaveData : ScriptableObject
{
    [System.Serializable]
    public class EnemySpawnInfo
    {
        public EnemyType enemyType;
        public Vector3 spawnPosition;
        public float delayAfter; // �� ���� ��ȯ�� �� ���� ��ȯ������ ������
    }

    public EnemySpawnInfo[] enemiesInWave;
}