public class StageEndState : GameStateInterface
{
    private StageState endState;

    public StageEndState(StageState state)
    {
        endState = state;
    }

    public void Enter(StageManager manager)
    {
        // HUD와 전투 관련 UI 끄기
        manager.UIHUD.SetActive(false);
        manager.HealthBar.SetActive(false);
        manager.BossHealthBar.SetActive(false);

        // 승리/패배 패널 처리
        if (endState == StageState.Victory)
        {
            manager.VictoryPanel.SetActive(true);  // 승리 패널 켜기
            manager.DefeatPanel.SetActive(false);  // 패배 패널 끄기
        }
        else if (endState == StageState.Defeat)
        {
            manager.DefeatPanel.SetActive(true);   // 패배 패널 켜기
            manager.VictoryPanel.SetActive(false); // 승리 패널 끄기
        }
    }

    public void Update(StageManager manager)
    {
        // 별도 처리 필요 없으면 비워둬도 됨
    }

    public void Exit(StageManager manager)
    {
        // 필요 시 종료 시 처리
    }
}
