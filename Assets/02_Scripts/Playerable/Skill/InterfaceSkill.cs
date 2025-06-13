using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SkillContext
{
    public GameObject Caster { get; set; }
    public GameObject Target { get; set; }
    public Vector3 TargetPosition { get; set; }
    public List<GameObject> Targets { get; set; }

    public Vector3 WorldPosition;
}
public interface InterfaceSkill
{
    void Execute(SkillContext context);
}

