public class CreateState : IEnemyState
{
    public void EnterState(Enemy_base enemy)
    {
        // ���� ���� �� �ʱ�ȭ �۾�
        enemy.Initialize();
    }

    public void UpdateState(Enemy_base enemy)
    {
        // ���� �ʱ�ȭ�� ��, Idle ���·� ��ȯ
        if (!enemy.isCreate)  // ���� �����Ǿ�����
        {
            enemy.currentState = EnemyState.Idle;
        }
    }

    public void ExitState(Enemy_base enemy)
    {
        // ���� ���� �� �۾� (�ʿ��� ��� �߰�)
    }
}
