public class StageEndState : GameStateInterface
{
    private StageState result;

    public StageEndState(StageState result)
    {
        this.result = result;
    }

    public void Enter(StageManager manager)
    {
        manager.startAndResult.ShowUI(result, manager.battleTime);
    }

    public void Update(StageManager manager)
    {
        // 엔드 상태에서는 특별히 업데이트할 필요 없음
    }

    public void Exit(StageManager manager)
    {
        // End UI 닫기 시점에 필요하면 처리
    }
}
