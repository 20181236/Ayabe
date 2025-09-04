using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface GameStateInterface
{
    void Enter(StageManager stageManager);
    void Update(StageManager stageManager);
    void Exit(StageManager stageManager);
}
