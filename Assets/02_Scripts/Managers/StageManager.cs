using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StageManager : MonoBehaviour
{
    public static StageManager instance { get; private set; }

    public bool hasBoss = false;
    public bool isEnemyAllClear = false;
    public bool isBossClear = false;
    public bool isStageClear = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        WaveManager.instance.StartFirstWave();
        StartCoroutine(CheckWaveProgressCoroutine());
    }

    private IEnumerator CheckWaveProgressCoroutine()
    {
        while (!isStageClear)
        {
            // ��� ���̺갡 ������ �ʾҰ�, ���� ���� �� ���� ���̺� ����
            if (!WaveManager.instance.IsAllWaveSpawned() && !EnemyManager.instance.HasEnemy())
            {
                WaveManager.instance.StartWave();
            }
            // ��� ���̺갡 ������ ���� ���� ������ �����ٸ� �������� Ŭ����
            else if (WaveManager.instance.IsAllWaveSpawned() &&
                         !EnemyManager.instance.HasEnemy() &&
                         !EnemyManager.instance.HasBoss())
            {
                isEnemyAllClear = true;
                isStageClear = true;    
                OnStageClear();
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private void OnStageClear()
    {
        Debug.Log("�������� Ŭ����!");
        // ���â �� �߰�
    }
}

