using System.Collections;
using UnityEngine;

public class LudoSkill : SkillBase
{
    private float skillRadius;
    private GameObject poisonGrenadePrefab;

    public override void Initialize(SkillData data)
    {
        base.Initialize(data);
        skillRadius = data.skillRadius;
        poisonGrenadePrefab = data.weaponPrefab;
    }

    public override void Execute(SkillContext context)
    {
        if (context.Caster == null)
        {
            Debug.LogError("[LudoSkill] 실행 실패: Caster가 null입니다.");
            return;
        }

        Debug.Log($"[LudoSkill] 캐스터 이름: {context.Caster.name}");
        Debug.Log($"[LudoSkill] 캐스터 위치: {context.Caster.transform.position}");
        Debug.Log($"[LudoSkill] 타겟 위치: {context.TargetPosition}");

        // 공통 이펙트, 애니메이션 처리
        SpawnCastEffect(context.Caster);
        HandleAnimation(context.Caster);

        // 포이즌 그레네이드 생성 및 설정
        SpawnGrenade(context);
    }

    private void SpawnGrenade(SkillContext context)
    {
        if (poisonGrenadePrefab == null)
        {
            Debug.LogError("[LudoSkill] poisonGrenadePrefab이 설정되지 않았습니다.");
            return;
        }

        Vector3 spawnPosition = context.Caster.transform.position + context.Caster.transform.forward * 1.0f;
        GameObject grenadeObject = GameObject.Instantiate(poisonGrenadePrefab, spawnPosition, Quaternion.identity);

        if (!grenadeObject.TryGetComponent(out PoisonGrenade grenade))
        {
            Debug.LogError("[LudoSkill] PoisonGrenade 컴포넌트를 찾을 수 없습니다.");
            return;
        }

        float attackPower = context.Caster.GetComponent<PlayableBase>()?.AttackPower ?? 0f;

        grenade.SetTarget(context.TargetPosition);
        grenade.SetAttackPower(attackPower);
        grenade.InitializeShooter(context.Caster);

        if (Mathf.Approximately(Time.timeScale, 0f))
        {
            grenade.SetIgnoreTimeScale(true);
        }

        Debug.Log("[LudoSkill] 포이즌 그레네이드 생성 및 설정 완료");
    }
}
