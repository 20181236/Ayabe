using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorTreeConditionNode : InterfaceBehaviorTreeNode
{
    private Func<bool> condition;

    public void Initialize(Func<bool> condition)
    {
        this.condition = condition;
    }

    public BehaviorTreeState Evaluate() => condition() ? BehaviorTreeState.Success : BehaviorTreeState.Failed;
}