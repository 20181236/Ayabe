using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//OR 역할 자식 노드들을 왼쪽부터 순차 평가 < 평가가중요
public class BehaviorTreeSelectorNode : InterfaceBehaviorTreeNode
{
    private List<InterfaceBehaviorTreeNode> children = new List<InterfaceBehaviorTreeNode>();

    public void Initialize(List<InterfaceBehaviorTreeNode> nodes)
    {
        children = nodes;
    }

    public BehaviorTreeState Evaluate()
    {
        foreach (var child in children)
        {
            var state = child.Evaluate();
            switch (state)
            {
                // 수정된 핵심 로직: 성공이나 실행 중일 때 즉시 멈춤
                case BehaviorTreeState.Success:
                    return BehaviorTreeState.Success;
                case BehaviorTreeState.Running:
                    return BehaviorTreeState.Running;
                case BehaviorTreeState.Failed:
                    continue;
            }
        }
        return BehaviorTreeState.Failed;
    }
}