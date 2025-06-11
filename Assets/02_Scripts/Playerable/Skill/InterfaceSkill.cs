using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SkillContext
{
    public GameObject Caster { get; set; }
    public GameObject Target { get; set; }  // ���� Ÿ��
    public Vector3 TargetPosition { get; set; }  // ���� ���ݿ� ��ġ
    public List<GameObject> Targets { get; set; }  // ���� Ÿ�� (����, ������)
    // �ʿ�� �߰� ������ �ֱ�
}
public interface InterfaceSkill
{
    void Execute(SkillContext context);
}

