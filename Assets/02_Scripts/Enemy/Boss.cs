using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : EnemyBase
{
    public float missileSpawnRadius = 2f;    // �̻��� ���� ��ġ �ݰ�
    public float missileSpawnDelay = 0.1f;   // �̻��� ���� �� ������
    private bool isUsingSkill = false;
    public GameObject skillMissile;
    public GameObject exSkillMissile;
    public GameObject exSKillSubMissile;

    protected override void Skill()
    {
        if (!readySkill|| isUsingSkill)
            return;
        isUsingSkill = true;      // ��� �÷��� �ѱ�
        readySkill = false;       // ��ų ��� ���۰� ���ÿ� �غ� �÷��� ����
        StartCoroutine(MissilesPattern());
    }
    private IEnumerator MissilesPattern()
    {
        List<PlayableBase> playables = PlayableManager.instance.GetPlayables();
        int count = playables.Count;

        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPos = GetMissileSpawnPosition(i, count);
            Instantiate(skillMissile, spawnPos, Quaternion.identity);
            yield return new WaitForSeconds(missileSpawnDelay);
        }
        isUsingSkill = false;  // ��ų ��� ��
    }
    private Vector3 GetMissileSpawnPosition(int index, int total)
    {
        float angle = (360f / total) * index;
        float rad = angle * Mathf.Deg2Rad;

        Vector3 offset = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * missileSpawnRadius;
        return transform.position + offset;
    }
    protected override void ExSkill()
    {
        //    if (!readyExSkill)
        //        return;
        //    var targets = PlayableMnager.instance.playables.FindAll(t => t != null && !t.isDead);
        //    if (targets.Count == 0)
        //    {
        //        return;
        //    }
        //    var selectedTarget = targets[UnityEngine.Random.Range(0, targets.Count)];
        //    if (selectedTarget == null)
        //    {
        //        return;
        //    }
        //    Vector3 direction = (selectedTarget.transform.position - transform.position).normalized;
        //    Vector3 spawnPos = transform.position + Vector3.up * 10f + direction * 5f;
        //    if (exMissile == null)
        //    {
        //        return;
        //    }
        //    GameObject missileObj = Instantiate(
        //        exMissile, 
        //        spawnPos, 
        //        Quaternion.LookRotation(direction));
        //    exSkillTimer = 0;
        //    readyExSkill = false;
        //    var exMissileScript = missileObj.GetComponent<BossExMissile>();
        //    if (exMissileScript == null)
        //    {
        //        return;
        //    }
        //    exMissileScript.Init(selectedTarget.transform.position);

    }
}
