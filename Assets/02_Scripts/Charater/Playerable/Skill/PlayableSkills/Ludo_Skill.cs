using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LudoSkill : SkillBase
{
    private float skillRadius;

    public GameObject poisonGrenadePrefab;

    public LudoSkill(SkillData data) : base(data)
    {
        skillRadius = data.skillRadius;
        poisonGrenadePrefab = data.weaponPrefab;  // 여기서 프리팹 가져옴
    }

    public override void Execute(SkillContext context)
    {
        // context.TargetPosition: 수류탄 떨어질 중심 위치
        Vector3 center = context.TargetPosition;
        //수류탄 불러와서 시전하는애 앞에 생성하고
        GameObject poisonGrenade = GameObject.Instantiate(
            poisonGrenadePrefab,
            context.Caster.transform.position,
            Quaternion.identity);

        PoisonGrenade grenadeScript = poisonGrenade.GetComponent<PoisonGrenade>();

        if (grenadeScript != null)
        {
            // 캐스터의 공격력 가져오기
            var caster = context.Caster.GetComponent<PlayableBase>();
            float attackPower = caster != null ? caster.AttackPower : 0f;

            grenadeScript.SetTarget(context.TargetPosition);
        }
    }
}
