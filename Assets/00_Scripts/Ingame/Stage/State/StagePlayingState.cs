using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StagePlayingState : GameStateInterface
{
    public void Enter(StageManager manager)
    {
        // 게임 플레이 시작
        manager.StartStageGameplay();
    }

    public void Update(StageManager manager)
    {
        // 타이머와 진행 상황은 StageManager에서 UpdateStageTimer(), CheckWaveProgressCoroutine()으로 처리
    }

    public void Exit(StageManager manager)
    {
        // 게임 플레이 종료 처리
        manager.EndStageGameplay();
    }
}
