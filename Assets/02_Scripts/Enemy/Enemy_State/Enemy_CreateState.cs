public class CreateState : IEnemyState
{
    public void EnterState(Enemy_base enemy)
    {
        // 상태 진입 시 초기화 작업
        enemy.Initialize();
    }

    public void UpdateState(Enemy_base enemy)
    {
        // 적이 초기화된 후, Idle 상태로 전환
        if (!enemy.isCreate)  // 적이 생성되었으면
        {
            enemy.currentState = EnemyState.Idle;
        }
    }

    public void ExitState(Enemy_base enemy)
    {
        // 상태 종료 시 작업 (필요한 경우 추가)
    }
}
