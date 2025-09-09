using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageStartingState : GameStateInterface
{
    public void Enter(StageManager manager)
    {
        manager.StartCoroutine(StartSequence(manager));
    }

    private IEnumerator StartSequence(StageManager manager)
    {
        yield return manager.startAndResult.PlayStartSequence(manager.StartUIDuration);
        manager.SetStageState(new StagePlayingState());
    }

    public void Update(StageManager manager)
    {
        // 시작 상태에서는 특별한 Update 없음
    }

    public void Exit(StageManager manager)
    {
        // 시작 UI 종료 시점에 추가 처리 가능
    }
}

