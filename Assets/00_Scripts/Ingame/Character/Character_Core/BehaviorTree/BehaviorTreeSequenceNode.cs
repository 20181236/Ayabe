using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//하나라도 실패 failed and와 같은 보통 셀렉아래에 왼쪽부터 순차 실행<실행이중요
public class BehaviorTreeSequenceNode : InterfaceBehaviorTreeNode
{
    private List<InterfaceBehaviorTreeNode> children = new List<InterfaceBehaviorTreeNode>();

    // Initialize 메서드로 자식 노드 리스트를 받음
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
                // 수정된 핵심 로직: 실패나 실행 중일 때 즉시 멈춤
                case BehaviorTreeState.Failed:
                    return BehaviorTreeState.Failed;
                case BehaviorTreeState.Running:
                    return BehaviorTreeState.Running;
                case BehaviorTreeState.Success:
                    continue;
            }
        }
        return BehaviorTreeState.Success;
    }
}
