using System.Collections;
using System.Collections.Generic;
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
            Debug.LogError("LudoSkill 실행 실패: Caster가 null입니다.");
            return;
        }
        else
        {
            Debug.Log("Caster: " + context.Caster.name);
        }

        var playableBase = context.Caster.GetComponent<PlayableBase>();
        if (playableBase != null && playableBase.playableAnimator != null)
        {
            var animator = playableBase.playableAnimator;
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            animator.SetTrigger("doSkill");
            Debug.Log("애니메이션실행됨");
            SkillExecutor.instance.StartCoroutine(LogCurrentAnimationState(animator));
        }
        else
        {
            Debug.LogWarning("PlayableBase 또는 animator를 찾을 수 없습니다.");
        }

        Vector3 spawnPosition = context.Caster.transform.position + context.Caster.transform.forward * 1.0f;

        GameObject grenadeObject = GameObject.Instantiate(poisonGrenadePrefab, spawnPosition, Quaternion.identity);
        PoisonGrenade grenadeScript = grenadeObject.GetComponent<PoisonGrenade>();

        if (grenadeScript != null)
        {
            PlayableBase casterStats = context.Caster.GetComponent<PlayableBase>();
            float casterAttackPower = casterStats != null ? casterStats.AttackPower : 0f;

            grenadeScript.SetTarget(context.TargetPosition);
            grenadeScript.SetAttackPower(casterAttackPower);
            grenadeScript.InitializeShooter(context.Caster);

            bool isTimePaused = Mathf.Approximately(Time.timeScale, 0f);
            if (isTimePaused)
            {
                grenadeScript.SetIgnoreTimeScale(true);
            }
        }
    }
    private IEnumerator LogCurrentAnimationState(Animator animator)
    {
        yield return null;  // 한 프레임 기다리기

        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        Debug.Log($"현재 애니메이션 상태 이름 해시: {stateInfo.fullPathHash}, 길이: {stateInfo.length}, 진행 시간: {stateInfo.normalizedTime}");
    }
}

