using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

//이 클래스의 역할은 풀링과 캐릭터를 받아오는것 그리고 생성과 스폰은 다른곳에서 맡기되 호출할 것
public class BattleInitializer : MonoBehaviour
{
    public GameObject PlayableBulletPrefab;
    public GameObject EnemyBulletPrefab;
    [SerializeField] private int bulletPoolCount = 30;

    //public PlayableSpawnData[] spawnDatas;
    private int currentIndex = 0;

    public PlayableData[] members; // Leader 포함
    public Transform leaderStartPosition;
    public Transform[] memberStartPositions; // Leader 제외 나머지 캐릭터



    void Awake()
    {
        // GameDataManager에서 선택된 캐릭터를 가져와 members 배열에 할당
        members = GameDataManager.instance.selectedCharacters.ToArray();

        // Leader는 members[0]로 보장
        if (GameDataManager.instance.leaderCharacter != null && members.Length > 0)
            members[0] = GameDataManager.instance.leaderCharacter; // 항상 Leader가 첫 번째
    }

    //이거 오브젝트 풀링
    void Start()
    {
        //BulletPoolManager.instance.RegisterBulletPrefab(BulletPoolManager.PoolType.EnemyBullet, EnemyBulletPrefab.GetComponent<Bullet>());
        //BulletPoolManager.instance.CreatePooling(BulletPoolManager.PoolType.EnemyBullet, 30);
        //BulletPoolManager.instance.RegisterBulletPrefab(BulletPoolManager.PoolType.PlayableBullet, PlayableBulletPrefab.GetComponent<Bullet>());
        //BulletPoolManager.instance.CreatePooling(BulletPoolManager.PoolType.PlayableBullet, 30);

        RegisterAndCreatePool(BulletPoolManager.PoolType.EnemyBullet, EnemyBulletPrefab.GetComponent<Bullet>(), bulletPoolCount);
        RegisterAndCreatePool(BulletPoolManager.PoolType.PlayableBullet, PlayableBulletPrefab.GetComponent<Bullet>(), bulletPoolCount);

        StartPlaySpawn();
    }

    private void RegisterAndCreatePool(BulletPoolManager.PoolType type, Bullet prefab, int count)
    {
        BulletPoolManager.instance.RegisterBulletPrefab(type, prefab);
        BulletPoolManager.instance.CreatePooling(type, count);
    }

    public void StartPlaySpawn()
    {
        // Leader 소환
        if (currentIndex == 0 && members.Length > 0)
        {
            SpawnManager.instance.SpawnPlayable(members[0], leaderStartPosition.position);
        }

        // 나머지 멤버 소환
        for (int i = 1; i < members.Length; i++)
        {
            Vector3 position = (i - 1 < memberStartPositions.Length) ? memberStartPositions[i - 1].position : Vector3.zero;
            SpawnManager.instance.SpawnPlayable(members[i], position);
        }
    }
}
