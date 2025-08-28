//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class ChaseState : MonoBehaviour, PlayableStateInterface
//{
//    private PlayableBase playable;

//    public void Enter(PlayableBase p)
//    {
//        playable = p;
//        playable.animator.SetBool("isChase", true);
//    }

//    public void Update()
//    {
//        if (playable.currentTarget != null)
//        {
//            playable.MoveToTarget(playable.currentTarget.transform.position);

//            // 타겟이 공격 범위 안에 들어오면 Attack 상태로 전환
//            if (playable.HasTargetInRange())
//            {
//                playable.ChangeState(new AttackState());
//            }
//        }
//    }

//    public void Exit()
//    {
//        playable.animator.SetBool("isChase", false);
//    }
//}
