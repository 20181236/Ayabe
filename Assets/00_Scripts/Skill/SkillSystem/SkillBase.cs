using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class SkillBase : MonoBehaviour, InterfaceSkill
{
    protected SkillData skillData;

    public virtual void Initialize(SkillData data)
    {
        skillData = data;
    }

    public abstract void Execute(SkillContext context);
}