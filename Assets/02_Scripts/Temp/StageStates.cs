using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageStartState : InterfaceGameState
{
    private StageManager stageManager;

    public StageStartState(StageManager manager)
    {
        this.stageManager = manager;
    }

    public void Enter()
    {
        Debug.Log("Stage Start");
        WaveManager.instance.StartFirstWave();
    }

    public void Update()
    {
        stageManager.SetState(new StagePlayingState(stageManager));
    }

    public void Exit() { }
}

public class StagePlayingState : InterfaceGameState
{
    private StageManager stageManager;
    private EnemySpawnState enemySpawnState = EnemySpawnState.None;

    public StagePlayingState(StageManager manager)
    {
        this.stageManager = manager;
    }

    public void Enter()
    {
        stageManager.StartCoroutine(CheckProgress());
    }

    public void Update() { }

    public void Exit() { }

    private IEnumerator CheckProgress()
    {
        while (true)
        {
            //패배
            if (!PlayableManager.instance.HasPlayable())
            {
                stageManager.SetState(new StageLoseState(stageManager));
                yield break;
            }

            // 현재 웨이브가 모두 스폰됐고, 적이 없으면 다음 웨이브 시작
            if (WaveManager.instance.IsAllWaveSpawned() == false &&
                EnemyManager.instance.HasEnemy() == false)
            {
                WaveManager.instance.StartWave();
            }

            // 모든 웨이브 스폰 완료 + 적 없음 + 보스 없음 → 스테이지 클리어
            if (WaveManager.instance.IsAllWaveSpawned() &&
                !EnemyManager.instance.HasEnemy() &&
                !EnemyManager.instance.HasBoss())
            {
                stageManager.SetState(new StageClearState(stageManager));
                yield break;
            }

            yield return new WaitForSeconds(1f);
        }
    }
}

public class StageClearState : InterfaceGameState
{
    private StageManager stageManager;

    public StageClearState(StageManager manager)
    {
        this.stageManager = manager;
    }

    public void Enter()
    {
        Debug.Log("Stage Clear!");
        // 결과창 출력 등
    }

    public void Update() { }

    public void Exit() { }
}

public class StageLoseState : InterfaceGameState
{
    private StageManager stageManager;

    public StageLoseState(StageManager manager)
    {
        this.stageManager = manager;
    }

    public void Enter()
    {
        Debug.Log("Stage Failed!");
        // 실패 처리 로직 (예: 결과창 출력, 재시작 버튼 등)
        Time.timeScale = 0; // 일시정지
    }

    public void Update() { }

    public void Exit() { }
}
