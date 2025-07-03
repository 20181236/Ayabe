using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface InterfaceGameState
{
    void Enter();
    void Update();
    void Exit();
}
