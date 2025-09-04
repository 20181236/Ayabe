using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageStartingState : GameStateInterface
{
    public void Enter(StageManager manager)
    {
        // HUD는 꺼진 상태
        manager.UIHUD.SetActive(false);
        manager.HealthBar.SetActive(false);
        manager.BossHealthBar.SetActive(false);

        manager.StartCoroutine(manager.PlayStartSequence());
    }

    public void Update(StageManager manager)
    {
        // 시작 UI는 PlayStartSequence 코루틴에서 처리
    }

    public void Exit(StageManager manager)
    {
        // 필요시 종료 처리
    }
}
