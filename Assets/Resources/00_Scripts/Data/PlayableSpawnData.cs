using UnityEngine;

[CreateAssetMenu(fileName = "PlayableSpawnData", menuName = "PlayableSpawnData")]
public class PlayableSpawnData : ScriptableObject
{
    [System.Serializable]
    public class PlayableSpawnInfo
    {
        public PlayableID playableID;
        public Vector3 spawnPosition;
        public float delayAfter; // 이 적을 소환한 후 다음 소환까지의 딜레이
    }
    public PlayableSpawnInfo[] playableSpawn;
}