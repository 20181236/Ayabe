using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StageManager : MonoBehaviour
{
    public static StageManager instance { get; private set; }

    private InterfaceGameState currentState;

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
        SetState(new StageStartState(this));
    }

    private void Update()
    {
        currentState?.Update();
    }

    public void SetState(InterfaceGameState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    //private IEnumerator CheckWaveProgressCoroutine()
    //{
    //    while (!isStageClear)
    //    {
    //        // 모든 웨이브가 끝나지 않았고, 적이 없을 때 다음 웨이브 진행
    //        if (!WaveManager.instance.IsAllWaveSpawned() && !EnemyManager.instance.HasEnemy())
    //        {
    //            WaveManager.instance.StartWave();
    //        }
    //        // 모든 웨이브가 끝났고 적도 없고 보스도 끝났다면 스테이지 클리어 근데 처음에 소환되지도 않았는데 체크되면어떻게됨?
    //        else if (WaveManager.instance.IsAllWaveSpawned() &&
    //                     !EnemyManager.instance.HasEnemy() &&
    //                     !EnemyManager.instance.HasBoss())
    //        {
    //            isEnemyAllClear = true;
    //            isStageClear = true;    
    //            OnStageClear();
    //        }

    //        yield return new WaitForSeconds(1f);
    //    }
    //}

    private void OnStageClear()
    {
        Debug.Log("스테이지 클리어!");
        // 결과창 등 추가
    }
}

