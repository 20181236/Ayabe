using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetAttackSkill : ISkill
{
    public void Execute(GameObject caster, GameObject target)
    {
        Debug.Log($"{caster.name} uses Target Attack on {target.name}");
        // ���⿡ ������ ���� ���� �� ����
    }
}
