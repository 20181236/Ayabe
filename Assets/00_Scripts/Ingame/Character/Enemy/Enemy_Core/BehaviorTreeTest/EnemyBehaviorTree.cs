using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviorTree : BehaviorTreeBase
{
    public EnemyBase enemy;

    private void Awake()
    {
        enemy = GetComponent<EnemyBase>();
        BuildBehaviorTree();
    }

    public override void BuildBehaviorTree()
    {
        // 1. 가장 말단에 있는 노드들(Actions, Conditions)을 먼저 생성합니다.
        var hasTargetNode = new BehaviorTreeConditionNode();
        var isTargetInRangeNode = new BehaviorTreeConditionNode();
        var basicAttackNode = new BehaviorTreeActionNode();
        var chaseTargetNode = new BehaviorTreeActionNode();
        var idleNode = new BehaviorTreeActionNode();

        // 2. 생성된 말단 노드들을 Initialize 합니다. (람다식으로 로직 연결)
        hasTargetNode.Initialize(() => EnemyConditions.HasTarget(enemy));
        isTargetInRangeNode.Initialize(() => EnemyConditions.IsTargetInAttackRange(enemy));
        basicAttackNode.Initialize(() => EnemyActions.BasicAttack(enemy));
        chaseTargetNode.Initialize(() => EnemyActions.ChaseTarget(enemy));
        idleNode.Initialize(() => EnemyActions.Idle(enemy));

        // 3. 중간 계층의 노드들(Sequences, Selectors)을 생성하고 Initialize 합니다.
        // (공격 시퀀스: 사거리 체크 -> 공격)
        var attackSequence = new BehaviorTreeSequenceNode();
        attackSequence.Initialize(new List<InterfaceBehaviorTreeNode>
        {
            isTargetInRangeNode,
            basicAttackNode
        });

        // (공격/추격 선택: 공격 시퀀스 또는 추격)
        var attackOrChaseSelector = new BehaviorTreeSelectorNode();
        attackOrChaseSelector.Initialize(new List<InterfaceBehaviorTreeNode>
        {
            attackSequence,
            chaseTargetNode
        });

        // (타겟 존재 시 행동 시퀀스: 타겟 체크 -> 공격/추격 선택)
        var mainActionSequence = new BehaviorTreeSequenceNode();
        mainActionSequence.Initialize(new List<InterfaceBehaviorTreeNode>
        {
            hasTargetNode,
            attackOrChaseSelector
        });

        // 4. 최상위 루트 노드를 생성하고 Initialize 합니다.
        var rootSelector = new BehaviorTreeSelectorNode();
        rootSelector.Initialize(new List<InterfaceBehaviorTreeNode>
        {
            mainActionSequence,
            idleNode
        });

        // 5. 완성된 트리를 rootNode에 할당합니다.
        rootNode = rootSelector;
    }
}