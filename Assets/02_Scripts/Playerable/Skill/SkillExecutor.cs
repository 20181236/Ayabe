using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillExecutor: MonoBehaviour
{
    public GameObject caster; // ������

    public void OnSkillSelected(SkillData data)
    {
        SkillBase skill = SkillFactory.CreateSkill(data);

        SkillContext context = new SkillContext();
        context.Caster = caster;

        if(data.skillType == SkillType.PositionTarget)
        {
            // ��ġ ���� ��ų�̶�� UI���� ��ġ ���� �� ȣ���ϴ� ���� �޼��� �ʿ�
            StartCoroutine(WaitForPositionInputAndExecute(skill, context));
        }
        else
        {
            // ��� ���� ������ ��ų
            skill.Execute(context);
        }
    }

    private IEnumerator WaitForPositionInputAndExecute(SkillBase skill, SkillContext context)
    {
        Vector3 selectedPosition = Vector3.zero;
        bool positionSelected = false;

        // ��: UI���� ��ġ �Է� �޴� ���� (���콺 Ŭ�� ��)
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