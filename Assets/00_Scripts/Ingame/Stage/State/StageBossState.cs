using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageBossState : GameStateInterface
{
    private Boss boss;

    public void Enter(StageManager manager)
    {
        Debug.Log("보스 상태 진입");

        // UI는 StartAndResult에 위임
        manager.startAndResult.ShowStageStartUI(true);

        // 보스 개체 참조
        boss = GameObject.FindObjectOfType<Boss>();
        if (boss != null && boss.bossHpBar != null)
        {
            boss.bossHpBar.Show();
            boss.bossHpBar.SetHP((int)boss.CurrentHealth, (int)boss.MaxHealth);
        }

        // 음악, 효과 등 추가
        // AudioManager.Instance.PlayBGM("BossTheme");

        // 플레이어 제어 허용
        foreach (var player in PlayableManager.instance.GetPlayables())
            player.EnableActions();
    }

    public void Update(StageManager manager)
    {
        if (boss == null)
            return;

        if (!EnemyManager.instance.HasBoss())
        {
            manager.SetStageState(new StageEndState(StageState.Victory));
        }
    }

    public void Exit(StageManager manager)
    {
        if (boss != null && boss.bossHpBar != null)
        {
            boss.bossHpBar.Hide();
        }
    }
}

