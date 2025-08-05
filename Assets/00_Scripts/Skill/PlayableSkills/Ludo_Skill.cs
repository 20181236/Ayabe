using System.Collections;
using UnityEngine;

public class LudoSkill : SkillBase
{
    private float skillRadius;
    public GameObject poisonGrenadePrefab;

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

        HandleAnimation(context.Caster);
        SpawnGrenade(context);
        SpawnCastEffect(context);
    }

    private void HandleAnimation(GameObject caster)
    {
        var playable = caster.GetComponent<PlayableBase>();
        if (playable?.playableAnimator == null)
        {
            Debug.LogWarning("[LudoSkill] PlayableBase 또는 Animator가 없습니다.");
            return;
        }

        Animator animator = playable.playableAnimator;
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        animator.SetTrigger("doSkill");
        Debug.Log("[LudoSkill] 스킬 애니메이션 트리거 실행됨");

        SkillExecutor.instance.StartCoroutine(LogCurrentAnimationState(animator));
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

    private void SpawnCastEffect(SkillContext context)
    {
        if (skillData.castEffectPrefab == null)
        {
            Debug.Log("[LudoSkill] castEffectPrefab이 설정되지 않음");
            return;
        }

        Vector3 effectPos = context.Caster.transform.position + Vector3.up * 1f;
        GameObject effect = GameObject.Instantiate(skillData.castEffectPrefab, effectPos, Quaternion.identity);

        Debug.Log($"[LudoSkill] 이펙트 생성됨: {effect.name} at {effectPos}");

        SkillExecutor.instance.StartCoroutine(DestroyAfterRealtime(effect, 1.5f));
    }

    private IEnumerator LogCurrentAnimationState(Animator animator)
    {
        yield return null;  // 1프레임 대기 후 애니메이션 상태 출력

        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        Debug.Log($"[LudoSkill] 애니메이션 상태: 해시={stateInfo.fullPathHash}, 길이={stateInfo.length}, 진행률={stateInfo.normalizedTime}");
    }

    public static IEnumerator DestroyAfterRealtime(GameObject target, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        if (target != null)
        {
            GameObject.Destroy(target);
            Debug.Log($"[LudoSkill] {target.name} 오브젝트 파괴됨");
        }
    }
}


