using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameManager : MonoBehaviour
{
    public enum InGameState
    {
        Prepare,
        Battle,
        End
    }
    public InGameState inGameState;
    private float prepareTime = 1f; 

    private void Awake()
    {
        inGameState= InGameState.Prepare;
        EnemyManager.instance.SpawnEnemy(EnemyType.Normal, new Vector3(0, 0, 5));//이걸로 소환하면됨
    }
    private void Start()
    {
        StartCoroutine(StartGame());
    }
    private void Update()
    {
        if (inGameState != InGameState.Battle)
            return;
        if (!EnemyManager.instance.HasEnemyOfType(EnemyType.Boss))
        {
            EndGame();
            return;
        }
         if (!EnemyManager.instance.HasEnemy())
        {
            EndGame();
            return;
        }
    }
    public IEnumerator StartGame()
    {
        yield return new WaitForSeconds(prepareTime);
        inGameState = InGameState.Battle;
    }

    void EndGame()
    { 
        inGameState = InGameState.End;
        //여기에 게임 종료 뭔가 뭔가를 넣어야함
    }
}
