using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillExecutor: MonoBehaviour
{
    public GameObject caster; // 시전자

    public void OnSkillSelected(SkillData data)
    {
        SkillBase skill = SkillFactory.CreateSkill(data);

        SkillContext context = new SkillContext();
        context.Caster = caster;

        if(data.skillType == SkillType.PositionTarget)
        {
            // 위치 지정 스킬이라면 UI에서 위치 선택 후 호출하는 별도 메서드 필요
            StartCoroutine(WaitForPositionInputAndExecute(skill, context));
        }
        else
        {
            // 즉시 실행 가능한 스킬
            skill.Execute(context);
        }
    }

    private IEnumerator WaitForPositionInputAndExecute(SkillBase skill, SkillContext context)
    {
        Vector3 selectedPosition = Vector3.zero;
        bool positionSelected = false;

        // 예: UI에서 위치 입력 받는 로직 (마우스 클릭 등)
        while (!positionSelected)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    selectedPosition = hit.point;
                    positionSelected = true;
                }
            }
            yield return null;
        }

        context.TargetPosition = selectedPosition;
        skill.Execute(context);
    }
}