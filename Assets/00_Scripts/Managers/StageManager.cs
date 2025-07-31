using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StageManager : MonoBehaviour
{
    public static StageManager instance { get; private set; }

    private InterfaceGameState currentState;

    public bool hasBoss = false;
    public bool isEnemyAllClear = false;
    public bool isBossClear = false;
    public bool isStageClear = false;

    [SerializeField] private StageData stageData;
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private EnemyRemain enemyRemain;

    private int totalEnemyCount = 0;
    private int remainingEnemyCount = 0;
    
    private float stageTimer;
    private bool isTimeOver = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        waveManager.SetupWaves(stageData.waves);

        SetState(new StageStartState(this));
        CountTotalEnemies();
        enemyRemain.UpdateEnemyCount(remainingEnemyCount, totalEnemyCount);


        if (stageData != null)
            stageTimer = stageData.timeLimit;
        else
            Debug.LogError("StageData is null!");

        StartCoroutine(CheckWaveProgressCoroutine());
    }

    private void Update()
    {
        currentState?.Update();

        if (!isStageClear && !isTimeOver)
        {
            stageTimer -= Time.deltaTime;

            if (stageTimer <= 0f)
            {
                stageTimer = 0f;
                isTimeOver = true;
                OnStageFailed();
            }
        }
    }

    public void SetState(InterfaceGameState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }
    private void CountTotalEnemies()
    {
        foreach (var wave in stageData.waves)
        {
            totalEnemyCount += wave.enemiesInWave.Length;
        }
        remainingEnemyCount = totalEnemyCount;
    }

    public void NotifyEnemyKilled()
    {
        remainingEnemyCount--;
        enemyRemain.UpdateEnemyCount(remainingEnemyCount, totalEnemyCount);
    }

    private IEnumerator CheckWaveProgressCoroutine()
    {
        yield return new WaitUntil(() => waveManager.GetWaves().Length > 0);

        while (!isStageClear)
        {
            bool allWaveSpawned = waveManager.IsAllWaveSpawned();
            bool noEnemyRemain = !EnemyManager.instance.HasEnemy();
            bool bossDead = !EnemyManager.instance.HasBoss();

            // 보스를 잡았으면 조건 즉시 만족
            if (hasBoss && bossDead)
            {
                isBossClear = true;
                isStageClear = true;
                OnStageClear();
                yield break;
            }

            // 보스가 없는 스테이지라면 전체 적 제거 시 클리어
            if (!hasBoss && allWaveSpawned && noEnemyRemain)
            {
                isEnemyAllClear = true;
                isStageClear = true;
                OnStageClear();
                yield break;
            }

            // 잡몹만 남았고 다음 웨이브 가능하면 진행
            if (!allWaveSpawned && noEnemyRemain)
            {
                waveManager.StartWave();
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private void OnStageClear()
    {
        Debug.Log("스테이지 클리어!");
        // 결과창 등 추가
    }

    private void OnStageFailed()
    {
        Debug.Log("Stage Failed: Time Over");
        //SetState(new StageFailState(this)); // 새로운 실패 상태 클래스 필요 
    }

}

