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
    [SerializeField] private StageData stageData;
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private EnemyRemain enemyRemain;
    [SerializeField] private Timer limitTime;

    [HideInInspector] public float battleTime = 0f;
    private float stageTimer;
    private bool isTimeOver = false;
    public float StartUIDuration = 2f;

    public bool hasBoss = false;
    public bool isEnemyAllClear = false;
    public bool isBossClear = false;
    public bool isStageClear = false;

    private int totalEnemyCount = 0;
    private int remainingEnemyCount = 0;

    private GameStateInterface currentStageState;

    #region UI
    [Header("UI")]
    [SerializeField] private GameObject darkOverlay;

    [SerializeField] private GameObject startImage;

    [SerializeField] private GameObject resultPanel;
    //[SerializeField] private Image resultImage;

    [SerializeField] private GameObject victoryPanel;
    //[SerializeField] private Sprite victorySprite;

    [SerializeField] private GameObject defeatPanel;
    //[SerializeField] private Sprite defeatSprite;

    //[SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI battleTimeText;

    [SerializeField] private Button okButton;

    [SerializeField] private GameObject healthBar;
    [SerializeField] private GameObject bossHealthBar;
    [SerializeField] private GameObject uiHUD;
    [SerializeField] private GameObject skillCanvas;
    #endregion

    // 외부에서 접근 가능한 프로퍼티 추가
    public GameObject UIHUD => uiHUD;
    public GameObject HealthBar => healthBar;
    public GameObject BossHealthBar => bossHealthBar;
    public GameObject VictoryPanel => victoryPanel;
    public GameObject DefeatPanel => defeatPanel;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        // UI 초기 비활성화
        darkOverlay.SetActive(false);
        startImage.SetActive(false);
        resultPanel.SetActive(false);
        okButton.gameObject.SetActive(false);
        uiHUD.SetActive(false);
        healthBar.SetActive(false);
        bossHealthBar.SetActive(false);
    }

    private void Start()
    {
        SetStageState(new StageStartingState());
    }

    private void Update()
    {
        currentStageState?.Update(this);
        UpdateStageTimer();
    }

    public void SetStageState(GameStateInterface newState)
    {
        currentStageState?.Exit(this);
        currentStageState = newState;
        currentStageState.Enter(this);
    }

    public void StartStageGameplay()
    {
        waveManager.SetupWaves(stageData.waves);
        CountTotalEnemies();
        enemyRemain.UpdateEnemyCount(remainingEnemyCount, totalEnemyCount);

        stageTimer = stageData.timeLimit;
        battleTime = 0f;
        isTimeOver = false;

        StartCoroutine(CheckWaveProgressCoroutine());

        // HUD 켜기
        uiHUD.SetActive(true);
        healthBar.SetActive(true);
        bossHealthBar.SetActive(hasBoss);

        // 플레이어 행동 허용
        foreach (var player in PlayableManager.instance.GetPlayables())
            player.EnableActions();
    }

    public void EndStageGameplay()
    {
        // 플레이어 행동 제한
        foreach (var player in PlayableManager.instance.GetPlayables())
            player.DisableActions();
    }

    private void CountTotalEnemies()
    {
        totalEnemyCount = 0;
        foreach (var wave in stageData.waves)
            totalEnemyCount += wave.enemiesInWave.Length;

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

        while (currentStageState is StagePlayingState)
        {
            if (isTimeOver)
            {
                SetStageState(new StageEndState(StageState.Defeat));
                yield break;
            }

            bool allWaveSpawned = waveManager.IsAllWaveSpawned();
            bool noEnemyRemain = !EnemyManager.instance.HasEnemy();
            bool bossDead = !EnemyManager.instance.HasBoss();

            if (hasBoss && bossDead)
            {
                isBossClear = true;
                isStageClear = true;
                SetStageState(new StageEndState(StageState.Victory));
                yield break;
            }

            if (!hasBoss && allWaveSpawned && noEnemyRemain)
            {
                isEnemyAllClear = true;
                isStageClear = true;
                SetStageState(new StageEndState(StageState.Victory));
                yield break;
            }

            if (!allWaveSpawned && noEnemyRemain)
                waveManager.StartWave();

            yield return new WaitForSeconds(1f);
        }
    }

    public void UpdateStageTimer()
    {
        if (!(currentStageState is StagePlayingState) || isTimeOver) return;

        stageTimer -= Time.deltaTime;
        battleTime += Time.deltaTime;

        limitTime?.UpdateTimeDisplay(stageTimer);

        if (stageTimer <= 0f)
        {
            stageTimer = 0f;
            isTimeOver = true;
            SetStageState(new StageEndState(StageState.Defeat));
        }
    }

    // StageStartingState에서 호출
    public IEnumerator PlayStartSequence()
    {
        darkOverlay.SetActive(true);
        startImage.SetActive(true);

        yield return new WaitForSeconds(StartUIDuration);

        startImage.SetActive(false);
        darkOverlay.SetActive(false);

        SetStageState(new StagePlayingState());
    }

    // StageEndState에서 UI 처리
    public void ShowEndGameUI(StageState state)
    {
        EndStageGameplay();

        uiHUD.SetActive(false);
        healthBar.SetActive(false);
        bossHealthBar.SetActive(false);
        darkOverlay.SetActive(true);

        resultPanel.SetActive(true);
        //resultImage.sprite = state == StageState.Victory ? victorySprite : defeatSprite;
        //resultText.text = state == StageState.Victory ? "Win!" : "Defeat...";
        battleTimeText.text = $"소요 시간: {FormatTime(battleTime)}";
        //resultImage.gameObject.SetActive(true);

        StartCoroutine(HideResultImageAfterDelay());
    }

    private IEnumerator HideResultImageAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        //resultImage.gameObject.SetActive(false);
        okButton.gameObject.SetActive(true);
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return $"{minutes:00}:{seconds:00}";
    }
}

