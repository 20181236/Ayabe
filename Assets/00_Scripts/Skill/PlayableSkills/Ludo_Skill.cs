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
        if (context.Caster == null)
        {
            Debug.LogError("Execute 실패: context.Caster가 null입니다.");
            return;
        }

        Vector3 spawnPos = context.Caster.transform.position + context.Caster.transform.forward * 1.0f;

        GameObject poisonGrenade = GameObject.Instantiate(
            poisonGrenadePrefab,
            spawnPos,
            Quaternion.identity);

        PoisonGrenade grenadeScript = poisonGrenade.GetComponent<PoisonGrenade>();

        if (grenadeScript != null)
        {
            var casterStats = context.Caster.GetComponent<PlayableBase>();
            float attackPower = casterStats != null ? casterStats.AttackPower : 0f;

            grenadeScript.SetTarget(context.TargetPosition);
            grenadeScript.SetAttackPower(attackPower);
        }
    }
    //public override void Execute(SkillContext context)
    //{
    //    Debug.Log("스킬 시전자: " + context.Caster?.name);
    //    Debug.Log("Caster 객체: " + context.Caster);
    //    Debug.Log("Caster 위치: " + (context.Caster != null ? context.Caster.transform.position.ToString() : "null"));
    //    // context.TargetPosition: 수류탄 떨어질 중심 위치
    //    Vector3 center = context.Caster.transform.position;
    //    //수류탄 불러와서 시전하는애 앞에 생성하고

    //    GameObject poisonGrenade = GameObject.Instantiate(
    //        poisonGrenadePrefab,
    //        context.Caster.transform.position,
    //        Quaternion.identity);

    //    PoisonGrenade grenadeScript = poisonGrenade.GetComponent<PoisonGrenade>();

    //    if (grenadeScript != null)
    //    {
    //        // 캐스터의 공격력 가져오기
    //        var caster = context.Caster.GetComponent<PlayableBase>();
    //        float attackPower = caster != null ? caster.AttackPower : 0f;

    //        grenadeScript.SetTarget(context.TargetPosition);
    //        grenadeScript.SetAttackPower(attackPower);
    //    }
    //}
}
