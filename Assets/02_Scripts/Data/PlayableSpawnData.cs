using UnityEngine;

[CreateAssetMenu(fileName = "PlayableSpawnData", menuName = "PlayableSpawnData")]
public class PlayableSpawnData : ScriptableObject
{
    [System.Serializable]
    public class PlayableSpawnInfo
    {
        public PlayableID playableID;
        public Vector3 spawnPosition;
        public float delayAfter; // �� ���� ��ȯ�� �� ���� ��ȯ������ ������
    }
    public PlayableSpawnInfo[] playableSpawn;
}