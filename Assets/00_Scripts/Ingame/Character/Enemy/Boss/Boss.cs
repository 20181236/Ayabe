using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Boss : EnemyBase
{
    public float skillDuration = 2f; // 미사일 생성 지속 시간
    public float missileSpawnDelay = 1f;   // 미사일 생성 간 딜레이
    float skillDurationTimer = 0f;

    public float skillCooldown = 5f; // 스킬 쿨타임
    private bool canUseSkill = true;  // 스킬 사용 가능 여부

    public float exSkillCooldown = 10f; // EX 스킬 쿨타임
    private bool canUseExSkill = true;  // EX 스킬 사용 가능 여부


    public GameObject skillMissile;
    public GameObject exSkillMissile;
    public Transform enemyMissileFirePoint;
    public Transform enemyExMissileFirePoint;

    public BossHpBar bossHpBar;

    protected override void Start()
    {
        base.Start();

        bossHpBar = BossHpBar.instance;
        if (bossHpBar == null)
        {
            Debug.LogError("BossHpBar 싱글톤 인스턴스가 씬에 존재하지 않습니다!");
            return;
        }

        bossHpBar.Show();
        bossHpBar.SetHP((int)CurrentHealth, (int)MaxHealth);
    }

    protected override void BuildBehaviorTree()
    {
        // BossBehaviorTree를 통해 트리를 설정
        var bt = GetComponent<BossBehaviorTree>();
        if (bt != null)
        {
            bt.BuildBehaviorTree();
        }
        else
        {
            Debug.LogError("[Boss] BossBehaviorTree가 붙어있지 않습니다!");
        }
    }

    public override void ApplyDamage(float damage, bool isExplosion, Vector3? explosionPos = null)
    {
        base.ApplyDamage(damage, isExplosion, explosionPos);

        if (bossHpBar != null)
            bossHpBar.SetHP(Mathf.FloorToInt(currentHealth), Mathf.FloorToInt(MaxHealth));
    }

    // BT에서 호출할 Skill 패턴
    public IEnumerator UseSkillBT()
    {
        if (!canUseSkill)
            yield break; // 쿨타임 중이면 스킬 무시

        Debug.Log($"[Boss] UseSkillBT 호출됨! readySkill={readySkill}, isUsingSkill={isUsingSkill}, time={Time.time}");

        isUsingSkill = true;
        readySkill = false;
        canUseSkill = false; // 사용 직후 불가 처리

        skillDurationTimer = 0f;
        yield return StartCoroutine(MissilesPattern());

        isUsingSkill = false;

        // 쿨타임 적용
        yield return new WaitForSeconds(skillCooldown);
        canUseSkill = true;
    }

    // BT에서 호출할 EX Skill 패턴
    public IEnumerator UseExSkillBT()
    {
        if (!canUseExSkill)
            yield break; // 쿨타임 중이면 발동하지 않음

        Debug.Log($"[Boss] UseExSkillBT 호출됨! readyExSkill={readyExSkill}, isUsingExSkill={isUsingExSkill}, time={Time.time}");

        isUsingExSkill = true;
        readyExSkill = false;
        canUseExSkill = false; // 사용 직후 불가 처리

        // 미사일 생성
        GameObject exMissileObject = Instantiate(exSkillMissile, enemyExMissileFirePoint.position, Quaternion.identity);


        // 필요시 연출 딜레이
        yield return new WaitForSeconds(0.5f);

        isUsingExSkill = false;

        // EX 스킬 쿨타임 적용
        yield return new WaitForSeconds(exSkillCooldown);
        canUseExSkill = true;
    }

    // 미사일 발사 패턴
    private IEnumerator MissilesPattern()
    {
        List<PlayableBase> playables = PlayableManager.instance.GetPlayables();

        Debug.Log($"[Boss] MissilesPattern 시작! skillDuration={skillDuration}, playables={playables.Count}");

        while (skillDurationTimer < skillDuration)
        {
            foreach (var target in playables)
            {
                if (target == null)
                {
                    Debug.LogWarning("[Boss] 타겟이 null이라 스킵합니다.");
                    continue;
                }

                Debug.Log($"[Boss] 미사일 생성 -> 타겟: {target.name}, firePoint={enemyMissileFirePoint?.name}");

                Vector3 direction = (target.transform.position - enemyMissileFirePoint.position).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                GameObject instantMissile = Instantiate(skillMissile, enemyMissileFirePoint.position, targetRotation);
                Debug.Log($"[Boss] 미사일 인스턴스 생성 완료: {instantMissile.name}");

                BossSkillMissile missileScript = instantMissile.GetComponent<BossSkillMissile>();
                missileScript.target = target.transform;
            }

            yield return new WaitForSeconds(missileSpawnDelay);
            skillDurationTimer += missileSpawnDelay;
        }
    }

    protected override void Die()
    {
        bossHpBar?.Hide();
        base.Die();
    }
}
