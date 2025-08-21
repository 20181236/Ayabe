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

    protected virtual void HandleAnimation(GameObject caster, string triggerName = "doSkill")
    {
        var playable = caster.GetComponent<PlayableBase>();
        if (playable?.animator == null)
        {
            Debug.LogWarning("[SkillBase] PlayableBase 또는 Animator가 없습니다.");
            return;
        }

        Animator animator = playable.animator;
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        animator.SetTrigger(triggerName);
        Debug.Log("[SkillBase] 스킬 애니메이션 트리거 실행됨");

        SkillExecutor.instance.StartCoroutine(LogCurrentAnimationState(animator));
    }

    protected virtual void SpawnCastEffect(GameObject caster)
    {
        if (skillData.castEffectPrefab == null)
        {
            Debug.Log("[SkillBase] castEffectPrefab이 설정되지 않음");
            return;
        }

        Vector3 effectPos = caster.transform.position + Vector3.up * 1f;
        GameObject effect = GameObject.Instantiate(skillData.castEffectPrefab, effectPos, Quaternion.identity);

        ParticleSystem[] particleSystems = effect.GetComponentsInChildren<ParticleSystem>(true);
        foreach (var ps in particleSystems)
        {
            var main = ps.main;
            main.useUnscaledTime = true;
            ps.Play();
        }

        Debug.Log($"[SkillBase] 이펙트 생성됨: {effect.name} at {effectPos}");

        SkillExecutor.instance.StartCoroutine(DestroyAfterRealtime(effect, 1.5f));
    }

    private IEnumerator LogCurrentAnimationState(Animator animator)
    {
        yield return null;  // 1프레임 대기 후 애니메이션 상태 출력

        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        Debug.Log($"[SkillBase] 애니메이션 상태: 해시={stateInfo.fullPathHash}, 길이={stateInfo.length}, 진행률={stateInfo.normalizedTime}");
    }

    public static IEnumerator DestroyAfterRealtime(GameObject target, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        if (target != null)
        {
            GameObject.Destroy(target);
            Debug.Log($"[SkillBase] {target.name} 오브젝트 파괴됨");
        }
    }
}