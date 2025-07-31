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
    [SerializeField] private LimitTime limitTime;


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

        CountTotalEnemies();
        enemyRemain.UpdateEnemyCount(remainingEnemyCount, totalEnemyCount);

        //SetState(new StageStartState(this));

        if (stageData != null)
            stageTimer = stageData.timeLimit;
        else
            Debug.LogError("StageData is null!");

        StartCoroutine(CheckWaveProgressCoroutine());
    }

    private void Update()
    {
        currentState?.Update();
        UpdateStageTimer();
    }
    private void LateUpdate()
    {
        
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
            if (isTimeOver)
            {
                OnStageFailed();
                yield break;
            }

            bool allWaveSpawned = waveManager.IsAllWaveSpawned();
            bool noEnemyRemain = !EnemyManager.instance.HasEnemy();
            bool bossDead = !EnemyManager.instance.HasBoss();

            if (hasBoss && bossDead)
            {
                isBossClear = true;
                isStageClear = true;
                OnStageClear();
                yield break;
            }

            if (!hasBoss && allWaveSpawned && noEnemyRemain)
            {
                isEnemyAllClear = true;
                isStageClear = true;
                OnStageClear();
                yield break;
            }

            if (!allWaveSpawned && noEnemyRemain)
            {
                waveManager.StartWave();
            }

            yield return new WaitForSeconds(1f);
        }
    }


    private void OnStageClear()
    {
    }

    private void OnStageFailed()
    {
        Debug.Log("TimeOver");
    }

    private void UpdateStageTimer()
    {
        if (isStageClear || isTimeOver) return;

        stageTimer -= Time.deltaTime;

        limitTime?.UpdateTimeDisplay(stageTimer);

        if (stageTimer <= 0f)
        {
            stageTimer = 0f;
            isTimeOver = true;
            OnStageFailed();
        }
    }

}
