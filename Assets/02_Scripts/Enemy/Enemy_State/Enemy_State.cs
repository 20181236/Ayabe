using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyState
{
    void EnterState(Enemy_base enemy);
    void UpdateState(Enemy_base enemy);
    void ExitState(Enemy_base enemy);
}