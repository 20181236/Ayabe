using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehaviorTree : MonoBehaviour
{
    private InterfaceBehaviorTreeNode rootNode;

    private Boss boss;

    private void Awake()
    {
        {
            boss = GetComponent<Boss>();
        }
        if (boss == null)
        {
            Debug.LogError("BossBehaviorTree가 붙어있는 객체에 EnemyBase가 없습니다.");
            return;
        }
        BuildBehaviorTree();
    }

    private void Update()
    {
        rootNode?.Evaluate();
    }

    public void BuildBehaviorTree()
    {
        // --- 1. 스킬 조건 & 액션 ---
        var skillCondition = new BehaviorTreeConditionNode();
        skillCondition.Initialize(() => boss.readySkill && !boss.isUsingSkill);

        var skillAction = new BehaviorTreeActionNode();
        skillAction.Initialize(() =>
        {
            if (!boss.isUsingSkill)
            {
                boss.StartCoroutine(boss.UseSkillBT()); // 스킬 코루틴 실행
            }
            return boss.isUsingSkill ? BehaviorTreeState.Running : BehaviorTreeState.Success;
        });

        var skillSequence = new BehaviorTreeSequenceNode();
        skillSequence.Initialize(new List<InterfaceBehaviorTreeNode> { skillCondition, skillAction });

        // --- 2. EX 스킬 조건 & 액션 ---
        var exSkillCondition = new BehaviorTreeConditionNode();
        exSkillCondition.Initialize(() => boss.readyExSkill && !boss.isUsingExSkill);

        var exSkillAction = new BehaviorTreeActionNode();
        exSkillAction.Initialize(() =>
        {
            if (!boss.isUsingExSkill)
            {
                boss.StartCoroutine(boss.UseExSkillBT());
            }
            return boss.isUsingExSkill ? BehaviorTreeState.Running : BehaviorTreeState.Success;
        });

        var exSkillSequence = new BehaviorTreeSequenceNode();
        exSkillSequence.Initialize(new List<InterfaceBehaviorTreeNode> { exSkillCondition, exSkillAction });

        // --- 3. 기본 공격 ---
        var attackCondition = new BehaviorTreeConditionNode();
        attackCondition.Initialize(() => boss.readyBasicAttack && boss.CurrentTarget != null);

        var attackAction = new BehaviorTreeActionNode();
        attackAction.Initialize(() =>
        {
            if (boss.CurrentTarget != null)
            {
                boss.ExecuteBasicAttack();
                return BehaviorTreeState.Success;
            }
            return BehaviorTreeState.Failed;
        });

        var attackSequence = new BehaviorTreeSequenceNode();
        attackSequence.Initialize(new List<InterfaceBehaviorTreeNode> { attackCondition, attackAction });

        // --- 4. 추적 ---
        var chaseAction = new BehaviorTreeActionNode();
        chaseAction.Initialize(() =>
        {
            if (boss.CurrentTarget != null)
            {
                boss.MoveToTarget(boss.CurrentTarget.transform.position);
                return BehaviorTreeState.Success;
            }
            return BehaviorTreeState.Failed;
        });

        // --- 5. 대기 ---
        var idleAction = new BehaviorTreeActionNode();
        idleAction.Initialize(() =>
        {
            // 대기 애니메이션 등
            return BehaviorTreeState.Success;
        });

        // --- 6. 루트 셀렉터 ---
        var rootSelector = new BehaviorTreeSelectorNode();
        rootSelector.Initialize(new List<InterfaceBehaviorTreeNode>
        {
            skillSequence,   // 스킬 우선
            exSkillSequence, // EX 스킬
            attackSequence,  // 기본 공격
            chaseAction,     // 타겟 추적
            idleAction       // 대기
        });

        rootNode = rootSelector;
    }
}
//public class BossBehaviorTree : BehaviorTreeBase
//{
//    private Boss boss;

//    private void Awake()
//    {
//        boss = GetComponent<Boss>();
//    }

//    public override void BuildBehaviorTree()
//    {
//        // 1. 스킬 사용 노드
//        var useSkillNode = new BehaviorTreeActionNode();
//        useSkillNode.Initialize(() =>
//        {
//            if (boss.readySkill && !boss.isUsingSkill)
//            {
//                Debug.Log("[BossBT] 스킬 실행");
//                boss.StartCoroutine(boss.UseSkillBT());
//                return BehaviorTreeState.Success;
//            }
//            return BehaviorTreeState.Failed;
//        });

//        // 2. EX 스킬 사용 노드
//        var useExSkillNode = new BehaviorTreeActionNode();
//        useExSkillNode.Initialize(() =>
//        {
//            if (boss.readyExSkill && !boss.isUsingExSkill)
//            {
//                Debug.Log("[BossBT] EX 스킬 실행");
//                boss.StartCoroutine(boss.UseExSkillBT());
//                return BehaviorTreeState.Success;
//            }
//            return BehaviorTreeState.Failed;
//        });

//        // 3. Idle 노드 (필요하면 추가)
//        var idleNode = new BehaviorTreeActionNode();
//        idleNode.Initialize(() =>
//        {
//            // 단순 대기 상태
//            return BehaviorTreeState.Success;
//        });

//        // 루트 셀렉터 (스킬 → EX스킬 → Idle)
//        var rootSelector = new BehaviorTreeSelectorNode();
//        rootSelector.Initialize(new List<InterfaceBehaviorTreeNode>
//        {
//            useSkillNode,
//            useExSkillNode,
//            idleNode
//        });

//        rootNode = rootSelector;
//    }
//}