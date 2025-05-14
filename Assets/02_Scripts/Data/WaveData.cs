using UnityEngine;

[CreateAssetMenu(fileName = "WaveData", menuName = "WaveData")]
public class WaveData : ScriptableObject
{
    [System.Serializable]
    public class EnemySpawnInfo
    {
        public EnemyType enemyType;
        public Vector3 spawnPosition;
        public float delayAfter; // 이 적을 소환한 후 다음 소환까지의 딜레이
    }

    public EnemySpawnInfo[] enemiesInWave;
}