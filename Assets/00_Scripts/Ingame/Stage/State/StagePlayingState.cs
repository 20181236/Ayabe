using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StagePlayingState : GameStateInterface
{
    private float stageTimer;
    private bool bossSpawned = false;

    public void Enter(StageManager manager)
    {
        manager.waveManager.SetupWaves(manager.stageData.waves);
        manager.CountTotalEnemies();

        stageTimer = manager.stageData.timeLimit;
        manager.battleTime = 0f;

        // HUD 활성화
        manager.startAndResult.ShowStageStartUI(manager.hasBoss);

        // 플레이어 활성화
        foreach (var player in PlayableManager.instance.GetPlayables())
            player.EnableActions();
    }

    public void Update(StageManager manager)
    {
        // 타이머 갱신
        stageTimer -= Time.deltaTime;
        manager.battleTime += Time.deltaTime;
        manager.limitTime?.UpdateTimeDisplay(stageTimer);

        if (stageTimer <= 0f)
        {
            manager.SetStageState(new StageEndState(StageState.Defeat));
            return;
        }

        // 진행 상황 확인
        bool allWaveSpawned = manager.waveManager.IsAllWaveSpawned();
        bool noEnemyRemain = !EnemyManager.instance.HasEnemy();
        bool bossDead = !EnemyManager.instance.HasBoss();
        bool bossAlive = EnemyManager.instance.HasBoss();

        if (manager.hasBoss && bossAlive && !bossSpawned)
        {
            bossSpawned = true;
            Debug.Log("보스 등장!");
             manager.SetStageState(new StageBossState());
        }

        if (manager.hasBoss && bossDead)
        {
            manager.SetStageState(new StageEndState(StageState.Victory));
            return;
        }

        if (!manager.hasBoss && allWaveSpawned && noEnemyRemain)
        {
            manager.SetStageState(new StageEndState(StageState.Victory));
            return;
        }

        if (!allWaveSpawned && noEnemyRemain)
            manager.waveManager.StartWave();
    }

    public void Exit(StageManager manager)
    {
        foreach (var player in PlayableManager.instance.GetPlayables())
            player.DisableActions();
    }
}
