using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ludo_Skill : SkillBase
{
    private float skillRadius;

    public Ludo_Skill(SkillData data) : base(data)
    {
        skillRadius = data.skillRadius;
    }

    public override void Execute(SkillContext context)
    {

    }
}
