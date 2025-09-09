using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    public static StageManager instance { get; private set; }

    [Header("Stage Data")]
    [SerializeField] public StageData stageData;
    [SerializeField] public WaveManager waveManager;
    [SerializeField] public EnemyRemain enemyRemain;
    [SerializeField] public Timer limitTime;

    [Header("UI Manager")]
    [SerializeField] public StartAndResult startAndResult;

    [HideInInspector] public float battleTime = 0f;
    public float StartUIDuration = 2f;

    public bool hasBoss = false;

    private int totalEnemyCount = 0;
    private int remainingEnemyCount = 0;

    private GameStateInterface currentStageState;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        SetStageState(new StageStartingState());
    }

    private void Update()
    {
        currentStageState?.Update(this);
    }

    public void SetStageState(GameStateInterface newState)
    {
        currentStageState?.Exit(this);
        currentStageState = newState;
        currentStageState.Enter(this);
    }

    public void CountTotalEnemies()
    {
        totalEnemyCount = 0;
        foreach (var wave in stageData.waves)
            totalEnemyCount += wave.enemiesInWave.Length;

        remainingEnemyCount = totalEnemyCount;
        enemyRemain.UpdateEnemyCount(remainingEnemyCount, totalEnemyCount);
    }

    public void NotifyEnemyKilled()
    {
        remainingEnemyCount--;
        enemyRemain.UpdateEnemyCount(remainingEnemyCount, totalEnemyCount);
    }
}

//public class StageManager : MonoBehaviour
//{
//    public static StageManager instance { get; private set; }

//    [Header("Stage Data")]
//    [SerializeField] private StageData stageData;
//    [SerializeField] private WaveManager waveManager;
//    [SerializeField] private EnemyRemain enemyRemain;
//    [SerializeField] private Timer limitTime;

//    [Header("UI Manager")]
//    [SerializeField] private StartAndResult startAndResult; // 추가된 필드

//    [HideInInspector] public float battleTime = 0f;
//    private float stageTimer;
//    private bool isTimeOver = false;
//    public float StartUIDuration = 2f;

//    public bool hasBoss = false;
//    public bool isEnemyAllClear = false;
//    public bool isBossClear = false;
//    public bool isStageClear = false;

//    private int totalEnemyCount = 0;
//    private int remainingEnemyCount = 0;

//    private GameStateInterface currentStageState;

//    private void Awake()
//    {
//        if (instance == null) instance = this;
//        else Destroy(gameObject);
//    }

//    private void Start()
//    {
//        SetStageState(new StageStartingState());
//    }

//    private void Update()
//    {
//        currentStageState?.Update(this);
//        UpdateStageTimer();
//    }

//    public void SetStageState(GameStateInterface newState)
//    {
//        currentStageState?.Exit(this);
//        currentStageState = newState;
//        currentStageState.Enter(this);
//    }

//    public void StartStageGameplay()
//    {
//        waveManager.SetupWaves(stageData.waves);
//        CountTotalEnemies();
//        enemyRemain.UpdateEnemyCount(remainingEnemyCount, totalEnemyCount);

//        stageTimer = stageData.timeLimit;
//        battleTime = 0f;
//        isTimeOver = false;

//        StartCoroutine(CheckWaveProgressCoroutine());

//        // HUD 켜기
//        startAndResult.ShowStageStartUI(hasBoss);

//        // 플레이어 행동 허용
//        foreach (var player in PlayableManager.instance.GetPlayables())
//            player.EnableActions();
//    }

//    public void EndStageGameplay()
//    {
//        foreach (var player in PlayableManager.instance.GetPlayables())
//            player.DisableActions();
//    }

//    private void CountTotalEnemies()
//    {
//        totalEnemyCount = 0;
//        foreach (var wave in stageData.waves)
//            totalEnemyCount += wave.enemiesInWave.Length;

//        remainingEnemyCount = totalEnemyCount;
//    }

//    public void NotifyEnemyKilled()
//    {
//        remainingEnemyCount--;
//        enemyRemain.UpdateEnemyCount(remainingEnemyCount, totalEnemyCount);
//    }

//    private IEnumerator CheckWaveProgressCoroutine()
//    {
//        yield return new WaitUntil(() => waveManager.GetWaves().Length > 0);

//        bool bossSpawned = false;

//        while (currentStageState is StagePlayingState)
//        {
//            if (isTimeOver)
//            {
//                SetStageState(new StageEndState(StageState.Defeat));
//                yield break;
//            }

//            bool allWaveSpawned = waveManager.IsAllWaveSpawned();
//            bool noEnemyRemain = !EnemyManager.instance.HasEnemy();
//            bool bossDead = !EnemyManager.instance.HasBoss();
//            bool bossAlive = EnemyManager.instance.HasBoss();

//            // 보스가 처음 등장하면 Boss 상태로 전환
//            if (hasBoss && bossAlive && !bossSpawned)
//            {
//                bossSpawned = true;
//                Debug.Log("보스 등장!");
//                // 필요 시 상태 전환
//                // SetStageState(new StageBossState()); // 예시: Boss 상태
//            }

//            // 클리어 조건
//            if (hasBoss && bossDead)
//            {
//                OnBossDefeated();
//                yield break;
//            }

//            if (!hasBoss && allWaveSpawned && noEnemyRemain)
//            {
//                isEnemyAllClear = true;
//                isStageClear = true;
//                SetStageState(new StageEndState(StageState.Victory));
//                yield break;
//            }

//            if (!allWaveSpawned && noEnemyRemain)
//                waveManager.StartWave();

//            yield return new WaitForSeconds(1f);
//        }
//    }


//    public void UpdateStageTimer()
//    {
//        if (!(currentStageState is StagePlayingState) || isTimeOver) return;

//        stageTimer -= Time.deltaTime;
//        battleTime += Time.deltaTime;

//        limitTime?.UpdateTimeDisplay(stageTimer);

//        if (stageTimer <= 0f)
//        {
//            stageTimer = 0f;
//            isTimeOver = true;
//            SetStageState(new StageEndState(StageState.Defeat));
//        }
//    }

//    public IEnumerator PlayStartSequence()
//    {
//        yield return startAndResult.PlayStartSequence(StartUIDuration);
//        SetStageState(new StagePlayingState());
//    }

//    public void ShowEndGameUI(StageState state)
//    {
//        EndStageGameplay();
//        startAndResult.ShowUI(state, battleTime);
//    }
//    public void OnBossDefeated()
//    {
//        isBossClear = true;
//        isStageClear = true;

//        SetStageState(new StageEndState(StageState.Victory)); // 승리 처리
//    }
//}
