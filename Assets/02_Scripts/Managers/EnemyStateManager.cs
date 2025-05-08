using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateManager : MonoBehaviour
{
    public static EnemyStateManager instance { get; private set; }

    private Enemy_base enemy;
    private IEnemyState currentState;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Initialize(Enemy_base enemy)
    {
        this.enemy = enemy;
        ChangeState(new CreateState());
    }

    public void ChangeState(IEnemyState newState)
    {
        currentState.ExitState(enemy);
        currentState = newState;
        currentState.EnterState(enemy);
    }

    public void Update()
    {
        currentState.UpdateState(enemy);
    }
}
