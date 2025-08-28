//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class AttackState : MonoBehaviour
//{
//    private PlayableBase playable;
//    private PlayableAttackState currentAttackState;

//    public void Enter(PlayableBase p)
//    {
//        playable = p;
//        currentAttackState = PlayableAttackState.BasicAttack;
//        playable.animator.SetBool("isAttack", true);
//    }

//    public void Update()
//    {
//        switch (currentAttackState)
//        {
//            case PlayableAttackState.BasicAttack:
//                if (playable.readyBasicAttack)
//                {
//                    playable.BasicAttack();
//                }
//                break;
//            case PlayableAttackState.Skill:
//                if (playable.readySkill)
//                {
//                    playable.Skill();
//                }
//                break;
//            case PlayableAttackState.ExSkill:
//                if (playable.readyExSkill)
//                {
//                    playable.ExSkill();
//                }
//                break;
//            case PlayableAttackState.Reload:
//                playable.Reload();
//                break;
//        }

//        // 공격 대상이 범위를 벗어나면 Chase 상태로 전환
//        if (!playable.HasTargetInRange())
//        {
//            playable.ChangeState(new ChaseState());
//        }
//    }

//    public void Exit()
//    {
//        playable.animator.SetBool("isAttack", false);
//    }

//    public void SetAttackState(PlayableAttackState state)
//    {
//        currentAttackState = state;
//    }
//}
