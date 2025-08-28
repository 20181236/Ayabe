using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void SpawnWave(WaveData wave)
    {
        StartCoroutine(SpawnWaveCoroutine(wave));
    }

    private IEnumerator SpawnWaveCoroutine(WaveData wave)
    {
        foreach (var info in wave.enemiesInWave)
        {
            EnemyManager.instance.SpawnEnemy(info.enemyID, info.spawnPosition);
            yield return new WaitForSeconds(info.delayAfter);
        }
    }
    public GameObject SpawnPlayable(PlayableData playable, Vector3 position)
    {
        if (playable == null || playable.prefab == null)
        {
            Debug.LogError("PlayableData or prefab is null!");
            return null;
        }

        // Factory를 이용해 생성
        PlayableBase playableInstance = PlayableFactory.CreatePlayable(playable, position);

        if (playableInstance == null)
            return null;

        // GameObject 반환
        return playableInstance.gameObject;
    }


    //public void PlayableSpawn(PlayableSpawnData spawnData)
    //{
    //    StartCoroutine(SpawnCoroutine(spawnData));
    //}

    //private IEnumerator SpawnCoroutine(PlayableSpawnData spawnData)
    //{
    //    foreach (var info in spawnData.playableSpawn)
    //    {
    //        PlayableManager.instance.SpawnPlayable(info.playableID, info.spawnPosition);
    //        yield return new WaitForSeconds(info.delayAfter);
    //    }
    //}
}

