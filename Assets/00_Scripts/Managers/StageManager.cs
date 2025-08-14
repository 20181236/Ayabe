using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
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
    [SerializeField] private StartAndResult startAndResult;

    private int totalEnemyCount = 0;
    private int remainingEnemyCount = 0;

    private float battleTime = 0f; // 경과 시간 변수 추가
    private float stageTimer;
    private bool isTimeOver = false;
    [SerializeField] private Timer limitTime;

    private float startUIDuration = 2f;
    public StageState CurrentStageState { get; private set; } = StageState.None;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        CurrentStageState = StageState.Starting;
        BeginStageAfterUI();
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
        CurrentStageState = StageState.Victory;
        startAndResult.ShowUI(StageState.Victory, battleTime);
        //startAndResult.SetBattleTime(battleTime); // 경과 시간 전달
        //startAndResult.ShowVictory();
    }

    private void OnStageFailed()
    {
        CurrentStageState = StageState.Defeat;
        ScreenAndTimeEffectController.instance.StartEffect();
        StartCoroutine(EndClearEffectAfterDelay(2f));
        // 경과 시간 전달 + 결과 패널 표시
        startAndResult.ShowUI(StageState.Defeat, battleTime);
        //startAndResult.ShowDefeat();
    }
    private IEnumerator EndClearEffectAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        ScreenAndTimeEffectController.instance.EndEffect();
    }

    //private void UpdateStageTimer()
    //{
    //    if (isStageClear || isTimeOver) return;

    //    stageTimer -= Time.deltaTime;

    //    limitTime?.UpdateTimeDisplay(stageTimer);

    //    if (stageTimer <= 0f)
    //    {
    //        stageTimer = 0f;
    //        isTimeOver = true;
    //        OnStageFailed();
    //    }
    //}

    private void UpdateStageTimer()
    {
        if (isStageClear || isTimeOver) return;

        stageTimer -= Time.deltaTime;
        battleTime += Time.deltaTime; // 경과 시간도 누적

        limitTime?.UpdateTimeDisplay(stageTimer);

        if (stageTimer <= 0f)
        {
            stageTimer = 0f;
            isTimeOver = true;
            OnStageFailed();
        }
    }

    public void BeginStageAfterUI()
    {
        waveManager.SetupWaves(stageData.waves);

        CountTotalEnemies();
        enemyRemain.UpdateEnemyCount(remainingEnemyCount, totalEnemyCount);

        if (stageData != null)
            stageTimer = stageData.timeLimit;
        else
            Debug.LogError("StageData is null!");

        StartCoroutine(CheckWaveProgressCoroutine());

        StartCoroutine(StartSequenceCoroutine());
    }
    private IEnumerator StartSequenceCoroutine()
    {
        yield return startAndResult.PlayStartSequence(startUIDuration);

        // UI 연출이 끝난 후 상태를 Playing으로 설정
        CurrentStageState = StageState.Playing;
    }
}
