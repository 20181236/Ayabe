using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class BehaviorTreeBase : MonoBehaviour
{
    protected InterfaceBehaviorTreeNode rootNode;

    public abstract void BuildBehaviorTree();

    protected virtual void Update()
    {
        rootNode?.Evaluate();
    }
}