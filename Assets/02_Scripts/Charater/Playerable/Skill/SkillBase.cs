using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class SkillBase : InterfaceSkill
{
    protected SkillData skillData;

    public SkillBase(SkillData data)
    {
        skillData = data;
    }

    public abstract void Execute(SkillContext context);
}