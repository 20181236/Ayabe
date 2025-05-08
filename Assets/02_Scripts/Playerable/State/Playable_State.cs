using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayableState
{
    void EnterState(Playable_base playable);
    void UpdateState(Playable_base playable);
    void ExitState(Playable_base playable);
}
