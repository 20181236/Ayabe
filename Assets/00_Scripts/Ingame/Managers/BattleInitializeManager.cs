using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BattleInitializer : MonoBehaviour
{
    public GameObject PlayableBulletPrefab;
    public GameObject EnemyBulletPrefab;

    //public PlayableSpawnData[] spawnDatas;
    private int currentIndex = 0;

    public PlayableData[] members; // Leader 포함
    public Transform leaderStartPosition;
    public Transform[] memberStartPositions; // Leader 제외 나머지 캐릭터

    public Cinemachine.CinemachineDollyCart leaderDollyCart; // 씬에 배치된 DollyCart

    public Light[] pointLights; // 8개의 PointLight를 Inspector에서 할당
    public float delay = 0.5f;  // 켜지는 시간 간격


    void Awake()
    {
        // GameDataManager에서 선택된 캐릭터를 가져와 members 배열에 할당
        members = GameDataManager.instance.selectedCharacters.ToArray();

        // Leader는 members[0]로 보장
        if (GameDataManager.instance.leaderCharacter != null && members.Length > 0)
            members[0] = GameDataManager.instance.leaderCharacter; // 항상 Leader가 첫 번째

        if (leaderDollyCart == null)
            leaderDollyCart = GameObject.Find("Dolly Cart").GetComponent<Cinemachine.CinemachineDollyCart>();

        // Dolly Cart 시작 속도 0
        if (leaderDollyCart != null)
            leaderDollyCart.m_Speed = 0f;
    }

    //이거 오브젝트 풀링
    void Start()
    {
        BulletPoolManager.instance.RegisterBulletPrefab(BulletPoolManager.PoolType.EnemyBullet, EnemyBulletPrefab.GetComponent<Bullet>());
        BulletPoolManager.instance.CreatePooling(BulletPoolManager.PoolType.EnemyBullet, 30);
        BulletPoolManager.instance.RegisterBulletPrefab(BulletPoolManager.PoolType.PlayableBullet, PlayableBulletPrefab.GetComponent<Bullet>());

        BulletPoolManager.instance.CreatePooling(BulletPoolManager.PoolType.PlayableBullet, 30);
        StartPlaySpawn();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void StartPlaySpawn()
    {
        if (members.Length == 0)
            return;

        // Leader + 멤버 소환을 Coroutine으로 처리
        StartCoroutine(SpawnLeaderSequence());
    }

    IEnumerator SpawnLeaderSequence()
    {
        // 1. Leader 공중 스폰
       Vector3 spawnPos = leaderStartPosition.position + new Vector3(0, 20f, 0);
        GameObject leaderObj = SpawnManager.instance.SpawnPlayable(members[0], spawnPos);

        var leaderMove = leaderObj.AddComponent<LeaderMovement>();
        leaderMove.dollyCart = leaderDollyCart;
        leaderMove.acceleration = 10f;
        leaderMove.maxSpeed = 50f;

        // 2. PointLight 연출
        pointLights[0].gameObject.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        pointLights[1].gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);

        // 3. Dolly Cart 따라가기 시작
        leaderMove.StartFollowDolly();

        // 4. 나머지 멤버 소환
        for (int i = 1; i < members.Length; i++)
        {
            Vector3 pos = memberStartPositions.Length >= i ? memberStartPositions[i - 1].position : Vector3.zero;
            SpawnManager.instance.SpawnPlayable(members[i], pos);
        }
    }


    //public void StartPlaySpawn()
    //{
    //    // Leader 소환
    //    if (currentIndex == 0 && members.Length > 0)
    //    {
    //        SpawnManager.instance.SpawnPlayable(members[0], leaderStartPosition.position);
    //    }

    //    // 나머지 멤버 소환
    //    for (int i = 1; i < members.Length; i++)
    //    {
    //        Vector3 pos = memberStartPositions.Length >= i ? memberStartPositions[i - 1].position : Vector3.zero;
    //        SpawnManager.instance.SpawnPlayable(members[i], pos);
    //    }

    //    currentIndex++; // 필요하다면 유지, 단 members 기준으로만 사용
    //}

}
