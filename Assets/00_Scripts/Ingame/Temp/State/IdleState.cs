//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class IdleState : MonoBehaviour, PlayableStateInterface
//{
//    private PlayableBase playable;

//    public void Enter(PlayableBase p)
//    {
//        playable = p;
//        playable.animator.SetBool("isIdle", true);
//    }

//    public void Update()
//    {
//        // 타겟이 범위 안에 들어오면 Attack 상태로 전환
//        if (playable.HasTargetInRange())
//        {
//            playable.ChangeState(new AttackState());
//        }
//    }

//    public void Exit()
//    {
//        playable.animator.SetBool("isIdle", false);
//    }
//}
