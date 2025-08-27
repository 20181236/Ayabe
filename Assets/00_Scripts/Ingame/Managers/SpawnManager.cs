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
        GameObject spawnPlayable = Instantiate(playable.prefab, position, Quaternion.identity);
        spawnPlayable.SetActive(true);
        return spawnPlayable; // 생성된 오브젝트 반환
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

