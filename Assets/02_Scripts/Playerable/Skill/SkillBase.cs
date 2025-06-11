using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class SkillBase : MonoBehaviour, InterfaceSkill
{
    public SkillData skillData;
    public Action<GameObject> OnSkillExecute;

    public abstract void SkillExecute(GameObject caster, GameObject target);
}