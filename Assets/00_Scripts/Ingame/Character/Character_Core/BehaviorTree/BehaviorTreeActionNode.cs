using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorTreeActionNode : InterfaceBehaviorTreeNode
{
    private Func<BehaviorTreeState> action;

    public void Initialize(Func<BehaviorTreeState> action)
    {
        this.action = action;
    }

    public BehaviorTreeState Evaluate() => action?.Invoke() ?? BehaviorTreeState.Failed;
    //public BehaviorTreeState Evaluate()
    //{
    //    // action이 null이 아니면 실행하고, 반환값을 그대로 반환
    //    if (action != null)
    //    {
    //        return action.Invoke();
    //    }
    //    // action이 null이면 실패(Failed) 반환
    //    else
    //    {
    //        return BehaviorTreeState.Failed;
    //    }
    //}
}