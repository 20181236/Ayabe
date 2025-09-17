using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class Boss : EnemyBase
{
    public float skillDuration = 2f; // 미사일 생성 지속 시간

    public float missileSpawnDelay = 1f;   // 미사일 생성 간 딜레이

    float skillDurationTimer = 0f;

    public GameObject skillMissile;
    public GameObject exSkillMissile;
    public Transform enemyMissileFirePoint;
    public Transform enemyExMissileFirePoint;

    //public BossHpBar bossHpBar; // 연결할 체력바 UI
    public BossHpBar bossHpBar; // private으로 참조를 저장할 변수

    protected override void Start()
    {
        base.Start();

        // 1. FindObjectOfType 대신 싱글톤 인스턴스를 바로 가져옴
        bossHpBar = BossHpBar.instance;

        // 2. null 체크 (혹시 모를 상황 대비)
        if (bossHpBar == null)
        {
            Debug.LogError("BossHpBar 싱글톤 인스턴스가 씬에 존재하지 않습니다!");
            return;
        }

        // 3. HP바 활성화 및 설정
        bossHpBar.Show();
        bossHpBar.SetHP((int)CurrentHealth, (int)MaxHealth);
    }

    protected override void Update()
    {
        //if (bossHpBar != null)
        //{
        //    bossHpBar.SetHP((int)GetCurrentHealth(), (int)GetMaxHealth());
        //}
    }
    public override void ApplyDamage(float damage, bool isExplosion, Vector3? explosionPos = null)
    {
        base.ApplyDamage(damage, isExplosion, explosionPos);

        // base에서 currentHealth가 줄어든 이후에 반영
        if (bossHpBar != null)
        {
            bossHpBar.SetHP(Mathf.FloorToInt(currentHealth), Mathf.FloorToInt(MaxHealth));
        }
    }

    protected override void Skill()
    {
        if (!readySkill|| isUsingSkill)
            return;
        isUsingSkill = true;
        readySkill = false;
        skillDurationTimer = 0f;
        StartCoroutine(MissilesPattern());
    }
    private IEnumerator MissilesPattern()
    {

        List<PlayableBase> playables = PlayableManager.instance.GetPlayables();
        int count = playables.Count;

        while (skillDurationTimer < skillDuration)
        {
            foreach (var target in playables)
            {
                if (target == null) continue;

                Vector3 direction = (target.transform.position - enemyMissileFirePoint.position).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                GameObject instantMissile = Instantiate(skillMissile, enemyMissileFirePoint.position, targetRotation);
                BossSkillMissile missileScript = instantMissile.GetComponent<BossSkillMissile>();
                missileScript.target = target.transform;
            }

            yield return new WaitForSeconds(missileSpawnDelay);
            skillDurationTimer += missileSpawnDelay;
        }
        isUsingSkill = false;
        skillTimer = 0f;
        readySkill = false;     
    }   
    protected override void ExSkill()
    {
        if (!readySkill || isUsingExSkill)
            return;
        isUsingExSkill = true;
        GameObject exMissileObject = Instantiate(exSkillMissile, enemyExMissileFirePoint.position, Quaternion.identity);
        exSkillTimer = 0f;
        skillDurationTimer = 0f;
        isUsingExSkill = false;
        readyExSkill = false;
    }
    protected override void Die()
    {
        bossHpBar?.Hide();
        base.Die();
    }

    //protected override void Die()
    //{
    //    if (bossHpBar != null)
    //    {
    //        bossHpBar.Hide();
    //    }
    //    StageManager.instance.OnBossDefeated(); // ★ 여기서 처리

    //    base.Die();
    //}
}
