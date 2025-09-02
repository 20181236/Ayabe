using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LudoPlayable : PlayableBase
{
    public GameObject grenadePrefab;

    protected override void Skill()
    {
        if (currentTarget == null || currentTarget.isDead || isAttacking)
            return;

        // CharacterBase.ObjectType으로 타입을 비교합니다.
        if (currentTarget.ObjectType == this.ObjectType)
            return;

        // 스킬 실행을 코루틴에 위임
        StartCoroutine(SkillCoroutine());
    }

    private IEnumerator SkillCoroutine()
    {
        // --- 스킬 시작 ---
        isAttacking = true;
        isSkill = true;
        skillTimer = 0;
        readySkill = false;

        // 여기에 스킬 시전 애니메이션을 재생하는 코드를 넣을 수 있습니다.
        // animator.SetTrigger("doSkill");
        // float animLength = animator.GetCurrentAnimatorStateInfo(0).length;

        // --- 수류탄 투척 ---
        GameObject grenadeObject = Instantiate(
            grenadePrefab,
            transform.position + Vector3.up * 8f + Vector3.forward * 3f,
            Quaternion.identity);

        Rigidbody grenadeRigidbody = grenadeObject.GetComponent<Rigidbody>();
        Vector3 toTarget = (currentTarget.transform.position - transform.position).normalized;
        Vector3 force = toTarget * 8f + Vector3.up * 7f;
        grenadeRigidbody.AddForce(force, ForceMode.Impulse);

        Granade grenadeScript = grenadeObject.GetComponent<Granade>();
        if (grenadeScript != null)
        {
            grenadeScript.targetPosition = currentTarget.transform.position;
        }

        // --- 스킬 종료 ---
        // 애니메이션이 끝날 때까지 또는 일정 시간 대기
        yield return new WaitForSeconds(1.0f); // 예: 1초 대기

        isAttacking = false;
        isSkill = false;

        // 안전하게 상태를 전환합니다.
        TransitionToState(PlayableState.Idle);
    }
}
//public class LudoPlayable : PlayableBase
//{
//    public GameObject grenadePrefab;

//    protected override void Skill()
//    {
//        if (currentTarget == null || currentTarget.isDead)
//            return;

//        // CharacterBase.ObjectType으로 타입을 비교합니다.
//        if (currentTarget.ObjectType == this.ObjectType)
//            return;

//        readySkill = false;
//        skillTimer = 0;

//        isAttacking = true;
//        isSkill = true;

//        GameObject grenadeObject = Instantiate(
//            grenadePrefab,
//            transform.position + Vector3.up * 8f + Vector3.forward * 3f,
//            Quaternion.identity);

//        Rigidbody grenadeRigidbody = grenadeObject.GetComponent<Rigidbody>();
//        Vector3 toTarget = (currentTarget.transform.position - transform.position).normalized;
//        Vector3 force = toTarget * 8f + Vector3.up * 7f;
//        grenadeRigidbody.AddForce(force, ForceMode.Impulse);

//        Granade grenadeScript = grenadeObject.GetComponent<Granade>();
//        if (grenadeScript != null)
//        {
//            grenadeScript.targetPosition = currentTarget.transform.position;
//        }

//        skillTimer = 0;
//        readySkill = false;
//        isSkill = false;
//        isAttacking = false;

//        TransitionToState(PlayableState.Idle);
//    }
//}