//public class StageManager : MonoBehaviour
//{
//    public static StageManager instance { get; private set; }

//    private GameStateInterface currentState;

//    public bool hasBoss = false;
//    public bool isEnemyAllClear = false;
//    public bool isBossClear = false;
//    public bool isStageClear = false;

//    [SerializeField] private StageData stageData;
//    [SerializeField] private WaveManager waveManager;
//    [SerializeField] private EnemyRemain enemyRemain;
//    [SerializeField] private StartAndResult startAndResult;

//    private int totalEnemyCount = 0;
//    private int remainingEnemyCount = 0;

//    private float battleTime = 0f; // 경과 시간 변수 추가
//    private float stageTimer;
//    private bool isTimeOver = false;
//    [SerializeField] private Timer limitTime;

//    private float startUIDuration = 2f;
//    public StageState CurrentStageState { get; private set; } = StageState.None;

//    private void Awake()
//    {
//        if (instance == null)
//            instance = this;
//        else
//            Destroy(gameObject);
//    }

//    private void Start()
//    {
//        CurrentStageState = StageState.Starting;
//        BeginStageAfterUI();
//    }

//    private void Update()
//    {
//        currentState?.Update();
//        UpdateStageTimer();
//    }
//    private void LateUpdate()
//    {

//    }

//    public void SetState(GameStateInterface newState)
//    {
//        currentState?.Exit();
//        currentState = newState;
//        currentState.Enter();
//    }
//    private void CountTotalEnemies()
//    {
//        foreach (var wave in stageData.waves)
//        {
//            totalEnemyCount += wave.enemiesInWave.Length;
//        }
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

//        while (!isStageClear)
//        {
//            if (isTimeOver)
//            {
//                OnStageFailed();
//                yield break;
//            }

//            bool allWaveSpawned = waveManager.IsAllWaveSpawned();
//            bool noEnemyRemain = !EnemyManager.instance.HasEnemy();
//            bool bossDead = !EnemyManager.instance.HasBoss();

//            if (hasBoss && bossDead)
//            {
//                isBossClear = true;
//                isStageClear = true;
//                OnStageClear();
//                yield break;
//            }

//            if (!hasBoss && allWaveSpawned && noEnemyRemain)
//            {
//                isEnemyAllClear = true;
//                isStageClear = true;
//                OnStageClear();
//                yield break;
//            }

//            if (!allWaveSpawned && noEnemyRemain)
//            {
//                waveManager.StartWave();
//            }

//            yield return new WaitForSeconds(1f);
//        }
//    }

//    private void OnStageClear()
//    {
//        CurrentStageState = StageState.Victory;
//        startAndResult.ShowUI(StageState.Victory, battleTime);
//        startAndResult.SetBattleTime(battleTime); // 경과 시간 전달
//        startAndResult.ShowVictory();
//    }

//    private void OnStageFailed()
//    {
//        CurrentStageState = StageState.Defeat;
//        ScreenAndTimeEffectController.instance.StartEffect();
//        StartCoroutine(EndClearEffectAfterDelay(2f));
//         경과 시간 전달 + 결과 패널 표시
//        startAndResult.ShowUI(StageState.Defeat, battleTime);
//        startAndResult.ShowDefeat();
//    }
//    private IEnumerator EndClearEffectAfterDelay(float delay)
//    {
//        yield return new WaitForSecondsRealtime(delay);
//        ScreenAndTimeEffectController.instance.EndEffect();
//    }

//    private void UpdateStageTimer()
//    {
//        if (isStageClear || isTimeOver) return;

//        stageTimer -= Time.deltaTime;
//        battleTime += Time.deltaTime; // 경과 시간도 누적

//        limitTime?.UpdateTimeDisplay(stageTimer);

//        if (stageTimer <= 0f)
//        {
//            stageTimer = 0f;
//            isTimeOver = true;
//            OnStageFailed();
//        }
//    }

//    public void BeginStageAfterUI()
//    {
//        waveManager.SetupWaves(stageData.waves);    

//        CountTotalEnemies();
//        enemyRemain.UpdateEnemyCount(remainingEnemyCount, totalEnemyCount);

//        if (stageData != null)
//            stageTimer = stageData.timeLimit;
//        else
//            Debug.LogError("StageData is null!");

//        StartCoroutine(CheckWaveProgressCoroutine());

//        StartCoroutine(StartSequenceCoroutine());
//    }
//    private IEnumerator StartSequenceCoroutine()
//    {
//        yield return startAndResult.PlayStartSequence(startUIDuration);

//         UI 연출이 끝난 후 상태를 Playing으로 설정
//        CurrentStageState = StageState.Playing;
//    }
//}
