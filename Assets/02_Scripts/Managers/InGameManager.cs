using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameManager : MonoBehaviour
{
    public enum InGameState
    {
        Prepare,
        Battle,
        End
    }
    public InGameState inGameState;

    void StartGame()
    {
        inGameState = InGameState.Battle;
    }

    void EndGame()
    {
        inGameState = InGameState.End;
    }
}
