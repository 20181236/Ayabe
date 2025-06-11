using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SkillContext
{
    public GameObject Caster { get; set; }
    public GameObject Target { get; set; }  // 단일 타겟
    public Vector3 TargetPosition { get; set; }  // 범위 공격용 위치
    public List<GameObject> Targets { get; set; }  // 다중 타겟 (광역, 버프용)
    // 필요시 추가 정보도 넣기
}
public interface InterfaceSkill
{
    void Execute(SkillContext context);
}

